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
using System.Windows.Media.Effects;
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
                AddTipWindow.AddSimpleTip();
            }
            else if (this.radioButton2.IsChecked == true)
            {
                var fd = new System.Windows.Forms.OpenFileDialog() { Filter = @"支持的图片类型|*.jpg;*.png" };
                fd.ShowDialog();
                if (fd.FileName == String.Empty)
                {
                    return;
                }
                AddTipWindow.AddPictureTip(fd.FileName);
            }
            this.Close();
        }

        /// <summary>
        /// 添加一个文本框简单小贴纸
        /// </summary>
        public static void AddSimpleTip()
        {
            ContentControl cc = new ContentControl()
            {
                Width = 200,
                Height = 200,
                MinWidth = 128,
                MinHeight = 128,
                Padding = new Thickness(1),
                Visibility = Visibility.Visible,
                Style = AddTipWindow.core.mainWndRef.FindResource("DesignerItemStyle") as Style,
                Effect = new DropShadowEffect()
                {
                    Color = Colors.Black,
                    Direction = 0,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 5
                }
            };
            var bgbrush = new BitmapImage(new Uri(App.ParseURIToURL("Assets", "bg_tip_" + (tipCounter++ % 5) + ".png"), UriKind.RelativeOrAbsolute));
            Grid inGrid = new Grid()
            {
                Background = new ImageBrush()
                {
                    Stretch = Stretch.Fill,
                    ImageSource = bgbrush
                },
                Width = cc.Width,
                Height = cc.Height,
                Margin = new Thickness(5)
            };
            cc.Content = inGrid;
            Binding resizeWBinding = new Binding()
            {
                Source = cc,
                Path = new PropertyPath("Width"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            Binding resizeHBinding = new Binding()
            {
                Source = cc,
                Path = new PropertyPath("Height"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(inGrid, Grid.WidthProperty, resizeWBinding);
            BindingOperations.SetBinding(inGrid, Grid.HeightProperty, resizeHBinding);
            TextBox tb = new TextBox()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Padding = new Thickness(8),
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                FontSize = 16,
                FontFamily = new FontFamily("黑体"),
                TextWrapping = TextWrapping.Wrap
            };
            inGrid.Children.Add(tb);
            Canvas.SetLeft(cc, 233);
            Canvas.SetTop(cc, 233);
            AddTipWindow.core.mainWndRef.TipCanvas.Children.Add(cc);
        }

        /// <summary>
        /// 添加一个图片小贴纸
        /// </summary>
        /// <param name="picPath">图片的绝对路径</param>
        public static void AddPictureTip(string picPath)
        {
            var bmp = new BitmapImage(new Uri(picPath, UriKind.RelativeOrAbsolute));
            ContentControl cc = new ContentControl()
            {
                Width = 200,
                Height = 200 * (bmp.Height / bmp.Width),
                MinWidth = 64,
                MinHeight = 64,
                Padding = new Thickness(1),
                Visibility = Visibility.Visible,
                Style = AddTipWindow.core.mainWndRef.FindResource("DesignerItemStyle") as Style,
                Effect = new DropShadowEffect()
                {
                    Color = Colors.Black,
                    Direction = 0,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 5
                }
            };
            Grid inGrid = new Grid()
            {
                Background = new SolidColorBrush(Colors.White),
                Width = cc.Width,
                Height = cc.Height,
                Margin = new Thickness(5),
                IsHitTestVisible = false
            };
            cc.Content = inGrid;
            Binding resizeWBinding = new Binding()
            {
                Source = cc,
                Path = new PropertyPath("Width"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            Binding resizeHBinding = new Binding()
            {
                Source = cc,
                Path = new PropertyPath("Height"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(inGrid, Grid.WidthProperty, resizeWBinding);
            BindingOperations.SetBinding(inGrid, Grid.HeightProperty, resizeHBinding);
            Image img = new Image()
            {
                Margin = new Thickness(0),
                Source = bmp,
                Stretch = Stretch.Fill
            };
            inGrid.Children.Add(img);
            Canvas.SetLeft(cc, 255);
            Canvas.SetTop(cc, 255);
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
        /// 便签计数器
        /// </summary>
        private static int tipCounter = 0;

        /// <summary>
        /// 后台的引用
        /// </summary>
        private static readonly Controller core = Controller.GetInstance();
    }
}
