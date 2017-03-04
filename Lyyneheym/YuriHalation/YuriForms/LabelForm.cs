using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class LabelForm : Form
    {
        public LabelForm()
        {
            InitializeComponent();
            this.textBox1.Text = "";
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "")
            {
                Halation.GetInstance().DashLabel(this.textBox1.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("请填写标签");
            }
        }
    }
}
