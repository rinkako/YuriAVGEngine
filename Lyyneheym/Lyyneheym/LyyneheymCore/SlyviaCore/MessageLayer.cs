using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            this.Visibility = System.Windows.Visibility.Visible;
            this.Padding = GlobalDataContainer.GAME_MESSAGELAYER_PADDING;
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
                return this.displayBinding.Text;
            }
            set
            {
                this.displayBinding.Text = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层是否可见
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return this.displayBinding.Visibility;
            }
            set
            {
                this.displayBinding.Visibility = value;
            }
        }

        /// <summary>
        /// 设置文字层字体
        /// </summary>
        public string FontName
        {
            set
            {
                this.displayBinding.FontFamily = new FontFamily(value);
            }
        }

        /// <summary>
        /// 获取或设置文字层字号
        /// </summary>
        public double FontSize
        {
            get
            {
                this.displayBinding.FontStyle = new FontStyle();
                return this.displayBinding.FontSize;
            }
            set
            {
                this.displayBinding.FontSize = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层的纯色颜色
        /// </summary>
        public Color FontColor
        {
            get
            {
                return ((SolidColorBrush)this.displayBinding.Foreground).Color;
            }
            set
            {
                this.displayBinding.Foreground = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// 获取或设置行距
        /// </summary>
        public double LineHeight
        {
            get
            {
                return this.displayBinding.LineHeight;
            }
            set
            {
                this.displayBinding.LineHeight = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层透明度
        /// </summary>
        public double Opacity
        {
            get
            {
                return this.displayBinding.Opacity;
            }
            set
            {
                this.displayBinding.Opacity = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层横坐标
        /// </summary>
        public double X
        {
            get
            {
                return Canvas.GetLeft(this.displayBinding);
            }
            set
            {
                Canvas.SetLeft(this.displayBinding, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层纵坐标
        /// </summary>
        public double Y
        {
            get
            {
                return Canvas.GetTop(this.displayBinding);
            }
            set
            {
                Canvas.SetTop(this.displayBinding, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层深度坐标
        /// </summary>
        public int Z
        {
            get
            {
                return Panel.GetZIndex(this.displayBinding);
            }
            set
            {
                Panel.SetZIndex(this.displayBinding, value);
            }
        }

        /// <summary>
        /// 获取或设置文字层高度
        /// </summary>
        public double Height
        {
            get
            {
                return this.displayBinding.Height;
            }
            set
            {
                this.displayBinding.Height = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层宽度
        /// </summary>
        public double Width
        {
            get
            {
                return this.displayBinding.Width;
            }
            set
            {
                this.displayBinding.Width = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层边距
        /// </summary>
        public Thickness Padding
        {
            get
            {
                return this.displayBinding.Padding;
            }
            set
            {
                this.displayBinding.Padding = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层水平对齐属性
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.displayBinding.HorizontalAlignment;
            }
            set
            {
                this.displayBinding.HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层竖直对齐属性
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return this.displayBinding.VerticalAlignment;
            }
            set
            {
                this.displayBinding.VerticalAlignment = value;
            }
        }

        /// <summary>
        /// 获取或设置文字层的阴影状态
        /// </summary>
        public bool FontShadow
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置文字层的主文本块
        /// </summary>
        public TextBlock displayBinding
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置文字层的背景精灵
        /// </summary>
        public MySprite backgroundSprite
        {
            get
            {
                return this.bgSprite;
            }
            set
            {
                this.bgSprite = value;
            }
        }

        /// <summary>
        /// 文字层背景精灵
        /// </summary>
        private MySprite bgSprite = null;
    }
}
