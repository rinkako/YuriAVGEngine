using System;
using System.Collections.Generic;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 函数包装类
    /// </summary>
    [Serializable]
    class FunctionPackage : RunnablePackage
    {
        /// <summary>
        /// 创建一个指定函数名的函数
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parent">上级场景</param>
        /// <param name="argv">形参列表</param>
        public FunctionPackage(string funcName, ScenePackage parent, List<string> argv)
        {
            this.functionName = funcName;
            this.parent = parent;
            this.Argv = argv;
        }

        /// <summary>
        /// 形参列表
        /// </summary>
        public List<string> Argv;

        /// <summary>
        /// 函数名
        /// </summary>
        public string functionName;

        /// <summary>
        /// 所在场景
        /// </summary>
        public ScenePackage parent;

        /// <summary>
        /// 调用名
        /// </summary>
        public string functionCallName
        {
            get
            {
                return String.Format("{0}@{1}", functionName, parent.sceneName);
            }
        }
    }
}
