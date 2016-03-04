using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    /// <summary>
    /// 窗体：显示对话
    /// </summary>
    public partial class DialogForm : Form
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DialogForm(string text)
        {
            InitializeComponent();
            this.Text = text;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Text == "显示对话")
            {
                Halation.GetInstance().DashDialog(this.textBox1.Text);
            }
            else
            {
                Halation.GetInstance().DashScript(this.textBox1.Text);
            }
            this.Close();
        }
    }
}
