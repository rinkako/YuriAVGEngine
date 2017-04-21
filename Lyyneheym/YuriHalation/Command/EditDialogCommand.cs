using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示对话
    /// </summary>
    internal class EditDialogCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 显示对话
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="dialog">对话的内容</param>
        public EditDialogCommand(int line, int indent, RunnablePackage parent, string dialog)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("context", new KeyValuePair<ArgType, string>(ArgType.unknown, dialog)));
            base.Init(hal, ActionPackageType.act_dialog);
        }
    }
}
