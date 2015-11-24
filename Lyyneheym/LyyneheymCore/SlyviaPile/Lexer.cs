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
        public void Init(string scenario, bool resetFloatPointer = false)
        {
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
        /// 立即获得结果向量
        /// </summary>
        /// <returns>结果向量对应的数组</returns>
        public Token[] GetResultVector()
        {
            return this.resultVector.ToArray();
        }

        /// <summary>
        /// 返回是否已经完成了一轮词法分析
        /// </summary>
        /// <returns>词法分析完成标志位</returns>
        public bool IsComplete()
        {
            return this.finFlag;
        }

        /// <summary>
        /// 将一段文本追加到待分析字符串末尾
        /// </summary>
        /// <param name="appender">待追加的字符串</param>
        public void AppendCode(string appender)
        {
            this.sourceCode += appender;
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
            //bool sentenceFlag = false;
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                Token nextToken;
                //sentenceFlag = this.DFA(out nextToken);
                this.DFA(out nextToken);
                if (nextToken != null)
                {
                    this.resultVector.Add(nextToken);
                }
            }
            // 如果句子标志位还没有成功这一轮分析就没有完成
            //this.finFlag = sentenceFlag;
            // 把结果向量返回给上层
            return this.resultVector;
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
            // 如果finFlag还没有成立，直接进入剧本自动机路径
            if (finFlag == false)
            {
                successFlag = this.GetSceneCluster(res);
            }
            else
            {
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
                        successFlag = this.GetSingleCharaCalculator(res);
                        break;
                    // 可能双字符token
                    case CharacterType.Equality:
                    case CharacterType.LessThan:
                    case CharacterType.GreaterThan:
                    case CharacterType.And:
                    case CharacterType.Or:
                        successFlag = this.GetDoubleCharaCalculator(res);
                        break;
                    // 关键字
                    case CharacterType.Letter:
                        successFlag = this.GetReservedCalculator(res);
                        break;
                    // 标识符
                    case CharacterType.Dollar:
                        successFlag = this.GetIdentifierCalculator(res);
                        break;
                    // 字符串
                    case CharacterType.Quotation:
                    case CharacterType.DoubleQuotation:
                        successFlag = this.GetCluster(res);
                        break;
                    // 剧本对白
                    case CharacterType.LeftBrace:
                        successFlag = this.GetSceneCluster(res);
                        break;
                    // 剧本对白结束：
                    case CharacterType.RightBrace:
                        successFlag = this.EndSceneCluster(res);
                        break;
                    // 常数
                    case CharacterType.Number:
                        successFlag = this.GetConstant(res);
                        break;
                    // 中文（finFlag不成立）
                    case CharacterType.Chinese:
                        successFlag = this.GetSceneCluster(res);
                        break;
                    // 空白
                    case CharacterType.Space:
                        successFlag = this.GetSpace(res);
                        break;
                    // 谜
                    default:
                        successFlag = this.GetUnknown(res);
                        break;
                }
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
        private bool GetSingleCharaCalculator(Token res)
        {
            int glen = this.sourceCode.Length;
            if (this.nextCharPointer + 1 <= glen)
            {
                string str = this.sourceCode.Substring(this.nextCharPointer, 1);
                res.detail = str;
                switch (str)
                {
                    case "+":
                        res.aType = TokenType.Token_Plus;
                        break;
                    case "-":
                        res.aType = TokenType.Token_Minus;
                        break;
                    case "*":
                        res.aType = TokenType.Token_Multiply;
                        break;
                    case "/":
                        res.aType = TokenType.Token_Divide;
                        break;
                    case "!":
                        res.aType = TokenType.Token_Not;
                        break;
                    case "(":
                        res.aType = TokenType.Token_LeftParentheses;
                        break;
                    case ")":
                        res.aType = TokenType.Token_RightParentheses;
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
                this.Jump(1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 可能双字符的自动机路径（最长匹配）
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetDoubleCharaCalculator(Token res)
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
                        res.aType = TokenType.Token_Equality_Equality;
                        okFlag = true;
                        break;
                    case ">=":
                        res.aType = TokenType.Token_GreaterThan_Equality;
                        okFlag = true;
                        break;
                    case "<=":
                        res.aType = TokenType.Token_LessThan_Equality;
                        okFlag = true;
                        break;
                    case "<>":
                        res.aType = TokenType.Token_LessThan_GreaterThan;
                        okFlag = true;
                        break;
                    case "&&":
                        res.aType = TokenType.Token_And_And;
                        okFlag = true;
                        break;
                    case "||":
                        res.aType = TokenType.Token_Or_Or;
                        okFlag = true;
                        break;
                    default:
                        break;
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.detail = str;
                    this.Jump(2);
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
                        res.aType = TokenType.Token_GreaterThan;
                        okFlag = true;
                        break;
                    case "<":
                        res.aType = TokenType.Token_LessThan;
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
                    this.Jump(1);
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
        private bool GetReservedCalculator(Token res)
        {
            int glen = this.sourceCode.Length;
            // 13个字符的情况
            if (this.nextCharPointer + 13 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 13).ToLower();
                if (str == "deletepicture")
                {
                    res.aType = TokenType.Token_o_deletepicture;
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
                    this.Jump(13);
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
                    res.aType = TokenType.Token_o_deletecstand;
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
                    this.Jump(12);
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
                    res.aType = TokenType.Token_o_stopvocal;
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
                    this.Jump(9);
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
                    res.aType = TokenType.Token_o_shutdown;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 8]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "capacity")
                {
                    res.aType = TokenType.Token_p_capacity;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 8]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "filename")
                {
                    res.aType = TokenType.Token_p_filename;
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
                    this.Jump(8);
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
                    res.aType = TokenType.Token_o_picture;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 7]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "stopbgm")
                {
                    res.aType = TokenType.Token_o_stopbgm;
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
                    this.Jump(7);
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
                    res.aType = TokenType.Token_o_cstand;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "endfor")
                {
                    res.aType = TokenType.Token_o_endfor;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "switch")
                {
                    res.aType = TokenType.Token_o_switch;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "xscale")
                {
                    res.aType = TokenType.Token_p_xscale;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "yscale")
                {
                    res.aType = TokenType.Token_p_yscale;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 6]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "branch")
                {
                    res.aType = TokenType.Token_o_branch;
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
                    this.Jump(6);
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
                    res.aType = TokenType.Token_o_vocal;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "title")
                {
                    res.aType = TokenType.Token_o_title;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "lable")
                {
                    res.aType = TokenType.Token_o_lable;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "endif")
                {
                    res.aType = TokenType.Token_o_endif;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "scene")
                {
                    res.aType = TokenType.Token_o_scene;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "break")
                {
                    res.aType = TokenType.Token_o_break;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "track")
                {
                    res.aType = TokenType.Token_p_track;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "state")
                {
                    res.aType = TokenType.Token_p_state;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 5]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "right")
                {
                    res.aType = TokenType.Token_v_right;
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
                    this.Jump(5);
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
                    res.aType = TokenType.Token_o_move;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "menu")
                {
                    res.aType = TokenType.Token_o_menu;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "save")
                {
                    res.aType = TokenType.Token_o_save;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "load")
                {
                    res.aType = TokenType.Token_o_load;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "jump")
                {
                    res.aType = TokenType.Token_o_jump;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "else")
                {
                    res.aType = TokenType.Token_o_else;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "name")
                {
                    res.aType = TokenType.Token_p_name;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "face")
                {
                    res.aType = TokenType.Token_p_face;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "time")
                {
                    res.aType = TokenType.Token_p_time;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "cond")
                {
                    res.aType = TokenType.Token_p_cond;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "dash")
                {
                    res.aType = TokenType.Token_p_dash;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "wait")
                {
                    res.aType = TokenType.Token_o_wait;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "left")
                {
                    res.aType = TokenType.Token_v_left;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "xacc")
                {
                    res.aType = TokenType.Token_p_xacc;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "yacc")
                {
                    res.aType = TokenType.Token_p_yacc;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 4]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "link")
                {
                    res.aType = TokenType.Token_p_link;
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
                    this.Jump(4);
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
                    res.aType = TokenType.Token_o_bgm;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "for")
                {
                    res.aType = TokenType.Token_o_for;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "var")
                {
                    res.aType = TokenType.Token_o_var;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "vid")
                {
                    res.aType = TokenType.Token_p_vid;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "loc")
                {
                    res.aType = TokenType.Token_p_loc;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "mid")
                {
                    res.aType = TokenType.Token_v_mid;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 3]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "vol")
                {
                    res.aType = TokenType.Token_p_vol;
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
                    this.Jump(3);
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
                    res.aType = TokenType.Token_o_stopvocal;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "if")
                {
                    res.aType = TokenType.Token_o_if;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "id")
                {
                    res.aType = TokenType.Token_p_id;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 2]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "ro")
                {
                    res.aType = TokenType.Token_p_ro;
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
                    this.Jump(2);
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
                    res.aType = TokenType.Token_o_a;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "x")
                {
                    res.aType = TokenType.Token_p_x;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "y")
                {
                    res.aType = TokenType.Token_p_y;
                    // 如果后面还有英文字符，那说明这里不可以截断
                    if (this.GetCharType(this.sourceCode[this.nextCharPointer + 1]) != CharacterType.Letter)
                    {
                        okFlag = true;
                    }
                }
                else if (str == "z")
                {
                    res.aType = TokenType.Token_p_z;
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
                    this.Jump(1);
                    return true;
                }
            }
            // 在标识符中没有命中，转到文本内容自动机路径
            return this.GetSceneCluster(res);
        }

        /// <summary>
        /// 标识符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetIdentifierCalculator(Token res)
        {
            // 跳过朵拉符号$
            this.Jump(1);
            // 构造标识符
            StringBuilder sb = new StringBuilder();
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                if (cara == CharacterType.Letter || cara == CharacterType.Number || cara == CharacterType.UnderLine)
                {
                    sb.Append(this.sourceCode[this.nextCharPointer]);
                    this.Jump(1);
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
        private bool GetSceneCluster(Token res)
        {
            // 跳过左花括弧
            if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.LeftBrace)
            {
                this.Jump(1);
                this.finFlag = false;
            }
            // 构造字符串
            StringBuilder sb = new StringBuilder();
            bool entityFlag = false;
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                // 在右花括弧之前的输入都接受，并且这个花括弧不能是转义的
                if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.RightBrace)
                {
                    // 不跳游程，等待DFA进入段落终止符路径，并标志封闭性成立
                    finFlag = true;
                    break;
                }
                else
                {
                    // 处理转义字符并压入字符串构造器，游程在escaping里跳动
                    sb.Append(this.Escaping(this.sourceCode[this.nextCharPointer]));
                    entityFlag = true;
                }
            }
            // 如果成功封闭
            res.aType = TokenType.scenecluster;
            res.aTag = res.detail = sb.ToString();
            return entityFlag;
        }

        /// <summary>
        /// 剧本对白段落终止符的自动机路径
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool EndSceneCluster(Token res)
        {
            // 跳过游程
            this.Jump(1);
            // 修改token信息
            res.aType = TokenType.sceneterminator;
            res.detail = "}";
            return true;
        }

        /// <summary>
        /// 字符串的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetCluster(Token res)
        {
            // 取得左引号的类型并跳过她
            CharacterType latticeType = this.GetCharType(this.sourceCode[this.nextCharPointer]);
            this.Jump(1);
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
                    this.Jump(1);
                    lattice = true;
                    break;
                }
                else
                {
                    // 处理转义字符并压入字符串构造器，游程在escaping里跳动
                    sb.Append(this.Escaping(this.sourceCode[this.nextCharPointer]));
                }
            }
            // 如果成功封闭
            //res.aType = TokenType.cluster;
            res.aType = TokenType.identifier;
            res.aTag = (string)sb.ToString();
            res.detail = sb.ToString();
            res.errorBit = lattice == false;
            return true;
        }

        /// <summary>
        /// 常数数字的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetConstant(Token res)
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
                    this.Jump(1);
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
        private bool GetSpace(Token res)
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
                this.Jump(1);
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
        private bool GetUnknown(Token res)
        {
            // 生成错误的token
            res.aType = TokenType.unknown;
            res.detail = Convert.ToString(this.sourceCode[this.nextCharPointer]);
            res.aTag = "错误：不能匹配为token的字符：" + res.detail;
            // 跳游程
            this.Jump(1);
            return true;
        }

        /// <summary>
        /// 处理转义字符
        /// </summary>
        /// <param name="c">待判断的二义性字符</param>
        /// <returns>转义后字符</returns>
        private char Escaping(char c)
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
                    this.Jump(2);
                    return this.sourceCode[this.nextCharPointer + 1];
                }
            }
            // 跳游程
            this.Jump(1);
            return c;
        }

        /// <summary>
        /// 计算指针跳跃和列数的修正
        /// </summary>
        /// <param name="go"></param>
        private void Jump(int go)
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
    public enum CharacterType
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
}
