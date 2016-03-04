using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class PicResourceForm : Form
    {
        /// <summary>
        /// 图像资源管理窗体
        /// </summary>
        public PicResourceForm(int index)
        {
            InitializeComponent();
            // 加载文件夹
            this.dirInfoPictures = new DirectoryInfo(this.PicDir + @"\pictures");
            this.dirInfoCharacter = new DirectoryInfo(this.PicDir + @"\character");
            this.dirInfoBackground = new DirectoryInfo(this.PicDir + @"\background");
            // 选择默认项
            this.comboBox1.SelectedIndex = index;
        }

        /// <summary>
        /// 重新加载列表
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DirectoryInfo dirInfo = this.dirInfoPictures;
            this.pathVect.Clear();
            switch (this.comboBox1.SelectedIndex)
            {
                case 1:
                    dirInfo = this.dirInfoCharacter;
                    break;
                case 2:
                    dirInfo = this.dirInfoBackground;
                    break;
            }
            // 加载文件
            this.pathVect.Clear();
            this.listBox1.Items.Clear();
            foreach (var f in dirInfo.GetFiles())
            {
                this.listBox1.Items.Add(f.Name);
                this.pathVect.Add(f.FullName);
            }
        }

        /// <summary>
        /// 重新加载图像
        /// </summary>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Image = new Bitmap(this.pathVect[this.listBox1.SelectedIndex]);
            this.panel1.AutoScrollPosition = new Point(0, 0);
            this.pictureBox1.Location = new Point(0, 0);
        }

        /// <summary>
        /// 图片文件夹绝对路径
        /// </summary>
        private string PicDir = Halation.projectFolder + @"\PictureAssets";

        /// <summary>
        /// 图片文件夹
        /// </summary>
        DirectoryInfo dirInfoPictures;

        /// <summary>
        /// 立绘文件夹
        /// </summary>
        DirectoryInfo dirInfoCharacter;

        /// <summary>
        /// 背景文件夹
        /// </summary>
        DirectoryInfo dirInfoBackground;

        /// <summary>
        /// 路径信息向量
        /// </summary>
        List<string> pathVect = new List<string>();
    }
}
