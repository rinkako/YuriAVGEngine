using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class ButtonForm : Form
    {
        private bool isEditing;

        public ButtonForm(bool isEdit, string id = "0", string x = "0", string y = "0", string target = "", string type = "once", string normal = "", string over = "", string on = "")
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 1;
            this.numericUpDown3.Maximum = Halation.project.Config.GameViewButtonCount - 1;
            this.isEditing = isEdit;
            if (isEdit)
            {
                this.numericUpDown3.Value = Convert.ToInt32(id);
                this.numericUpDown1.Value = Convert.ToInt32(x);
                this.numericUpDown2.Value = Convert.ToInt32(y);
                if (type == "eternal")
                {
                    this.comboBox1.SelectedIndex = 1;
                }
                this.textBox4.Text = target;
                this.textBox1.Text = normal;
                this.textBox2.Text = over;
                this.textBox3.Text = on;
            }
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
                MessageBox.Show(@"正常按钮图不可以为空");
                return;
            }
            if (this.textBox4.Text == String.Empty)
            {
                MessageBox.Show(@"跳转目标标签不可以为空");
                return;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditButton(this.numericUpDown3.Value.ToString(), this.numericUpDown1.Value.ToString(),
                        this.numericUpDown2.Value.ToString(),
                        this.textBox4.Text, this.comboBox1.SelectedItem.ToString(), this.textBox1.Text,
                        this.textBox2.Text, this.textBox3.Text);
            }
            else
            {
                Halation.GetInstance().DashButton(this.numericUpDown3.Value.ToString(), this.numericUpDown1.Value.ToString(),
                        this.numericUpDown2.Value.ToString(),
                        this.textBox4.Text, this.comboBox1.SelectedItem.ToString(), this.textBox1.Text,
                        this.textBox2.Text, this.textBox3.Text);
            }
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
