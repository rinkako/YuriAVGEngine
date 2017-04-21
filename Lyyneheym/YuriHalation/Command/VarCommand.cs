using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：变量操作
    /// </summary>
    internal class VarCommand : HalationSingleCommand
    {
        /// <summary>
        /// 变量操作
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="operandLeft">左操作数</param>
        /// <param name="operateMode">操作符</param>
        /// <param name="operandRight">右操作数</param>
        public VarCommand(int line, int indent, RunnablePackage parent, string operandLeft, string operateMode, string operandRight)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("opLeft", new KeyValuePair<ArgType, string>(ArgType.unknown, operandLeft)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("op", new KeyValuePair<ArgType, string>(ArgType.unknown, operateMode)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("opRight", new KeyValuePair<ArgType, string>(ArgType.unknown, operandRight)));
            base.Init(hal, ActionPackageType.act_var);
        }
    }
}
