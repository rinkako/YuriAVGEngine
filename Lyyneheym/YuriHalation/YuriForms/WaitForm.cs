using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class WaitForm : Form
    {
        public WaitForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashWait(this.numericUpDown1.Value.ToString());
            this.Close();
        }

    }
}
