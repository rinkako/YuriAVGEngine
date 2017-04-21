using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：状态变更
    /// </summary>
    internal class ACommand : HalationSingleCommand
    {
        /// <summary>
        /// 变更状态
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="name">目标角色的名字</param>
        /// <param name="face">目标立绘表情</param>
        /// <param name="loc">目标例会位置</param>
        /// <param name="vid">目标语音</param>
        public ACommand(int line, int indent, RunnablePackage parent, string name, string face, string loc, string vid)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("face", new KeyValuePair<ArgType, string>(ArgType.Arg_face, face)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("loc", new KeyValuePair<ArgType, string>(ArgType.Arg_loc, loc)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("vid", new KeyValuePair<ArgType, string>(ArgType.Arg_vid, vid)));
            base.Init(hal, ActionPackageType.act_a);
        }
    }
}
