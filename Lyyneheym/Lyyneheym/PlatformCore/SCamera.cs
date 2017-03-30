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
    /// <para>注意所有效果在施加到主调用堆栈上后该函数即刻结束，不等待动画完成，因此一般不应该在并行处理堆栈中调用她</para>
    /// <para>她是一个静态类，被画音渲染器UpdateRender引用</para>
    /// </summary>
    public static class SCamera
    {
        /// <summary>
        /// 将镜头中心平移到指定的区块
        /// </summary>
        /// <remarks>当缩放比不位于区间[1, 2]时，可能出现无法对齐区域中心的情况，需在后续版本中修正</remarks>
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
                double bgRatio = Director.ScrMana.SCameraScale * Math.Pow(SCamera.BackgroundDeepRatio, 2);
                double BgfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left;
                double BgBeginX = BgfromX;
                if (lastFromScaling)
                {
                    double actualX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                    if (actualX != BgBeginX)
                    {
                        BgBeginX = actualX;
                    }
                }
                Point normalPointBg = SCamera.GetNormalPoint(bgRatio);
                Point destinationPointBg = SCamera.GetScreenCoordination(r, c);
                Point originalPointBg = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineXBg = SCamera.GetManhattanDistance(destinationPointBg, normalPointBg, 1).X;
                double originalToNormalLineXBg = SCamera.GetManhattanDistance(originalPointBg, normalPointBg, 1).X;
                double BgToX = BgfromX - (destinationToNormalLineXBg - originalToNormalLineXBg) * bgRatio;
                DoubleAnimation doubleAniLeftBg = new DoubleAnimation(BgfromX, BgToX, SCamera.animationDuration);
                doubleAniLeftBg.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftBg, new PropertyPath(Canvas.LeftProperty));
                storyLeftBg.Children.Add(doubleAniLeftBg);
                storyLeftBg.Duration = SCamera.animationDuration;
                storyLeftBg.FillBehavior = FillBehavior.Stop;
                storyLeftBg.Completed += StoryLeftBg_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left = BgToX;
                // character
                storyLeftCs = new Storyboard();
                double CsfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left;
                double CsBeginX = CsfromX;
                if (lastFromScaling)
                {
                    double actualX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                    if (actualX != CsBeginX)
                    {
                        CsBeginX = actualX;
                    }
                }
                Point normalPointCs = SCamera.GetNormalPoint(Director.ScrMana.SCameraScale);
                Point destinationPointCs = SCamera.GetScreenCoordination(r, c);
                Point originalPointCs = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineXCs = SCamera.GetManhattanDistance(destinationPointCs, normalPointCs, 1).X;
                double originalToNormalLineXCs = SCamera.GetManhattanDistance(originalPointCs, normalPointCs, 1).X;
                double CsToX = CsfromX - (destinationToNormalLineXCs - originalToNormalLineXCs) * Director.ScrMana.SCameraScale;
                DoubleAnimation doubleAniLeftCs = new DoubleAnimation(CsBeginX, CsToX, SCamera.animationDuration);
                doubleAniLeftCs.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftCs, new PropertyPath(Canvas.LeftProperty));
                storyLeftCs.Children.Add(doubleAniLeftCs);
                storyLeftCs.Duration = SCamera.animationDuration;
                storyLeftCs.FillBehavior = FillBehavior.Stop;
                storyLeftCs.Completed += StoryLeftCs_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left = CsToX;
                // picture
                storyLeftPic = new Storyboard();
                double picRatio = Director.ScrMana.SCameraScale * Math.Pow(SCamera.PictureDeepRatio, 2);
                double PicfromX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Left;
                double PicBeginX = PicfromX;
                if (lastFromScaling)
                {
                    double actualX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                    if (actualX != PicBeginX)
                    {
                        PicBeginX = actualX;
                    }
                }
                Point normalPointPic = SCamera.GetNormalPoint(picRatio);
                Point destinationPointPic = SCamera.GetScreenCoordination(r, c);
                Point originalPointPic = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineXPic = SCamera.GetManhattanDistance(destinationPointPic, normalPointPic, 1).X;
                double originalToNormalLineXPic = SCamera.GetManhattanDistance(originalPointPic, normalPointPic, 1).X;
                double PicToX = PicfromX - (destinationToNormalLineXPic - originalToNormalLineXPic) * picRatio;
                DoubleAnimation doubleAniLeftPic = new DoubleAnimation(PicfromX, PicToX, SCamera.animationDuration);
                doubleAniLeftPic.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniLeftPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniLeftPic, new PropertyPath(Canvas.LeftProperty));
                storyLeftPic.Children.Add(doubleAniLeftPic);
                storyLeftPic.Duration = SCamera.animationDuration;
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
                double bgRatio = Director.ScrMana.SCameraScale * SCamera.BackgroundDeepRatio;
                double BgfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top;
                double BgBeginY = BgfromY;
                if (lastFromScaling)
                {
                    double actualY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                    if (actualY != BgBeginY)
                    {
                        BgBeginY = actualY;
                    }
                }
                Point normalPointBg = SCamera.GetNormalPoint(bgRatio);
                Point destinationPointBg = SCamera.GetScreenCoordination(r, c);
                Point originalPointBg = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineYBg = SCamera.GetManhattanDistance(destinationPointBg, normalPointBg, 1).Y;
                double originalToNormalLineYBg = SCamera.GetManhattanDistance(originalPointBg, normalPointBg, 1).Y;
                double BgToY = BgfromY - (destinationToNormalLineYBg - originalToNormalLineYBg) * bgRatio;
                DoubleAnimation doubleAniTopBg = new DoubleAnimation(BgfromY, BgToY, SCamera.animationDuration);
                doubleAniTopBg.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopBg, new PropertyPath(Canvas.TopProperty));
                storyTopBg.Children.Add(doubleAniTopBg);
                storyTopBg.Duration = SCamera.animationDuration;
                storyTopBg.FillBehavior = FillBehavior.Stop;
                storyTopBg.Completed += StoryTopBg_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top = BgToY;
                // character
                storyTopCs = new Storyboard();
                double CsfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top;
                double CsBeginY = CsfromY;
                if (lastFromScaling)
                {
                    double actualY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                    if (actualY != CsBeginY)
                    {
                        CsBeginY = actualY;
                    }
                }
                Point normalPointCs = SCamera.GetNormalPoint(Director.ScrMana.SCameraScale);
                Point destinationPointCs = SCamera.GetScreenCoordination(r, c);
                Point originalPointCs = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineYCs = SCamera.GetManhattanDistance(destinationPointCs, normalPointCs, 1).Y;
                double originalToNormalLineYCs = SCamera.GetManhattanDistance(originalPointCs, normalPointCs, 1).Y;
                double CsToY = CsfromY - (destinationToNormalLineYCs - originalToNormalLineYCs) * Director.ScrMana.SCameraScale;
                DoubleAnimation doubleAniTopCs = new DoubleAnimation(CsfromY, CsToY, SCamera.animationDuration);
                doubleAniTopCs.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopCs, new PropertyPath(Canvas.TopProperty));
                storyTopCs.Children.Add(doubleAniTopCs);
                storyTopCs.Duration = SCamera.animationDuration;
                storyTopCs.FillBehavior = FillBehavior.Stop;
                storyTopCs.Completed += StoryTopCs_Completed;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top = CsToY;
                // picture
                storyTopPic = new Storyboard();
                double picRatio = Director.ScrMana.SCameraScale * SCamera.PictureDeepRatio;
                double PicfromY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Top;
                double PicBeginY = PicfromY;
                if (lastFromScaling)
                {
                    double actualY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                    if (actualY != PicBeginY)
                    {
                        PicBeginY = actualY;
                    }
                }
                Point normalPointPic = SCamera.GetNormalPoint(picRatio);
                Point destinationPointPic = SCamera.GetScreenCoordination(r, c);
                Point originalPointPic = SCamera.GetScreenCoordination(Director.ScrMana.SCameraCenterRow, Director.ScrMana.SCameraCenterCol);
                double destinationToNormalLineYPic = SCamera.GetManhattanDistance(destinationPointPic, normalPointPic, 1).Y;
                double originalToNormalLineYPic = SCamera.GetManhattanDistance(originalPointPic, normalPointPic, 1).Y;
                double PicToY = PicfromY - (destinationToNormalLineYPic - originalToNormalLineYPic) * picRatio;
                DoubleAnimation doubleAniTopPic = new DoubleAnimation(PicfromY, PicToY, SCamera.animationDuration);
                doubleAniTopPic.DecelerationRatio = SCamera.DecelerateRatio;
                Storyboard.SetTarget(doubleAniTopPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
                Storyboard.SetTargetProperty(doubleAniTopPic, new PropertyPath(Canvas.TopProperty));
                storyTopPic.Children.Add(doubleAniTopPic);
                storyTopPic.Duration = SCamera.animationDuration;
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
            lastFromScaling = false;
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
            // 获取变换点绝对坐标
            var sPoint = SCamera.GetScreenCoordination(r, c);
            var viewMana = ViewManager.GetInstance();
            // 0时间间隔不会触发回调，因此使用1跳时延做动画以触发回调
            var timespan = immediate ? new Duration(TimeSpan.FromTicks(1)) : SCamera.animationDuration;
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
                if (c <= GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2)
                {
                    scaleTransformerBg.CenterX = SCamera.GetScreenCoordination(0, c + (GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2 - c) / 2).X;
                }
                // 聚焦点在水平中央
                else if (c == 0)
                {
                    scaleTransformerBg.CenterX = sPoint.X;
                }
                // 聚焦点在右边
                else
                {
                    var midR = GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2 + 1;
                    scaleTransformerBg.CenterX = SCamera.GetScreenCoordination(0, midR + (c - midR) / 2).X;
                }
                // 聚焦点在上边
                if (r < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2)
                {
                    scaleTransformerBg.CenterY = SCamera.GetScreenCoordination(r + (GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2 - r) / 2, 0).Y;
                }
                // 聚焦点在竖直中央
                else if (r == GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2)
                {
                    scaleTransformerBg.CenterY = sPoint.Y;
                }
                // 聚焦点在下边
                else
                {
                    scaleTransformerBg.CenterY = SCamera.GetScreenCoordination(GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2 + (r - GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2) / 2, 0).Y;
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
                if (c <= GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2)
                {
                    scaleTransformerPic.CenterX = SCamera.GetScreenCoordination(0, Math.Max(1, c - (GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2 - c) / 2)).X;
                }
                // 聚焦点在水平中央
                else if (c == 0)
                {
                    scaleTransformerPic.CenterX = sPoint.X;
                }
                // 聚焦点在右边
                else
                {
                    var midR = GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT / 2 + 1;
                    scaleTransformerPic.CenterX = SCamera.GetScreenCoordination(0, Math.Min(GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT, c + (c - midR) / 2)).X;
                }
                // 聚焦点在上边
                if (r < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2)
                {
                    scaleTransformerPic.CenterY = SCamera.GetScreenCoordination(Math.Max(0, r - (GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2 - r) / 2), 0).Y;
                }
                // 聚焦点在竖直中央
                else if (r == GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2)
                {
                    scaleTransformerPic.CenterY = sPoint.Y;
                }
                // 聚焦点在下边
                else
                {
                    scaleTransformerPic.CenterY = SCamera.GetScreenCoordination(Math.Min(GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT - 1, r + (r - GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2) / 2), 0).Y;
                }
            }
            // background
            var bgVbox = viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding;
            var bgVDesc = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground);
            Storyboard storyScaleBg = new Storyboard();
            DoubleAnimation doubleAniScaleXBg = new DoubleAnimation(bgVDesc.ScaleX, ratio * SCamera.BackgroundDeepRatio, timespan);
            DoubleAnimation doubleAniScaleYBg = new DoubleAnimation(bgVDesc.ScaleY, ratio * SCamera.BackgroundDeepRatio, timespan);
            doubleAniScaleXBg.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYBg.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXBg, bgVbox);
            Storyboard.SetTarget(doubleAniScaleYBg, bgVbox);
            Storyboard.SetTargetProperty(doubleAniScaleXBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleBg.Children.Add(doubleAniScaleXBg);
            storyScaleBg.Children.Add(doubleAniScaleYBg);
            storyScaleBg.Duration = timespan;
            storyScaleBg.FillBehavior = FillBehavior.Stop;
            storyScaleBg.Completed += StoryScaleBg_Completed;
            bgVDesc.ScaleX = bgVDesc.ScaleY = ratio * SCamera.BackgroundDeepRatio;
            // character
            var csVbox = viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding;
            var csVDesc = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand);
            Storyboard storyScaleCs = new Storyboard();
            DoubleAnimation doubleAniScaleXCs = new DoubleAnimation(csVDesc.ScaleX, ratio, timespan);
            DoubleAnimation doubleAniScaleYCs = new DoubleAnimation(csVDesc.ScaleY, ratio, timespan);
            doubleAniScaleXCs.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYCs.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXCs, csVbox);
            Storyboard.SetTarget(doubleAniScaleYCs, csVbox);
            Storyboard.SetTargetProperty(doubleAniScaleXCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleCs.Children.Add(doubleAniScaleXCs);
            storyScaleCs.Children.Add(doubleAniScaleYCs);
            storyScaleCs.Duration = timespan;
            storyScaleCs.FillBehavior = FillBehavior.Stop;
            storyScaleCs.Completed += StoryScaleCs_Completed;
            csVDesc.ScaleX = csVDesc.ScaleY = ratio;
            // 初步只支持1和2两个缩放倍率
            if (ratio <= 1)
            {
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left = 0;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top = 0;
            }
            else if (ratio > 1 && ratio <= 2.5)
            {
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left = -GlobalDataContext.GAME_WINDOW_WIDTH / ratio;
                Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top = -GlobalDataContext.GAME_WINDOW_HEIGHT / ratio;
            }
            // picture
            var picVbox = viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding;
            var picVDesc = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures);
            Storyboard storyScalePic = new Storyboard();
            DoubleAnimation doubleAniScaleXPic = new DoubleAnimation(picVDesc.ScaleX, ratio * SCamera.PictureDeepRatio, timespan);
            DoubleAnimation doubleAniScaleYPic = new DoubleAnimation(picVDesc.ScaleY, ratio * SCamera.PictureDeepRatio, timespan);
            doubleAniScaleXPic.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYPic.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXPic, picVbox);
            Storyboard.SetTarget(doubleAniScaleYPic, picVbox);
            Storyboard.SetTargetProperty(doubleAniScaleXPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScalePic.Children.Add(doubleAniScaleXPic);
            storyScalePic.Children.Add(doubleAniScaleYPic);
            storyScalePic.Duration = timespan;
            storyScalePic.FillBehavior = FillBehavior.Stop;
            storyScalePic.Completed += StoryScalePic_Completed;
            picVDesc.ScaleX = picVDesc.ScaleY = ratio * SCamera.PictureDeepRatio;
            // Apply animation
            storyScaleCs.Begin();
            storyScaleBg.Begin();
            storyScalePic.Begin();
            lastFromScaling = true;
        }

        /// <summary>
        /// 重置镜头将中央和焦点都对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        /// <param name="doubledDuration">是否1.5倍动画时间</param>
        public static void ResetFocus(bool doubledDuration)
        {
            var viewMana = ViewManager.GetInstance();
            var timespan = doubledDuration ? new Duration(TimeSpan.FromMilliseconds(SCamera.animationTimeMS * 1.5)) : SCamera.animationDuration;
            // background scale
            double bgRatio = Director.ScrMana.SCameraScale * SCamera.BackgroundDeepRatio;
            Storyboard storyScaleBg = new Storyboard();
            DoubleAnimation doubleAniScaleXBg = new DoubleAnimation(bgRatio, 1, timespan);
            DoubleAnimation doubleAniScaleYBg = new DoubleAnimation(bgRatio, 1, timespan);
            doubleAniScaleXBg.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYBg.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYBg, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleBg.Children.Add(doubleAniScaleXBg);
            storyScaleBg.Children.Add(doubleAniScaleYBg);
            storyScaleBg.Duration = timespan;
            // character scale
            Storyboard storyScaleCs = new Storyboard();
            DoubleAnimation doubleAniScaleXCs = new DoubleAnimation(Director.ScrMana.SCameraScale, 1, timespan);
            DoubleAnimation doubleAniScaleYCs = new DoubleAnimation(Director.ScrMana.SCameraScale, 1, timespan);
            doubleAniScaleXCs.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYCs.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYCs, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScaleCs.Children.Add(doubleAniScaleXCs);
            storyScaleCs.Children.Add(doubleAniScaleYCs);
            storyScaleCs.Duration = timespan;
            // picture scale
            double picRatio = Director.ScrMana.SCameraScale * SCamera.PictureDeepRatio;
            Storyboard storyScalePic = new Storyboard();
            DoubleAnimation doubleAniScaleXPic = new DoubleAnimation(picRatio, 1, timespan);
            DoubleAnimation doubleAniScaleYPic = new DoubleAnimation(picRatio, 1, timespan);
            doubleAniScaleXPic.DecelerationRatio = SCamera.DecelerateRatio;
            doubleAniScaleYPic.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniScaleXPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTarget(doubleAniScaleYPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniScaleXPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleYPic, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            storyScalePic.Children.Add(doubleAniScaleXPic);
            storyScalePic.Children.Add(doubleAniScaleYPic);
            storyScalePic.Duration = timespan;
            // background translate
            Storyboard storyTransBg = new Storyboard();
            double BgfromX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            double BgfromY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            DoubleAnimation doubleAniLeftBg = new DoubleAnimation(BgfromX, 0, timespan);
            doubleAniLeftBg.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniLeftBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniLeftBg, new PropertyPath(Canvas.LeftProperty));
            storyTransBg.Children.Add(doubleAniLeftBg);
            DoubleAnimation doubleAniTopBg = new DoubleAnimation(BgfromY, 0, timespan);
            doubleAniTopBg.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniTopBg, viewMana.GetViewport(ViewportType.VTBackground).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniTopBg, new PropertyPath(Canvas.TopProperty));
            storyTransBg.Children.Add(doubleAniTopBg);
            storyTransBg.Duration = timespan;
            // character translate
            Storyboard storyTransCs = new Storyboard();
            double CsfromX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            double CsfromY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            DoubleAnimation doubleAniLeftCs = new DoubleAnimation(CsfromX, 0, timespan);
            doubleAniLeftCs.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniLeftCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniLeftCs, new PropertyPath(Canvas.LeftProperty));
            storyTransCs.Children.Add(doubleAniLeftCs);
            DoubleAnimation doubleAniTopCs = new DoubleAnimation(CsfromY, 0, timespan);
            doubleAniTopCs.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniTopCs, viewMana.GetViewport(ViewportType.VTCharacterStand).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniTopCs, new PropertyPath(Canvas.TopProperty));
            storyTransCs.Children.Add(doubleAniTopCs);
            storyTransCs.Duration = timespan;
            // picture translate
            Storyboard storyTransPic = new Storyboard();
            double PicfromX = Canvas.GetLeft(viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            double PicfromY = Canvas.GetTop(viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            DoubleAnimation doubleAniLeftPic = new DoubleAnimation(PicfromX, 0, timespan);
            doubleAniLeftPic.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniLeftPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniLeftPic, new PropertyPath(Canvas.LeftProperty));
            storyTransPic.Children.Add(doubleAniLeftPic);
            DoubleAnimation doubleAniTopPic = new DoubleAnimation(PicfromY, 0, timespan);
            doubleAniTopPic.DecelerationRatio = SCamera.DecelerateRatio;
            Storyboard.SetTarget(doubleAniTopPic, viewMana.GetViewport(ViewportType.VTPictures).ViewboxBinding);
            Storyboard.SetTargetProperty(doubleAniTopPic, new PropertyPath(Canvas.TopProperty));
            storyTransPic.Children.Add(doubleAniTopPic);
            storyTransPic.Duration = timespan;
            // process callback
            storyScaleBg.FillBehavior = FillBehavior.Stop;
            storyScaleCs.FillBehavior = FillBehavior.Stop;
            storyScalePic.FillBehavior = FillBehavior.Stop;
            storyTransBg.FillBehavior = FillBehavior.Stop;
            storyTransCs.FillBehavior = FillBehavior.Stop;
            storyTransPic.FillBehavior = FillBehavior.Stop;
            storyScaleBg.Completed += StoryScaleBg_Reset_Completed;
            storyScaleCs.Completed += StoryScaleCs_Reset_Completed;
            storyScalePic.Completed += StoryScalePic_Reset_Completed;
            storyTransBg.Completed += StoryTransBg_Reset_Completed;
            storyTransCs.Completed += StoryTransCs_Reset_Completed;
            storyTransPic.Completed += StoryTransPic_Reset_Completed;
            // start dash
            storyTransBg.Begin();
            storyTransCs.Begin();
            storyTransPic.Begin();
            storyScaleCs.Begin();
            storyScaleBg.Begin();
            storyScalePic.Begin();
            // reset viewport
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            Director.ScrMana.SCameraCenterCol = 0;
            Director.ScrMana.SCameraCenterRow = GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            Director.ScrMana.SCameraScale = 1.0;
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
        /// 布置场景结束并准备进入时调用此方法以准备动画
        /// </summary>
        public static void PreviewEnterScene()
        {
            SCamera.FocusOn(GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2, 0, 1.8, true);
        }

        /// <summary>
        /// 进入场景并做默认拉长焦距效果
        /// </summary>
        public static void PostEnterScene()
        {
            SCamera.ResumeBlackFrame();
            SCamera.ResetFocus(true);
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
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(0, 1, SCamera.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = SCamera.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 1;
            };
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
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(masker.Opacity, 0, SCamera.animationDuration);
            doubleAniOpacity.DecelerationRatio = 0.8;
            Storyboard.SetTarget(doubleAniOpacity, masker);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(UIElement.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = SCamera.animationDuration;
            story.FillBehavior = FillBehavior.Stop;
            story.Completed += (sender, args) =>
            {
                masker.Opacity = 0;
                masker.Visibility = Visibility.Hidden;
            };
            story.Begin();
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
            SCamera.BackgroundDeepRatio = 0.75;
            SCamera.PictureDeepRatio = 1.2;
            // 尺度初始化
            Director.ScrMana.SCameraScale = 1.0;
            Director.ScrMana.SCameraCenterCol = 0;
            Director.ScrMana.SCameraCenterRow = 2;
            Director.ScrMana.SCameraFocusCol = 0;
            Director.ScrMana.SCameraFocusRow = 2;
            // 提前计算所有中心点坐标，避免每次调用的重复计算
            SCamera.screenPointMap = new Point[GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT, GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT + 1];
            var ScrBlockWidth = (double)GlobalDataContext.GAME_WINDOW_WIDTH / (GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT - GlobalDataContext.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT * 2);
            var ScrBlockHeight = (double)GlobalDataContext.GAME_WINDOW_HEIGHT / GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT;
            var beginX = 0.0 - GlobalDataContext.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT * ScrBlockWidth + ScrBlockWidth / 2.0;
            var beginY = ScrBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                // 变量j从1开始，为0预留位置，0是预留给屏幕横向的中央
                for (int j = 1; j <= GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    SCamera.screenPointMap[i, j] = new Point(beginX + (j - 1) * ScrBlockWidth, beginY + i * ScrBlockHeight);
                }
            }
            for (int i = 0; i < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                SCamera.screenPointMap[i, 0] = new Point(GlobalDataContext.GAME_WINDOW_WIDTH / 2.0, beginY + i * ScrBlockHeight);
            }
            SCamera.characterPointMap = new Point[GlobalDataContext.GAME_SCAMERA_CSTAND_ROWCOUNT];
            var CstBlockHeight = (double)GlobalDataContext.GAME_SCAMERA_CSTAND_HEIGHT / GlobalDataContext.GAME_SCAMERA_CSTAND_ROWCOUNT;
            var beginCstY = 0.0 - ((double)GlobalDataContext.GAME_SCAMERA_CSTAND_HEIGHT / 2.0) + CstBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContext.GAME_SCAMERA_CSTAND_ROWCOUNT; i++)
            {
                SCamera.characterPointMap[i] = new Point(0, beginCstY + i * CstBlockHeight);
            }
            // 计算标准距离字典
            SCamera.manhattanDistanceMap = new Dictionary<Point, Dictionary<Point, Point>>();
            for (int i = 0; i < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                for (int j = 0; j <= GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    var pt = SCamera.screenPointMap[i, j];
                    if (SCamera.manhattanDistanceMap.ContainsKey(pt) == false)
                    {
                        SCamera.manhattanDistanceMap[pt] = new Dictionary<Point, Point>();
                    }
                    for (int m = 0; m < GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT; m++)
                    {
                        for (int n = 0; n <= GlobalDataContext.GAME_SCAMERA_SCR_COLCOUNT; n++)
                        {
                            var rhsPt = SCamera.screenPointMap[m, n];
                            SCamera.manhattanDistanceMap[pt][rhsPt] = SCamera.GetManhattanDistance(pt, rhsPt, 1.0);
                        }
                    }
                }
            }
            // 计算法向量
            var centerPoint = SCamera.GetScreenCoordination(GlobalDataContext.GAME_SCAMERA_SCR_ROWCOUNT / 2, 0);
            SCamera.NormalVector = new Vector(centerPoint.X, centerPoint.Y);
        }

        /// <summary>
        /// 获得法向量
        /// </summary>
        /// <param name="ratio">当前画布的倍率，-1为当前倍率</param>
        /// <returns>指定倍率下的法线向量</returns>
        public static Vector GetNormalVector(double ratio = -1)
        {
            return ratio != -1 ? SCamera.NormalVector * ratio : SCamera.NormalVector * Director.ScrMana.SCameraScale;
        }
        
        /// <summary>
        /// 获得法向量点
        /// </summary>
        /// <param name="ratio">当前画布的倍率，-1为当前倍率</param>
        /// <returns>指定倍率下的法线向量点</returns>
        public static Point GetNormalPoint(double ratio = -1)
        {
            var nv = SCamera.GetNormalVector(ratio);
            return new Point(nv.X, nv.Y);
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
        /// 动画回调：还原镜头前景层缩放
        /// </summary>
        private static void StoryScalePic_Reset_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleX = 1.0;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).ScaleY = 1.0;
            scaleTransformer.CenterX = GlobalDataContext.GAME_WINDOW_WIDTH / 2.0;
            scaleTransformer.CenterY = GlobalDataContext.GAME_WINDOW_HEIGHT / 2.0;
        }

        /// <summary>
        /// 动画回调：还原镜头立绘层缩放
        /// </summary>
        private static void StoryScaleCs_Reset_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleX = 1.0;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).ScaleY = 1.0;
            scaleTransformer.CenterX = GlobalDataContext.GAME_WINDOW_WIDTH / 2.0;
            scaleTransformer.CenterY = GlobalDataContext.GAME_WINDOW_HEIGHT / 2.0;
        }

        /// <summary>
        /// 动画回调：还原镜头背景层缩放
        /// </summary>
        private static void StoryScaleBg_Reset_Completed(object sender, EventArgs e)
        {
            var aniGroup = (TransformGroup)ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding.RenderTransform;
            var scaleTransformer = (ScaleTransform)aniGroup.Children[1];
            scaleTransformer.ScaleX = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleX = 1.0;
            scaleTransformer.ScaleY = Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).ScaleY = 1.0;
            scaleTransformer.CenterX = GlobalDataContext.GAME_WINDOW_WIDTH / 2.0;
            scaleTransformer.CenterY = GlobalDataContext.GAME_WINDOW_HEIGHT / 2.0;
        }

        /// <summary>
        /// 动画回调：还原镜头前景层移动
        /// </summary>
        private static void StoryTransPic_Reset_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTPictures).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Left = 0);
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTPictures).Top = 0);
        }

        /// <summary>
        /// 动画回调：还原镜头立绘层移动
        /// </summary>
        private static void StoryTransCs_Reset_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTCharacterStand).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Left = 0);
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTCharacterStand).Top = 0);
        }

        /// <summary>
        /// 动画回调：还原镜头背景层移动
        /// </summary>
        private static void StoryTransBg_Reset_Completed(object sender, EventArgs e)
        {
            var vb = ViewManager.GetInstance().GetViewport(ViewportType.VTBackground).ViewboxBinding;
            Canvas.SetLeft(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Left = 0);
            Canvas.SetTop(vb, Director.ScrMana.GetViewboxDescriptor(ViewportType.VTBackground).Top = 0);
        }

        /// <summary>
        /// 获取法向量
        /// </summary>
        public static Vector NormalVector
        {
            get;
            private set;
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
                SCamera.animationDuration = TimeSpan.FromMilliseconds(SCamera.animationTimeMS = value);
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
        /// 屏幕分块中心的标准距离字典
        /// </summary>
        private static Dictionary<Point, Dictionary<Point, Point>> manhattanDistanceMap;
    }
}
