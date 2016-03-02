using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Yuri.YuriForms
{
    public partial class MsgLayerForm : Form
    {
        public MsgLayerForm()
        {
            InitializeComponent();
            this.numericUpDown1.Maximum = Halation.project.Config.MaxMessageLayer - 1;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
