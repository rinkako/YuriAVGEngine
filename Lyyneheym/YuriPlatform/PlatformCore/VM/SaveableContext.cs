using System;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 可保存上下文，这类上下文可以被序列化
    /// </summary>
    [Serializable]
    internal sealed class SaveableContext : EvaluatableContext
    {
        /// <summary>
        /// 构造一个可保存上下文
        /// </summary>
        /// <param name="scenario">上下文所绑定的场景名</param>
        public SaveableContext(string scenario)
        {
            this.ContextNamespace = this.BindingScene = scenario;
        }

        /// <summary>
        /// 获取或设置该上下文所绑定的场景名字
        /// </summary>
        public string BindingScene { get; set; }
    }
}
