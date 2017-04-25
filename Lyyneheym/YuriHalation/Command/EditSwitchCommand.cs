using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：开关操作
    /// </summary>
    internal class EditSwitchCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 开关操作
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">要操作的开关id</param>
        /// <param name="dash">目标状态</param>
        public EditSwitchCommand(int line, int indent, RunnablePackage parent, string id, string dash)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("dash", new KeyValuePair<ArgType, string>(ArgType.Arg_dash, dash)));
            base.Init(hal, ActionPackageType.act_switch);
        }
    }
}
