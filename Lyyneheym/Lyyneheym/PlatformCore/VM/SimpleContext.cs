using System;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 简单上下文类：提供基本的上下文服务
    /// </summary>
    [Serializable]
    internal sealed class SimpleContext : EvaluatableContext
    {
        /// <summary>
        /// 创建一个简单上下文
        /// </summary>
        /// <param name="nameSpace">上下文的命名空间</param>
        public SimpleContext(string nameSpace)
        {
            this.ContextNamespace = nameSpace;
        }
    }
}
