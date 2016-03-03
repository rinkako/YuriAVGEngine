using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 文字层描述类：用于画面管理和保存
    /// </summary>
    [Serializable]
    public class MessageLayerDescriptor
    {
        public int Id { get; set; }
        
        public string Text { get; set; }

        public bool Visible { get; set; }

        public string FontName { get; set; }

        public double FontSize { get; set; }

        public byte FontColorR { get; set; }

        public byte FontColorG { get; set; }

        public byte FontColorB { get; set; }

        public bool FontShadow { get; set; }

        public double LineHeight { get; set; }

        public double Opacity { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int Z { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public MyThickness Padding { get; set; }

        public HorizontalAlignment HorizonAlign { get; set; }

        public VerticalAlignment VertiAlign { get; set; }

        public string BackgroundResourceName { get; set; }
    }

    [Serializable]
    public struct MyThickness
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        public MyThickness(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public MyThickness(Thickness t)
        {
            this.Left = t.Left;
            this.Top = t.Top;
            this.Right = t.Right;
            this.Bottom = t.Bottom;
        }

        public static explicit operator Thickness(MyThickness mt)
        {
            return new Thickness(mt.Left, mt.Top, mt.Right, mt.Bottom);
        }
    }
}
