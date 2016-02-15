using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
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
        public static void XYMoveAnimation(MySprite sprite, Duration duration, double fromX, double toX, double fromY, double toY, double accX, double accY)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniLeft = new DoubleAnimation(fromX, toX, duration);
            DoubleAnimation doubleAniTop = new DoubleAnimation(fromY, toY, duration);
            doubleAniLeft.AccelerationRatio = accX;
            doubleAniTop.AccelerationRatio = accY;
            Storyboard.SetTarget(doubleAniLeft, sprite.displayBinding);
            Storyboard.SetTarget(doubleAniTop, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(doubleAniTop, new PropertyPath(Canvas.TopProperty));
            story.Children.Add(doubleAniLeft);
            story.Children.Add(doubleAniTop);
            story.Duration = duration;
            story.Completed += story_Completed;
            sprite.AnimateCount++;
            int tt = story.GetHashCode();
            SpriteAnimation.aniDict[story] = sprite;
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
        public static void ZMoveAnimation(MySprite sprite, Duration duration, int fromZ, int toZ, double accZ)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniZ = new DoubleAnimation(fromZ, toZ, duration);
            doubleAniZ.AccelerationRatio = accZ;
            Storyboard.SetTarget(doubleAniZ, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniZ, new PropertyPath(Canvas.ZIndexProperty));
            story.Children.Add(doubleAniZ);
            story.Duration = duration;
            story.Completed += story_Completed;
            sprite.AnimateCount++;
            SpriteAnimation.aniDict[story] = sprite;
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
        public static void ScaleAnimation(MySprite sprite, Duration duration, double fromScaleX, double toScaleX, double fromScaleY, double toScaleY, double accX, double accY)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniScaleX = new DoubleAnimation(fromScaleX, toScaleX, duration);
            DoubleAnimation doubleAniScaleY = new DoubleAnimation(fromScaleY, toScaleY, duration);
            doubleAniScaleX.AccelerationRatio = accX;
            doubleAniScaleY.AccelerationRatio = accY;
            Storyboard.SetTarget(doubleAniScaleX, sprite.displayBinding);
            Storyboard.SetTarget(doubleAniScaleY, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
            story.Children.Add(doubleAniScaleX);
            story.Children.Add(doubleAniScaleY);
            story.Duration = duration;
            story.Completed += story_Completed;
            sprite.AnimateCount++;
            SpriteAnimation.aniDict[story] = sprite;
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
        public static void OpacityAnimation(MySprite sprite, Duration duration, double fromOpacity, double toOpacity, double acc)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniOpacity = new DoubleAnimation(fromOpacity, toOpacity, duration);
            doubleAniOpacity.AccelerationRatio = acc;
            Storyboard.SetTarget(doubleAniOpacity, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(Image.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
            story.Duration = duration;
            story.Completed += story_Completed;
            sprite.AnimateCount++;
            SpriteAnimation.aniDict[story] = sprite;
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
        public static void RotateAnimation(MySprite sprite, Duration duration, double fromTheta, double toTheta, double acc)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAniRotate = new DoubleAnimation(fromTheta, toTheta, duration);
            doubleAniRotate.AccelerationRatio = acc;
            Storyboard.SetTarget(doubleAniRotate, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniRotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            story.Children.Add(doubleAniRotate);
            story.Duration = duration;
            story.Completed += story_Completed;
            sprite.AnimateCount++;
            SpriteAnimation.aniDict[story] = sprite;
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
        public static void XYMoveAnimation(MySprite sprite, Duration duration, double deltaX, double deltaY, double accX = 0, double accY = 0)
        {
            SpriteAnimation.XYMoveAnimation(sprite, duration, sprite.displayX, sprite.displayX + deltaX, sprite.displayY, sprite.displayY + deltaY, accX, accY);
        }

        /// <summary>
        /// 依据差分在层次深度上移动精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaZ">差分值</param>
        /// <param name="accZ">加速度Z</param>
        public static void ZMoveAnimation(MySprite sprite, Duration duration, int deltaZ, double accZ = 0)
        {
            SpriteAnimation.ZMoveAnimation(sprite, duration, sprite.displayZ, sprite.displayZ + deltaZ, accZ);
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
        public static void ScaleAnimation(MySprite sprite, Duration duration, double deltaScaleX, double deltaScaleY, double accX = 0, double accY = 0)
        {
            ScaleTransform scaler = ((TransformGroup)(sprite.displayBinding.RenderTransform)).Children[1] as ScaleTransform;
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
        public static void OpacityAnimation(MySprite sprite, Duration duration, double deltaOpacity, double acc = 0)
        {
            SpriteAnimation.OpacityAnimation(sprite, duration, sprite.displayOpacity, sprite.displayOpacity + deltaOpacity, acc);
        }

        /// <summary>
        /// 依据差分在笛卡尔平面上关于锚点旋转精灵
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="deltaTheta">差分</param>
        /// <param name="acc">加速度</param>
        public static void RotateAnimation(MySprite sprite, Duration duration, double deltaTheta, double acc = 0)
        {
            RotateTransform rotater = ((TransformGroup)(sprite.displayBinding.RenderTransform)).Children[2] as RotateTransform;
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
        public static void XYMoveToAnimation(MySprite sprite, Duration duration, double toX, double toY, double accX = 0, double accY = 0)
        {
            SpriteAnimation.XYMoveAnimation(sprite, duration, sprite.displayX, toX, sprite.displayY, toY, accX, accY);
        }

        /// <summary>
        /// 在层次深度上移动精灵到目标深度值
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toZ">目标Z</param>
        /// <param name="accZ">加速度Z</param>
        public static void ZMoveToAnimation(MySprite sprite, Duration duration, int toZ, double accZ = 0)
        {
            SpriteAnimation.ZMoveAnimation(sprite, duration, sprite.displayZ, toZ, accZ);
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
        public static void ScaleToAnimation(MySprite sprite, Duration duration, double toScaleX, double toScaleY, double accX = 0, double accY = 0)
        {
            ScaleTransform scaler = ((TransformGroup)(sprite.displayBinding.RenderTransform)).Children[1] as ScaleTransform;
            SpriteAnimation.ScaleAnimation(sprite, duration, scaler.ScaleX, toScaleX, scaler.ScaleY, toScaleY, accX, accY);
        }

        /// <summary>
        /// 变更精灵的不透明度到目标值
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toOpacity">目标不透明度</param>
        /// <param name="acc">加速度</param>
        public static void OpacityToAnimation(MySprite sprite, Duration duration, double toOpacity, double acc = 0)
        {
            SpriteAnimation.OpacityAnimation(sprite, duration, sprite.displayOpacity, toOpacity, acc);
        }

        /// <summary>
        /// 在笛卡尔平面上关于锚点旋转精灵到目标角度
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        /// <param name="duration">动画时长</param>
        /// <param name="toTheta">目标角度</param>
        /// <param name="acc">加速度</param>
        public static void RotateToAnimation(MySprite sprite, Duration duration, double toTheta, double acc = 0)
        {
            RotateTransform rotater = ((TransformGroup)(sprite.displayBinding.RenderTransform)).Children[2] as RotateTransform;
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
        public static void PropertyAnimation(MySprite sprite, Duration duration, double fromValue, double toValue, double acc, PropertyPath propath)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAni = new DoubleAnimation(fromValue, toValue, duration);
            doubleAni.AccelerationRatio = acc;
            Storyboard.SetTarget(doubleAni, sprite.displayBinding);
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
        public static void UpDownRepeatAnimation(MySprite sprite, Duration duration, double jumpDelta, double acc)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(0, jumpDelta, duration);
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            da.AccelerationRatio = acc;
            Storyboard.SetTarget(da, sprite.displayBinding);
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
        public static void SkipAnimation(MySprite sprite)
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
        public static bool isAnyAnimation()
        {
            return SpriteAnimation.aniDict.Count > 0;
        }

        /// <summary>
        /// 获取当前动画队列里最大的时间间隔值
        /// </summary>
        /// <returns>时间间隔</returns>
        public static TimeSpan findMaxTimeSpan()
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
        /// 正在进行的动画字典
        /// </summary>
        private static Dictionary<Storyboard, MySprite> aniDict = new Dictionary<Storyboard, MySprite>();
    }
}
