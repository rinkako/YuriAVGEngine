using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：删除立绘
    /// </summary>
    internal class EditDeletestandCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 删除立绘
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">要删除的立绘id</param>
        public EditDeletestandCommand(int line, int indent, RunnablePackage parent, string id)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            base.Init(hal, ActionPackageType.act_deletecstand);
        }
    }
}
