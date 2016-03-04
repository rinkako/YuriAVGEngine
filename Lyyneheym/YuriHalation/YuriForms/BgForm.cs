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
    public partial class BgForm : Form
    {
        public BgForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：选择图像
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择背景", 2);
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
                MessageBox.Show("请选择图像");
                return;
            }
            Halation.GetInstance().DashBg(this.radioButton1.Checked ? "0" : "1", this.textBox1.Text);
            this.Close();
        }

        /// <summary>
        /// 获取或设置背景资源名
        /// </summary>
        public string GotFileName { get; set; }
    }
}
