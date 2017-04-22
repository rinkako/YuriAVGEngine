using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Yuri.YuriHalation.HalationCore;

namespace Yuri.YuriHalation.YuriForms
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
            PackageUtils.Pack(pd.flist, pd.savefile, pd.pak, pd.sign);
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
            string packName = String.Empty;
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    packName = PackageUtils.PackURI_PA_BACKGROUND + PackageUtils.PackPostfix;
                    break;
                case 1:
                    packName = PackageUtils.PackURI_PA_CHARASTAND + PackageUtils.PackPostfix;
                    break;
                case 2:
                    packName = PackageUtils.PackURI_PA_PICTURES + PackageUtils.PackPostfix;
                    break;
                case 3:
                    packName = PackageUtils.PackURI_SO_BGM + PackageUtils.PackPostfix;
                    break;
                case 4:
                    packName = PackageUtils.PackURI_SO_BGS + PackageUtils.PackPostfix;
                    break;
                case 5:
                    packName = PackageUtils.PackURI_SO_SE + PackageUtils.PackPostfix;
                    break;
                case 6:
                    packName = PackageUtils.PackURI_SO_VOCAL + PackageUtils.PackPostfix;
                    break;
            }
            PackageData swapData = new PackageData()
            {
                flist = this.pendingList,
                savefile = Halation.projectFolder + "\\" + packName,
                pak = this.comboBox1.SelectedItem.ToString(),
                sign = String.Format("{0}?{1}", Halation.project.Config.GameProjKey, Halation.project.Config.GameProjVersion)
            };
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.listBox1.Enabled = this.button1.Enabled = false;
            this.Text = "包管理器 [正在生成封包文件]";
            paker = new Thread(new ParameterizedThreadStart(this.packThreadHandler)) { IsBackground = true };
            paker.Start(swapData);
            while (!finishFlag)
            {
                Application.DoEvents();
            }
            MessageBox.Show("打包完毕");
            this.listBox1.Enabled = this.button1.Enabled = true;
            this.progressBar1.Style = ProgressBarStyle.Continuous;
            this.progressBar1.Value = 0;
            this.Text = "包管理器";
            finishFlag = false;
        }

        /// <summary>
        /// 拉入文件到封包界面
        /// </summary>
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        /// <summary>
        /// 变更文件类型事件
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DirectoryInfo dinfo = null;
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    dinfo = new DirectoryInfo(this.PicDir + @"\background");
                    break;
                case 1:
                    dinfo = new DirectoryInfo(this.PicDir + @"\character");
                    break;
                case 2:
                    dinfo = new DirectoryInfo(this.PicDir + @"\pictures");
                    break;
                case 3:
                    dinfo = new DirectoryInfo(this.SndDir + @"\bgm");
                    break;
                case 4:
                    dinfo = new DirectoryInfo(this.SndDir + @"\bgs");
                    break;
                case 5:
                    dinfo = new DirectoryInfo(this.SndDir + @"\se");
                    break;
                case 6:
                    dinfo = new DirectoryInfo(this.SndDir + @"\vocal");
                    break;
            }
            if (dinfo != null)
            {
                this.pendingList = new List<string>();
                this.listBox1.Items.Clear();
                foreach (var f in dinfo.GetFiles())
                {
                    this.listBox1.Items.Add(f.Name);
                    this.pendingList.Add(f.FullName);
                }
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
        /// 图片文件夹绝对路径
        /// </summary>
        private readonly string PicDir = Halation.projectFolder + @"\PictureAssets";

        /// <summary>
        /// 音频文件夹绝对路径
        /// </summary>
        private readonly string SndDir = Halation.projectFolder + @"\Sound";

        /// <summary>
        /// 处理待加入文件的容器
        /// </summary>
        private List<string> pendingList = null;
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
