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
using Yuri.PlatformCore;

namespace Yuri
{
    /// <summary>
    /// NavWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NavWindow : Window
    {
        Page1 p1 = new Page1();
        Page2 p2 = new Page2();

        public NavWindow()
        {
            InitializeComponent();
            this.Width = 1280;
            this.Height = 720 + 32;
            this.mainCanvas.Width = 1280;
            this.mainCanvas.Height = 720;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.pc.Content = p1;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.pc.Content = p2;
        }
    }
}
