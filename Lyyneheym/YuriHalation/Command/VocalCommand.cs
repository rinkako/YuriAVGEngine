using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：播放Vocal
    /// </summary>
    internal class VocalCommand : HalationSingleCommand
    {
        /// <summary>
        /// 播放Vocal
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="name">Vocal所属的角色名</param>
        /// <param name="vid">语音文件的id号</param>
        public VocalCommand(int line, int indent, RunnablePackage parent, string name, string vid)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("vid", new KeyValuePair<ArgType, string>(ArgType.Arg_vid, vid)));
            base.Init(hal, ActionPackageType.act_vocal);
        }
    }
}
