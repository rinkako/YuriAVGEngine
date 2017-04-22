using System;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class AddSceneForm : Form
    {
        /// <summary>
        /// 私有的构造器
        /// </summary>
        public AddSceneForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            bool flag = this.core.DashAddScene(this.textBox1.Text);
            // 刷新前台
            if (!flag)
            {
                MessageBox.Show("建立失败，请检查是否有重名/名称不合法的场景", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.core.RefreshProjectTree(this.textBox1.Text);
            this.Close();
        }

        /// <summary>
        /// 控制器
        /// </summary>
        private Halation core = Halation.GetInstance();

    }
}
