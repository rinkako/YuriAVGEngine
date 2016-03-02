using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yuri.YuriForms
{
    /// <summary>
    /// 窗体：显示对话
    /// </summary>
    public partial class DialogForm : Form
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DialogForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            core.DashDialog(this.textBox1.Text);
            this.Close();
        }

        /// <summary>
        /// 控制器引用
        /// </summary>
        private Halation core = Halation.GetInstance();
    }
}
