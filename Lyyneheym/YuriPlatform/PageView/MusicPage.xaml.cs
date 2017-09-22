using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Yuri.PlatformCore;
using Yuri.PlatformCore.Audio;
using Yuri.PlatformCore.Graphic;

namespace Yuri.PageView
{
    /// <summary>
    /// MusicPage.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPage : Page
    {
        /// <summary>
        /// 资源管理器的引用
        /// </summary>
        private readonly ResourceManager rm = ResourceManager.GetInstance();

        private readonly DispatcherTimer ballTimer = new DispatcherTimer();

        private readonly DispatcherTimer progressTimer = new DispatcherTimer();

        private readonly Random rand = new Random();

        private readonly Dictionary<DoubleAnimation, Ellipse> activeBall = new Dictionary<DoubleAnimation, Ellipse>();

        private readonly UpdateRender render;

        private bool IsPlaying = false;

        /// <summary>
        /// 构造一个音乐盒页面
        /// </summary>
        public MusicPage()
        {
            InitializeComponent();

            this.render = Director.GetInstance().GetMainRender();

            this.ballTimer.Interval = TimeSpan.FromMilliseconds(300);
            this.ballTimer.Tick += BallTimer_Tick;

            this.progressTimer.Interval = TimeSpan.FromMilliseconds(25);
            this.progressTimer.Tick += ProgressTimer_Tick;

            this.Music_MainGrid.Background = new ImageBrush(rm.GetPicture("UI_MusicRoom_Bg.png", ResourceManager.FullImageRect).SpriteBitmapImage);
            this.Music_FloatStaticLayer.Source = rm.GetPicture("UI_MusicRoom_Float.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Music_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_1.png", ResourceManager.FullImageRect).SpriteBitmapImage;

            //this.Music_MBtn_0.Source = rm.GetPicture("UI_MusicRoom_MusicBtn_Normal.png", ResourceManager.FullImageRect).SpriteBitmapImage;

            this.RefreshMusicButton();

            this.Music_ProgressBar.Source = rm.GetPicture("UI_MusicRoom_BarItem.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Music_ProgressBar.Stretch = Stretch.Fill;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            foreach (var ballKVP in this.activeBall)
            {
                var ball = ballKVP.Value;
                var left = Convert.ToInt32(ball.Name.Split('_').Last()) == 0;
                var dt = this.rand.NextDouble() * 0.25;
                Canvas.SetLeft(ball, Canvas.GetLeft(ball) + (left ? -1 * dt : dt));
            }
        }

        private void RefreshMusicButton()
        {
            var rowCounter = 0;
            var colCounter = 0;
            var xcounter = 0;
            foreach (var bgmName in this.BgmList)
            {
                var btnImg = new Image
                {
                    Width = 258,
                    Height = 50,
                    Name = $"Music_MBtn_{xcounter}"
                };
                btnImg.MouseEnter += this.Music_MBtn_MouseEnter;
                btnImg.MouseLeave += this.Music_MBtn_MouseLeave;
                btnImg.MouseDown += this.Music_MBtn_MouseDown;
                btnImg.Source = rm.GetPicture("UI_MusicRoom_MusicBtn_Normal.png", ResourceManager.FullImageRect).SpriteBitmapImage;
                var leftMargin = colCounter == 0 ? 46 : 300;
                var topMargin = rowCounter * 55 + 50;
                var rightMargin = GlobalConfigContext.GAME_WINDOW_WIDTH - leftMargin - 258;
                var bottomMargin = GlobalConfigContext.GAME_WINDOW_HEIGHT - topMargin - 50;
                btnImg.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
                var textShadower = new DropShadowEffect
                {
                    BlurRadius = 6,
                    Color = Colors.Black,
                    ShadowDepth = 0,
                    Opacity = 1
                };
                Label btnText;
                try
                {
                    btnText = new Label
                    {
                        Content = bgmName,
                        Foreground = new SolidColorBrush(Colors.White),
                        Effect = textShadower,
                        Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin),
                        IsHitTestVisible = false,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("思源宋体"),
                        FontSize = 20
                    };
                }
                catch
                {
                    btnText = new Label
                    {
                        Content = bgmName,
                        Foreground = new SolidColorBrush(Colors.White),
                        Effect = textShadower,
                        Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin),
                        IsHitTestVisible = false,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 20
                    };
                }
                this.Music_ButtonContainer.Children.Add(btnImg);
                this.Music_ButtonContainer.Children.Add(btnText);
                xcounter++;
                rowCounter++;
                if (rowCounter > 8)
                {
                    rowCounter = 0;
                    colCounter++;
                }
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsPlaying == false)
            {
                this.progressTimer.Stop();
                return;
            }
            var ms = Musician.GetInstance();
            if (ms.IsBgmPlaying == false)
            {
                return;
            }
            var padSpace = GlobalConfigContext.GAME_WINDOW_WIDTH * (1 - ms.BgmPosition.TotalMilliseconds / ms.BgmDuration.TotalMilliseconds);
            if (Double.IsNaN(padSpace))
            {
                return;
            }
            this.Music_TimeStamp.Content = String.Format("{0:00}:{1:00} / {2:00}:{3:00}",
                ms.BgmPosition.Minutes, ms.BgmPosition.Seconds, ms.BgmDuration.Minutes, ms.BgmDuration.Seconds);
            this.Music_ProgressBar.Margin = new Thickness(0, GlobalConfigContext.GAME_WINDOW_HEIGHT - 10, padSpace, 0);
        }

        private void BallTimer_Tick(object sender, EventArgs e)
        {
            var border = this.rand.Next(0, 10000);
            var val = this.rand.Next(0, 50000);
            if (val > border)
            {
                this.AddLightBall();
            }
        }

        private void AddLightBall()
        {
            var rpx = this.rand.Next(15, 50);
            var ball = new Ellipse
            {
                Width = rpx,
                Height = rpx,
                Fill = new SolidColorBrush(Colors.Aqua)
            };
            Canvas.SetTop(ball, GlobalConfigContext.GAME_WINDOW_HEIGHT);
            Canvas.SetLeft(ball, this.rand.Next(0, GlobalConfigContext.GAME_WINDOW_WIDTH));
            this.Music_LightContainer.Children.Add(ball);
            var duration = Math.Sqrt(rpx);
            var trand = this.rand.Next(15000, 40000);
            var daY = new DoubleAnimation(GlobalConfigContext.GAME_WINDOW_HEIGHT, -100, TimeSpan.FromMilliseconds(trand));
            ball.BeginAnimation(Canvas.TopProperty, daY);
            var tsChanging = TimeSpan.FromMilliseconds(duration * 1000);
            var daOpa = new DoubleAnimation(0.7, 0, tsChanging);
            ball.BeginAnimation(Ellipse.OpacityProperty, daOpa);
            var daR = new DoubleAnimation(rpx, 0, tsChanging);
            ball.BeginAnimation(Ellipse.WidthProperty, daR);
            ball.BeginAnimation(Ellipse.HeightProperty, daR);
            lock (this.activeBall)
            {
                ball.Name = "MB_" + Guid.NewGuid().ToString().Replace('-', '_') + "_" + this.rand.Next(0, 2);
                this.activeBall.Add(daR, ball);
            }
            daR.Completed += delegate
            {
                lock (this.activeBall)
                {
                    this.activeBall.Remove(daR);
                }
            };
        }

        private void Music_Image_Back_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Music_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_2.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Music_Image_Back_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Music_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_1.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Music_Image_Back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewPageManager.CollapseUIPage();
            this.Music_Label_SongAuthor.Content = this.Music_Label_SongDisc.Content = this.Music_Label_SongName.Content = String.Empty;
        }

        private readonly string[] FileList =
        {
            "Test.mp3",
            "Bgm2.mp3",
            "Bgm3.mp3"
        };

        private readonly string[] BgmList =
        {
            "フィーネ 愛のテーマ",
            "田园花香",
            "温暖生活"
        };

        private readonly string[] DiscList =
        {
            "「終末のイゼッタ」オリジナルサウンドトラック",
            "",
            ""
        };

        private readonly string[] AuthorList =
        {
            "未知瑠",
            "大怪兽PANDA",
            "大怪兽PANDA"
        };
        
        private void HandlePlay(int idx)
        {
            this.render.Bgm(this.FileList[idx], 1000);
            this.IsPlaying = true;
            this.progressTimer.Start();
            this.ballTimer.Start();
            this.Music_Label_SongName.Opacity = this.Music_Label_SongDisc.Opacity = this.Music_Label_SongAuthor.Opacity = 0;
            this.Music_Label_SongName.Content = this.BgmList[idx];
            this.Music_Label_SongDisc.Content = this.DiscList[idx];
            this.Music_Label_SongAuthor.Content = this.AuthorList[idx];
            var duration = TimeSpan.FromMilliseconds(1000);
            var taName = new ThicknessAnimation(new Thickness(800, 520, -80, 160), new Thickness(800, 520, 40, 160), duration)
            {
                DecelerationRatio = 0.75
            };
            var taDisc = new ThicknessAnimation(new Thickness(800, 592, -80, 92), new Thickness(800, 592, 40, 92), duration)
            {
                DecelerationRatio = 0.75
            };
            var taAuthor = new ThicknessAnimation(new Thickness(800, 570, -80, 119), new Thickness(800, 570, 40, 119), duration)
            {
                DecelerationRatio = 0.75
            };
            var da = new DoubleAnimation(0, 1, duration);
            this.Music_Label_SongName.BeginAnimation(Label.MarginProperty, taName);
            this.Music_Label_SongDisc.BeginAnimation(Label.MarginProperty, taDisc);
            this.Music_Label_SongAuthor.BeginAnimation(Label.MarginProperty, taAuthor);
            this.Music_Label_SongName.BeginAnimation(Label.OpacityProperty, da);
            this.Music_Label_SongDisc.BeginAnimation(Label.OpacityProperty, da);
            this.Music_Label_SongAuthor.BeginAnimation(Label.OpacityProperty, da);
        }

        private void Music_MBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = rm.GetPicture("UI_MusicRoom_MusicBtn_Over.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Music_MBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = rm.GetPicture("UI_MusicRoom_MusicBtn_Normal.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Music_MBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            this.HandlePlay(Convert.ToInt32(btn.Name.Split('_')[2]));
        }
    }
}
