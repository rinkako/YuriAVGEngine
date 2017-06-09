using System;
using System.Drawing;
using System.Windows.Forms;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 控制器实例
        /// </summary>
        private readonly Halation core = Halation.GetInstance();

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
            this.core.SaveProject();
            //var dr = MessageBox.Show(@"确定要退出吗" + Environment.NewLine + @"未保存的工作将会丢失", @"退出Halation",
            //    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            //if (dr == DialogResult.Cancel)
            //{
            //    e.Cancel = true;
            //}
        }

        /// <summary>
        /// 窗体变更大小后
        /// </summary>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Width < 800) { this.Width = 800; }
            if (this.Height < 750) { this.Height = 750; }
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
                    if (act.indent == allAct[i].indent && allAct[i].nodeType == ActionPackageType.act_endfor)
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
                    if (act.indent == allAct[i].indent && allAct[i].nodeType == ActionPackageType.act_endif)
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
            this.button_AddNewFunc.Enabled = this.button_deleteScene.Enabled = this.projTreeView.SelectedNode.Level == 1;
            this.button_deleteFunc.Enabled = this.projTreeView.SelectedNode.Level == 2;
            this.core.ChangeCodePackage(this.projTreeView.SelectedNode.Text,
                this.projTreeView.SelectedNode.Level == 1 ? String.Empty : this.projTreeView.SelectedNode.Parent.Text);
            this.codeListBox.HorizontalExtent = this.codeListBox.Width - 16;
            this.core.RefreshCodeContext();
            this.core.RefreshRedoUndo();
        }

        /// <summary>
        /// 代码树重绘事件
        /// </summary>
        private void codeListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.Index > -1)
            {
                string itemFull = listBox.Items[e.Index].ToString();
                string trimItem = itemFull.Trim().Split(' ')[0];
                Brush FontBrush;
                if (trimItem.Length == 0)
                {
                    FontBrush = Brushes.Black;
                }
                else
                {
                    switch (trimItem.Substring(1))
                    {
                        case "角色状态":
                            FontBrush = Brushes.LightSlateGray;
                            break;
                        case "播放音乐":
                        case "播放声效":
                        case "播放BGS":
                        case "播放语音":
                            FontBrush = Brushes.MediumTurquoise;
                            break;
                        case "显示图片":
                        case "显示背景":
                        case "显示立绘":
                            FontBrush = Brushes.Violet;
                            break;
                        case "执行过渡":
                        case "动画":
                            FontBrush = Brushes.Plum;
                            break;
                        case "场景镜头":
                            FontBrush = Brushes.DeepSkyBlue;
                            break;
                        case "开关操作":
                        case "变量操作":
                            FontBrush = Brushes.Brown;
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
                        case "退出当前场景":
                        case "结束游戏":
                        case "返回标题":
                            FontBrush = Brushes.Purple;
                            break;
                        case "等待动画结束":
                        case "等待用户操作":
                        case "呼叫存档画面":
                        case "呼叫读档画面":
                        case "延时等待":
                            FontBrush = Brushes.OrangeRed;
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

        /// <summary>
        /// 按钮：删除场景
        /// </summary>
        private void button37_Click(object sender, EventArgs e)
        {
            if (this.projTreeView.SelectedNode.Text == @"main")
            {
                MessageBox.Show(@"main场景不可以被删除");
                return;
            }
            var dr =MessageBox.Show(@"真的要删除场景吗" + Environment.NewLine + @"这是一个不可撤销的动作",
                @"确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                this.core.DashDeleteScene(this.projTreeView.SelectedNode.Text);
            }
        }

        /// <summary>
        /// 按钮：删除函数
        /// </summary>
        private void button35_Click_1(object sender, EventArgs e)
        {
            if (this.projTreeView.SelectedNode.Parent.Text == @"main" &&
                this.projTreeView.SelectedNode.Text == @"rclick@main")
            {
                MessageBox.Show(@"main场景下的rclick函数不可以被删除");
                return;
            }
            var dr = MessageBox.Show(@"真的要删除函数吗" + Environment.NewLine + @"这是一个不可撤销的动作",
                @"确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                this.core.DashDeleteFunction(this.projTreeView.SelectedNode.Parent.Text, this.projTreeView.SelectedNode.Text);
            }
        }
        #endregion

        #region 添加命令项
        /// <summary>
        /// 按钮：显示对话
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogForm formDialog = new DialogForm("显示对话", false);
            formDialog.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：状态变更
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            AForm af = new AForm(false);
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
            MusicForm mf = new MusicForm("插入音乐", 0, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放BGS
        /// </summary>
        private void button13_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("插入音乐", 1, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放SE
        /// </summary>
        private void button16_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("插入音乐", 2, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：播放Vocal
        /// </summary>
        private void button15_Click(object sender, EventArgs e)
        {
            VocalForm vf = new VocalForm(false);
            vf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：标签
        /// </summary>
        private void button20_Click(object sender, EventArgs e)
        {
            LabelForm lf = new LabelForm(false);
            lf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：跳转
        /// </summary>
        private void button19_Click(object sender, EventArgs e)
        {
            JumpForm jf = new JumpForm(false);
            jf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：开关操作
        /// </summary>
        private void button22_Click(object sender, EventArgs e)
        {
            SwitchesForm sf = new SwitchesForm("开关操作", false);
            sf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：注释
        /// </summary>
        private void button23_Click(object sender, EventArgs e)
        {
            NotationForm nf = new NotationForm(false);
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
            WaitForm wf = new WaitForm(false);
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
            DialogForm df = new DialogForm("代码片段", false);
            df.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：移除图片
        /// </summary>
        private void button32_Click(object sender, EventArgs e)
        {
            DeleteViewForm dvf = new DeleteViewForm(0, false);
            dvf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：移除按钮
        /// </summary>
        private void button31_Click(object sender, EventArgs e)
        {
            SCameraForm scf = new SCameraForm(false);
            scf.ShowDialog(this);
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
            MoveForm mf = new MoveForm(false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：显示背景
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            BgForm bf = new BgForm(false);
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
            ButtonForm bf = new ButtonForm(false);
            bf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：显示图片
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            PicturesForm pf = new PicturesForm(false);
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
            VarForm vf = new VarForm(false);
            vf.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：条件分歧
        /// </summary>
        private void button26_Click(object sender, EventArgs e)
        {
            IfForm iff = new IfForm(false);
            iff.ShowDialog(this);
        }

        /// <summary>
        /// 按钮：呼叫存档画面
        /// </summary>
        private void button38_Click(object sender, EventArgs e)
        {
            this.core.DashSave();
        }

        /// <summary>
        /// 按钮：呼叫读档画面
        /// </summary>
        private void button37_Click_1(object sender, EventArgs e)
        {
            this.core.DashLoad();
        }

        /// <summary>
        /// 按钮：返回标题画面
        /// </summary>
        private void button39_Click(object sender, EventArgs e)
        {
            this.core.DashTitle();
        }

        /// <summary>
        /// 按钮：结束游戏
        /// </summary>
        private void button35_Click_2(object sender, EventArgs e)
        {
            this.core.DashShutdown();
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
        /// 菜单：编辑
        /// </summary>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedIndex == -1)
            {
                return;
            }
            var editPack = this.core.GetEditPackage(this.codeListBox.SelectedIndex);
            switch (editPack.nodeType)
            {
                case ActionPackageType.act_dialog:
                    DialogForm editDialogDf = new DialogForm("显示对话", true, editPack.argsDict["context"].valueExp);
                    editDialogDf.ShowDialog(this);
                    break;
                case ActionPackageType.script:
                    DialogForm editScriptDf = new DialogForm("代码片段", true, editPack.argsDict["context"].valueExp);
                    editScriptDf.ShowDialog(this);
                    break;
                case ActionPackageType.notation:
                    NotationForm editNf = new NotationForm(true, editPack.argsDict["context"].valueExp);
                    editNf.ShowDialog(this);
                    break;
                case ActionPackageType.act_wait:
                    WaitForm editWf = new WaitForm(true, Convert.ToInt32(editPack.argsDict["time"].valueExp));
                    editWf.ShowDialog(this);
                    break;
                case ActionPackageType.act_a:
                    AForm editAf = new AForm(true, editPack.argsDict["name"].valueExp,
                        editPack.argsDict["face"].valueExp, editPack.argsDict["loc"].valueExp,
                        editPack.argsDict["vid"].valueExp);
                    editAf.ShowDialog(this);
                    break;
                case ActionPackageType.act_picture:
                    PicturesForm editPf = new PicturesForm(true, editPack.argsDict["id"].valueExp,
                        editPack.argsDict["filename"].valueExp, editPack.argsDict["x"].valueExp,
                        editPack.argsDict["y"].valueExp, editPack.argsDict["xscale"].valueExp,
                        editPack.argsDict["yscale"].valueExp, editPack.argsDict["opacity"].valueExp,
                        editPack.argsDict["ro"].valueExp);
                    editPf.ShowDialog(this);
                    break;
                case ActionPackageType.act_move:
                    MoveForm editMf = new MoveForm(true, editPack.argsDict["name"].valueExp,
                        editPack.argsDict["id"].valueExp, editPack.argsDict["time"].valueExp,
                        editPack.argsDict["target"].valueExp, editPack.argsDict["dash"].valueExp,
                        editPack.argsDict["acc"].valueExp);
                    editMf.ShowDialog(this);
                    break;
                case ActionPackageType.act_button:
                    ButtonForm editBf = new ButtonForm(true, editPack.argsDict["id"].valueExp,
                        editPack.argsDict["x"].valueExp, editPack.argsDict["y"].valueExp,
                        editPack.argsDict["target"].valueExp, editPack.argsDict["type"].valueExp,
                        editPack.argsDict["normal"].valueExp, editPack.argsDict["over"].valueExp,
                        editPack.argsDict["on"].valueExp);
                    editBf.ShowDialog(this);
                    break;
                case ActionPackageType.act_jump:
                    JumpForm editJf = new JumpForm(true, editPack.argsDict["filename"].valueExp,
                        editPack.argsDict["target"].valueExp, editPack.argsDict["cond"].valueExp);
                    editJf.ShowDialog(this);
                    break;
                case ActionPackageType.act_label:
                    LabelForm editLf = new LabelForm(true, editPack.argsDict["name"].valueExp);
                    editLf.ShowDialog(this);
                    break;
                case ActionPackageType.act_vocal:
                    VocalForm editVocalf = new VocalForm(true, editPack.argsDict["name"].valueExp,
                        editPack.argsDict["vid"].valueExp);
                    editVocalf.ShowDialog();
                    break;
                case ActionPackageType.act_scamera:
                    SCameraForm editScf = new SCameraForm(true, editPack.argsDict["name"].valueExp,
                        editPack.argsDict["x"].valueExp, editPack.argsDict["y"].valueExp,
                        editPack.argsDict["ro"].valueExp);
                    editScf.ShowDialog(this);
                    break;
                case ActionPackageType.act_deletepicture:
                    DeleteViewForm editDpf = new DeleteViewForm(0, true, editPack.argsDict["id"].valueExp);
                    editDpf.ShowDialog(this);
                    break;
                case ActionPackageType.act_deletecstand:
                    DeleteViewForm editDcf = new DeleteViewForm(1, true, editPack.argsDict["id"].valueExp);
                    editDcf.ShowDialog(this);
                    break;
                case ActionPackageType.act_deletebutton:
                    DeleteViewForm editDsf = new DeleteViewForm(2, true, editPack.argsDict["id"].valueExp);
                    editDsf.ShowDialog(this);
                    break;
                case ActionPackageType.act_bg:
                    BgForm editBgf = new BgForm(true, editPack.argsDict["id"].valueExp,
                        editPack.argsDict["filename"].valueExp, editPack.argsDict["ro"].valueExp);
                    editBgf.ShowDialog(this);
                    break;
                case ActionPackageType.act_var:
                    VarForm editVf = new VarForm(true, editPack.argsDict["opLeft"].valueExp,
                        editPack.argsDict["op"].valueExp, editPack.argsDict["opRight"].valueExp);
                    editVf.ShowDialog(this);
                    break;
                case ActionPackageType.act_if:
                    IfForm editIff = new IfForm(true, Convert.ToBoolean(editPack.argsDict["?elseflag"].valueExp),
                        editPack.argsDict["expr"].valueExp, editPack.argsDict["op1"].valueExp,
                        editPack.argsDict["opr"].valueExp, editPack.argsDict["op2"].valueExp);
                    editIff.ShowDialog(this);
                    break;
                case ActionPackageType.act_switch:
                    SwitchesForm editSwf = new SwitchesForm("开关操作", true,
                        editPack.argsDict["id"].valueExp, editPack.argsDict["dash"].valueExp);
                    editSwf.ShowDialog(this);
                    break;
                //case ActionPackageType.act_bgm:
                //    MusicForm editBgmf = new MusicForm("插入音乐", 0, true);
                //    editBgmf.ShowDialog(this);
                //    break;
                default:
                    MessageBox.Show(@"该项目不支持编辑");
                    break;
            }
        }

        /// <summary>
        /// 菜单：复制
        /// </summary>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedIndex != -1)
            {
                this.core.CopyCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
            }
        }

        /// <summary>
        /// 菜单：删除
        /// </summary>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedIndex != -1)
            {
                var selected = this.codeListBox.SelectedIndex;
                this.core.DeleteCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
                this.core.RefreshCodeContext();
                this.actionGroupBox.Enabled = false;
                this.codeListBox.SelectedIndex = selected;
            }
        }

        /// <summary>
        /// 菜单：剪切
        /// </summary>
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedIndex != -1)
            {
                var selected = this.codeListBox.SelectedIndex;
                this.core.CutCode(this.codeListBox.SelectedIndex, this.codeListBox.SelectedIndices.Count);
                this.core.RefreshCodeContext();
                this.actionGroupBox.Enabled = false;
                this.codeListBox.SelectedIndex = selected;
            }
        }

        /// <summary>
        /// 菜单：粘贴
        /// </summary>
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.codeListBox.SelectedIndex != -1)
            {
                var selected = this.codeListBox.SelectedIndex;
                this.core.PasteCode(this.codeListBox.SelectedIndex);
                this.core.RefreshCodeContext();
                this.actionGroupBox.Enabled = false;
                this.codeListBox.SelectedIndex = selected;
            }
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
            MusicForm mf = new MusicForm("音乐管理", 0, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：BGS
        /// </summary>
        private void bGSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 1, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：SE
        /// </summary>
        private void sEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 2, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：Vocal
        /// </summary>
        private void vOCALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicForm mf = new MusicForm("音乐管理", 3, false);
            mf.ShowDialog(this);
        }

        /// <summary>
        /// 菜单：退出
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.core.SaveProject();
            //var dr = MessageBox.Show(@"确定要退出吗" + Environment.NewLine + @"未保存的工作将会丢失", @"退出Halation",
            //    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            //if (dr == DialogResult.OK)
            //{
            Environment.Exit(0);
            //}
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
            fd.Filter = @"Halation工程|*.yrproj";
            fd.ShowDialog(this);
            if (fd.FileName != String.Empty)
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
            SwitchesForm sf = new SwitchesForm("开关管理器", false);
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
            try
            {
                this.core.DashParse();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 菜单：生成并运行
        /// </summary>
        private void 生成并运行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.core.DashParse();
                DebugForm df = new DebugForm(Halation.projectFolder);
                df.ShowDialog(this);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 菜单：关于
        /// </summary>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow ab = new AboutWindow();
            ab.ShowDialog();
        }

        /// <summary>
        /// 菜单项：包管理器
        /// </summary>
        private void 包管理器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserPackerForm upf = new UserPackerForm();
            upf.ShowDialog(this);
        }
        #endregion
    }
}
