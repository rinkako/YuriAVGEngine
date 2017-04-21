using System;
using System.Collections.Generic;
using System.Linq;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    internal class HalationSingleCommand : IHalationCommand
    {
        /// <summary>
        /// 构造一个待执行的单条语句
        /// </summary>
        /// <param name="line">所在的行</param>
        /// <param name="indent">列偏移</param>
        /// <param name="parent">所述包装</param>
        public HalationSingleCommand(int line, int indent, RunnablePackage parent)
        {
            this.parent = parent;
            this.indent = indent;
            this.commandLine = line;
        }

        /// <summary>
        /// 执行这条指令
        /// </summary>
        public void Dash()
        {
            var ArgDict = this.ArgumentList.ToDictionary(kvp => kvp.Key, kvp => new ArgumentPackage() {aType = kvp.Value.Key, valueExp = kvp.Value.Value});
            ActionPackage ap = new ActionPackage()
            {
                indent = this.indent,
                argsDict = ArgDict,
                nodeName = String.Format("{0}@{1}", this.commandLine, this.apType),
                nodeType = this.apType
            };
            this.parent.AddAction(ap, this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap.indent,
                String.Format("◆{0}{1}{2}", ap.GetActionName(), ap.GetSpace(), ap.GetParaDescription()));
        }

        /// <summary>
        /// 撤销这条指令
        /// </summary>
        public void Undo()
        {
            this.parent.DeleteAction(this.commandLine);
            HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
        }

        /// <summary>
        /// 初始化执行参数
        /// </summary>
        /// <param name="argv">参数列表</param>
        /// <param name="apType">命令类型</param>
        public void Init(HalaAttrList argv, ActionPackageType apType)
        {
            this.ArgumentList = argv;
            this.apType = apType;
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        private HalaAttrList ArgumentList;

        /// <summary>
        /// 命令的类型
        /// </summary>
        private ActionPackageType apType;

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
