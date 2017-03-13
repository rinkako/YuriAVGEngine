using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>场景镜头类：为游戏提供有3D景深效果的镜头移动动画</para>
    /// <para>注意所有效果在施加到调用堆栈上后该函数即刻结束，不等待动画完成，因此一般不应该在并行处理器中调用她</para>
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
            if (Director.ScrMana.SCameraCenterCol != c)
            {
                leftFlag = true;
                // background
                storyLeftBg = new Storyboard();
                Point deltaPBg = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
                    Director.ScrMana.SCameraScale * Math.Pow(SCamera.BackgroundDeepRatio, 3));
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
                Point deltaPCs = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
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
                Point deltaPPic = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
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
            if (Director.ScrMana.SCameraCenterRow != r)
            {
                topFlag = true;
                // background
                storyTopBg = new Storyboard();
                Point deltaPBg = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
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
                Point deltaPCs = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
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
                Point deltaPPic = SCamera.GetScreenDistance(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol, r, c,
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
            // 更新属性
            Director.ScrMana.SCameraCenterRow = r;
            Director.ScrMana.SCameraCenterCol = c;
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
        /// 以某个区块为焦点调整焦距
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        public static void FocusOn(int r, int c, double ratio)
        {
            // 获取变换点绝对坐标
            var sPoint = SCamera.GetScreenCoordination(r, c);
            var viewMana = ViewManager.GetInstance();
            // 调整焦距和焦点
            Director.ScrMana.SCameraScale = ratio;
            if (r != Director.ScrMana.SCameraFocusRow || c != Director.ScrMana.SCameraFocusCol)
            {
                Director.ScrMana.SCameraFocusRow = r;
                Director.ScrMana.SCameraFocusCol = c;
                // 处理背景的焦点
                var aniGroupBg = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding.RenderTransform;
                var scaleTransformerBg = (ScaleTransform)aniGroupBg.Children[1];
                // 聚焦点在左边
                if (c <= 8)
                {
                    scaleTransformerBg.CenterX = SCamera.GetScreenCoordination(0, c + (8 - c) / 2).X;
                }
                // 聚焦点在水平中央
                else if (c == 0)
                {
                    scaleTransformerBg.CenterX = sPoint.X;
                }
                // 聚焦点在右边
                else
                {
                    scaleTransformerBg.CenterX = SCamera.GetScreenCoordination(0, 9 + (c - 9) / 2).X;
                }
                // 聚焦点在上边
                if (r < 2)
                {
                    scaleTransformerBg.CenterY = SCamera.GetScreenCoordination(r + (2 - r) / 2, 0).Y;
                }
                // 聚焦点在竖直中央
                else if (r == 2)
                {
                    scaleTransformerBg.CenterY = sPoint.Y;
                }
                // 聚焦点在下边
                else
                {
                    scaleTransformerBg.CenterY = SCamera.GetScreenCoordination(2 + (r - 2) / 2, 0).Y;
                }
                // 处理立绘的焦点
                var aniGroupCs = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding.RenderTransform;
                var scaleTransformerCs = (ScaleTransform)aniGroupCs.Children[1];
                scaleTransformerCs.CenterX = sPoint.X;
                scaleTransformerCs.CenterY = sPoint.Y;
                // 处理前景的焦点
                var aniGroupPic = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding.RenderTransform;
                var scaleTransformerPic = (ScaleTransform)aniGroupPic.Children[1];
                // 聚焦点在左边
                if (c <= 8)
                {
                    scaleTransformerPic.CenterX = SCamera.GetScreenCoordination(0, Math.Max(1, c - (8 - c) / 2)).X;
                }
                // 聚焦点在水平中央
                else if (c == 0)
                {
                    scaleTransformerPic.CenterX = sPoint.X;
                }
                // 聚焦点在右边
                else
                {
                    scaleTransformerPic.CenterX = SCamera.GetScreenCoordination(0, Math.Min(GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT, c + (c - 9) / 2)).X;
                }
                // 聚焦点在上边
                if (r < 2)
                {
                    scaleTransformerPic.CenterY = SCamera.GetScreenCoordination(Math.Max(0, r - (2 - r) / 2), 0).Y;
                }
                // 聚焦点在竖直中央
                else if (r == 2)
                {
                    scaleTransformerPic.CenterY = sPoint.Y;
                }
                // 聚焦点在下边
                else
                {
                    scaleTransformerPic.CenterY = SCamera.GetScreenCoordination(Math.Min(GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT - 1, r + (r - 2) / 2), 0).Y;
                }
            }
            // background
            Storyboard storyScaleBg = new Storyboard();
            DoubleAnimation doubleAniScaleXBg = new DoubleAnimation(1, ratio * SCamera.BackgroundDeepRatio, SCamera.AnimationDuration);
            DoubleAnimation doubleAniScaleYBg = new DoubleAnimation(1, ratio * SCamera.BackgroundDeepRatio, SCamera.AnimationDuration);
            doubleAniScaleXBg.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYBg.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleBg.Children.Add(doubleAniScaleXBg);
            storyScaleBg.Children.Add(doubleAniScaleYBg);
            storyScaleBg.Duration = SCamera.AnimationDuration;
            storyScaleBg.FillBehavior = FillBehavior.Stop;
            storyScaleBg.Completed += StoryScaleBg_Completed;
            Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleX =
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleY = ratio * SCamera.BackgroundDeepRatio;
            // character
            Storyboard storyScaleCs = new Storyboard();
            DoubleAnimation doubleAniScaleXCs = new DoubleAnimation(1, ratio, SCamera.AnimationDuration);
            DoubleAnimation doubleAniScaleYCs = new DoubleAnimation(1, ratio, SCamera.AnimationDuration);
            doubleAniScaleXCs.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYCs.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleCs.Children.Add(doubleAniScaleXCs);
            storyScaleCs.Children.Add(doubleAniScaleYCs);
            storyScaleCs.Duration = SCamera.AnimationDuration;
            storyScaleCs.FillBehavior = FillBehavior.Stop;
            storyScaleCs.Completed += StoryScaleCs_Completed;
            Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleX =
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleY = ratio;
            // picture
            Storyboard storyScalePic = new Storyboard();
            DoubleAnimation doubleAniScaleXPic = new DoubleAnimation(1, ratio * SCamera.PictureDeepRatio, SCamera.AnimationDuration);
            DoubleAnimation doubleAniScaleYPic = new DoubleAnimation(1, ratio * SCamera.PictureDeepRatio, SCamera.AnimationDuration);
            doubleAniScaleXPic.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYPic.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScalePic.Children.Add(doubleAniScaleXPic);
            storyScalePic.Children.Add(doubleAniScaleYPic);
            storyScalePic.Duration = SCamera.AnimationDuration;
            storyScalePic.FillBehavior = FillBehavior.Stop;
            storyScalePic.Completed += StoryScalePic_Completed;
            Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleX =
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleY = ratio * SCamera.PictureDeepRatio;
            // Apply animation
            storyScaleCs.Begin();
            storyScaleBg.Begin();
            storyScalePic.Begin();
        }

        /// <summary>
        /// 重置镜头中央和焦点都对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        public static void ResetFocus()
        {
            SCamera.Translate(2, 0);
            SCamera.Focus(1.0);
        }

        /// <summary>
        /// 在镜头聚焦的区块上调整焦距
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0</param>
        public static void Focus(double ratio)
        {
            SCamera.FocusOn(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, ratio);
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
            Director.ScrMana.SCameraCenterCol = 0;
            Director.ScrMana.SCameraCenterRow = 2;
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = 2;
            // 提前计算所有中心点坐标，避免每次调用的重复计算
            SCamera.screenPointMap = new Point[GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT, GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT + 1];
            var ScrBlockWidth = (double)GlobalDataContainer.GAME_WINDOW_WIDTH / (GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT - GlobalDataContainer.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT * 2);
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
        /// 动画回调：前景层缩放
        /// </summary>
        private static void StoryScalePic_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleX;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleY;
        }

        /// <summary>
        /// 动画回调：立绘层缩放
        /// </summary>
        private static void StoryScaleCs_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleX;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleY;
        }

        /// <summary>
        /// 动画回调：背景层缩放
        /// </summary>
        private static void StoryScaleBg_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleX;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleY;
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
