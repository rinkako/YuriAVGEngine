using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：场景镜头动画
    /// </summary>
    class SCameraCommand : IHalationSingleCommand
    {
        /// <summary>
        /// 动画
        /// </summary>
        /// <param name="name">命令名</param>
        /// <param name="x">作用横向块编号</param>
        /// <param name="y">作用纵向块编号</param>
        /// <param name="ro">缩放比</param>
        public SCameraCommand(int line, int indent, RunnablePackage parent, string name, string x, string y, string ro)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("x", new KeyValuePair<ArgType, string>(ArgType.Arg_x, x)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("y", new KeyValuePair<ArgType, string>(ArgType.Arg_y, y)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("ro", new KeyValuePair<ArgType, string>(ArgType.Arg_ro, ro)));
            base.Init(hal, ActionPackageType.act_scamera);
        }
    }
}
