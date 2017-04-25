using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：播放BGM
    /// </summary>
    internal class EditBGMCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="filename">BGM资源名</param>
        /// <param name="vol">音量</param>
        public EditBGMCommand(int line, int indent, RunnablePackage parent, string filename, string vol)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("filename", new KeyValuePair<ArgType, string>(ArgType.Arg_filename, filename)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("vol", new KeyValuePair<ArgType, string>(ArgType.Arg_vol, vol)));
            base.Init(hal, ActionPackageType.act_bgm);
        }
    }
}
