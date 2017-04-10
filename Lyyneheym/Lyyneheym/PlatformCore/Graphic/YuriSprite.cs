using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yuri.Utils;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 精灵类：为图形资源提供展示、用户互动和动画效果的类
    /// </summary>
    internal class YuriSprite
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
                this.ResourceName = resName;
                this.ResourceType = resType;
                this.SpriteBitmapImage = new BitmapImage();
                this.SpriteBitmapImage.BeginInit();
                this.SpriteBitmapImage.StreamSource = ms;
                if (cutrect != null)
                {
                    this.CutRect = (Int32Rect)cutrect;
                    this.SpriteBitmapImage.SourceRect = this.CutRect;
                }
                this.SpriteBitmapImage.EndInit();
                this.IsInit = true;
            }
            else
            {
                CommonUtils.ConsoleLine(String.Format("Sprite Init again: {0}", resName), "MySprite", OutputStyle.Error);
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
                this.ResourceName = resName;
                this.ResourceType = resType;
                this.SpriteBitmapImage = new BitmapImage();
                this.SpriteBitmapImage.BeginInit();
                this.SpriteBitmapImage.UriSource = uri;
                if (cutrect != null)
                {
                    this.CutRect = (Int32Rect)cutrect;
                    this.SpriteBitmapImage.SourceRect = this.CutRect;
                }
                this.SpriteBitmapImage.EndInit();
                this.IsInit = true;
            }
            else
            {
                CommonUtils.ConsoleLine(String.Format("Sprite Init again: {0}", resName), "MySprite", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 获取一个相对于左上角的像素点的颜色
        /// </summary>
        /// <param name="X">检测点X坐标</param>
        /// <param name="Y">检测点Y坐标</param>
        /// <returns>一个ARGB描述的Color实例</returns>
        public Color GetPixelColor(double X, double Y)
        {
            Color c = Color.FromArgb(Byte.MaxValue, 0, 0, 0);
            if (this.SpriteBitmapImage != null)
            {
                try
                {
                    CroppedBitmap cb = new CroppedBitmap(this.SpriteBitmapImage, new Int32Rect((int)X, (int)Y, 1, 1));
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
        /// <param name="X">检测点X坐标</param>
        /// <param name="Y">检测点Y坐标</param>
        /// <param name="threshold">透明度阈值</param>
        /// <returns>该点是否不超过透明阈值</returns>
        public bool IsEmptyRegion(double X, double Y, int threshold = 0)
        {
            return this.GetPixelColor(X, Y).A <= threshold;
        }

        /// <summary>
        /// 初始化精灵的动画依赖
        /// </summary>
        public void InitAnimationRenderTransform()
        {
            TransformGroup aniGroup = new TransformGroup();
            TranslateTransform XYTransformer = new TranslateTransform();
            ScaleTransform ScaleTransformer = new ScaleTransform();
            ScaleTransformer.CenterX = this.AnchorX;
            ScaleTransformer.CenterY = this.AnchorY;
            RotateTransform RotateTransformer = new RotateTransform();
            RotateTransformer.CenterX = this.AnchorX;
            RotateTransformer.CenterY = this.AnchorY;
            aniGroup.Children.Add(XYTransformer);
            aniGroup.Children.Add(ScaleTransformer);
            aniGroup.Children.Add(RotateTransformer);
            this.DisplayBinding.RenderTransform = aniGroup;
            this.TranslateTransformer = XYTransformer;
            this.RotateTransformer = RotateTransformer;
            this.ScaleTransformer = ScaleTransformer;
        }

        /// <summary>
        /// 获取或设置精灵动画锚点
        /// </summary>
        public SpriteAnchorType Anchor
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
        public double AnchorX
        {
            get
            {
                if (this.DisplayBinding == null)
                {
                    return 0;
                }
                return this.Anchor == SpriteAnchorType.Center ? this.DisplayBinding.Width / 2 : 0;
            }
        }

        /// <summary>
        /// 获取精灵锚点相对精灵左上角的Y坐标
        /// </summary>
        public double AnchorY
        {
            get
            {
                if (this.DisplayBinding == null)
                {
                    return 0;
                }
                return this.Anchor == SpriteAnchorType.Center ? this.DisplayBinding.Height / 2 : 0;
            }
        }

        /// <summary>
        /// 获取或设置纹理切割矩形
        /// </summary>
        public Int32Rect CutRect { get; set; }

        /// <summary>
        /// 获取或设置纹理源
        /// </summary>
        public BitmapImage SpriteBitmapImage { get; set; }

        /// <summary>
        /// 获取或设置前端显示控件
        /// </summary>
        public FrameworkElement DisplayBinding
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
        public double DisplayX
        {
            get
            {
                return Canvas.GetLeft(this.DisplayBinding);
            }
            set
            {
                if (this.DisplayBinding != null)
                {
                    Canvas.SetLeft(this.DisplayBinding, value);
                }
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的Y值
        /// </summary>
        public double DisplayY
        {
            get
            {
                return Canvas.GetTop(this.DisplayBinding);
            }
            set
            {
                if (this.DisplayBinding != null)
                {
                    Canvas.SetTop(this.DisplayBinding, value);
                }
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的Z值
        /// </summary>
        public int DisplayZ
        {
            get
            {
                return Canvas.GetZIndex(this.DisplayBinding);
            }
            set
            {
                Canvas.SetZIndex(this.DisplayBinding, value);
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的透明度
        /// </summary>
        public double DisplayOpacity
        {
            get
            {
                return this.DisplayBinding.Opacity;
            }
            set
            {
                this.DisplayBinding.Opacity = value;
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的宽度
        /// </summary>
        public double DisplayWidth
        {
            get
            {
                return this.DisplayBinding.Width;
            }
            set
            {
                this.DisplayBinding.Width = value;
            }
        }

        /// <summary>
        /// 获取或设置前端显示控件的高度
        /// </summary>
        public double DisplayHeight
        {
            get
            {
                return this.DisplayBinding.Height;
            }
            set
            {
                this.DisplayBinding.Height = value;
            }
        }

        /// <summary>
        /// 获取源图片的宽度
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return this.SpriteBitmapImage.Width;
            }
        }

        /// <summary>
        /// 获取源图片的高度
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return this.SpriteBitmapImage.Height;
            }
        }

        /// <summary>
        /// 获取当前精灵是否被绑定到Image前端对象上
        /// </summary>
        public bool IsDisplaying
        {
            get
            {
                return this.DisplayBinding != null;
            }
        }

        /// <summary>
        /// 获取精灵是否被缩放
        /// </summary>
        public bool IsScaling
        {
            get
            {
                return this.Descriptor.ScaleX != 1 || this.Descriptor.ScaleY != 1;
            }
        }

        /// <summary>
        /// 获取精灵的资源类型
        /// </summary>
        public ResourceType ResourceType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取精灵的资源名
        /// </summary>
        public string ResourceName
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
                return this.animateCounter;
            }
            set
            {
                this.animateCounter = Math.Max(0, value);
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
        /// 获取或设置精灵的描述子
        /// </summary>
        public SpriteDescriptor Descriptor
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定的平移变换器
        /// </summary>
        public TranslateTransform TranslateTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定的缩放变换器
        /// </summary>
        public ScaleTransform ScaleTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置绑定的旋转变换器
        /// </summary>
        public RotateTransform RotateTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置背景层实际显示控件的引用
        /// </summary>
        public FrameworkElement AnimationElement
        {
            get;
            set;
        }

        /// <summary>
        /// 精灵动画状态
        /// </summary>
        private int animateCounter = 0;

        /// <summary>
        /// 精灵动画锚点类型
        /// </summary>
        private SpriteAnchorType anchorType = SpriteAnchorType.Center;

        /// <summary>
        /// 前端控件绑定
        /// </summary>
        private FrameworkElement viewBinding = null;
    }

    /// <summary>
    /// 枚举：精灵的动画锚点
    /// </summary>
    internal enum SpriteAnchorType
    {
        LeftTop,
        Center
    }
}
