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
    public partial class DeleteViewForm : Form
    {
        public DeleteViewForm()
        {
            InitializeComponent();
            this.radioButton1.Checked = true;
            this.numericUpDown2.Enabled = false;
            this.comboBox1.Enabled = false;
            this.comboBox1.SelectedIndex = 0;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown2.Enabled = this.radioButton4.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBox1.Enabled = this.radioButton2.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.radioButton1.Checked;
        }
    }
}
