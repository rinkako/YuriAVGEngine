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
        /// 文字层的主文本块
        /// </summary>
        public TextBlock textBlock
        {
            get;
            set;
        }
    }
}
