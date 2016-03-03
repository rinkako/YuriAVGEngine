using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Yuri.YuriForms
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 控制器实例
        /// </summary>
        private Halation core = Halation.GetInstance();

        /// <summary>
        /// 构造器
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Halation.SetViewReference(this);
        }

        /// <summary>
        /// 按钮：显示对话
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogForm formDialog = new DialogForm();
            formDialog.ShowDialog(this);
        }

        /// <summary>
        /// 菜单按钮：撤销
        /// </summary>
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items.Find("撤销ToolStripMenuItem", true)[0].Enabled = core.MenuUndo();
            this.menuStrip1.Items.Find("重做ToolStripMenuItem", true)[0].Enabled = core.IsAbleRedo();
        }

        /// <summary>
        /// 菜单按钮：重做
        /// </summary>
        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items.Find("撤销ToolStripMenuItem", true)[0].Enabled = core.IsAbleUndo();
            this.menuStrip1.Items.Find("重做ToolStripMenuItem", true)[0].Enabled = core.MenuRedo();
        }

        /// <summary>
        /// 菜单按钮：新建
        /// </summary>
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectForm npf = new NewProjectForm();
            npf.ShowDialog(this);
            if (npf.InitSuccess)
            {
                core.RefreshProjectTree();
            }
        }

        /// <summary>
        /// 代码编辑框选择项改变事件
        /// </summary>
        private void codeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.actionGroupBox.Enabled = this.codeListBox.SelectedIndices.Count == 1;
        }

        /// <summary>
        /// 工程树选择事件
        /// </summary>
        private void projTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.projTreeView.SelectedNode == Halation.projectTreeRoot)
            {
                this.projTreeView.SelectedNode = Halation.projectTreeMain;
            }
            Halation.projectTreeChosen = this.projTreeView.SelectedNode;
            Halation.currentScriptName = this.projTreeView.SelectedNode.Text;
            this.codeGroupBox.Text = String.Format("脚本 [{0}]", Halation.currentScriptName);
            this.button_AddNewFunc.Enabled = this.projTreeView.SelectedNode.Level == 1;
            this.core.ChangeCodePackage(this.projTreeView.SelectedNode.Text,
                this.projTreeView.SelectedNode.Level == 1 ? "" : this.projTreeView.SelectedNode.Parent.Text);
            this.core.RefreshCodeContext();
            this.core.RefreshRedoUndo();
        }

        /// <summary>
        /// 代码树重绘事件
        /// </summary>
        private void codeListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush FontBrush = null;
            ListBox listBox = sender as ListBox;
            if (e.Index > -1)
            {
                string itemFull = listBox.Items[e.Index].ToString();
                switch ((itemFull.Split('\t')[0]).Substring(1))
                {
                    case "角色状态": FontBrush = Brushes.Brown; break;
                    case "Major": FontBrush = Brushes.Red; break;
                    case "标签": FontBrush = Brushes.Orange; break;
                    default: FontBrush = Brushes.Black; break;
                }
                e.DrawBackground();
                e.Graphics.DrawString(itemFull, e.Font, FontBrush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// 按钮：新建场景
        /// </summary>
        private void button36_Click(object sender, EventArgs e)
        {
            AddSceneForm arf = new AddSceneForm();
            arf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：新建函数
        /// </summary>
        private void button35_Click(object sender, EventArgs e)
        {
            AddFuncForm aff = new AddFuncForm();
            aff.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：状态变更
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            AFrom af = new AFrom();
            af.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：全局设定
        /// </summary>
        private void 全局设定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalConfigForm gcf = new GlobalConfigForm();
            gcf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：变更文字层
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            MsgLayerForm mlf = new MsgLayerForm();
            mlf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：直接描绘文本
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            DrawStringForm dsf = new DrawStringForm();
            dsf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：选择项
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            BranchForm bf = new BranchForm();
            bf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：停止BGM
        /// </summary>
        private void button18_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashStopBGM();
        }

        /// <summary>
        /// 按钮：停止BGS
        /// </summary>
        private void button17_Click(object sender, EventArgs e)
        {
            Halation.GetInstance().DashStopBGS();
        }

        /// <summary>
        /// 按钮：播放BGM
        /// </summary>
        private void button14_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("插入音乐", 0);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放BGS
        /// </summary>
        private void button13_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("插入音乐", 1);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放SE
        /// </summary>
        private void button16_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("插入音乐", 2);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放Vocal
        /// </summary>
        private void button15_Click(object sender, EventArgs e)
        {
            VocalForm vf = new VocalForm();
            vf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：BGM
        /// </summary>
        private void bGMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 0);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：BGS
        /// </summary>
        private void bGSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("99", 1);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：SE
        /// </summary>
        private void sEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 2);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：Vocal
        /// </summary>
        private void vOCALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 3);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：标签
        /// </summary>
        private void button20_Click(object sender, EventArgs e)
        {
            LabelForm lf = new LabelForm();
            lf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：跳转
        /// </summary>
        private void button19_Click(object sender, EventArgs e)
        {
            JumpForm jf = new JumpForm();
            jf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：开关操作
        /// </summary>
        private void button22_Click(object sender, EventArgs e)
        {
            SwitchesForm sf = new SwitchesForm();
            sf.ShowDialog(this);
        }









    }
}
