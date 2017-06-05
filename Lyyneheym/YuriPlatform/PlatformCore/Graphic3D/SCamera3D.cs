using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Yuri.PlatformCore.Graphic;

namespace Yuri.PlatformCore.Graphic3D
{
    /// <summary>
    /// <para>3D场景镜头类：为游戏提供有3D景深效果的镜头移动动画</para>
    /// <para>注意所有效果在施加到主调用堆栈上后该函数即刻结束，不等待动画完成，因此一般不应该在并行处理堆栈中调用她</para>
    /// <para>她是一个静态类，被画音渲染器UpdateRender引用</para>
    /// </summary>
    internal static class SCamera3D
    {
        /// <summary>
        /// 将镜头中心平移到指定的区块
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 14]，其中7是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        public static void Translate(int r, int c)
        {
            // 计算运动轨迹
            var orgPoint = SCamera3D.GetScreenCoordination(SCamera3D.lastFocusRow, SCamera3D.lastFocusCol);
            var destPoint = SCamera3D.GetScreenCoordination(r, c);
            var delta = SCamera3D.GetManhattanDistance(destPoint, orgPoint);
            var actualBeginPoint = ViewManager.View3D.ST3D_Camera.Position;
            // 动画
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, SCamera3D.lastZIndex),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X + delta.X, actualBeginPoint.Y + delta.Y, SCamera3D.lastZIndex),
                KeyTime = TimeSpan.FromMilliseconds(SCamera3D.animationTimeMS),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            AnimationClock aniClock = v3dAni.CreateClock();
            lock (SCamera3D.AnimatingStorySet)
            {
                SCamera3D.AnimatingStorySet.Add(aniClock);
            }
            v3dAni.Completed += delegate
            {
                lock (SCamera3D.AnimatingStorySet)
                {
                    SCamera3D.AnimatingStorySet.Remove(aniClock);
                }
                ViewManager.View3D.ST3D_Camera.Position = new Point3D(actualBeginPoint.X + delta.X,
                    actualBeginPoint.Y + delta.Y, SCamera3D.lastZIndex);
            };
            ViewManager.View3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            Director.ScrMana.SCameraFocusRow = SCamera3D.lastFocusRow = r;
            Director.ScrMana.SCameraFocusCol = SCamera3D.lastFocusCol = c;
            Director.ScrMana.SCameraScale = SCamera3D.lastZIndex;
        }

        /// <summary>
        /// 以某个区块为焦点调整焦距
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 14]，其中7是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        /// <param name="ratio">缩放的倍率，值域(0.0, +∞)，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        /// <param name="immediate">是否立即执行完毕</param>
        public static void FocusOn(int r, int c, double ratio, bool immediate = false)
        {
            // 计算运动轨迹
            var orgPoint = SCamera3D.GetScreenCoordination(SCamera3D.lastFocusRow, SCamera3D.lastFocusCol);
            var destPoint = SCamera3D.GetScreenCoordination(r, c);
            var destZ = SCamera3D.GetCameraZIndex(ratio);
            var deltaXY = SCamera3D.GetManhattanDistance(destPoint, orgPoint);
            var actualBeginPoint = ViewManager.View3D.ST3D_Camera.Position;
            // 瞬时动画就直接移动
            if (immediate)
            {
                ViewManager.View3D.ST3D_Camera.Position = new Point3D(actualBeginPoint.X + deltaXY.X,
                    actualBeginPoint.Y + deltaXY.Y, destZ);
                Director.ScrMana.SCameraFocusRow = SCamera3D.lastFocusRow = r;
                Director.ScrMana.SCameraFocusCol = SCamera3D.lastFocusCol = c;
                Director.ScrMana.SCameraScale = SCamera3D.lastZIndex = destZ;
                return;
            }
            // 动画
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, SCamera3D.lastZIndex),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X + deltaXY.X, actualBeginPoint.Y + deltaXY.Y, destZ),
                KeyTime = TimeSpan.FromMilliseconds(SCamera3D.animationTimeMS),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            AnimationClock aniClock = v3dAni.CreateClock();
            lock (SCamera3D.AnimatingStorySet)
            {
                SCamera3D.AnimatingStorySet.Add(aniClock);
            }
            v3dAni.Completed += delegate
            {
                lock (SCamera3D.AnimatingStorySet)
                {
                    SCamera3D.AnimatingStorySet.Remove(aniClock);
                }
                ViewManager.View3D.ST3D_Camera.Position = new Point3D(actualBeginPoint.X + deltaXY.X,
                    actualBeginPoint.Y + deltaXY.Y, destZ);
            };
            ViewManager.View3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            Director.ScrMana.SCameraFocusRow = SCamera3D.lastFocusRow = r;
            Director.ScrMana.SCameraFocusCol = SCamera3D.lastFocusCol = c;
            Director.ScrMana.SCameraScale = SCamera3D.lastZIndex = destZ;
        }

        /// <summary>
        /// 重置镜头将中央和焦点都对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        /// <param name="doubledDuration">是否2倍动画时间</param>
        public static void ResetFocus(bool doubledDuration)
        {
            int aniTime = doubledDuration ? SCamera3D.animationTimeMS * 2 : SCamera3D.animationTimeMS;
            var actualBeginPoint = ViewManager.View3D.ST3D_Camera.Position;
            Point3DAnimationUsingKeyFrames v3dAni = new Point3DAnimationUsingKeyFrames();
            EasingPoint3DKeyFrame k1 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(actualBeginPoint.X, actualBeginPoint.Y, SCamera3D.lastZIndex),
                KeyTime = TimeSpan.FromMilliseconds(0),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            EasingPoint3DKeyFrame k2 = new EasingPoint3DKeyFrame()
            {
                Value = new Point3D(0, 0, SCamera3D.orginalCameraZIndex),
                KeyTime = TimeSpan.FromMilliseconds(aniTime),
                EasingFunction = new CubicEase() {EasingMode = EasingMode.EaseOut}
            };
            v3dAni.KeyFrames.Add(k1);
            v3dAni.KeyFrames.Add(k2);
            v3dAni.FillBehavior = FillBehavior.Stop;
            AnimationClock aniClock = v3dAni.CreateClock();
            lock (SCamera3D.AnimatingStorySet)
            {
                SCamera3D.AnimatingStorySet.Add(aniClock);
            }
            v3dAni.Completed += delegate
            {
                lock (SCamera3D.AnimatingStorySet)
                {
                    SCamera3D.AnimatingStorySet.Remove(aniClock);
                }
                ViewManager.View3D.ST3D_Camera.Position = new Point3D(0, 0, SCamera3D.orginalCameraZIndex);
            };
            ViewManager.View3D.ST3D_Camera.BeginAnimation(ProjectionCamera.PositionProperty, v3dAni);
            // 更新后台
            Director.ScrMana.SCameraFocusRow = SCamera3D.lastFocusRow = GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            Director.ScrMana.SCameraFocusCol = SCamera3D.lastFocusCol = 0;
            Director.ScrMana.SCameraScale = SCamera3D.lastZIndex = SCamera3D.orginalCameraZIndex;
        }

        /// <summary>
        /// 在镜头聚焦的区块上调整焦距
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域(0.0, +∞)，原始尺寸对应于1.0</param>
        public static void Focus(double ratio)
        {
            SCamera3D.FocusOn(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, ratio);
        }

        /// <summary>
        /// 将镜头对准某个立绘的指定区块并调整焦距
        /// </summary>
        /// <param name="id">立绘id</param>
        /// <param name="blockId">立绘纵向划分区块id，值域[0, 11]，通常眼1，胸3，腹4，膝7，足10</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞)，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        public static void FocusCharacter(int id, int blockId, double ratio)
        {

        }

        /// <summary>
        /// 布置场景结束并准备进入时调用此方法以准备动画
        /// </summary>
        public static void PreviewEnterScene()
        {
            ViewManager.View3D.ST3D_Camera.Position = new Point3D(0, 0, SCamera3D.orginalCameraZIndex);
            SCamera3D.FocusOn(GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2, 0, 1.8, true);
        }

        /// <summary>
        /// 进入场景并做默认拉长焦距效果
        /// </summary>
        public static void PostEnterScene()
        {
            SCamera3D.ResumeBlackFrame();
            SCamera3D.ResetFocus(true);
        }

        /// <summary>
        /// 离开场景，切入黑场
        /// </summary>
        public static void LeaveSceneToBlackFrame()
        {
            var masker = ViewManager.MaskFrameRef;
            masker.Opacity = 0;
            masker.Visibility = Visibility.Visible;
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(0, 1, SCamera3D.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = SCamera3D.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 1;
                lock (SCamera3D.AnimatingStorySet)
                {
                    SCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (SCamera3D.AnimatingStorySet)
            {
                SCamera3D.AnimatingStorySet.Add(story);
            }
            story.Begin();
        }

        /// <summary>
        /// 直接从黑场中恢复
        /// </summary>
        public static void ResumeBlackFrame()
        {
            var masker = ViewManager.MaskFrameRef;
            if (masker.Visibility == Visibility.Hidden)
            {
                return;
            }
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(masker.Opacity, 0, SCamera3D.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = SCamera3D.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 0;
                masker.Visibility = Visibility.Hidden;
                lock (SCamera3D.AnimatingStorySet)
                {
                    SCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (SCamera3D.AnimatingStorySet)
            {
                SCamera3D.AnimatingStorySet.Add(story);
            }
            story.Begin();
        }

        /// <summary>
        /// 将立绘槽位置恢复到初始状态
        /// </summary>
        public static void ResetAllSlot()
        {
            for (int i = 0; i <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i++)
            {
                var desc = Director.ScrMana.GetCharacter3DDescriptor(i);
                if (desc != null)
                {
                    desc.Source = null;
                    desc.OffsetX = desc.OffsetY = desc.OffsetZ = 0;
                    desc.Opacity = desc.ScaleX = desc.ScaleY = 1;
                    ViewManager.GetInstance().Draw(i, ResourceType.Stand);
                }
            }
        }

        /// <summary>
        /// 初始化镜头系统，必须在使用场景镜头系统前调用它
        /// </summary>
        public static void Init()
        {
            // 动画属性
            SCamera3D.SCameraAnimationTimeMS = 500;
            // 尺度初始化
            Director.ScrMana.SCameraScale = SCamera3D.orginalCameraZIndex;
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            // 计算区块的横向中点坐标
            double blockWidth = SCamera3D.scrWidth / (GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT - GlobalConfigContext.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT);
            double blockOffset = blockWidth / 2.0;
            var xArr = new double[GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT + 1];
            for (int i = GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i >= 1; i--)
            {
                xArr[i] = 0 - blockOffset - blockWidth * (GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT / 2.0 - i);
            }
            xArr[0] = 0;
            // 计算区块的可视区间
            SCamera3D.CharacterBlockList = new List<Tuple<Point3D, Point3D, Point3D, Point3D>>();
            for (int i = 0; i <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i++)
            {
                var LeftBottom = new Point3D(xArr[i] - 2.031, -4.252, 0);
                var RightBottom = new Point3D(xArr[i] + 2.031, -4.252, 0);
                var LeftUp = new Point3D(xArr[i] - 2.031, 1.652, 0);
                var RightUp = new Point3D(xArr[i] + 2.031, 1.652, 0);
                SCamera3D.CharacterBlockList.Add(new Tuple<Point3D, Point3D, Point3D, Point3D>(LeftBottom, RightBottom, LeftUp, RightUp));
            }
            // 计算区块中点坐标
            double blockHeight = SCamera3D.scrHeight / GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT;
            SCamera3D.screenPointMap = new Point[GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT, GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT + 1];
            for (int i = 0; i < GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                // 变量j从1开始，为0预留位置，0是预留给屏幕横向的中央
                for (int j = 1; j <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    SCamera3D.screenPointMap[i, j] = new Point(xArr[j], (i - GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2) * blockHeight);
                }
            }
            for (int i = 0; i < GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                SCamera3D.screenPointMap[i, 0] = new Point(xArr[0], (i - GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2) * blockHeight);
            }
            // 计算标准曼哈顿距离表
            SCamera3D.manhattanDistanceMap = new Dictionary<Point, Dictionary<Point, Point>>();
            for (int i = 0; i < GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                for (int j = 0; j <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    var pt = SCamera3D.screenPointMap[i, j];
                    if (SCamera3D.manhattanDistanceMap.ContainsKey(pt) == false)
                    {
                        SCamera3D.manhattanDistanceMap[pt] = new Dictionary<Point, Point>();
                    }
                    for (int m = 0; m < GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT; m++)
                    {
                        for (int n = 0; n <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; n++)
                        {
                            var rhsPt = SCamera3D.screenPointMap[m, n];
                            SCamera3D.manhattanDistanceMap[pt][rhsPt] = SCamera3D.GetManhattanDistance(pt, rhsPt);
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
        /// <param name="r">区块的横向编号，值域[0, 14]，其中7是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 32]，其中0是屏幕横向正中</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetScreenCoordination(int r, int c) => SCamera3D.screenPointMap[r, c];

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
            lock (SCamera3D.AnimatingStorySet)
            {
                foreach (var ani in SCamera3D.AnimatingStorySet)
                {
                    if (ani is Storyboard asb)
                    {
                        asb.SkipToFill();
                    }
                    else if (ani is AnimationClock ac)
                    {
                        ac.Controller?.SkipToFill();
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
                lock (SCamera3D.AnimatingStorySet)
                {
                    return SCamera3D.AnimatingStorySet.Any();
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
                return SCamera3D.animationTimeMS;
            }
            set
            {
                SCamera3D.animationDuration = TimeSpan.FromMilliseconds(SCamera3D.animationTimeMS = value);
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
        private static readonly HashSet<object> AnimatingStorySet = new HashSet<object>();
        
        /// <summary>
        /// 屏幕分块中心的标准距离字典
        /// </summary>
        private static Dictionary<Point, Dictionary<Point, Point>> manhattanDistanceMap;

        /// <summary>
        /// 最后对准的行区块，用于回滚
        /// </summary>
        private static int lastFocusRow = GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;

        /// <summary>
        /// 最后对准的列区块，用于回滚
        /// </summary>
        private static int lastFocusCol = 0;

        /// <summary>
        /// 最后的镜头深度，用于回滚
        /// </summary>
        private static double lastZIndex = 8;

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
