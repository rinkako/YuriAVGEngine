using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    internal class MsgLayerCommand : HalationSingleCommand
    {
        /// <summary>
        /// 变更文字层
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="toId">目标层</param>
        public MsgLayerCommand(int line, int indent, RunnablePackage parent, string toId)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, toId)));
            base.Init(hal, ActionPackageType.act_msglayer);
        }
    }
}
