using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Yuri.Hemerocallis.Entity;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// 后台控制器的引用
        /// </summary>
        private readonly Controller core = Controller.GetInstance();

        /// <summary>
        /// 构造主页面
        /// </summary>
        public MainWindow()
        {
            // 初始化
            InitializeComponent();
            this.core.mainWndRef = this;
            this.DefaultBgBrush = this.MainAreaBackgroundBrush;
            // 根据配置恢复外观
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
                    var bgPicFilePath = App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory,
                        this.core.ConfigDesc.BgTag);
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
            // 刷新项目树
            this.ReDrawProjectTree();
            // 起始页
            var startPage = new TreeViewItem() {Header = "起始页"};
            this.TreeView_ProjectTree.Items.Insert(0, startPage);
            startPage.IsSelected = true;
            
        }

        #region 辅助函数

        
        #endregion

        #region 标题栏和工程树
        /// <summary>
        /// 标题栏按钮：打开关闭工程栏
        /// </summary>
        private void Button_Click_TitleTreeToggle(object sender, RoutedEventArgs e)
        {
            if (this.IsProjectTreeVisible)
            {
                this.MainGrid_Col_0.MaxWidth = this.MainGrid_Col_0.MinWidth = 0;
            }
            else
            {
                this.MainGrid_Col_0.MaxWidth = this.MainGrid_Col_0.MinWidth = 256;
            }
            this.IsProjectTreeVisible = !this.IsProjectTreeVisible;
        }

        /// <summary>
        /// 标题栏按钮：外观设置
        /// </summary>
        private void Button_Appearance_Click(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            AppearanceWindow aw = new AppearanceWindow();
            aw.ShowDialog();
        }

        /// <summary>
        /// 工程树：选择项改变
        /// </summary>
        private void TreeView_ProjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var selectedTag = (this.TreeView_ProjectTree.SelectedItem as TreeViewItem)?.Tag;
                if (selectedTag == null)
                {
                    return;
                }
                var sType = selectedTag.ToString().Split('#');
                // 书籍
                if (sType[0] == "HBook")
                {
                    var hb = this.core.BookVector.Find(b => b.BookRef.Id == $"HBook#{sType[1]}");
                    if (hb != null)
                    {
                        var hp = hb.BookRef.HomePage;
                        TextRange t = new TextRange(this.RichTextBox_TextArea.Document.ContentStart,
                            this.RichTextBox_TextArea.Document.ContentEnd);
                        t.Load(hp.DocumentMetadata, DataFormats.XamlPackage);
                        // TODO 此处应该是PAGE切换而不能直接换内容
                    }
                    this.CurrentBookId = sType[1];
                }
                // 文章
                else
                {
                    TextRange t = new TextRange(this.RichTextBox_TextArea.Document.ContentStart,
                           this.RichTextBox_TextArea.Document.ContentEnd);
                    var p = this.core.ArticleDict[sType[1]];
                    t.Load(p.DocumentMetadata, DataFormats.XamlPackage);
                    // TODO 此处应该是PAGE切换而不能直接换内容
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error was occured when toggle page." + Environment.NewLine + ex);
            }
        }

        /// <summary>
        /// 重绘工程树
        /// </summary>
        public void ReDrawProjectTree()
        {
            this.TreeView_ProjectTree.Items.Clear();
            var workStack = new Stack<HArticle>();
            var rootStack = new Stack<TreeViewItem>();
            for (int i = this.core.BookVector.Count - 1; i >= 0; i--)
            {
                var topBk = this.core.BookVector[i].BookRef;
                var tvi = new TreeViewItem()
                {
                    Header = topBk.Name,
                    Tag = topBk.Id
                };
                this.TreeView_ProjectTree.Items.Add(tvi);
                workStack.Push(this.core.BookVector[i].BookRef.HomePage);
                rootStack.Push(tvi);
            }
            while (workStack.Any())
            {
                var top = workStack.Pop();
                var croot = rootStack.Pop();
                for (int j = top.ChildrenList.Count - 1; j >= 0; j--)
                {
                    var cobj = top.ChildrenList[j];
                    var tvi = new TreeViewItem()
                    {
                        Header = cobj.Name,
                        Tag = cobj.Id
                    };
                    croot.Items.Add(tvi);
                    workStack.Push(top.ChildrenList[j]);
                    rootStack.Push(tvi);
                }
            }
        }

        /// <summary>
        /// 获取当前正在操作的书籍的ID
        /// </summary>
        public string CurrentBookId { get; private set; }

        /// <summary>
        /// 获取当前工程栏是否是打开状态
        /// </summary>
        public bool IsProjectTreeVisible { get; private set; } = true;

        /// <summary>
        /// 获取默认背景笔刷
        /// </summary>
        public ImageBrush DefaultBgBrush { get; private set; }
        #endregion

        #region 侧边栏
        /// <summary>
        /// 侧边菜单按钮：关于项目
        /// </summary>
        private void Button_Click_Menu_About(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
        }
        #endregion

        #region 命令栏
        /// <summary>
        /// 命令栏按钮：鼠标移入显现
        /// </summary>
        private void Image_TRBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.85;
        }

        /// <summary>
        /// 命令栏按钮：鼠标移出渐隐
        /// </summary>
        private void Image_TRBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.4;
        }

        /// <summary>
        /// 命令栏按钮：弹出右侧边菜单
        /// </summary>
        private void Image_MouseLeftButtonUp_MenuBtn(object sender, MouseButtonEventArgs e)
        {
            this.Flyout_Menu.IsOpen = true;
        }

        #endregion

        /// <summary>
        /// 事件：主窗体按下按键
        /// </summary>
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // CTRL时
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    // 新建
                    case Key.N:
                        new NewBookWindow().ShowDialog();
                        break;
                    // 强制保存
                    case Key.S:
                        this.core.FullCommit();
                        break;
                }
            }
        }
    }
}
