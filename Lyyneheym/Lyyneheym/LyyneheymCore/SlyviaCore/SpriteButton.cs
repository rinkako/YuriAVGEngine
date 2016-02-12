using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 精灵按钮类
    /// </summary>
    public class SpriteButton
    {
        /// <summary>
        /// 创建一个精灵按钮
        /// </summary>
        /// <param name="label">鼠标按下时跳转的标签名</param>
        /// <param name="normal">正常时的精灵</param>
        /// <param name="over">悬停时的精灵</param>
        /// <param name="on">按下时的精灵</param>
        public SpriteButton(string label, MySprite normal, MySprite over = null, MySprite on = null, SceneAction sa = null)
        {
            this.ImageNormal = normal;
            this.ImageMouseOver = over;
            this.ImageMouseOn = on;
            Interrupt intr = new Interrupt()
            {
                returnTarget = label,
                interruptSA = sa,
                type = InterruptType.ButtonJump,
                detail = ""
            };
            this.ntr = intr;
        }

        /// <summary>
        /// 按下时的中断
        /// </summary>
        public Interrupt ntr { get; set; }

        /// <summary>
        /// 获取或设置绑定前端显示控件
        /// </summary>
        public Image viewBinding { get; set; }

        /// <summary>
        /// 获取或设置按钮是否有效
        /// </summary>
        public bool Enable { get; set; }

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
        public bool isMouseOver
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取鼠标是否按下按钮
        /// </summary>
        public bool isMouseOn
        {
            get;
            private set;
        }

        /// <summary>
        /// 提供精灵按钮鼠标离开时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseLeaveHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOver = this.isMouseOn = false;
            BitmapImage myBitmapImage = this.ImageNormal.myImage;
            this.ImageNormal.displayBinding = this.viewBinding;
            this.viewBinding.Width = myBitmapImage.PixelWidth;
            this.viewBinding.Height = myBitmapImage.PixelHeight;
            this.viewBinding.Source = myBitmapImage;
        }

        /// <summary>
        /// 提供精灵按钮鼠标移入时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseEnterHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOver = true;
            BitmapImage myBitmapImage = this.ImageMouseOver.myImage;
            this.ImageNormal.displayBinding = null;
            this.ImageMouseOn.displayBinding = null;
            this.ImageMouseOver.displayBinding = this.viewBinding;
            this.viewBinding.Width = myBitmapImage.PixelWidth;
            this.viewBinding.Height = myBitmapImage.PixelHeight;
            this.viewBinding.Source = myBitmapImage;
        }

        /// <summary>
        /// 提供精灵按钮鼠标按下时的处理函数
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">鼠标参数</param>
        public void MouseOnHandler(object sender, MouseEventArgs e)
        {
            this.isMouseOn = true;
            BitmapImage myBitmapImage = this.ImageMouseOn.myImage;
            this.ImageNormal.displayBinding = null;
            this.ImageMouseOn.displayBinding = this.viewBinding;
            this.ImageMouseOver.displayBinding = null;
            this.viewBinding.Width = myBitmapImage.PixelWidth;
            this.viewBinding.Height = myBitmapImage.PixelHeight;
            this.viewBinding.Source = myBitmapImage;
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
                this.isMouseOn = false;
                BitmapImage myBitmapImage = this.ImageNormal.myImage;
                this.ImageNormal.displayBinding = this.viewBinding;
                this.ImageMouseOn.displayBinding = null;
                this.ImageMouseOver.displayBinding = null;
                this.viewBinding.Width = myBitmapImage.PixelWidth;
                this.viewBinding.Height = myBitmapImage.PixelHeight;
                this.viewBinding.Source = myBitmapImage;
                Slyvia.GetInstance().SubmitInterrupt(this.ntr);
            }
        }

    }
}
