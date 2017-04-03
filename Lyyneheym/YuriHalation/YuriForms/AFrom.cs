using System;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
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
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashA(this.comboBox1.SelectedItem.ToString(), this.textBox1.Text, this.numericUpDown1.Value.ToString(), this.textBox2.Text);
            this.Close();
        }
    }
}
