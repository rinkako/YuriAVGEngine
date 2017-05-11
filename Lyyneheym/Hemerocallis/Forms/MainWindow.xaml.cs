using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Point pos = new Point();

        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var tmp = (Canvas)sender;
            pos = e.GetPosition(null);
            tmp.CaptureMouse();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var tmp = (Canvas)sender;
                double dx = e.GetPosition(null).X - pos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - pos.Y + tmp.Margin.Top;
                tmp.Margin = new Thickness(dx, dy, 0, 0);
                pos = e.GetPosition(null);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tmp = (Canvas)sender;
            tmp.ReleaseMouseCapture();
        }
    }
}
