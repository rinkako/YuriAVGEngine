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
    public partial class TransForm : Form
    {
        /// <summary>
        /// 过渡窗体
        /// </summary>
        public TransForm()
        {
            InitializeComponent();
            foreach (var s in this.transTypeName)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashTrans(this.comboBox1.SelectedItem.ToString());
            this.Close();
        }

        /// <summary>
        /// 过渡类型名
        /// </summary>
        private string[] transTypeName = new string[]
        {
            "Fade",
            "Star",
            "Rotate",
            "VerticalWipe",
            "Page",
            "Roll",
            "Diamonds",
            "VerticalBlinds",
            "HorizontalWipe",
            "FadeAndBlur",
            "Explosion",
            "Checkerboard",
            "Translate",
            "RotateWipe",
            "Melt",
            "DiagonalWipe",
            "Flip",
            "Dots",
            "FadeAndGrow",
            "DoubleRotateWipe",
            "Door",
            "HorizontalBlinds"
        };
    }
}
