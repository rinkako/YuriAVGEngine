using System;
using System.Windows.Forms;
using Yuri.YuriLauncher.Utils;

namespace Yuri.YuriLauncher.Forms
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            this.textBox1.Text = LicenseContainer.Licenses_MSPL_Chs;
            this.textBox1.Select(0, 0);
        }

        /// <summary>
        /// 按钮：语言切换
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "English")
            {
                this.label1.Text = "Yuri Engine is licensed under the:";
                this.label2.Text = "Microsoft Public License (MS-PL)";
                this.textBox1.Text = LicenseContainer.Licenses_MSPL_Eng;
                this.button1.Text = "中文";
            }
            else
            {
                this.label1.Text = "优理引擎采用：";
                this.label2 .Text = "Microsoft公共许可证（MS-PL）";
                this.textBox1.Text = LicenseContainer.Licenses_MSPL_Chs;
                this.button1.Text = "English";
            }
            this.textBox1.Select(0, 0);
        }

            }
}
