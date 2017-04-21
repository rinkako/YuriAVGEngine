using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：修改文本层属性
    /// </summary>
    internal class MsgLayerOptCommand : HalationSingleCommand
    {
        /// <summary>
        /// 修改文本层属性
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="id">目标层</param>
        /// <param name="target">目标属性</param>
        /// <param name="dash">目标值</param>
        public MsgLayerOptCommand(int line, int indent, RunnablePackage parent, string id, string target, string dash)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, id)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("target", new KeyValuePair<ArgType, string>(ArgType.Arg_target, target)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("dash", new KeyValuePair<ArgType, string>(ArgType.Arg_dash, dash)));
            base.Init(hal, ActionPackageType.act_msglayeropt);
        }
    }
}
