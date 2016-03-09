using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>场景类：控制一个剧本章节的演出</para>
    /// <para>通常，一个场景拥有一个动作序列和生命在它上面的函数</para>
    /// <para>演绎剧本的过程就是遍历这个序列的过程</para>
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="scenario">场景名称</param>
        /// <param name="mainSa">主动作序列</param>
        /// <param name="funcVec">函数向量</param>
        public Scene(string scenario, SceneAction mainSa, List<SceneFunction> funcVec, Dictionary<string, SceneAction> labelDict)
        {
            this.scenario = scenario;
            this.mainSa = mainSa;
            this.funcContainer = funcVec;
            this.labelDictionary = labelDict;
        }

        /// <summary>
        /// 获取该场景的IL文件头
        /// </summary>
        /// <returns>IL文件头字符串</returns>
        public string GetILSign()
        {
            return String.Format(">>>YuriIL?{0}", this.scenario);
        }

        /// <summary>
        /// 获取指定场景的IL文件头
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>IL文件头字符串</returns>
        public static string GetILSign(Scene scene)
        {
            return String.Format(">>>YuriIL?{0}", scene.scenario);
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string scenario;

        /// <summary>
        /// 场景的主动作序列
        /// </summary>
        public SceneAction mainSa;

        /// <summary>
        /// 场景的函数向量
        /// </summary>
        public List<SceneFunction> funcContainer;

        /// <summary>
        /// 场景标签字典
        /// </summary>
        public Dictionary<string, SceneAction> labelDictionary;
    }
}
