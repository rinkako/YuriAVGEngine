using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 选择项按钮描述类
    /// </summary>
    [Serializable]
    public class BranchButtonDescriptor
    {
        /// <summary>
        /// 获取或设置选择项按钮id号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置选择项跳转目标
        /// </summary>
        public string JumpTarget { get; set; }

        /// <summary>
        /// 获取或设置选择项上的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮Z坐标
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮正常态描述子
        /// </summary>
        public SpriteDescriptor normalDescriptor { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮悬停态描述子
        /// </summary>
        public SpriteDescriptor overDescriptor { get; set; }

        /// <summary>
        /// 获取或设置选择项按钮按下态描述子
        /// </summary>
        public SpriteDescriptor onDescriptor { get; set; }
    }
}
