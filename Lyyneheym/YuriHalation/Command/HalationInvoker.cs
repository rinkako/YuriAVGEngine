using System.Collections.Generic;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// 命令执行器
    /// </summary>
    internal static class HalationInvoker
    {
        /// <summary>
        /// 执行指定的命令
        /// </summary>
        /// <param name="key">当前代码</param>
        /// <param name="command">命令实例</param>
        public static void Dash(string key, IHalationCommand command)
        {
            command.Dash();
            HalationInvoker.commandStackDict[key].Push(command);
            Halation.GetInstance().RefreshRedoUndo();
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        /// <param name="key">当前代码</param>
        public static void Undo(string key)
        {
            if (HalationInvoker.commandStackDict[key].Count > 0)
            {
                IHalationCommand command = HalationInvoker.commandStackDict[key].Pop();
                HalationInvoker.redoStackDict[key].Push(command);
                command.Undo();
            }
            Halation.GetInstance().RefreshRedoUndo();
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        /// <param name="key">当前代码</param>
        public static void Redo(string key)
        {
            if (HalationInvoker.redoStackDict[key].Count > 0)
            {
                IHalationCommand command = HalationInvoker.redoStackDict[key].Pop();
                command.Dash();
                HalationInvoker.commandStackDict[key].Push(command);
            }
            Halation.GetInstance().RefreshRedoUndo();
        }

        /// <summary>
        /// 当前是否可以撤销
        /// </summary>
        /// <param name="key">当前代码</param>
        public static bool IsAbleUndo(string key)
        {
            return HalationInvoker.commandStackDict[key].Count > 0;
        }

        /// <summary>
        /// 当前是否可以重做
        /// </summary>
        /// <param name="key">当前代码</param>
        public static bool IsAbleRedo(string key)
        {
            return HalationInvoker.redoStackDict[key].Count > 0;
        }

        /// <summary>
        /// 清空命令栈
        /// </summary>
        public static void Clear()
        {
            foreach (var st in HalationInvoker.commandStackDict)
            {
                st.Value.Clear();
            }
            foreach (var st in HalationInvoker.redoStackDict)
            {
                st.Value.Clear();
            }
        }

        /// <summary>
        /// 为栈添加场景
        /// </summary>
        /// <param name="scene">场景名/函数调用名</param>
        public static void AddScene(string scene)
        {
            HalationInvoker.commandStackDict[scene] = new Stack<IHalationCommand>();
            HalationInvoker.redoStackDict[scene] = new Stack<IHalationCommand>();
        }

        /// <summary>
        /// 为栈移除场景
        /// </summary>
        /// <param name="scene">场景名/函数调用名</param>
        public static void RemoveScene(string scene)
        {
            if (HalationInvoker.commandStackDict.ContainsKey(scene))
            {
                HalationInvoker.commandStackDict.Remove(scene);
            }
            if (HalationInvoker.redoStackDict.ContainsKey(scene))
            {
                HalationInvoker.redoStackDict.Remove(scene);
            }
        }

        /// <summary>
        /// 命令栈
        /// </summary>
        private static Dictionary<string, Stack<IHalationCommand>> commandStackDict = new Dictionary<string, Stack<IHalationCommand>>();

        /// <summary>
        /// 重做命令栈
        /// </summary>
        private static Dictionary<string, Stack<IHalationCommand>> redoStackDict = new Dictionary<string, Stack<IHalationCommand>>();
    }
}
