using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class JumpForm : Form
    {
        private readonly bool isEditing;

        public JumpForm(bool isEdit, string filename = "", string target = "", string cond = "")
        {
            InitializeComponent();
            var vList = Halation.project.GetScene();
            this.comboBox1.Items.Add(String.Empty);
            foreach (var v in vList)
            {
                this.comboBox1.Items.Add(v.sceneName);
            }
            this.comboBox1.SelectedIndex = 0;
            this.isEditing = isEdit;
            if (isEdit)
            {
                this.comboBox1.Text = filename;
                this.textBox1.Text = target;
                this.textBox2.Text = cond;
                if (cond != String.Empty)
                {
                    this.checkBox1.Checked = true;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox2.Enabled = this.checkBox1.Checked;
            if (this.checkBox1.Checked == false)
            {
                this.textBox2.Text = String.Empty;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditJump(this.textBox1.Text, this.comboBox1.SelectedItem.ToString(), this.textBox2.Text);
            }
            else
            {
                Halation.GetInstance().DashJump(this.textBox1.Text, comboBox1.SelectedItem?.ToString() ?? String.Empty, this.textBox2.Text);
            }
            this.Close();
        }
    }
}
