using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class VarForm : Form
    {
        /// <summary>
        /// 变量操作窗体
        /// </summary>
        public VarForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 处理左值
            string leftOp;
            if (this.radioButton1.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox1.Text))
                {
                    MessageBox.Show("变量名不合法");
                    return;
                }
                leftOp = "&" + this.textBox1.Text;
            }
            else
            {
                if (!Halation.IsValidVarname(this.textBox2.Text))
                {
                    MessageBox.Show("变量名不合法");
                    return;
                }
                leftOp = "$" + this.textBox2.Text;
            }
            // 处理操作符
            string opr;
            if (this.radioButton3.Checked)
            {
                opr = "=";
            }
            else if (this.radioButton4.Checked)
            {
                opr = "+=";
            }
            else if (this.radioButton5.Checked)
            {
                opr = "-=";
            }
            else if (this.radioButton6.Checked)
            {
                opr = "*=";
            }
            else
            {
                opr = "/=";
            }
            // 处理右值
            string rightOp;
            if (this.radioButton9.Checked)
            {
                rightOp = String.Format("1#{0}", this.numericUpDown1.Value.ToString());
            }
            else if (this.radioButton8.Checked)
            {
                rightOp = String.Format("2#{0}", this.textBox5.Text.Replace('#', ' '));
            }
            else if (this.radioButton10.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox3.Text))
                {
                    MessageBox.Show("变量名不合法");
                    return;
                }
                rightOp = String.Format("3#{0}", this.textBox3.Text);
            }
            else if (this.radioButton11.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox4.Text))
                {
                    MessageBox.Show("变量名不合法");
                    return;
                }
                rightOp = String.Format("4#{0}", this.textBox4.Text);
            }
            else if (this.radioButton12.Checked)
            {
                rightOp = String.Format("5#{0}:{1}", this.numericUpDown2.Value.ToString(), this.numericUpDown3.Value.ToString());
            }
            else
            {
                rightOp = String.Format("6#{0}", this.textBox6.Text);
            }
            // 提交到后台
            Halation.GetInstance().DashVar(leftOp, opr, rightOp);
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.Enabled = this.radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox2.Enabled = this.radioButton2.Checked;
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.radioButton9.Checked;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox3.Enabled = this.radioButton10.Checked;
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox4.Enabled = this.radioButton11.Checked;
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown2.Enabled = this.numericUpDown3.Enabled = this.radioButton12.Checked;
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox5.Enabled = this.radioButton8.Enabled;
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox6.Enabled = this.radioButton13.Enabled;
        }

        /// <summary>
        /// 随机数下界检查
        /// </summary>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown2.Value >= this.numericUpDown3.Value)
            {
                MessageBox.Show("随机数下界不可以超过或等于上界");
                this.numericUpDown2.Value = this.numericUpDown3.Value - 1;
                return;
            }
        }

        /// <summary>
        /// 随机数上界检查
        /// </summary>
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown3.Value <= this.numericUpDown2.Value)
            {
                MessageBox.Show("随机数上界不可以低于或等于下界");
                this.numericUpDown3.Value = this.numericUpDown2.Value + 1;
                return;
            }
        }
    }
}
