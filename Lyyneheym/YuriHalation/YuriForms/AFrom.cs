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
    public partial class AFrom : Form
    {

        public AFrom()
        {
            InitializeComponent();
        }

        private void AFrom_Load(object sender, EventArgs e)
        {
            var namelist = Halation.project.CharacterList;
            this.comboBox1.Items.Add("不变");
            foreach (var s in namelist)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashA(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text, this.comboBox2.SelectedItem.ToString(), this.textBox2.Text);
            this.Close();
        }
    }
}
