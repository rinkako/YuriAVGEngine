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
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.Enabled = this.checkBox1.Checked;
        }
    }
}
