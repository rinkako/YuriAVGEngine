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
    public partial class IfForm : Form
    {
        /// <summary>
        /// 窗体：条件分支
        /// </summary>
        public IfForm()
        {
            InitializeComponent();
            // 处理开关操作的可选性
            var switList = Halation.project.SwitchDescriptorList;
            for (int i = 0; i < switList.Count; i++)
            {
                this.comboBox3.Items.Add(String.Format("{0:D4}:{1}", i, switList[i]));
            }
            if (this.comboBox3.Items.Count > 0)
            {
                this.comboBox3.SelectedIndex = 0;
                this.comboBox4.SelectedIndex = 0;
            }
            else
            {
                this.radioButton12.Enabled = false;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 处理条件表达
            string op1, op2, opr, expr;
            if (this.checkBox2.Checked)
            {
                if (this.textBox2.Text == "")
                {
                    MessageBox.Show("请填写表达式");
                    return;
                }
                expr = this.textBox2.Text;
                op1 = op2 = opr = "";
            }
            else
            {
                // 处理操作数1
                if (this.radioButton1.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox4.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op1 = "1#" + this.textBox4.Text;
                }
                else if (this.radioButton2.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox1.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op1 = "2#" + this.textBox1.Text;
                }
                else
                {
                    op1 = "3#" + (Convert.ToInt32(this.comboBox3.SelectedItem.ToString().Split(':')[0])).ToString();
                }
                // 处理操作符号
                if (this.radioButton3.Checked)
                {
                    opr = "==";
                }
                else if (this.radioButton4.Checked)
                {
                    opr = "<>";
                }
                else if (this.radioButton5.Checked)
                {
                    opr = ">";
                }
                else if (this.radioButton6.Checked)
                {
                    opr = "<";
                }
                else if (this.radioButton7.Checked)
                {
                    opr = ">=";
                }
                else
                {
                    opr = "<=";
                }
                // 处理操作数2
                if (this.radioButton9.Checked)
                {
                    op2 = "1#" + this.numericUpDown1.Value.ToString();
                }
                else if (this.radioButton14.Checked)
                {
                    op2 = "2#" + this.textBox6.Text.Replace('#', ' ');
                }
                else if (this.radioButton10.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox5.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op2 = "3#" + this.textBox5.Text;
                }
                else if (this.radioButton11.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox3.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op2 = "4#" + this.textBox3.Text;
                }
                else
                {
                    op2 = "5#" + this.comboBox4.SelectedItem.ToString();
                }
                expr = "";
            }
            Halation.GetInstance().DashIf(this.checkBox1.Checked, expr, op1, opr, op2);
            this.Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox2.Enabled = true;
                this.groupBox1.Enabled = this.groupBox2.Enabled = this.groupBox3.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = false;
                this.groupBox1.Enabled = this.groupBox2.Enabled = this.groupBox3.Enabled = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox4.Enabled = this.radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.Enabled = this.radioButton2.Checked;
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBox3.Enabled = this.radioButton13.Enabled = this.radioButton12.Checked;
            this.comboBox4.Enabled = this.radioButton12.Checked;
            this.radioButton4.Enabled = this.radioButton5.Enabled = this.radioButton6.Enabled =
                this.radioButton7.Enabled = this.radioButton8.Enabled = !this.radioButton12.Checked;
            this.radioButton9.Enabled = this.radioButton14.Enabled = this.radioButton10.Enabled
                = this.radioButton11.Enabled = !this.radioButton12.Checked;
            if (this.radioButton12.Checked)
            {
                this.radioButton3.Checked = true;
                this.radioButton13.Checked = true;
            }
            else
            {
                this.radioButton9.Checked = true;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.radioButton9.Checked;
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox6.Enabled = this.radioButton14.Checked;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox5.Enabled = this.radioButton10.Checked;
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox3.Enabled = this.radioButton11.Checked;
        }
    }
}
