using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuriHalation;
using Yuri.YuriHalation.ScriptPackage;
using Yuri.YuriForms;

namespace Yuri
{
    /// <summary>
    /// 控制器类：负责前端与后台的交互
    /// </summary>
    internal sealed class Halation
    {

        #region 工程实例的引用
        /// <summary>
        /// 获取或设置当前工程包装
        /// </summary>
        public static ProjectPackage project { get; set; }
        #endregion

        #region 各模块的引用
        /// <summary>
        /// 为程序各模块设置更新主窗体的引用
        /// </summary>
        /// <param name="mw">主窗体实例</param>
        public static void SetViewReference(MainForm mw)
        {
            Halation.mainView = mw;
        }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        public static MainForm mainView = null;
        #endregion

        #region 类自身相关
        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>Halation实例</returns>
        public static Halation GetInstance()
        {
            return Halation.synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Halation()
        {

        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static readonly Halation synObject = new Halation();
        #endregion
    }
}
