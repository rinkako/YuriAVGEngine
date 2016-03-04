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
    public partial class VarForm : Form
    {
        public VarForm()
        {
            InitializeComponent();

            // 窗体加载时默认的选择情况
            this.radioButton1.Checked = true;
            this.radioButton3.Checked = true;
            this.radioButton9.Checked = true;
            this.textBox1.Enabled = false;
            this.textBox3.Enabled = false;
            this.numericUpDown2.Enabled = false;
            this.numericUpDown3.Enabled = false;
            this.comboBox2.Enabled = false;
            // 加载全局变量表
            List<string> globalVarList = null;// core.getGlobalVar();
            // 如果没有全局变量，那就封锁这个选项
            if (globalVarList.Count == 0)
            {
                this.radioButton2.Checked = true;
                this.radioButton1.Enabled = false;
                this.radioButton10.Enabled = false;
            }
            else
            {
                foreach (string s in globalVarList)
                {
                    this.comboBox1.Items.Add(s);
                    this.comboBox2.Items.Add(s);
                }
                this.comboBox1.SelectedIndex = 0;
                this.comboBox2.SelectedIndex = 0;
            }
            // 放置焦点
            this.numericUpDown1.Focus();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBox1.Enabled = this.radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.Enabled = this.radioButton2.Checked;
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.radioButton9.Checked;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBox2.Enabled = this.radioButton10.Checked;
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox3.Enabled = this.radioButton11.Checked;
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown2.Enabled = this.numericUpDown3.Enabled = this.radioButton12.Checked;
        }
    }
}
