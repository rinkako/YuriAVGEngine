using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class NotationForm : Form
    {
        public NotationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashNotation(this.textBox1.Text);
            this.Close();
        }
    }
}
