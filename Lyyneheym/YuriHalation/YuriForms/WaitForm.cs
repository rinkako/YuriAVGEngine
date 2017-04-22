using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class WaitForm : Form
    {
        private bool isEditing;

        public WaitForm(bool isEdit, int preWait = 0)
        {
            InitializeComponent();
            this.isEditing = isEdit;
            if (isEdit)
            {
                this.numericUpDown1.Value = Math.Max(this.numericUpDown1.Minimum, Math.Min(this.numericUpDown1.Maximum, preWait));
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditWait(this.numericUpDown1.Value.ToString());
            }
            else
            {
                Halation.GetInstance().DashWait(this.numericUpDown1.Value.ToString());
            }
            this.Close();
        }

    }
}
