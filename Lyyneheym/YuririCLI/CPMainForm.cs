using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Yuri.YuririCLI
{
    public partial class CPMainForm : Form
    {
        public CPMainForm()
        {
            InitializeComponent();
            this.button2.Enabled = this.button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };
            fd.ShowDialog();
            if (fd.SelectedPath == String.Empty) { return; }
            this.workPath = fd.SelectedPath;
            this.button2.Enabled = true;
            this.button3.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileDialog fd = new SaveFileDialog()
            {
                Filter = @"IL文件|*.sil"
            };
            fd.ShowDialog();
            if (fd.FileName == String.Empty) { return; }
            this.outPath = fd.FileName;
            this.button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(this.textBox2.Text, "^[a-zA-Z]{8}$"))
            {
                MessageBox.Show(@"工程密钥必须是半角英文字母，8位长度");
                return;
            }
            if (this.textBox1.Text.Trim() == String.Empty)
            {
                MessageBox.Show(@"工程名不能为空");
                return;
            }
            CLIAdapter.BeginCompile(this.textBox1.Text.Trim(), this.workPath, this.outPath, this.textBox2.Text, this.checkBox1.Checked);
            MessageBox.Show(@"Successfully Compiled.");
        }

        private string workPath;
        private string outPath;
    }
}
