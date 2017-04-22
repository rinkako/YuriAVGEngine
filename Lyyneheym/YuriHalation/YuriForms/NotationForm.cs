using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class NotationForm : Form
    {
        private bool isEditing;

        public NotationForm(bool isEdit, string preStr = "")
        {
            InitializeComponent();
            this.isEditing = isEdit;
            if (isEdit)
            {
                this.textBox1.Text = preStr;
            }
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
