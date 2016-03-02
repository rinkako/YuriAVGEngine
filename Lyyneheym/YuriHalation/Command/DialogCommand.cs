using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// 命令类：显示对话
    /// </summary>
    class DialogCommand : IHalationCommandAttribute, IHalationCommand
    {
        /// <summary>
        /// 显示对话
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="dialog">对话的内容</param>
        public DialogCommand(int line, int indent, RunnablePackage parent, string dialog)
            : base(line, indent, parent)
        {
            this.dialogContext = dialog;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public void Dash()
        {
            var ArgDict = new Dictionary<string, ArgumentPackage>();
            ArgDict.Add("context", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.dialogContext });
            ActionPackage ap = new ActionPackage()
            {
                line = this.commandLine,
                indent = this.indent,
                argsDict = ArgDict,
                nodeName = this.ToString(),
                nodeType = ActionPackageType.act_dialog
            };
            this.parent.AddAction(ap, this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap.indent,
                String.Format("◆{0}  {1}", ap.GetActionName(), ap.GetParaDescription()));
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            this.parent.DeleteAction(this.commandLine);
            HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
        }

        /// <summary>
        /// 对话的内容
        /// </summary>
        public string dialogContext { get; set; }
    }
}
