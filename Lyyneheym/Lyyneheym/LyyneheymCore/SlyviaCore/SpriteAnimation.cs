using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            Storyboard.SetTarget(doubleAniOpacity, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(Image.OpacityProperty));
            story.Children.Add(doubleAniOpacity);
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
            Storyboard.SetTarget(doubleAniRotate, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAniRotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            story.Children.Add(doubleAniRotate);
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
        public static void RotateAnimation(MySprite sprite, Duration duration, double deltaTheta, double acc)
        {
            RotateTransform rotater = ((TransformGroup)(sprite.displayBinding.RenderTransform)).Children[2] as RotateTransform;
            double curAngle = rotater.Angle;
            SpriteAnimation.RotateAnimation(sprite, duration, curAngle, curAngle + deltaTheta, acc);
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
        public static void ApplyAnimation(MySprite sprite, Duration duration, double fromValue, double toValue, double acc, PropertyPath propath)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation doubleAni = new DoubleAnimation(fromValue, toValue, duration);
            doubleAni.AccelerationRatio = acc;
            Storyboard.SetTarget(doubleAni, sprite.displayBinding);
            Storyboard.SetTargetProperty(doubleAni, propath);
            story.Children.Add(doubleAni);
            story.Begin();
        }
    }
}
