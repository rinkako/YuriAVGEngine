﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 系统中断类
    /// </summary>
    public class Interrupt
    {
        /// <summary>
        /// 中断类型
        /// </summary>
        public InterruptType type = InterruptType.NOP;

        /// <summary>
        /// 中断描述
        /// </summary>
        public string detail = "";

        /// <summary>
        /// 中断后执行的动作序列
        /// </summary>
        public SceneAction interruptSA = null;

        /// <summary>
        /// 中断结束后跳转的标签名
        /// </summary>
        public string returnTarget = null;
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
        /// 菜单调用
        /// </summary>
        MenuCalling,
    }
}
