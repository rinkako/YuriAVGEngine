using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 系统级通知管理器
    /// </summary>
    internal static class NotificationManager
    {
        /// <summary>
        /// 发布一个系统级通知
        /// </summary>
        /// <param name="label">通知的标题</param>
        /// <param name="detail">通知的具体内容</param>
        /// <param name="icoFilename">通知的图标</param>
        public static void Notify(string label, string detail, string icoFilename = "")
        {
            // 更新信息
            NotificationManager.labelUI.Text = label;
            NotificationManager.detailUI.Text = detail;
            if (icoFilename != "")
            {
                var icoRes = ResourceManager.GetInstance().GetPicture(icoFilename, new System.Windows.Int32Rect(-1, 0, 0, 0));
                if (icoRes != null)
                {
                    NotificationManager.IcoUI.Source = icoRes.SpriteBitmapImage;
                }
                else
                {
                    NotificationManager.IcoUI.Source = null;
                }
            }
            // 执行动画
            NotificationManager.ApplyNotificationAnimation();
        }

        /// <summary>
        /// 初始化通知管理器
        /// </summary>
        public static void Init()
        {
            var view = ViewPageManager.RetrievePage(GlobalDataContainer.FirstViewPage) as PageView.StagePage;
            NotificationManager.BoxUI = view.BO_Information;
            NotificationManager.IcoUI = view.BO_Information_Image;
            NotificationManager.labelUI = view.BO_Information_Name;
            NotificationManager.detailUI = view.BO_Information_Detail;
            TransformGroup aniGroup = new TransformGroup();
            TranslateTransform XYTransformer = new TranslateTransform();
            aniGroup.Children.Add(XYTransformer);
            NotificationManager.BoxUI.RenderTransform = aniGroup;
            var sp = ResourceManager.GetInstance().GetPicture("NotificationBox.png", new Int32Rect(-1, 0, 0, 0));
            NotificationManager.BoxUI.Background = new ImageBrush(sp.SpriteBitmapImage);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ApplyNotificationAnimation()
        {
            Storyboard story = new Storyboard();
            var beginRight = Canvas.GetRight(NotificationManager.BoxUI);
            var toRight = NotificationManager.DeltaBoxRight;
            DoubleAnimationUsingKeyFrames daukf_translate = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame k1_translate = new EasingDoubleKeyFrame(toRight, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS)));
            EasingDoubleKeyFrame k2_translate = new EasingDoubleKeyFrame(toRight, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS + PendingTimeMS)));
            EasingDoubleKeyFrame k3_translate = new EasingDoubleKeyFrame(beginRight, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS * 2 + PendingTimeMS)));
            k1_translate.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            k3_translate.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            daukf_translate.KeyFrames.Add(k1_translate);
            daukf_translate.KeyFrames.Add(k2_translate);
            daukf_translate.KeyFrames.Add(k3_translate);
            Storyboard.SetTarget(daukf_translate, NotificationManager.BoxUI);
            Storyboard.SetTargetProperty(daukf_translate, new PropertyPath(Canvas.RightProperty));
            DoubleAnimationUsingKeyFrames daukf_opacity = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame k1_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS)));
            EasingDoubleKeyFrame k2_opacity = new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS + PendingTimeMS)));
            EasingDoubleKeyFrame k3_opacity = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationTimeMS * 2 + PendingTimeMS)));
            daukf_opacity.KeyFrames.Add(k1_opacity);
            daukf_opacity.KeyFrames.Add(k2_opacity);
            daukf_opacity.KeyFrames.Add(k3_opacity);
            Storyboard.SetTarget(daukf_opacity, NotificationManager.BoxUI);
            Storyboard.SetTargetProperty(daukf_opacity, new PropertyPath(Canvas.OpacityProperty));
            story.Children.Add(daukf_translate);
            story.Children.Add(daukf_opacity);
            story.Begin();
        }

        /// <summary>
        /// 通知窗体容器的引用
        /// </summary>
        private static Grid BoxUI = null;

        /// <summary>
        /// 通知图标的引用
        /// </summary>
        private static Image IcoUI = null;

        /// <summary>
        /// 通知文本的引用
        /// </summary>
        private static TextBlock labelUI = null;

        /// <summary>
        /// 通知详情的引用
        /// </summary>
        private static TextBlock detailUI = null;

        /// <summary>
        /// 通知窗体距离右版边的距离
        /// </summary>
        public static double DeltaBoxRight = 50;

        /// <summary>
        /// 单趟动画的毫秒数
        /// </summary>
        public static int AnimationTimeMS = 1000;

        /// <summary>
        /// 消息显示的毫秒数
        /// </summary>
        public static int PendingTimeMS = 5000;
    }
}
