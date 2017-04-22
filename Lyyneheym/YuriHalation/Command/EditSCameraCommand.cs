using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：场景镜头动画
    /// </summary>
    internal class EditSCameraCommand : HalationSingleEditCommand
    {
        /// <summary>
        /// 场景镜头动画
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="name">命令名</param>
        /// <param name="r">作用横向块编号</param>
        /// <param name="c">作用纵向块编号</param>
        /// <param name="ro">缩放比</param>
        public EditSCameraCommand(int line, int indent, RunnablePackage parent, string name, string r, string c, string ro)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, name)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("x", new KeyValuePair<ArgType, string>(ArgType.Arg_x, r)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("y", new KeyValuePair<ArgType, string>(ArgType.Arg_y, c)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("ro", new KeyValuePair<ArgType, string>(ArgType.Arg_ro, ro)));
            base.Init(hal, ActionPackageType.act_scamera);
        }
    }
}
