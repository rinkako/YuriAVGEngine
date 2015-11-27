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
        /// <param name="myproxy">代理器</param>
        /// <param name="mynode">递归节点</param>
        /// <param name="flag">触发器类型</param>
        private void Mise(Scene myproxy, SyntaxTreeNode mynode, int flag)
        {

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
        private SyntaxTreeNode parseTree = null;
    }
}
