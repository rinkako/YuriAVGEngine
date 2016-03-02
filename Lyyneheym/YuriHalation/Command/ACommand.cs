using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    class ACommand : IHalationCommandAttribute, IHalationCommand
    {
        public ACommand(int line, int indent, RunnablePackage parent, string name, string face, string loc, string vid)
            : base(line, indent, parent)
        {
            this.toName = name;
            this.toFace = face;
            this.toLoc = loc;
            this.toVid = vid;
        }

        public void Dash()
        {
            var ArgDict = new Dictionary<string, ArgumentPackage>();
            ArgDict.Add("name", new ArgumentPackage() { aType = ArgType.Arg_name, valueExp = this.toName });
            ArgDict.Add("face", new ArgumentPackage() { aType = ArgType.Arg_face, valueExp = this.toFace });
            ArgDict.Add("loc", new ArgumentPackage() { aType = ArgType.Arg_loc, valueExp = this.toLoc });
            ArgDict.Add("vid", new ArgumentPackage() { aType = ArgType.Arg_vid, valueExp = this.toVid });
            ActionPackage ap = new ActionPackage()
            {
                line = this.commandLine,
                indent = this.indent,
                argsDict = ArgDict,
                nodeName = this.ToString(),
                nodeType = ActionPackageType.act_a
            };
            this.parent.AddAction(ap, this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap.indent,
                String.Format("◆{0}  {1}", ap.GetActionName(), ap.GetParaDescription()));
        }

        public void Undo()
        {
            this.parent.DeleteAction(this.commandLine);
            HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
        }

        private string toName;

        private string toFace;

        private string toLoc;

        private string toVid;
    }
}
