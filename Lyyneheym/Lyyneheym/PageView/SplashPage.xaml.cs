using System;
using System.Collections.Generic;
using System.Windows;
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
        /// 读取闪屏界面信息
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Splash_Image_Box.Opacity = 0;
            int readCounter = 0;
            var resMana = ResourceManager.GetInstance();
            if (this.splashQueue == null)
            {
                this.splashQueue = new List<string>();
                string curSplashName;
                while (resMana.IsResourceExist(curSplashName = String.Format("Splash_{0}.png", readCounter),
                    ResourceType.Pictures))
                {
                    this.splashQueue.Add(curSplashName);
                    ++readCounter;
                }
            }
            this.HandleSplashQueue();
        }

        /// <summary>
        /// 处理闪屏队列
        /// </summary>
        private void HandleSplashQueue()
        {
            if (splashCounter < splashQueue.Count)
            {
                string curSplashName = this.splashQueue[this.splashCounter];
                var resMana = ResourceManager.GetInstance();
                var mscMana = Musician.GetInstance();
                string seSplashName;
                if (resMana.IsResourceExist(seSplashName = String.Format("Splash_{0}.mp3", splashCounter), ResourceType.SE))
                {
                    var se = resMana.GetSE(seSplashName);
                    mscMana.PlaySE(se, GlobalConfigContext.GAME_SOUND_SEVOL);
                }
                var sp = resMana.GetPicture(curSplashName, ResourceManager.FullImageRect);
                this.Splash_Image_Box.Source = sp.SpriteBitmapImage;
                this.SplashAnimation();
            }
            else
            {
                // 返回主界面
                this.splashCounter = 0;
                this.Splash_Image_Box.Source = null;
                ViewManager.mWnd.GoToMainStage();
            }
        }

        /// <summary>
        /// 应用渐变动画
        /// </summary>
        private void SplashAnimation()
        {
            this.story = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf_opacity = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame k0_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            EasingDoubleKeyFrame k1_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(blackDelta)));
            EasingDoubleKeyFrame k2_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(blackDelta + AnimationTimeMS)));
            EasingDoubleKeyFrame k3_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(blackDelta + AnimationTimeMS + PendingTimeMS)));
            EasingDoubleKeyFrame k4_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(blackDelta + AnimationTimeMS * 2 + PendingTimeMS)));
            daukf_opacity.KeyFrames.Add(k0_opacity);
            daukf_opacity.KeyFrames.Add(k1_opacity);
            daukf_opacity.KeyFrames.Add(k2_opacity);
            daukf_opacity.KeyFrames.Add(k3_opacity);
            daukf_opacity.KeyFrames.Add(k4_opacity);
            Storyboard.SetTarget(daukf_opacity, this.Splash_Image_Box);
            Storyboard.SetTargetProperty(daukf_opacity, new PropertyPath(OpacityProperty));
            this.story.Children.Add(daukf_opacity);
            this.story.FillBehavior = FillBehavior.Stop;
            this.story.Completed += delegate
            {
                this.splashCounter++;
                this.HandleSplashQueue();
            };
            this.story.Begin();
        }

        /// <summary>
        /// 事件：按下鼠标跳过动画
        /// </summary>
        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.story.SkipToFill();
        }

        /// <summary>
        /// 闪屏时退出标记
        /// </summary>
        public static bool LoadingExitFlag { get; set; } = false;

        /// <summary>
        /// Splash动画的故事板
        /// </summary>
        private Storyboard story;

        /// <summary>
        /// 闪屏信息向量
        /// </summary>
        private List<string> splashQueue;

        /// <summary>
        /// 当前闪屏次数
        /// </summary>
        private int splashCounter = 0;

        /// <summary>
        /// 两张闪屏开始的时间间隔
        /// </summary>
        private const int blackDelta = 1000;

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
