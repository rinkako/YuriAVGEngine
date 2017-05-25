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
using System.Threading;
using System.Windows.Shapes;
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
            this.StartPageViewItem.IsSelected = true;
            
        }

        /// <summary>
        /// 获取首页菜单项的引用
        /// </summary>
        public TreeViewItem StartPageViewItem { get; private set; }

        /// <summary>
        /// 获取当前活跃的RTB页
        /// </summary>
        public RTBPage CurrentActivePage { get; private set; } = null;

        /// <summary>
        /// 获取当前是否选中根目录的书
        /// </summary>
        public bool IsChoosingBook
        {
            get
            {
                var t = this.TreeView_ProjectTree.SelectedItem;
                return t != null && (t as TreeViewItem).Tag.ToString().StartsWith("HBook", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 获取当前是否选中首页
        /// </summary>
        public bool IsChoosingIndex
        {
            get
            {
                var t = this.TreeView_ProjectTree.SelectedItem;
                return t != null && (t as TreeViewItem).Tag.ToString() == "HemeIndexPage";
            }
        }

        /// <summary>
        /// RTB页缓存字典
        /// </summary>
        public readonly Dictionary<string, RTBPage> RTBPageCacheDict = new Dictionary<string, RTBPage>();

        #region 辅助函数


        #endregion

        #region 标题栏和工程树
        /// <summary>
        /// 获取或设置当前主编辑区的背景画笔
        /// </summary>
        public Brush MainAreaBrush { get; set; }

        /// <summary>
        /// 获取或设置首页的背景画笔
        /// </summary>
        public ImageBrush IndexBackgroundBrush { get; set; }

        /// <summary>
        /// 获取或设置首页页面的引用
        /// </summary>
        public IndexPage IndexPageRef { get; set; }

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
        /// 工程树右键菜单：删除
        /// </summary>
        private void TreeViewContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            this.Button_Click_Menu_Delete(null, null);
        }

        /// <summary>
        /// 工程树：右键按下
        /// </summary>
        public void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

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
                    this.CurrentBookId = this.CurrentActivePage.ArticalRef.BookId;
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
                    var pid = $"HArticle#{sType[1]}";
                    var p = this.core.ArticleDict[pid];
                    this.CurrentBookId = p.BookId;
                    if (this.RTBPageCacheDict.ContainsKey(pid))
                    {
                        this.CurrentActivePage = this.RTBPageCacheDict[pid];
                    }
                    else
                    {
                        RTBPage np = new RTBPage() { ArticalRef = p };
                        this.CurrentActivePage = np;
                        this.RTBPageCacheDict.Add(pid, np);
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
                tvi.PreviewMouseRightButtonDown += this.TreeViewItem_PreviewMouseRightButtonDown;
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
                    tvi.PreviewMouseRightButtonDown += this.TreeViewItem_PreviewMouseRightButtonDown;
                }
            }
            // 起始页
            this.IndexBackgroundBrush =
                new ImageBrush(new BitmapImage(new Uri(App.ParseURIToURL("Assets/bg_Index.jpg"), UriKind.RelativeOrAbsolute)))
                {
                    Stretch = Stretch.UniformToFill,
                    AlignmentX = AlignmentX.Left
                };
            this.IndexPageRef = new IndexPage();
            this.StartPageViewItem = new TreeViewItem() { Header = "起始页", Tag = "HemeIndexPage" };
            this.TreeView_ProjectTree.Items.Insert(0, this.StartPageViewItem);
            this.StartPageViewItem.PreviewMouseRightButtonDown += this.TreeViewItem_PreviewMouseRightButtonDown;
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
        /// 侧边菜单按钮：导出文本文件
        /// </summary>
        private void Button_Click_Menu_Txt(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            try
            {
                if (this.CurrentActivePage != null)
                {
                    var fd = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = @"文本文件|*.txt;",
                        FileName = this.CurrentActivePage.ArticalRef?.Name
                    };
                    fd.ShowDialog();
                    if (fd.FileName != String.Empty)
                    {
                        var pureTxt = this.CurrentActivePage.GetText();
                        FileStream fs = new FileStream(fd.FileName, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(pureTxt);
                        sw.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"导出TXT文件失败" + Environment.NewLine + ex);
            }
        }

        /// <summary>
        /// 侧边菜单按钮：导出halation文件
        /// </summary>
        private void Button_Click_Menu_Halation(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            MessageBox.Show(@"暂未实装本功能");
        }

        /// <summary>
        /// 侧边菜单按钮：关于项目
        /// </summary>
        private void Button_Click_Menu_About(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            new AboutWindow().ShowDialog();
        }

        /// <summary>
        /// 侧边菜单按钮：统计
        /// </summary>
        private void Button_Click_Menu_Statistics(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            new StatisticsWindow().ShowDialog();
        }

        /// <summary>
        /// 侧边菜单按钮：删除
        /// </summary>
        private void Button_Click_Menu_Delete(object sender, RoutedEventArgs e)
        {
            this.Flyout_Menu.IsOpen = false;
            // 分析Tag
            var selectedTag = (this.TreeView_ProjectTree.SelectedItem as TreeViewItem)?.Tag;
            if (selectedTag == null)
            {
                return;
            }
            if (selectedTag.ToString() == "HemeIndexPage")
            {
                MessageBox.Show("你为什么会想删掉这个页面呢……");
                return;
            }
            var sType = selectedTag.ToString().Split('#');
            if (sType[0] == "HBook")
            {
                var dr1 = MessageBox.Show("你正在删除书籍[" + (this.TreeView_ProjectTree.SelectedItem as TreeViewItem).Header +
                    "]，这会使得该书籍下的所有文章都被永久删除。你确定要这么做吗？", "确认删除动作", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (dr1 == MessageBoxResult.Yes)
                {
                    Thread.Sleep(1000);
                    var dr2 = MessageBox.Show("删除动作不能被回滚。你真的真的要这么做吗？",
                    "确认删除动作", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (dr2 == MessageBoxResult.Yes)
                    {
                        // 提交后台
                        this.core.DeleteBook(selectedTag.ToString());
                        // 刷新
                        this.CurrentActivePage = null;
                        this.ReDrawProjectTree();
                        this.StartPageViewItem.IsSelected = true;
                    }
                }
            }
            else
            {
                
            }
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

        /// <summary>
        /// 命令栏按钮：管理工程
        /// </summary>
        private void Image_MouseLeftButtonUp_WorldBtn(object sender, MouseButtonEventArgs e)
        {
            new WorldWindow(this.CurrentBookId).ShowDialog();
        }

        /// <summary>
        /// 命令栏按钮：添加贴纸
        /// </summary>
        private void Image_MouseLeftButtonUp_TipBtn(object sender, MouseButtonEventArgs e)
        {
            new AddTipWindow().ShowDialog();
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
                        if (this.IsChoosingIndex)
                        {
                            new NewBookWindow("新书", "取一个书名吧：", String.Empty, "我的新坑").ShowDialog();
                        }
                        else
                        {
                            new NewBookWindow("新建", "取一个章节名吧：",
                                (this.TreeView_ProjectTree.SelectedItem as TreeViewItem).Tag.ToString(), String.Empty).ShowDialog();
                        }
                        break;
                    // 页保存
                    case Key.S:
                        this.core.PageCommit();
                        break;
                }
            }
            // 其他按键
            if (e.Key == Key.F2)
            {
                if (!this.IsChoosingIndex)
                {
                    new NewBookWindow("重命名", "你想要的名字是：", (this.TreeView_ProjectTree.SelectedItem as TreeViewItem).Tag.ToString(),
                        (this.TreeView_ProjectTree.SelectedItem as TreeViewItem).Header.ToString()).ShowDialog();
                }
            }
        }

        private void Image_MouseLeftButtonUp_NewBtn(object sender, MouseButtonEventArgs e)
        {
            FileStream f = new FileStream("ff.zip", FileMode.Create);
            TextRange st = new TextRange(this.CurrentActivePage.RichTextBox_FlowDocument.ContentStart, this.CurrentActivePage.RichTextBox_FlowDocument.ContentEnd);
            st.Save(f, DataFormats.XamlPackage);
        }

        /// <summary>
        /// 事件：主窗体关闭
        /// </summary>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.core.FullCommit();
        }



        /// <summary>
        /// 在WPF可视化树上搜索指定类型的父节点
        /// </summary>
        /// <typeparam name="T">T是父节点的类型</typeparam>
        /// <param name="source">开始冒泡搜索的可视化依赖项对象</param>
        /// <returns>冒泡过程中遇到的第一个满足类型条件的依赖项父节点</returns>
        public static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
        }

        /// <summary>
        /// 在WPF可视化树上搜索指定类型的父节点
        /// </summary>
        /// <typeparam name="T">T是父节点的类型</typeparam>
        /// <param name="source">开始冒泡搜索的可视化依赖项对象</param>
        /// <returns>冒泡过程中遇到的第一个满足类型条件的依赖项父节点</returns>
        public static DependencyObject VisualUpwardSearch<T>(DependencyObject source, string tname)
        {
            while (source != null && source.GetType() != typeof(T) && (source as FrameworkElement).Name != tname)
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
        }
        
    }
}
