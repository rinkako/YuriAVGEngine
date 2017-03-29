using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Yuri.YuriPacker
{
    public partial class UserPackerForm : Form
    {
        /// <summary>
        /// 窗体的构造函数
        /// </summary>
        public UserPackerForm()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
        }

        #region 封包解包线程
        /// <summary>
        /// 处理封包的线程函数
        /// </summary>
        private void packThreadHandler(object packageData)
        {
            PackageData pd = (PackageData)packageData;
            PackageUtils.pack(pd.flist, pd.savefile, pd.pak, pd.sign);
            this.finishFlag = true;
        }

        /// <summary>
        /// 处理解包的线程函数
        /// </summary>
        private void unpackThreadHandler(object packageData)
        {
            PackageData pd = (PackageData)packageData;
            foreach (string f in pd.flist)
            {
                PackageUtils.unpack(pd.pakFile, f, pd.savefile + "\\" + f);
            }
            this.finishFlag = true;
        }

        /// <summary>
        /// 处理封包和解包的线程
        /// </summary>
        private Thread paker;

        /// <summary>
        /// 线程完成信号量
        /// </summary>
        private bool finishFlag = false;
        #endregion

        #region 窗体响应函数
        /// <summary>
        /// 生成封包文件按钮
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0)
            {
                MessageBox.Show("先加入文件");
                return;
            }
            FileDialog fwindow = new SaveFileDialog();
            fwindow.Filter = "dat文件|*.dat";
            if (fwindow.ShowDialog() == DialogResult.OK)
            {
                List<string> flist = new List<string>();
                foreach (string s in this.listBox1.Items)
                {
                    flist.Add(s);
                }
                PackageData swapData = new PackageData()
                {
                    flist = flist,
                    savefile = fwindow.FileName,
                    pak = this.comboBox1.SelectedItem.ToString(),
                    sign = String.Empty
                };
                this.progressBar1.Style = ProgressBarStyle.Marquee;
                this.tabControl1.Enabled = false;
                this.Text = "Lyyneheym包管理器 [正在生成封包文件]";
                paker = new Thread(new ParameterizedThreadStart(this.packThreadHandler));
                paker.IsBackground = true;
                paker.Start(swapData);
                while (!finishFlag)
                {
                    Application.DoEvents();
                }
                this.tabControl1.Enabled = true;
                this.progressBar1.Style = ProgressBarStyle.Continuous;
                this.progressBar1.Value = 0;
                this.Text = "Lyyneheym包管理器";
                finishFlag = false;
            }
        }

        /// <summary>
        /// 解包按钮
        /// </summary>
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (exPakName == String.Empty)
            {
                MessageBox.Show("你还没拉入文件呢");
                return;
            }
            string savePath = String.Empty;
            FolderBrowserDialog fdg = new FolderBrowserDialog();
            fdg.Description = "选择资源提取到哪个目录";
            if (fdg.ShowDialog() == DialogResult.OK)
            {
                savePath = fdg.SelectedPath;
            }
            List<string> flist = new List<string>();
            foreach (object ob in this.listBox2.SelectedItems)
            {
                flist.Add(ob.ToString());
            }

            PackageData swapData = new PackageData()
            {
                flist = flist,
                savefile = savePath,
                pakFile = this.exPakName,
                sign = String.Empty
            };
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.tabControl1.Enabled = false;
            this.Text = "Lyyneheym包管理器 [正在提取文件]";
            paker = new Thread(new ParameterizedThreadStart(this.unpackThreadHandler));
            paker.IsBackground = true;
            paker.Start(swapData);
            while (!finishFlag)
            {
                Application.DoEvents();
            }
            this.tabControl1.Enabled = true;
            this.progressBar1.Style = ProgressBarStyle.Continuous;
            this.progressBar1.Value = 0;
            this.Text = "Lyyneheym包管理器";
            finishFlag = false;
        }

        /// <summary>
        /// 清空按钮
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
        }

        /// <summary>
        /// 拉入文件到封包界面
        /// </summary>
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 拉入文件到封包界面
        /// </summary>
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fnameList = (e.Data.GetData(DataFormats.FileDrop, false) as String[]);
            try
            {
                this.pendingList = new List<string>();
                foreach (string fname in fnameList)
                {
                    this.pendingList.Add(fname);
                    this.listBox1.Items.Add(fname);
                }
            }
            catch
            {
                MessageBox.Show("文件格式不匹配，必须是规定的文件格式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 拉入文件到解包界面
        /// </summary>
        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 拉入文件到解包界面
        /// </summary>
        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            this.listBox2.Items.Clear();
            this.exPakName = (e.Data.GetData(DataFormats.FileDrop, false) as String[])[0];
            List<string> pakFileList = PackageUtils.getPackList(this.exPakName);
            foreach (string s in pakFileList)
            {
                this.listBox2.Items.Add(s);
            }
        }

        /// <summary>
        /// 窗口大小更变时触发：防止窗口过小
        /// </summary>
        private void UserPackerForm_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Width < 500)
            {
                this.Width = 500;
            }
            if (this.Height < 450)
            {
                this.Height = 450;
            }
        }

        /// <summary>
        /// 处理待加入文件的容器
        /// </summary>
        private List<string> pendingList = null;

        /// <summary>
        /// 要解包的文件名
        /// </summary>
        private string exPakName = String.Empty;
        #endregion
    }

    /// <summary>
    /// 发送给封包解包线程的交换信息结构体
    /// </summary>
    public struct PackageData
    {
        /// <summary>
        /// 要处理的文件列表
        /// </summary>
        public List<string> flist;
        /// <summary>
        /// 保存路径
        /// </summary>
        public string savefile;
        /// <summary>
        /// 封包的类型
        /// </summary>
        public string pak;
        /// <summary>
        /// 包签名
        /// </summary>
        public string sign;
        /// <summary>
        /// 解包文件
        /// </summary>
        public string pakFile;
    }
}
