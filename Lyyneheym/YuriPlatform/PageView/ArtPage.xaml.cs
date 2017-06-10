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
            this.Art_Image_Preview_11.Source = rm.GetPicture("UI_Gallery_Art_Lock.png", ResourceManager.FullImageRect).SpriteBitmapImage;
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
    }
}
