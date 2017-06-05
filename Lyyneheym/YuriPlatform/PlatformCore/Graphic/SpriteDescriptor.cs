using System;
using System.Windows;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic
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
        /// 获取或设置3D精灵所在的立绘槽号
        /// </summary>
        public int Slot3D { get; set; }

        /// <summary>
        /// 获取或设置3D精灵距离镜头的深度Z坐标
        /// </summary>
        public double Deepth3D { get; set; }

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
        /// 获取或设置精灵的模糊半径
        /// </summary>
        public double BlurRadius { get; set; } = 0;

        /// <summary>
        /// 获取或设置精灵的投影半径
        /// </summary>
        public double ShadowRadius { get; set; } = 0;

        /// <summary>
        /// 获取或设置精灵在动画结束后的X坐标
        /// </summary>
        public double ToX { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的Y坐标
        /// </summary>
        public double ToY { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的Z坐标
        /// </summary>
        public int ToZ { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的角度
        /// </summary>
        public double ToAngle { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的不透明度
        /// </summary>
        public double ToOpacity { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的X缩放
        /// </summary>
        public double ToScaleX { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的Y缩放
        /// </summary>
        public double ToScaleY { get; set; }

        /// <summary>
        /// 获取或设置精灵在动画结束后的模糊半径
        /// </summary>
        public double ToBlurRadius { get; set; } = 0;

        /// <summary>
        /// 获取或设置精灵在动画结束后的投影半径
        /// </summary>
        public double ToShadowRadius { get; set; } = 0;

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
