using System.Collections.Generic;
using Yuri.Utils;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 持久化上下文：不会被回滚和存档影响的上下文
    /// </summary>
    internal sealed class PersistContext : EvaluatableContext
    {
        /// <summary>
        /// 保存持久上下文到稳定储存器
        /// </summary>
        /// <param name="filename">文件名字</param>
        public void SaveToSteadyMemory(string filename)
        {
            IOUtils.Serialization(this.symbols, filename);
        }

        /// <summary>
        /// 从稳定储存器将持久上下文读入内存
        /// </summary>
        /// <param name="filename">文件名字</param>
        public void LoadFromSteadyMemory(string filename)
        {
            this.symbols = IOUtils.Unserialization(filename) as Dictionary<string, object>;
        }
    }
}
