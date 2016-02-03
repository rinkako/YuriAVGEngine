using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Lyyneheym.LyyneheymCore.Utils;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 负责将场景动作转化为前端事物的类
    /// </summary>
    public class UpdateRender
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public UpdateRender()
        {

        }
        
        /// <summary>
        /// 设置主窗体引用
        /// </summary>
        /// <param name="mw">主窗体引用</param>
        public void SetPlatformReference(MainWindow mw)
        {
            this.view = mw;
        }

        /// <summary>
        /// 接受一个场景动作并演绎她
        /// </summary>
        /// <param name="action">场景动作实例</param>
        public void Accept(SceneAction action)
        {

        }

        /// <summary>
        /// 处理游戏窗体的鼠标按下信息
        /// </summary>
        /// <param name="e"></param>
        public void WMouseDownEventHandler(MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (this.view.BO_MessageBoxLayer.Visibility == System.Windows.Visibility.Hidden)
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            
        }

        public void WMouseUpEventHandler(MouseButtonEventArgs e)
        {

        }

        #region 文字层相关
        /// <summary>
        /// 隐藏小三角
        /// </summary>
        public void HideMessageTria()
        {
            this.view.BO_MsgTria.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 显示小三角
        /// </summary>
        /// <param name="opacity">透明度</param>
        public void ShowMessageTria(double opacity = 1.0f)
        {
            this.view.BO_MsgTria.Opacity = opacity;
            this.view.BO_MsgTria.Visibility = Visibility.Visible;
        }

        public void BeginMessageTriaUpDownAnimation()
        {

        }

        public void EndMessageTriaUpDownAnimation()
        {

        }

        /// <summary>
        /// 设置小三角位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetMessageTriaPosition(Point pos)
        {
            Canvas.SetLeft(this.view.BO_MsgTria, pos.X);
            Canvas.SetTop(this.view.BO_MsgTria, pos.Y);
        }
        #endregion

        /// <summary>
        /// 为更新器设置作用窗体
        /// </summary>
        /// <param name="mw">窗体引用</param>
        public void SetMainWindow(MainWindow mw)
        {
            this.view = mw;
        }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        private MainWindow view = null;

        #region 演绎函数
        private void Dialog()
        {

        }

        private void DialogTerminator()
        {

        }

        private void A()
        {

        }

        private void Picture()
        {

        }

        private void Move()
        {

        }

        private void Deletepicture()
        {

        }

        private void Cstand()
        {

        }

        private void Deletecstand()
        {

        }

        private void Se()
        {

        }
 
        private void Bgm()
        {

        }

        private void Stopbgm()
        {

        }

        private void Vocal()
        {

        }

        private void Stopvocal()
        {

        }

        private void Title()
        {

        }

        private void Menu()
        {

        }

        private void Save()
        {

        }

        private void Load()
        {

        }

        private void Lable()
        {

        }

        private void Jump()
        {

        }

        private void For()
        {

        }

        private void Endfor()
        {

        }

        private void If()
        {

        }

        private void Function()
        {

        }

        private void Endfunction()
        {

        }

        private void Var()
        {

        }

        private void Break()
        {

        }

        private void Shutdown()
        {

        }

        private void Wait()
        {

        }

        private void Branch()
        {

        }

        private void Call()
        {

        }

        private void Titlepoint()
        {

        }
        #endregion

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
    }
}
