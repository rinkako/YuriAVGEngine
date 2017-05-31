using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Yuri.YuririCLI
{
    /// <summary>
    /// GUI窗体
    /// </summary>
    public partial class CPMainForm : Form
    {
        /// <summary>
        /// 构造一个编译器GUI窗体
        /// </summary>
        public CPMainForm()
        {
            InitializeComponent();
            this.button2.Enabled = this.button3.Enabled = this.button4.Enabled = false;
        }

        /// <summary>
        /// 按钮：选输入路径
        /// </summary>
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
            this.button3.Enabled = this.button4.Enabled = false;
        }

        /// <summary>
        /// 按钮：选输出文件
        /// </summary>
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
            this.button4.Enabled = false;
        }

        /// <summary>
        /// 按钮：编译输出
        /// </summary>
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
            var beginStamp = DateTime.Now;
            CLIAdapter.BeginCompile(this.textBox1.Text.Trim(), this.workPath, this.outPath, this.textBox2.Text, this.checkBox1.Checked);
            Console.WriteLine();
            Console.WriteLine(@"Time Cost: {0} ms", (DateTime.Now - beginStamp).TotalMilliseconds);
            MessageBox.Show(@"Successfully Compiled.");
            this.button4.Enabled = true;
        }

        /// <summary>
        /// 按钮：解析树
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                FileDialog fd = new SaveFileDialog()
                {
                    Filter = @"TEXT|*.txt"
                };
                fd.ShowDialog();
                if (fd.FileName != String.Empty)
                {
                    FileStream fs = new FileStream(fd.FileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(CLIAdapter.Pile.GetTreeGraphic());
                    sw.Close();
                    fs.Close();
                    MessageBox.Show(@"OK!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Failed." + ex);
            }
        }

        /// <summary>
        /// 输入路径
        /// </summary>
        private string workPath;

        /// <summary>
        /// 输出路径
        /// </summary>
        private string outPath;
    }
}
