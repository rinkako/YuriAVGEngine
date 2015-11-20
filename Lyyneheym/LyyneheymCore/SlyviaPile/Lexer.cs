using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 词法分析器类：负责把用户脚本分割为单词流的类
    /// </summary>
    internal sealed class Lexer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Lexer()
        {
            this.currentLine = RowStartNum;
            this.currentColumn = ColStartNum;
        }

        /// <summary>
        /// 获取当前单词流信息
        /// </summary>
        /// <returns>单词流信息文本</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Token t in this.resultVector)
            {
                sb.AppendLine("Token: " + t.aType.ToString() + " " + (t.errorBit ? "[ERROR]" : ""));
                sb.AppendLine("  Location: " + t.aLine + ", " + t.aColumn);
                sb.AppendLine("  Detail: " + t.detail);
                sb.AppendLine("");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="scenario">待分析的剧本文本</param>
        /// <param name="resetFloatPointer">是否重置游标</param>
        public void init(string scenario, bool resetFloatPointer = false)
        {
            this.finFlag = true;
            this.blockFlag = false;
            this.sourceCode = scenario;
            this.nextCharPointer = 0;
            this.resultVector = new List<Token>();
            if (resetFloatPointer)
            {
                this.currentColumn = ColStartNum;
                this.currentLine = RowStartNum;
            }
        }

        /// <summary>
        /// 启动词法分析DFA自动机
        /// </summary>
        /// <returns>词法分析完毕的的单词流</returns>
        public List<Token> Analyse()
        {
            // 如果上一轮词法分析完成，那么就要重新构造结果向量
            if (this.finFlag == true)
            {
                this.resultVector = new List<Token>();
            }
            // 分析这个句子
            bool sentenceFlag = false;
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                Token nextToken;
                sentenceFlag = this.DFA(out nextToken);
                if (nextToken != null)
                {
                    this.resultVector.Add(nextToken);
                }
            }
            // 如果句子标志位还没有成功这一轮分析就没有完成
            this.finFlag = sentenceFlag;
            // 把结果向量返回给上层
            return this.resultVector;
        }

        /// <summary>
        /// 立即获得结果向量
        /// </summary>
        /// <returns>结果向量对应的数组</returns>
        public Token[] getResultVector()
        {
            return this.resultVector.ToArray();
        }

        /// <summary>
        /// 返回是否已经完成了一轮词法分析
        /// </summary>
        /// <returns>词法分析完成标志位</returns>
        public bool isComplete()
        {
            return this.finFlag;
        }

        /// <summary>
        /// 将一段文本追加到待分析字符串末尾
        /// </summary>
        /// <param name="appender">待追加的字符串</param>
        public void appendCode(string appender)
        {
            this.sourceCode += appender;
        }

        /// <summary>
        /// <para>DFA自动机</para>
        /// <para>处理当前代码指针所在的单词，并移动指针</para>
        /// <para>如果遇到未知的字符，将抛出错误</para>
        /// </summary>
        /// <param name="nextToken">当前代码指针所指单词实例</param>
        /// <returns>是否已经完成了一个句子的分析</returns>
        private bool DFA(out Token nextToken)
        {
            // 定义结果实例并初始化
            Token res = new Token();
            res.aLine = this.currentLine;
            res.aColumn = this.currentColumn;
            res.indexOfCode = this.nextCharPointer;
            int alen = this.sourceCode.Length;
            // 获取下一个字符来判断自动机路径
            bool successFlag = false;
            CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
            switch (cara)
            {
                // 单字符token
                case CharacterType.Plus:
                case CharacterType.Minus:
                case CharacterType.Multiply:
                case CharacterType.Divide:
                case CharacterType.Not:
                case CharacterType.At:
                case CharacterType.LeftParentheses:
                case CharacterType.LeftBracket:
                case CharacterType.RightParentheses:
                case CharacterType.RightBracket:
                    successFlag = this.getSingleCharaCalculator(res);
                    break;
                // 可能双字符token
                case CharacterType.Equality:
                case CharacterType.LessThan:
                case CharacterType.GreaterThan:
                case CharacterType.And:
                case CharacterType.Or:
                    successFlag = this.getDoubleCharaCalculator(res);
                    break;
                // 关键字
                case CharacterType.Letter:
                    successFlag = this.getReservedCalculator(res);
                    break;
                // 标识符
                case CharacterType.Dollar:
                    successFlag = this.getIdentifierCalculator(res);
                    break;
                // 字符串
                case CharacterType.Quotation:
                case CharacterType.DoubleQuotation:
                    successFlag = this.getCluster(res);
                    break;
                // 剧本对白
                case CharacterType.LeftBrace:
                case CharacterType.RightBrace:
                    successFlag = this.getSceneCluster(res);
                    break;
                // 常数
                case CharacterType.Number:
                    successFlag = this.getConstant(res);
                    break;
                // 空白
                case CharacterType.Space:
                    successFlag = this.getSpace(res);
                    break;
                // 谜
                default:
                    successFlag = this.getUnknown(res);
                    break;
            }
            // 如果成功获得了token，就返回给Lexer
            if (successFlag)
            {
                res.length = this.nextCharPointer - res.indexOfCode;
                nextToken = res;
                return blockFlag == false;
            }
            // 否则返回空
            nextToken = null;
            return false;
        }

        /// <summary>
        /// 单字符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSingleCharaCalculator(Token res)
        {
            int glen = this.sourceCode.Length;
            if (this.nextCharPointer + 1 <= glen)
            {
                string str = this.sourceCode.Substring(this.nextCharPointer, 1);
                res.detail = str;
                switch (str)
                {
                    case "+":
                        res.aType = TokenType.token_Plus;
                        break;
                    case "-":
                        res.aType = TokenType.token_Minus;
                        break;
                    case "*":
                        res.aType = TokenType.token_Multiply;
                        break;
                    case "/":
                        res.aType = TokenType.token_Divide;
                        break;
                    case "!":
                        res.aType = TokenType.token_Not;
                        break;
                    case "(":
                        res.aType = TokenType.token_LeftParentheses;
                        break;
                    case ")":
                        res.aType = TokenType.token_RightParentheses;
                        break;
                    case "@":
                        res.aType = TokenType.Token_At;
                        break;
                    case "[":
                        res.aType = TokenType.Token_LeftBracket;
                        break;
                    case "]":
                        res.aType = TokenType.Token_RightBracket;
                        break;
                    case "#":
                        res.aType = TokenType.Token_Sharp;
                        break;
                    default:
                        break;
                }
                // 递增字符指针
                this.jump(1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 可能双字符的自动机路径（最长匹配）
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getDoubleCharaCalculator(Token res)
        {
            int glen = this.sourceCode.Length;
            // 双字符的情况
            if (this.nextCharPointer + 2 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 2);
                switch (str)
                {
                    case "==":
                        res.aType = TokenType.token_Equality_Equality;
                        okFlag = true;
                        break;
                    case ">=":
                        res.aType = TokenType.token_GreaterThan_Equality;
                        okFlag = true;
                        break;
                    case "<=":
                        res.aType = TokenType.token_LessThan_Equality;
                        okFlag = true;
                        break;
                    case "<>":
                        res.aType = TokenType.token_LessThan_GreaterThan;
                        okFlag = true;
                        break;
                    case "&&":
                        res.aType = TokenType.token_And_And;
                        okFlag = true;
                        break;
                    case "||":
                        res.aType = TokenType.token_Or_Or;
                        okFlag = true;
                        break;
                    default:
                        break;
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(2);
                    return true;
                }
            }
            // 双字符匹配失败，进入单字符自动机路径
            if (this.nextCharPointer + 1 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 1);
                switch (str)
                {
                    case ">":
                        res.aType = TokenType.token_GreaterThan;
                        okFlag = true;
                        break;
                    case "<":
                        res.aType = TokenType.token_LessThan;
                        okFlag = true;
                        break;
                    case "=":
                        res.aType = TokenType.Token_Equality;
                        okFlag = true;
                        break;
                    default:
                        break;
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 关键字的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getReservedCalculator(Token res)
        {
            int glen = this.sourceCode.Length;
            // 13个字符的情况
            if (this.nextCharPointer + 13 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 13).ToLower();
                if (str == "deletepicture")
                {
                    res.aType = TokenType.Token_deletepicture;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 13]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(13);
                    return true;
                }
            }
            // 状态转移，12个字符的情况
            if (this.nextCharPointer + 12 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 12).ToLower();
                if (str == "deletecstand")
                {
                    res.aType = TokenType.Token_deletecstand;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 12]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(12);
                    return true;
                }
            }
            // 状态转移，9个字符的情况
            if (this.nextCharPointer + 9 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 9).ToLower();
                if (str == "stopvocal")
                {
                    res.aType = TokenType.Token_stopvocal;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 9]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(9);
                    return true;
                }
            }
            // 状态转移，8个字符的情况
            if (this.nextCharPointer + 8 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 8).ToLower();
                if (str == "shutdown")
                {
                    res.aType = TokenType.Token_shutdown;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 8]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "capacity")
                {
                    res.aType = TokenType.Token_capacity;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 8]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "filename")
                {
                    res.aType = TokenType.Token_filename;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 8]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(8);
                    return true;
                }
            }
            // 状态转移，7个字符的情况
            if (this.nextCharPointer + 7 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 7).ToLower();
                if (str == "picture")
                {
                    res.aType = TokenType.Token_picture;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 7]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "stopbgm")
                {
                    res.aType = TokenType.Token_stopbgm;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 7]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(7);
                    return true;
                }
            }
            // 状态转移，6个字符的情况
            if (this.nextCharPointer + 6 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 6).ToLower();
                if (str == "cstand")
                {
                    res.aType = TokenType.Token_cstand;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "endfor")
                {
                    res.aType = TokenType.Token_endfor;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "switch")
                {
                    res.aType = TokenType.Token_switch;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "xscale")
                {
                    res.aType = TokenType.Token_xscale;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "yscale")
                {
                    res.aType = TokenType.Token_yscale;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(6);
                    return true;
                }
            }
            // 状态转移，5个字符的情况
            if (this.nextCharPointer + 5 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 5).ToLower();
                if (str == "vocal")
                {
                    res.aType = TokenType.Token_vocal;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "title")
                {
                    res.aType = TokenType.Token_title;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "lable")
                {
                    res.aType = TokenType.Token_lable;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "endif")
                {
                    res.aType = TokenType.Token_endif;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "scene")
                {
                    res.aType = TokenType.Token_scene;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "break")
                {
                    res.aType = TokenType.Token_break;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "track")
                {
                    res.aType = TokenType.Token_track;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(5);
                    return true;
                }
            }
            // 状态转移，4个字符的情况
            if (this.nextCharPointer + 4 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 4).ToLower();
                if (str == "move")
                {
                    res.aType = TokenType.Token_move;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "menu")
                {
                    res.aType = TokenType.Token_menu;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "save")
                {
                    res.aType = TokenType.Token_save;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "load")
                {
                    res.aType = TokenType.Token_load;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "jump")
                {
                    res.aType = TokenType.Token_jump;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "else")
                {
                    res.aType = TokenType.Token_else;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "name")
                {
                    res.aType = TokenType.Token_name;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "face")
                {
                    res.aType = TokenType.Token_face;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "time")
                {
                    res.aType = TokenType.Token_time;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "cond")
                {
                    res.aType = TokenType.Token_cond;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "dash")
                {
                    res.aType = TokenType.Token_dash;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "wait")
                {
                    res.aType = TokenType.Token_wait;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(4);
                    return true;
                }
            }
            // 状态转移，3个字符的情况
            if (this.nextCharPointer + 3 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 3).ToLower();
                if (str == "bgm")
                {
                    res.aType = TokenType.Token_bgm;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "for")
                {
                    res.aType = TokenType.Token_for;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "var")
                {
                    res.aType = TokenType.Token_var;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "vid")
                {
                    res.aType = TokenType.Token_vid;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(3);
                    return true;
                }
            }
            // 状态转移，2个字符的情况
            if (this.nextCharPointer + 2 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 2).ToLower();
                if (str == "se")
                {
                    res.aType = TokenType.Token_stopvocal;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "if")
                {
                    res.aType = TokenType.Token_if;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "id")
                {
                    res.aType = TokenType.Token_id;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(2);
                    return true;
                }
            }
            // 状态转移，1个字符的情况
            if (this.nextCharPointer + 1 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 1).ToLower();
                if (str == "a")
                {
                    res.aType = TokenType.Token_a;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "x")
                {
                    res.aType = TokenType.Token_x;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "y")
                {
                    res.aType = TokenType.Token_y;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "z")
                {
                    res.aType = TokenType.Token_z;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.jump(1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 标识符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getIdentifierCalculator(Token res)
        {
            // 跳过朵拉符号$
            this.jump(1);
            // 构造标识符
            StringBuilder sb = new StringBuilder();
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                if (cara == CharacterType.Letter || cara == CharacterType.Number || cara == CharacterType.UnderLine)
                {
                    sb.Append(this.sourceCode[this.nextCharPointer]);
                    this.jump(1);
                }
                else
                {
                    break;
                }
            }
            // 修改token的标签
            res.aType = TokenType.identifier;
            res.detail = sb.ToString();
            return true;
        }

        /// <summary>
        /// 剧本对白的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSceneCluster(Token res)
        {
            // 跳过左花括弧
            this.jump(1);
            // 构造字符串
            bool lattice = false;
            StringBuilder sb = new StringBuilder();
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                // 在右花括弧之前的输入都接受，并且这个花括弧不能是转义的
                if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.RightBrace)
                {
                    // 跳游程并标志封闭性成立
                    this.jump(1);
                    lattice = true;
                    break;
                }
                else
                {
                    // 处理转义字符并压入字符串构造器，游程在escaping里跳动
                    sb.Append(this.escaping(this.sourceCode[this.nextCharPointer]));
                }
            }
            // 如果成功封闭
            res.aType = TokenType.scenecluster;
            res.detail = sb.ToString();
            res.errorBit = lattice == false;
            return true;
        }

        /// <summary>
        /// 字符串的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getCluster(Token res)
        {
            // 取得左引号的类型并跳过她
            CharacterType latticeType = this.GetCharType(this.sourceCode[this.nextCharPointer]);
            this.jump(1);
            // 构造字符串
            bool lattice = false;
            StringBuilder sb = new StringBuilder();
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                // 在双引号之前的输入都接受，并且这个双引号不能是转义的
                if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == latticeType)
                {
                    // 跳游程并标志封闭性成立
                    this.jump(1);
                    lattice = true;
                    break;
                }
                else
                {
                    // 处理转义字符并压入字符串构造器，游程在escaping里跳动
                    sb.Append(this.escaping(this.sourceCode[this.nextCharPointer]));
                }
            }
            // 如果成功封闭
            res.aType = TokenType.cluster;
            res.detail = sb.ToString();
            res.errorBit = lattice == false;
            return true;
        }

        /// <summary>
        /// 常数数字的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getConstant(Token res)
        {
            StringBuilder sb = new StringBuilder();
            bool successFlag = false;
            // 扫描数字序列
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                char c;
                c = this.sourceCode[this.nextCharPointer];
                if (this.GetCharType(c) == CharacterType.Number)
                {
                    sb.Append(c);
                    this.jump(1);
                    successFlag = true;
                }
                else
                {
                    break;
                }
            }
            // 成功得到数字token
            if (successFlag)
            {
                res.aType = TokenType.number;
                res.detail = sb.ToString();
                res.aTag = Convert.ToInt32(sb.ToString());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 空白符号的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSpace(Token res)
        {
            // 获取字符，看是否要换行
            char c = this.sourceCode[this.nextCharPointer];
            if (c == LineTerminator)
            {
                this.currentLine++;
                this.currentColumn = 0;
            }
            // 为空格跳游程
            if (c == ' ')
            {
                this.jump(1);
            }
            else
            {
                this.nextCharPointer++;
            }
            // 空token，永久为false
            return false;
        }

        /// <summary>
        /// 其余情况的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getUnknown(Token res)
        {
            // 生成错误的token
            res.aType = TokenType.unknown;
            res.detail = Convert.ToString(this.sourceCode[this.nextCharPointer]);
            res.aTag = "错误：不能匹配为token的字符：" + res.detail;
            // 跳游程
            this.jump(1);
            return true;
        }

        /// <summary>
        /// 处理转义字符
        /// </summary>
        /// <param name="c">待判断的二义性字符</param>
        /// <returns>转义后字符</returns>
        private char escaping(char c)
        {
            // 只有命中了反斜杠才需要考虑转义
            if (this.GetCharType(c) == CharacterType.Slash
                && this.nextCharPointer + 1 < this.sourceCode.Length)
            {
                CharacterType ct = this.GetCharType(this.sourceCode[this.nextCharPointer + 1]);
                if (ct == CharacterType.Slash ||
                    ct == CharacterType.RightBrace ||
                    ct == CharacterType.DoubleQuotation ||
                    ct == CharacterType.Quotation)
                {
                    // 跳游程
                    this.jump(2);
                    return this.sourceCode[this.nextCharPointer + 1];
                }
            }
            // 跳游程
            this.jump(1);
            return c;
        }

        /// <summary>
        /// 计算指针跳跃和列数的修正
        /// </summary>
        /// <param name="go"></param>
        private void jump(int go)
        {
            this.currentColumn += go;
            this.nextCharPointer += go;
        }

        /// <summary>
        /// 判断一个字符属于哪种类型
        /// </summary>
        /// <param name="c">待分析字符</param>
        /// <returns>字符的类型</returns>
        private CharacterType GetCharType(char c)
        {
            if (c <= 0x9fbb && c >= 0x4e00) { return CharacterType.Chinese; }
            if ('a' <= c && c <= 'z') { return CharacterType.Letter; }
            if ('A' <= c && c <= 'Z') { return CharacterType.Letter; }
            if ('0' <= c && c <= '9') { return CharacterType.Number; }
            if (c == ' ') { return CharacterType.Space; }
            if (c == '_') { return CharacterType.UnderLine; }
            if (c == '.') { return CharacterType.Dot; }
            if (c == ',') { return CharacterType.Comma; }
            if (c == '+') { return CharacterType.Plus; }
            if (c == '-') { return CharacterType.Minus; }
            if (c == '*') { return CharacterType.Multiply; }
            if (c == '/') { return CharacterType.Divide; }
            if (c == '%') { return CharacterType.Percent; }
            if (c == '^') { return CharacterType.Xor; }
            if (c == '&') { return CharacterType.And; }
            if (c == '|') { return CharacterType.Or; }
            if (c == '~') { return CharacterType.Reverse; }
            if (c == '$') { return CharacterType.Dollar; }
            if (c == '<') { return CharacterType.LessThan; }
            if (c == '>') { return CharacterType.GreaterThan; }
            if (c == '(') { return CharacterType.LeftParentheses; }
            if (c == ')') { return CharacterType.RightParentheses; }
            if (c == '[') { return CharacterType.LeftBracket; }
            if (c == ']') { return CharacterType.RightBracket; }
            if (c == '{') { return CharacterType.LeftBrace; }
            if (c == '}') { return CharacterType.RightBrace; }
            if (c == '!') { return CharacterType.Not; }
            if (c == '#') { return CharacterType.Pound; }
            if (c == '?') { return CharacterType.Question; }
            if (c == '"') { return CharacterType.DoubleQuotation; }
            if (c == ':') { return CharacterType.Colon; }
            if (c == ';') { return CharacterType.Semicolon; }
            if (c == '=') { return CharacterType.Equality; }
            if (c == '@') { return CharacterType.At; }
            if (c == '\\') { return CharacterType.Slash; }
            if (c == '\'') { return CharacterType.Quotation; }
            if (c == '\t') { return CharacterType.Space; }
            if (c == '\r') { return CharacterType.Space; }
            if (c == '\n') { return CharacterType.Space; }
            return CharacterType.cUnknown;
        }

        // 原剧本字串
        private string sourceCode = "";
        // 指针
        private int nextCharPointer = 0;
        // 行游标
        private int currentLine = 0;
        // 列游标
        private int currentColumn = 0;
        // 单词流向量
        private List<Token> resultVector = null;
        // 词法分析完成标志位
        private bool finFlag = true;
        // 剧本花括弧匹配标志位
        private bool blockFlag = false;
        // 列初始值
        private static readonly int ColStartNum = 1;
        // 行初始值
        private static readonly int RowStartNum = 1;
        // 换行符
        private static readonly char LineTerminator = '\n';
    }

    /// <summary>
    /// 枚举：字符类型
    /// </summary>
    internal enum CharacterType
    {
        // 未知
        cUnknown,
        // 字母
        Letter,
        // 中文
        Chinese,
        // 数字
        Number,
        // _
        UnderLine,
        // .
        Dot,
        // ,
        Comma,
        // +
        Plus,
        // -
        Minus,
        // *
        Multiply,
        // /
        Divide,
        // %
        Percent,
        // ^
        Xor,
        // &;
        And,
        // |
        Or,
        // ~
        Reverse,
        // $
        Dollar,
        // <
        LessThan,
        // >
        GreaterThan,
        // (
        LeftParentheses,
        // )
        RightParentheses,
        // [
        LeftBracket,
        // ]
        RightBracket,
        // {
        LeftBrace,
        // }
        RightBrace,
        // !
        Not,
        // #
        Pound,
        // "\\"
        Slash,
        // ?
        Question,
        // '
        Quotation,
        // "
        DoubleQuotation,
        // :
        Colon,
        // ;
        Semicolon,
        // =
        Equality,
        // @
        At,
        // space Tab \r\n
        Space
    };

    /// <summary>
    /// 枚举：单词类型
    /// </summary>
    internal enum TokenType
    {
        // 未知
        unknown,
        // 标识符
        identifier,
        // 字符串
        cluster,
        // 剧本字符串
        scenecluster,
        // 整数数字
        number,
        // 起始终止标记
        startend,
        // 符号：#
        Token_Sharp,
        // 符号：左方括号[
        Token_LeftBracket,
        // 符号：右方括号]
        Token_RightBracket,
        // 符号：左花括号{
        Token_LeftBrace,
        // 符号：右花括号}
        Token_RightBrace,
        // 符号：单引号'
        Token_Quotation,
        // 符号：双引号"
        Token_DoubleQuotation,
        // 符号：等号=
        Token_Equality,
        // 符号：艾特符@
        Token_At,
        // 符号：朵拉符$
        Token_Dollar,
        // 符号：左括弧(
        token_LeftParentheses,
        // 符号：右括弧)
        token_RightParentheses,
        // 符号：加+
        token_Plus,
        // 符号：减-
        token_Minus,
        // 符号：乘*
        token_Multiply,
        // 符号：除/
        token_Divide,
        // 符号：不等号<>
        token_LessThan_GreaterThan,
        // 符号：等于号==
        token_Equality_Equality,
        // 符号：大于号>
        token_GreaterThan,
        // 符号：小于号<
        token_LessThan,
        // 符号：大于等于号>=
        token_GreaterThan_Equality,
        // 符号：小于等于号<=
        token_LessThan_Equality,
        // 符号：逻辑或||
        token_Or_Or,
        // 符号：逻辑与&&
        token_And_And,
        // 符号：逻辑否!
        token_Not,
        // 空操作
        Token_NOP,
        // 显示文本
        Token_a,
        // 显示图片
        Token_picture,
        // 移动图片
        Token_move,
        // 消去图片
        Token_deletepicture,
        // 显示立绘
        Token_cstand,
        // 消去立绘
        Token_deletecstand,
        // 播放声效
        Token_se,
        // 播放音乐
        Token_bgm,
        // 停止音乐
        Token_stopbgm,
        // 播放语音
        Token_vocal,
        // 停止语音
        Token_stopvocal,
        // 返回标题
        Token_title,
        // 调用菜单
        Token_menu,
        // 调用存档
        Token_save,
        // 调用读档
        Token_load,
        // 标签
        Token_lable,
        // 标签跳转
        Token_jump,
        // 循环（头）
        Token_for,
        // 循环（尾）
        Token_endfor,
        // 条件（头）
        Token_if,
        // 条件（分支）
        Token_else,
        // 条件（尾）
        Token_endif,
        // 剧本跳转
        Token_scene,
        // 开关操作
        Token_switch,
        // 变量操作
        Token_var,
        // 退出循环
        Token_break,
        // 退出程序
        Token_shutdown,
        // 等待
        Token_wait,
        // 参数：名称
        Token_name,
        // 参数：语音id
        Token_vid,
        // 参数：立绘表情
        Token_face,
        // 参数：序号
        Token_id,
        // 参数：x坐标
        Token_x,
        // 参数：y坐标
        Token_y,
        // 参数：z坐标
        Token_z,
        // 参数：透明度
        Token_capacity,
        // 参数：x轴缩放比
        Token_xscale,
        // 参数：y轴缩放比
        Token_yscale,
        // 参数：时间
        Token_time,
        // 参数：文件名
        Token_filename,
        // 参数：音轨号
        Token_track,
        // 参数：条件子句
        Token_cond,
        // 参数：表达式
        Token_dash
    }
}
