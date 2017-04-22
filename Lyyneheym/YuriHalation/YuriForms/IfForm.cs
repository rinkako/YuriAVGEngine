using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class IfForm : Form
    {
        private readonly bool isEditing;

        /// <summary>
        /// 窗体：条件分支
        /// </summary>
        public IfForm(bool isEdit, bool containElse = false, string expr = "", string op1 = "", string opr = "", string op2 = "")
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
            this.isEditing = isEdit;
            if (isEdit)
            {
                this.checkBox1.Enabled = false;
                if (expr != String.Empty)
                {
                    this.checkBox2.Checked = true;
                    this.textBox2.Text = expr;
                }
                else
                {
                    switch (opr)
                    {
                        case "==":
                            this.radioButton3.Checked = true;
                            break;
                        case "<>":
                            this.radioButton4.Checked = true;
                            break;
                        case ">":
                            this.radioButton5.Checked = true;
                            break;
                        case "<":
                            this.radioButton6.Checked = true;
                            break;
                        case ">=":
                            this.radioButton7.Checked = true;
                            break;
                        default:
                            this.radioButton8.Checked = true;
                            break;
                    }
                    var ifLeftItems = op1.Split('#');
                    var ifRightItems = op2.Split('#');
                    switch (ifLeftItems[0])
                    {
                        case "0":
                            this.radioButton15.Checked = true;
                            this.textBox7.Text = ifLeftItems[1];
                            break;
                        case "1":
                            this.radioButton1.Checked = true;
                            this.textBox4.Text = ifLeftItems[1];
                            break;
                        case "2":
                            this.radioButton2.Checked = true;
                            this.textBox1.Text = ifLeftItems[1];
                            break;
                        case "3":
                            this.radioButton12.Checked = true;
                            this.comboBox3.SelectedIndex = Convert.ToInt32(ifLeftItems[1]);
                            break;
                    }
                    switch (ifRightItems[0])
                    {
                        case "0":
                            this.radioButton16.Checked = true;
                            this.textBox8.Text = ifRightItems[1];
                            break;
                        case "1":
                            this.radioButton9.Checked = true;
                            this.numericUpDown1.Value = Convert.ToInt32(ifRightItems[1]);
                            break;
                        case "2":
                            this.radioButton14.Checked = true;
                            this.textBox6.Text = ifRightItems[1];
                            break;
                        case "3":
                            this.radioButton10.Checked = true;
                            this.textBox5.Text = ifRightItems[1];
                            break;
                        case "4":
                            this.radioButton11.Checked = true;
                            this.textBox3.Text = ifRightItems[1];
                            break;
                        case "5":
                            this.radioButton13.Checked = true;
                            this.comboBox4.SelectedIndex = ifRightItems[1] == "off" ? 0 : 1;
                            break;
                    }
                    if (containElse)
                    {
                        this.checkBox1.Checked = true;
                    }
                }
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
                if (this.textBox2.Text == String.Empty)
                {
                    MessageBox.Show("请填写表达式");
                    return;
                }
                expr = this.textBox2.Text;
                op1 = op2 = opr = String.Empty;
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
                else if (this.radioButton15.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox7.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op1 = "0#" + this.textBox7.Text;
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
                else if (this.radioButton16.Checked)
                {
                    if (!Halation.IsValidVarname(this.textBox8.Text))
                    {
                        MessageBox.Show("操作数A变量名不合法");
                        return;
                    }
                    op2 = "0#" + this.textBox8.Text;
                }
                else
                {
                    op2 = "5#" + this.comboBox4.SelectedItem.ToString();
                }
                expr = String.Empty;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditIf(this.checkBox1.Checked, expr, op1, opr, op2);
            }
            else
            {
                Halation.GetInstance().DashIf(this.checkBox1.Checked, expr, op1, opr, op2);
            }
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

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox7.Enabled = this.radioButton15.Checked;
        }

        private void radioButton16_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox8.Enabled = this.radioButton16.Checked;
        }
    }
}
