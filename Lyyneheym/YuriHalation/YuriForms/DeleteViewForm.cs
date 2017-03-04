using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class DeleteViewForm : Form
    {
        public DeleteViewForm(int selectedIndex)
        {
            InitializeComponent();
            this.radioButton1.Checked = true;
            this.numericUpDown2.Enabled = false;
            this.comboBox1.Enabled = false;
            this.numericUpDown1.Maximum = Halation.project.Config.GameViewPicturesCount - 1;
            this.numericUpDown2.Maximum = Halation.project.Config.GameViewButtonCount - 1;
            switch (selectedIndex)
            {
                case 1:
                    this.radioButton2.Checked = true;
                    break;
                case 2:
                    this.radioButton4.Checked = true;
                    break;
                default:
                    this.radioButton1.Checked = true;
                    break;
            }
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

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                Halation.GetInstance().DashDeletepicture(this.numericUpDown1.Value.ToString());
            }
            else if (this.radioButton2.Checked)
            {
                Halation.GetInstance().DashDeletecstand(this.comboBox1.SelectedIndex.ToString());
            }
            else
            {
                Halation.GetInstance().DashDeletebutton(this.numericUpDown2.Value.ToString());
            }
            this.Close();
        }
    }
}
