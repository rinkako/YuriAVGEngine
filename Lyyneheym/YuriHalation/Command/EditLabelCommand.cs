using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：添加标签
    /// </summary>
    internal class EditLabelCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="name">标签名</param>
        public EditLabelCommand(int line, int indent, RunnablePackage parent, string name)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            base.Init(hal, ActionPackageType.act_label);
        }
    }
}
