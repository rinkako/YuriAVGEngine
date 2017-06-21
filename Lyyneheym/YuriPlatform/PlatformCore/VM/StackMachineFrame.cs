using System;
using System.Collections.Generic;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 游戏栈机中的栈帧
    /// </summary>
    [Serializable]
    internal sealed class StackMachineFrame
    {
        /// <summary>
        /// 执行一个微步，指针前移一位
        /// </summary>
        /// <returns>移动结束后IP指向的动作</returns>
        public SceneAction MircoStep()
        {
            if (this.IP == null)
            {
                return null;
            }
            this.PC++;
            this.IR = (++this.IP).NodeName;
            return this.IP;
        }

        /// <summary>
        /// 执行一个微步，指针前移到目标位置
        /// </summary>
        /// <returns>移动结束后IP指向的动作</returns>
        public SceneAction MircoStep(SceneAction destination)
        {
            this.PC++;
            this.IR = destination == null ? String.Empty : destination.NodeName;
            return this.IP = destination;
        }

        /// <summary>
        /// 执行一个宏步，指针移动到目标节点的基即basePtr的下一节点
        /// </summary>
        /// <param name="basePtr">目标节点的基</param>
        /// <returns>移动结束后IP指向的动作</returns>
        public SceneAction MacroStep(SceneAction basePtr)
        {
            this.PC++;
            this.IR = basePtr.Next == null ? String.Empty : basePtr.Next.NodeName;
            return this.IP = basePtr.Next;
        }
        
        /// <summary>
        /// 获取或设置下一指令指针。
        /// 它是一个运行时缓存，在保存游戏时被悬空
        /// </summary>
        public SceneAction IP { get; set; } = null;

        /// <summary>
        /// 获取或设置下一指令的索引
        /// </summary>
        public string IR { get; set; } = String.Empty;

        /// <summary>
        /// 获取或设置指令计数器
        /// </summary>
        public int PC { get; set; } = 0;

        /// <summary>
        /// 获取或设置栈帧状态
        /// </summary>
        public StackMachineState State { get; set; } = StackMachineState.NOP;
        
        /// <summary>
        /// 获取或设置正在执行的脚本名（场景名、函数名）
        /// </summary>
        public string ScriptName { get; set; } = null;

        /// <summary>
        /// 获取或设置绑定的场景名称
        /// </summary>
        public string BindingSceneName { get; set; } = null;

        /// <summary>
        /// 获取或设置绑定的函数调用名称
        /// </summary>
        public string BindingFunctionName { get; set; } = null;

        /// <summary>
        /// 获取或设置实参数列表
        /// </summary>
        public List<object> Argv { get; set; } = null;

        /// <summary>
        /// 获取或设置执行栈帧前的延迟
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 获取或设置时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 获取或设置绑定的中断动作
        /// </summary>
        public Interrupt BindingInterrupt { get; set; } = null;

        /// <summary>
        /// 获取或设置绑定的函数实例
        /// </summary>
        public SceneFunction BindingFunction { get; set; } = null;

        /// <summary>
        /// 获取或设置该栈帧的备注信息
        /// </summary>
        public string Tag { get; set; } = null;

        /// <summary>
        /// 字符串化方法
        /// </summary>
        public override string ToString()
        {
            return String.Format("StackFrame:{0} -> {1}", this.State.ToString(), IP?.ToString() ?? String.Empty);
        }
    }

    /// <summary>
    /// 枚举：游戏栈机的状态
    /// </summary>
    internal enum StackMachineState
    {
        /// <summary>
        /// 无动作
        /// </summary>
        NOP,
        /// <summary>
        /// 执行场景脚本
        /// </summary>
        Interpreting,
        /// <summary>
        /// 等待用户操作
        /// </summary>
        WaitUser,
        /// <summary>
        /// 等待指令
        /// </summary>
        Await,
        /// <summary>
        /// 等待动画指令
        /// </summary>
        WaitAnimation,
        /// <summary>
        /// 函数调用
        /// </summary>
        FunctionCalling,
        /// <summary>
        /// 系统中断
        /// </summary>
        Interrupt,
        /// <summary>
        /// 启动播放等待
        /// </summary>
        AutoWait
    }
}
