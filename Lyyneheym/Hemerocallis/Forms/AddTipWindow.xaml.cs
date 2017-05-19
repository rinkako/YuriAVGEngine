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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// AddTipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddTipWindow : MetroWindow
    {
        /// <summary>
        /// 构造新建小贴纸页面
        /// </summary>
        public AddTipWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 事件：按下键盘按钮
        /// </summary>
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.Button_OK_Click(null, null);
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// 按钮：确定按钮
        /// </summary>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.radioButton1.IsChecked == true)
            {
                //AddTipWindow.AddSimpleTip();
                ContentControl cc = new ContentControl()
                {
                    Width = 100,
                    Height = 100,
                    Padding = new Thickness(1),
                    Visibility = Visibility.Visible,
                    Style = AddTipWindow.core.mainWndRef.FindResource("DesignerItemStyle") as Style,
                    Content = new Ellipse()
                    {
                        Fill = new SolidColorBrush(Colors.Red),
                        IsHitTestVisible = false
                    },
                };
                Canvas.SetLeft(cc, 233);
                Canvas.SetTop(cc, 233);
                AddTipWindow.core.mainWndRef.TipCanvas.Children.Add(cc);
            }
            this.Close();
        }

        public static void AddSimpleTip()
        {
            ContentControl cc = new ContentControl()
            {
                Width = 250,
                Height = 250,
                Padding = new Thickness(1),
                Visibility = Visibility.Visible,
                Style = AddTipWindow.core.mainWndRef.FindResource("DesignerItemStyle") as Style
            };
            //Grid inGrid = new Grid()
            //{
            //    Background = new SolidColorBrush(Colors.Red),
            //    Width = cc.Width,
            //    Height = cc.Height
            //};
            //cc.Content = inGrid;
            var bgbrush = new BitmapImage(new Uri(App.ParseURIToURL("Assets", "bg_tip_blue.png"), UriKind.RelativeOrAbsolute));
            TextBox tb = new TextBox()
            {
                Background = new ImageBrush()
                {
                    Stretch = Stretch.Fill,
                    ImageSource = bgbrush
                },
                Padding = new Thickness(5),
                Margin = new Thickness(0)
            };
            //inGrid.Children.Add(tb);
            var ee = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.Red),
                IsHitTestVisible = false
            };
            cc.Content = ee;
            Canvas.SetLeft(cc, 233);
            Canvas.SetTop(cc, 233);




            AddTipWindow.core.mainWndRef.TipCanvas.Children.Add(cc);
        }

        /// <summary>
        /// 事件：选择便签类型
        /// </summary>
        private void radioButton3_Checked(object sender, RoutedEventArgs e)
        {
            this.comboBox.IsEnabled = true;
        }

        /// <summary>
        /// 事件：选择非便签类型
        /// </summary>
        private void radioButton3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.comboBox.IsEnabled = false;
        }

        /// <summary>
        /// 后台的引用
        /// </summary>
        private static readonly Controller core = Controller.GetInstance();
    }
}
