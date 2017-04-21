using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：注释
    /// </summary>
    internal class EditNotationCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 注释
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="context">内容</param>
        public EditNotationCommand(int line, int indent, RunnablePackage parent, string context)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("context", new KeyValuePair<ArgType, string>(ArgType.unknown, context)));
            base.Init(hal, ActionPackageType.notation);
        }
    }
}
