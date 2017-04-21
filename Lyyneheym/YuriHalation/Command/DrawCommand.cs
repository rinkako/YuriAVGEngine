using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：直接描绘文本
    /// </summary>
    internal class DrawCommand : HalationSingleCommand
    {
        /// <summary>
        /// 直接描绘文本
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="dialog">对话的内容</param>
        public DrawCommand(int line, int indent, RunnablePackage parent, string dialog)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("dash", new KeyValuePair<ArgType, string>(ArgType.Arg_dash, dialog)));
            base.Init(hal, ActionPackageType.act_draw);
        }
    }
}
