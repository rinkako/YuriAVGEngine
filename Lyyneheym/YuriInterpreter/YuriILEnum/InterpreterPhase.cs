namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：编译过程
    /// </summary>
    public enum InterpreterPhase
    {
        /// <summary>
        /// 词法分析
        /// </summary>
        Lexer,
        /// <summary>
        /// 语法分析
        /// </summary>
        Parser,
        /// <summary>
        /// 语义分析
        /// </summary>
        Sematicer,
        /// <summary>
        /// 代码优化
        /// </summary>
        Optimizer,
        /// <summary>
        /// IL生成
        /// </summary>
        ILGenerator
    }
}