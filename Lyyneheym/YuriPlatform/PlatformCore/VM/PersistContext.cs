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
        /// <param name="filename">文件路径</param>
        public void SaveToSteadyMemory(string filename)
        {
            lock (this)
            {
                IOUtils.Serialization(this.symbols, filename);
            }
        }

        /// <summary>
        /// 从稳定储存器将持久上下文读入内存
        /// </summary>
        /// <param name="filename">文件路径</param>
        public void LoadFromSteadyMemory(string filename)
        {
            lock (this)
            {
                this.symbols = IOUtils.Unserialization(filename) as Dictionary<string, object>;
            }
        }

        /// <summary>
        /// 从此上下文中取一个变量名对应的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量代表的对象引用</returns>
        public override object Fetch(string varName)
        {
            lock (this)
            {
                return this.symbols.ContainsKey(varName) ? this.symbols[varName] : (double)0;
            }
        }
    }
}
