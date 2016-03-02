using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示对话
    /// </summary>
    class DialogCommand : IHalationSingleCommand
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
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("context", new KeyValuePair<ArgType, string>(ArgType.unknown, this.dialogContext)));
            base.Init(hal, ActionPackageType.act_dialog);
        }
        
        /// <summary>
        /// 对话的内容
        /// </summary>
        public string dialogContext { get; set; }
    }
}
