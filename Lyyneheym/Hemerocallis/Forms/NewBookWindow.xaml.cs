using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// NewBookWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewBookWindow : MetroWindow
    {
        /// <summary>
        /// 后台控制器引用
        /// </summary>
        private readonly Controller core = Controller.GetInstance();

        /// <summary>
        /// 父级的唯一标识符
        /// </summary>
        private string parentId = String.Empty;

        /// <summary>
        /// 构造一个新建书籍窗体
        /// </summary>
        /// <param name="title">窗体标题</param>
        /// <param name="label">窗体说明文本</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="box">文本框内容</param>
        public NewBookWindow(string title, string label, string parentId, string box)
        {
            InitializeComponent();
            this.Label_Title.Content = title;
            this.Label_Description.Content = label;
            this.Title = title;
            this.TextBox_BookName.Text = box;
            this.parentId = parentId;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            var bname = this.TextBox_BookName.Text.Trim();
            if (bname == String.Empty)
            {
                MessageBox.Show("名字不可以为空");
                return;
            }
            // 书籍
            if (this.Title == "新书")
            {
                // 更新后台
                var adb = this.core.AddBook(bname);
                // 更新前台
                var item = new TreeViewItem()
                {
                    Header = bname,
                    Tag = adb
                };
                this.core.mainWndRef.TreeView_ProjectTree.Items.Add(item);
                item.IsSelected = true;
                item.PreviewMouseRightButtonDown += this.core.mainWndRef.TreeViewItem_PreviewMouseRightButtonDown;
            }
            // 文章
            else if (this.Title == "新建")
            {
                // 更新后台


            }
            // 重命名
            else
            {
                var flag = this.parentId.StartsWith("HBook")
                    ? this.core.RenameBook(this.parentId, bname)
                    : this.core.RenameArtical(this.parentId, bname);
                if (flag)
                {
                    (this.core.mainWndRef.TreeView_ProjectTree.SelectedItem as TreeViewItem).Header = bname;
                    this.core.mainWndRef.TextBlock_CurrentChapterName.Text = bname;
                }
            }
            this.Close();
        }

        /// <summary>
        /// 事件：窗体按钮按下
        /// </summary>
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.Enter:
                    this.Button_OK_Click(null, null);
                    break;
            }
        }
    }
}
