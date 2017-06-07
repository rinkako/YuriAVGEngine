namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：变量作用域
    /// </summary>
    public enum VarScopeType
    {
        /// <summary>
        /// 非变量
        /// </summary>
        NOTVAR,
        /// <summary>
        /// 局部变量
        /// </summary>
        LOCAL,
        /// <summary>
        /// 全局变量
        /// </summary>
        GLOBAL,
        /// <summary>
        /// 持久性变量
        /// </summary>
        PERSIST
    }
}