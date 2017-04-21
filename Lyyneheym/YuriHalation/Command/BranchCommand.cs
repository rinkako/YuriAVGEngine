using System.Text;
using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    using HalaAttrList = List<KeyValuePair<string, KeyValuePair<ArgType, string>>>;

    /// <summary>
    /// 命令类：状态变更
    /// </summary>
    internal class BranchCommand : HalationSingleCommand
    {
        /// <summary>
        /// 直接描绘文本
        /// </summary>
        /// <param name="line">命令的行</param>
        /// <param name="indent">对齐偏移</param>
        /// <param name="parent">所属的可运行包装</param>
        /// <param name="items">选择项描述向量</param>
        public BranchCommand(int line, int indent, RunnablePackage parent, List<string> items)
            : base(line, indent, parent)
        {
            // 处理跳转描述列表
            StringBuilder sb = new StringBuilder();
            foreach (var s in items)
            {
                sb.Append(';');
                sb.Append(s);
            }
            HalaAttrList hal = new HalaAttrList();
            hal.Add(new KeyValuePair<string, KeyValuePair<ArgType, string>>("link", new KeyValuePair<ArgType, string>(ArgType.Arg_link, sb.ToString().Substring(1))));
            base.Init(hal, ActionPackageType.act_branch);
        }
    }
}
