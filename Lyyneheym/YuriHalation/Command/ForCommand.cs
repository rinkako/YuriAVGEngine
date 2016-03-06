using System;
using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    class ForCommand : IHalationCommand
    {
        /// <summary>
        /// 循环
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        public ForCommand(int line, int indent, RunnablePackage parent)
        {
            this.parent = parent;
            this.indent = indent;
            this.commandLine = line;
        }

        /// <summary>
        /// 重载执行命令
        /// </summary>
        public void Dash()
        {
            // FOR节点
            ActionPackage ap = new ActionPackage()
            {
                line = this.commandLine,
                indent = this.indent,
                argsDict = new Dictionary<string,ArgumentPackage>(),
                nodeName = String.Format("{0}@{1}", this.commandLine, ActionPackageType.act_for.ToString()),
                nodeType = ActionPackageType.act_for
            };
            this.parent.AddAction(ap, ap.line);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap.indent, "◆循环");
            // PAD节点
            ActionPackage ap2 = new ActionPackage()
            {
                line = this.commandLine + 1,
                indent = this.indent + 2,
                argsDict = new Dictionary<string,ArgumentPackage>(),
                nodeName = "pad",
                nodeType = ActionPackageType.NOP
            };
            this.parent.AddAction(ap2, ap2.line);
            HalationViewCommand.AddItemToCodeListbox(ap2.line, ap2.indent, "◆");
            // ENDFOR节点
            ActionPackage ap3 = new ActionPackage()
            {
                line = this.commandLine + 2,
                indent = this.indent,
                argsDict = new Dictionary<string, ArgumentPackage>(),
                nodeName = String.Format("{0}@{1}", this.commandLine + 2, ActionPackageType.act_endfor.ToString()),
                nodeType = ActionPackageType.act_endfor
            };
            this.parent.AddAction(ap3, ap3.line);
            HalationViewCommand.AddItemToCodeListbox(ap3.line, ap3.indent, ":以上反复");
        }

        /// <summary>
        /// 重载撤销命令
        /// </summary>
        public void Undo()
        {
            // 这里没错，因为删除后下标变化了
            for (int i = 0; i < 3; i++)
            {
                this.parent.DeleteAction(this.commandLine);
                HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
            }
        }

        /// <summary>
        /// 属于的场景或函数
        /// </summary>
        public RunnablePackage parent { get; set; }

        /// <summary>
        /// 前端显示的对齐偏移
        /// </summary>
        public int indent { get; set; }

        /// <summary>
        /// 所在的行
        /// </summary>
        public int commandLine { get; set; }
    }
}
