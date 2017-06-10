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
        private void Art_Image_Preview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var row = Grid.GetRow(image) == 2 ? 1 : 0;
            var col = Grid.GetColumn(image);
            var isOpened = this.OpenVect[row, col];
            viewSprite = rm.GetPicture("CG_" + (row * 4 + col + 1) + ".png", ResourceManager.FullImageRect);
            SpriteDescriptor desc = new SpriteDescriptor()
            {
                ResourceType = ResourceType.Pictures
            };
            viewSprite.Descriptor = desc;
            if (isOpened)
            {
                this.Art_Image_Viewbox.Source = viewSprite.SpriteBitmapImage;
                this.Art_Image_Viewbox.Visibility = Visibility.Visible;
                this.Art_Image_Viewbox.Opacity = 0;
                desc.ToOpacity = 1;
                viewSprite.DisplayBinding = viewSprite.AnimationElement = this.Art_Image_Viewbox;
                SpriteAnimation.BlurMutexAnimation(viewSprite, TimeSpan.Zero, 0, 50);
                SpriteAnimation.OpacityToAnimation(viewSprite, TimeSpan.FromMilliseconds(500), 1);
                SpriteAnimation.BlurMutexAnimation(viewSprite, TimeSpan.FromMilliseconds(500), 50, 0);
            }
        }

        private void Art_Image_Viewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Art_Image_Viewbox.Source = null;
            this.Art_Image_Viewbox.Visibility = Visibility.Hidden;
        }
    }
}
