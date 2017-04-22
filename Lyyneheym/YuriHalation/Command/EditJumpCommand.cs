using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：跳转
    /// </summary>
    internal class EditJumpCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="filename">目标场景名</param>
        /// <param name="target">目标标签名</param>
        /// <param name="cond">条件</param>
        public EditJumpCommand(int line, int indent, RunnablePackage parent, string filename, string target, string cond)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("filename", new KeyValuePair<ArgType, string>(ArgType.Arg_filename, filename)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("target", new KeyValuePair<ArgType, string>(ArgType.Arg_target, target)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("cond", new KeyValuePair<ArgType, string>(ArgType.Arg_cond, cond)));
            base.Init(hal, ActionPackageType.act_jump);
        }
    }
}
