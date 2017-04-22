using System;
using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    internal class IfCommand : IHalationCommand
    {
        /// <summary>
        /// 循环
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="containElse">是否包含else子句</param>
        /// <param name="expr">表达式</param>
        /// <param name="op1">操作数1</param>
        /// <param name="opr">操作符</param>
        /// <param name="op2">操作数2</param>
        public IfCommand(int line, int indent, RunnablePackage parent, bool containElse, string expr, string op1, string opr, string op2)
        {
            this.parent = parent;
            this.indent = indent;
            this.commandLine = line;
            this.isContainElse = containElse;
            this.operand1 = op1;
            this.operand2 = op2;
            this.operateMode = opr;
            this.condExpr = expr;
        }

        /// <summary>
        /// 重载执行命令
        /// </summary>
        public void Dash()
        {
            // IF节点
            Dictionary<string, ArgumentPackage> ifArgDict = new Dictionary<string, ArgumentPackage>();
            ifArgDict.Add("op1", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.operand1 });
            ifArgDict.Add("op2", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.operand2 });
            ifArgDict.Add("opr", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.operateMode });
            ifArgDict.Add("expr", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.condExpr });
            ifArgDict.Add("?elseflag", new ArgumentPackage() { aType = ArgType.unknown, valueExp = this.isContainElse.ToString() });
            ActionPackage ap1 = new ActionPackage()
            {
                indent = this.indent,
                argsDict = ifArgDict,
                nodeName = String.Format("{0}@{1}", this.commandLine, ActionPackageType.act_if.ToString()),
                nodeType = ActionPackageType.act_if
            };
            this.parent.AddAction(ap1, this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap1.indent,
                String.Format("◆{0}{1}{2}", ap1.GetActionName(), ap1.GetSpace(), ap1.GetParaDescription()));
            // PAD节点
            ActionPackage ap2 = new ActionPackage()
            {
                indent = this.indent + 2,
                argsDict = new Dictionary<string, ArgumentPackage>(),
                nodeName = "pad",
                nodeType = ActionPackageType.NOP
            };
            this.parent.AddAction(ap2, this.commandLine + 1);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine + 1, ap2.indent, "◆");
            // 考虑ELSE子句
            if (this.isContainElse)
            {
                // ELSE节点
                ActionPackage ap3 = new ActionPackage()
                {
                    indent = this.indent,
                    argsDict = new Dictionary<string, ArgumentPackage>(),
                    nodeName = String.Format("{0}@{1}", this.commandLine + 2, ActionPackageType.act_else.ToString()),
                    nodeType = ActionPackageType.act_else
                };
                this.parent.AddAction(ap3, this.commandLine + 2);
                HalationViewCommand.AddItemToCodeListbox(this.commandLine + 2, ap3.indent, ":除此以外的情况");
                // PAD节点
                ActionPackage ap4 = new ActionPackage()
                {
                    indent = this.indent + 2,
                    argsDict = new Dictionary<string, ArgumentPackage>(),
                    nodeName = "pad",
                    nodeType = ActionPackageType.NOP
                };
                this.parent.AddAction(ap4, this.commandLine + 3);
                HalationViewCommand.AddItemToCodeListbox(this.commandLine + 3, ap4.indent, "◆");
                // ENDIF节点
                ActionPackage ap5 = new ActionPackage()
                {
                    indent = this.indent,
                    argsDict = new Dictionary<string, ArgumentPackage>(),
                    nodeName = String.Format("{0}@{1}", this.commandLine + 2, ActionPackageType.act_endif.ToString()),
                    nodeType = ActionPackageType.act_endif
                };
                this.parent.AddAction(ap5, this.commandLine + 4);
                HalationViewCommand.AddItemToCodeListbox(this.commandLine + 4, ap5.indent, ":分支结束");
            }
            else
            {
                // ENDIF节点（这里不能与上面的endif合并，因为commandline有变化）
                ActionPackage ap6 = new ActionPackage()
                {
                    indent = this.indent,
                    argsDict = new Dictionary<string, ArgumentPackage>(),
                    nodeName = String.Format("{0}@{1}", this.commandLine + 2, ActionPackageType.act_endif.ToString()),
                    nodeType = ActionPackageType.act_endif
                };
                this.parent.AddAction(ap6, this.commandLine + 2);
                HalationViewCommand.AddItemToCodeListbox(this.commandLine + 2, ap6.indent, ":分支结束");
            }
        }

        /// <summary>
        /// 重载撤销命令
        /// </summary>
        public void Undo()
        {
            // 这里没错，因为删除后下标变化了
            int bound = this.isContainElse ? 5 : 3;
            for (int i = 0; i < bound; i++)
            {
                this.parent.DeleteAction(this.commandLine);
                HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
            }
        }

        /// <summary>
        /// 条件表达式
        /// </summary>
        public string condExpr { get; set; }

        /// <summary>
        /// 操作数1
        /// </summary>
        public string operand1 { get; set; }

        /// <summary>
        /// 操作数2
        /// </summary>
        public string operand2 { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public string operateMode { get; set; }

        /// <summary>
        /// 是否包含有else子句
        /// </summary>
        public bool isContainElse { get; set; }

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
