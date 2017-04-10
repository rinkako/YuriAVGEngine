using System;
using System.Collections.Generic;
using Yuri.Utils;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 持久类：为游戏提供持久性上下文，它不会被回滚和存读档影响
    /// </summary>
    internal static class PersistenceContext
    {
        /// <summary>
        /// 保存持久上下文到稳定储存器
        /// </summary>
        public static void SaveToSteadyMemory() => IOUtils.Serialization(PersistenceContext.symbols, GlobalConfigContext.PersistenceFileName);

        /// <summary>
        /// 从稳定储存器将持久上下文读入内存
        /// </summary>
        public static void LoadFromSteadyMemory() => PersistenceContext.symbols =
            IOUtils.Unserialization(GlobalConfigContext.PersistenceFileName) as Dictionary<string, object>;

        /// <summary>
        /// 将一个变量放入持久容器中，如果指定变量名已存在，就覆盖原来的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="varObj">要存入的对象</param>
        public static void Assign(string varName, object varObj) => PersistenceContext.symbols[varName] = varObj;
        
        /// <summary>
        /// 从符号表里移除一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>是否移除成功（变量原本是否存在）</returns>
        public static bool Remove(string varName) => PersistenceContext.symbols.Remove(varName);

        /// <summary>
        /// 查找符号表中是否存在某个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量是否存在</returns>
        public static bool Exist(string varName) => PersistenceContext.symbols.ContainsKey(varName);

        /// <summary>
        /// 从持久容器中取一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量的引用</returns>
        public static object Fetch(string varName)
        {
            if (PersistenceContext.symbols.ContainsKey(varName))
            {
                return PersistenceContext.symbols[varName];
            }
            CommonUtils.ConsoleLine("持久化变量 " + varName + " 在作为左值之前被引用", "PersistenceContext", OutputStyle.Error);
            throw new NullReferenceException("持久化变量 " + varName + " 在作为左值之前被引用");
        }
        
        /// <summary>
        /// 持久符号表
        /// </summary>
        private static Dictionary<string, object> symbols = new Dictionary<string, object>();
    }
}
