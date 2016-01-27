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
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="scenario">场景名称</param>
        /// <param name="mainSa">主动作序列</param>
        /// <param name="funcVec">函数向量</param>
        public Scene(string scenario, SceneAction mainSa, List<SceneFunction> funcVec)
        {
            this.scenario = scenario;
            this.mainSa = mainSa;
            this.funcContainer = funcVec;
        }

        /// <summary>
        /// 初始化该场景
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 为消息队列更新当前场景的数据
        /// </summary>
        public void UpdateContext()
        {

        }

        /// <summary>
        /// 渲染当前画面
        /// </summary>
        public void Render()
        {

        }

        /// <summary>
        /// 获取该场景的IL文件头
        /// </summary>
        /// <returns>IL文件头字符串</returns>
        public string GetILSign()
        {
            return String.Format(">>>SlyviaIL?{0}", this.scenario);
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
