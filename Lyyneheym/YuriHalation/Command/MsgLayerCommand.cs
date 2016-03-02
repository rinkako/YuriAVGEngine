using System;
using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    class MsgLayerCommand : IHalationSingleCommand
    {
        public MsgLayerCommand(int line, int indent, RunnablePackage parent, string toId)
            : base(line, indent, parent)
        {
            this.toId = toId;
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("id", new KeyValuePair<ArgType, string>(ArgType.Arg_id, this.toId)));
            base.Init(hal, ActionPackageType.act_msglayer);
        }

        public string toId;
    }
}
