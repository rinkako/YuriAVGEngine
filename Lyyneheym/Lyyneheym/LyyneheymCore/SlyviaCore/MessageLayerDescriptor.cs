using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 文字层描述类：用于画面管理和保存
    /// </summary>
    [Serializable]
    public class MessageLayerDescriptor
    {
        public string text { get; set; }

        public bool Visible { get; set; }

        public string FontName { get; set; }

        public double FontSize { get; set; }

        public Color FontColor { get; set; }

        public bool FontShadow { get; set; }

        public double LineHeight { get; set; }

        public double Opacity { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int Z { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public Thickness Padding { get; set; }

        public HorizontalAlignment HorizonAlign { get; set; }

        public VerticalAlignment VertiAlign { get; set; }

        public string BackgroundResourceName { get; set; }
    }
}
