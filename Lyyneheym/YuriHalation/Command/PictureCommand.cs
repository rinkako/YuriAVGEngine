using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示背景
    /// </summary>
    internal class PictureCommand : HalationSingleCommand
    {
        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">层id</param>
        /// <param name="filename">资源名</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="ro">角度</param>
        public PictureCommand(int line, int indent, RunnablePackage parent, string id, string filename, string x, string y, string xscale, string yscale, string opacity, string ro)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("filename", new KeyValuePair<ArgType, string>(ArgType.Arg_filename, filename)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("x", new KeyValuePair<ArgType, string>(ArgType.Arg_x, x)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("y", new KeyValuePair<ArgType, string>(ArgType.Arg_y, y)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("xscale", new KeyValuePair<ArgType, string>(ArgType.Arg_xscale, xscale)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("yscale", new KeyValuePair<ArgType, string>(ArgType.Arg_yscale, yscale)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("opacity", new KeyValuePair<ArgType, string>(ArgType.Arg_normal, opacity)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("ro", new KeyValuePair<ArgType, string>(ArgType.Arg_over, ro)));
            base.Init(hal, ActionPackageType.act_picture);
        }
    }
}
