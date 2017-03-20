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
    /// Stage3D.xaml 的交互逻辑
    /// </summary>
    public partial class Stage3D : Page
    {
        public Stage3D()
        {
            InitializeComponent();

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = ResourceManager.GetInstance().GetPicture("CA01_01S.png", ResourceManager.FullImageRect);
            this.imger.Source = p.SpriteBitmapImage;
            this.imger.Height = p.SpriteBitmapImage.PixelHeight;
            this.imger.Width = p.SpriteBitmapImage.PixelWidth;
        }
    }
}
