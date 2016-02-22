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
    public partial class SwitchesForm : Form
    {
        public SwitchesForm()
        {
            InitializeComponent();
            // 选中默认开关值
            this.comboBox1.SelectedIndex = 0;
        }
    }
}
