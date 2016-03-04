using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class MoveForm : Form
    {
        public MoveForm()
        {
            InitializeComponent();
            foreach (var s in this.propertyItemDesc)
            {
                this.comboBox2.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    this.numericUpDown1.Maximum = Halation.project.Config.PictureLayerCount - 1;
                    break;
                case 1:
                    this.numericUpDown1.Maximum = 4;
                    break;
                case 2:
                    this.numericUpDown1.Maximum = 1;
                    break;
                case 3:
                    this.numericUpDown1.Maximum = Halation.project.Config.ButtonLayerCount;
                    break;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("目标值不能为空");
                return;
            }
            Halation.GetInstance().DashMove(this.comboBox1.SelectedItem.ToString(),
                this.numericUpDown1.Value.ToString(), this.numericUpDown3.Value.ToString(),
                this.propertyItem[this.comboBox2.SelectedIndex], this.textBox1.Text, this.numericUpDown2.Value.ToString());
            this.Close();
        }

        public string[] propertyItem = new string[]
        {
            "x",
            "y",
            "opacity",
            "angle",
            "scale",
            "scalex",
            "scaley"
        };

        public string[] propertyItemDesc = new string[]
        {
            "X坐标",
            "Y坐标",
            "不透明度",
            "角度",
            "缩放比",
            "X缩放比",
            "Y缩放比"
        };
    }
}
