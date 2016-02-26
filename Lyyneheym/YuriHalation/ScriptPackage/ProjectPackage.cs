using System;
using System.Collections.Generic;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 工程包装类
    /// </summary>
    [Serializable]
    class ProjectPackage
    {
        /// <summary>
        /// 建立一个工程
        /// </summary>
        /// <param name="proj">工程名</param>
        public ProjectPackage(string proj)
        {
            this.projectName = proj;
        }

        /// <summary>
        /// 增加一个场景
        /// </summary>
        /// <param name="scenario">场景名</param>
        /// <returns>操作成功与否</returns>
        public bool AddScene(string scenario)
        {
            if (this.sceneDict.ContainsKey(scenario))
            {
                return false;
            }
            sceneDict.Add(scenario, new ScenePackage(scenario));
            return true;
        }

        /// <summary>
        /// 删除一个场景
        /// </summary>
        /// <param name="scenario">场景名</param>
        /// <returns>操作成功与否</returns>
        public bool DeleteScene(string scenario)
        {
            if (this.sceneDict.ContainsKey(scenario))
            {
                this.sceneDict.Remove(scenario);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个场景
        /// </summary>
        /// <param name="scenario">场景名</param>
        /// <returns>场景实例</returns>
        public ScenePackage GetScene(string scenario)
        {
            if (this.sceneDict.ContainsKey(scenario))
            {
                return this.sceneDict[scenario];
            }
            return null;
        }

        /// <summary>
        /// 全局变量向量
        /// </summary>
        private List<VariablePackage> globalVarList = new List<VariablePackage>();

        /// <summary>
        /// 场景字典
        /// </summary>
        private Dictionary<string, ScenePackage> sceneDict = new Dictionary<string, ScenePackage>();

        /// <summary>
        /// 工程名称
        /// </summary>
        private string projectName = "";
    }
}
