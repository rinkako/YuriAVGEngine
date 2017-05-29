using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class MoveForm : Form
    {
        private bool isEditing;

        public MoveForm(bool isEdit, string name = "picture", string id = "0", string time = "0", string target = "opacity", string dash = "1", string acc = "0")
        {
            InitializeComponent();
            foreach (var s in this.propertyItemDesc)
            {
                this.comboBox2.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.isEditing = isEdit;
            if (isEdit)
            {
                for (int i = 0; i < this.comboBox1.Items.Count; i++)
                {
                    if (this.comboBox1.Items[i].ToString() == name)
                    {
                        this.comboBox1.SelectedIndex = i;
                        break;
                    }
                }
                this.numericUpDown1.Value = Convert.ToInt32(id);
                for (int i = 0; i < this.comboBox2.Items.Count; i++)
                {
                    if (this.propertyItem[i] == target)
                    {
                        this.comboBox2.SelectedIndex = i;
                        break;
                    }
                }
                this.textBox1.Text = dash;
                this.numericUpDown2.Value = Convert.ToInt32(acc);
                this.numericUpDown3.Value = Convert.ToInt32(time);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    this.numericUpDown1.Maximum = Halation.project.Config.GameViewPicturesCount - 1;
                    break;
                case 1:
                    this.numericUpDown1.Maximum = Halation.project.Config.Game3DStage ? 32 : 4;
                    break;
                case 2:
                    this.numericUpDown1.Maximum = 1;
                    break;
                case 3:
                    this.numericUpDown1.Maximum = Halation.project.Config.GameViewButtonCount;
                    break;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == String.Empty)
            {
                MessageBox.Show(@"目标值不能为空");
                return;
            }
            if (this.propertyItem[this.comboBox2.SelectedIndex] == "opacity"
                && Double.TryParse(this.textBox1.Text, out double topa)
                && (topa < 0 || topa > 1))
            {
                MessageBox.Show(@"不透明度的范围是[0, 1]之间的浮点数");
                return;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditMove(this.comboBox1.SelectedItem.ToString(),
                    this.numericUpDown1.Value.ToString(), this.numericUpDown3.Value.ToString(),
                    this.propertyItem[this.comboBox2.SelectedIndex], this.textBox1.Text,
                    this.numericUpDown2.Value.ToString());
            }
            else
            {
                Halation.GetInstance().DashMove(this.comboBox1.SelectedItem.ToString(),
                    this.numericUpDown1.Value.ToString(), this.numericUpDown3.Value.ToString(),
                    this.propertyItem[this.comboBox2.SelectedIndex], this.textBox1.Text,
                    this.numericUpDown2.Value.ToString());
            }
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
