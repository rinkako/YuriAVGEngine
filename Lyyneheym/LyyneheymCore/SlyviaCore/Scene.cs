using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LyyneheymCore.SlyviaPile;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>场景类：控制一个剧本章节的演出</para>
    /// <para>通常，一个场景拥有一个动作序列和生命在它上面的函数</para>
    /// <para>演绎剧本的过程就是遍历这个序列的过程</para>
    /// </summary>
    [Serializable]
    public class Scene
    {
        public Scene(string scenario, SceneAction mainSa, List<SceneFunction> funcVec)
        {
            this.scenario = scenario;
            this.mainSa = mainSa;
            this.funcContainer = funcVec;
        }


        // 场景进行指针
        public int SP = 0;
        // 场景名称
        public string scenario;
        // 场景的主动作序列
        public SceneAction mainSa;
        // 场景的函数向量
        public List<SceneFunction> funcContainer;
    }
}
