using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuriHalation.ScriptPackage
{
    /// <summary>
    /// 函数包装类
    /// </summary>
    [Serializable]
    class FunctionPackage
    {
        /// <summary>
        /// 函数名
        /// </summary>
        public string functionName = "";

        /// <summary>
        /// 所在场景的名称
        /// </summary>
        public string parentName = "";

        /// <summary>
        /// 动作向量
        /// </summary>
        public List<ActionPackage> APList = new List<ActionPackage>();

        /// <summary>
        /// 函数内变量向量
        /// </summary>
        public List<VariablePackage> varList = new List<VariablePackage>();
    }
}
