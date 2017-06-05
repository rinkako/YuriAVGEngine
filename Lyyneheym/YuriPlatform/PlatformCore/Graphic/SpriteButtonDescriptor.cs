using System;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 精灵按钮描述类
    /// </summary>
    [Serializable]
    internal class SpriteButtonDescriptor : CloneableDescriptor
    {
        /// <summary>
        /// 构造一个按钮描述子
        /// </summary>
        public SpriteButtonDescriptor()
        {
            this.NormalDescriptor = this.OverDescriptor = this.OnDescriptor = null;
            this.JumpLabel = String.Empty;
            this.X = this.Y = 0;
            this.Z = GlobalConfigContext.GAME_Z_BUTTON;
            this.Opacity = 1;
            this.Enable = true;
        }

        /// <summary>
        /// 获取或设置按钮正常态精灵描述子
        /// </summary>
        public SpriteDescriptor NormalDescriptor { get; set; }

        /// <summary>
        /// 获取或设置按钮悬停态精灵描述子
        /// </summary>
        public SpriteDescriptor OverDescriptor { get; set; }

        /// <summary>
        /// 获取或设置按钮按下态精灵描述子
        /// </summary>
        public SpriteDescriptor OnDescriptor { get; set; }
        
        /// <summary>
        /// 获取或设置按钮跳转标签
        /// </summary>
        public string JumpLabel { get; set; }

        /// <summary>
        /// 获取或设置按钮中断调用函数
        /// </summary>
        public string InterruptFuncSign { get; set; }

        /// <summary>
        /// 获取或设置按钮X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 获取或设置按钮Y坐标
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// 获取或设置按钮Z坐标
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 获取或设置不透明度
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// 获取或设置按钮是否可以点击
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 获取或设置按钮是否可以在被点击后仍留存在屏幕上
        /// </summary>
        public bool Eternal { get; set; }
    }
}
