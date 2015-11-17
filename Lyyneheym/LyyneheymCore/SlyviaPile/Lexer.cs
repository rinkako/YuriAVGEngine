using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 词法分析器类：负责把用户脚本分割为单词流的类
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scenario">待分析的剧本文本</param>
        public Lexer(string scenario)
        {
            this.sourceCode = scenario;
        }

        /// <summary>
        /// 启动词法分析DFA自动机
        /// </summary>
        /// <returns>词法分析完毕的的单词流</returns>
        public List<Token> DFA()
        {
            List<Token> resultVec = new List<Token>();
            /* for implementation */
            return resultVec;
        }

        /// <summary>
        /// <para>DFA自动机</para>
        /// <para>处理当前代码指针所在的单词，并移动指针</para>
        /// <para>如果遇到未知的字符，将抛出错误</para>
        /// </summary>
        /// <returns>当前代码指针所指单词实例</returns>
        private Token NextToken()
        {
            /* for implementation */
            return null;
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

        /// <summary>
        /// 枚举：字符类型
        /// </summary>
        private enum CharacterType
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
}
