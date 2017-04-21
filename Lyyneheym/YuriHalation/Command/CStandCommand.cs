using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：显示背景
    /// </summary>
    internal class CstandCommand : HalationSingleCommand
    {
        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">背景层id</param>
        /// <param name="name">角色名</param>
        /// <param name="face">立绘表情</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="loc">相对位置</param>
        public CstandCommand(int line, int indent, RunnablePackage parent, string id, string name, string face, string x, string y, string loc)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("face", new KeyValuePair<ArgType, string>(ArgType.Arg_face, face)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("x", new KeyValuePair<ArgType, string>(ArgType.Arg_x, x)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("y", new KeyValuePair<ArgType, string>(ArgType.Arg_x, y)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("loc", new KeyValuePair<ArgType, string>(ArgType.Arg_loc, loc)));
            base.Init(hal, ActionPackageType.act_cstand);
        }
    }
}
