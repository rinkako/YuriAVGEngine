using System;
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
    public partial class PicturesForm : Form
    {
        public PicturesForm()
        {
            InitializeComponent();
            this.numericUpDown3.Maximum = Halation.project.Config.PictureLayerCount - 1;
        }

        /// <summary>
        /// 获取或设置背景资源名
        /// </summary>
        public string GotFileName { get; set; }

        /// <summary>
        /// 按钮：选择图像
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择显示图片", 0);
            prf.ShowDialog(this);
            this.textBox1.Text = this.GotFileName;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("请选择图片文件");
                return;
            }
            Halation.GetInstance().DashPicture(this.numericUpDown3.Value.ToString(),
                this.textBox1.Text, this.numericUpDown1.Value.ToString(), this.numericUpDown2.Value.ToString(),
                this.numericUpDown4.Value.ToString(), this.numericUpDown5.Value.ToString(),
                this.numericUpDown7.Value.ToString(), this.numericUpDown6.Value.ToString());
            this.Close();
        }
    }
}
