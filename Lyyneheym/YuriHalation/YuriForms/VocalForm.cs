using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class VocalForm : Form
    {
        private bool isEditing;

        public VocalForm(bool isEdit, string name = "", string vid = "")
        {
            InitializeComponent();
            var namelist = Halation.project.CharacterList;
            foreach (var s in namelist)
            {
                this.comboBox1.Items.Add(s);
            }
            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
                for (int i = 0; i < this.comboBox1.Items.Count; i++)
                {
                    if (this.comboBox1.Items[i].ToString() == name)
                    {
                        this.comboBox1.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                this.button1.Enabled = false;
            }
            this.textBox1.Text = vid;
            this.isEditing = isEdit;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == String.Empty)
            {
                MessageBox.Show(@"vid不可以为空");
                return;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditVocal(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text.Trim());
            }
            else
            {
                Halation.GetInstance().DashVocal(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text.Trim());
            }
            this.Close();
        }
    }
}
