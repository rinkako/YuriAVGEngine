using System;
using System.Collections.Generic;

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

        /// <summary>
        /// 从指定字典创建一个简单上下文
        /// </summary>
        /// <param name="ctxDict">要拷贝的数据源字典</param>
        public SimpleContext(Dictionary<string, object> ctxDict)
        {
            if (ctxDict != null)
            {
                foreach (var kvp in ctxDict)
                {
                    this.symbols[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}
