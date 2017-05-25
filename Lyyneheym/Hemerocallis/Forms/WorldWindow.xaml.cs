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
using Yuri.Hemerocallis.Entity;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// WorldWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorldWindow : MetroWindow
    {
        /// <summary>
        /// 后台的引用
        /// </summary>
        private static readonly Controller core = Controller.GetInstance();

        /// <summary>
        /// 启动里程碑界面的书籍ID
        /// </summary>
        private readonly HBook BookRef;

        /// <summary>
        /// 里程碑向量
        /// </summary>
        private readonly List<HMilestone> MsList;

        /// <summary>
        /// 文章字数缓存
        /// </summary>
        private int wordCountCache;

        /// <summary>
        /// 构造世界设定窗体
        /// </summary>
        /// <param name="bookId">书籍的唯一标识符</param>
        public WorldWindow(string bookId)
        {
            InitializeComponent();
            this.BookRef = WorldWindow.core.BookVector.Find(t => t.BookRef.Id == bookId).BookRef;
            this.Title = $"World - [{this.BookRef.Name}]";
            this.ComboBox_Chapters.Items.Clear();
            WorldWindow.core.RetrieveOffspringArticle(this.BookRef.HomePage.Id, true, out var chapterVec);
            foreach (var ci in chapterVec)
            {
                this.ComboBox_Chapters.Items.Add(ci.Name);
            }
            WorldWindow.core.RetrieveMilestone(this.BookRef.Id, t => true, out this.MsList);
            foreach (var msi in this.MsList)
            {
                var lbi = new ListBoxItem();
                var lbiGrid = new Grid();
                var lbiLabel = new Label() { Content = msi.Detail };
                var flagEllipse = new Ellipse()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Fill = new SolidColorBrush(Colors.Green),
                    Margin = new Thickness(0, 0, 10, 0),
                    MaxHeight = 12,
                    MaxWidth = 12,
                    Width = 25,
                    Visibility = Visibility.Hidden
                };
                lbiGrid.Children.Add(lbiLabel);
                lbiGrid.Children.Add(flagEllipse);
                lbi.Content = lbiGrid;
                this.Listbox_Milestone.Items.Add(lbi);
            }
            this.Listbox_Milestone.SelectedIndex = 0;
        }

        /// <summary>
        /// 事件：里程碑选择改变
        /// </summary>
        private void Listbox_Milestone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Listbox_Milestone.SelectedIndex == -1)
            {
                return;
            }
            if (e.AddedItems[0] == this.Listbox_Milestone.Items[0])
            {
                this.Button_Milestone_Delete.Visibility = Visibility.Hidden;
                this.ComboBox_Chapters.IsEnabled = true;
            }
            else
            {
                this.Button_Milestone_Delete.Visibility = Visibility.Visible;
                var msObj = this.MsList[this.Listbox_Milestone.SelectedIndex - 1];
                var articleObj = WorldWindow.core.ArticleDict[msObj.ArticleId];
                for (int i = 0; i < this.ComboBox_Chapters.Items.Count; i++)
                {
                    if (this.ComboBox_Chapters.Items[i].ToString() == articleObj.Name)
                    {
                        this.ComboBox_Chapters.SelectedIndex = i;
                        break;
                    }
                }
                this.ComboBox_Chapters.IsEnabled = false;
                this.TextBox_MilestoneName.Text = msObj.Detail;
                this.Numberic_WordThreshold.Value = msObj.Destination;
                this.wordCountCache = StatisticsWindow.GetSimpleWordCountByMetadata(articleObj);
                this.Label_WordNow.Content = this.wordCountCache.ToString();
                this.ProgessBar_Process.Value = this.wordCountCache / (double)this.Numberic_WordThreshold.Value * 100.0;
                this.MetroProgressBar_Working.Visibility =
                    this.wordCountCache >= (double) this.Numberic_WordThreshold.Value ? Visibility.Hidden : Visibility.Visible;
            }
        }
        
        /// <summary>
        /// 事件：更改当前里程碑字数阈值
        /// </summary>
        private void Numberic_WordThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (this.ProgessBar_Process != null)
            {
                this.ProgessBar_Process.Value = this.wordCountCache / (double) e.NewValue * 100.0;
            }
        }

        /// <summary>
        /// 事件：更改新建里程碑的文章绑定
        /// </summary>
        private void ComboBox_Chapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBox_Chapters.SelectedIndex == -1)
            {
                return;
            }
            WorldWindow.core.RetrieveOffspringArticle(this.BookRef.HomePage.Id, true, out var chapterVec);
            var ao = chapterVec[this.ComboBox_Chapters.SelectedIndex];
            this.wordCountCache = StatisticsWindow.GetSimpleWordCountByMetadata(ao);
            this.Label_WordNow.Content = this.wordCountCache.ToString();
            this.ProgessBar_Process.Value = this.wordCountCache / (double)this.Numberic_WordThreshold.Value * 100.0;
            if (this.wordCountCache >= (double) this.Numberic_WordThreshold.Value)
            {
                this.MetroProgressBar_Working.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 按钮：保存里程碑
        /// </summary>
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // 修改
            if (this.Listbox_Milestone.SelectedIndex > 0)
            {
                // 合法性检查
                var tname = this.TextBox_MilestoneName.Text.Trim();
                if (this.DateTimePicker_endTime.SelectedDate == null ||
                    this.DateTimePicker_endTime.SelectedTime == null ||
                    this.Numberic_WordThreshold.Value == null ||
                    tname == String.Empty)
                {
                    MessageBox.Show(@"请完整填写");
                    return;
                }
            }
            // 新建
            else
            {
                // 合法性检查
                var tname = this.TextBox_MilestoneName.Text.Trim();
                if (this.DateTimePicker_endTime.SelectedDate == null ||
                    this.DateTimePicker_endTime.SelectedTime == null ||
                    this.Numberic_WordThreshold.Value == null ||
                    tname == String.Empty)
                {
                    MessageBox.Show(@"请完整填写");
                    return;
                }
                // 重复性检查
                if (this.BookRef.Milestones.Any(ems => ems.Detail == tname))
                {
                    MessageBox.Show(@"重复的里程碑名");
                    return;
                }
                // 更新后台
                WorldWindow.core.RetrieveOffspringArticle(this.BookRef.HomePage.Id, true, out var chapterVec);
                var ao = chapterVec[this.ComboBox_Chapters.SelectedIndex];
                var msObj = WorldWindow.core.AddMilestone(MilestoneType.Aritical, ao.Id, (long)this.Numberic_WordThreshold.Value,
                    tname, DateTime.Now, (DateTime)this.DateTimePicker_endTime.SelectedDate);
                // 更新前台
                var lbi = new ListBoxItem();
                var lbiGrid = new Grid();
                var lbiLabel = new Label() { Content = tname };
                var flagEllipse = new Ellipse()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Fill = new SolidColorBrush(Colors.Green),
                    Margin = new Thickness(0, 0, 10, 0),
                    MaxHeight = 12,
                    MaxWidth = 12,
                    Width = 25,
                    Visibility = Visibility.Hidden
                };
                lbiGrid.Children.Add(lbiLabel);
                lbiGrid.Children.Add(flagEllipse);
                lbi.Content = lbiGrid;
                this.Listbox_Milestone.Items.Add(lbi);
            }
        }
    }
}
