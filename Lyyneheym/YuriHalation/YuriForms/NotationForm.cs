using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class NotationForm : Form
    {
        private bool isEditing;

        public NotationForm(bool isEdit)
        {
            InitializeComponent();
            this.isEditing = isEdit;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditNotation(this.textBox1.Text);
            }
            else
            {
                Halation.GetInstance().DashNotation(this.textBox1.Text);
            }
            this.Close();
        }
    }
}
