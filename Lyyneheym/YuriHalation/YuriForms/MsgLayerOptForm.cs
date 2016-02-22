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
    public partial class MsgLayerOptForm : Form
    {
        public MsgLayerOptForm()
        {
            InitializeComponent();
            this.radioButton1.Checked = true;
            this.radioButton2.Checked = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox1.Enabled = this.radioButton1.Checked;
        }

    }
}
