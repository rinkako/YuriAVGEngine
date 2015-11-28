using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.symboler = SymbolTable.getInstance();
        }

        /// <summary>
        /// 复位语义分析器
        /// </summary>
        public void Reset()
        {
            this.parseTree = null;
        }

        /// <summary>
        /// 进行一趟用户脚本编译，并把所有语句子树规约到一个共同的根节点上，
        /// 并返回语义分析、流程逻辑处理和代码优化后的动作序列向量
        /// </summary>
        /// <param name="sourceCodeItem">以行分割的源代码字符串向量</param>
        public void StartDash(List<string> sourceCodeItem)
        {
            // 变量初期化
            this.parseTree = new SyntaxTreeNode(SyntaxType.case_kotori);
            this.parseTree.nodeName = "myKotori_Root";
            this.symboler.AddTable(this.parseTree);
            this.parser.iBlockStack.Push(this.parseTree);
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
            string ggs = this.parseTree.ToString();
            Console.WriteLine(ggs);
        }

        /// <summary>
        /// 启动语义分析器
        /// </summary>
        /// <param name="root">语法树根节点</param>
        /// <returns>动作序列向量</returns>
        private List<SceneAction> Semanticer(SyntaxTreeNode root)
        {
            List<SceneAction> resVector = new List<SceneAction>();
            return null;
        }

        /// <summary>
        /// 语法树遍历
        /// </summary>
        /// <param name="mynode">递归节点</param>
        /// <param name="curSa">当前动作序列</param>
        /// <param name="flag">触发器类型</param>
        private void Mise(SyntaxTreeNode mynode, SceneAction curSa, int flag)
        {
            // 自顶向下递归遍历语法树
            switch (mynode.nodeSyntaxType)
            {
                case SyntaxType.Unknown:
                case SyntaxType.case_wexpr:
                case SyntaxType.case_disjunct:
                    break;
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
                            foreach (KeyValuePair<string, SyntaxTreeNode> kvp in mynode.paramDict)
                            {
                                sa.argsDict.Add(kvp.Key, kvp.Value.children[0]);
                            }
                        }
                        // 接下来递归这些孩子，加到真分支去
                        kotoriTrueList.Add(sa);
                        this.Mise(child, sa, flag);
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
                    this.Mise(mynode.children[0], saIfTrue, flag);
                    // 处理假分支
                    curSa.falseRouting = new List<SceneAction>();
                    if (mynode.children[1] == null)
                    {
                        break;
                    }
                    SceneAction saIfFalse = new SceneAction();
                    this.Mise(mynode.children[1], saIfFalse, flag);
                    break;
                case SyntaxType.synr_for:
                    // 处理条件指针
                    curSa.condPointer = mynode.paramDict["cond"];
                    // 处理真分支
                    curSa.trueRouting = new List<SceneAction>();
                    if (mynode.children[0] == null)
                    {
                        break;
                    }
                    SceneAction saForTrue = new SceneAction();
                    this.Mise(mynode.children[0], saForTrue, flag);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 抽象语法树求表达式值
        /// </summary>
        /// <param name="myproxy">代理器</param>
        /// <param name="mynode">递归节点</param>
        /// <returns></returns>
        public static bool AST(SceneAction myproxy, SyntaxTreeNode mynode)
        {
            return false;
        }


        private Lexer lexer = null;
        private Parser parser = null;
        private SymbolTable symboler = null;
        private SceneAction rootSa = null;
        private SyntaxTreeNode parseTree = null;

    }
}
