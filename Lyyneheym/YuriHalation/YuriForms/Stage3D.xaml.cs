using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Yuri.YuriHalation.YuriForms
{
    /// <summary>
    /// Stage3D.xaml 的交互逻辑
    /// </summary>
    public partial class Stage3D : Page
    {
        /// <summary>
        /// 初始化标记位
        /// </summary>
        private bool isInit = false;

        /// <summary>
        /// 构造器
        /// </summary>
        public Stage3D()
        {
            InitializeComponent();

            this.BO_MainGrid.Width = 1280;
            this.BO_MainGrid.Height = 720;
            this.ST3D_Viewport.Width = 1280;
            this.ST3D_Viewport.Height = 720;

            PreviewSCamera3D.MaskFrameRef = this.maskFrame;
            PreviewSCamera3D.ST3D_Camera = this.ST3D_Camera;
        }

        /// <summary>
        /// 事件：页面加载完毕
        /// </summary>
        private void Stage3D_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.isInit == false)
            {
                PreviewSCamera3D.Init();
                this.isInit = true;
            }
        }
    }
    
    /// <summary>
    /// <para>3D场景镜头类：为游戏提供有3D景深效果的镜头移动动画</para>
    /// <para>注意所有效果在施加到主调用堆栈上后该函数即刻结束，不等待动画完成，因此一般不应该在并行处理堆栈中调用她</para>
    /// <para>她是一个静态类，被画音渲染器UpdateRender引用</para>
    /// </summary>
    internal static class PreviewSCamera3D
    {
        public static int GAME_SCAMERA_SCR_ROWCOUNT = 15;
        public static int GAME_SCAMERA_SCR_COLCOUNT = 32;
        public static int GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT = 6;
        public static int SCameraFocusRow = 7;
        public static int SCameraFocusCol = 0;
        public static double SCameraScale = 8;
        public static PerspectiveCamera ST3D_Camera { get; set; } = null;
        public static Frame MaskFrameRef { get; set; } = null;
        /// <summary>
        /// 将镜头中心平移到指定的区块
        /// </summary>
        /// <remarks>当缩放比不位于区间[1, 2]时，可能出现无法对齐区域中心的情况，需在后续版本中修正</remarks>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        public static void Translate(int r, int c)
        {
            // 计算运动轨迹
            var orgPoint = PreviewSCamera3D.GetScreenCoordination(PreviewSCamera3D.SCameraFocusRow, PreviewSCamera3D.SCameraFocusCol);
            var destPoint = PreviewSCamera3D.GetScreenCoordination(r, c);
            var delta = PreviewSCamera3D.GetManhattanDistance(destPoint, orgPoint);
            var actualBeginPoint = PreviewSCamera3D.ST3D_Camera.Position;
            // 动画
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, PreviewSCamera3D.SCameraScale),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X + delta.X, actualBeginPoint.Y + delta.Y, PreviewSCamera3D.SCameraScale),
                KeyTime = TimeSpan.FromMilliseconds(PreviewSCamera3D.animationTimeMS),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            v3dAni.Completed += delegate
            {
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    PreviewSCamera3D.AnimatingStorySet.Remove(v3dAni);
                }
                PreviewSCamera3D.ST3D_Camera.Position = new Point3D(actualBeginPoint.X + delta.X,
                    actualBeginPoint.Y + delta.Y, PreviewSCamera3D.SCameraScale);
            };
            PreviewSCamera3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            PreviewSCamera3D.SCameraFocusRow = r;
            PreviewSCamera3D.SCameraFocusCol = c;
        }

        /// <summary>
        /// 以某个区块为焦点调整焦距
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        /// <param name="immediate">是否立即执行完毕</param>
        public static void FocusOn(int r, int c, double ratio, bool immediate = false)
        {
            var aniTime = immediate ? 1 : PreviewSCamera3D.animationTimeMS;
            // 计算运动轨迹
            var orgPoint = PreviewSCamera3D.GetScreenCoordination(PreviewSCamera3D.SCameraFocusRow, PreviewSCamera3D.SCameraFocusCol);
            var destPoint = PreviewSCamera3D.GetScreenCoordination(r, c);
            var destZ = PreviewSCamera3D.GetCameraZIndex(ratio);
            var deltaXY = PreviewSCamera3D.GetManhattanDistance(destPoint, orgPoint);
            var actualBeginPoint = PreviewSCamera3D.ST3D_Camera.Position;
            // 动画
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, PreviewSCamera3D.SCameraScale),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X + deltaXY.X, actualBeginPoint.Y + deltaXY.Y, destZ),
                KeyTime = TimeSpan.FromMilliseconds(aniTime),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            v3dAni.Completed += delegate
            {
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    PreviewSCamera3D.AnimatingStorySet.Remove(v3dAni);
                }
                PreviewSCamera3D.ST3D_Camera.Position = new Point3D(actualBeginPoint.X + deltaXY.X, actualBeginPoint.Y + deltaXY.Y, destZ);
            };
            PreviewSCamera3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            PreviewSCamera3D.SCameraFocusRow = r;
            PreviewSCamera3D.SCameraFocusCol = c;
            PreviewSCamera3D.SCameraScale = destZ;
        }

        /// <summary>
        /// 重置镜头将中央和焦点都对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        /// <param name="doubledDuration">是否2倍动画时间</param>
        public static void ResetFocus(bool doubledDuration)
        {
            int aniTime = doubledDuration ? PreviewSCamera3D.animationTimeMS * 2 : PreviewSCamera3D.animationTimeMS;
            var actualBeginPoint = PreviewSCamera3D.ST3D_Camera.Position;
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, PreviewSCamera3D.SCameraScale),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(0, 0, PreviewSCamera3D.orginalCameraZIndex),
                KeyTime = TimeSpan.FromMilliseconds(aniTime),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            v3dAni.Completed += delegate
            {
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    PreviewSCamera3D.AnimatingStorySet.Remove(v3dAni);
                }
                PreviewSCamera3D.ST3D_Camera.Position = new Point3D(0, 0, PreviewSCamera3D.orginalCameraZIndex);
            };
            PreviewSCamera3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            PreviewSCamera3D.SCameraFocusRow = PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            PreviewSCamera3D.SCameraFocusCol = 0;
            PreviewSCamera3D.SCameraScale = PreviewSCamera3D.orginalCameraZIndex;
        }

        /// <summary>
        /// 在镜头聚焦的区块上调整焦距
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0</param>
        public static void Focus(double ratio)
        {
            PreviewSCamera3D.FocusOn(PreviewSCamera3D.SCameraFocusRow, PreviewSCamera3D.SCameraFocusCol, ratio);
        }

        /// <summary>
        /// 将镜头对准某个立绘的指定区块并调整焦距
        /// </summary>
        /// <param name="id">立绘id</param>
        /// <param name="blockId">立绘纵向划分区块id，值域[0, 11]，通常眼1，胸3，腹4，膝7，足10</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        public static void FocusCharacter(int id, int blockId, double ratio)
        {

        }

        /// <summary>
        /// 布置场景结束并准备进入时调用此方法以准备动画
        /// </summary>
        public static void PreviewEnterScene()
        {
            PreviewSCamera3D.ST3D_Camera.Position = new Point3D(0, 0, PreviewSCamera3D.orginalCameraZIndex);
            PreviewSCamera3D.FocusOn(PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT / 2, 0, 1.8, true);
        }

        /// <summary>
        /// 进入场景并做默认拉长焦距效果
        /// </summary>
        public static void PostEnterScene()
        {
            PreviewSCamera3D.ResumeBlackFrame();
            PreviewSCamera3D.ResetFocus(true);
        }

        /// <summary>
        /// 离开场景，切入黑场
        /// </summary>
        public static void LeaveSceneToBlackFrame()
        {
            var masker = PreviewSCamera3D.MaskFrameRef;
            masker.Opacity = 0;
            masker.Visibility = Visibility.Visible;
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(0, 1, PreviewSCamera3D.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = PreviewSCamera3D.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 1;
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    PreviewSCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (PreviewSCamera3D.AnimatingStorySet)
            {
                PreviewSCamera3D.AnimatingStorySet.Add(story);
            }
            story.Begin();
        }

        /// <summary>
        /// 直接从黑场中恢复
        /// </summary>
        public static void ResumeBlackFrame()
        {
            var masker = PreviewSCamera3D.MaskFrameRef;
            if (masker.Visibility == Visibility.Hidden)
            {
                return;
            }
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(masker.Opacity, 0, PreviewSCamera3D.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = PreviewSCamera3D.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 0;
                masker.Visibility = Visibility.Hidden;
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    PreviewSCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (PreviewSCamera3D.AnimatingStorySet)
            {
                PreviewSCamera3D.AnimatingStorySet.Add(story);
            }
            story.Begin();
        }

        /// <summary>
        /// 初始化镜头系统，必须在使用场景镜头系统前调用它
        /// </summary>
        public static void Init()
        {
            // 动画属性
            PreviewSCamera3D.SCameraAnimationTimeMS = 500;
            // 尺度初始化
            PreviewSCamera3D.SCameraScale = PreviewSCamera3D.orginalCameraZIndex;
            PreviewSCamera3D.SCameraFocusCol = 0;
            PreviewSCamera3D.SCameraFocusRow = PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            // 计算区块的横向中点坐标
            double blockWidth = PreviewSCamera3D.scrWidth / (PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT - PreviewSCamera3D.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT);
            double blockOffset = blockWidth / 2.0;
            var xArr = new double[PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT + 1];
            for (int i = PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT; i >= 1; i--)
            {
                xArr[i] = 0 - blockOffset - blockWidth * (PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT / 2.0 - i);
            }
            xArr[0] = 0;
            // 计算区块的可视区间
            PreviewSCamera3D.CharacterBlockList = new List<Tuple<Point3D, Point3D, Point3D, Point3D>>();
            for (int i = 0; i <= PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT; i++)
            {
                var LeftBottom = new Point3D(xArr[i] - 2.031, -4.252, 0);
                var RightBottom = new Point3D(xArr[i] + 2.031, -4.252, 0);
                var LeftUp = new Point3D(xArr[i] - 2.031, 1.652, 0);
                var RightUp = new Point3D(xArr[i] + 2.031, 1.652, 0);
                PreviewSCamera3D.CharacterBlockList.Add(new Tuple<Point3D, Point3D, Point3D, Point3D>(LeftBottom, RightBottom, LeftUp, RightUp));
            }
            // 计算区块中点坐标
            double blockHeight = PreviewSCamera3D.scrHeight / PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT;
            PreviewSCamera3D.screenPointMap = new Point[PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT, PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT + 1];
            for (int i = 0; i < PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                // 变量j从1开始，为0预留位置，0是预留给屏幕横向的中央
                for (int j = 1; j <= PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    PreviewSCamera3D.screenPointMap[i, j] = new Point(xArr[j], (i - PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT / 2) * blockHeight);
                }
            }
            for (int i = 0; i < PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                PreviewSCamera3D.screenPointMap[i, 0] = new Point(xArr[0], (i - PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT / 2) * blockHeight);
            }
            // 计算标准曼哈顿距离表
            PreviewSCamera3D.manhattanDistanceMap = new Dictionary<Point, Dictionary<Point, Point>>();
            for (int i = 0; i < PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                for (int j = 0; j <= PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    var pt = PreviewSCamera3D.screenPointMap[i, j];
                    if (PreviewSCamera3D.manhattanDistanceMap.ContainsKey(pt) == false)
                    {
                        PreviewSCamera3D.manhattanDistanceMap[pt] = new Dictionary<Point, Point>();
                    }
                    for (int m = 0; m < PreviewSCamera3D.GAME_SCAMERA_SCR_ROWCOUNT; m++)
                    {
                        for (int n = 0; n <= PreviewSCamera3D.GAME_SCAMERA_SCR_COLCOUNT; n++)
                        {
                            var rhsPt = PreviewSCamera3D.screenPointMap[m, n];
                            PreviewSCamera3D.manhattanDistanceMap[pt][rhsPt] = PreviewSCamera3D.GetManhattanDistance(pt, rhsPt);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算两点之间曼哈顿距离
        /// </summary>
        /// <param name="pA">世界坐标对镜头平面的投影点A</param>
        /// <param name="pB">世界坐标对镜头平面的投影点B</param>
        /// <returns>两点在世界坐标下的曼哈顿距离</returns>
        public static Point GetManhattanDistance(Point pA, Point pB) => new Point(pA.X - pB.X, pA.Y - pB.Y);

        /// <summary>
        /// 获取屏幕分区的中心坐标
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetScreenCoordination(int r, int c) => PreviewSCamera3D.screenPointMap[r, c];

        /// <summary>
        /// 获取缩放指定尺度时的镜头深度
        /// </summary>
        /// <param name="scale">立绘层缩放尺度</param>
        /// <returns>镜头的Z坐标</returns>
        public static double GetCameraZIndex(double scale) => 8.0 * Math.Pow(scale, -1);

        /// <summary>
        /// 获取指定镜头深度下的缩放尺度
        /// </summary>
        /// <param name="zindex">镜头的Z坐标</param>
        /// <returns>立绘层缩放尺度</returns>
        public static double GetCameraScale(double zindex) => 8.0 * Math.Pow(zindex, -1);

        /// <summary>
        /// 立即跳过所有动画
        /// </summary>
        public static void SkipAll()
        {
            lock (PreviewSCamera3D.AnimatingStorySet)
            {
                foreach (var ani in PreviewSCamera3D.AnimatingStorySet)
                {
                    if (ani is Storyboard asb)
                    {
                        asb.SkipToFill();
                    }
                }
            }
        }

        /// <summary>
        /// 获取是否有动画正在进行
        /// </summary>
        public static bool IsAnyAnimation
        {
            get
            {
                lock (PreviewSCamera3D.AnimatingStorySet)
                {
                    return PreviewSCamera3D.AnimatingStorySet.Any();
                }
            }
        }

        /// <summary>
        /// 获取或设置场景镜头动画时间（毫秒）
        /// </summary>
        public static int SCameraAnimationTimeMS
        {
            get
            {
                return PreviewSCamera3D.animationTimeMS;
            }
            set
            {
                PreviewSCamera3D.animationDuration = TimeSpan.FromMilliseconds(PreviewSCamera3D.animationTimeMS = value);
            }
        }

        /// <summary>
        /// 场景镜头动画时间间隔
        /// </summary>
        private static Duration animationDuration;

        /// <summary>
        /// 场景镜头动画时间（毫秒）
        /// </summary>
        private static int animationTimeMS;

        /// <summary>
        /// 屏幕分块中心绝对坐标字典
        /// </summary>
        private static Point[,] screenPointMap;

        /// <summary>
        /// 立绘分区表
        /// </summary>
        public static List<Tuple<Point3D, Point3D, Point3D, Point3D>> CharacterBlockList { get; private set; }

        /// <summary>
        /// 正在进行的动画计数
        /// </summary>
        private static readonly HashSet<Timeline> AnimatingStorySet = new HashSet<Timeline>();

        /// <summary>
        /// 屏幕分块中心的标准距离字典
        /// </summary>
        private static Dictionary<Point, Dictionary<Point, Point>> manhattanDistanceMap;

        /// <summary>
        /// 镜头默认深度
        /// </summary>
        private const double orginalCameraZIndex = 8.0;

        /// <summary>
        /// Z为0时屏幕横向尺寸
        /// </summary>
        private const double scrWidth = 6.66;

        /// <summary>
        /// Z为0时屏幕纵向尺寸
        /// </summary>
        private const double scrHeight = 3.76;
    }
}
