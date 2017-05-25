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
        private HBook BookRef;

        /// <summary>
        /// 里程碑向量
        /// </summary>
        private List<HMilestone> MsList;

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
                    Width = 25
                };
                lbiGrid.Children.Add(lbiLabel);
                lbiGrid.Children.Add(flagEllipse);
                lbi.Content = lbiGrid;
                this.Listbox_Milestone.Items.Add(lbi);
            }
        }

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
                this.Label_WordNow = 
            }
        }
    }
}
