using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 为精灵类提供静态的动画方法
    /// </summary>
    internal static class SpriteAnimation
    {
        /// <summary>
        /// 在笛卡尔平面上移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromX">起始X</param>
        /// <param name="toX">目标X</param>
        /// <param name="fromY">起始Y</param>
        /// <param name="toY">目标Y</param>
        /// <param name="accX">加速度X</param>
        /// <param name="accY">加速度Y</param>
        public static void XYMoveAnimation(YuriSprite sprite, Duration duration, double fromX, double toX, double fromY, double toY, double accX, double accY)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniLeft = new DoubleAnimation(fromX, toX - sprite.DisplayWidth / 2.0, duration);
            DoubleAnimation doubleAniTop = new DoubleAnimation(fromY, toY - sprite.DisplayHeight / 2.0, duration);
            doubleAniLeft.AccelerationRatio = accX;
            doubleAniTop.AccelerationRatio = accY;
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniLeft, ViewManager.GetInstance().GetTransitionBox());
                Storyboard.SetTarget(doubleAniTop, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniLeft, sprite.DisplayBinding);
                Storyboard.SetTarget(doubleAniTop, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(doubleAniTop, new PropertyPath(Canvas.TopProperty));
            story.Children.Add(doubleAniLeft);
            story.Children.Add(doubleAniTop);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 在笛卡尔平面上水平方向移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromX">起始X</param>
        /// <param name="toX">目标X</param>
        /// <param name="accX">加速度X</param>
        public static void XMoveAnimation(YuriSprite sprite, Duration duration, double fromX, double toX, double accX)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniLeft = new DoubleAnimation(fromX, toX - sprite.DisplayWidth / 2.0, duration);
            if (accX >= 0)
            {
                doubleAniLeft.AccelerationRatio = accX;
            }
            else
            {
                doubleAniLeft.DecelerationRatio = -accX;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniLeft, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniLeft, sprite.DisplayBinding);

            }
            Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
            story.Children.Add(doubleAniLeft);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 在笛卡尔平面上竖直移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromY">起始Y</param>
        /// <param name="toY">目标Y</param>
        /// <param name="accY">加速度Y</param>
        public static void YMoveAnimation(YuriSprite sprite, Duration duration, double fromY, double toY, double accY)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniTop = new DoubleAnimation(fromY, toY - sprite.DisplayHeight / 2.0, duration);
            if (accY >= 0)
            {
                doubleAniTop.AccelerationRatio = accY;
            }
            else
            {
                doubleAniTop.DecelerationRatio = -accY;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniTop, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniTop, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(doubleAniTop, new PropertyPath(Canvas.TopProperty));
            story.Children.Add(doubleAniTop);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 在层次深度上移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromZ">起始Z</param>
        /// <param name="toZ">目标Z</param>
        /// <param name="accZ">加速度Z</param>
        public static void ZMoveAnimation(YuriSprite sprite, Duration duration, int fromZ, int toZ, double accZ)
        {
            Storyboard story = new Storyboard();
            Int32Animation int32AniZ = new Int32Animation(fromZ, toZ, duration);
            if (accZ >= 0)
            {
                int32AniZ.AccelerationRatio = accZ;
            }
            else
            {
                int32AniZ.DecelerationRatio = -accZ;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(int32AniZ, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(int32AniZ, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(int32AniZ, new PropertyPath(Canvas.ZIndexProperty));
            story.Children.Add(int32AniZ);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 在笛卡尔平面上关于锚点放缩精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromScaleX">起始横向比例</param>
        /// <param name="toScaleX">目标横向比例</param>
        /// <param name="fromScaleY">起始纵向比例</param>
        /// <param name="toScaleY">目标纵向比例</param>
        /// <param name="accX">横向加速度</param>
        /// <param name="accY">纵向加速度</param>
        public static void ScaleAnimation(YuriSprite sprite, Duration duration, double fromScaleX, double toScaleX, double fromScaleY, double toScaleY, double accX, double accY)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniScaleX = new DoubleAnimation(fromScaleX, toScaleX, duration);
            DoubleAnimation doubleAniScaleY = new DoubleAnimation(fromScaleY, toScaleY, duration);
            if (accX >= 0)
            {
                doubleAniScaleX.AccelerationRatio = accX;
            }
            else
            {
                doubleAniScaleX.DecelerationRatio = -accX;
            }
            if (accY >= 0)
            {
                doubleAniScaleY.AccelerationRatio = accY;
            }
            else
            {
                doubleAniScaleY.DecelerationRatio = -accY;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniScaleX, ViewManager.GetInstance().GetTransitionBox());
                Storyboard.SetTarget(doubleAniScaleY, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniScaleX, sprite.DisplayBinding);
                Storyboard.SetTarget(doubleAniScaleY, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            story.Children.Add(doubleAniScaleX);
            story.Children.Add(doubleAniScaleY);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 变更精灵的不透明度
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromOpacity">起始不透明度</param>
        /// <param name="toOpacity">目标不透明度</param>
        /// <param name="acc">加速度</param>
        public static void OpacityAnimation(YuriSprite sprite, Duration duration, double fromOpacity, double toOpacity, double acc)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(fromOpacity, toOpacity, duration);
            if (acc >= 0)
            {
                doubleAniOpacity.AccelerationRatio = acc;
            }
            else
            {
                doubleAniOpacity.DecelerationRatio = -acc;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniOpacity, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniOpacity, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(Image.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 在笛卡尔平面上关于锚点旋转精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromTheta">起始角度</param>
        /// <param name="toTheta">目标角度</param>
        /// <param name="acc">加速度</param>
        public static void RotateAnimation(YuriSprite sprite, Duration duration, double fromTheta, double toTheta, double acc)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniRotate = new DoubleAnimation(fromTheta, toTheta, duration);
            if (acc >= 0)
            {
                doubleAniRotate.AccelerationRatio = acc;
            }
            else
            {
                doubleAniRotate.DecelerationRatio = -acc;
            }
            if (sprite.Descriptor.ResourceType == ResourceType.Background)
            {
                Storyboard.SetTarget(doubleAniRotate, ViewManager.GetInstance().GetTransitionBox());
            }
            else
            {
                Storyboard.SetTarget(doubleAniRotate, sprite.DisplayBinding);
            }
            Storyboard.SetTargetProperty(doubleAniRotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            story.Children.Add(doubleAniRotate);
            story.Duration = duration;
            if (duration.TimeSpan.TotalMilliseconds != 0)
            {
                story.Completed += story_Completed;
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
            }
            story.Begin();
        }

        /// <summary>
        /// 依据差分在笛卡尔平面上平移精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaX">差分X</param>
        /// <param name="deltaY">差分Y</param>
        /// <param name="accX">加速度X</param>
        /// <param name="accY">加速度Y</param>
        public static void XYMoveAnimation(YuriSprite sprite, Duration duration, double deltaX, double deltaY, double accX = 0, double accY = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.XYMoveAnimation(sprite, duration, sprite.DisplayX, sprite.DisplayX + deltaX, sprite.DisplayY, sprite.DisplayY + deltaY, accX, accY);
        }

        /// <summary>
        /// 依据差分在层次深度上移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaZ">差分值</param>
        /// <param name="accZ">加速度Z</param>
        public static void ZMoveAnimation(YuriSprite sprite, Duration duration, int deltaZ, double accZ = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.ZMoveAnimation(sprite, duration, sprite.DisplayZ, sprite.DisplayZ + deltaZ, accZ);
        }

        /// <summary>
        /// 依据差分在笛卡尔平面上关于锚点放缩精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaScaleX">横向比例差分</param>
        /// <param name="deltaScaleY">纵向比例差分</param>
        /// <param name="accX">横向加速度</param>
        /// <param name="accY">纵向加速度</param>
        public static void ScaleAnimation(YuriSprite sprite, Duration duration, double deltaScaleX, double deltaScaleY, double accX = 0, double accY = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            ScaleTransform scaler = ((TransformGroup)(sprite.DisplayBinding.RenderTransform)).Children[1] as ScaleTransform;
            double curScaleX = scaler.ScaleX;
            double curScaleY = scaler.ScaleY;
            SpriteAnimation.ScaleAnimation(sprite, duration, curScaleX, curScaleX + deltaScaleX, curScaleY, curScaleY + deltaScaleY, accX, accY);
        }

        /// <summary>
        /// 依据差分变更精灵的不透明度
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaOpacity">差分值</param>
        /// <param name="acc">加速度</param>
        public static void OpacityAnimation(YuriSprite sprite, Duration duration, double deltaOpacity, double acc = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.OpacityAnimation(sprite, duration, sprite.DisplayOpacity, sprite.DisplayOpacity + deltaOpacity, acc);
        }

        /// <summary>
        /// 依据差分在笛卡尔平面上关于锚点旋转精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaTheta">差分</param>
        /// <param name="acc">加速度</param>
        public static void RotateAnimation(YuriSprite sprite, Duration duration, double deltaTheta, double acc = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            RotateTransform rotater = ((TransformGroup)(sprite.DisplayBinding.RenderTransform)).Children[2] as RotateTransform;
            double curAngle = rotater.Angle;
            SpriteAnimation.RotateAnimation(sprite, duration, curAngle, curAngle + deltaTheta, acc);
        }

        /// <summary>
        /// 笛卡尔平面上平移精灵到目标点
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toX">目标X</param>
        /// <param name="toY">目标Y</param>
        /// <param name="accX">加速度X</param>
        /// <param name="accY">加速度Y</param>
        public static void XYMoveToAnimation(YuriSprite sprite, Duration duration, double toX, double toY, double accX = 0, double accY = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.XYMoveAnimation(sprite, duration, sprite.DisplayX, toX, sprite.DisplayY, toY, accX, accY);
        }

        /// <summary>
        /// 笛卡尔平面上水平移动精灵到目标点
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toX">目标X</param>
        /// <param name="accX">加速度X</param>
        public static void XMoveToAnimation(YuriSprite sprite, Duration duration, double toX, double accX = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.XMoveAnimation(sprite, duration, sprite.DisplayX, toX, accX);
        }

        /// <summary>
        /// 笛卡尔平面上竖直移动精灵到目标点
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toY">目标X</param>
        /// <param name="accY">加速度X</param>
        public static void YMoveToAnimation(YuriSprite sprite, Duration duration, double toY, double accY = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.YMoveAnimation(sprite, duration, sprite.DisplayY, toY, accY);
        }

        /// <summary>
        /// 在层次深度上移动精灵到目标深度值
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toZ">目标Z</param>
        /// <param name="accZ">加速度Z</param>
        public static void ZMoveToAnimation(YuriSprite sprite, Duration duration, int toZ, double accZ = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.ZMoveAnimation(sprite, duration, sprite.DisplayZ, toZ, accZ);
        }

        /// <summary>
        /// 在笛卡尔平面上关于锚点放缩精灵到目标比例
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toScaleX">横向目标比例</param>
        /// <param name="toScaleY">纵向目标比例</param>
        /// <param name="accX">横向加速度</param>
        /// <param name="accY">纵向加速度</param>
        public static void ScaleToAnimation(YuriSprite sprite, Duration duration, double toScaleX, double toScaleY, double accX = 0, double accY = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            ScaleTransform scaler = ((TransformGroup)(sprite.DisplayBinding.RenderTransform)).Children[1] as ScaleTransform;
            SpriteAnimation.ScaleAnimation(sprite, duration, scaler.ScaleX, toScaleX, scaler.ScaleY, toScaleY, accX, accY);
        }

        /// <summary>
        /// 变更精灵的不透明度到目标值
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toOpacity">目标不透明度</param>
        /// <param name="acc">加速度</param>
        public static void OpacityToAnimation(YuriSprite sprite, Duration duration, double toOpacity, double acc = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            SpriteAnimation.OpacityAnimation(sprite, duration, sprite.DisplayOpacity, toOpacity, acc);
        }

        /// <summary>
        /// 在笛卡尔平面上关于锚点旋转精灵到目标角度
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toTheta">目标角度</param>
        /// <param name="acc">加速度</param>
        public static void RotateToAnimation(YuriSprite sprite, Duration duration, double toTheta, double acc = 0)
        {
            if (sprite.DisplayBinding == null) { return; }
            RotateTransform rotater = ((TransformGroup)(sprite.DisplayBinding.RenderTransform)).Children[2] as RotateTransform;
            SpriteAnimation.RotateAnimation(sprite, duration, rotater.Angle, toTheta, acc);
        }

        /// <summary>
        /// 为精灵的指定依赖属性作用一个双精度动画
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromValue">起始值</param>
        /// <param name="toValue">目标值</param>
        /// <param name="acc">加速度</param>
        /// <param name="propath">依赖链</param>
        public static void PropertyAnimation(YuriSprite sprite, Duration duration, double fromValue, double toValue, double acc, PropertyPath propath)
        {
            if (sprite.DisplayBinding == null) { return; }
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAni = new DoubleAnimation(fromValue, toValue, duration);
            if (acc >= 0)
            {
                doubleAni.AccelerationRatio = acc;
            }
            else
            {
                doubleAni.DecelerationRatio = -acc;
            }
            Storyboard.SetTarget(doubleAni, sprite.DisplayBinding);
            Storyboard.SetTargetProperty(doubleAni, propath);
            story.Children.Add(doubleAni);
            story.Begin();
        }

        /// <summary>
        /// 在笛卡尔平面上让精灵上下循环跳动
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="jumpDelta">下跳幅度</param>
        /// <param name="acc">加速度</param>
        public static void UpDownRepeatAnimation(YuriSprite sprite, Duration duration, double jumpDelta, double acc)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(0, jumpDelta, duration);
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            if (acc >= 0)
            {
                da.AccelerationRatio = acc;
            }
            else
            {
                da.DecelerationRatio = -acc;
            }
            Storyboard.SetTarget(da, sprite.DisplayBinding);
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
                Image.RenderTransformProperty,
                TranslateTransform.YProperty,
            };
            Storyboard.SetTargetProperty(da, new PropertyPath("(0).(1)", propertyChain));
            sb.Children.Add(da);
            sb.Begin();
        }

        /// <summary>
        /// 跳过所有动画
        /// </summary>
        public static void SkipAllAnimation()
        {
            foreach (var ani in SpriteAnimation.aniDict)
            {
                ani.Key.SkipToFill();
            }
        }

        /// <summary>
        /// 跳过指定精灵上的动画
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        public static void SkipAnimation(YuriSprite sprite)
        {
            if (sprite != null)
            {
                foreach (var ani in SpriteAnimation.aniDict)
                {
                    if (ani.Value == sprite)
                    {
                        ani.Key.SkipToFill();
                    }
                }
            }
        }

        /// <summary>
        /// 返回当前是否有精灵动画正在进行
        /// </summary>
        /// <returns>是否正在播放动画</returns>
        public static bool IsAnyAnimation()
        {
            return SpriteAnimation.aniDict.Count > 0;
        }

        /// <summary>
        /// 获取当前动画队列里最大的时间间隔值
        /// </summary>
        /// <returns>时间间隔</returns>
        public static TimeSpan FindMaxTimeSpan()
        {
            TimeSpan maxt = TimeSpan.FromMilliseconds(0);
            foreach (var st in SpriteAnimation.aniDict)
            {
                if (st.Key.Duration > maxt)
                {
                    maxt = st.Key.Duration.TimeSpan;
                }
            }
            return maxt;
        }

        /// <summary>
        /// 结束全部动画并清空字典
        /// </summary>
        public static void ClearAnimateWaitingDict()
        {
            foreach (var a in SpriteAnimation.aniDict)
            {
                a.Key.SkipToFill();
            }
            SpriteAnimation.aniDict.Clear();
        }

        /// <summary>
        /// 精灵动画完成回调
        /// </summary>
        private static void story_Completed(object sender, EventArgs e)
        {
            Queue<Storyboard> removeQueue = new Queue<Storyboard>();
            foreach (var ani in SpriteAnimation.aniDict)
            {
                if (ani.Key.GetCurrentTime() == ani.Key.Duration)
                {
                    ani.Value.AnimateCount--;
                    removeQueue.Enqueue(ani.Key);
                }
            }
            while (removeQueue.Count != 0)
            {
                SpriteAnimation.aniDict.Remove(removeQueue.Dequeue());
            }
        }

        /// <summary>
        /// 正在进行的动画字典
        /// </summary>
        private static Dictionary<Storyboard, YuriSprite> aniDict = new Dictionary<Storyboard, YuriSprite>();
    }
}
