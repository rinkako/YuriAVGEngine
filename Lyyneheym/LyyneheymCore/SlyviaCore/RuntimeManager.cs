using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>运行时管理器：维护运行时的所有信息</para>
    /// <para>她是一个单例类，只有唯一实例，并且可以序列化</para>
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    public class RuntimeManager
    {










        #region 全局状态控制



        /// <summary>
        /// 游戏的总体状态
        /// </summary>
        public enum GameState
        {
            // 游戏剧情进行时
            Performing,
            // 用户操作界面
            UserPanel,
            // 系统执行中
            Loading
        }

        /// <summary>
        /// 游戏的稳态
        /// </summary>
        public enum GameStableState
        {
            // 等待用户操作的稳态
            Stable,
            // 用户等待系统的不稳态
            Unstable,
            // 当前状态不明确
            Unknown
        }
        #endregion

        #region 单例相关方法
        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>运行时管理器</returns>
        public static RuntimeManager getInstance()
        {
            return null == synObject ? synObject = new RuntimeManager() : synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private RuntimeManager()
        {

        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static RuntimeManager synObject = null;
        #endregion
    }
}
