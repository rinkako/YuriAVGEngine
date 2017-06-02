using System;
using System.Collections.Generic;
using System.Linq;
using Yuri.Utils;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 可求值上下文类
    /// </summary>
    [Serializable]
    internal abstract class EvaluatableContext : IRuntimeContext
    {
        /// <summary>
        /// 在此上下文中申请一个变量来储存指定对象的引用，如果指定变量名已存在，将覆盖原有的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="varObj">要存入的对象引用</param>
        public virtual void Assign(string varName, object varObj)
        {
            lock (this)
            {
                this.symbols[varName] = varObj;
            }
        }

        /// <summary>
        /// 从此上下文移除一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>是否移除成功（变量原本是否存在）</returns>
        public virtual bool Remove(string varName)
        {
            lock (this)
            {
                return this.symbols.Remove(varName);
            }
        }

        /// <summary>
        /// 查找此上下文中是否存在某个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量是否存在</returns>
        public virtual bool Exist(string varName)
        {
            lock (this)
            {
                return this.symbols.ContainsKey(varName);
            }
        }

        /// <summary>
        /// 从此上下文中取一个变量名对应的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量代表的对象引用</returns>
        public virtual object Fetch(string varName)
        {
            lock (this)
            {
                if (this.symbols.ContainsKey(varName))
                {
                    return this.symbols[varName];
                }
                LogUtils.LogLine("变量 " + varName + " 在作为左值之前被引用", "EvaluatableContext", LogLevel.Error);
                throw new NullReferenceException("变量 " + varName + " 在作为左值之前被引用");
            }
        }

        /// <summary>
        /// 清空此上下文
        /// </summary>
        public virtual void Clear()
        {
            lock (this)
            {
                this.symbols.Clear();
            }
        }

        /// <summary>
        /// 使用指定的筛选谓词查找此上下文符合条件的变量名
        /// </summary>
        /// <param name="varNamePred">变量名筛选谓词</param>
        /// <returns>满足约束的键值对</returns>
        public virtual List<KeyValuePair<string, object>> GetSymbols(Predicate<string> varNamePred = null)
        {
            lock (this)
            {
                return varNamePred == null ? this.symbols.ToList() : this.symbols.Where(kvp => varNamePred(kvp.Key)).ToList();
            }
        }

        /// <summary>
        /// 获取或设置该上下文的命名空间
        /// </summary>
        public string ContextNamespace { get; set; } = String.Empty;

        /// <summary>
        /// 上下文符号表
        /// </summary>
        protected internal Dictionary<string, object> symbols = new Dictionary<string, object>();
    }
}
