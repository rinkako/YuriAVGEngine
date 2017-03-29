using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class ButtonForm : Form
    {
        public ButtonForm()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            this.numericUpDown3.Maximum = Halation.project.Config.GameViewButtonCount - 1;
        }

        /// <summary>
        /// 获取或设置背景资源名
        /// </summary>
        public string GotFileName { get; set; }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == String.Empty)
            {
                MessageBox.Show("正常按钮图不可以为空");
                return;
            }
            if (this.textBox4.Text == String.Empty)
            {
                MessageBox.Show("跳转目标标签不可以为空");
                return;
            }
            Halation.GetInstance().DashButton(this.numericUpDown3.Value.ToString(), this.numericUpDown1.Value.ToString(), this.numericUpDown2.Value.ToString(),
                this.textBox4.Text, this.comboBox1.SelectedItem.ToString(), this.textBox1.Text, this.textBox2.Text, this.textBox3.Text);
            this.Close();
        }

        /// <summary>
        /// 按钮：选择正常图
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择按钮图像", 0);
            prf.ShowDialog(this);
            this.textBox1.Text = this.GotFileName;
        }

        /// <summary>
        /// 按钮：选择悬停图
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择按钮图像", 0);
            prf.ShowDialog(this);
            this.textBox2.Text = this.GotFileName;
        }

        /// <summary>
        /// 按钮：选择按下图
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择按钮图像", 0);
            prf.ShowDialog(this);
            this.textBox3.Text = this.GotFileName;
        }
    }
}
