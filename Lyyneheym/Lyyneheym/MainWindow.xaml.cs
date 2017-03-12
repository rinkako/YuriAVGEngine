using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using Yuri.PlatformCore;
using Yuri.ILPackage;
using Yuri.YuriInterpreter;
using System.Windows.Media.Animation;
using System.Windows.Media;

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
        private static bool AltDown = false;

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
            this.BO_Bg_Canvas.Width = GlobalDataContainer.GAME_WINDOW_WIDTH;
            this.BO_Bg_Canvas.Height = GlobalDataContainer.GAME_WINDOW_HEIGHT;
            this.BO_Cstand_Canvas.Width = GlobalDataContainer.GAME_WINDOW_WIDTH;
            this.BO_Cstand_Canvas.Height = GlobalDataContainer.GAME_WINDOW_HEIGHT;
            this.BO_Pics_Canvas.Width = GlobalDataContainer.GAME_WINDOW_WIDTH;
            this.BO_Pics_Canvas.Height = GlobalDataContainer.GAME_WINDOW_HEIGHT;
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
                MainWindow.AltDown = true;
            }
            else if (e.SystemKey == Key.F4 && MainWindow.AltDown)
            {
                this.core.GetMainRender().Shutdown();
            }
            else if (e.SystemKey == Key.Enter && MainWindow.AltDown)
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
                MainWindow.AltDown = false;
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

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                ResourceType = ResourceType.Pictures
            };
            var xxx = this.BO_Cstand_Canvas;
            var xxxx = this.BO_Cstand_Viewbox;
            var rm = ResourceManager.GetInstance();
            var izetta = rm.GetPicture("伊泽塔1.png", new Int32Rect(-1, 0, 0, 0));
            Image img1 = new Image();
            img1.Source = izetta.SpriteBitmapImage;
            img1.Width = izetta.SpriteBitmapImage.PixelWidth;
            img1.Height = izetta.SpriteBitmapImage.PixelHeight;
            izetta.DisplayBinding = img1;
            izetta.Descriptor = sd;
            Canvas.SetLeft(img1, 150 - izetta.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetTop(img1, 630 - izetta.SpriteBitmapImage.PixelHeight / 2.0);
            Canvas.SetZIndex(img1, 50);
            this.BO_Cstand_Canvas.Children.Add(img1);
            izetta.InitAnimationRenderTransform();
            SpriteAnimation.ScaleToAnimation(izetta, TimeSpan.FromMilliseconds(0), 0.4, 0.4, 0, 0);

            var fine = rm.GetPicture("公主1.png", new Int32Rect(-1, 0, 0, 0));
            Image img2 = new Image();
            img2.Source = fine.SpriteBitmapImage;
            img2.Width = fine.SpriteBitmapImage.PixelWidth;
            img2.Height = fine.SpriteBitmapImage.PixelHeight;
            fine.DisplayBinding = img2;
            fine.Descriptor = sd;
            Canvas.SetLeft(img2, 400 - fine.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetTop(img2, 730 - fine.SpriteBitmapImage.PixelHeight / 2.0);
            Canvas.SetZIndex(img2, 50);
            this.BO_Cstand_Canvas.Children.Add(img2);
            fine.InitAnimationRenderTransform();
            SpriteAnimation.ScaleToAnimation(fine, TimeSpan.FromMilliseconds(0), 0.5, 0.5, 0, 0);

            var mt = rm.GetPicture("Zoithyt-4-2.png", new Int32Rect(-1, 0, 0, 0));
            Image img4 = new Image();
            img4.Source = mt.SpriteBitmapImage;
            img4.Width = mt.SpriteBitmapImage.PixelWidth;
            img4.Height = mt.SpriteBitmapImage.PixelHeight;
            mt.DisplayBinding = img4;
            mt.Descriptor = sd;
            modelX = 1000 - fine.SpriteBitmapImage.PixelWidth / 2.0;
            Canvas.SetLeft(img4, modelX);
            Canvas.SetTop(img4, 630 - fine.SpriteBitmapImage.PixelHeight / 2.0);
            Canvas.SetZIndex(img4, 50);
            this.BO_Cstand_Canvas.Children.Add(img4);
            mt.InitAnimationRenderTransform();
            SpriteAnimation.ScaleToAnimation(mt, TimeSpan.FromMilliseconds(0), 0.43, 0.43, 0, 0);

            var bgg = rm.GetPicture("bg_school.jpg", new Int32Rect(-1, 0, 0, 0));
            Image img3 = new Image();
            img3.Source = bgg.SpriteBitmapImage;
            img3.Width = bgg.SpriteBitmapImage.PixelWidth;
            img3.Height = bgg.SpriteBitmapImage.PixelHeight;
            bgg.DisplayBinding = img3;
            bgg.Descriptor = sd;
            Canvas.SetLeft(img3, 0);
            Canvas.SetTop(img3, 0);
            Canvas.SetZIndex(img3, 5);
            this.BO_Bg_Canvas.Children.Add(img3);
            bgg.InitAnimationRenderTransform();

            var scalePoint = SCamera.GetScreenCoordination(0, 1);

            TransformGroup aniGroup = new TransformGroup();
            TranslateTransform XYTransformer = new TranslateTransform();
            ScaleTransform ScaleTransformer = new ScaleTransform();
            ScaleTransformer.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 16.0;
            ScaleTransformer.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 16.0;
            RotateTransform RotateTransformer = new RotateTransform();
            RotateTransformer.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 2.0;
            RotateTransformer.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 2.0;
            CsScaleT = ScaleTransformer;
            aniGroup.Children.Add(XYTransformer);
            aniGroup.Children.Add(ScaleTransformer);
            aniGroup.Children.Add(RotateTransformer);
            this.BO_Cstand_Viewbox.RenderTransform = aniGroup;

            TransformGroup aniGroup2 = new TransformGroup();
            TranslateTransform XYTransformer2 = new TranslateTransform();
            ScaleTransform ScaleTransformer2 = new ScaleTransform();
            ScaleTransformer2.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 4.0;
            ScaleTransformer2.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 4.0;
            RotateTransform RotateTransformer2 = new RotateTransform();
            RotateTransformer2.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 2.0;
            RotateTransformer2.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 2.0;
            BgScaleT = ScaleTransformer2;
            aniGroup2.Children.Add(XYTransformer2);
            aniGroup2.Children.Add(ScaleTransformer2);
            aniGroup2.Children.Add(RotateTransformer2);
            this.BO_Bg_Viewbox.RenderTransform = aniGroup2;
            BgTG = aniGroup2;
            CsTG = aniGroup;
        }

        ScaleTransform BgScaleT;
        ScaleTransform CsScaleT;

        TransformGroup BgTG;
        TransformGroup CsTG;

        double bgNowX;
        double csNowX;

        int testcount = 0;

        double modelX;

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Duration duration = TimeSpan.FromMilliseconds(500);
            double StandToScale = 2.0, StandaccX = -0.7, StandaccY = -0.7;
            double bgtoScale = 1.2;
            if (testcount == 0)
            {
                SCamera.Init();

                Storyboard story = new Storyboard();
                Storyboard story2 = new Storyboard();
                DoubleAnimation doubleAniScaleX = new DoubleAnimation(1, StandToScale, duration);
                DoubleAnimation doubleAniScaleY = new DoubleAnimation(1, StandToScale, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX, this.BO_Cstand_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY, this.BO_Cstand_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story.Children.Add(doubleAniScaleX);
                story.Children.Add(doubleAniScaleY);
                story.Duration = duration;
                
                DoubleAnimation doubleAniScaleX2 = new DoubleAnimation(1, bgtoScale, duration);
                DoubleAnimation doubleAniScaleY2 = new DoubleAnimation(1, bgtoScale, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX2.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX2.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY2.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY2.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX2, this.BO_Bg_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY2, this.BO_Bg_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story2.Children.Add(doubleAniScaleX2);
                story2.Children.Add(doubleAniScaleY2);
                story2.Duration = duration;
                story.Begin();
                story2.Begin();
                
                core.GetMainRender().DrawStringToMsgLayer(0, "【伊泽塔】" + Environment.NewLine + "公主公主，你看那边有个画风和我们不一样的人！");

            }
            else if (testcount == 1)
            {
                Storyboard story = new Storyboard();
                double BgfromX = Canvas.GetLeft(this.BO_Bg_Viewbox);
                double BgtoX = SCamera.GetScreenCoordination(0, 9).X * Math.Abs(1 - bgtoScale) - modelX * bgtoScale;
                bgNowX = -BgtoX;
                DoubleAnimation doubleAniLeft = new DoubleAnimation(BgfromX, -BgtoX, duration);
                if (StandaccX >= 0)
                {
                    doubleAniLeft.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniLeft.DecelerationRatio = -StandaccX;
                }
                Storyboard.SetTarget(doubleAniLeft, this.BO_Bg_Viewbox);
                Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
                story.Children.Add(doubleAniLeft);
                story.Duration = duration;


                Storyboard story2 = new Storyboard();
                double CsfromX = Canvas.GetLeft(this.BO_Cstand_Viewbox);
                double CstoX = SCamera.GetScreenCoordination(0, 9).X * StandToScale;
                csNowX = -CstoX;
                DoubleAnimation doubleAniLeft2 = new DoubleAnimation(CsfromX, -CstoX, duration);
                if (StandaccX >= 0)
                {
                    doubleAniLeft2.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniLeft2.DecelerationRatio = -StandaccX;
                }
                Storyboard.SetTarget(doubleAniLeft2, this.BO_Cstand_Viewbox);
                Storyboard.SetTargetProperty(doubleAniLeft2, new PropertyPath(Canvas.LeftProperty));
                story2.Children.Add(doubleAniLeft2);
                story2.Duration = duration;

                story.FillBehavior = FillBehavior.Stop;
                story2.FillBehavior = FillBehavior.Stop;

                story.Completed += Story_1_Bg_Completed;
                story2.Completed += Story2_Cs_Completed;

                story.Begin();
                story2.Begin();

                core.GetMainRender().DrawStringToMsgLayer(0, "【佐茜】" + Environment.NewLine + "……我只是一个路过的人。");
            }
            else if (testcount == 2)
            {
                Storyboard story = new Storyboard();
                Storyboard story2 = new Storyboard();
                DoubleAnimation doubleAniScaleX = new DoubleAnimation(StandToScale, 1, duration);
                DoubleAnimation doubleAniScaleY = new DoubleAnimation(StandToScale, 1, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX, this.BO_Cstand_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY, this.BO_Cstand_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story.Children.Add(doubleAniScaleX);
                story.Children.Add(doubleAniScaleY);
                story.Duration = duration;

                DoubleAnimation doubleAniScaleX2 = new DoubleAnimation(bgtoScale, 1, duration);
                DoubleAnimation doubleAniScaleY2 = new DoubleAnimation(bgtoScale, 1, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX2.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX2.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY2.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY2.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX2, this.BO_Bg_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY2, this.BO_Bg_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story2.Children.Add(doubleAniScaleX2);
                story2.Children.Add(doubleAniScaleY2);
                story2.Duration = duration;


                Storyboard story3 = new Storyboard();
                double BgfromX = Canvas.GetLeft(this.BO_Bg_Viewbox);
                bgNowX = 0;
                DoubleAnimation doubleAniLeft = new DoubleAnimation(BgfromX, 0, duration);
                if (StandaccX >= 0)
                {
                    doubleAniLeft.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniLeft.DecelerationRatio = -StandaccX;
                }
                Storyboard.SetTarget(doubleAniLeft, this.BO_Bg_Viewbox);
                Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
                story3.Children.Add(doubleAniLeft);
                story3.Duration = duration;


                Storyboard story4 = new Storyboard();
                double CsfromX = Canvas.GetLeft(this.BO_Cstand_Viewbox);
                csNowX = 0;
                DoubleAnimation doubleAniLeft2 = new DoubleAnimation(CsfromX, 0, duration);
                if (StandaccX >= 0)
                {
                    doubleAniLeft2.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniLeft2.DecelerationRatio = -StandaccX;
                }
                Storyboard.SetTarget(doubleAniLeft2, this.BO_Cstand_Viewbox);
                Storyboard.SetTargetProperty(doubleAniLeft2, new PropertyPath(Canvas.LeftProperty));
                story4.Children.Add(doubleAniLeft2);
                story4.Duration = duration;

                var t = Canvas.GetLeft(this.BO_Cstand_Viewbox);

                story3.FillBehavior = FillBehavior.Stop;
                story4.FillBehavior = FillBehavior.Stop;
                story3.Completed += Story_1_Bg_Completed;
                story4.Completed += Story2_Cs_Completed;

                story3.Begin();
                story4.Begin();



                story.Begin();
                story2.Begin();
                core.GetMainRender().DrawStringToMsgLayer(0, "【公主】" + Environment.NewLine + "看上去场景镜头系统做好了呢！");

            }
            else if (testcount == 3)
            {
                BgScaleT.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 4.0 * 3;
                BgScaleT.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 4.0;
                CsScaleT.CenterX = GlobalDataContainer.GAME_WINDOW_WIDTH / 16.0 * 15;
                CsScaleT.CenterY = GlobalDataContainer.GAME_WINDOW_HEIGHT / 16.0;

                Storyboard story = new Storyboard();
                Storyboard story2 = new Storyboard();
                DoubleAnimation doubleAniScaleX = new DoubleAnimation(1, StandToScale, duration);
                DoubleAnimation doubleAniScaleY = new DoubleAnimation(1, StandToScale, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX, this.BO_Cstand_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY, this.BO_Cstand_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story.Children.Add(doubleAniScaleX);
                story.Children.Add(doubleAniScaleY);
                story.Duration = duration;

                DoubleAnimation doubleAniScaleX2 = new DoubleAnimation(1, bgtoScale, duration);
                DoubleAnimation doubleAniScaleY2 = new DoubleAnimation(1, bgtoScale, duration);
                if (StandaccX >= 0)
                {
                    doubleAniScaleX2.AccelerationRatio = StandaccX;
                }
                else
                {
                    doubleAniScaleX2.DecelerationRatio = -StandaccX;
                }
                if (StandaccY >= 0)
                {
                    doubleAniScaleY2.AccelerationRatio = StandaccY;
                }
                else
                {
                    doubleAniScaleY2.DecelerationRatio = -StandaccY;
                }
                Storyboard.SetTarget(doubleAniScaleX2, this.BO_Bg_Viewbox);
                Storyboard.SetTarget(doubleAniScaleY2, this.BO_Bg_Viewbox);
                Storyboard.SetTargetProperty(doubleAniScaleX2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story2.Children.Add(doubleAniScaleX2);
                story2.Children.Add(doubleAniScaleY2);
                story2.Duration = duration;

                story.Begin();
                story2.Begin();
                core.GetMainRender().DrawStringToMsgLayer(0, "【佐茜】" + Environment.NewLine + "这回聚焦我了吧……");
            }
            testcount++;
            if (testcount == 4)
            {
                testcount = 0;
            }
        }

        private void Story2_Cs_Completed(object sender, EventArgs e)
        {
            Canvas.SetLeft(this.BO_Cstand_Viewbox, csNowX);
        }

        private void Story_1_Bg_Completed(object sender, EventArgs e)
        {
            Canvas.SetLeft(this.BO_Bg_Viewbox, bgNowX);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Storyboard story3 = new Storyboard();
            double BgfromX = Canvas.GetLeft(this.BO_Cstand_Viewbox);
            DoubleAnimation doubleAniLeft = new DoubleAnimation(BgfromX, 1000, TimeSpan.FromMilliseconds(0));
            doubleAniLeft.DecelerationRatio = 0.75;
            Storyboard.SetTarget(doubleAniLeft, this.BO_Cstand_Viewbox);
            Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
            story3.Children.Add(doubleAniLeft);
            story3.Duration = TimeSpan.FromMilliseconds(0);
            story3.Begin();
        }
    }
}
