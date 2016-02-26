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
        /// <param name="parentName">上级场景名</param>
        public FunctionPackage(string funcName, string parentName)
        {
            this.functionName = funcName;
            this.parentName = parentName;
        }

        /// <summary>
        /// 函数名
        /// </summary>
        public string functionName = "";

        /// <summary>
        /// 所在场景的名称
        /// </summary>
        public string parentName = "";
    }
}
