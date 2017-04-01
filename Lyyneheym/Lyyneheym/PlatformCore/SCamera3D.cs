using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Yuri.PlatformCore
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
        /// <remarks>当缩放比不位于区间[1, 2]时，可能出现无法对齐区域中心的情况，需在后续版本中修正</remarks>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        public static void Translate(int r, int c)
        {
            
        }

        /// <summary>
        /// 以某个区块为焦点调整焦距
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        /// <param name="immediate">是否立即执行完毕</param>
        public static void FocusOn(int r, int c, double ratio, bool immediate = false)
        {
            
        }

        /// <summary>
        /// 重置镜头将中央和焦点都对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        /// <param name="doubledDuration">是否1.5倍动画时间</param>
        public static void ResetFocus(bool doubledDuration)
        {
            
        }

        /// <summary>
        /// 在镜头聚焦的区块上调整焦距
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0</param>
        public static void Focus(double ratio)
        {

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
                lock (SCamera3D.aniCountMutex)
                {
                    SCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (SCamera3D.aniCountMutex)
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
                lock (SCamera3D.aniCountMutex)
                {
                    SCamera3D.AnimatingStorySet.Remove(story);
                }
            };
            lock (SCamera3D.aniCountMutex)
            {
                SCamera3D.AnimatingStorySet.Add(story);
            }
            story.Begin();
        }
        
        /// <summary>
        /// 初始化镜头系统，必须在使用场景镜头系统前调用它
        /// </summary>
        public static void Init()
        {
            // 动画属性
            SCamera3D.SCameraAnimationTimeMS = 500;
            SCamera3D.DecelerateRatio = 0.7;
            // 尺度初始化
            Director.ScrMana.SCameraScale = SCamera3D.orginalCameraZIndex;
            Director.ScrMana.SCameraCenterCol = 0;
            Director.ScrMana.SCameraCenterRow = 2;
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = 2;
            // 计算区块的中轴
            const double scrWidth = 6.66;
            double blockWidth = scrWidth / (GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT - GlobalConfigContext.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT);
            double blockOffset = blockWidth / 2.0;
            var xArr = new double[GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT + 1];
            for (int i = GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i >= 1; i--)
            {
                xArr[i] = 0 - blockOffset - blockWidth * (GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT / 2.0 - i);
            }
            xArr[0] = 0;
            // 计算区块的可视区间
            SCamera3D.characterBlockList = new List<Tuple<Point3D, Point3D, Point3D, Point3D>>();
            for (int i = 0; i <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i++)
            {
                var LeftBottom = new Point3D(xArr[i] - 2.031, -4.252, 0);
                var RightBottom = new Point3D(xArr[i] + 2.031, -4.252, 0);
                var LeftUp = new Point3D(xArr[i] - 2.031, 1.652, 0);
                var RightUp = new Point3D(xArr[i] + 2.031, 1.652, 0);
                SCamera3D.characterBlockList.Add(new Tuple<Point3D, Point3D, Point3D, Point3D>(LeftBottom, RightBottom, LeftUp, RightUp));
            }
        }

        /// <summary>
        /// 立即跳过所有动画
        /// </summary>
        public static void SkipAll()
        {
            lock (SCamera3D.aniCountMutex)
            {
                foreach (var ani in SCamera3D.AnimatingStorySet)
                {
                    ani.SkipToFill();
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
                lock (SCamera3D.aniCountMutex)
                {
                    return AnimatingStorySet.Any();
                }
            }
        }
        
        /// <summary>
        /// 获取或设置动画缓动率
        /// </summary>
        public static double DecelerateRatio
        {
            get;
            set;
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
        /// 上一动作是否为缩放
        /// </summary>
        private static bool lastFromScaling = false;

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
        /// 立绘分块中心相对坐标字典
        /// </summary>
        private static Point[] characterPointMap;

        /// <summary>
        /// 立绘分区表
        /// </summary>
        private static List<Tuple<Point3D, Point3D, Point3D, Point3D>> characterBlockList;

        /// <summary>
        /// 正在进行的动画计数
        /// </summary>
        private static readonly HashSet<Storyboard> AnimatingStorySet = new HashSet<Storyboard>();

        /// <summary>
        /// 动画计数器互斥量
        /// </summary>
        private static readonly Mutex aniCountMutex = new Mutex();

        /// <summary>
        /// 屏幕分块中心的标准距离字典
        /// </summary>
        private static Dictionary<Point, Dictionary<Point, Point>> manhattanDistanceMap;

        /// <summary>
        /// 镜头默认深度
        /// </summary>
        private const double orginalCameraZIndex = 8.0;
    }
}
