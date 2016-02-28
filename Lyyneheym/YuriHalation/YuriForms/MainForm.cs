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
            DialogForm formDialog = new DialogForm(this.codeListBox.SelectedIndex);
            formDialog.ShowDialog(this);
        }

        /// <summary>
        /// 菜单按钮：撤销
        /// </summary>
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items["撤销ToolStripMenuItem"].Enabled = core.MenuUndo();
            this.menuStrip1.Items["重做ToolStripMenuItem"].Enabled = core.IsAbleRedo();
        }

        /// <summary>
        /// 菜单按钮：重做
        /// </summary>
        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items["撤销ToolStripMenuItem"].Enabled = core.IsAbleUndo();
            this.menuStrip1.Items["重做ToolStripMenuItem"].Enabled = core.MenuRedo();
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
            this.codeListBox.Text = String.Format("脚本 [{0}]", this.projTreeView.SelectedNode.Text);
        }



    }
}
