using System;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 可视化2D视窗描述子
    /// </summary>
    [Serializable]
    internal sealed class Viewport2DDescriptor : CloneableDescriptor
    {
        /// <summary>
        /// 获取或设置视窗类型
        /// </summary>
        public ViewportType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗相对系统窗口左边界的距离
        /// </summary>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗相对系统窗口上边界的距离
        /// </summary>
        public double Top
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗在系统窗口上的深度
        /// </summary>
        public int ZIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗横向缩放比
        /// </summary>
        public double ScaleX { get; set; } = 1.0;

        /// <summary>
        /// 获取或设置视窗纵向缩放比
        /// </summary>
        public double ScaleY { get; set; } = 1.0;

        /// <summary>
        /// 获取或设置视窗动画X锚点
        /// </summary>
        public double AnchorX
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗动画Y锚点
        /// </summary>
        public double AnchorY
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗旋转角度
        /// </summary>
        public double Angle
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 枚举：视窗类型
    /// </summary>
    [Serializable]
    internal enum ViewportType
    {
        /// <summary>
        /// 背景视窗
        /// </summary>
        VTBackground = 0,
        /// <summary>
        /// 立绘视窗
        /// </summary>
        VTCharacterStand = 1,
        /// <summary>
        /// 前景贴图视窗
        /// </summary>
        VTPictures = 2,
        /// <summary>
        /// 文字层视窗
        /// </summary>
        VTMessage = 3
    }
}
