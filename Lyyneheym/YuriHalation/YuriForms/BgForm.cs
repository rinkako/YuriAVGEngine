using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class BgForm : Form
    {
        private readonly bool isEditing;

        public BgForm(bool isEdit, string id = "1", string filename = "", string ro = "-8")
        {
            InitializeComponent();
            this.isEditing = isEdit;
            if (isEdit)
            {
                if (id == "1")
                {
                    this.radioButton2.Checked = true;
                }
                else
                {
                    this.radioButton1.Checked = true;
                }
                this.textBox1.Text = filename;
                this.textBox2.Text = ro;
            }
        }

        /// <summary>
        /// 按钮：选择图像
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("选择背景", 2);
            prf.ShowDialog(this);
            this.textBox1.Text = this.GotFileName;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == String.Empty)
            {
                MessageBox.Show(@"请选择图像");
                return;
            }
            if (this.isEditing)
            {
                Halation.GetInstance().DashEditBg(this.radioButton1.Checked ? "0" : "1", this.textBox1.Text, this.textBox2.Text);
            }
            else
            {
                Halation.GetInstance() .DashBg(this.radioButton1.Checked ? "0" : "1", this.textBox1.Text, this.textBox2.Text);
            }
            this.Close();
        }

        /// <summary>
        /// 获取或设置背景资源名
        /// </summary>
        public string GotFileName { get; set; }
    }
}
