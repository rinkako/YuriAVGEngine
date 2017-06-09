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
            this.Art_Image_Background.Source = rm.GetPicture("UI_Gallery_Art_Background.png", ResourceManager.FullImageRect).SpriteBitmapImage;
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
                var tx = rx * 50;
                var ty = ry * 50;
                this.Art_Image_Background.Margin = new Thickness(
                    -50 - tx, 
                    -50 - ty, 
                    -50 + tx,
                    -50 + ty);
            }
        }
    }
}
