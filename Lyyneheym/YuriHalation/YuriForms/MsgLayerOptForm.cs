using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class MsgLayerOptForm : Form
    {
        public MsgLayerOptForm()
        {
            InitializeComponent();
            foreach (var s in this.msgLayOptionDescription)
            {
                this.comboBox1.Items.Add(s);
            }
            this.numericUpDown1.Maximum = Halation.project.Config.GameMsgLayerCount - 1;
            this.comboBox1.SelectedIndex = 0;
        }

        public string[] msgLayOptionDescription = new string[]
        { 
            "字号", "字体", "颜色",
            "可见性", "行距", "不透明度",
            "X坐标", "Y坐标", "Z坐标",
            "高度", "宽度", "侧边距",
            "文本对齐", "层背景图名称",
            "重置", "重置风格"
        };

        public string[] msgLayOptions = new string[]
        { 
            "fontsize", "fontname", "fontcolor",
            "visible", "lineheight", "opacity",
            "x", "y", "z",
            "height", "width", "padding",
            "texthorizontal", "backgroundname",
            "reset", "stylereset"
        };

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 14 && this.textBox1.Text == String.Empty)
            {
                MessageBox.Show("请正确填写目标值");
                return;
            }
            Halation.GetInstance().DashMsgLayerOpt(this.numericUpDown1.Value.ToString(), this.msgLayOptions[this.comboBox1.SelectedIndex], this.textBox1.Text);
            this.Close();
        }
    }
}
