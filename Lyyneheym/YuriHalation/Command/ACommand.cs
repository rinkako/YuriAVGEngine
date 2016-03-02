using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    class ACommand : IHalationSingleCommand
    {
        public ACommand(int line, int indent, RunnablePackage parent, string name, string face, string loc, string vid)
            : base(line, indent, parent)
        {
            this.toName = name;
            this.toFace = face;
            this.toLoc = loc;
            this.toVid = vid;
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, this.toName)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("face", new KeyValuePair<ArgType, string>(ArgType.Arg_face, this.toName)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("loc", new KeyValuePair<ArgType, string>(ArgType.Arg_loc, this.toName)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("vid", new KeyValuePair<ArgType, string>(ArgType.Arg_vid, this.toName)));
            base.Init(hal, ActionPackageType.act_a);
        }

        private string toName { get; set; }

        private string toFace { get; set; }

        private string toLoc { get; set; }

        private string toVid { get; set; }
    }
}
