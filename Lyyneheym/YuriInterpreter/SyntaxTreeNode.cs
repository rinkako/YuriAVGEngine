using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
{
    /// <summary>
    /// 语法节点类：构成语法树的最小单元
    /// </summary>
    internal sealed class SyntaxTreeNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <param name="parent">节点的双亲</param>
        public SyntaxTreeNode(SyntaxType type = SyntaxType.Unknown, SyntaxTreeNode parent = null)
        {
            this.NodeName = type.ToString();
            this.NodeSyntaxType = type;
            this.Parent = parent;
        }

        /// <summary>
        /// 节点名字
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 双亲指针
        /// </summary>
        public SyntaxTreeNode Parent { get; set; }

        /// <summary>
        /// 绑定处理函数
        /// </summary>
        public CandidateFunction CandidateFunction = null;

        /// <summary>
        /// 子树向量
        /// </summary>
        public List<SyntaxTreeNode> Children = null;

        /// <summary>
        /// 命中token附加值
        /// </summary>
        public string NodeValue = null;

        /// <summary>
        /// 命中产生式类型
        /// </summary>
        public CFunctionType NodeType = CFunctionType.None;

        /// <summary>
        /// 命中的Token在源代码的行号
        /// </summary> 
        public int Line = 0;
        
        /// <summary>
        /// 命中的Token在源代码的列号
        /// </summary>
        public int Column = 0;
        
        /// <summary>
        /// 附加值
        /// </summary>
        public object Tag = null;
        
        /// <summary>
        /// 逆波兰表达
        /// </summary>
        public string Polish = null;

        /// <summary>
        /// 错误位
        /// </summary>
        public bool ErrorBit = false;

        /// <summary>
        /// 不推导节点参数孩子字典
        /// </summary>
        public Dictionary<string, SyntaxTreeNode> ParamDict = null;

        /// <summary>
        /// 不推导节点参数Token子流
        /// </summary>
        public List<Token> ParamTokenStream = null;

        /// <summary>
        /// 节点变量类型
        /// </summary>
        public VarScopeType NodeVarType = VarScopeType.NOTVAR;

        /// <summary>
        /// 命中语法结构类型
        /// </summary>
        public SyntaxType NodeSyntaxType
        {
            get
            {
                return nodeSyntaxTyper;
            }
            set
            {
                nodeSyntaxTyper = value;
                this.NodeName = value.ToString();
            }
        }

        /// <summary>
        /// 将树描绘成字符串
        /// </summary>
        public string DrawTreeString => this.ToString();

        /// <summary>
        /// 命中语法结构类型
        /// </summary>
        private SyntaxType nodeSyntaxTyper = SyntaxType.Unknown;

        /// <summary>
        /// 树的递归遍历文本化
        /// </summary>
        /// <returns>表示树的字符串</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("> SyntaxTreeNode String Format: ");
            builder.AppendLine(this.NodeName + ", Type:" + this.NodeSyntaxType.ToString() + ", Func:" + this.NodeType.ToString() + String.Empty);
            int identation = 0;
            this.GetTree(builder, this, ref identation, false);
            return builder.ToString();
        }

        /// <summary>
        /// 递归遍历显示
        /// </summary>
        /// <param name="builder">字符串构造器</param>
        /// <param name="myNode">当前节点</param>
        /// <param name="identation">缩进量</param>
        /// <param name="dflag">该节点是否为字典节点</param>
        private void GetTree(StringBuilder builder, SyntaxTreeNode myNode, ref int identation, bool dflag)
        {
            // 如果空就没必要继续了
            if (myNode == null)
            {
                return;
            }
            // 画树
            builder.Append(DrawTree(myNode));
            if (dflag)
            {
                builder.Append("[d]");
            }
            builder.Append(myNode.NodeName.ToString());
            if (myNode.NodeSyntaxType >= SyntaxType.Unknown
              && myNode.NodeSyntaxType != SyntaxType.epsilonLeave
              && myNode.NodeSyntaxType != SyntaxType.tail_startEndLeave)
            {
                builder.Append(" (" + myNode.NodeValue + ")");
            }
            else if (myNode.NodeSyntaxType == SyntaxType.synr_dialog)
            {
                string sub = myNode.NodeValue.Replace("\r", String.Empty).Replace("\n", String.Empty);
                builder.Append(" (" + (sub.Length < 12 ? sub : sub.Substring(0, 11) + " ...") + ")");
            }
            builder.Append(Environment.NewLine);
            // 缩进并打印结果
            identation++;
            if (myNode.NodeSyntaxType.ToString().StartsWith("synr_") && myNode.ParamDict != null)
            {
                foreach (var kvp in myNode.ParamDict)
                {
                    GetTree(builder, kvp.Value, ref identation, true);
                }
            }
            if (myNode.Children != null)
            {
                foreach (SyntaxTreeNode t in myNode.Children)
                {
                    GetTree(builder, t, ref identation, false);
                }
            }
            // 回归缩进
            identation--;
        }

        /// <summary>
        /// 获取缩进
        /// </summary>
        /// <param name="myNode">当前节点</param>
        /// <returns>树的缩进字符串</returns>
        private string DrawTree(SyntaxTreeNode myNode)
        {
            // 若空就不需要继续了，否则取父母节点，若空就不需要画线了
            SyntaxTreeNode parent = myNode?.Parent;
            if (parent == null)
            {
                return String.Empty;
            }
            // 否则查询祖父母节点来看父母节点的排位
            var lstline = new List<bool>();
            while (parent != null)
            {
                SyntaxTreeNode pp = parent.Parent;
                int indexOfParent = 0;
                if (pp != null)
                {
                    if (pp.NodeSyntaxType.ToString().StartsWith("synr_") && pp.ParamDict != null)
                    {
                        indexOfParent += pp.ParamDict.TakeWhile(kvp => kvp.Value != parent).Count();
                        int nocCount = 0;
                        if (pp.Children != null)
                        {
                            nocCount += pp.Children.Count;
                        }
                        lstline.Add(indexOfParent < pp.ParamDict.Count + nocCount - 1);
                    }
                    else if (pp.Children != null)
                    {
                        for (; indexOfParent < pp.Children.Count; indexOfParent++)
                        {
                            if (parent == pp.Children[indexOfParent])
                            {
                                break;
                            }
                        }
                        lstline.Add(indexOfParent < pp.Children.Count - 1);
                    }
                }
                parent = pp;
            }
            // 画纵向线
            string builder = String.Empty;
            for (int i = lstline.Count - 1; i >= 0; i--)
            {
                builder += lstline[i] ? "│  " : "    ";
            }
            // 获得自己在兄弟姐妹中的排行
            parent = myNode.Parent;
            int indexOfParent2 = 0;
            if (parent.NodeSyntaxType.ToString().StartsWith("synr_") && parent.ParamDict != null)
            {
                builder += "└─";
            }
            else if (parent.Children != null)
            {
                for (; indexOfParent2 < parent.Children.Count; indexOfParent2++)
                {
                    if (myNode == parent.Children[indexOfParent2])
                    {
                        break;
                    }
                }
                // 如果是最后一个就不要出头了
                if (indexOfParent2 < parent.Children.Count - 1)
                {
                    builder += "├─";
                }
                else
                {
                    builder += "└─";
                }
            }
            return builder;
        }
    }
}
