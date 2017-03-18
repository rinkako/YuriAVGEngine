using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuri.PlatformCore;

namespace Yuri.PageView
{
    /// <summary>
    /// SLPage.xaml 的交互逻辑
    /// </summary>
    public partial class SLPage : Page
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SLPage()
        {
            InitializeComponent();
            var bsp = ResourceManager.GetInstance().GetPicture("SaveLoad.jpg", new Int32Rect(-1, 0, 0, 0));
            this.SL_MainGrid.Background = new ImageBrush(bsp.SpriteBitmapImage);
            this.LoadSaveCache();
        }

        private void LoadSaveCache()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(GlobalDataContainer.GAME_SAVE_DIR);
            saveList = new List<FileInfo>();
            for (int i = 0; i < 16; i++)
            {
                saveList.Add(null);
            }
            foreach (var fInfo in dirInfo.GetFiles())
            {
                if (fInfo.Name.StartsWith("snapshot-") && String.Compare(fInfo.Extension, ".jpg", true) == 0)
                {
                    saveList[Convert.ToInt32(fInfo.Name.Split('-')[1]) - 1] = fInfo;
                }
            }
        }

        private List<FileInfo> saveList;

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                int pointId = Convert.ToInt32(((Button)sender).Name.Split('_')[1]) - 1;
                if (saveList[pointId] != null)
                {
                    var p = ResourceManager.GetInstance().GetPicture(saveList[pointId].Name, new Int32Rect(-1, 0, 0, 0));
                    this.SL_SnapshotImage.Source = p.SpriteBitmapImage;
                    var timeItems = saveList[pointId].Name.Split('-');
                    var timeStr = String.Format("{0}-{1}-{2} {3}:{4}:{5}",
                        timeItems[2], timeItems[3], timeItems[4], timeItems[5], timeItems[6], timeItems[7]);
                    this.SL_TimeStampTextBlock.Text = timeStr;
                    this.SL_TimeStampTextBlock.Visibility = Visibility.Visible;
                }

            }
            catch (Exception ex)
            {
                // empty
            }
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            this.SL_SnapshotImage.Source = null;
            this.SL_TimeStampTextBlock.Text = String.Empty;
            this.SL_TimeStampTextBlock.Visibility = Visibility.Hidden;
        }
    }
}
