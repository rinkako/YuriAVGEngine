using System;
using System.Windows.Forms;
using Yuri.YuriLauncher.Utils;

namespace Yuri.YuriLauncher.Forms
{
    public partial class RightsForm : Form
    {
        public RightsForm()
        {
            InitializeComponent();
            this.listBox1.SelectedIndex = 0;
            this.textBox1.Text = LicenseContainer.SIL_Eng;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.listBox1.SelectedIndex)
            {
                // IRONPYTHON
                case 0:
                    this.label5.Text = "Apache License 2.0";
                    this.textBox2.Text = LicenseContainer.Licenses_Apache_2_Eng;
                    break;
                // MahMetro
                case 1:
                    this.label5.Text = "MIT License";
                    this.textBox2.Text = LicenseContainer.MIT_Eng;
                    break;
                // NAudio
                case 2:
                    this.label5.Text = "Microsoft Public License(Ms-PL)";
                    this.textBox2.Text = LicenseContainer.Licenses_MSPL_Eng;
                    break;
                // Transitionals
                case 3:
                    this.label5.Text = "Microsoft Public License(Ms-PL)";
                    this.textBox2.Text = LicenseContainer.Licenses_MSPL_Eng;
                    break;
            }
            if (this.listBox1.SelectedIndex != -1)
            {
                this.label4.Text = String.Format("{0}软件许可证：", this.listBox1.SelectedItem);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://weibo.com/lostlililili");
        }
    }
}
