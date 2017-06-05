using System;
using System.Windows;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 文字层描述类：用于画面管理和保存
    /// </summary>
    [Serializable]
    internal class MessageLayerDescriptor : CloneableDescriptor
    {
        /// <summary>
        /// 获取或设置文本层id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置文本层上的文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置文本层可见性
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 获取或设置文本层字体名称
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 获取或设置文本层字号
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 获取或设置文本层字色R通道值
        /// </summary>
        public byte FontColorR { get; set; }

        /// <summary>
        /// 获取或设置文本层字色G通道值
        /// </summary>
        public byte FontColorG { get; set; }

        /// <summary>
        /// 获取或设置文本层字色B通道值
        /// </summary>
        public byte FontColorB { get; set; }

        /// <summary>
        /// 获取或设置文本层字体投影与否
        /// </summary>
        public bool FontShadow { get; set; }

        /// <summary>
        /// 获取或设置文本层行距
        /// </summary>
        public double LineHeight { get; set; }

        /// <summary>
        /// 获取或设置文本层不透明度
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// 获取或设置文本层X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 获取或设置文本层Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 获取或设置文本层Z坐标
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 获取或设置文本层高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 获取或设置文本层宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取或设置文本层文字与层的边距
        /// </summary>
        public MyThickness Padding { get; set; }

        /// <summary>
        /// 获取或设置文本层文字横向对齐属性
        /// </summary>
        public TextAlignment TextHorizonAlign { get; set; }

        /// <summary>
        /// 获取或设置文本层在视窗中横向对齐属性
        /// </summary>
        public HorizontalAlignment HorizonAlign { get; set; }

        /// <summary>
        /// 获取或设置文本层在视窗中纵向对齐属性
        /// </summary>
        public VerticalAlignment VertiAlign { get; set; }

        /// <summary>
        /// 获取或设置文本层背景图资源名称
        /// </summary>
        public string BackgroundResourceName { get; set; }
    }

    /// <summary>
    /// 可序列化厚度结构体
    /// </summary>
    [Serializable]
    public struct MyThickness
    {
        /// <summary>
        /// 矩形的左边厚度
        /// </summary>
        public double Left;

        /// <summary>
        /// 矩形的上边厚度
        /// </summary>
        public double Top;

        /// <summary>
        /// 矩形的右边厚度
        /// </summary>
        public double Right;

        /// <summary>
        /// 矩形的下边厚度
        /// </summary>
        public double Bottom;

        /// <summary>
        /// 构造一个可序列化的矩形
        /// </summary>
        /// <param name="left">左边厚度</param>
        /// <param name="top">上边厚度</param>
        /// <param name="right">右边厚度</param>
        /// <param name="bottom">下边厚度</param>
        public MyThickness(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        /// <summary>
        /// 从系统厚度构造一个可序列化厚度矩形
        /// </summary>
        /// <param name="t">系统厚度类</param>
        public MyThickness(Thickness t)
        {
            this.Left = t.Left;
            this.Top = t.Top;
            this.Right = t.Right;
            this.Bottom = t.Bottom;
        }

        /// <summary>
        /// 从可序列化厚度矩形构造一个系统厚度
        /// </summary>
        /// <param name="mt">可序列化厚度实例</param>
        /// <returns>系统厚度实例</returns>
        public static explicit operator Thickness(MyThickness mt)
        {
            return new Thickness(mt.Left, mt.Top, mt.Right, mt.Bottom);
        }
    }

    /// <summary>
    /// 文本展示类型
    /// </summary>
    internal enum MessageLayerType
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Disposed,
        /// <summary>
        /// 对话框
        /// </summary>
        Dialog,
        /// <summary>
        /// 全屏文本
        /// </summary>
        Novel,
        /// <summary>
        /// 对话气泡
        /// </summary>
        Bubble,
        /// <summary>
        /// 全透明
        /// </summary>
        Transparent
    }
}
