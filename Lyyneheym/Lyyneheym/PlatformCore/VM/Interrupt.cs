using System;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 系统中断类
    /// </summary>
    [Serializable]
    internal sealed class Interrupt
    {
        /// <summary>
        /// 中断类型
        /// </summary>
        public InterruptType Type = InterruptType.NOP;

        /// <summary>
        /// 中断描述
        /// </summary>
        public string Detail = String.Empty;

        /// <summary>
        /// <para>中断后执行的中断处理动作</para>
        /// <para>它在中断发生后最优先被执行，但它的后继结点将被忽略</para>
        /// </summary>
        public SceneAction InterruptSA = null;

        /// <summary>
        /// <para>中断处理函数调用签名</para>
        /// <para>这个动作将在处理完中断动作后被施加到调用堆栈</para>
        /// </summary>
        public string InterruptFuncSign = String.Empty;

        /// <summary>
        /// <para>中断结束后跳转的标签名</para>
        /// <para>这个动作将在处理完中断函数调用后被施加到调用堆栈</para>
        /// </summary>
        public string ReturnTarget = null;

        /// <summary>
        /// 在执行完中断动作后是否处理后续动作
        /// </summary>
        public bool PureInterrupt = false;

        /// <summary>
        /// 是否在执行时弹空所有等待
        /// </summary>
        public bool ExitWait = false;
    }

    /// <summary>
    /// 枚举：系统中断的类型
    /// </summary>
    public enum InterruptType
    {
        /// <summary>
        /// 空中断
        /// </summary>
        NOP,
        /// <summary>
        /// 按钮触发的跳转
        /// </summary>
        ButtonJump,
        /// <summary>
        /// 读取保存数据后重现动作
        /// </summary>
        LoadReaction,
        /// <summary>
        /// 菜单调用
        /// </summary>
        MenuCalling
    }
}
