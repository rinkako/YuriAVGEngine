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
    class DialogCommand : IHalationCommand
    {
        /// <summary>
        /// 显示对话
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="dialog">对话的内容</param>
        public DialogCommand(int line, string dialog)
        {
            this.dialogContext = dialog;
            ActionPackage ap = new ActionPackage()
            {
                line = line,
                argsVector = null,
                nodeName = "dialog",
                nodeType = ActionPackageType.act_dialog
            };

            HalationViewCommand.AddItemToCodeListbox(line, String.Format("◆ 显示"));
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public void Dash()
        {
            
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {

        }

        /// <summary>
        /// 对话的内容
        /// </summary>
        public string dialogContext { get; set; }
    }
}
