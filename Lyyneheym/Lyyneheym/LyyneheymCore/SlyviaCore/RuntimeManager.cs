using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

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

        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WMouseDownEventHandler(MouseButtonEventArgs e)
        {
            // 修改全局状态
            RuntimeManager.KS_MOUSE_LEFT = e.LeftButton == MouseButtonState.Pressed;
            RuntimeManager.KS_MOUSE_RIGHT = e.RightButton == MouseButtonState.Pressed;
            RuntimeManager.KS_MOUSE_MID = e.MiddleButton == MouseButtonState.Pressed;
            // 传递给演绎器
            this.updateRender.WMouseDownEventHandler(e);
        }

        public void WMouseUpEventHandler(MouseButtonEventArgs e)
        {
            // 修改全局状态
            RuntimeManager.KS_MOUSE_LEFT = e.LeftButton == MouseButtonState.Pressed;
            RuntimeManager.KS_MOUSE_RIGHT = e.RightButton == MouseButtonState.Pressed;
            RuntimeManager.KS_MOUSE_MID = e.MiddleButton == MouseButtonState.Pressed;
            // 传递给演绎器
            this.updateRender.WMouseUpEventHandler(e);

        }

        public void SetMWReference(MainWindow mw)
        {
            this.updateRender.SetPlatformReference(mw);
        }

        /// <summary>
        /// 游戏刷新器
        /// </summary>
        private UpdateRender updateRender;
        
        /// <summary>
        /// 游戏调用堆栈
        /// </summary>
        private StackMachine stackMachine;

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

        #region 键位按钮状态
        public static bool KS_MOUSE_LEFT = false;
        public static bool KS_MOUSE_RIGHT = false;
        public static bool KS_MOUSE_MID = false;
        public static bool KS_KEY_ESC = false;
        public static bool KS_KEY_SHIFT = false;
        public static bool KS_KEY_CTRL = false;
        public static bool KS_KEY_TAB = false;
        public static bool KS_KEY_SPACE = false;
        public static bool KS_KEY_Z = false;
        public static bool KS_KEY_X = false;
        public static bool KS_KEY_W = false;
        public static bool KS_KEY_S = false;
        public static bool KS_KEY_A = false;
        public static bool KS_KEY_D = false;
        public static bool KS_KEY_ENTER = false;
        public static bool KS_KEY_UP = false;
        public static bool KS_KEY_DOWN = false;
        public static bool KS_KEY_LEFT = false;
        public static bool KS_KEY_RIGHT = false;
        #endregion

        #region 自身相关方法
        /// <summary>
        /// 将运行时环境恢复最初状态
        /// </summary>
        public void Reset()
        {
            this.stackMachine = new StackMachine();
            this.updateRender = new UpdateRender();
        }

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
            this.Reset();
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static RuntimeManager synObject = null;
        #endregion
    }
}
