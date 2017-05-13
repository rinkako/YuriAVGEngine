using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly Controller core = Controller.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            this.core.mainWndRef = this;
            this.DefaultBgBrush = this.MainAreaBackgroundBrush;

            switch (this.core.ConfigDesc.BgType)
            {
                case Entity.AppearanceBackgroundType.Pure:
                    var rgbItems = this.core.ConfigDesc.BgTag.Split(',');
                    if (Byte.TryParse(rgbItems[0], out byte tr) &&
                        Byte.TryParse(rgbItems[1], out byte tg) &&
                        Byte.TryParse(rgbItems[2], out byte tb))
                    {
                        this.core.mainWndRef.Grid_MainArea.Background = new SolidColorBrush(Color.FromRgb(tr, tg, tb));
                    }
                    else
                    {
                        this.core.ConfigDesc.BgType = Entity.AppearanceBackgroundType.Default;
                        this.core.WriteConfigToSteady();
                    }
                    break;
                case Entity.AppearanceBackgroundType.Picture:
                    var bgPicFilePath = App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory, this.core.ConfigDesc.BgTag);
                    try
                    {
                        var bmp = new BitmapImage(new Uri(bgPicFilePath));
                        this.core.mainWndRef.Grid_MainArea.Background = new ImageBrush(bmp)
                        {
                            TileMode = TileMode.Tile,
                            ViewportUnits = BrushMappingMode.Absolute,
                            Viewport = new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight)
                        };
                    }
                    catch
                    {
                        this.core.ConfigDesc.BgType = Entity.AppearanceBackgroundType.Default;
                        this.core.WriteConfigToSteady();
                    }
                    break;
            }
            var fontColorItem = this.core.ConfigDesc.FontColor.Split(',');
            if (Byte.TryParse(fontColorItem[0], out byte fr) &&
                  Byte.TryParse(fontColorItem[1], out byte fg) &&
                  Byte.TryParse(fontColorItem[2], out byte fb))
            {
                this.RichTextBox_FlowDocument.Foreground = new SolidColorBrush(Color.FromRgb(fr, fg, fb));
            }
            this.RichTextBox_TextArea.FontSize = this.core.ConfigDesc.FontSize;
            this.RichTextBox_TextArea.FontFamily = new FontFamily(this.core.ConfigDesc.FontName);
            this.RichTextBox_DropShadowEffect.Opacity = this.core.ConfigDesc.ZeOpacity;
            this.RichTextBox_FlowDocument.LineHeight = this.core.ConfigDesc.LineHeight;
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

        private void Image_TRBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.85;
        }

        private void Image_TRBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.4;
        }

        private void Button_Appearance_Click(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            AppearanceWindow aw = new AppearanceWindow();
            aw.ShowDialog();
        }

        public ImageBrush DefaultBgBrush { get; private set; }

        private void Canvas_Tip_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                (sender as Canvas).Width *= 1.1;
                (sender as Canvas).Height *= 1.1;
            }
            else
            {
                (sender as Canvas).Width /= 1.1;
                (sender as Canvas).Height /= 1.1;
            }
        }
    }
}
