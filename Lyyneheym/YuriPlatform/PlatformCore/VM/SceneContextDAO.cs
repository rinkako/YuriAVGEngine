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
        /// 获取当前场景上下文集合是否包含指定场景
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <returns>是否已有该场景对应的上下文</returns>
        public bool ExistScene(string sceneName)
        {
            return this.userSymbolTableContainer.ContainsKey(sceneName);
        }

        /// <summary>
        /// 为指定的场景创建上下文
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        public void CreateSceneContext(string sceneName)
        {
            this.userSymbolTableContainer.Add(sceneName, new SaveableContext(sceneName));
        }

        /// <summary>
        /// 清除指定场景上下文中的内容
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <returns>是否存在这个场景并清除成功</returns>
        public bool ClearSceneContext(string sceneName)
        {
            if (this.ExistScene(sceneName))
            {
                this.userSymbolTableContainer[sceneName].Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将一个变量赋值，如果变量不存在，将被注册后再赋值
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名称</param>
        /// <param name="value">变量的值</param>
        public void Assign(string sceneName, string varName, object value)
        {
            var ctx = this.FindSceneContext(sceneName);
            ctx.Assign(varName, value);
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
            var ctx = this.FindSceneContext(sceneObject.Scenario);
            // 如果查无此键
            if (ctx.Exist(varName) == false)
            {
                LogUtils.LogLine("变量 " + varName + " 在作为左值之前被引用", "SymbolTable", LogLevel.Error);
                throw new NullReferenceException("变量 " + varName + " 在作为左值之前被引用");
            }
            return ctx.Fetch(varName);
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
        public SaveableContext FindSceneContext(string sceneName)
        {
            return this.userSymbolTableContainer.ContainsKey(sceneName)
                ? this.userSymbolTableContainer[sceneName] : null;
        }
        
        /// <summary>
        /// 场景符号上下文
        /// </summary>
        private readonly Dictionary<string, SaveableContext> userSymbolTableContainer = new Dictionary<string, SaveableContext>();
    }
}
