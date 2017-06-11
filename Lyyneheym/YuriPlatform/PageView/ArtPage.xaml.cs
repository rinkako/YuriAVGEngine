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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Yuri.PlatformCore;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.Graphic3D;

namespace Yuri.PageView
{
    /// <summary>
    /// ArtPage.xaml 的交互逻辑
    /// </summary>
    public partial class ArtPage : Page
    {
        private static readonly ResourceManager rm = ResourceManager.GetInstance();

        public ArtPage()
        {
            InitializeComponent();

            if (GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality)
            {
                this.MouseMove += this.Page_MouseMove;
            }

            this.Art_Image_Background.Stretch = Stretch.UniformToFill;
            this.Art_Image_Background.Opacity = 0.5;
            this.Art_MainGrid.Background = new ImageBrush(rm.GetPicture("UI_Gallery_Background.png", ResourceManager.FullImageRect).SpriteBitmapImage);
            this.Atr_Image_Title.Source = rm.GetPicture("UI_Gallery_Art_Title.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Atr_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_1.png", ResourceManager.FullImageRect).SpriteBitmapImage;

            this.canvasSprite = new YuriSprite()
            {
                DisplayBinding = this.Art_Grid_Controls,
                AnimationElement = this.Art_Grid_Controls,
                Descriptor = new SpriteDescriptor()
            };
        }

        private void Page_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Art_Image_Background != null)
            {
                var page = sender as ArtPage;
                var p = e.GetPosition(page);
                var mx = page.ActualWidth / 2;
                var my = page.ActualHeight / 2;
                var dx = p.X - mx;
                var dy = p.Y - my;
                var rx = dx / mx;
                var ry = dy / my;
                var tx = rx * 25;
                var ty = ry * 25;
                this.Art_Image_Background.Margin = new Thickness(-25 + tx, -25 + ty, -25 - tx, -25 - ty);
            }
        }

        private void Atr_Image_Back_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Atr_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_2.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Atr_Image_Back_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Atr_Image_Back.Source = rm.GetPicture("UI_System_Btn_Back_1.png", ResourceManager.FullImageRect).SpriteBitmapImage;
        }

        private void Atr_Image_Back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewPageManager.CollapseUIPage();
        }

        private readonly bool[,] OpenVect = new bool[2, 4];

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.OpenVect[i, j] = false;
                }
            }
            string bgFile = "bg_forest.jpg";
            switch (new Random().Next() % 4)
            {
                case 0:
                    bgFile = "bg_forest.jpg";
                    break;
                case 1:
                    bgFile = "bg_coach.jpg";
                    break;
                case 2:
                    bgFile = "bg_room.jpg";
                    break;
                case 3:
                    bgFile = "bg_market.jpg";
                    break;
            }
            this.Art_Image_Background.Source = rm.GetBackground(bgFile, ResourceManager.FullImageRect).SpriteBitmapImage;
            var tr_cg1 = (double) PlatformCore.VM.PersistContextDAO.Fetch("cg_open_1");
            if (tr_cg1 == 0)
            {
                this.Art_Image_Preview_11.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            }
            else
            {
                this.Art_Image_Preview_11.Source = rm.GetPicture("UI_Gallery_Art_pv_1.png", ResourceManager.FullImageRect).SpriteBitmapImage;
                OpenVect[0, 0] = true;
            }
            this.Art_Image_Preview_12.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_13.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_14.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_21.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_22.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_23.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            this.Art_Image_Preview_24.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
            var p = Mouse.GetPosition(this);
            var mx = this.ActualWidth / 2;
            var my = this.ActualHeight / 2;
            var dx = p.X - mx;
            var dy = p.Y - my;
            var rx = dx / mx;
            var ry = dy / my;
            var tx = rx * 25;
            var ty = ry * 25;
            this.Art_Image_Background.Margin = new Thickness( -25 + tx, -25 + ty, -25 - tx, -25 - ty);
            var wnd = Window.GetWindow(this);
            wnd.KeyDown += this.Art_Image_Viewbox_KeyDown;
        }

        private void Art_Image_Preview_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 1;
        }

        private void Art_Image_Preview_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.8;
        }

        private YuriSprite viewSprite;

        private YuriSprite canvasSprite;

        private void Art_Image_Preview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var row = Grid.GetRow(image) == 2 ? 1 : 0;
            var col = Grid.GetColumn(image);
            var isOpened = this.OpenVect[row, col];
            if (isOpened)
            {
                viewSprite = rm.GetPicture("CG_" + (row * 4 + col + 1) + ".png", ResourceManager.FullImageRect);
                SpriteDescriptor desc = new SpriteDescriptor()
                {
                    ResourceType = ResourceType.Pictures
                };
                viewSprite.Descriptor = desc;
                this.Art_Image_Viewbox.Margin = new Thickness(0, 0, 0, 0);
                this.RollCounter = 0;
                this.Art_Image_Viewbox.Source = viewSprite.SpriteBitmapImage;
                this.Art_Image_Viewbox.Visibility = Visibility.Visible;
                this.Art_Image_Viewbox.Opacity = 0;
                desc.ToOpacity = 1;
                viewSprite.DisplayBinding = viewSprite.AnimationElement = this.Art_Image_Viewbox;
                SpriteAnimation.OpacityToAnimation(viewSprite, TimeSpan.FromMilliseconds(500), 1);
                if (GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality)
                {
                    SpriteAnimation.BlurMutexAnimation(canvasSprite, TimeSpan.FromMilliseconds(800), 0, 50);
                }
                this.Art_Grid_Controls.IsHitTestVisible = false;
            }
        }

        private void Art_Image_Viewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Art_Image_Viewbox.Source = null;
            this.Art_Image_Viewbox.Visibility = Visibility.Hidden;
            this.Art_Grid_Controls.IsHitTestVisible = true;
            if (GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality)
            {
                SpriteAnimation.BlurMutexAnimation(canvasSprite, TimeSpan.FromMilliseconds(300), 50, 0);
            }
        }

        private void Art_Image_Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (RollCounter < 20)
                {
                    var oldMargin = this.Art_Image_Viewbox.Margin;
                    this.Art_Image_Viewbox.Margin = new Thickness(
                        oldMargin.Left - 30,
                        oldMargin.Top - 30,
                        oldMargin.Right - 30,
                        oldMargin.Bottom - 30
                    );
                    RollCounter++;
                }
            }
            else
            {
                if (RollCounter > 0)
                {
                    var oldMargin = this.Art_Image_Viewbox.Margin;
                    this.Art_Image_Viewbox.Margin = new Thickness(
                        oldMargin.Left + 30,
                        oldMargin.Top + 30,
                        oldMargin.Right + 30,
                        oldMargin.Bottom + 30
                    );
                    RollCounter--;
                }
            }
        }

        private int RollCounter = 0;

        private void Art_Image_Viewbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Art_Image_Viewbox.Visibility != Visibility.Visible)
            {
                return;
            }
            var oldMargin = this.Art_Image_Viewbox.Margin;
            switch (e.Key)
            {
                case Key.Up:
                    if (oldMargin.Top < 0)
                    {
                        this.Art_Image_Viewbox.Margin = new Thickness(
                            oldMargin.Left,
                            oldMargin.Top + 10,
                            oldMargin.Right,
                            oldMargin.Bottom - 10
                        );
                    }
                    break;
                case Key.Down:
                    if (oldMargin.Bottom < 0)
                    {
                        this.Art_Image_Viewbox.Margin = new Thickness(
                            oldMargin.Left,
                            oldMargin.Top - 10,
                            oldMargin.Right,
                            oldMargin.Bottom + 10
                        );
                    }
                    break;
                case Key.Left:
                    if (oldMargin.Left < 0)
                    {
                        this.Art_Image_Viewbox.Margin = new Thickness(
                            oldMargin.Left + 10,
                            oldMargin.Top,
                            oldMargin.Right - 10,
                            oldMargin.Bottom
                        );
                    }
                    break;
                case Key.Right:
                    if (oldMargin.Right < 0)
                    {
                        this.Art_Image_Viewbox.Margin = new Thickness(
                            oldMargin.Left - 10,
                            oldMargin.Top,
                            oldMargin.Right + 10,
                            oldMargin.Bottom
                        );
                    }
                    break;
                case Key.Escape:
                    this.Art_Image_Viewbox_MouseLeftButtonUp(null, null);
                    break;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewManager.mWnd.KeyDown -= this.Art_Image_Viewbox_KeyDown;
        }
    }
}
