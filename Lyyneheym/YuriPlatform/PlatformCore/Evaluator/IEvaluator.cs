using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Evaluator
{
    /// <summary>
    /// 接口：表达式求值器
    /// </summary>
    internal interface IEvaluator
    {
        /// <summary>
        /// 计算表达式的值
        /// </summary>
        /// <param name="expr">表达式字符串</param>
        /// <param name="ctx">求值上下文</param>
        /// <returns>计算结果的值（Double/字符串）</returns>
        object Eval(string expr, EvaluatableContext ctx);

        /// <summary>
        /// 计算表达式的真值
        /// </summary>
        /// <param name="polish">表达式字符串</param>
        /// <param name="ctx">求值上下文</param>
        /// <returns>表达式的真值</returns>
        bool EvalBoolean(string polish, EvaluatableContext ctx);
    }
}