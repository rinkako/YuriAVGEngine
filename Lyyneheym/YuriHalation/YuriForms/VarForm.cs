using System;
using System.Globalization;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class VarForm : Form
    {
        private readonly bool isEditing;

        /// <summary>
        /// 变量操作窗体
        /// </summary>
        public VarForm(bool isEdit, string opLeft = "", string op = "", string opRight = "")
        {
            InitializeComponent();
            this.isEditing = isEdit;
            if (isEdit)
            {
                switch (opLeft[0])
                {
                    case '&':
                        this.radioButton1.Checked = true;
                        this.textBox1.Text = opLeft.Substring(1);
                        break;
                    case '%':
                        this.radioButton14.Checked = true;
                        this.textBox7.Text = opLeft.Substring(1);
                        break;
                    default:
                        this.radioButton2.Checked = true;
                        this.textBox2.Text = opLeft.Substring(1);
                        break;
                }
                var opRightItem = opRight.Split('#');
                switch (opRightItem[0])
                {
                    case "0":
                        this.radioButton15.Checked = true;
                        this.textBox8.Text = opRightItem[1];
                        break;
                    case "1":
                        this.radioButton9.Checked = true;
                        this.numericUpDown1.Value = Convert.ToInt32(opRightItem[1]);
                        break;
                    case "2":
                        this.radioButton8.Checked = true;
                        this.textBox5.Text = opRightItem[1];
                        break;
                    case "3":
                        this.radioButton10.Checked = true;
                        this.textBox3.Text = opRightItem[1];
                        break;
                    case "4":
                        this.radioButton10.Checked = true;
                        this.textBox4.Text = opRightItem[1];
                        break;
                    case "5":
                        this.radioButton12.Checked = true;
                        var randomRandItem = opRightItem[1].Split(':');
                        this.numericUpDown2.Value = Convert.ToInt32(randomRandItem[0]);
                        this.numericUpDown3.Value = Convert.ToInt32(randomRandItem[1]);
                        break;
                    default:
                        this.radioButton13.Checked = true;
                        this.textBox6.Text = opRightItem[1];
                        break;
                }
                switch (op)
                {
                    case "=":
                        this.radioButton3.Checked = true;
                        break;
                    case "+=":
                        this.radioButton4.Checked = true;
                        break;
                    case "-=":
                        this.radioButton5.Checked = true;
                        break;
                    case "*=":
                        this.radioButton6.Checked = true;
                        break;
                    default:
                        this.radioButton7.Checked = true;
                        break;
                }
            }
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
                    MessageBox.Show(@"变量名不合法");
                    return;
                }
                leftOp = "&" + this.textBox1.Text;
            }
            else if (this.radioButton14.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox7.Text))
                {
                    MessageBox.Show(@"变量名不合法");
                    return;
                }
                leftOp = "%" + this.textBox7.Text;
            }
            else
            {
                if (!Halation.IsValidVarname(this.textBox2.Text))
                {
                    MessageBox.Show(@"变量名不合法");
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
                rightOp = String.Format("1#{0}", this.numericUpDown1.Value);
            }
            else if (this.radioButton8.Checked)
            {
                rightOp = String.Format("2#{0}", this.textBox5.Text.Replace('#', ' '));
            }
            else if (this.radioButton10.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox3.Text))
                {
                    MessageBox.Show(@"变量名不合法");
                    return;
                }
                rightOp = String.Format("3#{0}", this.textBox3.Text);
            }
            else if (this.radioButton11.Checked)
            {
                if (!Halation.IsValidVarname(this.textBox4.Text))
                {
                    MessageBox.Show(@"变量名不合法");
                    return;
                }
                rightOp = String.Format("4#{0}", this.textBox4.Text);
            }
            else if (this.radioButton12.Checked)
            {
                rightOp = String.Format("5#{0}:{1}", this.numericUpDown2.Value, this.numericUpDown3.Value);
            }
            else if (this.radioButton15.Checked)
            {
                rightOp = String.Format("0#{0}", this.textBox8.Text);
            }
            else
            {
                rightOp = String.Format("6#{0}", this.textBox6.Text);
            }
            // 除0检查
            if (opr == "/=" && rightOp == "1#0")
            {
                MessageBox.Show(@"0不可以作为除数");
                return;
            }
            // 提交到后台
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditVar(leftOp, opr, rightOp);
            }
            else
            {
                Halation.GetInstance().DashVar(leftOp, opr, rightOp);
            }
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
            this.textBox5.Enabled = this.radioButton8.Checked;
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox6.Enabled = this.radioButton13.Checked;
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox7.Enabled = this.radioButton14.Checked;
        }

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox8.Enabled = this.radioButton15.Checked;
        }

        /// <summary>
        /// 随机数下界检查
        /// </summary>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown2.Value >= this.numericUpDown3.Value)
            {
                MessageBox.Show(@"随机数下界不可以超过或等于上界");
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
                MessageBox.Show(@"随机数上界不可以低于或等于下界");
                this.numericUpDown3.Value = this.numericUpDown2.Value + 1;
                return;
            }
        }


    }
}
