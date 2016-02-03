using System;
using System.Collections.Generic;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 游戏栈机中的栈帧
    /// </summary>
    [Serializable]
    public class StackMachineFrame
    {
        /// <summary>
        /// 获取或设置栈帧状态
        /// </summary>
        public GameStackMachineState state
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置指令计数器
        /// </summary>
        public int PC
        {
            get { return this.PC; }
            set { this.PC = value >= 0 ? value : 0; }
        }

        /// <summary>
        /// 获取或设置下一指令指针
        /// </summary>
        public SceneAction IP
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取或设置正在执行的脚本名（场景名、函数名）
        /// </summary>
        public string scriptName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定的场景名称
        /// </summary>
        public string bindingSceneName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定的函数调用名称
        /// </summary>
        public string bindingFunctionName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置实参数列表
        /// </summary>
        public List<object> argv
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置执行栈帧前的延迟
        /// </summary>
        public TimeSpan delay
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置该栈帧的备注信息
        /// </summary>
        public string aTag
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 枚举：游戏栈机的状态
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
        FunctionCalling,
        // 系统中断
        Interrupt
    }
}
