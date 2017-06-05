using System;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic3D
{
    /// <summary>
    /// 3D模型描述子类
    /// </summary>
    [Serializable]
    internal sealed class ModelDescriptor3D : CloneableDescriptor
    {
        /// <summary>
        /// 获取或设置槽id
        /// </summary>
        public int SlotId { get; set; } = 0;

        /// <summary>
        /// 获取或设置资源名
        /// </summary>
        public string Source { get; set; } = null;

        /// <summary>
        /// 获取或设置X方向累计变化量
        /// </summary>
        public double OffsetX { get; set; } = 0;

        /// <summary>
        /// 获取或设置动画结束之后X方向的累计变化量
        /// </summary>
        public double ToOffsetX { get; set; } = 0;

        /// <summary>
        /// 获取或设置Y方向累计变化量
        /// </summary>
        public double OffsetY { get; set; } = 0;

        /// <summary>
        /// 获取或设置动画结束之后Y方向的累计变化量
        /// </summary>
        public double ToOffsetY { get; set; } = 0;

        /// <summary>
        /// 获取或设置Z方向累计变化量
        /// </summary>
        public double OffsetZ { get; set; } = 0;

        /// <summary>
        /// 获取或设置动画结束之后Z方向的累计变化量
        /// </summary>
        public double ToOffsetZ { get; set; } = 0;

        /// <summary>
        /// 获取或设置不透明度
        /// </summary>
        public double Opacity { get; set; } = 1;

        /// <summary>
        /// 获取或设置动画结束之后不透明度
        /// </summary>
        public double ToOpacity { get; set; } = 1;

        /// <summary>
        /// 获取或设置角度
        /// </summary>
        public double Angle { get; set; } = 0;

        /// <summary>
        /// 获取或设置动画结束之后角度
        /// </summary>
        public double ToAngle { get; set; } = 0;

        /// <summary>
        /// 获取或设置横向缩放比
        /// </summary>
        public double ScaleX { get; set; } = 1;

        /// <summary>
        /// 获取或设置动画结束之后
        /// </summary>
        public double ToScaleX { get; set; } = 1;

        /// <summary>
        /// 获取或设置纵向缩放比
        /// </summary>
        public double ScaleY { get; set; } = 1;

        /// <summary>
        /// 获取或设置动画结束之后
        /// </summary>
        public double ToScaleY { get; set; } = 1;
    }
}
