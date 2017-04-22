using System;
using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    internal class EditIfCommand : IHalationCommand
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
        public EditIfCommand(int line, int indent, RunnablePackage parent, bool containElse, string expr, string op1, string opr, string op2)
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
            // 缓存编辑前的动作
            this.LastAP = this.parent.GetAction(this.commandLine);
            // 更新后台
            this.parent.ReplaceAction(ap1, this.commandLine);
            // 更新前端
            HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, ap1.indent,
                String.Format("◆{0}{1}{2}", ap1.GetActionName(), ap1.GetSpace(), ap1.GetParaDescription()));
        }

        /// <summary>
        /// 重载撤销命令
        /// </summary>
        public void Undo()
        {
            // 更新后台
            this.parent.ReplaceAction(this.LastAP, this.commandLine);
            // 更新前端
            HalationViewCommand.RemoveItemFromCodeListbox(this.commandLine);
            HalationViewCommand.AddItemToCodeListbox(this.commandLine, this.LastAP.indent,
                String.Format("◆{0}{1}{2}", this.LastAP.GetActionName(), this.LastAP.GetSpace(), this.LastAP.GetParaDescription()));
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
        /// 旧内容包装
        /// </summary>
        private ActionPackage LastAP;

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
