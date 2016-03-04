using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuriHalation;
using YuriHalation.YuriForms;
using Yuri.YuriHalation.ScriptPackage;
using Yuri.YuriHalation.HalationCore;
using Yuri.YuriHalation.Command;

namespace Yuri
{
    /// <summary>
    /// 控制器类：负责前端与后台的交互
    /// </summary>
    internal sealed class Halation
    {
        #region 辅助函数
        /// <summary>
        /// 获取要插入节点的对齐偏移
        /// </summary>
        /// <param name="insertLine">要插入的位置</param>
        /// <returns>偏移值</returns>
        public int GetIndent(int insertLine)
        {
            var act = Halation.currentCodePackage.GetAction(insertLine);
            return act.indent;
        }
        #endregion

        #region 前端响应相关
        /// <summary>
        /// 根据后台数据刷新前端工程树
        /// </summary>
        public void RefreshProjectTree(string selectSc = "main")
        {
            if (Halation.projectTreeRoot != null)
            {
                Halation.mainView.projTreeView.Nodes.Remove(Halation.projectTreeRoot);
            }
            Halation.projectTreeRoot = Halation.mainView.projTreeView.Nodes.Add(Halation.projectName);
            TreeNode selectNode = null;
            foreach (var sc in Halation.project.GetScene())
            {
                var curNode = Halation.projectTreeRoot.Nodes.Add(sc.sceneName);
                foreach (var fc in sc.GetFunc())
                {
                    curNode.Nodes.Add(fc.functionCallName);
                }
                if (sc.sceneName == selectSc)
                {
                    selectNode = curNode;
                }
                if (sc.sceneName == "main")
                {
                    Halation.projectTreeMain = curNode;
                }
            }
            Halation.mainView.projTreeView.SelectedNode = selectNode != null ? selectNode : Halation.projectTreeMain;
            Halation.mainView.projTreeView.ExpandAll();
        }

        /// <summary>
        /// 根据后台数据更新前端代码
        /// </summary>
        public void RefreshCodeContext()
        {
            HalationViewCommand.ClearAll();
            var ActList = Halation.currentCodePackage.GetAction();
            foreach (var act in ActList)
            {
                if (act.nodeName != "pad")
                {
                    HalationViewCommand.AddItemToCodeListbox(-1, act.indent,
                        String.Format("{0}{1}  {2}", act.GetFlag(), act.GetActionName(), act.GetParaDescription()));
                }
                else
                {
                    HalationViewCommand.AddItemToCodeListbox(-1, act.indent, "◆ ");
                }
            }
            this.RefreshRedoUndo();
        }

        /// <summary>
        /// 变更当前操作的场景或函数
        /// </summary>
        /// <param name="toRunnable">目标场景或函数</param>
        public void ChangeCodePackage(string toRunnable, string parent)
        {
            RunnablePackage rp = null;
            if (parent == "")
            {
                rp = Halation.project.GetScene(toRunnable);
            }
            else
            {
                rp = ((ScenePackage)Halation.project.GetScene(parent)).GetFunc(toRunnable.Split('@')[0]);
            }

            Halation.currentCodePackage = rp;
        }

        /// <summary>
        /// 刷新菜单重做与否的可执行性
        /// </summary>
        public void RefreshRedoUndo()
        {
            Halation.mainView.menuStrip1.Items.Find("撤销ToolStripMenuItem", true)[0].Enabled = this.IsAbleUndo();
            Halation.mainView.menuStrip1.Items.Find("重做ToolStripMenuItem", true)[0].Enabled = this.IsAbleRedo();
        }

        /// <summary>
        /// 正在编辑的可运行代码名称
        /// </summary>
        public static string currentScriptName = "";

        /// <summary>
        /// 代码树被选中节点
        /// </summary>
        public static TreeNode projectTreeChosen = null;

        /// <summary>
        /// 代码树前端根节点
        /// </summary>
        public static TreeNode projectTreeRoot = null;

        /// <summary>
        /// 代码树main节点
        /// </summary>
        public static TreeNode projectTreeMain = null;
        #endregion

        #region 前端命令相关
        
        /// <summary>
        /// 当前选中的行号
        /// </summary>
        public static int CurrentSelectedLine
        {
            get
            {
                return Halation.mainView.codeListBox.SelectedIndex;
            }
        }

        public bool DashAddScene(string scenario)
        {
            bool flag = Halation.project.AddScene(scenario);
            if (flag)
            {
                this.RefreshProjectTree(scenario);
            }
            return flag;
        }

        public void DashDeleteScene(string scenario)
        {
            Halation.project.DeleteScene(scenario);
            HalationInvoker.RemoveScene(scenario);
            this.RefreshProjectTree();
        }

        public bool DashAddFunction(string functionName, List<string> argv)
        {
            bool flag = ((ScenePackage)Halation.currentCodePackage).AddFunction(functionName, argv);
            if (flag)
            {
                string callName = String.Format("{0}@{1}", functionName, Halation.currentScriptName);
                this.RefreshProjectTree(callName);
            }
            return flag;
        }
        
        public void DashDeleteFunction(string functionName)
        {
            ((ScenePackage)Halation.currentCodePackage).DeleteFunction(functionName);
            string callName = String.Format("{0}@{1}", functionName, Halation.projectTreeChosen.Parent.Text);
            HalationInvoker.RemoveScene(callName);
            this.RefreshProjectTree();
        }

        public void DashDialog(string context)
        {
            IHalationCommand cmd = new DialogCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, context);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashA(string toName, string toFace, string toLoc, string toVocal)
        {
            IHalationCommand cmd = new ACommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, toName, toFace, toLoc, toVocal);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashMsgLayer(string toLayerId)
        {
            IHalationCommand cmd = new MsgLayerCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, toLayerId);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashDraw(string str)
        {
            IHalationCommand cmd = new DrawCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, str);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashBranch(List<string> branchItems)
        {
            IHalationCommand cmd = new BranchCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, branchItems);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashMsgLayerOpt(string layerId, string target, string value)
        {
            IHalationCommand cmd = new MsgLayerOptCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, layerId, target, value);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashStopBGM()
        {
            IHalationCommand cmd = new StopBGMCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashStopBGS()
        {
            IHalationCommand cmd = new StopBGSCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashPlayBGM(string filename, string vol)
        {
            IHalationCommand cmd = new BGMCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, filename, vol);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashPlayBGS(string filename, string vol)
        {
            IHalationCommand cmd = new BGSCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, filename, vol);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashPlaySE(string filename, string vol)
        {
            IHalationCommand cmd = new SECommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, filename, vol);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashPlayVocal(string name, string vid)
        {
            IHalationCommand cmd = new VocalCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, name, vid);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashLabel(string label)
        {
            IHalationCommand cmd = new LabelCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, label);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashJump(string target, string filename = "", string cond = "")
        {
            IHalationCommand cmd = new JumpCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, filename, target, cond);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashSwitches(string id, string state)
        {
            IHalationCommand cmd = new SwitchCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, id, state);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashNotation(string context)
        {
            IHalationCommand cmd = new NotationCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, context);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashBreak()
        {
            IHalationCommand cmd = new BreakCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashReturn()
        {
            IHalationCommand cmd = new ReturnCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashWaituser()
        {
            IHalationCommand cmd = new WaituserCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashWait(string span)
        {
            IHalationCommand cmd = new WaitCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, span);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashWaitani()
        {
            IHalationCommand cmd = new WaitaniCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashFor()
        {
            IHalationCommand cmd = new ForCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashScript(string context)
        {
            IHalationCommand cmd = new ScriptCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, context);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashDeletepicture(string id)
        {
            IHalationCommand cmd = new DeletepicCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, id);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashDeletecstand(string id)
        {
            IHalationCommand cmd = new DeletestandCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, id);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashDeletebutton(string id)
        {
            IHalationCommand cmd = new DeletebuttonCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, id);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashTrans(string type)
        {
            IHalationCommand cmd = new TransCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, type);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        public void DashMove(string name, string id, string time, string target, string dash, string acc)
        {
            IHalationCommand cmd = new MoveCommand(Halation.CurrentSelectedLine, this.GetIndent(Halation.CurrentSelectedLine), Halation.currentCodePackage, name, id, time, target, dash, acc);
            HalationInvoker.Dash(Halation.currentScriptName, cmd);
        }

        #endregion

        #region 前端菜单相关

        public bool MenuUndo()
        {
            HalationInvoker.Undo(Halation.currentScriptName);
            return HalationInvoker.IsAbleUndo(Halation.currentScriptName);
        }

        public bool MenuRedo()
        {
            HalationInvoker.Redo(Halation.currentScriptName);
            return HalationInvoker.IsAbleRedo(Halation.currentScriptName);
        }

        public bool IsAbleUndo()
        {
            return HalationInvoker.IsAbleUndo(Halation.currentScriptName);
        }

        public bool IsAbleRedo()
        {
            return HalationInvoker.IsAbleRedo(Halation.currentScriptName);
        }

        /// <summary>
        /// 建立一个新工程
        /// </summary>
        /// <param name="path">要建立工程的目录</param>
        /// <param name="projName">工程名称</param>
        public void NewProject(string path, string projName)
        {
            FileManager.Instance.CreateInitFolder(string.Format("{0}\\{1}", path, projName));
            Halation.project = new Yuri.YuriHalation.ScriptPackage.ProjectPackage(projName);
            Halation.project.AddScene("main");
            Halation.projectName = projName;
            Halation.mainView.Text = String.Format("Yuri Halation - [{0}]", Halation.projectName);
            FileManager.serialization(Halation.project, string.Format("{0}\\{1}\\game.yrproj", path, projName));
            Halation.projectFolder = string.Format("{0}\\{1}", path, projName);
        }

        /// <summary>
        /// 保存工程
        /// </summary>
        public void SaveProject()
        {
            FileManager.serialization(Halation.project, string.Format("{0}\\game.yrproj", projectFolder));
        }

        /// <summary>
        /// 读取工程
        /// </summary>
        /// <param name="projFile">工程文件</param>
        public void LoadProject(string projFile)
        {
            FileInfo fileinf = new FileInfo(projFile);
            Halation.projectName = fileinf.Directory.Name;
            Halation.projectFolder = fileinf.DirectoryName;
            Halation.project = (ProjectPackage)FileManager.unserialization(projFile);
            foreach (var sc in Halation.project.GetScene())
            {
                HalationInvoker.AddScene(sc.sceneName);
                foreach (var fc in sc.GetFunc())
                {
                    HalationInvoker.AddScene(fc.functionCallName);
                }
            }
            this.RefreshProjectTree("main");
        }

        /// <summary>
        /// 目前工程的根目录
        /// </summary>
        public static string projectFolder;

        #endregion

        #region 工程实例相关
        /// <summary>
        /// 获取或设置工程的名字
        /// </summary>
        public static string projectName { get; set; }

        /// <summary>
        /// 获取或设置当前工程包装
        /// </summary>
        public static ProjectPackage project { get; set; }

        /// <summary>
        /// 现在正在编辑的代码包装
        /// </summary>
        public static RunnablePackage currentCodePackage { get; set; }
        #endregion

        #region 前端信息相关
        /// <summary>
        /// 为程序各模块设置更新主窗体的引用
        /// </summary>
        /// <param name="mw">主窗体实例</param>
        public static void SetViewReference(MainForm mw)
        {
            Halation.mainView = mw;
        }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        public static MainForm mainView = null;

        /// <summary>
        /// 获取代码列表框引用
        /// </summary>
        public static ListBox codeListBox
        {
            get
            {
                return Halation.mainView != null ? Halation.mainView.codeListBox : null;
            }
        }

        /// <summary>
        /// 代码缩进偏移量
        /// </summary>
        public static readonly int codeIndent = 4;
        #endregion

        #region 类自身相关
        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>Halation实例</returns>
        public static Halation GetInstance()
        {
            return Halation.synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Halation()
        {

        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static readonly Halation synObject = new Halation();
        #endregion
    }
}
