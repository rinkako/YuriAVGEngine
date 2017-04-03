using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Yuri.YuriInterpreter.ILPackage;

namespace Yuri.YuriInterpreter
{
    /// <summary>
    /// 语义分析器：将语法树翻译为运行时环境能够解析的中间代码
    /// </summary>
    internal sealed class Pile
    {
        private const string Encryptor = "yurayuri";

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
        /// <param name="itype">编译类型，决定返回的实例</param>
        /// <returns>Debug模式返回Scene实例，Release时返回该剧本的IL</returns>
        public object StartDash(List<string> sourceCodeItem, string sceneName, InterpreterType itype)
        {
            try
            {
                // 初期化
                this.Reset(sceneName);
                for (int line = 0; line < sourceCodeItem.Count; line++)
                {
                    // 词法分析
                    this.lexer.Init(this.scenario, sourceCodeItem[line]);
                    List<Token> tokenStream = this.lexer.Analyse();
                    // 语法分析
                    if (tokenStream.Count > 0)
                    {
                        this.parser.SetTokenStream(this.scenario, tokenStream);
                        this.parser.Parse(line);
                    }
                }
                // 语义分析
                KeyValuePair<SceneAction, List<SceneFunction>> r = this.Semanticer(this.parseTree);
                this.rootScene = this.ConstructScene(r);
                string il = this.ILGenerator(this.rootScene);
                if (itype == InterpreterType.DEBUG)
                {
                    return this.rootScene;
                }
                else
                {
                    return il;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 进行语义分析
        /// </summary>
        /// <param name="root">语法树根节点</param>
        /// <returns>一个键值对，剧本的动作序列和函数向量</returns>
        private KeyValuePair<SceneAction, List<SceneFunction>> Semanticer(SyntaxTreeNode root)
        {
            SceneAction resSa = null;
            List<SceneAction> funcSaVec = new List<SceneAction>();
            List<SceneFunction> funcVec = new List<SceneFunction>();
            this.forStack = new Stack<SceneAction>();
            this.removeQueueDict = new Dictionary<SceneAction, Queue<SceneAction>>();
            this.blockDict = new Dictionary<string, SceneAction>();
            this.AST(this.parseTree, ref resSa, funcSaVec);
            this.BackpatchOptimizer(resSa, resSa, false);
            funcSaVec.ForEach((x) => funcVec.Add(this.BackpatchOptimizer(x, x, true)));
            resSa.Tag = this.scenario;
            return new KeyValuePair<SceneAction, List<SceneFunction>>(resSa, funcVec);
        }

        /// <summary>
        /// 递归遍历抽象语法树，构造动作序列
        /// </summary>
        /// <param name="mynode">递归节点</param>
        /// <param name="curSa">当前场景的动作序列头部</param>
        /// <param name="funcSaVec">依附在该场景下的函数的动作序列向量</param>
        private void AST(SyntaxTreeNode mynode, ref SceneAction curSa, List<SceneAction> funcSaVec)
        {
            // 设置SA的行列属性
            if (curSa != null)
            {
                curSa.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
            }
            // 自顶向下递归遍历语法树
            switch (mynode.NodeSyntaxType)
            {
                case SyntaxType.case_kotori:
                    // 如果是总的根节点
                    if (curSa == null)
                    {
                        curSa = new SceneAction();
                        curSa.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                    }
                    if (mynode.Children == null)
                    {
                        break;
                    }
                    List<SceneAction> kotoriTrueList = new List<SceneAction>();
                    curSa.TrueRouting = kotoriTrueList;
                    // 递归遍历
                    foreach (SyntaxTreeNode child in mynode.Children)
                    {
                        SceneAction sa = new SceneAction();
                        sa.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                        sa.Type = (SActionType)Enum.Parse(typeof(SActionType), "act_" + child.NodeSyntaxType.ToString().Replace("synr_", String.Empty));
                        // 跳过增广文法节点，拷贝参数字典
                        if (child.NodeSyntaxType.ToString().StartsWith("synr_")
                            && child.ParamDict != null)
                        {
                            foreach (KeyValuePair<string, SyntaxTreeNode> kvp in child.ParamDict)
                            {
                                if (kvp.Value.Children != null)
                                {
                                    sa.ArgsDict.Add(kvp.Key, this.Folding(this.ConstructArgPolish(kvp.Value.Children[0]), mynode));
                                }
                                else
                                {
                                    sa.ArgsDict.Add(kvp.Key, String.Empty);
                                }
                            }
                        }
                        // 如果不是函数定义的话递归就这个孩子，加到真分支去
                        if (child.NodeSyntaxType != SyntaxType.synr_function)
                        {
                            kotoriTrueList.Add(sa);
                        }
                        this.AST(child, ref sa, funcSaVec);
                    }
                    // 处理序列关系
                    for (int i = 0; i < kotoriTrueList.Count - 1; i++)
                    {
                        kotoriTrueList[i].Next = kotoriTrueList[i + 1];
                    }
                    break;
                case SyntaxType.synr_if:
                    // 处理条件指针
                    curSa.CondPolish = this.Folding(this.ConstructArgPolish(mynode.ParamDict["cond"]), mynode);
                    // 处理真分支
                    curSa.TrueRouting = new List<SceneAction>();
                    if (mynode.Children[0] == null)
                    {
                        break;
                    }
                    SceneAction saIfTrue = new SceneAction();
                    saIfTrue.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                    this.AST(mynode.Children[0], ref saIfTrue, funcSaVec);
                    for (int i = 0; i < saIfTrue.TrueRouting.Count; i++)
                    {
                        curSa.TrueRouting.Add(saIfTrue.TrueRouting[i]);
                    }
                    // 处理假分支
                    curSa.FalseRouting = new List<SceneAction>();
                    if (mynode.Children[1] == null || (mynode.Children[1].NodeSyntaxType == SyntaxType.synr_endif))
                    {
                        break;
                    }
                    SceneAction saIfFalse = new SceneAction();
                    saIfFalse.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                    this.AST(mynode.Children[1], ref saIfFalse, funcSaVec);
                    for (int i = 0; i < saIfFalse.TrueRouting.Count; i++)
                    {
                        // 这里之所以是trueRouting是因为kotori节点的缘故
                        curSa.FalseRouting.Add(saIfFalse.TrueRouting[i]);
                    }
                    break;
                case SyntaxType.synr_for:
                    // 处理条件指针
                    if (mynode.ParamDict.ContainsKey("cond"))
                    {
                        curSa.CondPolish = this.Folding(this.ConstructArgPolish(mynode.ParamDict["cond"]), mynode);
                    }
                    // 处理真分支
                    curSa.TrueRouting = new List<SceneAction>();
                    if (mynode.Children[0] == null)
                    {
                        break;
                    }
                    SceneAction saForTrue = new SceneAction();
                    saForTrue.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                    this.AST(mynode.Children[0], ref saForTrue, funcSaVec);
                    for (int i = 0; i < saForTrue.TrueRouting.Count; i++)
                    {
                        curSa.TrueRouting.Add(saForTrue.TrueRouting[i]);
                    }
                    break;
                case SyntaxType.synr_function:
                    // 处理真分支
                    if (mynode.Children[0] == null)
                    {
                        break;
                    }
                    SceneAction saFuncTrue = new SceneAction();
                    saFuncTrue.Tag = mynode.Line.ToString() + "-" + mynode.Column.ToString();
                    curSa.TrueRouting = new List<SceneAction>();
                    this.AST(mynode.Children[0], ref saFuncTrue, funcSaVec);
                    for (int i = 0; i < saFuncTrue.TrueRouting.Count; i++)
                    {
                        curSa.TrueRouting.Add(saFuncTrue.TrueRouting[i]);
                    }
                    curSa.IsBelongFunc = true;
                    // 加到函数向量里
                    funcSaVec.Add(curSa);
                    break;
                case SyntaxType.synr_label:
                    string labelKey = mynode.ParamDict["name"].Children[0].NodeValue;
                    curSa.Tag = labelKey;
                    this.blockDict[labelKey] = curSa;
                    break;
                case SyntaxType.synr_jump:
                    break;
                case SyntaxType.synr_dialog:
                    curSa.Tag = mynode.NodeValue;
                    break;
                default:
                    break;
            }
            // 给节点命名
            curSa.NodeName = String.Format("{0}_{1}@{2}", this.scenario, mynode.Line, curSa.Type.ToString());
        }

        /// <summary>
        /// 递归遍历LL1文法，构造逆波兰式
        /// </summary>
        /// <param name="mynode">递归语法树根节点</param>
        /// <returns>该节点的逆波兰式</returns>
        private string ConstructArgPolish(SyntaxTreeNode mynode)
        {
            switch (mynode.NodeSyntaxType)
            {
                // 需要处理逆波兰式的节点
                case SyntaxType.case_wexpr:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wexpr__wmulti__wexpr_pi_45)
                    {
                        this.ConstructArgPolish(mynode.Children[0]); // 因式
                        this.ConstructArgPolish(mynode.Children[1]); // 加项
                        mynode.Polish += mynode.Children[0].Polish + mynode.Children[1].Polish;
                    }
                    else
                    {
                        mynode.Polish = String.Empty;
                    }
                    break;
                case SyntaxType.case_wexpr_pi:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wexpr_pi__wplus__wexpr_pi_72)
                    {
                        this.ConstructArgPolish(mynode.Children[0]); // 加项
                        this.ConstructArgPolish(mynode.Children[1]); // 加项闭包
                        mynode.Polish += mynode.Children[0].Polish + mynode.Children[1].Polish;
                    }
                    break;
                case SyntaxType.case_wplus:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wplus__plus_wmulti_46)
                    {
                        this.ConstructArgPolish(mynode.Children[1]);
                        // 加法
                        mynode.Polish = mynode.Children[1].Polish + " + ";
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wplus__minus_wmulti_47)
                    {
                        this.ConstructArgPolish(mynode.Children[1]);
                        // 减法
                        mynode.Polish = mynode.Children[1].Polish + " - ";
                    }
                    else
                    {
                        mynode.Polish = String.Empty;
                    }
                    break;
                case SyntaxType.case_wmulti:
                    this.ConstructArgPolish(mynode.Children[0]); // 乘项
                    this.ConstructArgPolish(mynode.Children[1]); // 乘项闭包
                    mynode.Polish = " " + mynode.Children[0].Polish + mynode.Children[1].Polish;
                    break;
                case SyntaxType.case_wmultiOpt:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wmultiOpt__multi_wunit__wmultiOpt_50)
                    {
                        this.ConstructArgPolish(mynode.Children[1]); // 乘项
                        this.ConstructArgPolish(mynode.Children[2]); // 乘项闭包
                        // 乘法
                        mynode.Polish = " " + mynode.Children[1].Polish + " * " + mynode.Children[2].Polish;
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wmultiOpt__div_wunit__wmultiOpt_51)
                    {
                        this.ConstructArgPolish(mynode.Children[1]); // 乘项
                        this.ConstructArgPolish(mynode.Children[2]); // 乘项闭包
                        // 除法
                        mynode.Polish = " " + mynode.Children[1].Polish + " / " + mynode.Children[2].Polish;
                    }
                    else
                    {
                        mynode.Polish = String.Empty;
                    }
                    break;
                case SyntaxType.case_wunit:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wunit__number_53)
                    {
                        mynode.Polish = mynode.Children[0].NodeValue;
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wunit__minus_wunit_55)
                    {
                        this.ConstructArgPolish(mynode.Children[1]);
                        mynode.Polish = "-" + mynode.Children[1].Polish;
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wunit__plus_wunit_56)
                    {
                        this.ConstructArgPolish(mynode.Children[1]);
                        mynode.Polish = mynode.Children[1].Polish;
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wunit__brucket_disjunct_57)
                    {
                        this.ConstructArgPolish(mynode.Children[1]);
                        mynode.Polish = mynode.Children[1].Polish;
                    }
                    else if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___wunit__iden_54)
                    {
                        if (mynode.Children[0].NodeVarType == VarScopeType.GLOBAL)
                        {
                            mynode.Polish = "&" + mynode.Children[0].NodeValue;
                        }
                        else if (mynode.Children[0].NodeVarType == VarScopeType.LOCAL)
                        {
                            mynode.Polish = "$" + mynode.Children[0].NodeValue;
                        }
                        else
                        {
                            mynode.Polish = mynode.Children[0].NodeValue;
                        }
                    }
                    break;
                case SyntaxType.case_disjunct:
                    this.ConstructArgPolish(mynode.Children[0]); // 合取项
                    this.ConstructArgPolish(mynode.Children[1]); // 析取闭包
                    mynode.Polish = mynode.Children[0].Polish + mynode.Children[1].Polish;
                    break;
                case SyntaxType.case_disjunct_pi:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___disjunct_pi__conjunct__disjunct_pi_36)
                    {
                        this.ConstructArgPolish(mynode.Children[1]); // 合取项
                        this.ConstructArgPolish(mynode.Children[2]); // 析取闭包
                        mynode.Polish = mynode.Children[1].Polish + mynode.Children[2].Polish;
                        if (mynode.Children[2].Polish != String.Empty)
                        {
                            mynode.Polish += " || ";
                        }
                    }
                    else
                    {
                        mynode.Polish = String.Empty;
                    }
                    break;
                case SyntaxType.case_conjunct:
                    this.ConstructArgPolish(mynode.Children[0]); // 布尔项
                    this.ConstructArgPolish(mynode.Children[1]); // 合取闭包
                    mynode.Polish = mynode.Children[0].Polish + mynode.Children[1].Polish;
                    break;
                case SyntaxType.case_conjunct_pi:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___conjunct_pi__bool__conjunct_pi_39)
                    {
                        this.ConstructArgPolish(mynode.Children[1]); // 布尔项
                        this.ConstructArgPolish(mynode.Children[2]); // 合取闭包
                        mynode.Polish = mynode.Children[1].Polish + mynode.Children[2].Polish;
                        if (mynode.Children[2].Polish != String.Empty)
                        {
                            mynode.Polish += " && ";
                        }
                    }
                    else
                    {
                        mynode.Polish = String.Empty;
                    }
                    break;
                case SyntaxType.case_bool:
                    if (mynode.CandidateFunction.GetCFType() == CFunctionType.deri___bool__not_bool_42)
                    {
                        this.ConstructArgPolish(mynode.Children[1]); // 非项
                        mynode.Polish = mynode.Children[1].Polish + " ! ";
                    }
                    else
                    {
                        this.ConstructArgPolish(mynode.Children[0]); // 表达式
                        mynode.Polish = mynode.Children[0].Polish;
                    }
                    break;
                case SyntaxType.case_comp:
                    if (mynode.Children[1].CandidateFunction.GetCFType() == CFunctionType.deri___rop__epsilon_80)
                    {
                        this.ConstructArgPolish(mynode.Children[0]); // 左边
                        mynode.Polish = mynode.Children[0].Polish;
                    }
                    else
                    {
                        string optype = mynode.Children[1].NodeValue; // 运算符
                        this.ConstructArgPolish(mynode.Children[0]); // 左边
                        this.ConstructArgPolish(mynode.Children[2]); // 右边
                        mynode.Polish = String.Empty;
                        if (optype == "<>")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " <> ";
                        }
                        else if (optype == "==")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " == ";
                        }
                        else if (optype == ">")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " > ";
                        }
                        else if (optype == "<")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " < ";
                        }
                        else if (optype == ">=")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " >= ";
                        }
                        else if (optype == "<=")
                        {
                            mynode.Polish = mynode.Children[0].Polish + mynode.Children[2].Polish + " <= ";
                        }
                    }
                    break;
                case SyntaxType.tail_idenLeave:
                    if (mynode.NodeVarType == VarScopeType.GLOBAL)
                    {
                        mynode.Polish = "&" + mynode.NodeValue;
                    }
                    else if (mynode.NodeVarType == VarScopeType.LOCAL)
                    {
                        mynode.Polish = "$" + mynode.NodeValue;
                    }
                    else
                    {
                        mynode.Polish = mynode.NodeValue;
                    }
                    break;
                default:
                    break;
            }
            if (mynode.NodeSyntaxType.ToString().StartsWith("para_"))
            {
                if (mynode.Children != null && mynode.Children.Count > 0)
                {
                    mynode.Polish = mynode.Children[0].Polish;
                }
            }
            return mynode.Polish;
        }

        /// <summary>
        /// 递归遍历动作序列，回填控制流程，进行代码优化
        /// </summary>
        /// <param name="saNode">要处理的动作序列头部</param>
        /// <param name="parent">当前序列头部的双亲</param>
        /// <param name="funcFlag">函数序列标记</param>
        /// <returns>函数实例</returns>
        private SceneFunction BackpatchOptimizer(SceneAction saNode, SceneAction parent, bool funcFlag)
        {
            switch (saNode.Type)
            {    
                case SActionType.NOP:
                case SActionType.act_function:
                    if (saNode.TrueRouting == null || saNode.TrueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.TrueRouting.Count - 1; i++)
                    {
                        this.BackpatchOptimizer(saNode.TrueRouting[i], saNode, false);
                    }
                    // 清理要移除的节点
                    while (this.removeQueueDict.ContainsKey(saNode) && this.removeQueueDict[saNode].Count != 0)
                    {
                        saNode.TrueRouting.Remove(this.removeQueueDict[saNode].Dequeue());
                    }
                    // 最后一个孩子的下一节点修改为它母节点的后继
                    SceneAction lastNop = saNode.TrueRouting[saNode.TrueRouting.Count - 1];
                    if (lastNop.Type != SActionType.act_break && lastNop.Type != SActionType.act_endfor)
                    {
                        this.BackpatchOptimizer(lastNop, saNode, false);
                        lastNop.Next = saNode.Next;
                    }
                    else
                    {
                        this.BackpatchOptimizer(lastNop, saNode, false);
                    }
                    break;
                case SActionType.act_dialog:
                    // 合并dialog项
                    if (saNode.DialogDirtyBit) { break; }
                    SceneAction basePtr = saNode;
                    SceneAction iterPtr = saNode;
                    string dialogBuilder = iterPtr.Tag;
                    iterPtr = iterPtr.Next;
                    // 有可能是最后一个孩子递归时DT已经被移除了
                    if (iterPtr == null)
                    {
                        break;
                    }
                    if (this.removeQueueDict.ContainsKey(parent) == false)
                    {
                        this.removeQueueDict[parent] = new Queue<SceneAction>();
                    }
                    while (iterPtr.Type != SActionType.act_dialogTerminator)
                    {
                        iterPtr.DialogDirtyBit = true;
                        dialogBuilder += iterPtr.Tag;
                        this.removeQueueDict[parent].Enqueue(iterPtr);
                        iterPtr = iterPtr.Next;
                    }
                    this.removeQueueDict[parent].Enqueue(iterPtr);
                    if (iterPtr.Next != null &&
                        (iterPtr.Next.Type == SActionType.act_dialog ||
                        iterPtr.Next.Type == SActionType.act_a))
                    {
                        dialogBuilder += "#1";
                    }
                    else
                    {
                        dialogBuilder += "#0";
                    }
                    basePtr.Tag = dialogBuilder;
                    basePtr.Next = iterPtr.Next;
                    break;
                case SActionType.act_dialogTerminator:
                    // 处理对话继续标志位
                    if (saNode.Next != null && 
                        (saNode.Next.Type == SActionType.act_dialog ||
                        saNode.Next.Type == SActionType.act_a))
                    {
                        saNode.Tag += "#1";
                    }
                    else
                    {
                        saNode.Tag += "#0";
                    }
                    break;
                case SActionType.act_for:
                    this.forStack.Push(saNode);
                    if (saNode.TrueRouting == null || saNode.TrueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.TrueRouting.Count - 1; i++)
                    {
                        this.BackpatchOptimizer(saNode.TrueRouting[i], saNode, false);
                    }
                    // 清理要移除的节点
                    while (this.removeQueueDict.ContainsKey(saNode) && this.removeQueueDict[saNode].Count != 0)
                    {
                        saNode.TrueRouting.Remove(this.removeQueueDict[saNode].Dequeue());
                    }
                    // 最后一个孩子的下一节点修改为for子句本身
                    SceneAction lastFor = saNode.TrueRouting[saNode.TrueRouting.Count - 1];
                    if (lastFor.Type != SActionType.act_break && lastFor.Type != SActionType.act_endfor)
                    {
                        lastFor.Next = saNode.Next;
                    }
                    else
                    {
                        this.BackpatchOptimizer(lastFor, saNode, false);
                    }
                    break;
                case SActionType.act_endfor:
                    // endfor节点的下一节点是她的for母节点
                    saNode.Next = parent;
                    // 弹for结构栈
                    this.forStack.Pop();
                    break;
                case SActionType.act_return:
                    // 下一节点是null，这样运行时环境就会弹栈
                    saNode.Next = null;
                    break;
                case SActionType.act_break:
                    // break节点的下一节点是她的for母节点的后继
                    if (this.forStack.Count > 0)
                    {
                        saNode.Next = this.forStack.Peek().Next;
                    }
                    else
                    {
                        throw new InterpreterException("break必须存在for结构的内部");
                    }
                    break;
                case SActionType.act_if:
                    // 处理真分支
                    if (saNode.TrueRouting == null || saNode.TrueRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.TrueRouting.Count - 1; i++)
                    {
                        this.BackpatchOptimizer(saNode.TrueRouting[i], saNode, false);
                    }
                    // 清理要移除的节点
                    while (this.removeQueueDict.ContainsKey(saNode) && this.removeQueueDict[saNode].Count != 0)
                    {
                        saNode.TrueRouting.Remove(this.removeQueueDict[saNode].Dequeue());
                    }
                    // 最后一个孩子的下一节点修改为if子句节点的后继
                    SceneAction lastIfTrue = saNode.TrueRouting[saNode.TrueRouting.Count - 1];
                    // 考虑要更变next属性的节点
                    if (lastIfTrue.Type != SActionType.act_break 
                        && lastIfTrue.Type != SActionType.act_endfor
                        && lastIfTrue.Type != SActionType.act_return)
                    {
                        lastIfTrue.Next = saNode.Next;
                    }
                    else
                    {
                        this.BackpatchOptimizer(lastIfTrue, saNode, false);
                    }
                    // 处理假分支
                    if (saNode.FalseRouting == null || saNode.FalseRouting.Count == 0)
                    {
                        break;
                    }
                    // 递归访问子节点
                    for (int i = 0; i < saNode.FalseRouting.Count - 1; i++)
                    {
                        this.BackpatchOptimizer(saNode.FalseRouting[i], saNode, false);
                    }
                    // 清理要移除的节点
                    while (this.removeQueueDict.ContainsKey(saNode) && this.removeQueueDict[saNode].Count != 0)
                    {
                        saNode.FalseRouting.Remove(this.removeQueueDict[saNode].Dequeue());
                    }
                    // 最后一个孩子的下一节点修改为if子句节点的后继
                    SceneAction lastIfFalse = saNode.FalseRouting[saNode.FalseRouting.Count - 1];
                    // 考虑要更变next属性的节点
                    if (lastIfFalse.Type != SActionType.act_break 
                        && lastIfFalse.Type != SActionType.act_endfor
                        && lastIfFalse.Type != SActionType.act_return)
                    {
                        lastIfFalse.Next = saNode.Next;
                    }
                    else
                    {
                        this.BackpatchOptimizer(lastIfFalse, saNode, false);
                    }
                    break;
                default:
                    break;
            }
            // 如果是函数序列就返回一个函数实例
            SceneFunction retSF = null;
            if (funcFlag)
            {
                retSF = this.ConstructSceneFunction(saNode);
            }
            // 最后让递归过程去修改子节点的属性
            saNode.IsBelongFunc = funcFlag;
            return retSF;
        }

        /// <summary>
        /// 将动作序列绑定到一个新的场景函数
        /// </summary>
        /// <param name="funcSa">动作序列</param>
        /// <returns>场景函数</returns>
        private SceneFunction ConstructSceneFunction(SceneAction funcSa)
        {
            if (funcSa.IsBelongFunc != true)
            {
                throw new InterpreterException()
                {
                    Message = "一个非函数节点被作为函数声明处理",
                    HitLine = Convert.ToInt32((funcSa.Tag.Split('-'))[0]),
                    HitColumn = Convert.ToInt32((funcSa.Tag.Split('-'))[1]),
                    HitPhase = InterpreterException.InterpreterPhase.Sematicer,
                    SceneFileName = this.scenario
                };
            }
            // 获得函数签名
            string signature = funcSa.ArgsDict["sign"];
            string[] signItem = signature.Split(new char[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
            if (signItem.Length < 1 || !IsSymbol(signItem[0].Trim()))
            {
                throw new InterpreterException()
                {
                    Message = "函数签名不合法",
                    HitLine = Convert.ToInt32((funcSa.Tag.Split('-'))[0]),
                    HitColumn = Convert.ToInt32((funcSa.Tag.Split('-'))[1]),
                    HitPhase = InterpreterException.InterpreterPhase.Sematicer,
                    SceneFileName = this.scenario
                };
            }
            List<string> funcParas = new List<string>();
            // 如果没有参数就跳过参数遍历
            if (signItem.Length > 1)
            {
                string[] varItem = signItem[1].Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ivar in varItem)
                {
                    if (ivar.StartsWith("$") && IsSymbol(ivar.Substring(1)))
                    {
                        funcParas.Add(ivar);
                    }
                    else
                    {
                        throw new InterpreterException()
                        {
                            Message = "函数签名的参数列表不合法",
                            HitLine = Convert.ToInt32((funcSa.Tag.Split('-'))[0]),
                            HitColumn = Convert.ToInt32((funcSa.Tag.Split('-'))[1]),
                            HitPhase = InterpreterException.InterpreterPhase.Sematicer,
                            SceneFileName = this.scenario
                        };
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
        private PackageScene ConstructScene(KeyValuePair<SceneAction, List<SceneFunction>> sceneItem)
        {
            return new PackageScene(this.scenario, sceneItem.Key, sceneItem.Value);
        }

        /// <summary>
        /// 将动作序列和从属于它的动作序列全部IL化
        /// </summary>
        /// <param name="saRoot">递归开始节点</param>
        /// <returns>IL字符串</returns>
        private string ILGenerator(SceneAction saRoot)
        {
            StringBuilder resSb = new StringBuilder(String.Empty);
            Stack<SceneAction> processStack = new Stack<SceneAction>();
            processStack.Push(saRoot);
            while (processStack.Count != 0)
            {
                SceneAction topSa = processStack.Pop();
                // 栈，先处理falseRouting
                if (topSa.FalseRouting != null)
                {
                    for (int i = topSa.FalseRouting.Count - 1; i >= 0; i--)
                    {
                        processStack.Push(topSa.FalseRouting[i]);
                    }
                }
                // 处理trueRouting
                if (topSa.TrueRouting != null)
                {
                    for (int i = topSa.TrueRouting.Count - 1; i >= 0; i--)
                    {
                        processStack.Push(topSa.TrueRouting[i]);
                    }
                }
                resSb.AppendLine(this.needEncryption
                    ? YuriEncryptor.EncryptString(topSa.ToIL(), Pile.Encryptor)
                    : topSa.ToIL());
            }
            return resSb.ToString();
        }
        
        /// <summary>
        /// 将场景做IL序列化
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>IL字符串</returns>
        private string ILGenerator(PackageScene scene)
        {
            List<SceneFunction> sf = scene.FuncContainer;
            SceneAction mainSa = scene.Ctor;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.needEncryption
                ? YuriEncryptor.EncryptString(scene.GetILSign(), Pile.Encryptor)
                : scene.GetILSign());
            sb.Append(this.ILGenerator(mainSa));
            foreach (SceneFunction scenefunc in sf)
            {
                sb.Append(this.ILGenerator(scenefunc.Sa));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 常数折叠
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <param name="mynode">该逆波兰式在语法树上的节点</param>
        /// <returns>优化后的逆波兰式</returns>
        private string Folding(string polish, SyntaxTreeNode mynode)
        {
            if (polish == null)
            {
                return null;
            }
            Stack<string> polishStack = new Stack<string>();
            string[] polishItem = polish.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in polishItem)
            {
                PolishItemType ptype = this.GetPolishItemType(item);
                // 如果是操作数就入栈
                if (ptype < PolishItemType.CAL_PLUS)
                {
                    polishStack.Push(item);
                }
                // 操作符就出栈计算再入栈结果
                else
                {
                    // 只有!操作符是单目操作
                    if (ptype == PolishItemType.CAL_NOT && polishStack.Count >= 1)
                    {
                        if (this.GetPolishItemType(polishStack.Peek()) == PolishItemType.CONSTANT)
                        {
                            string booleanItem = polishStack.Pop();
                            polishStack.Push(Math.Abs(Convert.ToDouble(booleanItem)) < 1e-15 ? "1" : "0");
                        }
                        // 非常数项就不能做常数折叠，把操作符入栈
                        else
                        {
                            polishStack.Push(item);
                        }
                    }
                    else if (ptype >= PolishItemType.CAL_PLUS && polishStack.Count >= 2)
                    {
                        string operand2 = polishStack.Pop();
                        string operand1 = polishStack.Pop();
                        // 如果两个操作数有不是常数项的，那就不可以做常数折叠
                        if (this.GetPolishItemType(operand1) != PolishItemType.CONSTANT
                            || this.GetPolishItemType(operand2) != PolishItemType.CONSTANT)
                        {
                            polishStack.Push(operand1);
                            polishStack.Push(operand2);
                            polishStack.Push(item);
                            continue;
                        }
                        // 计算
                        double op1 = Convert.ToDouble(operand1);
                        double op2 = Convert.ToDouble(operand2);
                        double res = 0.0f;
                        switch (ptype)
                        {
                            case PolishItemType.CAL_PLUS:
                                res = op1 + op2;
                                break;
                            case PolishItemType.CAL_MINUS:
                                res = op1 - op2;
                                break;
                            case PolishItemType.CAL_MULTI:
                                res = op1 * op2;
                                break;
                            case PolishItemType.CAL_DIV:
                                if (Math.Abs(op2) < 1e-15)
                                {
                                    throw new InterpreterException()
                                    {
                                        Message = String.Format("除零错误：（{0}/{1}）", op1.ToString(), op2.ToString()),
                                        HitLine = mynode.Line,
                                        HitColumn = mynode.Column,
                                        HitPhase = InterpreterException.InterpreterPhase.Optimizer,
                                        SceneFileName = this.scenario
                                    };
                                }
                                res = op1 / op2;
                                break;
                            case PolishItemType.CAL_ANDAND:
                                res = (Math.Abs(op1) > 1e-15) && (Math.Abs(op2) > 1e-15) ? 1 : 0;
                                break;
                            case PolishItemType.CAL_OROR:
                                res = (Math.Abs(op1) > 1e-15) || (Math.Abs(op2) > 1e-15) ? 1 : 0;
                                break;
                            case PolishItemType.CAL_EQUAL:
                                res = Math.Abs(op1 - op2) < 1e-15 ? 1 : 0;
                                break;
                            case PolishItemType.CAL_NOTEQUAL:
                                res = Math.Abs(op1 - op2) > 1e-15 ? 1 : 0;
                                break;
                            case PolishItemType.CAL_BIG:
                                res = op1 > op2 ? 1 : 0;
                                break;
                            case PolishItemType.CAL_SMALL:
                                res = op1 < op2 ? 1 : 0;
                                break;
                            case PolishItemType.CAL_BIGEQUAL:
                                res = op1 >= op2 ? 1 : 0;
                                break;
                            case PolishItemType.CAL_SMALLEQUAL:
                                res = op1 <= op2 ? 1 : 0;
                                break;
                            default:
                                break;
                        }
                        // 把计算结果压栈
                        polishStack.Push(Convert.ToString(res));
                    }
                    else
                    {
                        string polishStackTrace = String.Empty;
                        while (polishStack.Count != 0)
                        {
                            polishStackTrace = polishStack.Pop() + " ";
                        }
                        throw new InterpreterException()
                        {
                            Message = "polish栈运算错误，栈（顶->底）：" + polishStackTrace,
                            HitLine = -1,
                            HitColumn = -1,
                            HitPhase = InterpreterException.InterpreterPhase.Optimizer,
                            SceneFileName = this.scenario
                        };
                    }
                }
            }
            // 将栈转化为逆波兰式
            string resStr = " ";
            while (polishStack.Count != 0)
            {
                string pitem = polishStack.Pop();
                resStr = " " + pitem + resStr;
            }
            return resStr.TrimStart().TrimEnd();
        }

        /// <summary>
        /// 得到逆波兰式项目的类型
        /// </summary>
        /// <param name="item">项目字符串</param>
        /// <returns>逆波兰式中的类型</returns>
        private PolishItemType GetPolishItemType(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return PolishItemType.NONE;
            }
            else if (item.StartsWith("$") || item.StartsWith("&"))
            {
                return PolishItemType.VAR;
            }
            else if (item[0] >= '0' && item[0] <= '9')
            {
                return PolishItemType.CONSTANT;
            }
            else if (item == "+")
            {
                return PolishItemType.CAL_PLUS;
            }
            else if (item == "-")
            {
                return PolishItemType.CAL_MINUS;
            }
            else if (item == "*")
            {
                return PolishItemType.CAL_MULTI;
            }
            else if (item == "/")
            {
                return PolishItemType.CAL_DIV;
            }
            else if (item == "!")
            {
                return PolishItemType.CAL_NOT;
            }
            else if (item == "&&")
            {
                return PolishItemType.CAL_ANDAND;
            }
            else if (item == "||")
            {
                return PolishItemType.CAL_OROR;
            }
            else if (item == "<>")
            {
                return PolishItemType.CAL_NOTEQUAL;
            }
            else if (item == "==")
            {
                return PolishItemType.CAL_EQUAL;
            }
            else if (item == ">")
            {
                return PolishItemType.CAL_BIG;
            }
            else if (item == "<")
            {
                return PolishItemType.CAL_SMALL;
            }
            else if (item == ">=")
            {
                return PolishItemType.CAL_BIGEQUAL;
            }
            else if (item == "<=")
            {
                return PolishItemType.CAL_SMALLEQUAL;
            }
            return PolishItemType.STRING;
        }

        /// <summary>
        /// 初始化语义分析器
        /// </summary>
        /// <param name="scenario">场景名称</param>
        private void Reset(string scenario)
        {
            this.scenario = scenario;
            this.parseTree = new SyntaxTreeNode(SyntaxType.case_kotori) { NodeName = "myKotori_Root" };
            this.parser.BlockStack.Push(this.parseTree);
        }

        /// <summary>
        /// 测试一个字符串是否可以作为一个idnetity符号
        /// </summary>
        /// <param name="parStr">待匹配字符串</param>
        /// <returns>是否可以作为符号</returns>
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

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool needEncryption = true;

        /// <summary>
        /// 场景名称
        /// </summary>
        private string scenario = null;
        
        /// <summary>
        /// 词法分析器
        /// </summary>
        private readonly Lexer lexer = null;
        
        /// <summary>
        /// 语法分析器
        /// </summary>
        private readonly Parser parser = null;
        
        /// <summary>
        /// 剧本场景实例
        /// </summary>
        private PackageScene rootScene = null;
        
        /// <summary>
        /// 语法树根节点
        /// </summary>
        private SyntaxTreeNode parseTree = null;
        
        /// <summary>
        /// For嵌套栈
        /// </summary>
        private Stack<SceneAction> forStack = null;
        
        /// <summary>
        /// 等待移除节点字典
        /// </summary>
        private Dictionary<SceneAction, Queue<SceneAction>> removeQueueDict = null;
        
        /// <summary>
        /// 标签跳转字典
        /// </summary>
        private Dictionary<string, SceneAction> blockDict = null;
    }

    /// <summary>
    /// 枚举：逆波兰式项类型
    /// </summary>
    internal enum PolishItemType
    {
        NONE,
        CONSTANT,
        STRING,
        VAR,
        VAR_NUM,
        VAR_STRING,
        CAL_PLUS,
        CAL_MINUS,
        CAL_MULTI,
        CAL_DIV,
        CAL_ANDAND,
        CAL_OROR,
        CAL_NOT,
        CAL_EQUAL,
        CAL_NOTEQUAL,
        CAL_BIG,
        CAL_SMALL,
        CAL_BIGEQUAL,
        CAL_SMALLEQUAL
    }
}
