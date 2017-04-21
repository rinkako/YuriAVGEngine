using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示背景
    /// </summary>
    internal class BgCommand : HalationSingleCommand
    {
        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">背景层id</param>
        /// <param name="filename">资源文件名</param>
        /// <param name="ro">3D世界深度</param>
        public BgCommand(int line, int indent, RunnablePackage parent, string id, string filename, string ro)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("ro", new KeyValuePair<ArgType, string>(ArgType.Arg_ro, ro)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("filename", new KeyValuePair<ArgType, string>(ArgType.Arg_filename, filename)));
            base.Init(hal, ActionPackageType.act_bg);
        }
    }
}
