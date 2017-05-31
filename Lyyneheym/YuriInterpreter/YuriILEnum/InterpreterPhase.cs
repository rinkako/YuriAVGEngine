namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：编译过程
    /// </summary>
    public enum InterpreterPhase
    {
        Lexer,
        Parser,
        Sematicer,
        Optimizer,
        ILGenerator
    }
}