using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Yuri.PlatformCore;
using Yuri.YuriInterpreter;
using Yuri.Yuriri;

namespace Yuri.PageView
{
    /// <summary>
    /// StagePage.xaml 的交互逻辑：主舞台页面
    /// </summary>
    public partial class Stage2D : Page
    {
        /// <summary>
        /// 导演类
        /// </summary>
        private readonly Director core = Director.GetInstance();

        /// <summary>
        /// 初始化标记位
        /// </summary>
        private bool isInit = false;

        /// <summary>
        /// 过渡效果的数据包装
        /// </summary>
        public ObjectDataProvider TransitionDS = new ObjectDataProvider();

        /// <summary>
        /// 构造器
        /// </summary>
        public Stage2D()
        {
            InitializeComponent();
            this.Width = this.BO_MainGrid.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.BO_MainGrid.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.Title = GlobalConfigContext.GAME_TITLE_NAME;
            this.TransitionBox.DataContext = this.TransitionDS;
            this.BO_Bg_Canvas.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.BO_Bg_Canvas.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.BO_Cstand_Canvas.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.BO_Cstand_Canvas.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.BO_Pics_Canvas.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.BO_Pics_Canvas.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
        }

        /// <summary>
        /// 事件：页面加载完毕
        /// </summary>
        private void StagePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.isInit == false)
            {
                SCamera2D.Init();
                this.core.GetMainRender().ViewLoaded();
                NotificationManager.Init();
                this.isInit = true;
            }
        }

        #region 窗体监听事件
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
        #endregion
        
        // DEBUG
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //this.core.GetMainRender().Save("mysave");
            ViewManager.RenderFrameworkElementToJPEG(this.BO_MainGrid, GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg");
            SLPage p = (SLPage)ViewPageManager.RetrievePage("SavePage");
            p.ReLoadFileInfo();
            NavigationService.GetNavigationService(this)?.Navigate(p);
        }

        // DEBUG
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //this.core.GetMainRender().Load("mysave");
            SLPage p = (SLPage)ViewPageManager.RetrievePage("LoadPage");
            p.ReLoadFileInfo();
            NavigationService.GetNavigationService(this)?.Navigate(p);
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

            var izettaPoint = SCamera2D.GetScreenCoordination(4, 8);
            var finePoint = SCamera2D.GetScreenCoordination(4, 11);
            var zoiPoint = SCamera2D.GetScreenCoordination(4, 24);


            var rm = ResourceManager.GetInstance();
            var izetta = rm.GetPicture("伊泽塔1.png", new Int32Rect(-1, 0, 0, 0));
            Image img1 = new Image();
            img1.Source = izetta.SpriteBitmapImage;
            img1.Width = izetta.SpriteBitmapImage.PixelWidth;
            img1.Height = izetta.SpriteBitmapImage.PixelHeight;
            izetta.DisplayBinding = img1;
            izetta.AnimationElement = img1;
            var izettad = (SpriteDescriptor)sd.Clone();
            izettad.ToScaleX = izettad.ToScaleY = 0.4;
            izetta.Descriptor = izettad;
            izetta.Descriptor.X = izettaPoint.X - izetta.SpriteBitmapImage.PixelWidth / 2.0;
            izetta.Descriptor.Y = izettaPoint.Y - izetta.SpriteBitmapImage.PixelHeight / 2.0;

            //Canvas.SetLeft(img1, 150 - izetta.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetLeft(img1, izettaPoint.X - izetta.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetTop(img1, izettaPoint.Y - izetta.SpriteBitmapImage.PixelHeight / 2.0);
            //Canvas.SetTop(img1, 630 - izetta.SpriteBitmapImage.PixelHeight / 2.0);
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
            fine.AnimationElement = img2;
            var fined = (SpriteDescriptor)sd.Clone();
            fined.ToScaleX = fined.ToScaleY = 0.5;
            fine.Descriptor = fined;
            fine.Descriptor.X = finePoint.X - fine.SpriteBitmapImage.PixelWidth / 2.0;
            fine.Descriptor.Y = finePoint.Y + 100 - fine.SpriteBitmapImage.PixelHeight / 2.0;
            //Canvas.SetLeft(img2, 400 - fine.SpriteBitmapImage.PixelWidth / 2.0);
            //Canvas.SetTop(img2, 730 - fine.SpriteBitmapImage.PixelHeight / 2.0);
            Canvas.SetLeft(img2, finePoint.X - fine.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetTop(img2, finePoint.Y + 100 - fine.SpriteBitmapImage.PixelHeight / 2.0);
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
            mt.AnimationElement = img4;
            var zoid = (SpriteDescriptor)sd.Clone();
            zoid.ToScaleX = zoid.ToScaleY = 0.43;
            mt.Descriptor = zoid;
            mt.Descriptor.X = zoiPoint.X - mt.SpriteBitmapImage.PixelWidth / 2.0;
            mt.Descriptor.Y = zoiPoint.Y - mt.SpriteBitmapImage.PixelHeight / 2.0;
            Canvas.SetLeft(img4, zoiPoint.X - mt.SpriteBitmapImage.PixelWidth / 2.0);
            Canvas.SetTop(img4, zoiPoint.Y - mt.SpriteBitmapImage.PixelHeight / 2.0);
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
            bgg.AnimationElement = img3;
            bgg.Descriptor = (SpriteDescriptor)sd.Clone();
            bgg.Descriptor.X = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0;
            bgg.Descriptor.Y = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0;
            Canvas.SetLeft(img3, bgg.Descriptor.X - img3.Width / 2);
            Canvas.SetTop(img3, bgg.Descriptor.Y - img3.Height / 2);
            Canvas.SetZIndex(img3, 5);
            this.BO_Bg_Canvas.Children.Add(img3);
            bgg.InitAnimationRenderTransform();

            //bgg.Descriptor.ToScaleX = 0.5;
            //bgg.Descriptor.ToScaleY = 0.5;
            //SpriteAnimation.ScaleAnimation(bgg, TimeSpan.Zero, 1, 0.5, 1, 0.5, 0, 0);

            TransformGroup aniGroup = new TransformGroup();
            TranslateTransform XYTransformer = new TranslateTransform();
            ScaleTransform ScaleTransformer = new ScaleTransform();
            ScaleTransformer.CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0;
            ScaleTransformer.CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0;
            RotateTransform RotateTransformer = new RotateTransform();
            RotateTransformer.CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0;
            RotateTransformer.CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0;
            CsScaleT = ScaleTransformer;
            aniGroup.Children.Add(XYTransformer);
            aniGroup.Children.Add(ScaleTransformer);
            aniGroup.Children.Add(RotateTransformer);
            this.BO_Cstand_Viewbox.RenderTransform = aniGroup;

            TransformGroup aniGroup2 = new TransformGroup();
            TranslateTransform XYTransformer2 = new TranslateTransform();
            ScaleTransform ScaleTransformer2 = new ScaleTransform();
            ScaleTransformer2.CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0;
            ScaleTransformer2.CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0;
            RotateTransform RotateTransformer2 = new RotateTransform();
            RotateTransformer2.CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0;
            RotateTransformer2.CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0;
            BgScaleT = ScaleTransformer2;
            aniGroup2.Children.Add(XYTransformer2);
            aniGroup2.Children.Add(ScaleTransformer2);
            aniGroup2.Children.Add(RotateTransformer2);
            this.BO_Bg_Viewbox.RenderTransform = aniGroup2;
            BgTG = aniGroup2;
            CsTG = aniGroup;

            bgg.Descriptor.ScaleX = 0.75;
            bgg.Descriptor.ScaleY = 0.75;
            BgScaleT.ScaleX = 1 * 0.75;
            BgScaleT.ScaleY = 1 * 0.75;

        }

        ScaleTransform BgScaleT;
        ScaleTransform CsScaleT;

        TransformGroup BgTG;
        TransformGroup CsTG;

        private void button3_Click(object sender, RoutedEventArgs e)
        {
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (bt4 == -2)
            {
                SCamera2D.LeaveSceneToBlackFrame();
            }
            else if (bt4 == -1)
            {
                SCamera2D.PreviewEnterScene();
                SCamera2D.PostEnterScene();
            }
            else if (bt4 == 0)
            {
                SCamera2D.FocusOn(0, 6, 2);
            }
            else if (bt4 == 1)
            {
                SCamera2D.Translate(2, 8);
            }
            else if (bt4 == 2)
            {
                SCamera2D.Translate(2, 12);
            }
            else if (bt4 == 3)
            {
                SCamera2D.Translate(2, 0);
            }
            else if (bt4 == 4)
            {
                SCamera2D.Translate(2, 24);
            }
            else if (bt4 == 5)
            {
                SCamera2D.Translate(2, 12);
            }
            else if (bt4 == 6)
            {
                SCamera2D.Translate(2, 24);
            }
            else if (bt4 == 7)
            {
                SCamera2D.ResetFocus(false);
            }
            else if (bt4 <= 32)
            {
                SCamera2D.Translate(bt4 % 5, bt4);
            }
            else if (bt4 == 33)
            {
                if (MessageBox.Show("reset?", "i", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SCamera2D.ResetFocus(false);
                    bt4 = -3;
                }
            }
            bt4++;
        }

        int bt4 = -2;

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            ViewManager.RenderFrameworkElementToJPEG(this.BO_MainGrid, GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg");
            NavigationService.GetNavigationService(this).Navigate(ViewPageManager.RetrievePage("SavePage"));
        }

        private void noti_Click(object sender, RoutedEventArgs e)
        {
            NotificationManager.Notify("菲涅的伊泽塔", "恭喜全部通关！鉴赏模式已经开放了。", "Info_Silver.png");
        }

        private int blurCount = -1;
        private YuriSprite Blurbgg = null;
        private SpriteDescriptor Blursd = null;
        private void Blur_OnClick(object sender, RoutedEventArgs e)
        {

            if (blurCount == -1)
            {
                Blursd = new SpriteDescriptor()
                {
                    ResourceType = ResourceType.Pictures
                };
                var rm = ResourceManager.GetInstance();
                Blurbgg = rm.GetPicture("UUZ.jpg", ResourceManager.FullImageRect);
                Image img3 = new Image();
                img3.Source = Blurbgg.SpriteBitmapImage;
                img3.Width = Blurbgg.SpriteBitmapImage.PixelWidth;
                img3.Height = Blurbgg.SpriteBitmapImage.PixelHeight;
                Blurbgg.DisplayBinding = img3;
                Blurbgg.AnimationElement = img3;
                Blurbgg.Descriptor = Blursd;
                Canvas.SetLeft(img3, 0);
                Canvas.SetTop(img3, 0);
                Canvas.SetZIndex(img3, 5);
                this.BO_Bg_Canvas.Children.Add(img3);
                Blurbgg.InitAnimationRenderTransform();
                Blursd.BlurRadius = 0;
                Blursd.ToBlurRadius = 20;
            }
            else if (blurCount % 2 == 0)
            {
                Blursd.ToBlurRadius = 50;
                SpriteAnimation.BlurMutexAnimation(Blurbgg, TimeSpan.FromMilliseconds(1000), Blursd.BlurRadius, Blursd.ToBlurRadius);
            }
            else
            {
                Blursd.ToBlurRadius = 0;
                SpriteAnimation.BlurMutexAnimation(Blurbgg, TimeSpan.FromMilliseconds(1000), Blursd.BlurRadius, Blursd.ToBlurRadius);
            }
            blurCount++;
        }
    }
}
