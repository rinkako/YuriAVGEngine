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

        private void codeListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush FontBrush = null;
            ListBox listBox = sender as ListBox;
            if (e.Index > -1)
            {
                string itemFull = listBox.Items[e.Index].ToString();
                switch ((itemFull.Split(' ')[0]).Substring(1))
                {
                    case "角色状态": FontBrush = Brushes.Brown; break;
                    case "Major": FontBrush = Brushes.Red; break;
                    case "Minor": FontBrush = Brushes.Orange; break;
                    case "Warning": FontBrush = Brushes.Yellow; break;
                    default: FontBrush = Brushes.Black; break;
                }
                e.DrawBackground();
                e.Graphics.DrawString(itemFull, e.Font, FontBrush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// 按钮：变更文字层
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            MsgLayerForm mlf = new MsgLayerForm();
            mlf.ShowDialog(this);
        }





    }
}
