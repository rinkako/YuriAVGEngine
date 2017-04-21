using System.Collections.Generic;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// IDE命令类：删除
    /// </summary>
    internal class MenuDeleteCommand : IHalationCommand
    {
        /// <summary>
        /// 删除代码向量里的代码
        /// </summary>
        /// <param name="parent">所属包装</param>
        /// <param name="begin">开始删除的行</param>
        /// <param name="count">受影响的行数</param>
        public MenuDeleteCommand(RunnablePackage parent, int begin, int count)
        {
            this.parent = parent;
            this.begin = begin;
            this.count = count;
        }

        /// <summary>
        /// 重载执行命令
        /// </summary>
        public void Dash()
        {
            for (int i = begin; i < this.begin + this.count; i++)
            {
                this.deleteBuffer.Add(this.parent.GetAction()[i]);
            }
            for (int i = 0; i < this.count; i++)
            {
                this.parent.DeleteAction(this.begin);
            }
        }

        /// <summary>
        /// 重载撤销命令
        /// </summary>
        public void Undo()
        {
            for (int i = 0; i < this.count; i++)
            {
                this.parent.AddAction(this.deleteBuffer[i], this.begin + i);
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
        /// 删除的行数
        /// </summary>
        private int count;

        /// <summary>
        /// 被删除的项目
        /// </summary>
        private List<ActionPackage> deleteBuffer = new List<ActionPackage>();
    }
}
