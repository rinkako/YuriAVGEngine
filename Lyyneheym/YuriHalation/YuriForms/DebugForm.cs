using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class DebugForm : Form
    {
        /// <summary>
        /// 重定向标准输出的委托
        /// </summary>
        public delegate void DelReadStdOutput(string result);

        /// <summary>
        /// 重定向标准输出的事件
        /// </summary>
        public event DelReadStdOutput ReadStdOutput;

        /// <summary>
        /// 跨线程修改按钮
        /// </summary>
        private delegate void SetButtonCallback();

        /// <summary>
        /// 程序根目录
        /// </summary>
        private string rootPath;

        /// <summary>
        /// 调试进程
        /// </summary>
        private Process debugGameProcess;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="path">启动的路径根目录</param>
        public DebugForm(string path)
        {
            InitializeComponent();
            this.rootPath = path;
            ReadStdOutput += new DelReadStdOutput(ReadStdOutputAction);  
        }

        /// <summary>
        /// 按钮：启动程序
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            
            this.richTextBox1.Text = String.Empty;
            try
            {
                this.button1.Enabled = false;
                this.ControlBox = false;
                debugGameProcess = new Process();
                debugGameProcess.StartInfo.FileName = rootPath + @"\Yuri.exe";
                debugGameProcess.StartInfo.Arguments = String.Empty;
                debugGameProcess.StartInfo.UseShellExecute = false;
                debugGameProcess.StartInfo.RedirectStandardOutput = true;
                debugGameProcess.StartInfo.RedirectStandardError = true;
                debugGameProcess.EnableRaisingEvents = true;
                debugGameProcess.Exited += ps_Exited;
                debugGameProcess.OutputDataReceived += debugGameProcess_OutputDataReceived;
                debugGameProcess.ErrorDataReceived += debugGameProcess_ErrorDataReceived;
                debugGameProcess.Start();
                debugGameProcess.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button1.Enabled = true;
                this.ControlBox = true;
            }
        }

        /// <summary>
        /// 当调试进程的错误到来时
        /// </summary>
        private void debugGameProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Invoke(ReadStdOutput, new object[] { "[CLR Error]" + Environment.NewLine + "信  息：" + e.Data + Environment.NewLine });
            }
        }

        /// <summary>
        /// 当调试进程的输出到来时
        /// </summary>
        private void debugGameProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Invoke(ReadStdOutput, new object[] { e.Data });
            }
        }

        /// <summary>
        /// 回显函数
        /// </summary>
        private void ReadStdOutputAction(string result)
        {
            var beginStamp = this.richTextBox1.Text.Length;
            var selectCount = result.Length;
            if (result.StartsWith("[") && result.EndsWith("]"))
            {
                this.richTextBox1.AppendText(result + Environment.NewLine);
                this.richTextBox1.Select(beginStamp, selectCount);
                switch (result)
                {
                    case "[Important]":
                        this.richTextBox1.SelectionColor = Color.Blue;
                        break;
                    case "[Warning]":
                        this.richTextBox1.SelectionColor = Color.DarkOrange;
                        break;
                    case "[Error]":
                        this.richTextBox1.SelectionColor = Color.Red;
                        break;
                    case "[CLR Error]":
                        this.richTextBox1.SelectionColor = Color.DarkRed;
                        break;
                    default:
                        this.richTextBox1.SelectionColor = Color.Black;
                        break;
                }
            }
            else
            {
                this.richTextBox1.AppendText(result + Environment.NewLine);
                this.richTextBox1.Select(beginStamp, selectCount);
                this.richTextBox1.SelectionColor = Color.Black;
            }
            this.richTextBox1.ScrollToCaret();
            this.richTextBox1.SelectionLength = 0;  
        }

        /// <summary>
        /// 调试进程退出时
        /// </summary>
        private void ps_Exited(object sender, EventArgs e)
        {
            if (this.button1.InvokeRequired)
            {
                SetButtonCallback d = new SetButtonCallback(SetControlsEnable);
                this.Invoke(d);
            }
            else
            {
                this.button1.Enabled = true;
                this.ControlBox = true;
            }
        }

        /// <summary>
        /// 跨线程启动按钮
        /// </summary>
        private void SetControlsEnable()
        {
            this.button1.Enabled = true;
            this.ControlBox = true;
        }

        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        private void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (debugGameProcess!= null && !debugGameProcess.HasExited)
            {
                debugGameProcess.Kill();
            }
        }
    }
}
