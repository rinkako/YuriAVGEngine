using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 精灵按钮类
    /// </summary>
    public class SpriteButton
    {
        /// <summary>
        /// 构造精灵按钮
        /// </summary>
        /// <param name="bid"></param>
        public SpriteButton(int bid)
        {
            this.id = bid;
            this.ntr = null;
            this.displayBinding = null;
            this.Enable = true;
            this.Eternal = false;
            this.X = this.Y = 0;
            this.Z = GlobalDataContainer.GAME_Z_BUTTON;
            this.isMouseOn = this.isMouseOver = false;
        }

        /// <summary>
        /// 按钮编号
        /// </summary>
        public int id { get; set; }

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
        /// 获取或设置按钮是否在点击后仍存留屏幕上
        /// </summary>
        public bool Eternal { get; set; }

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
        public YuriSprite ImageNormal { get; set; }

        /// <summary>
        /// 获取或设置鼠标按下时的按钮图像
        /// </summary>
        public YuriSprite ImageMouseOn { get; set; }

        /// <summary>
        /// 获取或设置鼠标悬停时的按钮图像
        /// </summary>
        public YuriSprite ImageMouseOver { get; set; }

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
            if (this.Enable)
            {
                this.isMouseOver = this.isMouseOn = false;
                if (this.displayBinding != null && (this.ImageMouseOver != null || this.ImageMouseOn != null))
                {
                    BitmapImage myBitmapImage = this.ImageNormal.myImage;
                    this.ImageNormal.displayBinding = this.displayBinding;
                    this.displayBinding.Width = myBitmapImage.PixelWidth;
                    this.displayBinding.Height = myBitmapImage.PixelHeight;
                    this.displayBinding.Source = myBitmapImage;
                }
            }
        }

        /// <summary>
        /// 提供精灵按钮鼠标移入时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (this.Enable)
            {
                this.isMouseOver = true;
                if (this.displayBinding != null && this.ImageMouseOver != null)
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
        }

        /// <summary>
        /// 提供精灵按钮鼠标按下时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseOnHandler(object sender, MouseEventArgs e)
        {
            if (this.Enable)
            {
                this.isMouseOn = true;
                if (this.displayBinding != null && this.ImageMouseOn != null)
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
        }

        /// <summary>
        /// 提供精灵按钮鼠标松开时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseUpHandler(object sender, MouseEventArgs e)
        {
            if (this.Enable)
            {
                if (this.isMouseOn)
                {
                    if (this.displayBinding != null && this.isMouseOver && this.ImageMouseOver != null)
                    {
                        BitmapImage myBitmapImage2 = this.ImageMouseOver.myImage;
                        this.ImageNormal.displayBinding = null;
                        this.ImageMouseOn.displayBinding = null;
                        this.ImageMouseOver.displayBinding = this.displayBinding;
                        this.displayBinding.Width = myBitmapImage2.PixelWidth;
                        this.displayBinding.Height = myBitmapImage2.PixelHeight;
                        this.displayBinding.Source = myBitmapImage2;
                        // 向运行时环境提交中断
                        Director.GetInstance().SubmitInterrupt(this.ntr);
                        // 移除按钮
                        if (!this.Eternal)
                        {
                            this.Enable = false;
                            Director.GetInstance().RemoveButton(this.id);
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
                    Director.GetInstance().SubmitInterrupt(this.ntr);
                    // 移除按钮
                    if (!this.Eternal)
                    {
                        this.Enable = false;
                        Director.GetInstance().RemoveButton(this.id);
                    }
                }
                this.isMouseOn = false;
            }
        }
    }
}
