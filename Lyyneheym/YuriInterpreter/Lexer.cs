using System;
using System.Collections.Generic;
using System.Text;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
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
                sb.AppendLine("Token: " + t.Type.ToString() + " " + (t.ErrorBit ? "[ERROR]" : String.Empty));
                sb.AppendLine("  Location: " + t.Line + ", " + t.Column);
                sb.AppendLine("  Detail: " + t.OriginalCodeStr);
                sb.AppendLine(String.Empty);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="fileName">脚本文件名</param>
        /// <param name="scenarioCode">待分析的剧本文本</param>
        /// <param name="resetFloatPointer">是否重置游标</param>
        public void Init(string fileName, string scenarioCode, bool resetFloatPointer = false)
        {
            this.dealingFile = fileName;
            this.blockFlag = false;
            this.sourceCode = scenarioCode;
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
            if (this.finFlag)
            {
                this.resultVector = new List<Token>();
            }
            // 分析这个句子
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                this.DFA(out Token nextToken);
                if (nextToken != null)
                {
                    this.resultVector.Add(nextToken);
                }
            }
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
            Token res = new Token
            {
                Line = this.currentLine,
                Column = this.currentColumn,
                IndexOfCode = this.nextCharPointer
            };
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
                    // 注释
                    case CharacterType.Pound:
                        this.GetNotation();
                        break;
                    // 单字符token
                    case CharacterType.Plus:
                    case CharacterType.Minus:
                    case CharacterType.Multiply:
                    case CharacterType.Divide:
                    case CharacterType.Not:
                    case CharacterType.At:
                    case CharacterType.LeftParentheses:
                    case CharacterType.RightParentheses:
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
                    // 标识符（$、&、%，其中&在&&路径中处理）
                    case CharacterType.Dollar:
                    case CharacterType.Percent:
                        successFlag = this.GetIdentifierCalculator(res);
                        break;
                    // 字符串
                    case CharacterType.Quotation:
                    case CharacterType.DoubleQuotation:
                        successFlag = this.GetCluster(res);
                        break;
                    // 剧本对白
                    case CharacterType.LeftBracket:
                        successFlag = this.GetSceneCluster(res);
                        break;
                    // 剧本对白结束：
                    case CharacterType.RightBracket:
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
                        successFlag = this.GetSpace();
                        break;
                    // 谜
                    default:
                        successFlag = this.GetUnknown(res);
                        throw new InterpreterException()
                        {
                            Message = "有未识别的字符输入：" + res.OriginalCodeStr,
                            HitLine = res.Line,
                            HitColumn = res.Column,
                            HitPhase = InterpreterPhase.Lexer,
                            SceneFileName = this.dealingFile
                        };
                }
            }
            // 如果成功获得了token，就返回给Lexer
            if (successFlag)
            {
                res.Length = this.nextCharPointer - res.IndexOfCode;
                nextToken = res;
                return blockFlag == false;
            }
            // 否则返回空
            nextToken = null;
            return false;
        }

        /// <summary>
        /// 注释的自动机路径
        /// </summary>
        private void GetNotation()
        {
            // 循环，直到行末
            while (++this.nextCharPointer < this.sourceCode.Length
                && this.sourceCode[this.nextCharPointer] != '\n');
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
                res.OriginalCodeStr = str;
                switch (str)
                {
                    case "+":
                        res.Type = TokenType.Token_Plus;
                        break;
                    case "-":
                        res.Type = TokenType.Token_Minus;
                        break;
                    case "*":
                        res.Type = TokenType.Token_Multiply;
                        break;
                    case "/":
                        res.Type = TokenType.Token_Divide;
                        break;
                    case "!":
                        res.Type = TokenType.Token_Not;
                        break;
                    case "(":
                        res.Type = TokenType.Token_LeftParentheses;
                        break;
                    case ")":
                        res.Type = TokenType.Token_RightParentheses;
                        break;
                    case "@":
                        res.Type = TokenType.Token_At;
                        break;
                    case "[":
                        res.Type = TokenType.Token_LeftBracket;
                        break;
                    case "]":
                        res.Type = TokenType.Token_RightBracket;
                        break;
                    case "#":
                        res.Type = TokenType.Token_Sharp;
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
                        res.Type = TokenType.Token_Equality_Equality;
                        okFlag = true;
                        break;
                    case ">=":
                        res.Type = TokenType.Token_GreaterThan_Equality;
                        okFlag = true;
                        break;
                    case "<=":
                        res.Type = TokenType.Token_LessThan_Equality;
                        okFlag = true;
                        break;
                    case "<>":
                        res.Type = TokenType.Token_LessThan_GreaterThan;
                        okFlag = true;
                        break;
                    case "&&":
                        res.Type = TokenType.Token_And_And;
                        okFlag = true;
                        break;
                    case "||":
                        res.Type = TokenType.Token_Or_Or;
                        okFlag = true;
                        break;
                    default:
                        break;
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.OriginalCodeStr = str;
                    this.Jump(2);
                    return true;
                }
            }
            bool idFlag = false;
            // 双字符匹配失败，进入单字符自动机路径
            if (this.nextCharPointer + 1 <= glen)
            {
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, 1);
                switch (str)
                {
                    case ">":
                        res.Type = TokenType.Token_GreaterThan;
                        okFlag = true;
                        break;
                    case "<":
                        res.Type = TokenType.Token_LessThan;
                        okFlag = true;
                        break;
                    case "=":
                        res.Type = TokenType.Token_Equality;
                        okFlag = true;
                        break;
                    // 遇到&符号就跳转到变量处理路径上
                    case "&":
                        idFlag = this.GetIdentifierCalculator(res);
                        break;
                    default:
                        break;
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.OriginalCodeStr = str;
                    this.Jump(1);
                    return true;
                }
            }
            return idFlag;
        }

        /// <summary>
        /// 关键字的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetReservedCalculator(Token res)
        {
            var okFlag = this.ReservedRouter(res, 13, "`deletepicture");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 12, "`deletecstand", "`deletebutton");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 11, "`msglayeropt", "`endfunction", "*deactivator");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 10, "`titlepoint");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 9, "`stopvocal", "`semaphore", "*activator");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 8, "`waituser", "`msglayer", "`shutdown", "`function", "`snapshot", "*filename");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 7, "`picture", "`waitani", "`stopbgm", "`stopbgs", "`scamera", "`yurimsg", "`chapter", "`bgmfade", "`enabler", "*opacity");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 6, "`button", "`branch", "`switch", "`freeze", "`cstand", "`return", "`endfor", "`notify", "`sysset", "`uipage", "*target", "*normal", "*xscale", "*yscale");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 5, "`label", "`trans", "`endif", "`vocal", "`break", "`alert", "`title", "*state");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 4, "`move", "`jump", "`wait", "`else", "`menu", "`call", "`draw", "`save", "`load", "*name", "*face", "*time", "*link", "*dash", "*cond", "*type", "*over", "*sign");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 3, "`bgm", "`bgs", "`for", "`var", "*vid", "*loc", "*vol", "*acc");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 2, "`se", "`bg", "`if", "*id", "*ro", "*on");
            if (okFlag) { return true; }
            okFlag = this.ReservedRouter(res, 1, "`a", "*x", "*y", "*z");
            if (okFlag) { return true; }
            // 在标识符中没有命中，转到文本内容自动机路径
            return this.GetSceneCluster(res);
        }

        /// <summary>
        /// 处理最长匹配
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <param name="maxLen">最长匹配长度</param>
        /// <param name="reservedList">候选关键字</param>
        /// <returns>是否成功命中</returns>
        private bool ReservedRouter(Token res, int maxLen, params string[] reservedList)
        {
            if (this.nextCharPointer + maxLen <= this.sourceCode.Length)
            {
                // 寻找匹配项
                bool okFlag = false;
                string str = this.sourceCode.Substring(this.nextCharPointer, maxLen).ToLower();
                foreach (string matchStr in reservedList)
                {
                    string pureMatch = matchStr.Substring(1);
                    if (pureMatch == str &&
                        this.GetCharType(this.sourceCode[this.nextCharPointer + maxLen]) != CharacterType.Letter)
                    {
                        // 参数
                        if (matchStr[0] == '*')
                        {
                            res.Type = (TokenType)Enum.Parse(typeof(TokenType), String.Format("Token_p_{0}", pureMatch));
                        }
                        // 动作
                        else
                        {
                            res.Type = (TokenType)Enum.Parse(typeof(TokenType), String.Format("Token_o_{0}", pureMatch));
                        }
                        okFlag = true;
                        break;
                    }
                }
                // 如果命中了符号就返回
                if (okFlag)
                {
                    res.OriginalCodeStr = str;
                    this.Jump(maxLen);
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
        private bool GetIdentifierCalculator(Token res)
        {
            // 跳过变量引用符号$或&或%
            switch (this.GetCharType(this.sourceCode[this.nextCharPointer]))
            {
                case CharacterType.And:
                    res.ScopeFlag = 1;
                    break;
                case CharacterType.Percent:
                    res.ScopeFlag = 2;
                    break;
                default:
                    res.ScopeFlag = 0;
                    break;
            }
            this.Jump(1);
            // 构造标识符
            StringBuilder sb = new StringBuilder();
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
                if (cara == CharacterType.Letter || cara == CharacterType.Number
                    || cara == CharacterType.UnderLine || cara == CharacterType.LeftBrace || cara == CharacterType.RightBrace)
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
            res.IsVar = true;
            res.Type = TokenType.identifier;
            res.OriginalCodeStr = sb.ToString();
            return true;
        }

        /// <summary>
        /// 剧本对白的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool GetSceneCluster(Token res)
        {
            // 跳过左方括弧
            if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.LeftBracket)
            {
                this.Jump(1);
                this.finFlag = false;
            }
            // 构造字符串
            StringBuilder sb = new StringBuilder();
            bool entityFlag = false;
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                this.GetCharType(this.sourceCode[this.nextCharPointer]);
                // 在右方括弧之前的输入都接受，并且这个符号不能是转义的
                if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.RightBracket)
                {
                    // 不跳游程，等待DFA进入段落终止符路径，并标志封闭性成立
                    finFlag = true;
                    break;
                }
                // 如果遇到注释就直接截断，进入注释自动机路径，但封闭性不改变
                else if (this.GetCharType(this.sourceCode[this.nextCharPointer]) == CharacterType.Pound)
                {
                    this.GetNotation();
                    break;
                }
                // 如果是换行符也要压入字符串，但它不可以构成一个实体
                else if (this.sourceCode[this.nextCharPointer] == '\r' || this.sourceCode[this.nextCharPointer] == '\n')
                {
                    // 压入，跳游程，不改变实体性质
                    sb.Append(this.sourceCode[this.nextCharPointer++]);
                }
                else
                {
                    // 处理转义字符并压入字符串构造器，游程在escaping里跳动
                    sb.Append(this.Escaping(this.sourceCode[this.nextCharPointer]));
                    entityFlag = true;
                }
            }
            // 如果成功封闭
            res.Type = TokenType.scenecluster;
            res.Tag = res.OriginalCodeStr = sb.ToString();
            return entityFlag;
        }

        /// <summary>
        /// 剧本对白段落终止符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool EndSceneCluster(Token res)
        {
            // 跳过游程
            this.Jump(1);
            // 修改token信息
            res.Type = TokenType.sceneterminator;
            res.OriginalCodeStr = "]";
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
                this.GetCharType(this.sourceCode[this.nextCharPointer]);
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
            res.Type = TokenType.identifier;
            res.Tag = sb.ToString();
            res.OriginalCodeStr = sb.ToString();
            res.ErrorBit = lattice == false;
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
                var c = this.sourceCode[this.nextCharPointer];
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
                res.Type = TokenType.number;
                res.OriginalCodeStr = sb.ToString();
                res.Tag = Convert.ToInt32(sb.ToString());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 空白符号的自动机路径
        /// </summary>
        /// <returns>是否命中</returns>
        private bool GetSpace()
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
            res.Type = TokenType.unknown;
            res.OriginalCodeStr = Convert.ToString(this.sourceCode[this.nextCharPointer]);
            res.Tag = "错误：不能匹配为token的字符：" + res.OriginalCodeStr;
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
                    ct == CharacterType.RightBracket ||
                    ct == CharacterType.RightBrace ||
                    ct == CharacterType.At ||
                    ct == CharacterType.Pound ||
                    ct == CharacterType.DoubleQuotation ||
                    ct == CharacterType.Quotation)
                {
                    // 跳游程
                    char retChar = this.sourceCode[this.nextCharPointer + 1];
                    this.Jump(2);
                    return retChar;
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
            if (0x4e00 <= c && c <= 0x9fbb) { return CharacterType.Chinese; }
            if ('a' <= c && c <= 'z') { return CharacterType.Letter; }
            if ('A' <= c && c <= 'Z') { return CharacterType.Letter; }
            if ('0' <= c && c <= '9') { return CharacterType.Number; }
            switch (c)
            {
                case ' ':
                    return CharacterType.Space;
                case '_':
                    return CharacterType.UnderLine;
                case '.':
                    return CharacterType.Dot;
                case ',':
                    return CharacterType.Comma;
                case '+':
                    return CharacterType.Plus;
                case '-':
                    return CharacterType.Minus;
                case '*':
                    return CharacterType.Multiply;
                case '/':
                    return CharacterType.Divide;
                case '%':
                    return CharacterType.Percent;
                case '^':
                    return CharacterType.Xor;
                case '&':
                    return CharacterType.And;
                case '|':
                    return CharacterType.Or;
                case '~':
                    return CharacterType.Reverse;
                case '$':
                    return CharacterType.Dollar;
                case '<':
                    return CharacterType.LessThan;
                case '>':
                    return CharacterType.GreaterThan;
                case '(':
                    return CharacterType.LeftParentheses;
                case ')':
                    return CharacterType.RightParentheses;
                case '[':
                    return CharacterType.LeftBracket;
                case ']':
                    return CharacterType.RightBracket;
                case '{':
                    return CharacterType.LeftBrace;
                case '}':
                    return CharacterType.RightBrace;
                case '!':
                    return CharacterType.Not;
                case '#':
                    return CharacterType.Pound;
                case '?':
                    return CharacterType.Question;
                case '"':
                    return CharacterType.DoubleQuotation;
                case ':':
                    return CharacterType.Colon;
                case ';':
                    return CharacterType.Semicolon;
                case '=':
                    return CharacterType.Equality;
                case '@':
                    return CharacterType.At;
                case '\\':
                    return CharacterType.Slash;
                case '\'':
                    return CharacterType.Quotation;
                case '\t':
                    return CharacterType.Space;
                case '\r':
                    return CharacterType.Space;
                case '\n':
                    return CharacterType.Space;
            }
            return CharacterType.cUnknown;
        }

        /// <summary>
        /// 处理的文件
        /// </summary>
        private string dealingFile = String.Empty;

        /// <summary>
        /// 原剧本字串
        /// </summary>
        private string sourceCode = String.Empty;
        
        /// <summary>
        /// 下一字符指针
        /// </summary>
        private int nextCharPointer = 0;

        /// <summary>
        /// 行游标
        /// </summary>
        private int currentLine = 0;
        
        /// <summary>
        /// 列游标
        /// </summary>
        private int currentColumn = 0;

        /// <summary>
        /// 单词流向量
        /// </summary>
        private List<Token> resultVector = null;
        
        /// <summary>
        /// 词法分析完成标志位
        /// </summary>
        private bool finFlag = true;
        
        /// <summary>
        /// 剧本括弧匹配标志位
        /// </summary>
        private bool blockFlag = false;

        /// <summary>
        /// 列初始值
        /// </summary>
        private const int ColStartNum = 0;

        /// <summary>
        /// 行初始值
        /// </summary>
        private const int RowStartNum = 0;

        /// <summary>
        /// 换行符
        /// </summary>
        private const char LineTerminator = '\n';
    }

    /// <summary>
    /// 枚举：字符类型
    /// </summary>
    public enum CharacterType
    {
        /// <summary>
        /// 未知
        /// </summary>
        cUnknown,
        /// <summary>
        /// 字母
        /// </summary>
        Letter,
        /// <summary>
        /// 中文
        /// </summary>
        Chinese,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// _
        /// </summary>
        UnderLine,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// *
        /// </summary>
        Multiply,
        /// <summary>
        /// /
        /// </summary>
        Divide,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// ^
        /// </summary>
        Xor,
        /// <summary>
        /// &amp;
        /// </summary>
        And,
        /// <summary>
        /// |
        /// </summary>
        Or,
        /// <summary>
        /// ~
        /// </summary>
        Reverse,
        /// <summary>
        /// $
        /// </summary>
        Dollar,
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,
        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan,
        /// <summary>
        /// (
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// )
        /// </summary>
        RightParentheses,
        /// <summary>
        /// [
        /// </summary>
        LeftBracket,
        /// <summary>
        /// ]
        /// </summary>
        RightBracket,
        /// <summary>
        /// {
        /// </summary>
        LeftBrace,
        /// <summary>
        /// }
        /// </summary>
        RightBrace,
        /// <summary>
        /// !
        /// </summary>
        Not,
        /// <summary>
        /// #
        /// </summary>
        Pound,
        /// <summary>
        /// \
        /// </summary>
        Slash,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// '
        /// </summary>
        Quotation,
        /// <summary>
        /// "
        /// </summary>
        DoubleQuotation,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
        /// <summary>
        /// =
        /// </summary>
        Equality,
        /// <summary>
        /// @
        /// </summary>
        At,
        /// <summary>
        /// space Tab \r\n
        /// </summary>
        Space
    };
}
