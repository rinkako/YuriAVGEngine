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
using System.Windows.Navigation;

namespace Yuri
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {
        Page2 p2 = new Page2();
        public Page1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.label.Content = DateTime.Now.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //new Uri("Page2.xaml", UriKind.Relative)
            this.SaveFrameworkElementToImage(this.p1Grid, ("snapshot_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".jpg"));
            NavigationService.GetNavigationService(this).Navigate(p2);
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="ui">控件名称</param>
        /// <param name="filename">图片文件名</param>
        public void SaveFrameworkElementToImage(FrameworkElement ui, string filename)
        {
            try
            {
                System.IO.FileStream ms = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                System.Windows.Media.Imaging.RenderTargetBitmap bmp = new System.Windows.Media.Imaging.RenderTargetBitmap((int)ui.ActualWidth, (int)ui.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Pbgra32);
                bmp.Render(ui);
                System.Windows.Media.Imaging.JpegBitmapEncoder encoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));
                encoder.Save(ms);
                ms.Close();
                
            }
            catch (Exception ex)
            {
                //记录异常
            }
        }
    }
}
