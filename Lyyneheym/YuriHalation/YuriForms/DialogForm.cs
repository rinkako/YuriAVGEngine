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
        public DialogForm(int line)
        {
            InitializeComponent();
            this.line = line;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            core.DashDialog(line, this.textBox1.Text);
        }

        /// <summary>
        /// 插入的行号
        /// </summary>
        private int line = 0;

        /// <summary>
        /// 控制器引用
        /// </summary>
        private Halation core = Halation.GetInstance();
    }
}
