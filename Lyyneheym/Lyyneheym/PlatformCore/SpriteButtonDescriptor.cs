using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 精灵按钮描述类
    /// </summary>
    [Serializable]
    public class SpriteButtonDescriptor
    {
        /// <summary>
        /// 构造一个按钮描述子
        /// </summary>
        public SpriteButtonDescriptor()
        {
            this.normalDescriptor = this.overDescriptor = this.onDescriptor = null;
            this.jumpLabel = String.Empty;
            this.X = this.Y = 0;
            this.Z = GlobalDataContainer.GAME_Z_BUTTON;
            this.Opacity = 1;
            this.Enable = true;
        }

        /// <summary>
        /// 获取或设置按钮正常态精灵描述子
        /// </summary>
        public SpriteDescriptor normalDescriptor { get; set; }

        /// <summary>
        /// 获取或设置按钮悬停态精灵描述子
        /// </summary>
        public SpriteDescriptor overDescriptor { get; set; }

        /// <summary>
        /// 获取或设置按钮按下态精灵描述子
        /// </summary>
        public SpriteDescriptor onDescriptor { get; set; }
        
        /// <summary>
        /// 获取或设置按钮跳转标签
        /// </summary>
        public string jumpLabel { get; set; }

        /// <summary>
        /// 获取或设置按钮终端调用函数
        /// </summary>
        public string interruptFuncSign { get; set; }

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
