using System;
using System.Collections.Generic;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// 命令执行器
    /// </summary>
    static class HalationInvoker
    {
        /// <summary>
        /// 执行指定的命令
        /// </summary>
        /// <param name="command">命令实例</param>
        public static void Dash(IHalationCommand command)
        {
            command.Dash();
            HalationInvoker.commandStack.Push(command);
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public static void Undo()
        {
            if (HalationInvoker.commandStack.Count > 0)
            {
                IHalationCommand command = HalationInvoker.commandStack.Pop();
                HalationInvoker.redoStack.Push(command);
                command.Undo();
            }
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        public static void Redo()
        {
            if (HalationInvoker.redoStack.Count > 0)
            {
                IHalationCommand command = HalationInvoker.redoStack.Pop();
                command.Dash();
                HalationInvoker.commandStack.Push(command);
            }
        }

        /// <summary>
        /// 当前是否可以撤销
        /// </summary>
        public static bool IsAbleUndo()
        {
            return HalationInvoker.commandStack.Count > 0;
        }

        /// <summary>
        /// 当前是否可以重做
        /// </summary>
        public static bool IsAbleRedo()
        {
            return HalationInvoker.redoStack.Count > 0;
        }

        /// <summary>
        /// 清空命令栈
        /// </summary>
        public static void Clear()
        {
            HalationInvoker.commandStack.Clear();
        }

        /// <summary>
        /// 命令栈
        /// </summary>
        private static Stack<IHalationCommand> commandStack = new Stack<IHalationCommand>();

        /// <summary>
        /// 重做命令栈
        /// </summary>
        private static Stack<IHalationCommand> redoStack = new Stack<IHalationCommand>();
    }
}
