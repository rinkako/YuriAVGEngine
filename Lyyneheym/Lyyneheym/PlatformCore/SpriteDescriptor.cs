using System;
using System.Collections.Generic;
using System.Windows;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 精灵描述类：用于画面管理和保存
    /// </summary>
    [Serializable]
    internal class SpriteDescriptor : CloneableDescriptor
    {
        /// <summary>
        /// 构造一个精灵
        /// </summary>
        public SpriteDescriptor()
        {
            this.Id = this.Z = 0;
            this.X = this.Y = this.Angle = 0;
            this.Opacity = this.ScaleX = this.ScaleY = 1;
            this.AnchorType = SpriteAnchorType.Center;
            this.CutRect = new Int32Rect(-1, 0, 0, 0);
            this.ResourceName = null;
            this.ResourceType = ResourceType.Unknown;
        }

        /// <summary>
        /// 获取或设置精灵id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置精灵X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 获取或设置精灵Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 获取或设置精灵Z坐标
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 获取或设置精灵角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 获取或设置精灵不透明度
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// 获取或设置精灵X缩放
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// 获取或设置精灵Y缩放
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// 获取或设置精灵锚点方式
        /// </summary>
        public SpriteAnchorType AnchorType { get; set; }

        /// <summary>
        /// 获取或设置精灵的资源类型
        /// </summary>
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// 获取或设置精灵的资源名
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 获取或设置精灵的纹理切割矩
        /// </summary>
        public Int32Rect CutRect { get; set; }
    }
}
