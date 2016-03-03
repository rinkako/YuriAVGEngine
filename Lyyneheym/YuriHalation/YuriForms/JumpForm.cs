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
    public partial class JumpForm : Form
    {
        public JumpForm()
        {
            InitializeComponent();
            var vList = Halation.project.GetScene();
            foreach (var v in vList)
            {
                this.comboBox1.Items.Add(v.sceneName);
            }
            this.comboBox1.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox2.Enabled = this.checkBox1.Checked;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashJump(this.textBox1.Text, this.comboBox1.SelectedItem.ToString(), this.textBox2.Text);
            this.Close();
        }
    }
}
