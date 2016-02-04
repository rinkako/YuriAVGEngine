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
    /// 精灵类：为图形资源提供展示、用户互动和动画效果的类
    /// </summary>
    public class MySprite
    {
        /// <summary>
        /// 初始化精灵对象，它只能被执行一次
        /// </summary>
        /// <param name="ms">材质的内存流</param>
        /// <param name="cutrect">材质切割矩形</param>
        public void Init(MemoryStream ms, Int32Rect? cutrect = null)
        {
            this.myImage = new BitmapImage();
            this.myImage.BeginInit();
            this.myImage.StreamSource = ms;
            if (cutrect != null)
            {
                this.cutRect = (Int32Rect)cutrect;
                this.myImage.SourceRect = this.cutRect;
            }
            this.myImage.EndInit();
        }

        /// <summary>
        /// 初始化精灵对象，它只能被执行一次
        /// </summary>
        /// <param name="uri">材质的路径</param>
        /// <param name="cutrect">材质切割矩形</param>
        public void Init(Uri uri, Int32Rect? cutrect = null)
        {
            this.myImage = new BitmapImage();
            this.myImage.BeginInit();
            this.myImage.UriSource = uri;
            if (cutrect != null)
            {
                this.cutRect = (Int32Rect)cutrect;
                this.myImage.SourceRect = this.cutRect;
            }
            this.myImage.EndInit();
        }

        /// <summary>
        /// 初始化精灵的动画依赖
        /// </summary>
        private void InitAnimationRenderTransform()
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
        /// 获取一个相对于左上角的像素点的颜色
        /// </summary>
        /// <param name="pos">像素点坐标</param>
        /// <returns>一个ARGB描述的Color实例</returns>
        public Color GetPixelColor(Point pos)
        {
            Color c = Color.FromArgb(Byte.MaxValue, 0, 0, 0);
            if (this.myImage != null)
            {
                try
                {
                    CroppedBitmap cb = new CroppedBitmap(this.myImage, new Int32Rect((int)pos.X, (int)pos.Y, 1, 1));
                    byte[] pixels = new byte[4];
                    cb.CopyPixels(pixels, 4, 0);
                    c = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                }
                catch (Exception) { }
            }
            return c;
        }

        /// <summary>
        /// 判断一个相对于左上角的像素点是否全透明
        /// </summary>
        /// <param name="pos">像素点坐标</param>
        /// <param name="threshold">透明度阈值</param>
        /// <returns>该点是否不超过透明阈值</returns>
        public bool IsEmptyRegion(Point pos, int threshold = 0)
        {
            return this.GetPixelColor(pos).A <= threshold;
        }
        
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
        /// 获取精灵锚点的X坐标
        /// </summary>
        public double anchorX
        {
            get
            {
                return this.anchor == SpriteAnchorType.Center ?
                    this.displayX + this.displayBinding.Width / 2 : this.displayX;
            }
        }

        /// <summary>
        /// 获取精灵锚点的Y坐标
        /// </summary>
        public double anchorY
        {
            get
            {
                return this.anchor == SpriteAnchorType.Center ?
                    this.displayY + this.displayBinding.Height / 2 : this.displayY;
            }
        }

        /// <summary>
        /// 获取或设置纹理切割矩形
        /// </summary>
        public Int32Rect cutRect { get; set; }

        /// <summary>
        /// 获取或设置纹理源
        /// </summary>
        public BitmapImage myImage { get; set; }

        /// <summary>
        /// 获取或设置前端显示控件
        /// </summary>
        public Image displayBinding
        {
            get
            {
                return this.viewBinding;
            }
            set
            {
                this.viewBinding = value;
                this.InitAnimationRenderTransform();
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的X值
        /// </summary>
        public double displayX
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
        /// 获取或设置前端显示控件的Y值
        /// </summary>
        public double displayY
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
        /// 获取或设置前端显示控件的Z值
        /// </summary>
        public int displayZ
        {
            get
            {
                return Canvas.GetZIndex(this.displayBinding);
            }
            set
            {
                Canvas.SetZIndex(this.displayBinding, value);
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的透明度
        /// </summary>
        public double displayOpacity
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
        /// 获取或设置前端显示控件的宽度
        /// </summary>
        public double displayWidth
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
        /// 获取或设置前端显示控件的高度
        /// </summary>
        public double displayHeight
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
        /// 获取源图片的宽度
        /// </summary>
        public double imageWidth
        {
            get
            {
                return this.myImage.Width;
            }
        }

        /// <summary>
        /// 获取源图片的高度
        /// </summary>
        public double imageHeight
        {
            get
            {
                return this.myImage.Height;
            }
        }

        /// <summary>
        /// 获取当前精灵是否被绑定到Image前端对象上
        /// </summary>
        public bool isDisplaying
        {
            get
            {
                return this.displayBinding != null;
            }
        }

        /// <summary>
        /// 精灵动画锚点类型
        /// </summary>
        private SpriteAnchorType anchorType = SpriteAnchorType.LeftTop;

        /// <summary>
        /// 前端控件绑定
        /// </summary>
        private Image viewBinding = null;
    }

    /// <summary>
    /// 枚举：精灵的动画锚点
    /// </summary>
    public enum SpriteAnchorType
    {
        LeftTop,
        Center
    }
}
