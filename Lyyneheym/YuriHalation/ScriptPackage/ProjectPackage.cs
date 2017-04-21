using System;
using System.Collections.Generic;
using Yuri.YuriHalation.Command;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 工程包装类
    /// </summary>
    [Serializable]
    internal class ProjectPackage
    {
        /// <summary>
        /// 建立一个工程
        /// </summary>
        /// <param name="proj">工程名</param>
        public ProjectPackage(string proj)
        {
            this.projectName = proj;
            for (int i = 0; i < this.Config.GameMaxSwitchCount; i++)
            {
                this.SwitchDescriptorList.Add(String.Empty);
            }
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
            HalationInvoker.AddScene(scenario);
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
        /// 获取所有场景并按字典序排序
        /// </summary>
        /// <returns>场景向量</returns>
        public List<ScenePackage> GetScene()
        {
            List<ScenePackage> resVect = new List<ScenePackage>();
            foreach (var sc in this.sceneDict)
            {
                resVect.Add(sc.Value);
            }
            resVect.Sort(delegate(ScenePackage x, ScenePackage y) { return x.sceneName.CompareTo(y.sceneName); });
            return resVect;
        }

        /// <summary>
        /// 游戏设置信息
        /// </summary>
        public ConfigPackage Config = new ConfigPackage();

        /// <summary>
        /// 角色向量
        /// </summary>
        public List<string> CharacterList = new List<string>();
            
        /// <summary>
        /// 开关描述向量
        /// </summary>
        public List<string> SwitchDescriptorList = new List<string>();

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
        private string projectName = String.Empty;
    }
}
