using System;
using System.Collections.Generic;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>场景类：控制一个剧本章节的演出</para>
    /// <para>通常，一个场景拥有一个动作序列和依存她的函数</para>
    /// <para>演绎剧本就是在调用堆栈上遍历这个序列的过程</para>
    /// </summary>
    internal class Scene
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="scenario">场景名称</param>
        /// <param name="mainSa">构造序列</param>
        /// <param name="funcVec">函数向量</param>
        public Scene(string scenario, SceneAction mainSa, List<SceneFunction> funcVec, Dictionary<string, SceneAction> labelDict)
        {
            this.Scenario = scenario;
            this.Ctor = mainSa;
            this.FuncContainer = funcVec;
            this.LabelDictionary = labelDict;
            this.ParallellerContainer = new List<SceneFunction>();
            foreach (var sf in this.FuncContainer)
            {
                if (sf.Callname.StartsWith("sync_", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ParallellerContainer.Add(sf);
                }
            }
        }

        /// <summary>
        /// 获取该场景的IL文件头
        /// </summary>
        /// <returns>IL文件头字符串</returns>
        public string GetILSign()
        {
            return String.Format(">>>YuriIL?{0}", this.Scenario);
        }

        /// <summary>
        /// 获取指定场景的IL文件头
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>IL文件头字符串</returns>
        public static string GetILSign(Scene scene)
        {
            return String.Format(">>>YuriIL?{0}", scene.Scenario);
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>场景的描述字符串</returns>
        public override string ToString()
        {
            return String.Format("Scene: {0} (func:{1} with para:{2})",
                this.Scenario, this.FuncContainer.Count, this.ParallellerContainer.Count);
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string Scenario { get; set; }

        /// <summary>
        /// 场景的构造序列
        /// </summary>
        public SceneAction Ctor { get; set; }

        /// <summary>
        /// 场景的函数向量
        /// </summary>
        public List<SceneFunction> FuncContainer { get; set; }

        /// <summary>
        /// 场景内的并行处理器向量
        /// </summary>
        public List<SceneFunction> ParallellerContainer { get; set; }

        /// <summary>
        /// 场景标签字典
        /// </summary>
        public Dictionary<string, SceneAction> LabelDictionary { get; set; }
    }
}
