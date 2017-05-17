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
                case AppearanceBackgroundType.Pure:
                    var rgbItems = this.core.ConfigDesc.BgTag.Split(',');
                    if (Byte.TryParse(rgbItems[0], out byte tr) &&
                        Byte.TryParse(rgbItems[1], out byte tg) &&
                        Byte.TryParse(rgbItems[2], out byte tb))
                    {
                        this.core.mainWndRef.Grid_MainArea.Background = new SolidColorBrush(Color.FromRgb(tr, tg, tb));
                    }
                    else
                    {
                        this.core.ConfigDesc.BgType = AppearanceBackgroundType.Default;
                        this.core.WriteConfigToSteady();
                    }
                    break;
                case AppearanceBackgroundType.Picture:
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
                        this.core.ConfigDesc.BgType = AppearanceBackgroundType.Default;
                        this.core.WriteConfigToSteady();
                    }
                    break;
            }
            RTBPage.CFontColorItem = this.core.ConfigDesc.FontColor.Split(',');
            RTBPage.CFontSize = this.core.ConfigDesc.FontSize;
            RTBPage.CFontName = this.core.ConfigDesc.FontName;
            RTBPage.CZeOpacity = this.core.ConfigDesc.ZeOpacity;
            RTBPage.CLineHeight = this.core.ConfigDesc.LineHeight;
            this.MainAreaBrush = this.Grid_MainArea.Background;
            // 刷新项目树
            this.ReDrawProjectTree();
            // 起始页
            this.IndexBackgroundBrush =
                new ImageBrush(new BitmapImage(new Uri(App.ParseURIToURL("Assets/bg_Index.jpg"), UriKind.RelativeOrAbsolute)))
                {
                    Stretch = Stretch.UniformToFill,
                    AlignmentX = AlignmentX.Left
                };
            this.IndexPageRef = new IndexPage();
            var startPage = new TreeViewItem() { Header = "起始页", Tag = "HemeIndexPage" };
            this.TreeView_ProjectTree.Items.Insert(0, startPage);
            startPage.IsSelected = true;



            
        }

        /// <summary>
        /// 获取当前活跃的RTB页
        /// </summary>
        public RTBPage CurrentActivePage { get; private set; } = null;

        /// <summary>
        /// RTB页缓存字典
        /// </summary>
        public readonly Dictionary<string, RTBPage> RTBPageCacheDict = new Dictionary<string, RTBPage>();

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

        public Brush MainAreaBrush { get; set; }

        public ImageBrush IndexBackgroundBrush { get; set; }

        public IndexPage IndexPageRef { get; set; }

        /// <summary>
        /// 工程树：选择项改变
        /// </summary>
        private void TreeView_ProjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                // 保存更改
                if (this.CurrentActivePage != null)
                {
                    this.core.PageCommit();
                }
                // 分析Tag
                var selectedTag = (this.TreeView_ProjectTree.SelectedItem as TreeViewItem)?.Tag;
                if (selectedTag == null)
                {
                    return;
                }
                if (selectedTag.ToString() == "HemeIndexPage")
                {
                    this.CurrentActivePage = null;
                    this.Grid_MainArea.Background = this.IndexBackgroundBrush;
                    this.Frame_RTB.NavigationService.Navigate(this.IndexPageRef);
                    this.StackPanel_Commands.Visibility = this.TextBlock_CurrentChapterName.Visibility =
                        this.TextBlock_MsgBar.Visibility = this.TextBlock_StateBar.Visibility = Visibility.Hidden;
                    return;
                }
                else
                {
                    this.StackPanel_Commands.Visibility = this.TextBlock_CurrentChapterName.Visibility =
                        this.TextBlock_MsgBar.Visibility = this.TextBlock_StateBar.Visibility = Visibility.Visible;
                    this.Grid_MainArea.Background = this.MainAreaBrush;
                    this.TextBlock_CurrentChapterName.Text = (this.TreeView_ProjectTree.SelectedItem as TreeViewItem).Header.ToString();
                }
                var sType = selectedTag.ToString().Split('#');
                // 书籍
                if (sType[0] == "HBook")
                {
                    var hb = this.core.BookVector.Find(b => b.BookRef.Id == $"HBook#{sType[1]}");
                    if (hb != null)
                    {
                        var hp = hb.BookRef.HomePage;
                        if (this.RTBPageCacheDict.ContainsKey(hp.Id))
                        {
                            this.CurrentActivePage = this.RTBPageCacheDict[hp.Id];
                        }
                        else
                        {
                            RTBPage np = new RTBPage() { ArticalRef = hp };
                            this.CurrentActivePage = np;
                            this.RTBPageCacheDict.Add(hp.Id, np);
                        }
                        TextRange t = new TextRange(this.CurrentActivePage.RichTextBox_TextArea.Document.ContentStart,
                            this.CurrentActivePage.RichTextBox_TextArea.Document.ContentEnd);
                        t.Load(hp.DocumentMetadata, DataFormats.XamlPackage);
                        this.CurrentActivePage.UpdateRTBStyle();
                        this.Frame_RTB.NavigationService.Navigate(this.CurrentActivePage);
                    }
                    this.CurrentBookId = selectedTag.ToString();
                }
                // 文章
                else
                {
                    var p = this.core.ArticleDict[sType[1]];
                    if (this.RTBPageCacheDict.ContainsKey(sType[1]))
                    {
                        this.CurrentActivePage = this.RTBPageCacheDict[sType[1]];
                    }
                    else
                    {
                        RTBPage np = new RTBPage() { ArticalRef = p };
                        this.CurrentActivePage = np;
                        this.RTBPageCacheDict.Add(sType[1], np);
                    }
                    TextRange t = new TextRange(this.CurrentActivePage.RichTextBox_TextArea.Document.ContentStart,
                           this.CurrentActivePage.RichTextBox_TextArea.Document.ContentEnd);
                    t.Load(p.DocumentMetadata, DataFormats.XamlPackage);
                    this.CurrentActivePage.UpdateRTBStyle();
                    this.Frame_RTB.NavigationService.Navigate(this.CurrentActivePage);
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

        /// <summary>
        /// 命令栏按钮：保存
        /// </summary>
        private void Image_MouseLeftButtonDown_SaveBtn(object sender, MouseButtonEventArgs e)
        {
            this.core.FullCommit();
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
                    // 页保存
                    case Key.S:
                        this.core.PageCommit();
                        break;
                }
            }
        }

        private void Image_MouseLeftButtonUp_NewBtn(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
