using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    /// <summary>
    /// 窗体：显示对话
    /// </summary>
    public partial class DialogForm : Form
    {
        /// <summary>
        /// 是否编辑模式
        /// </summary>
        private bool isEditing;

        /// <summary>
        /// 构造器
        /// </summary>
        public DialogForm(string text, bool isEdit, string editStr = "")
        {
            InitializeComponent();
            this.Text = text;
            this.isEditing = isEdit;
            if (this.isEditing)
            {
                this.textBox1.Text = editStr;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isEditing)
            {
                if (this.Text == "显示对话")
                {
                    Halation.GetInstance().DashEditDialog(this.textBox1.Text);
                }
                else
                {
                    Halation.GetInstance().DashEditScript(this.textBox1.Text);
                }
            }
            else
            {
                if (this.Text == "显示对话")
                {
                    Halation.GetInstance().DashDialog(this.textBox1.Text);
                }
                else
                {
                    Halation.GetInstance().DashScript(this.textBox1.Text);
                }
            }
            this.Close();
        }

        private void DialogForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Enter))
            {
                this.button1_Click(null, null);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Enter))
            {
                this.button1_Click(null, null);
            }
        }
    }
}
