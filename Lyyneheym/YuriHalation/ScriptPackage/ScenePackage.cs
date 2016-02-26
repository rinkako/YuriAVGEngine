using System;
using System.Collections.Generic;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 场景包装类
    /// </summary>
    [Serializable]
    class ScenePackage : RunnablePackage
    {
        /// <summary>
        /// 创建一个指定名称的场景
        /// </summary>
        /// <param name="scenario">场景名</param>
        public ScenePackage(string scenario)
        {
            this.sceneName = scenario;
        }

        /// <summary>
        /// 增加一个函数
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <returns>操作成功与否</returns>
        public bool AddFunction(string funcName)
        {
            if (this.funcList.Find((x) => x.functionName == funcName) != null)
            {
                return false;
            }
            this.funcList.Add(new FunctionPackage(funcName, this.sceneName));
            return true;
        }

        /// <summary>
        /// 删除一个函数
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <returns>操作成功与否</returns>
        public bool DeleteFunction(string funcName)
        {
            var removeOne = this.funcList.Find((x) => x.functionName == funcName);
            if (removeOne != null)
            {
                this.funcList.Remove(removeOne);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个函数包装
        /// </summary>
        /// <param name="name">函数名</param>
        /// <returns>函数包装实例</returns>
        public FunctionPackage GetFunc(string name)
        {
            return this.funcList.Find((x) => x.functionName == name);
        }
        
        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>场景的名字和行数</returns>
        public override string ToString()
        {
            return String.Format("Scene: {0} ({1} lines, {2} funcs)",
                this.sceneName, this.APListCount(), funcList.Count);
        }

        /// <summary>
        /// 场景的名称
        /// </summary>
        public string sceneName = "";

        /// <summary>
        /// 函数向量
        /// </summary>
        private List<FunctionPackage> funcList = new List<FunctionPackage>();
    }
}
