using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 文字层类：为游戏提供在某个区域描绘文字的服务
    /// </summary>
    public class MessageLayer
    {
        /// <summary>
        /// 将文字层恢复初始状态
        /// </summary>
        public void Reset()
        {
            this.Text = "";
            this.X = this.Y = 0;
            this.Opacity = 1.0f;
            this.Visible = true;
            this.Margin = new Thickness(GlobalDataContainer.GAME_MESSAGELAYER_MARGIN_LEFT, GlobalDataContainer.GAME_MESSAGELAYER_MARGIN_TOP, GlobalDataContainer.GAME_MESSAGELAYER_MARGIN_RIGHT, GlobalDataContainer.GAME_MESSAGELAYER_MARGIN_BOTTOM);
            this.StyleReset();
        }

        /// <summary>
        /// 恢复默认文字层的样式
        /// </summary>
        public void StyleReset()
        {
            this.FontColor = GlobalDataContainer.GAME_FONT_COLOR;
            this.FontSize = GlobalDataContainer.GAME_FONT_FONTSIZE;
            this.FontName = GlobalDataContainer.GAME_FONT_NAME;
            this.LineHeight = GlobalDataContainer.GAME_FONT_LINEHEIGHT;
        }

        /// <summary>
        /// 获取或设置文字层的文本
        /// </summary>
        public string Text
        {
            get
            {
                return this.textBlock.Text;
            }
            set
            {
                this.textBlock.Text = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.textBlock.Visibility == Visibility.Visible;
            }
            set
            {
                this.textBlock.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// 设置文字层字体
        /// </summary>
        public string FontName
        {
            set
            {
                this.textBlock.FontFamily = new FontFamily(value);
            }
        }

        /// <summary>
        /// 获取或设置文字层字号
        /// </summary>
        public double FontSize
        {
            get
            {
                this.textBlock.FontStyle = new FontStyle();
                return this.textBlock.FontSize;
            }
            set
            {
                this.textBlock.FontSize = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层的纯色颜色
        /// </summary>
        public Color FontColor
        {
            get
            {
                return ((SolidColorBrush)this.textBlock.Foreground).Color;
            }
            set
            {
                this.textBlock.Foreground = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// 获取或设置行距
        /// </summary>
        public double LineHeight
        {
            get
            {
                return this.textBlock.LineHeight;
            }
            set
            {
                this.textBlock.LineHeight = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层透明度
        /// </summary>
        public double Opacity
        {
            get
            {
                return this.textBlock.Opacity;
            }
            set
            {
                this.textBlock.Opacity = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层横坐标
        /// </summary>
        public double X
        {
            get
            {
                return Canvas.GetLeft(this.textBlock);
            }
            set
            {
                Canvas.SetLeft(this.textBlock, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层纵坐标
        /// </summary>
        public double Y
        {
            get
            {
                return Canvas.GetTop(this.textBlock);
            }
            set
            {
                Canvas.SetTop(this.textBlock, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层深度坐标
        /// </summary>
        public int Z
        {
            get
            {
                return Panel.GetZIndex(this.textBlock);
            }
            set
            {
                Panel.SetZIndex(this.textBlock, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层宽度
        /// </summary>
        public double Height
        {
            get
            {
                return this.textBlock.Height;
            }
            set
            {
                this.textBlock.Height = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层高度
        /// </summary>
        public double Width
        {
            get
            {
                return this.textBlock.Width;
            }
            set
            {
                this.textBlock.Width = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层边距
        /// </summary>
        public Thickness Margin
        {
            get
            {
                return this.textBlock.Margin;
            }
            set
            {
                this.textBlock.Margin = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层水平对齐属性
        /// </summary>
        public string HorizontalAlignment
        {
            get
            {
                return this.textBlock.HorizontalAlignment.ToString();
            }
            set
            {
                this.textBlock.HorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), value, false);
            }
        }

        /// <summary>
        /// 获取或设置文字层竖直对齐属性
        /// </summary>
        public string VerticalAlignment
        {
            get
            {
                return this.textBlock.VerticalAlignment.ToString();
            }
            set
            {
                this.textBlock.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), value);
            }
        }

        /// <summary>
        /// 文字层的主文本块
        /// </summary>
        public TextBlock textBlock
        {
            get;
            set;
        }
    }
}
