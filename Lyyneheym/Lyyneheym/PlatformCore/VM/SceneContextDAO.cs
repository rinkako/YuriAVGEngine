using System;
using System.Collections.Generic;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 场景上下文DAO：为游戏全局所有场景的上下文提供包装
    /// </summary>
    [Serializable]
    internal sealed class SceneContextDAO : ForkableState
    {
        /// <summary>
        /// 将一个变量赋值，如果变量不存在，将被注册后再赋值
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名称</param>
        /// <param name="value">变量的值</param>
        public void Assign(string sceneName, string varName, object value)
        {
            var table = this.FindSymbolTable(sceneName);
            table[varName] = value;
        }
       
        /// <summary>
        /// <para>使用一个变量作为右值</para>
        /// <para>如果这个变量从未使用过，将抛出错误</para>
        /// </summary>
        /// <param name="sceneObject">场景实例</param>
        /// <param name="varName">变量名</param>
        /// <returns>变量在运行时环境的引用</returns>
        public object Fetch(Scene sceneObject, string varName)
        {
            var table = this.FindSymbolTable(sceneObject.Scenario);
            // 如果查无此键
            if (table.ContainsKey(varName) == false)
            {
                CommonUtils.ConsoleLine("变量 " + varName + " 在作为左值之前被引用", "SymbolTable", OutputStyle.Error);
                throw new NullReferenceException("变量 " + varName + " 在作为左值之前被引用");
            }
            return table[varName];
        }

        /// <summary>
        /// 清空所有场景的上下文
        /// </summary>
        public void Clear()
        {
            foreach (var sctx in this.userSymbolTableContainer)
            {
                sctx.Value.Clear();
            }
        }
        
        /// <summary>
        /// 寻找当前场景作用域绑定的符号表
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns>符号表</returns>
        private Dictionary<string, object> FindSymbolTable(string sceneName)
        {
            return this.userSymbolTableContainer.ContainsKey(sceneName)
                ? this.userSymbolTableContainer[sceneName]
                : null;
        }

        /// <summary>
        /// 场景符号表
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> userSymbolTableContainer = null;
    }
}
