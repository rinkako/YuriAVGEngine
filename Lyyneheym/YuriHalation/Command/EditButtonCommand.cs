using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示背景
    /// </summary>
    internal class EditButtonCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">按钮id</param>
        /// <param name="x">按钮X坐标</param>
        /// <param name="y">按钮Y坐标</param>
        /// <param name="target">处理标签</param>
        /// <param name="type">按钮类型</param>
        /// <param name="normal">正常图</param>
        /// <param name="over">悬停图</param>
        /// <param name="on">按下图</param>
        public EditButtonCommand(int line, int indent, RunnablePackage parent, string id, string x, string y, string target, string type, string normal, string over, string on)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("x", new KeyValuePair<ArgType, string>(ArgType.Arg_x, x)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("y", new KeyValuePair<ArgType, string>(ArgType.Arg_y, y)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("target", new KeyValuePair<ArgType, string>(ArgType.Arg_target, target)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("type", new KeyValuePair<ArgType, string>(ArgType.Arg_type, type)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("normal", new KeyValuePair<ArgType, string>(ArgType.Arg_normal, normal)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("over", new KeyValuePair<ArgType, string>(ArgType.Arg_over, over)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("on", new KeyValuePair<ArgType, string>(ArgType.Arg_on, on)));
            base.Init(hal, ActionPackageType.act_button);
        }
    }
}
