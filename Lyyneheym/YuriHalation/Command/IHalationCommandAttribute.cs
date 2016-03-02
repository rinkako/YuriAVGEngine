using System;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    class IHalationCommandAttribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="line">所在的行</param>
        /// <param name="indent">列偏移</param>
        /// <param name="parent">所述包装</param>
        public IHalationCommandAttribute(int line, int indent, RunnablePackage parent)
        {
            this.parent = parent;
            this.indent = indent;
            this.commandLine = line;
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
