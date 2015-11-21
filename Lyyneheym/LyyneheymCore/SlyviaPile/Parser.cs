using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 语法匹配器类：负责把单词流匹配成语法树的类
    /// </summary>
    public sealed class Parser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Parser()
        {
            this.root = new SyntaxTreeNode();
            this.root.nodeName = "root";
            this.root.children = new List<SyntaxTreeNode>();
        }

        /// <summary>
        /// 为语法匹配器重新设置单词流
        /// </summary>
        /// <param name="myStream">单词流向量</param>
        public void SetTokenStream(List<Token> myStream)
        {

        }

        /// <summary>
        /// 复位匹配器
        /// </summary>
        public void Reset()
        {

        }

        /// <summary>
        /// 启动语法分析器
        /// </summary>
        /// <returns>匹配完毕的语法树</returns>
        public SyntaxTreeNode Parse()
        {
            return null;
        }

        /// <summary>
        /// 取语法展开式子节点向量
        /// </summary>
        /// <param name="cft">展开式类型</param>
        /// <returns>产生式子节点类型向量</returns>
        private List<SyntaxType> GetVector(CFunctionType cft)
        {
            return null;
        }

        /// <summary>
        /// 取匹配栈指针
        /// </summary>
        /// <returns>匹配栈的指针</returns>
        private Stack<SyntaxType> GetStack()
        {
            return null;
        }

        /// <summary>
        /// 初始化预测分析表、链接向量和LL1文法预测表
        /// </summary>
        private void Init()
        {

        }

        /// <summary>
        /// 取下一节点的指针
        /// </summary>
        /// <param name="res">父母节点</param>
        /// <returns>下一个拿去展开的产生式</returns>
        private SyntaxTreeNode NextNode(SyntaxTreeNode res)
        {
            return null;
        }

        /// <summary>
        /// 预处理器：将所有非推导项构造到语法树上
        /// </summary>
        /// <returns>预处理完毕的语法树根节点</returns>
        private SyntaxTreeNode Kaguya()
        {
            // 扫描token流，命中的第一个关键字token决定了节点类型
            if (this.istream[this.nextTokenPointer].aType == TokenType.Token_LeftBracket)
            {
                // 跳过左方括弧，读下一个token
                this.nextTokenPointer++;
                Token mainToken = this.istream[this.nextTokenPointer++];
                // 从下一token的类型决定构造的语法树根节点类型
                switch (mainToken.aType)
                {
                    case TokenType.Token_a:
                        this.root.nodeSyntaxType = SyntaxType.synr_a;
                        this.root.children.Add(new SyntaxTreeNode(SyntaxType.para_name));
                        this.root.children.Add(new SyntaxTreeNode(SyntaxType.para_face));
                        this.root.children.Add(new SyntaxTreeNode(SyntaxType.para_vid));

                        break;
                    case TokenType.Token_picture:
                        this.root.nodeSyntaxType = SyntaxType.synr_picture;
                        break;
                    case TokenType.Token_move:
                        this.root.nodeSyntaxType = SyntaxType.synr_move;
                        break;
                    case TokenType.Token_deletepicture:
                        this.root.nodeSyntaxType = SyntaxType.synr_deletepicture;
                        break;
                    case TokenType.Token_cstand:
                        this.root.nodeSyntaxType = SyntaxType.synr_cstand;
                        break;
                    case TokenType.Token_deletecstand:
                        this.root.nodeSyntaxType = SyntaxType.synr_deletecstand;
                        break;
                    case TokenType.Token_se:
                        this.root.nodeSyntaxType = SyntaxType.synr_se;
                        break;
                    case TokenType.Token_bgm:
                        this.root.nodeSyntaxType = SyntaxType.synr_bgm;
                        break;
                    case TokenType.Token_stopbgm:
                        this.root.nodeSyntaxType = SyntaxType.synr_stopbgm;
                        break;
                    case TokenType.Token_vocal:
                        this.root.nodeSyntaxType = SyntaxType.synr_vocal;
                        break;
                    case TokenType.Token_stopvocal:
                        this.root.nodeSyntaxType = SyntaxType.synr_stopvocal;
                        break;
                    case TokenType.Token_title:
                        this.root.nodeSyntaxType = SyntaxType.synr_title;
                        break;
                    case TokenType.Token_menu:
                        this.root.nodeSyntaxType = SyntaxType.synr_menu;
                        break;
                    case TokenType.Token_save:
                        this.root.nodeSyntaxType = SyntaxType.synr_save;
                        break;
                    case TokenType.Token_load:
                        this.root.nodeSyntaxType = SyntaxType.synr_load;
                        break;
                    case TokenType.Token_lable:
                        this.root.nodeSyntaxType = SyntaxType.synr_lable;
                        break;
                    case TokenType.Token_jump:
                        this.root.nodeSyntaxType = SyntaxType.synr_jump;
                        break;
                    case TokenType.Token_for:
                        this.root.nodeSyntaxType = SyntaxType.synr_for;
                        break;
                    case TokenType.Token_endfor:
                        this.root.nodeSyntaxType = SyntaxType.synr_endfor;
                        break;
                    case TokenType.Token_if:
                        this.root.nodeSyntaxType = SyntaxType.synr_if;
                        break;
                    case TokenType.Token_else:
                        this.root.nodeSyntaxType = SyntaxType.synr_else;
                        break;
                    case TokenType.Token_endif:
                        this.root.nodeSyntaxType = SyntaxType.synr_endfor;
                        break;
                    case TokenType.Token_scene:
                        this.root.nodeSyntaxType = SyntaxType.synr_scene;
                        break;
                    case TokenType.Token_switch:
                        this.root.nodeSyntaxType = SyntaxType.synr_switch;
                        break;
                    case TokenType.Token_var:
                        this.root.nodeSyntaxType = SyntaxType.synr_var;
                        break;
                    case TokenType.Token_break:
                        this.root.nodeSyntaxType = SyntaxType.synr_break;
                        break;
                    case TokenType.Token_shutdown:
                        this.root.nodeSyntaxType = SyntaxType.synr_shutdown;
                        break;
                    case TokenType.Token_wait:
                        this.root.nodeSyntaxType = SyntaxType.synr_wait;
                        break;
                    default:
                        break;
                }

            }
            // 如果是剧情文本的情况下
            else if (this.istream[this.nextTokenPointer].aType == TokenType.Token_LeftBrace)
            {

            }
            // 除此以外
            else
            {

            }
            return null;
        }

        /// <summary>
        /// 核心处理器：通用产生式处理函数
        /// </summary>
        /// <param name="myNode">产生式节点</param>
        /// <param name="myType">候选式类型</param>
        /// <param name="mySyntax">节点语法类型</param>
        /// <param name="myValue">命中单词文本</param>
        /// <returns>下一个展开节点的指针</returns>
        private SyntaxTreeNode Homura(SyntaxTreeNode myNode, CFunctionType myType, SyntaxType mySyntax, string myValue)
        {
            return null;
        }

        // 语法树根节点
        public SyntaxTreeNode root = null;
        // 下一Token指针
        public int nextTokenPointer = 0;
        // 单词流
        private List<Token> istream = new List<Token>();
        // 句子容器
        private string scentence = null;
        // 匹配栈
        private Stack<SyntaxType> iParseStack = new Stack<SyntaxType>();
        // 候选式类型的向量
        private List<List<SyntaxType>> iDict = new List<List<SyntaxType>>();
    }
}
