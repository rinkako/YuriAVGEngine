using System;
using System.Windows;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yuri.PlatformCore;
using Yuri.PlatformCore.Audio;
using Yuri.PlatformCore.Graphic;

namespace Yuri.PageView
{
    /// <summary>
    /// SplashPage.xaml 的交互逻辑
    /// </summary>
    public partial class SplashPage : Page
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SplashPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 读取闪屏界面并轮流展示
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Splash_Image_Box.Opacity = 0;
            int splashCounter = 1;
            var resMana = ResourceManager.GetInstance();
            var mscMana = Musician.GetInstance();
            string curSplashName;
            while (resMana.IsResourceExist(curSplashName = String.Format("Splash_{0}.png", splashCounter), ResourceType.Pictures))
            {
                string seSplashName;
                if (resMana.IsResourceExist(seSplashName = String.Format("Splash_{0}.mp3", splashCounter), ResourceType.SE))
                {
                    var se = resMana.GetSE(seSplashName);
                    mscMana.PlaySE(se, GlobalConfigContext.GAME_SOUND_SEVOL);
                }
                var sp = resMana.GetPicture(curSplashName, ResourceManager.FullImageRect);
                this.Splash_Image_Box.Source = sp.SpriteBitmapImage;
                this.SplashAnimation();
                while (this.isAnimating)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                this.Splash_Image_Box.Source = null;
                ++splashCounter;
            }
            // 返回主界面
            ViewManager.mWnd.GoToTitle();
        }

        /// <summary>
        /// 应用渐变动画
        /// </summary>
        private void SplashAnimation()
        {
            const int delta = 1000;
            this.story = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf_opacity = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame k0_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            EasingDoubleKeyFrame k1_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delta + 0)));
            EasingDoubleKeyFrame k2_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delta + AnimationTimeMS)));
            EasingDoubleKeyFrame k3_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delta + AnimationTimeMS + PendingTimeMS)));
            EasingDoubleKeyFrame k4_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delta + AnimationTimeMS * 2 + PendingTimeMS)));
            daukf_opacity.KeyFrames.Add(k0_opacity);
            daukf_opacity.KeyFrames.Add(k1_opacity);
            daukf_opacity.KeyFrames.Add(k2_opacity);
            daukf_opacity.KeyFrames.Add(k3_opacity);
            daukf_opacity.KeyFrames.Add(k4_opacity);
            Storyboard.SetTarget(daukf_opacity, this.Splash_Image_Box);
            Storyboard.SetTargetProperty(daukf_opacity, new PropertyPath(OpacityProperty));
            this.story.Children.Add(daukf_opacity);
            this.story.FillBehavior = FillBehavior.Stop;
            this.story.Completed += delegate { this.isAnimating = false; };
            this.isAnimating = true;
            this.story.Begin();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.story.SkipToFill();
        }

        /// <summary>
        /// Splash动画的故事板
        /// </summary>
        private Storyboard story;

        /// <summary>
        /// 是否正在动画
        /// </summary>
        private bool isAnimating = false;
        
        /// <summary>
        /// 渐变动画时长
        /// </summary>
        private const int AnimationTimeMS = 600;

        /// <summary>
        /// 渐变动画时长
        /// </summary>
        private const int PendingTimeMS = 2500;
    }
}
