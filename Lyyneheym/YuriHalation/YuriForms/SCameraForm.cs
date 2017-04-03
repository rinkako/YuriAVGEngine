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
            if (this.radioButton1.Checked)
            {
                this.scameraAct = "blackframe";
            }
        }

        /// <summary>
        /// 单选：退出黑场进入场景
        /// </summary>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton2.Checked;
            if (this.radioButton2.Checked)
            {
                this.scameraAct = "enterscene";
            }
        }

        /// <summary>
        /// 单选：直接退出黑场
        /// </summary>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton3.Checked;
            if (this.radioButton3.Checked)
            {
                this.scameraAct = "outblackframe";
            }
        }

        /// <summary>
        /// 单选：平移镜头
        /// </summary>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = this.radioButton4.Checked;
            this.label3.Visible = this.textBox1.Visible = !this.radioButton4.Checked;
            if (this.radioButton4.Checked)
            {
                this.scameraAct = "translate";
            }
        }

        /// <summary>
        /// 单选：调整焦距
        /// </summary>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = this.radioButton5.Checked;
            this.label3.Visible = this.textBox1.Visible = this.radioButton5.Checked;
            if (this.radioButton5.Checked)
            {
                this.scameraAct = "focus";
            }
        }

        /// <summary>
        /// 单选：复位
        /// </summary>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = !this.radioButton6.Checked;
            if (this.radioButton6.Checked)
            {
                this.scameraAct = "reset";
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
             if (this.radioButton5.Checked && this.textBox1.Text.Trim() == String.Empty)
            {
                MessageBox.Show("聚焦比不能为空，且应该是 [0.5, 2.5] 区间中的值");
                return;
            }
            Halation.GetInstance().DashSCamera(this.scameraAct,
                this.numericUpDown1.Value.ToString(), this.numericUpDown2.Value.ToString(), this.textBox1.Text.Trim());
            this.Close();
        }

        /// <summary>
        /// 当前选中的动作名
        /// </summary>
        private string scameraAct = "blackframe";
    }
}
