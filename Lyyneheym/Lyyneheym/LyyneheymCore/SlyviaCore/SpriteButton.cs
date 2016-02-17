using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 精灵按钮类
    /// </summary>
    public class SpriteButton
    {
        public SpriteButton()
        {
            this.ntr = null;
            this.displayBinding = null;
            this.Enable = true;
            this.X = this.Y = 0;
            this.Z = GlobalDataContainer.GAME_Z_BUTTON;
            this.isMouseOn = this.isMouseOver = false;
        }

        /// <summary>
        /// 按下时的中断
        /// </summary>
        public Interrupt ntr { get; set; }

        /// <summary>
        /// 获取或设置绑定前端显示控件
        /// </summary>
        public Image displayBinding { get; set; }

        /// <summary>
        /// 获取或设置按钮是否有效
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 获取或设置按钮的X坐标
        /// </summary>
        public double X
        {
            get
            {
                if (this.displayBinding == null) { return 0; }
                return Canvas.GetLeft(this.displayBinding);
            }
            set
            {
                if (this.displayBinding != null)
                {
                    Canvas.SetLeft(this.displayBinding, value);
                }
            }
        }

        /// <summary>
        /// 获取或设置按钮的Y坐标
        /// </summary>
        public double Y
        {
            get
            {
                if (this.displayBinding == null) { return 0; }
                return Canvas.GetTop(this.displayBinding);
            }
            set
            {
                if (this.displayBinding != null)
                {
                    Canvas.SetTop(this.displayBinding, value);
                }
            }
        }

        /// <summary>
        /// 获取或设置按钮的Z坐标
        /// </summary>
        public int Z
        {
            get
            {
                if (this.displayBinding == null) { return 0; }
                return Canvas.GetZIndex(this.displayBinding);
            }
            set
            {
                if (this.displayBinding != null)
                {
                    Canvas.SetZIndex(this.displayBinding, value);
                }
            }
        }

        /// <summary>
        /// 获取或设置正常时的按钮图像
        /// </summary>
        public MySprite ImageNormal { get; set; }

        /// <summary>
        /// 获取或设置鼠标按下时的按钮图像
        /// </summary>
        public MySprite ImageMouseOn { get; set; }

        /// <summary>
        /// 获取或设置鼠标悬停时的按钮图像
        /// </summary>
        public MySprite ImageMouseOver { get; set; }

        /// <summary>
        /// 获取鼠标是否悬停在按钮上
        /// </summary>
        public bool isMouseOver { get; private set; }

        /// <summary>
        /// 获取鼠标是否按下按钮
        /// </summary>
        public bool isMouseOn { get; private set; }

        /// <summary>
        /// 获取或设置精灵动画锚点
        /// </summary>
        public SpriteAnchorType anchor
        {
            get
            {
                return this.anchorType;
            }
            set
            {
                this.anchorType = value;
                this.InitAnimationRenderTransform();
            }
        }

        /// <summary>
        /// 动画锚点类型
        /// </summary>
        private SpriteAnchorType anchorType = SpriteAnchorType.Center;

        /// <summary>
        /// 获取精灵锚点相对精灵左上角的X坐标
        /// </summary>
        public double anchorX
        {
            get
            {
                if (this.displayBinding == null)
                {
                    return 0;
                }
                return this.anchor == SpriteAnchorType.Center ? this.displayBinding.Width / 2 : 0;
            }
        }

        /// <summary>
        /// 获取精灵锚点相对精灵左上角的Y坐标
        /// </summary>
        public double anchorY
        {
            get
            {
                if (this.displayBinding == null)
                {
                    return 0;
                }
                return this.anchor == SpriteAnchorType.Center ? this.displayBinding.Height / 2 : 0;
            }
        }

        /// <summary>
        /// 初始化精灵的动画依赖
        /// </summary>
        public void InitAnimationRenderTransform()
        {
            TransformGroup aniGroup = new TransformGroup();
            TranslateTransform XYTransformer = new TranslateTransform();
            ScaleTransform ScaleTransformer = new ScaleTransform();
            ScaleTransformer.CenterX = this.anchorX;
            ScaleTransformer.CenterY = this.anchorY;
            RotateTransform RotateTransformer = new RotateTransform();
            RotateTransformer.CenterX = this.anchorX;
            RotateTransformer.CenterY = this.anchorY;
            aniGroup.Children.Add(XYTransformer);
            aniGroup.Children.Add(ScaleTransformer);
            aniGroup.Children.Add(RotateTransformer);
            this.displayBinding.RenderTransform = aniGroup;
        }

        /// <summary>
        /// 提供精灵按钮鼠标离开时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseLeaveHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOver = this.isMouseOn = false;
            if (this.ImageMouseOver != null || this.ImageMouseOn != null)
            {
                BitmapImage myBitmapImage = this.ImageNormal.myImage;
                this.ImageNormal.displayBinding = this.displayBinding;
                this.displayBinding.Width = myBitmapImage.PixelWidth;
                this.displayBinding.Height = myBitmapImage.PixelHeight;
                this.displayBinding.Source = myBitmapImage;
            }
        }

        /// <summary>
        /// 提供精灵按钮鼠标移入时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseEnterHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOver = true;
            if (this.ImageMouseOver != null)
            {
                BitmapImage myBitmapImage = this.ImageMouseOver.myImage;
                this.ImageNormal.displayBinding = null;
                this.ImageMouseOn.displayBinding = null;
                this.ImageMouseOver.displayBinding = this.displayBinding;
                this.displayBinding.Width = myBitmapImage.PixelWidth;
                this.displayBinding.Height = myBitmapImage.PixelHeight;
                this.displayBinding.Source = myBitmapImage;
            }
        }

        /// <summary>
        /// 提供精灵按钮鼠标按下时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseOnHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOn = true;
            if (this.ImageMouseOn != null)
            {
                BitmapImage myBitmapImage = this.ImageMouseOn.myImage;
                this.ImageNormal.displayBinding = null;
                this.ImageMouseOn.displayBinding = this.displayBinding;
                this.ImageMouseOver.displayBinding = null;
                this.displayBinding.Width = myBitmapImage.PixelWidth;
                this.displayBinding.Height = myBitmapImage.PixelHeight;
                this.displayBinding.Source = myBitmapImage;
            }
        }

        /// <summary>
        /// 提供精灵按钮鼠标松开时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseUpHandler(object sender, MouseEventArgs e)
        {
            if (this.isMouseOn)
            {
                if (this.isMouseOver && this.ImageMouseOver != null)
                {
                    BitmapImage myBitmapImage2 = this.ImageNormal.myImage;
                    this.ImageNormal.displayBinding = this.displayBinding;
                    this.ImageMouseOn.displayBinding = null;
                    this.ImageMouseOver.displayBinding = null;
                    this.displayBinding.Width = myBitmapImage2.PixelWidth;
                    this.displayBinding.Height = myBitmapImage2.PixelHeight;
                    this.displayBinding.Source = myBitmapImage2;
                    // 向运行时环境提交中断
                    if (this.Enable)
                    {
                        Slyvia.GetInstance().SubmitInterrupt(this.ntr);
                    }
                    return;
                }
                BitmapImage myBitmapImage = this.ImageNormal.myImage;
                this.ImageNormal.displayBinding = this.displayBinding;
                this.ImageMouseOn.displayBinding = null;
                this.ImageMouseOver.displayBinding = null;
                this.displayBinding.Width = myBitmapImage.PixelWidth;
                this.displayBinding.Height = myBitmapImage.PixelHeight;
                this.displayBinding.Source = myBitmapImage;
                // 向运行时环境提交中断
                if (this.Enable)
                {
                    Slyvia.GetInstance().SubmitInterrupt(this.ntr);
                }
            }
            this.isMouseOn = false;
        }
    }
}
