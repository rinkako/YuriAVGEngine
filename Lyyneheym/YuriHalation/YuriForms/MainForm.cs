using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Yuri;

namespace YuriHalation.YuriForms
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
            this.资源ToolStripMenuItem.Enabled = this.编辑ToolStripMenuItem.Enabled =
                this.工程ToolStripMenuItem.Enabled = this.编译ToolStripMenuItem.Enabled =
                this.保存ToolStripMenuItem.Enabled = false;
            Halation.SetViewReference(this);
        }

        #region 前端响应
        /// <summary>
        /// 窗体关闭时
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dr = MessageBox.Show("确定要退出吗" + Environment.NewLine + "未保存的工作将会丢失", "退出Halation",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 代码编辑框选择项改变事件
        /// </summary>
        private void codeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 跳过空的情况
            if (this.codeListBox.SelectedItem == null || this.codeListBox.SelectedIndex == -1)
            {
                return;
            }
            // 可插入性
            this.actionGroupBox.Enabled = this.codeListBox.Enabled && this.codeListBox.SelectedIndex != -1;
            // 可选择性
            string itemStr = this.codeListBox.SelectedItem.ToString();
            if (itemStr.TrimStart().StartsWith(":"))
            {
                this.codeListBox.SelectedIndices.Clear();
                this.codeListBox.SelectedIndex = -1;
                this.codeListBox.Refresh();
                this.actionGroupBox.Enabled = false;
                return;
            }
            // 连续选择嵌套语句块
            if (itemStr.Trim().Substring(1) == "循环")
            {
                var act = Halation.currentCodePackage.GetAction(this.codeListBox.SelectedIndex);
                var allAct = Halation.currentCodePackage.GetAction();
                for (int i = this.codeListBox.SelectedIndex + 1; i < allAct.Count; i++)
                {
                    if (act.indent == allAct[i].indent && allAct[i].nodeType == Yuri.YuriHalation.ScriptPackage.ActionPackageType.act_endfor)
                    {
                        for (int j = this.codeListBox.SelectedIndex; j <= i; j++)
                        {
                            this.codeListBox.SelectedIndices.Add(j);
                        }
                        break;
                    }
                }
            }
            else if (itemStr.Trim().Substring(1).StartsWith("条件分支"))
            {
                var act = Halation.currentCodePackage.GetAction(this.codeListBox.SelectedIndex);
                var allAct = Halation.currentCodePackage.GetAction();
                for (int i = this.codeListBox.SelectedIndex + 1; i < allAct.Count; i++)
                {
                    if (act.indent == allAct[i].indent && allAct[i].nodeType == Yuri.YuriHalation.ScriptPackage.ActionPackageType.act_endif)
                    {
                        for (int j = this.codeListBox.SelectedIndex; j <= i; j++)
                        {
                            this.codeListBox.SelectedIndices.Add(j);
                        }
                        break;
                    }
                }
            }
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
            this.codeListBox.HorizontalExtent = this.codeListBox.Width - 16;
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
                string trimItem = itemFull.Trim().Split(' ')[0];
                if (trimItem.Length == 0)
                {
                    FontBrush = Brushes.Black;
                }
                else
                {
                    switch (trimItem.Substring(1))
                    {
                        case "角色状态":
                            FontBrush = Brushes.Brown;
                            break;
                        case "开关操作":
                        case "变量操作":
                            FontBrush = Brushes.Red;
                            break;
                        case "标签":
                        case "标签跳转":
                            FontBrush = Brushes.Orange;
                            break;
                        case "注释":
                            FontBrush = Brushes.Green;
                            break;
                        case "循环":
                        case "以上反复":
                        case "中断循环":
                        case "条件分支":
                        case "除此以外的情况":
                        case "分支结束":
                            FontBrush = Brushes.Blue;
                            break;
                        case "代码片段":
                            FontBrush = Brushes.Gray;
                            break;
                        case "函数调用":
                            FontBrush = Brushes.Purple;
                            break;
                        default:
                            FontBrush = Brushes.Black;
                            break;
                    }
                }
                e.DrawBackground();
                e.Graphics.DrawString(itemFull, e.Font, FontBrush, e.Bounds.Location);
                e.DrawFocusRectangle();
                this.codeListBox.HorizontalExtent = Math.Max(this.codeListBox.HorizontalExtent,
                    (int)e.Graphics.MeasureString(itemFull, e.Font).Width);
            }
        }
        #endregion

        #region 其他命令按钮
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
        #endregion

        #region 添加命令项
        /// <summary>
        /// 按钮：显示对话
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogForm formDialog = new DialogForm("显示对话");
            formDialog.ShowDialog(this);
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
            this.core.DashStopBGM();
        }

        /// <summary>
        /// 按钮：停止BGS
        /// </summary>
        private void button17_Click(object sender, EventArgs e)
        {
            this.core.DashStopBGS();
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
            SwitchesForm sf = new SwitchesForm("开关操作");
            sf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：注释
        /// </summary>
        private void button23_Click(object sender, EventArgs e)
        {
            NotationForm nf = new NotationForm();
            nf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：中断循环
        /// </summary>
        private void button27_Click(object sender, EventArgs e)
        {
            this.core.DashBreak();
        }

        /// <summary>
        /// 按钮：退出当前场景
        /// </summary>
        private void button34_Click(object sender, EventArgs e)
        {
            this.core.DashReturn();
        }

        /// <summary>
        /// 按钮：等待用户操作
        /// </summary>
        private void button30_Click(object sender, EventArgs e)
        {
            this.core.DashWaituser();
        }

        /// <summary>
        /// 按钮：延时等待
        /// </summary>
        private void button25_Click(object sender, EventArgs e)
        {
            WaitForm wf = new WaitForm();
            wf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：循环
        /// </summary>
        private void button28_Click(object sender, EventArgs e)
        {
            this.core.DashFor();
        }

        /// <summary>
        /// 按钮：代码片段
        /// </summary>
        private void button33_Click(object sender, EventArgs e)
        {
            DialogForm df = new DialogForm("代码片段");
            df.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：移除图片
        /// </summary>
        private void button32_Click(object sender, EventArgs e)
        {
            DeleteViewForm dvf = new DeleteViewForm(0);
            dvf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：移除按钮
        /// </summary>
        private void button31_Click(object sender, EventArgs e)
        {
            DeleteViewForm dvf = new DeleteViewForm(2);
            dvf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：等待动画结束
        /// </summary>
        private void button11_Click(object sender, EventArgs e)
        {
            this.core.DashWaitani();
        }

        /// <summary>
        /// 按钮：过渡
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            TransForm tf = new TransForm();
            tf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：修改文字层属性
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            MsgLayerOptForm mlof = new MsgLayerOptForm();
            mlof.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：动画
        /// </summary>
        private void button12_Click(object sender, EventArgs e)
        {
            MoveForm mf = new MoveForm();
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：显示背景
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            BgForm bf = new BgForm();
            bf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：显示立绘
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            CStandForm csf = new CStandForm();
            csf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：放置按钮
        /// </summary>
        private void button24_Click(object sender, EventArgs e)
        {
            ButtonForm bf = new ButtonForm();
            bf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：显示图片
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            PicturesForm pf = new PicturesForm();
            pf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：函数调用
        /// </summary>
        private void button29_Click(object sender, EventArgs e)
        {
            FunctionCallForm fcf = new FunctionCallForm();
            fcf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：变量操作
        /// </summary>
        private void button21_Click(object sender, EventArgs e)
        {
            VarForm vf = new VarForm();
            vf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：条件分歧
        /// </summary>
        private void button26_Click(object sender, EventArgs e)
        {
            IfForm iff = new IfForm();
            iff.ShowDialog(this);
        }
        #endregion

        #region 菜单项
        /// <summary>
        /// 代码框右键菜单
        /// </summary>
        private void codeListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.codeListBox.SelectedIndices.Clear();
                int posindex = this.codeListBox.IndexFromPoint(new Point(e.X, e.Y));
                this.codeListBox.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < this.codeListBox.Items.Count)
                {
                    if ((this.codeListBox.Items[posindex].ToString()).TrimStart().StartsWith(":"))
                    {
                        this.codeListBox.Refresh();
                        return;
                    }
                    this.codeListBox.SelectedIndex = posindex;
                    this.CodeListContextMenuStrip.Show(this.codeListBox, new Point(e.X, e.Y));
                }
            }
            this.codeListBox.Refresh();
        }

        /// <summary>
        /// 菜单：复制
        /// </summary>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.CopyCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
        }

        /// <summary>
        /// 菜单：删除
        /// </summary>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.DeleteCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
            this.core.RefreshCodeContext();
        }

        /// <summary>
        /// 菜单：剪切
        /// </summary>
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.CutCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
            this.core.RefreshCodeContext();
        }

        /// <summary>
        /// 菜单：粘贴
        /// </summary>
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.PasteCode(this.codeListBox.SelectedIndex);
            this.core.RefreshCodeContext();
        }

        /// <summary>
        /// 菜单：编辑
        /// </summary>
        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedItem != null &&
                this.codeListBox.SelectedItem.ToString().TrimStart().StartsWith(":"))
            {
                this.复制ToolStripMenuItem.Enabled = false;
                this.粘贴ToolStripMenuItem.Enabled = false;
                this.剪切ToolStripMenuItem.Enabled = false;
                this.删除ToolStripMenuItem.Enabled = false;
            }
            if (this.codeListBox.Focused == false ||
                this.codeListBox.SelectedIndex == -1)
            {
                this.复制ToolStripMenuItem.Enabled = false;
                this.粘贴ToolStripMenuItem.Enabled = false;
                this.剪切ToolStripMenuItem.Enabled = false;
                this.删除ToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.复制ToolStripMenuItem.Enabled = true;
                this.粘贴ToolStripMenuItem.Enabled = Halation.CopyItems != null && Halation.CopyItems.Count > 0;
                this.剪切ToolStripMenuItem.Enabled = true;
                this.删除ToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// 菜单：撤销
        /// </summary>
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items.Find("撤销ToolStripMenuItem", true)[0].Enabled = core.MenuUndo();
            this.menuStrip1.Items.Find("重做ToolStripMenuItem", true)[0].Enabled = core.IsAbleRedo();
            this.core.RefreshCodeContext();
        }

        /// <summary>
        /// 菜单：重做
        /// </summary>
        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.menuStrip1.Items.Find("撤销ToolStripMenuItem", true)[0].Enabled = core.IsAbleUndo();
            this.menuStrip1.Items.Find("重做ToolStripMenuItem", true)[0].Enabled = core.MenuRedo();
            this.core.RefreshCodeContext();
        }

        /// <summary>
        /// 菜单：新建
        /// </summary>
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectForm npf = new NewProjectForm();
            npf.ShowDialog(this);
            if (npf.InitSuccess)
            {
                core.RefreshProjectTree();
                this.资源ToolStripMenuItem.Enabled = this.编辑ToolStripMenuItem.Enabled =
                    this.工程ToolStripMenuItem.Enabled = this.编译ToolStripMenuItem.Enabled =
                    this.保存ToolStripMenuItem.Enabled = true;
                this.Text = String.Format("Yuri Halation - {0}", Halation.projectName);
            }
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
            MusicForm mf = new MusicForm("音乐管理", 1);
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
        /// 菜单：退出
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("确定要退出吗" + Environment.NewLine + "未保存的工作将会丢失", "退出Halation",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 菜单：保存
        /// </summary>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.SaveProject();
        }

        /// <summary>
        /// 菜单：打开
        /// </summary>
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.Filter = "Halation工程|*.yrproj";
            fd.ShowDialog(this);
            if (fd.FileName != "")
            {
                this.core.LoadProject(fd.FileName);
                this.资源ToolStripMenuItem.Enabled = this.编辑ToolStripMenuItem.Enabled =
                    this.工程ToolStripMenuItem.Enabled = this.编译ToolStripMenuItem.Enabled =
                    this.保存ToolStripMenuItem.Enabled = true;
                this.Text = String.Format("Yuri Halation - {0}", Halation.projectName);
            }
        }

        /// <summary>
        /// 菜单：背景
        /// </summary>
        private void 背景ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("图像资源管理器", 2);
            prf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：立绘
        /// </summary>
        private void 立绘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("图像资源管理器", 1);
            prf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：图片
        /// </summary>
        private void 图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PicResourceForm prf = new PicResourceForm("图像资源管理器", 0);
            prf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：开关管理器
        /// </summary>
        private void 开关管理器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchesForm sf = new SwitchesForm("开关管理器");
            sf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：打开游戏目录
        /// </summary>
        private void 打开游戏目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e," + Halation.projectFolder;
            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 菜单：翻译
        /// </summary>
        private void 生成工程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.DashParse();
        }

        /// <summary>
        /// 菜单：生成并运行
        /// </summary>
        private void 生成并运行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.DashParse();
            DebugForm df = new DebugForm(Halation.projectFolder);
            df.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：关于
        /// </summary>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog(this);
        }
        #endregion


    }
}
