using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class CStandForm : Form
    {
        public CStandForm()
        {
            InitializeComponent();
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown4.Enabled = this.radioButton2.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.numericUpDown2.Enabled = this.radioButton1.Checked;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
             if (this.textBox1.Text == String.Empty)
            {
                MessageBox.Show("请选择图像");
                return;
            }
            var nameItem = this.textBox1.Text.Split(new char[] {'_', '.'});
            if (this.radioButton1.Checked)
            {
                Halation.GetInstance().DashCstand(this.numericUpDown3.Value.ToString(), nameItem[0], nameItem[1], this.numericUpDown1.Value.ToString(), this.numericUpDown2.Value.ToString(), String.Empty);
            }
            else
            {
                Halation.GetInstance().DashCstand(this.numericUpDown3.Value.ToString(), nameItem[0], nameItem[1], String.Empty, String.Empty, this.numericUpDown4.Value.ToString());
            }
            this.Close();
        }

        /// <summary>
        /// 按钮：选择图像
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择立绘", 1);
            prf.ShowDialog(this);
            this.textBox1.Text = this.GotFileName;
        }

        /// <summary>
        /// 获取或设置背景资源名
        /// </summary>
        public string GotFileName { get; set; }
    }
}
