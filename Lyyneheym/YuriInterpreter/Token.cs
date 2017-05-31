using System;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
{
    /// <summary>
    /// 单词类：负责装填词法分析阶段的结果的最小单元
    /// </summary>
    internal sealed class Token
    {
        /// <summary>
        /// 命中行
        /// </summary>
        public int Line = -1;

        /// <summary>
        /// 命中列
        /// </summary>
        public int Column = -1;

        /// <summary>
        /// 位置戳
        /// </summary>
        public int IndexOfCode = -1;

        /// <summary>
        /// 配对长
        /// </summary>
        public int Length = 0;

        /// <summary>
        /// 附加值
        /// </summary>
        public object Tag = null;

        /// <summary>
        /// 原字串
        /// </summary>
        public string OriginalCodeStr = String.Empty;

        /// <summary>
        /// 错误位
        /// </summary>
        public bool ErrorBit = false;

        /// <summary>
        /// 变量位
        /// </summary>
        public bool IsVar = false;

        /// <summary>
        /// 作用域(0局部，1全局，2持久)
        /// </summary>
        public int ScopeFlag = 0;

        /// <summary>
        /// 词类型
        /// </summary>
        public TokenType Type = TokenType.Token_NOP;

        /// <summary>
        /// 重载字符串化方法
        /// </summary>
        /// <returns>Token的简要信息</returns>
        public override string ToString()
        {
            return "Token: " + Type + " (" + this.OriginalCodeStr + ") -> " + (Tag?.ToString() ?? "null");
        }
    }
}
