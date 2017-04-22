using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：动画
    /// </summary>
    internal class EditMoveCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 动画
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="name">目标角色的名字</param>
        /// <param name="id">目标立绘表情</param>
        /// <param name="time">目标例会位置</param>
        /// <param name="target">目标语音</param>
        /// <param name="dash">目标值</param>
        /// <param name="acc">加速度比</param>
        public EditMoveCommand(int line, int indent, RunnablePackage parent, string name, string id, string time, string target, string dash, string acc)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("time", new KeyValuePair<ArgType, string>(ArgType.Arg_time, time)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("target", new KeyValuePair<ArgType, string>(ArgType.Arg_target, target)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("dash", new KeyValuePair<ArgType, string>(ArgType.Arg_dash, dash)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("acc", new KeyValuePair<ArgType, string>(ArgType.Arg_acc, acc)));
            base.Init(hal, ActionPackageType.act_move);
        }
    }
}
