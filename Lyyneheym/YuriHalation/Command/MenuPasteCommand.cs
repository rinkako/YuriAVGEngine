using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// IDE命令类：粘贴
    /// </summary>
    internal class MenuPasteCommand : IHalationCommand
    {
        /// <summary>
        /// 粘贴代码向量里的代码
        /// </summary>
        /// <param name="parent">所属包装</param>
        /// <param name="begin">插入行</param>
        /// <param name="copys">待复制项目</param>
        public MenuPasteCommand(RunnablePackage parent, int begin, List<ActionPackage> copys)
        {
            this.parent = parent;
            this.begin = begin;
            this.copyAPRef = new List<ActionPackage>();
            // 深拷贝
            foreach (var ap in copys)
            {
                this.copyAPRef.Add(ap.Clone());
            }
            // 处理缩进
            this.baseIndentOld = this.copyAPRef[0].indent;
            this.baseIndentNew = this.parent.GetAction(this.begin).indent;
            foreach (var ap in this.copyAPRef)
            {
                ap.indent -= this.baseIndentOld;
            }
        }

        /// <summary>
        /// 重载执行命令
        /// </summary>
        public void Dash()
        {
            for (int i = 0; i < this.copyAPRef.Count; i++)
            {
                this.copyAPRef[i].indent = this.copyAPRef[i].indent + this.baseIndentNew;
                this.parent.AddAction(this.copyAPRef[i], this.begin + i);
            }
        }

        /// <summary>
        /// 重载撤销命令
        /// </summary>
        public void Undo()
        {
            for (int i = 0; i < this.copyAPRef.Count; i++)
            {
                this.parent.DeleteAction(this.begin);
            }
        }

        /// <summary>
        /// 所属代码包装
        /// </summary>
        private RunnablePackage parent;

        /// <summary>
        /// 开始删除的行号
        /// </summary>
        private int begin;

        /// <summary>
        /// 旧缩进基
        /// </summary>
        private int baseIndentOld;

        /// <summary>
        /// 新缩进基
        /// </summary>
        private int baseIndentNew;

        /// <summary>
        /// 拷贝项目向量的引用
        /// </summary>
        private List<ActionPackage> copyAPRef;
    }
}
