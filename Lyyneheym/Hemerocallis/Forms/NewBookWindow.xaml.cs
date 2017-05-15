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
    /// NewBookWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewBookWindow : MetroWindow
    {
        /// <summary>
        /// 后台控制器引用
        /// </summary>
        private readonly Controller core = Controller.GetInstance();

        /// <summary>
        /// 构造一个新建书籍窗体
        /// </summary>
        public NewBookWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            var bname = this.TextBox_BookName.Text.Trim();
            if (bname == String.Empty)
            {
                MessageBox.Show("书名不可以为空");
                return;
            }
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
