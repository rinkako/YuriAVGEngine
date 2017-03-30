using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YuriHalation.YuriForms
{
    public partial class SCameraForm : Form
    {
        public SCameraForm()
        {
            InitializeComponent();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// 单选：进入黑场
        /// </summary>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton1.Checked;
        }

        /// <summary>
        /// 单选：退出黑场进入场景
        /// </summary>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton2.Checked;
        }

        /// <summary>
        /// 单选：直接退出黑场
        /// </summary>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton3.Checked;
        }

        /// <summary>
        /// 单选：平移镜头
        /// </summary>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = this.radioButton4.Checked;
            this.label3.Visible = this.textBox1.Visible = !this.radioButton4.Checked;
        }

        /// <summary>
        /// 单选：调整焦距
        /// </summary>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = this.radioButton5.Checked;
            this.label3.Visible = this.textBox1.Visible = this.radioButton5.Checked;
        }
    }
}
