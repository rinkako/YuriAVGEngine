using System;
using System.Collections.Generic;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 游戏栈机中的栈帧
    /// </summary>
    [Serializable]
    internal class StackMachineFrame
    {
        /// <summary>
        /// 获取或设置栈帧状态
        /// </summary>
        public StackMachineState State = StackMachineState.NOP;

        /// <summary>
        /// 获取或设置指令计数器
        /// </summary>
        public int PC = 0;

        /// <summary>
        /// 获取或设置下一指令指针
        /// </summary>
        public SceneAction IP = null;
        
        /// <summary>
        /// 获取或设置正在执行的脚本名（场景名、函数名）
        /// </summary>
        public string ScriptName = null;

        /// <summary>
        /// 获取或设置绑定的场景名称
        /// </summary>
        public string BindingSceneName = null;

        /// <summary>
        /// 获取或设置绑定的函数调用名称
        /// </summary>
        public string BindingFunctionName = null;

        /// <summary>
        /// 获取或设置实参数列表
        /// </summary>
        public List<object> Argv = null;

        /// <summary>
        /// 获取或设置执行栈帧前的延迟
        /// </summary>
        public TimeSpan Delay = TimeSpan.Zero;

        /// <summary>
        /// 获取或设置时间戳
        /// </summary>
        public DateTime TimeStamp = DateTime.Now;

        /// <summary>
        /// 获取或设置绑定的中断动作
        /// </summary>
        public Interrupt BindingInterrupt = null;

        /// <summary>
        /// 获取或设置该栈帧的备注信息
        /// </summary>
        public string Tag = null;

        /// <summary>
        /// 字符串化方法
        /// </summary>
        public override string ToString()
        {
            return String.Format("StackFrame:{0} -> {1}", this.State.ToString(), this.IP == null ? "" : this.IP.ToString());
        }
    }

    /// <summary>
    /// 枚举：游戏栈机的状态
    /// </summary>
    internal enum StackMachineState
    {
        // 无动作
        NOP,
        // 执行场景脚本
        Interpreting,
        // 等待用户操作
        WaitUser,
        // 等待指令
        Await,
        // 等待动画指令
        WaitAnimation,
        // 函数调用
        FunctionCalling,
        // 系统中断
        Interrupt
    }
}
