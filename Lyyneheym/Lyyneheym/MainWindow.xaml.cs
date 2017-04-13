using System;
using System.Windows;
using System.Windows.Input;
using Yuri.PlatformCore;
using Yuri.PageView;
using Yuri.PlatformCore.Graphic;
using Yuri.Utils;

namespace Yuri
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 导演类的引用
        /// </summary>
        private Director world;

        /// <summary>
        /// Alt键正在被按下的标记
        /// </summary>
        private static bool altDown = false;
        
        /// <summary>
        /// 构造器
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ViewManager.SetWindowReference(this);
            this.Title = GlobalConfigContext.GAME_TITLE_NAME;
            this.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.Height = GlobalConfigContext.GAME_WINDOW_ACTUALHEIGHT;
            this.mainCanvas.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.mainCanvas.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.ResizeMode = GlobalConfigContext.GAME_WINDOW_RESIZEABLE ? ResizeMode.CanResize : ResizeMode.NoResize;
            // 加载主页面
            this.mainFrame.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.mainFrame.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            ViewPageManager.RegisterPage("SplashPage", new SplashPage());
            this.mainFrame.Content = ViewPageManager.RetrievePage("SplashPage");
            this.maskFrame.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.maskFrame.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            ViewManager.MaskFrameRef = this.maskFrame;
        }

        /// <summary>
        /// 强制跳转到标题画面
        /// </summary>
        public void GoToTitle()
        {
            this.world = Director.GetInstance();
            if (GlobalConfigContext.GAME_IS3D)
            {
                this.world.SetStagePageReference(new Stage3D());
            }
            else
            {
                this.world.SetStagePageReference(new Stage2D());
            }
            // 预注册保存和读取页面
            ViewPageManager.RegisterPage("SavePage", new SLPage(isSave: true));
            ViewPageManager.RegisterPage("LoadPage", new SLPage(isSave: false));
            this.mainFrame.Content = ViewPageManager.RetrievePage(GlobalConfigContext.FirstViewPage);
        }
        
        #region 窗体监听事件
        /// <summary>
        /// 事件：窗体关闭
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.world.CollapseWorld();
        }

        /// <summary>
        /// 事件：键盘即将按下按钮
        /// </summary>
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                MainWindow.altDown = true;
            }
            else if (e.SystemKey == Key.F4 && MainWindow.altDown)
            {
                this.world.GetMainRender().Shutdown();
            }
            else if (e.SystemKey == Key.Enter && MainWindow.altDown)
            {
                if (Director.IsFullScreen)
                {
                    this.WindowScreenTransform();
                }
                else
                {
                    this.FullScreenTransform();
                }
                Director.IsFullScreen = !Director.IsFullScreen;
            }
        }

        /// <summary>
        /// 事件：键盘即将松开按钮
        /// </summary>
        private void window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                MainWindow.altDown = false;
            }
        }
        
        /// <summary>
        /// 事件：窗体大小改变
        /// </summary>
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = GlobalConfigContext.GAME_WINDOW_ACTUALHEIGHT * this.Width / GlobalConfigContext.GAME_WINDOW_WIDTH;
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

        /// <summary>
        /// 事件：键盘按下按钮
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.world.UpdateKeyboard(e, true);
        }

        /// <summary>
        /// 事件：键盘松开按钮
        /// </summary>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.world.UpdateKeyboard(e, false);
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 切换到全屏模式
        /// </summary>
        public void FullScreenTransform()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// 切换到窗口模式
        /// </summary>
        public void WindowScreenTransform()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            this.Topmost = false;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.Height = GlobalConfigContext.GAME_WINDOW_ACTUALHEIGHT;
        }
        #endregion
    }
}
