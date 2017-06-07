namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：编译结果的类型
    /// </summary>
    public enum InterpreterType
    {
        /// <summary>
        /// DEBUG模式
        /// </summary>
        DEBUG,
        /// <summary>
        /// 生成IL
        /// </summary>
        RELEASE_WITH_IL,
        /// <summary>
        /// 生成二进制场景实例
        /// </summary>
        RELEASE_WITH_BINARY_SCENE
    }
}