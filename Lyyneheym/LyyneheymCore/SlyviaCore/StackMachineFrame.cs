using System;
using System.Collections.Generic;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 游戏栈机中的栈帧
    /// </summary>
    public class StackMachineFrame
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public GameStackMachineState state
        {
            get;
            set;
        }

        /// <summary>
        /// 指令计数器
        /// </summary>
        public int PC
        {
            get { return this.PC; }
            set { this.PC = value >= 0 ? value : 0; }
        }

        /// <summary>
        /// 正在执行的脚本名（场景名、函数名）
        /// </summary>
        public string scriptName
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定的场景
        /// </summary>
        public Scene bindingScene
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定的函数调用
        /// </summary>
        public SceneFunction bindingFunction
        {
            get;
            set;
        }

        /// <summary>
        /// 实参数列表
        /// </summary>
        public List<object> argv
        {
            get;
            set;
        }

        /// <summary>
        /// 执行栈帧前的延迟
        /// </summary>
        public TimeSpan delay
        {
            get;
            set;
        }

        /// <summary>
        /// 该栈帧的备注信息
        /// </summary>
        public string aTag
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 游戏栈机的状态
    /// </summary>
    public enum GameStackMachineState
    {
        // 无动作
        NOP,
        // 执行场景脚本
        Interpreting,
        // 等待用户操作
        WaitUser,
        // 等待指令
        Await,
        // 函数调用
        Calling,
        // 系统中断
        Interrupt
    }
}
