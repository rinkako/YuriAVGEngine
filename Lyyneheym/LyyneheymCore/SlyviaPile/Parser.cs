using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    using iHandle = Func<SyntaxTreeNode, CFunctionType, SyntaxType, Token, SyntaxTreeNode>;
    /// <summary>
    /// 语法匹配器类：负责把单词流匹配成语法树的类
    /// </summary>
    internal sealed class Parser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Parser()
        {
            this.Reset();
            this.Init();
            this.iBlockStack = new Stack<SyntaxTreeNode>();
        }

        /// <summary>
        /// 为语法匹配器重新设置单词流
        /// </summary>
        /// <param name="myStream">单词流向量</param>
        public void SetTokenStream(List<Token> myStream)
        {
            this.istream = myStream;
        }

        /// <summary>
        /// 清空语法块嵌套匹配栈
        /// </summary>
        public void ClearBlockStack()
        {
            this.iBlockStack.Clear();
        }

        /// <summary>
        /// 复位匹配器
        /// </summary>
        public void Reset()
        {
            this.nextTokenPointer = 0;
            this.dashReset(SyntaxType.case_kotori);
        }

        /// <summary>
        /// 启动语法分析器
        /// </summary>
        /// <param name="line">处理的行号</param>
        /// <returns>匹配完毕的语法树</returns>
        public SyntaxTreeNode Parse(int line)
        {
            // 初期化
            this.Reset();
            // 获得原始节点的引用
            SyntaxTreeNode parsePtr;
            SyntaxTreeNode currentPtr;
            // 如果语句块嵌套栈顶端是可以推出语句块的类型就直接把她作为双亲节点
            if (this.iBlockStack.Peek().nodeSyntaxType == SyntaxType.case_kotori)
            {
                parsePtr = currentPtr = this.iBlockStack.Peek();
            }
            // 否则处理这个节点的语句块的从属关系
            else
            {
                parsePtr = new SyntaxTreeNode(SyntaxType.case_kotori);
                currentPtr = parsePtr;
                currentPtr.parent = this.iBlockStack.Peek();
                currentPtr.children = new List<SyntaxTreeNode>();
                if (this.iBlockStack.Peek().children == null)
                {
                    this.iBlockStack.Peek().children = new List<SyntaxTreeNode>();
                }
                this.iBlockStack.Peek().children.Add(currentPtr);
            }
            // 自顶向下，语法分析
            bool fromKaguya = false;
            while (this.iParseStack.Count != 0 || this.iQueue.Count != 0)
            {
                // 语句根节点，就调用不推导函数去处理，随后立即迭代掉本次循环
                if (this.iParseStack.Count > 0 && this.iParseStack.Peek() == SyntaxType.case_kotori)
                {
                    SyntaxTreeNode stn = this.Kaguya();
                    stn.line = line;
                    // 如果这个节点是if、for、function子句，那就要为她增加一个kotori节点
                    if (stn.nodeSyntaxType == SyntaxType.synr_if ||
                        stn.nodeSyntaxType == SyntaxType.synr_for ||
                        stn.nodeSyntaxType == SyntaxType.synr_function)
                    {
                        // 把原来的子句节点加到原来的匹配树上
                        stn.parent = currentPtr;
                        if (currentPtr.children == null)
                        {
                            currentPtr.children = new List<SyntaxTreeNode>();
                        }
                        currentPtr.children.Add(stn);
                        currentPtr = stn;
                        currentPtr.line = line;
                        // 为子句节点加上一个真分支
                        currentPtr.children = new List<SyntaxTreeNode>();
                        SyntaxTreeNode trueBranch = new SyntaxTreeNode(SyntaxType.case_kotori);
                        trueBranch.line = line;
                        trueBranch.children = new List<SyntaxTreeNode>();
                        trueBranch.nodeName = trueBranch.nodeSyntaxType.ToString() + 
                            (stn.nodeSyntaxType == SyntaxType.synr_if ? "_trueBranch" :
                            stn.nodeSyntaxType == SyntaxType.synr_for ? "_forBranch" : "_funDeclaration");
                        trueBranch.parent = currentPtr;
                        stn.children.Add(trueBranch);
                        // 追加到语句块栈
                        iBlockStack.Push(trueBranch);
                        currentPtr = trueBranch;
                    }
                    // 如果这个节点是else子句，那就直接用kotori节点换掉她
                    else if (stn.nodeSyntaxType == SyntaxType.synr_else)
                    {
                        currentPtr = currentPtr.parent;
                        SyntaxTreeNode falseBranch = new SyntaxTreeNode(SyntaxType.case_kotori);
                        falseBranch.line = line;
                        falseBranch.children = new List<SyntaxTreeNode>();
                        falseBranch.nodeName = falseBranch.nodeSyntaxType.ToString() + "_falseBranch";
                        falseBranch.parent = currentPtr;
                        currentPtr.children.Add(falseBranch);
                        // 追加到语句块栈
                        iBlockStack.Push(falseBranch);
                        currentPtr = falseBranch;
                    }
                    // 如果这个节点是endif子句，那就直接用她换掉kotori节点
                    else if (stn.nodeSyntaxType == SyntaxType.synr_endif)
                    {
                        SyntaxTreeNode originTop = parsePtr.parent;
                        originTop.children.Add(stn);
                        stn.parent = originTop;
                        currentPtr = parsePtr = stn;
                    }
                    // 其余情况就把该节点作为当前展开节点的孩子节点
                    else
                    {
                        stn.parent = currentPtr;
                        if (currentPtr.children == null)
                        {
                            currentPtr.children = new List<SyntaxTreeNode>();
                        }
                        currentPtr.children.Add(stn);
                        currentPtr = stn;
                    }
                    fromKaguya = true;
                    continue;
                }
                // 如果栈中只有startend，队列中没有元素，那就意味着不需要继续推导了
                if (this.iParseStack.Count > 0 && this.iParseStack.Peek() == SyntaxType.tail_startEndLeave
                    && this.iQueue.Count == 0)
                {
                    break;
                }
                // 如果栈中不空，说明在做LL1文法的推导
                if (this.iParseStack.Count != 0 && fromKaguya == false)
                {
                    // 查预测表，获得产生式处理函数
                    SyntaxType nodeType = this.iParseStack.Peek();
                    Token iToken = this.istream[this.nextTokenPointer];
                    TokenType tokenType = iToken.aType;
                    CandidateFunction func = this.iMap.GetCFunciton(nodeType, tokenType, this.Homura);
                    // 语法出错时
                    if (func.GetCFType() == CFunctionType.umi_errorEnd)
                    {
                        Console.WriteLine("语法匹配出错");
                        return null;
                    }
                    // 如果处于非终结符，就设置她的候选式
                    if (currentPtr != null)
                    {
                        currentPtr.candidateFunction = func;
                    }
                    // 调用产生式，下降
                    if (func != null)
                    {
                        if (currentPtr != null)
                        {
                            currentPtr = currentPtr.candidateFunction.Call(currentPtr, nodeType, iToken);
                        }
                        else
                        {
                            currentPtr = this.Homura(currentPtr, func.GetCFType(), nodeType, iToken);
                        }
                        if (currentPtr != null)
                        {
                            currentPtr.line = line;
                        }
                    }
                    // 没有对应的候选式时
                    else
                    {
                        if (currentPtr != null)
                        {
                            currentPtr.errorBit = true;
                        }
                        break;
                    }
                }
                // 最后看不推导队列
                else if (this.iQueue.Count != 0)
                {
                    SyntaxTreeNode reroot = this.iQueue.Dequeue();
                    // 析取范式时
                    if (reroot.nodeSyntaxType == SyntaxType.case_disjunct)
                    {
                        this.iParseStack.Push(SyntaxType.case_disjunct);
                    }
                    // 值表达式时
                    else
                    {
                        this.iParseStack.Push(reroot.nodeSyntaxType);
                    }
                    // 把token流改为私有的token流
                    this.istream = reroot.paramTokenStream;
                    // 在流的末尾，放置结束标记
                    Token ccToken = new Token();
                    ccToken.length = 1;
                    ccToken.detail = "#";
                    ccToken.errorBit = false;
                    ccToken.aType = TokenType.startend;
                    this.istream.Add(ccToken);
                    // 复位游程
                    this.nextTokenPointer = 0;
                    // 修改当前指针
                    currentPtr = reroot;
                }
                fromKaguya = false;
            }
            return parsePtr;
        }

        /// <summary>
        /// 初始化预测分析表、链接向量和LL1文法预测表
        /// </summary>
        private void Init()
        {
            // 初始化链接向量
            for (int i = 0; i < 64; i++)
            {
                this.iDict.Add(new List<SyntaxType>());
            }
            // <disjunct> -> <conjunct> <disjunct_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct__conjunct__disjunct_pi_35)].Add(SyntaxType.case_conjunct);
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct__conjunct__disjunct_pi_35)].Add(SyntaxType.case_disjunct_pi);
            // <disjunct_pi> -> "||" <conjunct> <disjunct_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36)].Add(SyntaxType.tail_or_Or_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36)].Add(SyntaxType.case_conjunct);
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36)].Add(SyntaxType.case_disjunct_pi);
            // <disjunct_pi> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___disjunct_pi__epsilon_37)].Add(SyntaxType.epsilonLeave);
            // <conjunct> -> <bool> <conjunct_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct__bool__conjunct_pi_38)].Add(SyntaxType.case_bool);
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct__bool__conjunct_pi_38)].Add(SyntaxType.case_conjunct_pi);
            // <conjunct_pi> -> "&&" <bool> <conjunct_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39)].Add(SyntaxType.tail_and_And_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39)].Add(SyntaxType.case_bool);
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39)].Add(SyntaxType.case_conjunct_pi);
            // <conjunct_pi> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___conjunct_pi__epsilon_40)].Add(SyntaxType.epsilonLeave);
            // <bool> -> "!" <bool>
            this.iDict[Convert.ToInt32(CFunctionType.deri___bool__not_bool_42)].Add(SyntaxType.tail_not_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___bool__not_bool_42)].Add(SyntaxType.case_bool);
            // <bool> -> <comp>
            this.iDict[Convert.ToInt32(CFunctionType.deri___bool__comp_43)].Add(SyntaxType.case_comp);
            // <comp> -> <wexpr> <rop> <wexpr>
            this.iDict[Convert.ToInt32(CFunctionType.deri___comp__wexpr__rop__wexpr_44)].Add(SyntaxType.case_wexpr);
            this.iDict[Convert.ToInt32(CFunctionType.deri___comp__wexpr__rop__wexpr_44)].Add(SyntaxType.case_rop);
            this.iDict[Convert.ToInt32(CFunctionType.deri___comp__wexpr__rop__wexpr_44)].Add(SyntaxType.case_wexpr);
            // <rop> -> "<>"
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__lessgreater_58)].Add(SyntaxType.tail_lessThan_GreaterThan_Leave);
            // <rop> -> "=="
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__equalequal_59)].Add(SyntaxType.tail_equality_Equality_Leave);
            // <rop> -> ">"
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__greater_60)].Add(SyntaxType.tail_greaterThan_Leave);
            // <rop> -> "<"
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__less_61)].Add(SyntaxType.tail_lessThan_Leave);
            // <rop> -> ">="
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__greaterequal_62)].Add(SyntaxType.tail_greaterThan_Equality_Leave);
            // <rop> -> "<="
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__lessequal_63)].Add(SyntaxType.tail_lessThan_Equality_Leave);
            // <rop> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___rop__epsilon_80)].Add(SyntaxType.epsilonLeave);
            // <wexpr> -> <wmulti> <wexpr_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr__wmulti__wexpr_pi_45)].Add(SyntaxType.case_wmulti);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr__wmulti__wexpr_pi_45)].Add(SyntaxType.case_wexpr_pi);
            // <wexpr> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr__epsilon_81)].Add(SyntaxType.epsilonLeave);
            // <wplus> -> "+" <wmulti>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wplus__plus_wmulti_46)].Add(SyntaxType.tail_plus_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wplus__plus_wmulti_46)].Add(SyntaxType.case_wmulti);
            // <wplus> -> "-" <wmulti>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wplus__minus_wmulti_47)].Add(SyntaxType.tail_minus_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wplus__minus_wmulti_47)].Add(SyntaxType.case_wmulti);
            // <wexpr_pi> -> <wplus> <wexpr_pi>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72)].Add(SyntaxType.case_wplus);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72)].Add(SyntaxType.case_wexpr_pi);
            // <wexpr_pi> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___wexpr_pi__epsilon_73)].Add(SyntaxType.epsilonLeave);
            // <wmulti> -> <wunit> <wmultiOpt>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmulti__wunit__wmultiOpt_49)].Add(SyntaxType.case_wunit);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmulti__wunit__wmultiOpt_49)].Add(SyntaxType.case_wmultiOpt);
            // <wmultiOpt> -> "*" <wunit>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50)].Add(SyntaxType.tail_multiply_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50)].Add(SyntaxType.case_wunit);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50)].Add(SyntaxType.case_wmultiOpt);
            // <wmultiOpt> -> "/" <wunit>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51)].Add(SyntaxType.tail_divide_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51)].Add(SyntaxType.case_wunit);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51)].Add(SyntaxType.case_wmultiOpt);
            // <wmultiOpt> -> epsilon
            this.iDict[Convert.ToInt32(CFunctionType.deri___wmultiOpt__epsilon_52)].Add(SyntaxType.epsilonLeave);
            // <wunit> -> number
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__number_53)].Add(SyntaxType.numberLeave);
            // <wunit> -> iden
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__iden_54)].Add(SyntaxType.tail_idenLeave);
            // <wunit> -> "-" <wunit>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__minus_wunit_55)].Add(SyntaxType.tail_minus_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__minus_wunit_55)].Add(SyntaxType.case_wunit);
            // <wunit> -> "+" <wunit>
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__plus_wunit_56)].Add(SyntaxType.tail_plus_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__plus_wunit_56)].Add(SyntaxType.case_wunit);
            // <wunit> -> "(" <disjunct> ")"
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__brucket_disjunct_57)].Add(SyntaxType.tail_leftParentheses_Leave);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__brucket_disjunct_57)].Add(SyntaxType.case_disjunct);
            this.iDict[Convert.ToInt32(CFunctionType.deri___wunit__brucket_disjunct_57)].Add(SyntaxType.tail_rightParentheses_Leave);

            // 初始化预测分析表的表头
            // 设置行属性：非终结符
            iMap.SetRow(0, SyntaxType.case_disjunct);
            iMap.SetRow(1, SyntaxType.case_disjunct_pi);
            iMap.SetRow(2, SyntaxType.case_conjunct);
            iMap.SetRow(3, SyntaxType.case_conjunct_pi);
            iMap.SetRow(4, SyntaxType.case_bool);
            iMap.SetRow(5, SyntaxType.case_comp);
            iMap.SetRow(6, SyntaxType.case_rop);
            iMap.SetRow(7, SyntaxType.case_wexpr);
            iMap.SetRow(8, SyntaxType.case_wexpr_pi);
            iMap.SetRow(9, SyntaxType.case_wplus);
            iMap.SetRow(10, SyntaxType.case_wmulti);
            iMap.SetRow(11, SyntaxType.case_wmultiOpt);
            iMap.SetRow(12, SyntaxType.case_wunit);
            // 设置行属性：终结符
            iMap.SetRow(13, SyntaxType.tail_idenLeave);
            iMap.SetRow(14, SyntaxType.tail_leftParentheses_Leave);
            iMap.SetRow(15, SyntaxType.tail_rightParentheses_Leave);
            iMap.SetRow(16, SyntaxType.epsilonLeave);
            iMap.SetRow(17, SyntaxType.tail_equality_Leave);
            iMap.SetRow(18, SyntaxType.tail_plus_Leave);
            iMap.SetRow(19, SyntaxType.tail_minus_Leave);
            iMap.SetRow(20, SyntaxType.tail_multiply_Leave);
            iMap.SetRow(21, SyntaxType.tail_divide_Leave);
            iMap.SetRow(22, SyntaxType.numberLeave);
            iMap.SetRow(23, SyntaxType.tail_or_Or_Leave);
            iMap.SetRow(24, SyntaxType.tail_and_And_Leave);
            iMap.SetRow(25, SyntaxType.tail_not_Leave);
            iMap.SetRow(26, SyntaxType.tail_lessThan_GreaterThan_Leave);
            iMap.SetRow(27, SyntaxType.tail_equality_Equality_Leave);
            iMap.SetRow(28, SyntaxType.tail_greaterThan_Leave);
            iMap.SetRow(29, SyntaxType.tail_lessThan_Leave);
            iMap.SetRow(30, SyntaxType.tail_greaterThan_Equality_Leave);
            iMap.SetRow(31, SyntaxType.tail_lessThan_Equality_Leave);
            iMap.SetRow(32, SyntaxType.tail_startEndLeave);
            // 设置列属性：向前看的一个token
            iMap.SetCol(0, TokenType.identifier);
            iMap.SetCol(1, TokenType.Token_LeftParentheses);
            iMap.SetCol(2, TokenType.Token_RightParentheses);
            iMap.SetCol(3, TokenType.Token_Equality);
            iMap.SetCol(4, TokenType.Token_Plus);
            iMap.SetCol(5, TokenType.Token_Minus);
            iMap.SetCol(6, TokenType.Token_Multiply);
            iMap.SetCol(7, TokenType.Token_Divide);
            iMap.SetCol(8, TokenType.number);
            iMap.SetCol(9, TokenType.Token_Or_Or);
            iMap.SetCol(10, TokenType.Token_And_And);
            iMap.SetCol(11, TokenType.Token_Not);
            iMap.SetCol(12, TokenType.Token_LessThan_GreaterThan);
            iMap.SetCol(13, TokenType.Token_Equality_Equality);
            iMap.SetCol(14, TokenType.Token_GreaterThan);
            iMap.SetCol(15, TokenType.Token_LessThan);
            iMap.SetCol(16, TokenType.Token_GreaterThan_Equality);
            iMap.SetCol(17, TokenType.Token_LessThan_Equality);
            iMap.SetCol(18, TokenType.startend);

            // 初始化LL1-上下文无关文法
            // iProco都指向通用展开式函数Homura
            iHandle iProco = this.Homura;
            // 错误的情况下，没有考虑短语层次的错误恢复，因此错误处理器都指向null
            for (int i = 0; i < LL1PARSERMAPROW; i++)
            {
                for (int j = 0; j < LL1PARSERMAPCOL; j++)
                {
                    this.iMap.SetCellular(i, j, new CandidateFunction(null, CFunctionType.umi_errorEnd));
                }
            }
            // 流命中： <disjunct> ::= iden的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct> ::= "("的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct> ::= "+"的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct> ::= "-"的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct> ::= number的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct> ::= "!"的Token
            // 语句命中： <disjunct> ::= <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct, TokenType.Token_Not,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct__conjunct__disjunct_pi_35));
            // 流命中： <disjunct_pi> ::= ")"的Token
            // 语句命中： <disjunct_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_disjunct_pi, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct_pi__epsilon_37));
            // 流命中： <disjunct_pi> ::= startend的Token
            // 语句命中： <disjunct_pi> ::= null  #change#
            this.iMap.SetCellular(SyntaxType.case_disjunct_pi, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct_pi__epsilon_37));
            // 流命中： <disjunct_pi> ::= "||"的Token
            // 语句命中： <disjunct_pi> ::= "||" <conjunct> <disjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_disjunct_pi, TokenType.Token_Or_Or,
              new CandidateFunction(iProco, CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36));
            // 流命中： <conjunct> ::= iden的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct> ::= "("的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct> ::= "+"的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct> ::= "-"的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct> ::= number的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct> ::= "!"的Token
            // 语句命中： <conjunct> ::= <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct, TokenType.Token_Not,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct__bool__conjunct_pi_38));
            // 流命中： <conjunct_pi> ::= ")"的Token
            // 语句命中： <conjunct_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_conjunct_pi, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct_pi__epsilon_40));
            // 流命中： <conjunct_pi> ::= startend的Token
            // 语句命中： <conjunct_pi> ::= null #change#
            this.iMap.SetCellular(SyntaxType.case_conjunct_pi, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct_pi__epsilon_40));
            // 流命中： <conjunct_pi> ::= "||"的Token
            // 语句命中： <conjunct_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_conjunct_pi, TokenType.Token_Or_Or,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct_pi__epsilon_40));
            // 流命中： <conjunct_pi> ::= "&&"的Token
            // 语句命中： <conjunct_pi> ::= "&&" <bool> <conjunct_pi>
            this.iMap.SetCellular(SyntaxType.case_conjunct_pi, TokenType.Token_And_And,
              new CandidateFunction(iProco, CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39));
            // 流命中： <bool> ::= iden的Token
            // 语句命中： <bool> ::= <comp>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___bool__comp_43));
            // 流命中： <bool> ::= "("的Token
            // 语句命中： <bool> ::= <comp>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___bool__comp_43));
            // 流命中： <bool> ::= "+"的Token
            // 语句命中： <bool> ::= <comp>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___bool__comp_43));
            // 流命中： <bool> ::= "-"的Token
            // 语句命中： <bool> ::= <comp>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___bool__comp_43));
            // 流命中： <bool> ::= number的Token
            // 语句命中： <bool> ::= <comp>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___bool__comp_43));
            // 流命中： <bool> ::= "!"的Token
            // 语句命中： <bool> ::= "!" <bool>
            this.iMap.SetCellular(SyntaxType.case_bool, TokenType.Token_Not,
              new CandidateFunction(iProco, CFunctionType.deri___bool__not_bool_42));
            // 流命中： <comp> ::= iden的Token
            // 语句命中： <comp> ::= <wexpr> <rop> <wexpr>
            this.iMap.SetCellular(SyntaxType.case_comp, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___comp__wexpr__rop__wexpr_44));
            // 流命中： <comp> ::= "("的Token
            // 语句命中： <comp> ::= <wexpr> <rop> <wexpr>
            this.iMap.SetCellular(SyntaxType.case_comp, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___comp__wexpr__rop__wexpr_44));
            // 流命中： <comp> ::= "+"的Token
            // 语句命中： <comp> ::= <wexpr> <rop> <wexpr>
            this.iMap.SetCellular(SyntaxType.case_comp, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___comp__wexpr__rop__wexpr_44));
            // 流命中： <comp> ::= "-"的Token
            // 语句命中： <comp> ::= <wexpr> <rop> <wexpr>
            this.iMap.SetCellular(SyntaxType.case_comp, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___comp__wexpr__rop__wexpr_44));
            // 流命中： <comp> ::= number的Token
            // 语句命中： <comp> ::= <wexpr> <rop> <wexpr>
            this.iMap.SetCellular(SyntaxType.case_comp, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___comp__wexpr__rop__wexpr_44));
            // 流命中： <rop> ::= "<>"的Token
            // 语句命中： <rop> ::= "<>"
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_LessThan_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___rop__lessgreater_58));
            // 流命中： <rop> ::= "=="的Token
            // 语句命中： <rop> ::= "=="
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_Equality_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___rop__equalequal_59));
            // 流命中： <rop> ::= ">"的Token
            // 语句命中： <rop> ::= ">"
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___rop__greater_60));
            // 流命中： <rop> ::= "<"的Token
            // 语句命中： <rop> ::= "<"
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_LessThan,
              new CandidateFunction(iProco, CFunctionType.deri___rop__less_61));
            // 流命中： <rop> ::= ">="的Token
            // 语句命中： <rop> ::= ">="
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_GreaterThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___rop__greaterequal_62));
            // 流命中： <rop> ::= "<="的Token
            // 语句命中： <rop> ::= "<="
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_LessThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___rop__lessequal_63));
            // 流命中： <rop> ::= startend的Token
            // 语句命中： <rop> ::= epsilon  #change#
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___rop__epsilon_80));
            // 流命中： <rop> ::= "("的Token
            // 语句命中： <rop> ::= epsilon
            this.iMap.SetCellular(SyntaxType.case_rop, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___rop__epsilon_80));
            // 流命中： <wexpr> ::= iden的Token
            // 语句命中： <wexpr> ::= <wmulti> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__wmulti__wexpr_pi_45));
            // 流命中： <wexpr> ::= startend的Token
            // 语句命中： <wexpr> ::= epsilon   #change#
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__epsilon_81));
            // 流命中： <wexpr> ::= ")"的Token
            // 语句命中： <wexpr> ::= epsilon
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__epsilon_81));
            // 流命中： <wexpr> ::= "("的Token
            // 语句命中： <wexpr> ::= <wmulti> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__wmulti__wexpr_pi_45));
            // 流命中： <wexpr> ::= "+"的Token
            // 语句命中： <wexpr> ::= <wmulti> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__wmulti__wexpr_pi_45));
            // 流命中： <wexpr> ::= "-"的Token
            // 语句命中： <wexpr> ::= <wmulti> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__wmulti__wexpr_pi_45));
            // 流命中： <wexpr> ::= number的Token
            // 语句命中： <wexpr> ::= <wmulti> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr__wmulti__wexpr_pi_45));
            // 流命中： <wexpr_pi> ::= ")"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= startend的Token
            // 语句命中： <wexpr_pi> ::= null   #change#
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= ","的Token
            // 语句命中： <wexpr_pi> ::= null
            //this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Comma_,
            //  new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "+"的Token
            // 语句命中： <wexpr_pi> ::= <wplus> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72));
            // 流命中： <wexpr_pi> ::= "-"的Token
            // 语句命中： <wexpr_pi> ::= <wplus> <wexpr_pi>
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72));
            // 流命中： <wexpr_pi> ::= "||"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Or_Or,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "&&"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_And_And,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "<>"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_LessThan_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "=="的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Equality_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= ">"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "<"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_LessThan,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= ">="的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_GreaterThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= "<="的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_LessThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wexpr_pi> ::= ")"的Token
            // 语句命中： <wexpr_pi> ::= null
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wplus> ::= startend的Token
            // 语句命中： <wplus> ::= null  #change#
            this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wplus> ::= ","的Token
            // 语句命中： <wplus> ::= null
            //this.iMap.SetCellular(SyntaxType.case_wexpr_pi, TokenType.Token_Comma_,
            //  new CandidateFunction(iProco, CFunctionType.deri___wexpr_pi__epsilon_73));
            // 流命中： <wplus> ::= "+"的Token
            // 语句命中： <wplus> ::= "+" <wmulti>
            this.iMap.SetCellular(SyntaxType.case_wplus, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wplus__plus_wmulti_46));
            // 流命中： <wplus> ::= "-"的Token
            // 语句命中： <wplus> ::= "-" <wmulti>
            this.iMap.SetCellular(SyntaxType.case_wplus, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wplus__minus_wmulti_47));
            // 流命中： <wmulti> ::= iden的Token
            // 语句命中： <wmulti> ::= <wunit> <wmultiOpt>
            this.iMap.SetCellular(SyntaxType.case_wmulti, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___wmulti__wunit__wmultiOpt_49));
            // 流命中： <wmulti> ::= "("的Token
            // 语句命中： <wmulti> ::= <wunit> <wmultiOpt>
            this.iMap.SetCellular(SyntaxType.case_wmulti, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wmulti__wunit__wmultiOpt_49));
            // 流命中： <wmulti> ::= "+"的Token
            // 语句命中： <wmulti> ::= <wunit> <wmultiOpt>
            this.iMap.SetCellular(SyntaxType.case_wmulti, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wmulti__wunit__wmultiOpt_49));
            // 流命中： <wmulti> ::= "-"的Token
            // 语句命中： <wmulti> ::= <wunit> <wmultiOpt>
            this.iMap.SetCellular(SyntaxType.case_wmulti, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wmulti__wunit__wmultiOpt_49));
            // 流命中： <wmulti> ::= number的Token
            // 语句命中： <wmulti> ::= <wunit> <wmultiOpt>
            this.iMap.SetCellular(SyntaxType.case_wmulti, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___wmulti__wunit__wmultiOpt_49));
            // 流命中： <wmultiOpt> ::= ")"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= startend的Token
            // 语句命中： <wmultiOpt> ::= null   #change#
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= ","的Token
            // 语句命中： <wmultiOpt> ::= null
            //this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Comma,
            //  new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "+"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "-"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "*"的Token
            // 语句命中： <wmultiOpt> ::= "*" <wunit>
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Multiply,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50));
            // 流命中： <wmultiOpt> ::= "/"的Token
            // 语句命中： <wmultiOpt> ::= "/" <wunit>
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Divide,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51));
            // 流命中： <wmultiOpt> ::= "||"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Or_Or,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "&&"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_And_And,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "<>"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_LessThan_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "=="的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_Equality_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= ">"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "<"的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_LessThan,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= ">="的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_GreaterThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wmultiOpt> ::= "<="的Token
            // 语句命中： <wmultiOpt> ::= null
            this.iMap.SetCellular(SyntaxType.case_wmultiOpt, TokenType.Token_LessThan_Equality,
              new CandidateFunction(iProco, CFunctionType.deri___wmultiOpt__epsilon_52));
            // 流命中： <wunit> ::= iden的Token
            // 语句命中： <wunit> ::= iden
            this.iMap.SetCellular(SyntaxType.case_wunit, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.deri___wunit__iden_54));
            // 流命中： <wunit> ::= "("的Token
            // 语句命中： <wunit> ::= "(" <disjunct> ")"
            this.iMap.SetCellular(SyntaxType.case_wunit, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.deri___wunit__brucket_disjunct_57));
            // 流命中： <wunit> ::= "+"的Token
            // 语句命中： <wunit> ::= "+" <wunit>
            this.iMap.SetCellular(SyntaxType.case_wunit, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.deri___wunit__plus_wunit_56));
            // 流命中： <wunit> ::= "-"的Token
            // 语句命中： <wunit> ::= "-" <wunit>
            this.iMap.SetCellular(SyntaxType.case_wunit, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.deri___wunit__minus_wunit_55));
            // 流命中： <wunit> ::= number的Token
            // 语句命中： <wunit> ::= number
            this.iMap.SetCellular(SyntaxType.case_wunit, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.deri___wunit__number_53));
            // 叶命中： iden 的Token
            this.iMap.SetCellular(SyntaxType.tail_idenLeave, TokenType.identifier,
              new CandidateFunction(iProco, CFunctionType.umi_iden));
            // 叶命中： "(" 的Token
            this.iMap.SetCellular(SyntaxType.tail_leftParentheses_Leave, TokenType.Token_LeftParentheses,
              new CandidateFunction(iProco, CFunctionType.umi_leftParentheses_));
            // 叶命中： ")" 的Token
            this.iMap.SetCellular(SyntaxType.tail_rightParentheses_Leave, TokenType.Token_RightParentheses,
              new CandidateFunction(iProco, CFunctionType.umi_rightParentheses_));
            // 叶命中： "=" 的Token
            this.iMap.SetCellular(SyntaxType.tail_equality_Leave, TokenType.Token_Equality,
              new CandidateFunction(iProco, CFunctionType.umi_equality_));
            // 叶命中： "+" 的Token
            this.iMap.SetCellular(SyntaxType.tail_plus_Leave, TokenType.Token_Plus,
              new CandidateFunction(iProco, CFunctionType.umi_plus_));
            // 叶命中： "-" 的Token
            this.iMap.SetCellular(SyntaxType.tail_minus_Leave, TokenType.Token_Minus,
              new CandidateFunction(iProco, CFunctionType.umi_minus_));
            // 叶命中： "*" 的Token
            this.iMap.SetCellular(SyntaxType.tail_multiply_Leave, TokenType.Token_Multiply,
              new CandidateFunction(iProco, CFunctionType.umi_multiply_));
            // 叶命中： "/" 的Token
            this.iMap.SetCellular(SyntaxType.tail_divide_Leave, TokenType.Token_Divide,
              new CandidateFunction(iProco, CFunctionType.umi_divide_));
            // 叶命中： number 的Token
            this.iMap.SetCellular(SyntaxType.numberLeave, TokenType.number,
              new CandidateFunction(iProco, CFunctionType.umi_number));
            // 叶命中： "||" 的Token
            this.iMap.SetCellular(SyntaxType.tail_or_Or_Leave, TokenType.Token_Or_Or,
              new CandidateFunction(iProco, CFunctionType.umi_or_Or_));
            // 叶命中： "&&" 的Token
            this.iMap.SetCellular(SyntaxType.tail_and_And_Leave, TokenType.Token_And_And,
              new CandidateFunction(iProco, CFunctionType.umi_and_And_));
            // 叶命中： "!" 的Token
            this.iMap.SetCellular(SyntaxType.tail_not_Leave, TokenType.Token_Not,
              new CandidateFunction(iProco, CFunctionType.umi_not_));
            // 叶命中： "<>" 的Token
            this.iMap.SetCellular(SyntaxType.tail_lessThan_GreaterThan_Leave, TokenType.Token_LessThan_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.umi_lessThan_GreaterThan_));
            // 叶命中： "==" 的Token
            this.iMap.SetCellular(SyntaxType.tail_equality_Equality_Leave, TokenType.Token_Equality_Equality,
              new CandidateFunction(iProco, CFunctionType.umi_equality_Equality_));
            // 叶命中： ">" 的Token
            this.iMap.SetCellular(SyntaxType.tail_greaterThan_Leave, TokenType.Token_GreaterThan,
              new CandidateFunction(iProco, CFunctionType.umi_greaterThan_));
            // 叶命中： "<" 的Token
            this.iMap.SetCellular(SyntaxType.tail_lessThan_Leave, TokenType.Token_LessThan,
              new CandidateFunction(iProco, CFunctionType.umi_lessThan_));
            // 叶命中： ">=" 的Token
            this.iMap.SetCellular(SyntaxType.tail_greaterThan_Equality_Leave, TokenType.Token_GreaterThan_Equality,
              new CandidateFunction(iProco, CFunctionType.umi_greaterThan_Equality_));
            // 叶命中： "<=" 的Token
            this.iMap.SetCellular(SyntaxType.tail_lessThan_Equality_Leave, TokenType.Token_LessThan_Equality,
              new CandidateFunction(iProco, CFunctionType.umi_lessThan_Equality_));
            // 叶命中： # 的Token
            this.iMap.SetCellular(SyntaxType.tail_startEndLeave, TokenType.startend,
              new CandidateFunction(iProco, CFunctionType.umi_startEnd));
        }

        /// <summary>
        /// 复位匹配器：不推导向推导过程转化时内部使用
        /// </summary>
        private void dashReset(SyntaxType startNodeType)
        {
            // 变数初期化
            this.nextTokenPointer = 0;
            this.iParseStack.Clear();
            this.iQueue.Clear();
            // 放置初始节点
            this.iParseStack.Push(SyntaxType.tail_startEndLeave);
            this.iParseStack.Push(startNodeType);
            // 在流的末尾，放置结束标记
            Token ccToken = new Token();
            ccToken.length = 1;
            ccToken.detail = "#";
            ccToken.errorBit = false;
            ccToken.aType = TokenType.startend;
            this.istream.Add(ccToken);
        }

        /// <summary>
        /// 核心算法：递归下降构造语法树并取下一节点
        /// </summary>
        /// <param name="res">母亲节点</param>
        /// <returns>下一个拿去展开的产生式</returns>
        private SyntaxTreeNode Kotori(SyntaxTreeNode res)
        {
            // 已经没有需要递归下降的节点
            if (res == null)
            {
                return null;
            }
            // 否则取她的母亲节点来取得自己的姐妹
            SyntaxTreeNode parent = res.parent;
            // 如果没有母亲，就说明已经回退到了树的最上层
            if (parent == null || parent.children == null)
            {
                return null;
            }
            int i = 0;
            // 遍历寻找自己在姐妹中的排位
            for (; i < parent.children.Count
                && parent.children[i] != res; i++);
            // 跳过自己，找最大的妹妹
            if (i + 1 < parent.children.Count)
            {
                return parent.children[i + 1];
            }
            // 如果自己没有妹妹，那就递归去找母亲的妹妹
            SyntaxTreeNode obac = this.Kotori(parent);
            if (obac != null)
            {
                return obac;
            }
            // 最后，看是否还有没用到的不推导节点
            //if (this.iQueue.Count != 0)
            //{
            //    return iQueue.Dequeue();
            //}
            return null;
        }

        /// <summary>
        /// 核心处理器：将所有非推导项构造到语法树上
        /// </summary>
        /// <returns>预处理完毕的单语句语法树根节点</returns>
        private SyntaxTreeNode Kaguya()
        {
            // 匹配栈出栈
            this.iParseStack.Pop();
            int prescanPointer = this.nextTokenPointer;
            // 扫描token流，命中的第一个关键字token决定了节点类型
            if (this.istream[prescanPointer].aType == TokenType.Token_At)
            {
                // 跳过At符号，读下一个token
                prescanPointer++;
                Token mainToken = this.istream[prescanPointer];
                // 从下一token的类型决定构造的语法树根节点类型，构造参数字典
                SyntaxTreeNode statementNode = new SyntaxTreeNode();
                statementNode.paramDict = new Dictionary<string, SyntaxTreeNode>();
                switch (mainToken.aType)
                {
                    case TokenType.Token_o_a:
                        statementNode.nodeSyntaxType = SyntaxType.synr_a;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["vid"] = new SyntaxTreeNode(SyntaxType.para_vid, statementNode);
                        statementNode.paramDict["face"] = new SyntaxTreeNode(SyntaxType.para_face, statementNode);
                        statementNode.paramDict["loc"] = new SyntaxTreeNode(SyntaxType.para_loc, statementNode);
                        break;
                    case TokenType.Token_o_picture:
                        statementNode.nodeSyntaxType = SyntaxType.synr_picture;
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        statementNode.paramDict["filename"] = new SyntaxTreeNode(SyntaxType.para_filename, statementNode);
                        statementNode.paramDict["x"] = new SyntaxTreeNode(SyntaxType.para_x, statementNode);
                        statementNode.paramDict["y"] = new SyntaxTreeNode(SyntaxType.para_y, statementNode);
                        statementNode.paramDict["capacity"] = new SyntaxTreeNode(SyntaxType.para_capacity, statementNode);
                        statementNode.paramDict["xscale"] = new SyntaxTreeNode(SyntaxType.para_xscale, statementNode);
                        statementNode.paramDict["yscale"] = new SyntaxTreeNode(SyntaxType.para_yscale, statementNode);
                        statementNode.paramDict["ro"] = new SyntaxTreeNode(SyntaxType.para_ro, statementNode);
                        break;
                    case TokenType.Token_o_move:
                        statementNode.nodeSyntaxType = SyntaxType.synr_move;
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        statementNode.paramDict["time"] = new SyntaxTreeNode(SyntaxType.para_time, statementNode);
                        statementNode.paramDict["x"] = new SyntaxTreeNode(SyntaxType.para_x, statementNode);
                        statementNode.paramDict["y"] = new SyntaxTreeNode(SyntaxType.para_y, statementNode);
                        statementNode.paramDict["xacc"] = new SyntaxTreeNode(SyntaxType.para_xacc, statementNode);
                        statementNode.paramDict["yacc"] = new SyntaxTreeNode(SyntaxType.para_yacc, statementNode);
                        statementNode.paramDict["capacity"] = new SyntaxTreeNode(SyntaxType.para_capacity, statementNode);
                        statementNode.paramDict["xscale"] = new SyntaxTreeNode(SyntaxType.para_xscale, statementNode);
                        statementNode.paramDict["yscale"] = new SyntaxTreeNode(SyntaxType.para_yscale, statementNode);
                        statementNode.paramDict["ro"] = new SyntaxTreeNode(SyntaxType.para_ro, statementNode);
                        break;
                    case TokenType.Token_o_deletepicture:
                        statementNode.nodeSyntaxType = SyntaxType.synr_deletepicture;
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        statementNode.paramDict["time"] = new SyntaxTreeNode(SyntaxType.para_time, statementNode);
                        break;
                    case TokenType.Token_o_cstand:
                        statementNode.nodeSyntaxType = SyntaxType.synr_cstand;
                        statementNode.paramDict["cstand"] = new SyntaxTreeNode(SyntaxType.synr_cstand, statementNode);
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["face"] = new SyntaxTreeNode(SyntaxType.para_face, statementNode);
                        statementNode.paramDict["x"] = new SyntaxTreeNode(SyntaxType.para_x, statementNode);
                        statementNode.paramDict["y"] = new SyntaxTreeNode(SyntaxType.para_y, statementNode);
                        break;
                    case TokenType.Token_o_deletecstand:
                        statementNode.nodeSyntaxType = SyntaxType.synr_deletecstand;
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        break;
                    case TokenType.Token_o_se:
                        statementNode.nodeSyntaxType = SyntaxType.synr_se;
                        statementNode.paramDict["filename"] = new SyntaxTreeNode(SyntaxType.para_filename, statementNode);
                        statementNode.paramDict["vol"] = new SyntaxTreeNode(SyntaxType.para_vol, statementNode);
                        break;
                    case TokenType.Token_o_bgm:
                        statementNode.nodeSyntaxType = SyntaxType.synr_bgm;
                        statementNode.paramDict["filename"] = new SyntaxTreeNode(SyntaxType.para_filename, statementNode);
                        statementNode.paramDict["vol"] = new SyntaxTreeNode(SyntaxType.para_vol, statementNode);
                        break;
                    case TokenType.Token_o_stopbgm:
                        statementNode.nodeSyntaxType = SyntaxType.synr_stopbgm;
                        break;
                    case TokenType.Token_o_vocal:
                        statementNode.nodeSyntaxType = SyntaxType.synr_vocal;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["vid"] = new SyntaxTreeNode(SyntaxType.para_vid, statementNode);
                        statementNode.paramDict["vol"] = new SyntaxTreeNode(SyntaxType.para_vol, statementNode);
                        break;
                    case TokenType.Token_o_stopvocal:
                        statementNode.nodeSyntaxType = SyntaxType.synr_stopvocal;
                        break;
                    case TokenType.Token_o_title:
                        statementNode.nodeSyntaxType = SyntaxType.synr_title;
                        break;
                    case TokenType.Token_o_menu:
                        statementNode.nodeSyntaxType = SyntaxType.synr_menu;
                        break;
                    case TokenType.Token_o_save:
                        statementNode.nodeSyntaxType = SyntaxType.synr_save;
                        break;
                    case TokenType.Token_o_load:
                        statementNode.nodeSyntaxType = SyntaxType.synr_load;
                        break;
                    case TokenType.Token_o_lable:
                        statementNode.nodeSyntaxType = SyntaxType.synr_lable;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        break;
                    case TokenType.Token_o_jump:
                        statementNode.nodeSyntaxType = SyntaxType.synr_jump;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["cond"] = new SyntaxTreeNode(SyntaxType.para_cond, statementNode);
                        break;
                    case TokenType.Token_o_call:
                        statementNode.nodeSyntaxType = SyntaxType.synr_call;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["sign"] = new SyntaxTreeNode(SyntaxType.para_sign, statementNode);
                        break;
                    case TokenType.Token_o_for:
                        statementNode.nodeSyntaxType = SyntaxType.synr_for;
                        statementNode.paramDict["cond"] = new SyntaxTreeNode(SyntaxType.para_cond, statementNode);
                        // 这里不追加语句块，因为它将在Parse中处理
                        break;
                    case TokenType.Token_o_endfor:
                        statementNode.nodeSyntaxType = SyntaxType.synr_endfor;
                        // 消除语句块栈
                        if (iBlockStack.Peek().nodeSyntaxType == SyntaxType.case_kotori && iBlockStack.Peek().nodeName.EndsWith("_forBranch"))
                        {
                            iBlockStack.Pop();
                        }
                        else
                        {
                            throw new Exception("语句块匹配不成立");
                        }
                        break;
                    case TokenType.Token_o_if:
                        statementNode.nodeSyntaxType = SyntaxType.synr_if;
                        statementNode.paramDict["cond"] = new SyntaxTreeNode(SyntaxType.para_cond, statementNode);
                        // 这里不追加语句块，因为它将在Parse中处理
                        break;
                    case TokenType.Token_o_else:
                        statementNode.nodeSyntaxType = SyntaxType.synr_else;
                        // 这里不追加语句块，因为它将在Parse中处理
                        break;
                    case TokenType.Token_o_endif:
                        statementNode.nodeSyntaxType = SyntaxType.synr_endif;
                        // 消除语句块栈
                        if (iBlockStack.Peek().nodeSyntaxType == SyntaxType.case_kotori && iBlockStack.Peek().nodeName.EndsWith("_falseBranch"))
                        {
                            iBlockStack.Pop();
                        }
                        if (iBlockStack.Peek().nodeSyntaxType == SyntaxType.case_kotori && iBlockStack.Peek().nodeName.EndsWith("_trueBranch"))
                        {
                            iBlockStack.Pop();
                        }
                        else
                        {
                            throw new Exception("语句块匹配不成立");
                        }
                        break;
                    case TokenType.Token_o_function:
                        statementNode.nodeSyntaxType = SyntaxType.synr_function;
                        statementNode.paramDict["sign"] = new SyntaxTreeNode(SyntaxType.para_sign, statementNode);
                        // 这里不追加语句块，因为它将在Parse中处理
                        break;
                    case TokenType.Token_o_endfunction:
                        statementNode.nodeSyntaxType = SyntaxType.synr_endfunction;
                        // 消除语句块栈
                        if (iBlockStack.Peek().nodeSyntaxType == SyntaxType.case_kotori && iBlockStack.Peek().nodeName.EndsWith("_funDeclaration"))
                        {
                            iBlockStack.Pop();
                        }
                        else
                        {
                            throw new Exception("函数定义不成立");
                        }
                        break;
                    case TokenType.Token_o_scene:
                        statementNode.nodeSyntaxType = SyntaxType.synr_scene;
                        statementNode.paramDict["filename"] = new SyntaxTreeNode(SyntaxType.para_filename, statementNode);
                        statementNode.paramDict["lable"] = new SyntaxTreeNode(SyntaxType.synr_lable, statementNode);
                        break;
                    case TokenType.Token_o_switch:
                        statementNode.nodeSyntaxType = SyntaxType.synr_switch;
                        statementNode.paramDict["id"] = new SyntaxTreeNode(SyntaxType.para_id, statementNode);
                        statementNode.paramDict["state"] = new SyntaxTreeNode(SyntaxType.para_state, statementNode);
                        break;
                    case TokenType.Token_o_var:
                        statementNode.nodeSyntaxType = SyntaxType.synr_var;
                        statementNode.paramDict["name"] = new SyntaxTreeNode(SyntaxType.para_name, statementNode);
                        statementNode.paramDict["dash"] = new SyntaxTreeNode(SyntaxType.para_dash, statementNode);
                        break;
                    case TokenType.Token_o_break:
                        statementNode.nodeSyntaxType = SyntaxType.synr_break;
                        break;
                    case TokenType.Token_o_shutdown:
                        statementNode.nodeSyntaxType = SyntaxType.synr_shutdown;
                        break;
                    case TokenType.Token_o_wait:
                        statementNode.nodeSyntaxType = SyntaxType.synr_wait;
                        statementNode.paramDict["time"] = new SyntaxTreeNode(SyntaxType.para_time, statementNode);
                        break;
                    case TokenType.Token_o_branch:
                        statementNode.nodeSyntaxType = SyntaxType.synr_branch;
                        statementNode.paramDict["link"] = new SyntaxTreeNode(SyntaxType.para_link, statementNode);
                        break;
                    case TokenType.scenecluster:
                        statementNode.nodeSyntaxType = SyntaxType.synr_dialog;
                        break;
                    case TokenType.sceneterminator:
                        statementNode.nodeSyntaxType = SyntaxType.synr_dialogTerminator;
                        break;
                    default:
                        break;
                }
                // 跳过主Token
                prescanPointer++;
                // 接下来预备扫描整个token序列，构造参数字典子树
                bool latticeFlag = false;
                while (prescanPointer < this.istream.Count)
                {
                    // 解析参数列表
                    switch (this.istream[prescanPointer].aType)
                    {
                        case TokenType.Token_p_name:
                            statementNode.paramDict["name"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_name = new SyntaxTreeNode(SyntaxType.tail_idenLeave, statementNode.paramDict["name"]);
                            w_name.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && this.istream[prescanPointer].aType.ToString().StartsWith("Token_p") == false
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_name.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["name"].children.Add(w_name);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_name);
                            break;
                        case TokenType.Token_p_vid:
                            statementNode.paramDict["vid"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_vid = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["vid"]);
                            w_vid.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_vid.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["vid"].children.Add(w_vid);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_vid);
                            break;
                        case TokenType.Token_p_face:
                            statementNode.paramDict["face"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_face = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["face"]);
                            w_face.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_face.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["face"].children.Add(w_face);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_face);
                            break;
                        case TokenType.Token_p_id:
                            statementNode.paramDict["id"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_id = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["id"]);
                            w_id.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_id.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["id"].children.Add(w_id);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_id);
                            break;
                        case TokenType.Token_p_x:
                            statementNode.paramDict["x"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_x = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["x"]);
                            w_x.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_x.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["x"].children.Add(w_x);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_x);
                            break;
                        case TokenType.Token_p_y:
                            statementNode.paramDict["y"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_y = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["y"]);
                            w_y.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_y.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["y"].children.Add(w_y);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_y);
                            break;
                        case TokenType.Token_p_z:
                            statementNode.paramDict["z"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_z = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["z"]);
                            w_z.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_z.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["z"].children.Add(w_z);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_z);
                            break;
                        case TokenType.Token_p_xacc:
                            statementNode.paramDict["xacc"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_xacc = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["xacc"]);
                            w_xacc.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_xacc.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["xacc"].children.Add(w_xacc);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_xacc);
                            break;
                        case TokenType.Token_p_yacc:
                            statementNode.paramDict["yacc"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_yacc = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["yacc"]);
                            w_yacc.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_yacc.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["yacc"].children.Add(w_yacc);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_yacc);
                            break;
                        case TokenType.Token_p_capacity:
                            statementNode.paramDict["capacity"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_capacity = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["capacity"]);
                            w_capacity.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_capacity.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["capacity"].children.Add(w_capacity);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_capacity);
                            break;
                        case TokenType.Token_p_xscale:
                            statementNode.paramDict["xscale"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_xscale = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["xscale"]);
                            w_xscale.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_xscale.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["xscale"].children.Add(w_xscale);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_xscale);
                            break;
                        case TokenType.Token_p_yscale:
                            statementNode.paramDict["yscale"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_yscale = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["yscale"]);
                            w_yscale.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_yscale.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["yscale"].children.Add(w_yscale);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_yscale);
                            break;
                        case TokenType.Token_p_time:
                            statementNode.paramDict["time"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_time = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["time"]);
                            w_time.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_time.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["time"].children.Add(w_time);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_time);
                            break;
                        case TokenType.Token_p_filename:
                            statementNode.paramDict["filename"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_filename = new SyntaxTreeNode(SyntaxType.tail_idenLeave, statementNode.paramDict["filename"]);
                            w_filename.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_filename.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["filename"].children.Add(w_filename);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_filename);
                            break;
                        case TokenType.Token_p_track:
                            statementNode.paramDict["track"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_track = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["track"]);
                            w_track.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_track.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["track"].children.Add(w_track);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_track);
                            break;
                        case TokenType.Token_p_cond:
                            statementNode.paramDict["cond"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_cond = new SyntaxTreeNode(SyntaxType.case_disjunct, statementNode.paramDict["cond"]);
                            w_cond.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_cond.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["cond"].children.Add(w_cond);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_cond);
                            break;
                        case TokenType.Token_p_dash:
                            statementNode.paramDict["dash"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_dash = new SyntaxTreeNode(SyntaxType.case_disjunct, statementNode.paramDict["dash"]);
                            w_dash.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_dash.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["dash"].children.Add(w_dash);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_dash);
                            break;
                        case TokenType.Token_p_state:
                            statementNode.paramDict["state"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_state = new SyntaxTreeNode(SyntaxType.case_disjunct, statementNode.paramDict["state"]);
                            w_state.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_state.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["state"].children.Add(w_state);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_state);
                            break;
                        case TokenType.Token_p_vol:
                            statementNode.paramDict["vol"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_vol = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["vol"]);
                            w_vol.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_vol.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["vol"].children.Add(w_vol);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_vol);
                            break;
                        case TokenType.Token_p_loc:
                            statementNode.paramDict["loc"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_loc = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["loc"]);
                            w_loc.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_loc.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["loc"].children.Add(w_loc);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_loc);
                            break;
                        case TokenType.Token_p_ro:
                            statementNode.paramDict["ro"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_ro = new SyntaxTreeNode(SyntaxType.case_wunit, statementNode.paramDict["ro"]);
                            w_ro.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_ro.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["ro"].children.Add(w_ro);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_ro);
                            break;
                        case TokenType.Token_p_link:
                            statementNode.paramDict["link"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_link = new SyntaxTreeNode(SyntaxType.tail_idenLeave, statementNode.paramDict["link"]);
                            w_link.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_link.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["link"].children.Add(w_link);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_link);
                            break;
                        case TokenType.Token_p_sign:
                            statementNode.paramDict["sign"].children = new List<SyntaxTreeNode>();
                            SyntaxTreeNode w_sign = new SyntaxTreeNode(SyntaxType.tail_idenLeave, statementNode.paramDict["sign"]);
                            w_sign.paramTokenStream = new List<Token>();
                            prescanPointer += 2;
                            while (prescanPointer < this.istream.Count
                                && !this.istream[prescanPointer].aType.ToString().StartsWith("Token_p")
                                && this.istream[prescanPointer].aType != TokenType.startend)
                            {
                                w_sign.paramTokenStream.Add(this.istream[prescanPointer++]);
                            }
                            statementNode.paramDict["sign"].children.Add(w_sign);
                            // 加入不推导队列
                            this.iQueue.Enqueue(w_sign);
                            break;
                        default:
                            break;
                    }
                    // 如果遇到startend就结束
                    if (this.istream[prescanPointer].aType == TokenType.startend)
                    {
                        latticeFlag = true;
                        break;
                    }
                }
                // 如果语句成功封闭，则返回根节点
                if (latticeFlag)
                {
                    return statementNode;
                }
                // 如果语句没有封闭就抛错误给上层
                else
                {
                    return null;
                }
            }
            // 如果是剧情文本的情况下
            else if (this.istream[prescanPointer].aType == TokenType.scenecluster)
            {
                // 把所有的剧情文本聚合成篇章
                SyntaxTreeNode statementNode = new SyntaxTreeNode();
                Token sc = new Token();
                sc.aType = TokenType.scenecluster;
                sc.aColumn = this.istream[prescanPointer].aColumn;
                sc.aLine = this.istream[prescanPointer].aLine;
                while (prescanPointer < this.istream.Count - 1 // 减1是要消掉startend的影响
                    && this.istream[prescanPointer].aType != TokenType.sceneterminator)
                {
                    // 如果匹配错误，就直接向上层抛错误
                    if (!this.istream[prescanPointer].aType.ToString().StartsWith("scene"))
                    {
                        return null;
                    }
                    sc.aTag = (sc.detail += this.istream[prescanPointer++].detail);
                }
                // 把这个唯一token加到语法树上
                statementNode.nodeSyntaxType = SyntaxType.synr_dialog;
                statementNode.nodeValue = (string)sc.aTag;
                statementNode.paramTokenStream = new List<Token>();
                statementNode.paramTokenStream.Add(sc);
                return statementNode;
            }
            else if (this.istream[prescanPointer].aType == TokenType.sceneterminator)
            {
                SyntaxTreeNode statementNode = new SyntaxTreeNode();
                statementNode.nodeSyntaxType = SyntaxType.synr_dialogTerminator;
                return statementNode;
            }
            // 除此以外，直接抛错误给上层
            return null;
        }

        /// <summary>
        /// 核心处理器：通用产生式处理函数
        /// </summary>
        /// <param name="myNode">产生式节点</param>
        /// <param name="myType">候选式类型</param>
        /// <param name="mySyntax">节点语法类型</param>
        /// <param name="myToken">命中单词</param>
        /// <returns>下一个展开节点的指针</returns>
        private SyntaxTreeNode Homura(SyntaxTreeNode myNode, CFunctionType myType, SyntaxType mySyntax, Token myToken)
        {
            // 更新节点信息
            if (myNode != null)
            {
                myNode.nodeType = myType;
                myNode.nodeValue = myToken.detail;
                myNode.nodeSyntaxType = mySyntax;
                myNode.nodeName = mySyntax.ToString();
                if (myToken.isVar)
                {
                    myNode.nodeVarType = myToken.isGlobal ? VarScopeType.GLOBAL : VarScopeType.LOCAL;
                }
            }
            // 取候选向量
            List<SyntaxType> iSvec = this.iDict[Convert.ToInt32(myType)];
            // 左边出栈
            this.iParseStack.Pop();
            // 如果她是一个非终结符
            if (myType < CFunctionType.DERI_UMI_BOUNDARY)
            {
                // 自右向左压匹配栈
                for (int i = iSvec.Count - 1; i >= 0; i--)
                {
                    this.iParseStack.Push(iSvec[i]);
                }
                // 自左向右构造子节点
                bool flag = false;
                SyntaxTreeNode successor = null;
                myNode.children = new List<SyntaxTreeNode>();
                for (int i = 0; i < iSvec.Count; i++)
                {
                    SyntaxTreeNode newNode = new SyntaxTreeNode();
                    newNode.parent = myNode;
                    myNode.children.Add(newNode);
                    if (flag == false)
                    {
                        successor = newNode;
                        flag = true;
                    }
                }
                // 返回第一个产生式
                return successor;
            }
            // 如果她是一个终结符
            else
            {
                // 递增token指针
                if (myType != CFunctionType.umi_epsilon)
                {
                    this.nextTokenPointer++;
                }
                // 返回她的后继
                return this.Kotori(myNode);
            }
        }

        // 语句块嵌套栈
        internal Stack<SyntaxTreeNode> iBlockStack = null;
        // 下一Token指针
        private int nextTokenPointer = 0;
        // 单词流
        private List<Token> istream = new List<Token>();
        // 符号管理器
        private SymbolTable symboler = SymbolTable.getInstance();
        // 匹配栈
        private Stack<SyntaxType> iParseStack = new Stack<SyntaxType>();
        // 候选式类型的向量
        private List<List<SyntaxType>> iDict = new List<List<SyntaxType>>();
        // 不推导候选节点队列
        private Queue<SyntaxTreeNode> iQueue = new Queue<SyntaxTreeNode>();
        // 预测分析表
        private LL1ParseMap iMap = new LL1ParseMap(LL1PARSERMAPROW, LL1PARSERMAPCOL);
        // 分析表行数
        private static readonly int LL1PARSERMAPROW = 33;
        // 分析表列数
        private static readonly int LL1PARSERMAPCOL = 19;
    }
}
