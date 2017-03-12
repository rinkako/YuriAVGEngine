using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>场景镜头类：为游戏提供有3D景深效果的镜头移动动画</para>
    /// <para>注意所有效果在施加到调用堆栈上后该函数即刻结束，不等待动画完成</para>
    /// <para>她是一个静态类，被画音渲染器UpdateRender引用</para>
    /// </summary>
    public static class SCamera
    {
        /// <summary>
        /// 将镜头中心平移到指定的区块
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        public static void Translate(int r, int c)
        {
            var viewMana = ViewManager.GetInstance();
            bool leftFlag = false, topFlag = false;
            Storyboard storyLeftBg = null, storyLeftCs = null, storyLeftPic = null;
            Storyboard storyTopBg = null, storyTopCs = null, storyTopPic = null;
            // Left
            if (Director.ScrMana.SCameraFocusCol != c)
            {
                // background
                storyLeftBg = new Storyboard();
                Point deltaPBg = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale * SCamera.BackgroundDeepRatio);
                double BgfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left;
                double BgToX = BgfromX + deltaPBg.X;
                DoubleAnimation doubleAniLeftBg = new DoubleAnimation(BgfromX, BgToX, SCamera.AnimationDuration);
                doubleAniLeftBg.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftBg, new PropertyPath(Canvas.LeftProperty));
                storyLeftBg.Children.Add(doubleAniLeftBg);
                storyLeftBg.Duration = SCamera.AnimationDuration;
                storyLeftBg.FillBehavior = FillBehavior.Stop;
                storyLeftBg.Completed += StoryLeftBg_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left = BgToX;
                // character
                storyLeftCs = new Storyboard();
                Point deltaPCs = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale);
                double CsfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left;
                double CsToX = CsfromX + deltaPCs.X;
                DoubleAnimation doubleAniLeftCs = new DoubleAnimation(CsfromX, CsToX, SCamera.AnimationDuration);
                doubleAniLeftCs.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftCs, new PropertyPath(Canvas.LeftProperty));
                storyLeftCs.Children.Add(doubleAniLeftCs);
                storyLeftCs.Duration = SCamera.AnimationDuration;
                storyLeftCs.FillBehavior = FillBehavior.Stop;
                storyLeftCs.Completed += StoryLeftCs_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left = CsToX;
                // picture
                storyLeftPic = new Storyboard();
                Point deltaPPic = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale * SCamera.PictureDeepRatio);
                double PicfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Left;
                double PicToX = PicfromX + deltaPPic.X;
                DoubleAnimation doubleAniLeftPic = new DoubleAnimation(PicfromX, PicToX, SCamera.AnimationDuration);
                doubleAniLeftPic.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftPic, new PropertyPath(Canvas.LeftProperty));
                storyLeftPic.Children.Add(doubleAniLeftPic);
                storyLeftPic.Duration = SCamera.AnimationDuration;
                storyLeftPic.FillBehavior = FillBehavior.Stop;
                storyLeftPic.Completed += StoryLeftPic_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Left = PicToX;
            }
            // Top
            if (Director.ScrMana.SCameraFocusRow != r)
            {
                // background
                storyTopBg = new Storyboard();
                Point deltaPBg = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale * SCamera.BackgroundDeepRatio);
                double BgfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top;
                double BgToY = BgfromY + deltaPBg.Y;
                DoubleAnimation doubleAniTopBg = new DoubleAnimation(BgfromY, BgToY, SCamera.AnimationDuration);
                doubleAniTopBg.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopBg, new PropertyPath(Canvas.TopProperty));
                storyTopBg.Children.Add(doubleAniTopBg);
                storyTopBg.Duration = SCamera.AnimationDuration;
                storyTopBg.FillBehavior = FillBehavior.Stop;
                storyTopBg.Completed += StoryTopBg_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top = BgToY;
                // character
                storyTopCs = new Storyboard();
                Point deltaPCs = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale);
                double CsfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top;
                double CsToY = CsfromY + deltaPCs.Y;
                DoubleAnimation doubleAniTopCs = new DoubleAnimation(CsfromY, CsToY, SCamera.AnimationDuration);
                doubleAniTopCs.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopCs, new PropertyPath(Canvas.TopProperty));
                storyTopCs.Children.Add(doubleAniTopCs);
                storyTopCs.Duration = SCamera.AnimationDuration;
                storyTopCs.FillBehavior = FillBehavior.Stop;
                storyTopCs.Completed += StoryTopCs_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top = CsToY;
                // picture
                storyTopPic = new Storyboard();
                Point deltaPPic = SCamera.GetScreenDistance(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, r, c,
                    Director.ScrMana.SCameraScale * SCamera.PictureDeepRatio);
                double PicfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Top;
                double PicToY = PicfromY + deltaPPic.Y;
                DoubleAnimation doubleAniTopPic = new DoubleAnimation(PicfromY, PicToY, SCamera.AnimationDuration);
                doubleAniTopPic.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopPic, new PropertyPath(Canvas.TopProperty));
                storyTopPic.Children.Add(doubleAniTopPic);
                storyTopPic.Duration = SCamera.AnimationDuration;
                storyTopPic.FillBehavior = FillBehavior.Stop;
                storyTopPic.Completed += StoryTopPic_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Top = PicToY;
            }
            // Left Ani
            if (leftFlag)
            {
                storyLeftBg.Begin();
                storyLeftCs.Begin();
                storyLeftPic.Begin();
            }
            // Top Ani
            if (topFlag)
            {
                storyTopBg.Begin();
                storyTopCs.Begin();
                storyTopPic.Begin();
            }
        }
        
        /// <summary>
        /// 在镜头即将对准的区块上调整焦距
        /// 即将对准的意思是：当前全部带平移的动作都完成之后的镜头中心所对准的位置
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0</param>
        public static void Focus(double ratio)
        {

        }

        /// <summary>
        /// 将镜头对准某个指定区块并调整焦距
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        public static void FocusOn(int r, int c, double ratio)
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
        /// 重置镜头对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        public static void ResetFocus()
        {

        }

        /// <summary>
        /// 进入场景时的默认拉长焦距效果
        /// </summary>
        public static void EnterSceneZoomOut()
        {

        }

        /// <summary>
        /// 获取屏幕分区的中心坐标
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetScreenCoordination(int r, int c)
        {
            return SCamera.screenPointMap[r, c];
        }

        /// <summary>
        /// 获取屏幕分区之间的曼哈顿距离
        /// </summary>
        /// <param name="rA">A点分区行号</param>
        /// <param name="cA">A点分区列号</param>
        /// <param name="rB">B点分区行号</param>
        /// <param name="cB">B点分区列号</param>
        /// <param name="scale">尺度系数，-1为当前尺度</param>
        /// <returns>分区中点在指定尺度下的曼哈顿距离，不指定尺度即为当前尺度</returns>
        public static Point GetScreenDistance(int rA, int cA, int rB, int cB, double scale = -1)
        {
            var stdDist = SCamera.manhattanDistanceMap[SCamera.GetScreenCoordination(rA, cA)][SCamera.GetScreenCoordination(rB, cB)];
            return scale == -1 ? new Point(stdDist.X * Director.ScrMana.SCameraScale, stdDist.Y * Director.ScrMana.SCameraScale) :
                new Point(stdDist.X * scale, stdDist.Y * scale);
        }
        
        /// <summary>
        /// 获取角色分区的中心坐标
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 11]</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetCharacterCoordination(int r)
        {
            return SCamera.characterPointMap[r];
        }
        
        /// <summary>
        /// 计算两点之间距离
        /// </summary>
        /// <param name="pA">坐标点A</param>
        /// <param name="pB">坐标点B</param>
        /// <param name="scale">画布尺度</param>
        /// <returns>两点在这一尺度下的距离</returns>
        public static double GetPointDistance(Point pA, Point pB, double scale)
        {
            return Math.Sqrt(Math.Pow(pA.X - pB.X, 2) + Math.Pow(pA.Y - pB.Y, 2)) * scale;
        }

        /// <summary>
        /// 计算两点之间曼哈顿距离
        /// </summary>
        /// <param name="pA">坐标点A</param>
        /// <param name="pB">坐标点B</param>
        /// <param name="scale">画布尺度</param>
        /// <returns>两点在这一尺度下的曼哈顿距离</returns>
        public static Point GetManhattanDistance(Point pA, Point pB, double scale)
        {
            return new Point((pA.X - pB.X) * scale, (pA.Y - pB.Y) * scale);
        }

        /// <summary>
        /// 初始化镜头系统，必须在使用场景镜头系统前调用它
        /// </summary>
        public static void Init()
        {
            // 动画属性
            SCamera.SCameraAnimationTimeMS = 500;
            SCamera.DecelerateRatio = 0.7;
            // 景深尺度
            SCamera.BackgroundDeepRatio = 0.6;
            SCamera.PictureDeepRatio = 1.2;
            // 尺度初始化
            Director.ScrMana.SCameraScale = 1.0;
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = 2;
            // 提前计算所有中心点坐标，避免每次调用的重复计算
            SCamera.screenPointMap = new Point[GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT, GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT + 1];
            var ScrBlockWidth = (double)GlobalDataContainer.GAME_WINDOW_WIDTH / GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT;
            var ScrBlockHeight = (double)GlobalDataContainer.GAME_WINDOW_HEIGHT / GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT;
            var beginX = 0.0 - GlobalDataContainer.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT * ScrBlockWidth + ScrBlockWidth / 2.0;
            var beginY = ScrBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                // 变量j从1开始，为0预留位置，0是预留给屏幕横向的中央
                for (int j = 1; j <= GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    SCamera.screenPointMap[i, j] = new Point(beginX + (j - 1) * ScrBlockWidth, beginY + i * ScrBlockHeight);
                }
            }
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                SCamera.screenPointMap[i, 0] = new Point(GlobalDataContainer.GAME_WINDOW_WIDTH / 2.0, beginY + i * ScrBlockHeight);
            }
            SCamera.characterPointMap = new Point[GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT];
            var CstBlockHeight = (double)GlobalDataContainer.GAME_SCAMERA_CSTAND_HEIGHT / GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT;
            var beginCstY = 0.0 - ((double)GlobalDataContainer.GAME_SCAMERA_CSTAND_HEIGHT / 2.0) + CstBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT; i++)
            {
                SCamera.characterPointMap[i] = new Point(0, beginCstY + i * CstBlockHeight);
            }
            // 计算标准距离字典
            SCamera.manhattanDistanceMap = new Dictionary<Point, Dictionary<Point, Point>>();
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                for (int j = 0; j <= GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    var pt = SCamera.screenPointMap[i, j];
                    if (SCamera.manhattanDistanceMap.ContainsKey(pt) == false)
                    {
                        SCamera.manhattanDistanceMap[pt] = new Dictionary<Point, Point>();
                    }
                    for (int m = 0; m < GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT; m++)
                    {
                        for (int n = 0; n <= GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT; n++)
                        {
                            var rhsPt = SCamera.screenPointMap[m, n];
                            SCamera.manhattanDistanceMap[pt][rhsPt] = SCamera.GetManhattanDistance(pt, rhsPt, 1.0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 动画回调：前景层纵向移动
        /// </summary>
        private static void StoryTopPic_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding;
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Top);
        }

        /// <summary>
        /// 动画回调：立绘层纵向移动
        /// </summary>
        private static void StoryTopCs_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding;
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top);
        }

        /// <summary>
        /// 动画回调：背景层纵向移动
        /// </summary>
        private static void StoryTopBg_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding;
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top);
        }

        /// <summary>
        /// 动画回调：前景层横向移动
        /// </summary>
        private static void StoryLeftPic_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Left);
        }

        /// <summary>
        /// 动画回调：立绘层横向移动
        /// </summary>
        private static void StoryLeftCs_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left);
        }

        /// <summary>
        /// 动画回调：背景层横向移动
        /// </summary>
        private static void StoryLeftBg_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left);
        }

        /// <summary>
        /// 获取或设置背景变化率
        /// </summary>
        public static double BackgroundDeepRatio
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置前景贴图变化率
        /// </summary>
        public static double PictureDeepRatio
        {
            get;
            set;
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
                return SCamera.animationTimeMS;
            }
            set
            {
                SCamera.AnimationDuration = TimeSpan.FromMilliseconds(SCamera.animationTimeMS = value);
            }
        }
        
        /// <summary>
        /// 场景镜头动画时间间隔
        /// </summary>
        private static Duration AnimationDuration;

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
        /// 屏幕分块中心的标准距离字典
        /// </summary>
        private static Dictionary<Point, Dictionary<Point, Point>> manhattanDistanceMap;
    }
}
