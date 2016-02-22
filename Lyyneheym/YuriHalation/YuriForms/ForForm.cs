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
    public partial class ForForm : Form
    {
        private CondLoopType condLoopType = CondLoopType.CLT_FOREVER;
        private string operand = "___KAGA_FOREVER";

        public ForForm()
        {
            InitializeComponent();
            // 设置初始状态
            this.radioButton1.Checked = true;
            this.comboBox1.Enabled = false;
            this.textBox1.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.condLoopType = CondLoopType.CLT_FOREVER;
                this.operand = "___KAGA_FOREVER";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                this.comboBox1.Enabled = true;
                this.condLoopType = CondLoopType.CLT_SWITCH;
            }
            else
            {
                this.comboBox1.Enabled = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3.Checked)
            {
                this.textBox1.Enabled = true;
                this.condLoopType = CondLoopType.CLT_EXPRESSION;
            }
            else
            {
                this.textBox1.Enabled = false;
            }
        }
    }

    /// <summary>
    /// 枚举类型：条件循环类型
    /// </summary>
    public enum CondLoopType
    {
        CLT_FOREVER,
        CLT_SWITCH,
        CLT_EXPRESSION
    }
}
