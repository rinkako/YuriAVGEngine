using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using Yuri.PlatformCore;
using Yuri.ILPackage;
using Yuri.YuriInterpreter;


namespace Yuri
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 导演类
        /// </summary>
        private Director core = Director.GetInstance();

        /// <summary>
        /// 过渡效果的数据包装
        /// </summary>
        public ObjectDataProvider TransitionDS = new ObjectDataProvider();

        /// <summary>
        /// Alt键正在被按下的标记
        /// </summary>
        private bool AltDown = false;

        /// <summary>
        /// 构造器
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Width = this.BO_MainGrid.Width = GlobalDataContainer.GAME_WINDOW_WIDTH;
            this.Height = GlobalDataContainer.GAME_WINDOW_ACTUALHEIGHT;
            this.BO_MainGrid.Height = GlobalDataContainer.GAME_WINDOW_HEIGHT;
            this.Title = GlobalDataContainer.GAME_TITLE_NAME;
            this.ResizeMode = GlobalDataContainer.GAME_WINDOW_RESIZEABLE ? System.Windows.ResizeMode.CanResize : System.Windows.ResizeMode.NoResize;
            core.SetMainWindow(this);
            this.TransitionBox.DataContext = this.TransitionDS;
        }
        
        #region 窗体监听事件
        /// <summary>
        /// 事件：窗体关闭
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            core.DisposeResource();
        }

        /// <summary>
        /// 事件：键盘按下按钮
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.core.UpdateKeyboard(e);
        }

        /// <summary>
        /// 事件：键盘松开按钮
        /// </summary>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.core.UpdateKeyboard(e);
        }

        /// <summary>
        /// 事件：键盘即将按下按钮
        /// </summary>
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltDown = true;
            }
            else if (e.SystemKey == Key.F4 && AltDown)
            {
                this.core.GetMainRender().Shutdown();
            }
            else if (e.SystemKey == Key.Enter && AltDown)
            {
                if (Director.FullScreen == true)
                {
                    this.WindowScreenTransform();
                }
                else
                {
                    this.FullScreenTransform();
                }
                Director.FullScreen = !Director.FullScreen;
            }
        }

        /// <summary>
        /// 事件：键盘即将松开按钮
        /// </summary>
        private void window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltDown = false;
            }
        }

        /// <summary>
        /// 事件：鼠标按下按钮
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        /// <summary>
        /// 事件：鼠标松开按钮
        /// </summary>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        /// <summary>
        /// 事件：鼠标滚轮滑动
        /// </summary>
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.core.UpdateMouseWheel(e.Delta);
        }

        /// <summary>
        /// 事件：窗体大小改变
        /// </summary>
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = (double)GlobalDataContainer.GAME_WINDOW_ACTUALHEIGHT * this.Width / (double)GlobalDataContainer.GAME_WINDOW_WIDTH;
        }

        /// <summary>
        /// 事件：窗口尺寸模式改变
        /// </summary>
        private void window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.FullScreenTransform();
            }
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 切换到全屏模式
        /// </summary>
        public void FullScreenTransform()
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// 切换到窗口模式
        /// </summary>
        public void WindowScreenTransform()
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.Topmost = false;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = GlobalDataContainer.GAME_WINDOW_WIDTH;
            this.Height = GlobalDataContainer.GAME_WINDOW_ACTUALHEIGHT;
        }
        #endregion

        // DEBUG
        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.core.GetMainRender().Save("mysave");
        }

        // DEBUG
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.core.GetMainRender().Load("mysave");
        }

        // DEBUG
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Interpreter ip = new Interpreter("TestProj", @"C:\Users\Kako\Desktop\testDir");
            ip.Dash(InterpreterType.RELEASE_WITH_IL, 8);
            ip.GenerateIL(@"Scenario\main.sil");
            ILConvertor ilc = ILConvertor.GetInstance();
            List<Scene> rS = ilc.Dash(@"Scenario");
        }
        
    }
}
