using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示对话
    /// </summary>
    internal class EditWaitCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 显示对话
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="span">等待的时长</param>
        public EditWaitCommand(int line, int indent, RunnablePackage parent, string span)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("time", new KeyValuePair<ArgType, string>(ArgType.Arg_time, span)));
            base.Init(hal, ActionPackageType.act_wait);
        }
    }
}
