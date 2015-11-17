using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 单词类：负责装填词法分析阶段的结果的最小单元
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iType">单词类型</param>
        /// <param name="iStr">原字串内容</param>
        public Token(TokenType iType, string iStr)
        {
            this.aType = iType;
            this.detail = iStr;
        }

        // 词类型
        TokenType aType = TokenType.unknown;
        // 命中行
        public int aLine = -1;
        // 命中列
        public int aColumn = -1;
        // 位置戳
        public int indexOfCode = -1;
        // 配对长
        public int length = 0;
        // 附加值
        public string aTag = "";
        // 原字串
        public string detail = "";
    }

    /// <summary>
    /// 枚举：单词类型
    /// </summary>
    public enum TokenType
    {
        // 未知的单词符号
        unknown,
        // null
        epsilon,
        // 常数
        number,
        // #
        token_startEnd,
        // 标识符
        identifier

        /* For Implementation */
    }
}
