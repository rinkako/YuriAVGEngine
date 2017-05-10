using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class LabelForm : Form
    {
        private readonly bool isEditing;

        public LabelForm(bool isEdit, string name = "")
        {
            InitializeComponent();
            this.textBox1.Text = (this.isEditing = isEdit) ? name : String.Empty;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == String.Empty)
            {
                MessageBox.Show(@"请填写标签");
                return;
            }
            if (!Halation.IsValidVarname(this.textBox1.Text) && this.textBox1.Text != @"~finalizer")
            {
                MessageBox.Show(@"标签名只能是由下划线、数字和字母组成，并且不能以数字开头。");
                return;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditLabel(this.textBox1.Text);
            }
            else
            {
                Halation.GetInstance().DashLabel(this.textBox1.Text);
            }
            this.Close();
        }
    }
}
