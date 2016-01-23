using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LyyneheymCore.SlyviaCore;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 语义分析器：将语法树翻译为运行时环境能够解析的中间代码
    /// </summary>
    public sealed class Pile
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Pile()
        {
            this.lexer = new Lexer();
            this.parser = new Parser();
        }

        /// <summary>
        /// <para>进行一趟用户脚本编译，并把所有语句子树规约到一个共同的根节点上</para>
        /// <para>并返回语义分析、流程逻辑处理和代码优化后的场景实例</para>
        /// </summary>
        /// <param name="sourceCodeItem">以行分割的源代码字符串向量</param>
        /// <param name="sceneName">场景文件的名称，不带路径和后缀名</param>
        /// <returns>该剧本的场景</returns>
        public Scene StartDash(List<string> sourceCodeItem, string sceneName)
        {
            // 初期化
            this.Reset(sceneName);
            foreach (string s in sourceCodeItem)
            {
                // 词法分析
                this.lexer.Init(s);
                List<Token> tokenStream = this.lexer.Analyse();
                // 语法分析
                if (tokenStream.Count > 0)
                {
                    this.parser.SetTokenStream(tokenStream);
                    this.parser.Parse();
                }
            }
            // 语义分析
            this.rootScene = this.parseScene(this.Semanticer(this.parseTree));
            return this.rootScene;
        }

        /// <summary>
        /// 抽象语法树求表达式值
        /// </summary>
        /// <param name="mynode">递归节点</param>
        /// <param name="myproxy">代理器</param>
        /// <returns>表达式的真值</returns>
        public static bool AST(SyntaxTreeNode mynode, SceneAction myproxy)
        {
            switch (mynode.nodeSyntaxType)
            {
                case SyntaxType.case_wexpr:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wexpr__wmulti__wexpr_pi_45)
                    {
                        AST(mynode.children[0], myproxy); // 因式
                        AST(mynode.children[1], myproxy); // 加项
                        if (mynode.children[0].aTag.GetType() == typeof(double) &&
                            mynode.children[1].aTag.GetType() == typeof(double))
                        {
                            mynode.aTag = (double)mynode.children[0].aTag + (double)mynode.children[1].aTag;
                        }
                        else
                        {
                            mynode.aTag = (string)mynode.children[0].aTag + (string)mynode.children[1].aTag;
                        }
                    }
                    else
                    {
                        mynode.aTag = 0;
                    }
                    break;
                case SyntaxType.case_wexpr_pi:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72)
                    {
                        AST(mynode.children[0], myproxy); // 加项
                        AST(mynode.children[1], myproxy); // 加项闭包
                        if (mynode.children[0].aTag.GetType() == typeof(double) &&
                            mynode.children[1].aTag.GetType() == typeof(double))
                        {
                            mynode.aTag = (double)mynode.children[0].aTag + (double)mynode.children[1].aTag;
                        }
                        else
                        {
                            mynode.aTag = (string)mynode.children[0].aTag + (string)mynode.children[1].aTag;
                        }
                    }
                    break;
                case SyntaxType.case_wplus:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wplus__plus_wmulti_46)
                    {
                        AST(mynode.children[1], myproxy);
                        mynode.aTag = mynode.children[1].aTag; // 加法
                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wplus__minus_wmulti_47)
                    {
                        AST(mynode.children[1], myproxy);
                        // 减法
                        if (mynode.children[1].aTag.GetType() == typeof(double))
                        {
                            mynode.aTag = (-1.0) * (double)mynode.children[1].aTag;
                        }
                        else
                        {
                            mynode.aTag = "-" + (string)mynode.children[1].aTag;
                        }
                    }
                    else
                    {
                        mynode.aTag = 0;
                    }
                    break;
                case SyntaxType.case_wmulti:
                    AST(mynode.children[0], myproxy); // 乘项
                    AST(mynode.children[1], myproxy); // 乘项闭包
                    if (mynode.children[0].aTag.GetType() == typeof(double) &&
                        mynode.children[1].aTag.GetType() == typeof(double))
                    {
                        mynode.aTag = (double)mynode.children[0].aTag * (double)mynode.children[1].aTag;
                    }
                    else
                    {
                        mynode.aTag = (string)mynode.children[0].aTag + "*" + (string)mynode.children[1].aTag;
                    }
                    break;
                case SyntaxType.case_wmultiOpt:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50)
                    {
                        AST(mynode.children[1], myproxy); // 乘项
                        AST(mynode.children[2], myproxy); // 乘项闭包
                        // 乘法
                        if (mynode.children[1].aTag.GetType() == typeof(double) &&
                        mynode.children[2].aTag.GetType() == typeof(double))
                        {
                            mynode.aTag = (double)mynode.children[1].aTag * (double)mynode.children[2].aTag;
                        }
                        else
                        {
                            mynode.aTag = (string)mynode.children[1].aTag + "*" + (string)mynode.children[2].aTag;
                        }
                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51)
                    {
                        AST(mynode.children[1], myproxy); // 乘项
                        AST(mynode.children[2], myproxy); // 乘项闭包

                        if (mynode.children[1].aTag.GetType() == typeof(double) &&
                        mynode.children[2].aTag.GetType() == typeof(double))
                        {
                            if ((double)mynode.children[1].aTag * (double)mynode.children[2].aTag == 0)
                            {
                                mynode.aTag = 0.0;
                                // 除零错误
                            }
                            else
                            {
                                mynode.aTag = 1.0f / (double)mynode.children[1].aTag * (double)mynode.children[2].aTag;
                            }

                            mynode.aTag = (double)mynode.children[1].aTag * (double)mynode.children[2].aTag;
                        }
                        else
                        {
                            mynode.aTag = (string)mynode.children[1].aTag + "/" + (string)mynode.children[2].aTag;
                        }
                    }
                    else
                    {
                        mynode.aTag = 1.0f;
                    }
                    break;
                case SyntaxType.case_wunit:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wunit__number_53)
                    {
                        mynode.aTag = Convert.ToDouble(mynode.children[0].nodeValue);
                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wunit__minus_wunit_55)
                    {
                        AST(mynode.children[1], myproxy);
                        if (mynode.children[1].aTag.GetType() == typeof(double))
                        {
                            mynode.aTag = (-1) * (double)mynode.children[1].aTag;
                        }
                        else
                        {
                            mynode.aTag = "-" + (string)mynode.children[1].aTag;
                        }

                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wunit__plus_wunit_56)
                    {
                        AST(mynode.children[1], myproxy);
                        mynode.aTag = mynode.children[1].aTag;
                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wunit__brucket_disjunct_57)
                    {
                        AST(mynode.children[1], myproxy);
                        mynode.aTag = mynode.children[1].aTag;
                    }
                    else if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___wunit__iden_54)
                    {
                        //mynode.aTag = myexec->Reference(mynode.children[0].nodeValue); // 查参数字典
                        // 这里需要绑定运行时环境的变量
                    }
                    break;
                case SyntaxType.case_disjunct:
                    AST(mynode.children[0], myproxy); // 合取项
                    AST(mynode.children[1], myproxy); // 析取闭包
                    mynode.aTag = (bool)mynode.children[0].aTag || (bool)mynode.children[1].aTag;
                    return (bool)mynode.aTag;
                case SyntaxType.case_disjunct_pi:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36)
                    {
                        AST(mynode.children[1], myproxy); // 合取项
                        AST(mynode.children[2], myproxy); // 析取闭包
                        mynode.aTag = (bool)mynode.children[1].aTag || (bool)mynode.children[2].aTag;
                    }
                    else
                    {
                        mynode.aTag = false; // 析取false不影响结果
                    }
                    break;
                case SyntaxType.case_conjunct:
                    AST(mynode.children[0], myproxy); // 布尔项
                    AST(mynode.children[1], myproxy); // 合取闭包
                    mynode.aTag = (bool)mynode.children[0].aTag && (bool)mynode.children[1].aTag;
                    break;
                case SyntaxType.case_conjunct_pi:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39)
                    {
                        AST(mynode.children[1], myproxy); // 布尔项
                        AST(mynode.children[2], myproxy); // 合取闭包
                        mynode.aTag = (bool)mynode.children[1].aTag && (bool)mynode.children[2].aTag;
                    }
                    else
                    {
                        mynode.aTag = true; // 合取true不影响结果
                    }
                    break;
                case SyntaxType.case_bool:
                    if (mynode.candidateFunction.GetCFType() == CFunctionType.deri___bool__not_bool_42)
                    {
                        AST(mynode.children[1], myproxy); // 非项
                        mynode.aTag = ((bool)mynode.children[1].aTag) == false;
                    }
                    else
                    {
                        AST(mynode.children[0], myproxy); // 表达式
                        mynode.aTag = (bool)mynode.children[0].aTag;
                    }
                    break;
                case SyntaxType.case_comp:
                    if (mynode.children[1].candidateFunction.GetCFType() == CFunctionType.deri___rop__epsilon_80)
                    {
                        AST(mynode.children[0], myproxy); // 左边
                        mynode.aTag = mynode.children[0].aTag;
                    }
                    else
                    {
                        string optype = mynode.children[1].nodeValue; // 运算符
                        AST(mynode.children[0], myproxy); // 左边
                        AST(mynode.children[2], myproxy); // 右边
                        mynode.aTag = false;
                        if (mynode.children[0].aTag.GetType() != typeof(double) && 
                            mynode.children[2].aTag.GetType() != typeof(double))
                        {
                            if (optype == "<>")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) != 0;
                            }
                            else if (optype == "==")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) == 0;
                            }
                            else if (optype == ">")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) > 0;
                            }
                            else if (optype == "<")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) < 0;
                            }
                            else if (optype == ">=")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) >= 0;
                            }
                            else if (optype == "<=")
                            {
                                mynode.aTag = string.Compare((string)mynode.children[0].aTag, (string)mynode.children[2].aTag) <= 0;
                            }
                        }
                        else
                        {
                            if (optype == "<>")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag != (double)mynode.children[2].aTag;
                            }
                            else if (optype == "==")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag == (double)mynode.children[2].aTag;
                            }
                            else if (optype == ">")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag > (double)mynode.children[2].aTag;
                            }
                            else if (optype == "<")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag < (double)mynode.children[2].aTag;
                            }
                            else if (optype == ">=")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag >= (double)mynode.children[2].aTag;
                            }
                            else if (optype == "<=")
                            {
                                mynode.aTag = (double)mynode.children[0].aTag <= (double)mynode.children[2].aTag;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// 启动语义分析器，返回动作语法树对应的序列
        /// </summary>
        /// <param name="root">语法树根节点</param>
        /// <returns>一个键值对，剧本的动作序列和函数向量</returns>
        private KeyValuePair<SceneAction, List<SceneFunction>> Semanticer(SyntaxTreeNode root)
        {
            SceneAction resSa = null;
            List<SceneAction> funcSaVec = new List<SceneAction>();
            List<SceneFunction> funcVec = new List<SceneFunction>();
            this.saStack = new Stack<SceneAction>();
            this.blockDict = new Dictionary<string, SceneAction>();
            this.Mise(this.parseTree, ref resSa, funcSaVec);
            this.Tamao(resSa, resSa, false);
            funcSaVec.ForEach((x) => funcVec.Add(this.Tamao(x, x, true)));
            resSa.aTag = this.scenario;
            return new KeyValuePair<SceneAction, List<SceneFunction>>(resSa, funcVec);
        }

        /// <summary>
        /// 递归遍历语法树，构造动作序列
        /// </summary>
        /// <param name="mynode">递归节点</param>
        /// <param name="curSa">当前场景的动作序列头部</param>
        /// <param name="funcSaVec">依附在该场景下的函数的动作序列向量</param>
        private void Mise(SyntaxTreeNode mynode, ref SceneAction curSa, List<SceneAction> funcSaVec)
        {
            // 自顶向下递归遍历语法树
            switch (mynode.nodeSyntaxType)
            {
                case SyntaxType.case_kotori:
                    // 如果是总的根节点
                    if (curSa == null)
                    {
                        curSa = new SceneAction();
                    }
                    if (mynode.children == null)
                    {
                        break;
                    }
                    List<SceneAction> kotoriTrueList = new List<SceneAction>();
                    curSa.trueRouting = kotoriTrueList;
                    // 递归遍历
                    foreach (SyntaxTreeNode child in mynode.children)
                    {
                        SceneAction sa = new SceneAction();
                        sa.aType = (SActionType)Enum.Parse(typeof(SActionType), "act_" + child.nodeSyntaxType.ToString().Replace("synr_", ""));
                        // 跳过增广文法节点，拷贝参数字典
                        if (child.nodeSyntaxType.ToString().StartsWith("synr_")
                            && child.paramDict != null)
                        {
                            foreach (KeyValuePair<string, SyntaxTreeNode> kvp in child.paramDict)
                            {
                                if (kvp.Value.children != null)
                                {
                                    sa.argsDict.Add(kvp.Key, kvp.Value.children[0]);
                                }
                            }
                        }
                        // 如果不是函数定义的话递归就这个孩子，加到真分支去
                        if (child.nodeSyntaxType != SyntaxType.synr_function)
                        {
                            kotoriTrueList.Add(sa);
                        }
                        this.Mise(child, ref sa, funcSaVec);
                    }
                    // 处理序列关系
                    for (int i = 0; i < kotoriTrueList.Count - 1; i++)
                    {
                        kotoriTrueList[i].next = kotoriTrueList[i + 1];
                    }
                    break;
                case SyntaxType.synr_if:
                    // 处理条件指针
                    curSa.condPointer = mynode.paramDict["cond"];
                    // 处理真分支
                    curSa.trueRouting = new List<SceneAction>();
                    if (mynode.children[0] == null)
                    {
                        break;
                    }
                    SceneAction saIfTrue = new SceneAction();
                    this.Mise(mynode.children[0], ref saIfTrue, funcSaVec);
                    for (int i = 0; i < saIfTrue.trueRouting.Count; i++)
                    {
                        curSa.trueRouting.Add(saIfTrue.trueRouting[i]);
                    }
                    // 处理假分支
                    curSa.falseRouting = new List<SceneAction>();
                    if (mynode.children[1] == null || (mynode.children[1].nodeSyntaxType == SyntaxType.synr_endif))
                    {
                        break;
                    }
                    SceneAction saIfFalse = new SceneAction();
                    this.Mise(mynode.children[1], ref saIfFalse, funcSaVec);
                    for (int i = 0; i < saIfFalse.trueRouting.Count; i++)
                    {
                        // 这里之所以是trueRouting是因为kotori节点的缘故
                        curSa.falseRouting.Add(saIfFalse.trueRouting[i]);
                    }
                    break;
                case SyntaxType.synr_for:
                    // 处理条件指针
                    if (mynode.paramDict.ContainsKey("cond"))
                    {
                        curSa.condPointer = mynode.paramDict["cond"];
                    }
                    // 处理真分支
                    curSa.trueRouting = new List<SceneAction>();
                    if (mynode.children[0] == null)
                    {
                        break;
                    }
                    SceneAction saForTrue = new SceneAction();
                    this.Mise(mynode.children[0], ref saForTrue, funcSaVec);
                    for (int i = 0; i < saForTrue.trueRouting.Count; i++)
                    {
                        curSa.trueRouting.Add(saForTrue.trueRouting[i]);
                    }
                    break;
                case SyntaxType.synr_function:
                    // 处理真分支
                    if (mynode.children[0] == null)
                    {
                        break;
                    }
                    SceneAction saFuncTrue = new SceneAction();
                    curSa.trueRouting = new List<SceneAction>();
                    this.Mise(mynode.children[0], ref saFuncTrue, funcSaVec);
                    for (int i = 0; i < saFuncTrue.trueRouting.Count; i++)
                    {
                        curSa.trueRouting.Add(saFuncTrue.trueRouting[i]);
                    }
                    curSa.isBelongFunc = true;
                    // 加到函数向量里
                    funcSaVec.Add(curSa);
                    break;
                case SyntaxType.synr_lable:
                    string labelKey = mynode.paramDict["name"].children[0].nodeValue;
                    curSa.aTag = labelKey;
                    this.blockDict[labelKey] = curSa;
                    break;
                case SyntaxType.synr_jump:
                    string jumpKey = mynode.paramDict["name"].children[0].nodeValue;
                    curSa.aTag = jumpKey;
                    break;
                case SyntaxType.synr_dialog:
                    curSa.aTag = mynode.nodeValue;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 递归遍历动作序列，处理控制流程
        /// </summary>
        /// <param name="saNode">要处理的动作序列头部</param>
        /// <param name="parent">当前序列头部的双亲</param>
        /// <param name="funcFlag">函数序列标记</param>
        /// <returns>函数实例</returns>
        private SceneFunction Tamao(SceneAction saNode, SceneAction parent, bool funcFlag)
        {
            switch (saNode.aType)
            {
                case SActionType.NOP:
                    if (saNode.trueRouting == null || saNode.trueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.trueRouting.Count - 1; i++)
                    {
                        this.Tamao(saNode.trueRouting[i], saNode, false);
                    }
                    // 最后一个孩子的下一节点修改为它母节点的后继
                    saNode.trueRouting[saNode.trueRouting.Count - 1].next = parent.next;
                    break;
                case SActionType.act_for:
                    if (saNode.trueRouting == null || saNode.trueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.trueRouting.Count - 1; i++)
                    {
                        this.Tamao(saNode.trueRouting[i], saNode, false);
                    }
                    // 最后一个孩子的下一节点修改为for子句本身
                    saNode.trueRouting[saNode.trueRouting.Count - 1].next = saNode;
                    break;
                case SActionType.act_endfor:
                    // break节点的下一节点是她的for母节点
                    saNode.next = parent;
                    break;
                case SActionType.act_break:
                    // break节点的下一节点是她的for母节点的后继
                    saNode.next = parent.next;
                    break;
                case SActionType.act_if:
                    // 处理真分支
                    if (saNode.trueRouting == null || saNode.trueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.trueRouting.Count - 1; i++)
                    {
                        this.Tamao(saNode.trueRouting[i], saNode, false);
                    }
                    // 最后一个孩子的下一节点修改为if子句节点的后继
                    saNode.trueRouting[saNode.trueRouting.Count - 1].next = saNode.next;
                    // 处理假分支
                    if (saNode.falseRouting == null || saNode.falseRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.falseRouting.Count - 1; i++)
                    {
                        this.Tamao(saNode.falseRouting[i], saNode, false);
                    }
                    // 最后一个孩子的下一节点修改为if子句节点的后继
                    saNode.falseRouting[saNode.falseRouting.Count - 1].next = saNode.next;
                    break;
                case SActionType.act_jump:
                    // 跳转指令的下一节点通过lable字典指定
                    string jumpToLable = saNode.aTag;
                    if (this.blockDict.ContainsKey(jumpToLable))
                    {
                        saNode.next = this.blockDict[jumpToLable];
                    }
                    else
                    {
                        // 如果没有直接后继节点就返回母节点的后继
                        if (saNode.next == null)
                        {
                            saNode.next = parent.next;
                        }
                    }
                    break;
                default:
                    break;
            }
            // 如果是函数序列就返回一个函数实例
            SceneFunction retSF = null;
            if (funcFlag)
            {
                retSF = this.parseSaToSF(saNode);
            }
            // 最后让递归过程去修改子节点的属性
            saNode.isBelongFunc = funcFlag;
            return retSF;
        }

        /// <summary>
        /// 将动作序列绑定到一个新的场景函数
        /// </summary>
        /// <param name="funcSa">动作序列</param>
        /// <returns>场景函数</returns>
        private SceneFunction parseSaToSF(SceneAction funcSa)
        {
            if (funcSa.isBelongFunc != true)
            {
                throw new Exception("语义错误：一个非函数节点被作为函数声明处理");
            }
            // 获得函数签名
            string signature = funcSa.argsDict["sign"].nodeValue.ToString();
            string[] signItem = signature.Split(new char[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
            if (signItem.Length < 1 || !IsSymbol(signItem[0].Trim()))
            {
                throw new Exception("语义错误：函数签名不合法");
            }
            List<string> funcParas = new List<string>();
            // 如果没有参数就跳过参数遍历
            if (signItem.Length > 1)
            {
                string[] varItem = signItem[1].Split(',');
                foreach (string ivar in varItem)
                {
                    string xvar = ivar.Trim();
                    if (IsSymbol(xvar))
                    {
                        funcParas.Add(xvar);
                    }
                    else
                    {
                        throw new Exception("语义错误：函数签名的参数列表不合法");
                    }
                }
            }
            return new SceneFunction(signItem[0].Trim(), this.scenario, funcSa);
        }

        /// <summary>
        /// 将场景的组件构造成场景实例
        /// </summary>
        /// <param name="sceneItem">一个键值对，主场景序列头部和函数向量</param>
        /// <returns>场景实例</returns>
        private Scene parseScene(KeyValuePair<SceneAction, List<SceneFunction>> sceneItem)
        {
            return new Scene(this.scenario, sceneItem.Key, sceneItem.Value);
        }

        /// <summary>
        /// 初始化语义分析器
        /// </summary>
        /// <param name="scenario">场景名称</param>
        private void Reset(string scenario)
        {
            this.scenario = scenario;
            this.parseTree = new SyntaxTreeNode(SyntaxType.case_kotori);
            this.parseTree.nodeName = "myKotori_Root";
            this.parser.iBlockStack.Push(this.parseTree);
        }

        /// <summary>
        /// 测试一个字符串是否可以作为一个idnetity符号
        /// </summary>
        /// <param name="parStr">待匹配字符串</param>
        /// <returns>是否可以作为C符号</returns>
        private bool IsSymbol(string parStr)
        {
            return IsMatchRegEx(parStr, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        /// <summary>
        /// 测试一个字符串是否满足一个正则式
        /// </summary>
        /// <param name="parStr">待校验字符串</param>
        /// <param name="regEx">正则表达式</param>
        /// <returns>正则式真值</returns>
        public static bool IsMatchRegEx(string parStr, string regEx)
        {
            Regex myRegex = new Regex(regEx);
            return myRegex.IsMatch(parStr);
        }

        // 场景名称
        private string scenario = null;
        // 词法分析器
        private Lexer lexer = null;
        // 语法分析器
        private Parser parser = null;
        // 剧本场景实例
        private Scene rootScene = null;
        // 语法树根节点
        private SyntaxTreeNode parseTree = null;
        // 动作序列嵌套栈
        private Stack<SceneAction> saStack = null;
        // 标签跳转字典
        private Dictionary<string, SceneAction> blockDict = null;
    }
}
