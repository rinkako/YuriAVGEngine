using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Yuri.YuriHalation.HalationCore;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class MusicForm : Form
    {
        private readonly bool isEditing;

        public MusicForm(string title, int index, bool isEdit)
        {
            InitializeComponent();
            this.Text = title;
            this.tabControl1.SelectedIndex = index;
            if (title == "音乐管理")
            {
                this.isMusicMana = true;
            }
            else
            {
                this.isMusicMana = false;
                this.tabControl1.TabPages.RemoveAt(3);
                this.button1.Text = "选中";
                this.button2.Text = "取消";
            }
            this.isEditing = isEdit;
        }

        private void MusicForm_Load(object sender, EventArgs e)
        {
            // 加载文件夹
            DirectoryInfo dirInfoBGM = new DirectoryInfo(this.SoundDir + @"\bgm");
            DirectoryInfo dirInfoBGS = new DirectoryInfo(this.SoundDir + @"\bgs");
            DirectoryInfo dirInfoSE = new DirectoryInfo(this.SoundDir + @"\se");
            DirectoryInfo dirInfoVocal = new DirectoryInfo(this.SoundDir + @"\vocal");
            // 加载文件
            foreach (var f in dirInfoBGM.GetFiles())
            {
                this.listBoxBGM.Items.Add(f.Name);
                this.BGMVec.Add(f.FullName);
            }
            foreach (var f in dirInfoBGS.GetFiles())
            {
                this.listBoxBGS.Items.Add(f.Name);
                this.BGSVec.Add(f.FullName);
            }
            foreach (var f in dirInfoSE.GetFiles())
            {
                this.listBoxSE.Items.Add(f.Name);
                this.SEVec.Add(f.FullName);
            }
            foreach (var f in dirInfoVocal.GetFiles())
            {
                this.listBoxVocal.Items.Add(f.Name);
                this.VocalVec.Add(f.FullName);
            }
        }

        /// <summary>
        /// 按钮：播放
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.isMusicMana)
            {
                switch (this.tabControl1.SelectedTab.Text)
                {
                    case "BGM":
                        if (this.listBoxBGM.SelectedIndex != -1)
                        {
                            Musician.GetInstance().PlayBGM(this.listBoxBGM.SelectedItem.ToString(), this.BGMVec[this.listBoxBGM.SelectedIndex], this.volTrackBar.Value);
                        }
                        break;
                    case "BGS":
                        if (this.listBoxBGS.SelectedIndex != -1)
                        {
                            Musician.GetInstance().PlayBGS(this.BGSVec[this.listBoxBGS.SelectedIndex], this.volTrackBar.Value);
                        }
                        break;
                    case "SE":
                        if (this.listBoxSE.SelectedIndex != -1)
                        {
                            Musician.GetInstance().PlaySE(this.SEVec[this.listBoxSE.SelectedIndex], this.volTrackBar.Value);
                        }
                        break;
                    case "Vocal":
                        if (this.listBoxVocal.SelectedIndex != -1)
                        {
                            Musician.GetInstance().PlayVocal(this.VocalVec[this.listBoxVocal.SelectedIndex], this.volTrackBar.Value);
                        }
                        break;
                }
            }
            else
            {
                switch (this.tabControl1.SelectedTab.Text)
                {
                    case "BGM":
                        if (this.listBoxBGM.SelectedIndex != -1)
                        {
                            if (this.isEditing)
                            {
                                Halation.GetInstance().DashEditPlayBGM(
                                       this.listBoxBGM.SelectedItem.ToString(), this.volTrackBar.Value.ToString());
                            }
                            else
                            {
                                Halation.GetInstance().DashPlayBGM(
                                    this.listBoxBGM.SelectedItem.ToString(), this.volTrackBar.Value.ToString());
                            }
                        }
                        break;
                    case "BGS":
                        if (this.listBoxBGS.SelectedIndex != -1)
                        {
                            Halation.GetInstance().DashPlayBGS(this.listBoxBGS.SelectedItem.ToString(), this.volTrackBar.Value.ToString());
                        }
                        break;
                    case "SE":
                        if (this.listBoxSE.SelectedIndex != -1)
                        {
                            Halation.GetInstance().DashPlaySE(this.listBoxSE.SelectedItem.ToString(), this.volTrackBar.Value.ToString());
                        }
                        break;
                }
                this.Close();
            }
        }

        /// <summary>
        /// 按钮：停止
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.isMusicMana)
            {
                switch (this.tabControl1.SelectedTab.Text)
                {
                    case "BGM":
                        Musician.GetInstance().StopAndReleaseBGM();
                        break;
                    case "BGS":
                        Musician.GetInstance().StopBGS();
                        break;
                    case "Vocal":
                        Musician.GetInstance().StopAndReleaseVocal();
                        break;
                }
            }
            else
            {
                this.Close();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabControl1.SelectedTab.Text)
            {
                case "BGM":
                    if (this.listBoxBGM.Items.Count > 0)
                    {
                        this.listBoxBGM.SelectedIndex = 0;
                    }
                    else
                    {
                        this.listBoxBGM.SelectedIndex = -1;
                    }
                    break;
                case "BGS":
                    if (this.listBoxBGS.Items.Count > 0)
                    {
                        this.listBoxBGS.SelectedIndex = 0;
                    }
                    else
                    {
                        this.listBoxBGS.SelectedIndex = -1;
                    }
                    break;
                case "SE":
                    if (this.listBoxSE.Items.Count > 0)
                    {
                        this.listBoxSE.SelectedIndex = 0;
                    }
                    else
                    {
                        this.listBoxSE.SelectedIndex = -1;
                    }
                    break;
                case "Vocal":
                    if (this.listBoxVocal.Items.Count > 0)
                    {
                        this.listBoxVocal.SelectedIndex = 0;
                    }
                    else
                    {
                        this.listBoxVocal.SelectedIndex = -1;
                    }
                    break;
            }
        }

        /// <summary>
        /// 是否音乐管理模式
        /// </summary>
        private bool isMusicMana = false;

        /// <summary>
        /// 音频文件夹绝对路径
        /// </summary>
        private string SoundDir = Halation.projectFolder + @"\Sound";

        /// <summary>
        /// BGM绝对路径向量
        /// </summary>
        private List<string> BGMVec = new List<string>();

        /// <summary>
        /// BGS绝对路径向量
        /// </summary>
        private List<string> BGSVec = new List<string>();

        /// <summary>
        /// SE绝对路径向量
        /// </summary>
        private List<string> SEVec = new List<string>();

        /// <summary>
        /// Vocal绝对路径向量
        /// </summary>
        private List<string> VocalVec = new List<string>();
    }
}
