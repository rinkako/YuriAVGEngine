using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;
using Yuri.YuriHalation.ScriptPackage;

namespace YuriHalation.YuriForms
{
    /// <summary>
    /// 窗体：全局设定
    /// </summary>
    public partial class GlobalConfigForm : Form
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public GlobalConfigForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 为窗体载入数据
        /// </summary>
        private void GlobalConfigForm_Load(object sender, EventArgs e)
        {
            ConfigPackage config = Halation.project.Config;
            // 工程页
            this.textBox1.Text = Halation.projectFolder;
            this.textBox2.Text = config.GameProjName;
            this.textBox3.Text = config.GameProjVersion;
            this.textBox4.Text = config.GameProjKey;
            // 可视页
            this.numericUpDown1.Value = config.GameViewWindowWidth;
            this.numericUpDown2.Value = config.GameViewWindowWidth;
            this.checkBox1.Checked = config.GameViewWindowResizeable;
            this.numericUpDown3.Value = config.GameViewCStandLeftX;
            this.numericUpDown4.Value = config.GameViewCStandLeftY;
            this.numericUpDown5.Value = config.GameViewCStandMidleftY;
            this.numericUpDown6.Value = config.GameViewCStandMidleftX;
            this.numericUpDown7.Value = config.GameViewCStandMidY;
            this.numericUpDown8.Value = config.GameViewCStandMidX;
            this.numericUpDown9.Value = config.GameViewCStandMidrightY;
            this.numericUpDown10.Value = config.GameViewCStandMidrightX;
            this.numericUpDown11.Value = config.GameViewCStandRightY;
            this.numericUpDown12.Value = config.GameViewCStandRightX;
            this.numericUpDown13.Value = config.GameViewPicturesCount;
            this.numericUpDown14.Value = config.GameViewPicturesZ;
            this.numericUpDown15.Value = config.GameViewButtonZ;
            this.numericUpDown16.Value = config.GameViewButtonCount;
            this.numericUpDown17.Value = config.GameViewBackgroundZ;
            this.numericUpDown18.Value = config.GameViewCStandZ;
            // 文本层
            this.numericUpDown19.Value = config.GameMsgLayerCount;
            this.numericUpDown20.Value = config.GameMsgLayerY;
            this.numericUpDown21.Value = config.GameMsgLayerX;
            this.numericUpDown22.Value = config.GameMsgLayerH;
            this.numericUpDown23.Value = config.GameMsgLayerW;
            this.numericUpDown24.Value = config.GameMsgLayerU;
            this.numericUpDown25.Value = config.GameMsgLayerL;
            this.numericUpDown26.Value = config.GameMsgLayerR;
            this.numericUpDown27.Value = config.GameMsgLayerB;
            this.textBox5.Text = config.GameMsgLayerTriaName;
            this.numericUpDown28.Value = config.GameMsgLayerTriaY;
            this.numericUpDown29.Value = config.GameMsgLayerTriaX;
            this.numericUpDown30.Value = config.GameMsgLayerFontSize;
            this.numericUpDown31.Value = config.GameMsgLayerFontLineheight;
            this.textBox7.Text = config.GameMsgLayerFontName;
            this.numericUpDown32.Value = Convert.ToInt32(config.GameMsgLayerFontColor.Split(',')[0]);
            this.numericUpDown33.Value = Convert.ToInt32(config.GameMsgLayerFontColor.Split(',')[1]);
            this.numericUpDown34.Value = Convert.ToInt32(config.GameMsgLayerFontColor.Split(',')[2]);
            this.checkBox2.Checked = config.GameMsgLayerFontShadow;
            this.numericUpDown35.Value = config.GameMsgLayerTypeSpeed;
            this.textBox6.Text = config.GameMsgLayerBackgroundName;
            this.numericUpDown36.Value = config.GameMsgLayerZ;
            // 选项页
            this.textBox9.Text = config.GameBranchBackgroundNormal;
            this.textBox10.Text = config.GameBranchBackgroundOver;
            this.numericUpDown42.Value = config.GameBranchH;
            this.numericUpDown43.Value = config.GameBranchW;
            this.numericUpDown44.Value = config.GameBranchFontSize;
            this.textBox11.Text = config.GameBranchFontName;
            this.numericUpDown45.Value = Convert.ToInt32(config.GameBranchFontColor.Split(',')[2]);
            this.numericUpDown46.Value = Convert.ToInt32(config.GameBranchFontColor.Split(',')[1]);
            this.numericUpDown47.Value = Convert.ToInt32(config.GameBranchFontColor.Split(',')[0]);
            this.numericUpDown48.Value = config.GameBranchPadTop;
            this.numericUpDown49.Value = config.GameBranchCount;
            this.numericUpDown50.Value = config.GameBranchZ;
            // 音频页
            this.numericUpDown37.Value = config.GameMusicBGMVol;
            this.numericUpDown38.Value = config.GameMusicBGSVol;
            this.numericUpDown39.Value = config.GameMusicSEVol;
            this.numericUpDown40.Value = config.GameMusicVocalVol;
            this.numericUpDown41.Value = config.GameMusicBgsCount;
            this.comboBox1.SelectedIndex = config.GameMusicVocalPostfix == ".mp3" ? 0 : 1;
            // 杂项页
            this.numericUpDown51.Value = config.GameMaxSwitchCount;
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            ConfigPackage config = Halation.project.Config;
            // 工程页
            config.GameProjName = this.textBox2.Text;
            config.GameProjVersion = this.textBox3.Text;
            config.GameProjKey = this.textBox4.Text;
            // 可视页
            config.GameViewWindowWidth = (int)this.numericUpDown1.Value;
            config.GameViewWindowWidth = (int)this.numericUpDown2.Value;
            config.GameViewWindowResizeable = this.checkBox1.Checked;
            config.GameViewCStandLeftX = (int)this.numericUpDown3.Value;
            config.GameViewCStandLeftY = (int)this.numericUpDown4.Value;
            config.GameViewCStandMidleftY = (int)this.numericUpDown5.Value;
            config.GameViewCStandMidleftX = (int)this.numericUpDown6.Value;
            config.GameViewCStandMidY = (int)this.numericUpDown7.Value;
            config.GameViewCStandMidX = (int)this.numericUpDown8.Value;
            config.GameViewCStandMidrightY = (int)this.numericUpDown9.Value;
            config.GameViewCStandMidrightX = (int)this.numericUpDown10.Value;
            config.GameViewCStandRightY = (int)this.numericUpDown11.Value;
            config.GameViewCStandRightX = (int)this.numericUpDown12.Value;
            config.GameViewPicturesCount = (int)this.numericUpDown13.Value;
            config.GameViewPicturesZ = (int)this.numericUpDown14.Value;
            config.GameViewButtonZ = (int)this.numericUpDown15.Value;
            config.GameViewButtonCount = (int)this.numericUpDown16.Value;
            config.GameViewBackgroundZ = (int)this.numericUpDown17.Value;
            config.GameViewCStandZ = (int)this.numericUpDown18.Value;
            // 文本层
            config.GameMsgLayerCount = (int)this.numericUpDown19.Value;
            config.GameMsgLayerY = (int)this.numericUpDown20.Value;
            config.GameMsgLayerX = (int)this.numericUpDown21.Value;
            config.GameMsgLayerH = (int)this.numericUpDown22.Value;
            config.GameMsgLayerW = (int)this.numericUpDown23.Value;
            config.GameMsgLayerU = (int)this.numericUpDown24.Value;
            config.GameMsgLayerL = (int)this.numericUpDown25.Value;
            config.GameMsgLayerR = (int)this.numericUpDown26.Value;
            config.GameMsgLayerB = (int)this.numericUpDown27.Value;
            config.GameMsgLayerTriaName = this.textBox5.Text;
            config.GameMsgLayerTriaY = (int)this.numericUpDown28.Value;
            config.GameMsgLayerTriaX = (int)this.numericUpDown29.Value;
            config.GameMsgLayerFontSize = (int)this.numericUpDown30.Value;
            config.GameMsgLayerFontLineheight = (int)this.numericUpDown31.Value;
            config.GameMsgLayerFontName = this.textBox7.Text;
            config.GameMsgLayerFontColor = String.Format("{0},{1},{2}", this.numericUpDown32.Value, this.numericUpDown33.Value, this.numericUpDown34.Value);
            config.GameMsgLayerFontShadow = this.checkBox2.Checked;
            config.GameMsgLayerTypeSpeed = (int)this.numericUpDown35.Value;
            config.GameMsgLayerBackgroundName = this.textBox6.Text;
            config.GameMsgLayerZ = (int)this.numericUpDown36.Value;
            // 选项页
            config.GameBranchBackgroundNormal = this.textBox9.Text;
            config.GameBranchBackgroundOver = this.textBox10.Text;
            config.GameBranchH = (int)this.numericUpDown42.Value;
            config.GameBranchW = (int)this.numericUpDown43.Value;
            config.GameBranchFontSize = (int)this.numericUpDown44.Value;
            config.GameBranchFontName = this.textBox11.Text;
            config.GameBranchFontColor = String.Format("{0},{1},{2}", this.numericUpDown47.Value, this.numericUpDown46.Value, this.numericUpDown45.Value);
            config.GameBranchPadTop = (int)this.numericUpDown48.Value;
            config.GameBranchCount = (int)this.numericUpDown49.Value;
            config.GameBranchZ = (int)this.numericUpDown50.Value;
            // 音频页
            config.GameMusicBGMVol = (int)this.numericUpDown37.Value;
            config.GameMusicBGSVol = (int)this.numericUpDown38.Value;
            config.GameMusicSEVol = (int)this.numericUpDown39.Value;
            config.GameMusicVocalVol = (int)this.numericUpDown40.Value;
            config.GameMusicBgsCount = (int)this.numericUpDown41.Value;
            config.GameMusicVocalPostfix = this.comboBox1.SelectedItem.ToString();
            // 杂项页
            config.GameMaxSwitchCount = (int)this.numericUpDown51.Value;
            // 关闭窗体
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var str = Halation.GetNameAndValue<ConfigPackage>(Halation.project.Config);
        }
    }
}
