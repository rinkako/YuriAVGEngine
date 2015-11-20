using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 单词类：负责装填词法分析阶段的结果的最小单元
    /// </summary>
    internal sealed class Token
    {
        // 命中行
        public int aLine = -1;
        // 命中列
        public int aColumn = -1;
        // 位置戳
        public int indexOfCode = -1;
        // 配对长
        public int length = 0;
        // 附加值
        public object aTag = null;
        // 原字串
        public string detail = "";
        // 错误位
        public bool errorBit = false;
        // 词类型
        public TokenType aType = TokenType.Token_NOP;
    }
}
