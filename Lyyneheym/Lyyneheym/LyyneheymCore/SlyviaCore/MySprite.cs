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
using Lyyneheym.LyyneheymCore.Utils;

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
        public void Init(string resName, ResourceType resType, MemoryStream ms, Int32Rect? cutrect = null)
        {
            if (!this.IsInit)
            {
                this.resourceName = resName;
                this.resourceType = resType;
                this.myImage = new BitmapImage();
                this.myImage.BeginInit();
                this.myImage.StreamSource = ms;
                if (cutrect != null)
                {
                    this.cutRect = (Int32Rect)cutrect;
                    this.myImage.SourceRect = this.cutRect;
                }
                this.myImage.EndInit();
                this.IsInit = true;
            }
            else
            {
                DebugUtils.ConsoleLine(String.Format("Sprite Init again: {0}", resName), "MySprite", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 初始化精灵对象，它只能被执行一次
        /// </summary>
        /// <param name="uri">材质的路径</param>
        /// <param name="cutrect">材质切割矩形</param>
        public void Init(string resName, ResourceType resType, Uri uri, Int32Rect? cutrect = null)
        {
            if (!this.IsInit)
            {
                this.resourceName = resName;
                this.resourceType = resType;
                this.myImage = new BitmapImage();
                this.myImage.BeginInit();
                this.myImage.UriSource = uri;
                if (cutrect != null)
                {
                    this.cutRect = (Int32Rect)cutrect;
                    this.myImage.SourceRect = this.cutRect;
                }
                this.myImage.EndInit();
                this.IsInit = true;
            }
            else
            {
                DebugUtils.ConsoleLine(String.Format("Sprite Init again: {0}", resName), "MySprite", OutputStyle.Error);
            }
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
                if (this.displayBinding != null)
                {
                    Canvas.SetLeft(this.displayBinding, value);
                }
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
        /// 获取精灵的资源类型
        /// </summary>
        public ResourceType resourceType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取精灵的资源名
        /// </summary>
        public string resourceName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置正在进行的动画数量
        /// </summary>
        public int AnimateCount
        {
            get
            {
                return this.AnimateCounter;
            }
            set
            {
                this.AnimateCounter = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 获取精灵是否已经初始化
        /// </summary>
        public bool IsInit
        {
            get;
            private set;
        }

        /// <summary>
        /// 精灵动画状态
        /// </summary>
        private int AnimateCounter = 0;

        /// <summary>
        /// 精灵动画锚点类型
        /// </summary>
        private SpriteAnchorType anchorType = SpriteAnchorType.Center;

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
