using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class VocalForm : Form
    {
        public VocalForm()
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
            }
            else
            {
                this.button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
