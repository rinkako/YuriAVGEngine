using System.Collections.Generic;

using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：代码片段
    /// </summary>
    internal class ScriptCommand : HalationSingleCommand
    {
        /// <summary>
        /// 代码片段
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="code">代码片段</param>
        public ScriptCommand(int line, int indent, RunnablePackage parent, string code)
            : base(line, indent, parent)
        {
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("context", new KeyValuePair<ArgType, string>(ArgType.unknown, code)));
            base.Init(hal, ActionPackageType.script);
        }
    }
}
