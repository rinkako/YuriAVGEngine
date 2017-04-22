using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class DeleteViewForm : Form
    {
        private readonly bool isEditing;

        public DeleteViewForm(int selectedIndex, bool isEdit, string id = "")
        {
            InitializeComponent();
            this.radioButton1.Checked = true;
            this.numericUpDown2.Enabled = false;
            this.numericUpDown3.Enabled = false;
            this.numericUpDown1.Maximum = Halation.project.Config.GameViewPicturesCount - 1;
            this.numericUpDown2.Maximum = Halation.project.Config.GameViewButtonCount - 1;
            switch (selectedIndex)
            {
                case 1:
                    this.radioButton2.Checked = true;
                    if (isEdit)
                    {
                        this.numericUpDown3.Value = Convert.ToInt32(id);
                    }
                    break;
                case 2:
                    this.radioButton4.Checked = true;
                    if (isEdit)
                    {
                        this.numericUpDown2.Value = Convert.ToInt32(id);
                    }
                    break;
                default:
                    this.radioButton1.Checked = true;
                    if (isEdit)
                    {
                        this.numericUpDown1.Value = Convert.ToInt32(id);
                    }
                    break;
            }
            this.isEditing = isEdit;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown2.Enabled = this.radioButton4.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown3.Enabled = this.radioButton2.Checked;
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
                if (this.isEditing)
                {
                    Halation.GetInstance().DashEditDeletepicture(this.numericUpDown1.Value.ToString());
                }
                else
                {
                    Halation.GetInstance().DashDeletepicture(this.numericUpDown1.Value.ToString());
                }
            }
            else if (this.radioButton2.Checked)
            {
                if (this.isEditing)
                {
                    Halation.GetInstance().DashEditDeletecstand(this.numericUpDown1.Value.ToString());
                }
                else
                {
                    Halation.GetInstance().DashDeletecstand(this.numericUpDown3.Value.ToString());
                }
            }
            else
            {
                if (this.isEditing)
                {
                    Halation.GetInstance().DashEditDeletebutton(this.numericUpDown2.Value.ToString());
                }
                else
                {
                    Halation.GetInstance().DashDeletebutton(this.numericUpDown2.Value.ToString());
                }
            }
            this.Close();
        }
    }
}
