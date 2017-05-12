using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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
            var item = new TreeViewItem()
            {
                Header = "萱草，忘却的爱"
            };
            int t = this.TreeView_ProjectTree.Items.Add(item);
            var root = item;
            item = new TreeViewItem()
            {
                Header = "Chapter 1 - 于盛夏之末"
            };
            root.Items.Add(item);
            var pp = item = new TreeViewItem()
            {
                Header = "Chapter 2 - 恋恋不舍"
            };
            root.Items.Add(item);
            item = new TreeViewItem()
            {
                Header = "Chapter 2.1 - 我是第二级"
            };
            pp.Items.Add(item);
            (this.TreeView_ProjectTree.Items[0] as TreeViewItem).IsExpanded = true;
            
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
                double dx = e.GetPosition(null).X - pos.X + Canvas.GetLeft(tmp);
                double dy = e.GetPosition(null).Y - pos.Y + Canvas.GetTop(tmp);
                Canvas.SetLeft(tmp, dx);
                Canvas.SetTop(tmp, dy);
                pos = e.GetPosition(null);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tmp = (Canvas)sender;
            tmp.ReleaseMouseCapture();
        }
        

        private void Button_Click_TitleTreeToggle(object sender, RoutedEventArgs e)
        {
            if (this.isProjectTreeVisible)
            {
                this.MainGrid_Col_0.MaxWidth = this.MainGrid_Col_0.MinWidth = 0;
            }
            else
            {
                this.MainGrid_Col_0.MaxWidth = this.MainGrid_Col_0.MinWidth = 256;
            }
            this.isProjectTreeVisible = !this.isProjectTreeVisible;
        }
        private bool isProjectTreeVisible = true;

        private void Image_MouseLeftButtonUp_MenuBtn(object sender, MouseButtonEventArgs e)
        {
            this.Flyout_Menu.IsOpen = true;
        }

        private void Button_Click_Menu_About(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
        }
    }
}
