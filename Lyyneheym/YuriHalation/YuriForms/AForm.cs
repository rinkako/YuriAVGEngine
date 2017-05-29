using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class AForm : Form
    {
        private bool isEditing;

        public AForm(bool isEdit, string name = "", string face = "", string loc = "", string vid = "")
        {
            InitializeComponent();
            this.isEditing = isEdit;
            var namelist = Halation.project.CharacterList;
            foreach (var s in namelist)
            {
                this.comboBox1.Items.Add(s);
            }
            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
            else
            {
                this.button1.Enabled = false;
            }
            if (isEdit)
            {
                for (int i = 0; i < this.comboBox1.Items.Count; i++)
                {
                    if (name == this.comboBox1.Items[i].ToString())
                    {
                        this.comboBox1.SelectedIndex = i;
                        break;
                    }
                }
                this.textBox1.Text = face;
                this.textBox2.Text = vid;
                this.numericUpDown1.Value = Convert.ToInt32(loc);
            }
        }
        

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isEditing == false)
            {
                Halation.GetInstance().DashA(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text, this.numericUpDown1.Value.ToString(), this.textBox2.Text);
            }
            else
            {
                Halation.GetInstance().DashEditA(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text, this.numericUpDown1.Value.ToString(), this.textBox2.Text);

            }
            this.Close();
        }
    }
}
