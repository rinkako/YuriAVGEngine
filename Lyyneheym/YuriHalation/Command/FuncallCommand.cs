using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：函数调用
    /// </summary>
    internal class FuncallCommand : HalationSingleCommand
    {
        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="funCallName">函数名</param>
        /// <param name="args">实参列表</param>
        public FuncallCommand(int line, int indent, RunnablePackage parent, string funCallName, string args)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("name", new KeyValuePair<ArgType, string>(ArgType.Arg_name, funCallName)));
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("sign", new KeyValuePair<ArgType, string>(ArgType.Arg_sign, args)));
            base.Init(hal, ActionPackageType.act_call);
        }
    }
}
