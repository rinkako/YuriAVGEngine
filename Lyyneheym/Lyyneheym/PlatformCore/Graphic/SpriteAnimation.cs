using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Yuri.PlatformCore.Graphic3D;

namespace Yuri.PlatformCore.Graphic
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.X = sprite.Descriptor.ToX;
                sprite.Descriptor.Y = sprite.Descriptor.ToY;
                Canvas.SetLeft(sprite.AnimationElement, sprite.Descriptor.ToX - sprite.AnchorX);
                Canvas.SetTop(sprite.AnimationElement, sprite.Descriptor.ToY - sprite.AnchorY);
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation doubleAniLeft = new DoubleAnimation(fromX, toX - sprite.AnchorX, duration);
                DoubleAnimation doubleAniTop = new DoubleAnimation(fromY, toY - sprite.AnchorY, duration);
                doubleAniLeft.AccelerationRatio = accX;
                doubleAniTop.AccelerationRatio = accY;
                if (sprite.Descriptor.ResourceType == ResourceType.Background)
                {
                    Storyboard.SetTarget(doubleAniLeft, ViewManager.GetInstance().GetTransitionBox());
                    Storyboard.SetTarget(doubleAniTop, ViewManager.GetInstance().GetTransitionBox());
                }
                else
                {
                    Storyboard.SetTarget(doubleAniLeft, sprite.AnimationElement);
                    Storyboard.SetTarget(doubleAniTop, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
                Storyboard.SetTargetProperty(doubleAniTop, new PropertyPath(Canvas.TopProperty));
                story.Children.Add(doubleAniLeft);
                story.Children.Add(doubleAniTop);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.X = sprite.Descriptor.ToX;
                    sprite.Descriptor.Y = sprite.Descriptor.ToY;
                    Canvas.SetLeft(sprite.AnimationElement, sprite.Descriptor.ToX - sprite.AnchorX);
                    Canvas.SetTop(sprite.AnimationElement, sprite.Descriptor.ToY - sprite.AnchorY);
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.X = sprite.Descriptor.ToX;
                Canvas.SetLeft(sprite.AnimationElement, sprite.Descriptor.ToX - sprite.AnchorX);
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation doubleAniLeft = new DoubleAnimation(fromX, toX - sprite.AnchorX, duration);
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
                    Storyboard.SetTarget(doubleAniLeft, sprite.AnimationElement);

                }
                Storyboard.SetTargetProperty(doubleAniLeft, new PropertyPath(Canvas.LeftProperty));
                story.Children.Add(doubleAniLeft);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.X = sprite.Descriptor.ToX;
                    Canvas.SetLeft(sprite.AnimationElement, sprite.Descriptor.ToX - sprite.AnchorX);
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
        }

        /// <summary>
        /// 在3D空间的X轴方向移动模型
        /// </summary>
        /// <param name="geom">模型实例</param>
        /// <param name="descriptor3D">模型的描述子</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromX">起始X</param>
        /// <param name="toX">目标X</param>
        /// <param name="accX">加速度X</param>
        public static void XMoveAnimation3D(GeometryModel3D geom, ModelDescriptor3D descriptor3D, Duration duration, double fromX, double toX, double accX)
        {
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                var translator = (geom.Transform as Transform3DGroup).Children.First(t => t is TranslateTransform3D) as TranslateTransform3D;
                translator.OffsetX = descriptor3D.OffsetX = descriptor3D.ToOffsetX;
            }
            else
            {
                DoubleAnimation cardAnimation = new DoubleAnimation
                {
                    From = fromX,
                    To = toX,
                    Duration = duration,
                    FillBehavior = FillBehavior.Stop
                };
                if (accX >= 0)
                {
                    cardAnimation.AccelerationRatio = accX;
                }
                else
                {
                    cardAnimation.DecelerationRatio = -accX;
                }
                var transform = (geom.Transform as Transform3DGroup).Children.First(t => t is TranslateTransform3D);
                var flagSb = new Storyboard { Name = "FlagSb_" + Guid.NewGuid() };
                SpriteAnimation.aniDict[flagSb] = null;
                cardAnimation.Completed += delegate
                {
                    (transform as TranslateTransform3D).OffsetX = descriptor3D.OffsetX = descriptor3D.ToOffsetX;
                    SpriteAnimation.aniDict.Remove(flagSb);
                };
                transform.BeginAnimation(TranslateTransform3D.OffsetXProperty, cardAnimation);
            }
        }

        /// <summary>
        /// 在3D空间的Y轴方向移动模型
        /// </summary>
        /// <param name="geom">模型实例</param>
        /// <param name="descriptor3D">模型的描述子</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromX">起始X</param>
        /// <param name="toX">目标X</param>
        /// <param name="accX">加速度X</param>
        public static void YMoveAnimation3D(GeometryModel3D geom, ModelDescriptor3D descriptor3D, Duration duration, double fromY, double toY, double accY)
        {
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                var translator = (geom.Transform as Transform3DGroup).Children.First(t => t is TranslateTransform3D) as TranslateTransform3D;
                translator.OffsetY = descriptor3D.OffsetY = descriptor3D.ToOffsetY;
            }
            else
            {
                DoubleAnimation cardAnimation = new DoubleAnimation
                {
                    From = fromY,
                    To = toY,
                    Duration = duration,
                    FillBehavior = FillBehavior.Stop
                };
                if (accY >= 0)
                {
                    cardAnimation.AccelerationRatio = accY;
                }
                else
                {
                    cardAnimation.DecelerationRatio = -accY;
                }
                var transform = (geom.Transform as Transform3DGroup).Children.First(t => t is TranslateTransform3D);
                var flagSb = new Storyboard { Name = "FlagSb_" + Guid.NewGuid() };
                SpriteAnimation.aniDict[flagSb] = null;
                cardAnimation.Completed += delegate
                {
                    (transform as TranslateTransform3D).OffsetY = descriptor3D.OffsetY = descriptor3D.ToOffsetY;
                    SpriteAnimation.aniDict.Remove(flagSb);
                };
                transform.BeginAnimation(TranslateTransform3D.OffsetYProperty, cardAnimation);
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.Y = sprite.Descriptor.ToY;
                Canvas.SetTop(sprite.AnimationElement, sprite.Descriptor.ToY - sprite.AnchorY);
            }
            else
            {
                Storyboard story = new Storyboard();
                DoubleAnimation doubleAniTop = new DoubleAnimation(fromY, toY - sprite.AnchorY, duration);
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
                    Storyboard.SetTarget(doubleAniTop, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(doubleAniTop, new PropertyPath(Canvas.TopProperty));
                story.Children.Add(doubleAniTop);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.Y = sprite.Descriptor.ToY;
                    Canvas.SetTop(sprite.AnimationElement, sprite.Descriptor.ToY - sprite.AnchorY);
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.Z = sprite.Descriptor.ToZ;
                Canvas.SetZIndex(sprite.AnimationElement, sprite.Descriptor.ToZ);
            }
            else
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
                    Storyboard.SetTarget(int32AniZ, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(int32AniZ, new PropertyPath(Canvas.ZIndexProperty));
                story.Children.Add(int32AniZ);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.Z = sprite.Descriptor.ToZ;
                    Canvas.SetZIndex(sprite.AnimationElement, sprite.Descriptor.ToZ);
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.ScaleX = sprite.Descriptor.ToScaleX;
                sprite.Descriptor.ScaleY = sprite.Descriptor.ToScaleY;
                if (sprite.ScaleTransformer != null)
                {
                    sprite.ScaleTransformer.ScaleX = sprite.Descriptor.ToScaleX;
                    sprite.ScaleTransformer.ScaleY = sprite.Descriptor.ToScaleY;
                }
            }
            else
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
                    Storyboard.SetTarget(doubleAniScaleX, sprite.AnimationElement);
                    Storyboard.SetTarget(doubleAniScaleY, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(doubleAniScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(doubleAniScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));
                story.Children.Add(doubleAniScaleX);
                story.Children.Add(doubleAniScaleY);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.ScaleX = sprite.Descriptor.ToScaleX;
                    sprite.Descriptor.ScaleY = sprite.Descriptor.ToScaleY;
                    sprite.ScaleTransformer.ScaleX = sprite.Descriptor.ToScaleX;
                    sprite.ScaleTransformer.ScaleY = sprite.Descriptor.ToScaleY;
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.Opacity = sprite.Descriptor.ToOpacity;
                sprite.AnimationElement.Opacity = sprite.Descriptor.ToOpacity;
            }
            else
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
                    Storyboard.SetTarget(doubleAniOpacity, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(doubleAniOpacity, new PropertyPath(Image.OpacityProperty));
                story.Children.Add(doubleAniOpacity);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.Opacity = sprite.Descriptor.ToOpacity;
                    sprite.AnimationElement.Opacity = sprite.Descriptor.ToOpacity;
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
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
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                sprite.Descriptor.Angle = sprite.Descriptor.ToAngle;
                sprite.RotateTransformer.Angle = sprite.Descriptor.ToAngle;
            }
            else
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
                    Storyboard.SetTarget(doubleAniRotate, sprite.AnimationElement);
                }
                Storyboard.SetTargetProperty(doubleAniRotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
                story.Children.Add(doubleAniRotate);
                story.Duration = duration;
                story.FillBehavior = FillBehavior.Stop;
                story.Completed += (sender, args) =>
                {
                    sprite.Descriptor.Angle = sprite.Descriptor.ToAngle;
                    sprite.RotateTransformer.Angle = sprite.Descriptor.ToAngle;
                };
                sprite.AnimateCount++;
                SpriteAnimation.aniDict[story] = sprite;
                story.Completed += (sender, args) =>
                {
                    sprite.AnimateCount--;
                    SpriteAnimation.aniDict.Remove(story);
                };
                story.Begin();
            }
        }

        /// <summary>
        /// 变更精灵的模糊度动画
        /// <para>这是一个特效互斥动画，多个特效互斥动画之间不叠加</para>
        /// </summary>
        /// <param name="sprite">精灵对象</param>
        /// <param name="duration">动画时长</param>
        /// <param name="fromRadius">起始模糊半径</param>
        /// <param name="toRadius">目标模糊半径</param>
        public static void BlurMutexAnimation(YuriSprite sprite, Duration duration, double fromRadius, double toRadius)
        {
            if (duration.TimeSpan.TotalMilliseconds == 0
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                BlurEffect m_BlurEffect = new BlurEffect();
                sprite.AnimationElement.Effect = m_BlurEffect;
                m_BlurEffect.RenderingBias = GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality
                    ? RenderingBias.Quality : RenderingBias.Performance;
                m_BlurEffect.Radius = toRadius;
                sprite.Descriptor.BlurRadius = toRadius;
            }
            else
            {
                BlurEffect m_BlurEffect = new BlurEffect();
                sprite.AnimationElement.Effect = m_BlurEffect;
                m_BlurEffect.RenderingBias = GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality
                    ? RenderingBias.Quality : RenderingBias.Performance;
                DoubleAnimation m_DA = new DoubleAnimation
                {
                    From = fromRadius,
                    To = toRadius,
                    Duration = duration,
                    EasingFunction = new CubicEase(),
                };
                sprite.AnimateCount++;
                var flagSb = new Storyboard { Name = "FlagSb_" + Guid.NewGuid() };
                SpriteAnimation.aniDict[flagSb] = sprite;
                m_DA.Completed += (sender, args) =>
                {
                    sprite.Descriptor.BlurRadius = sprite.Descriptor.ToBlurRadius;
                    m_BlurEffect.Radius = toRadius;
                    sprite.AnimateCount--;
                    aniDict.Remove(flagSb);
                };
                m_BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, m_DA);
            }
        }

        /// <summary>
        /// 变更精灵的投影效果动画
        /// <para>这是一个特效互斥动画，多个特效互斥动画之间不叠加</para>
        /// </summary>
        /// <param name="sprite">精灵对象</param>
        /// <param name="duration">动画时长</param>
        /// <param name="shadColor">投影颜色</param>
        /// <param name="shadOpacity">投影不透明度</param>
        /// <param name="fromRadius">起始模糊半径</param>
        /// <param name="toRadius">目标模糊半径</param>
        public static void ShadowingMutexAnimation(YuriSprite sprite, Duration duration, Color shadColor, double shadOpacity, double fromRadius, double toRadius)
        {
            if (duration.TimeSpan.TotalMilliseconds == 0 
                || GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.NoEffect)
            {
                DropShadowEffect m_DSEffect = new DropShadowEffect();
                sprite.AnimationElement.Effect = m_DSEffect;
                m_DSEffect.RenderingBias = GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality
                    ? RenderingBias.Quality : RenderingBias.Performance;
                m_DSEffect.BlurRadius = toRadius;
                m_DSEffect.Color = shadColor;
                m_DSEffect.Opacity = shadOpacity;
                sprite.Descriptor.ShadowRadius = toRadius;
            }
            else
            {
                DropShadowEffect m_DSEffect = new DropShadowEffect();
                sprite.AnimationElement.Effect = m_DSEffect;
                m_DSEffect.RenderingBias = GlobalConfigContext.GAME_PERFORMANCE_TYPE == GlobalConfigContext.PerformanceType.HighQuality
                    ? RenderingBias.Quality : RenderingBias.Performance;
                m_DSEffect.Color = shadColor;
                m_DSEffect.Opacity = shadOpacity;
                DoubleAnimation m_DA = new DoubleAnimation
                {
                    From = fromRadius,
                    To = toRadius,
                    Duration = duration,
                    EasingFunction = new CubicEase(),
                };
                sprite.AnimateCount++;
                var flagSb = new Storyboard { Name = "FlagSb_" + Guid.NewGuid() };
                SpriteAnimation.aniDict[flagSb] = sprite;
                m_DA.Completed += (sender, args) =>
                {
                    sprite.Descriptor.BlurRadius = sprite.Descriptor.ToBlurRadius;
                    m_DSEffect.BlurRadius = toRadius;
                    sprite.AnimateCount--;
                    aniDict.Remove(flagSb);
                };
                m_DSEffect.BeginAnimation(BlurEffect.RadiusProperty, m_DA);
            }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
            ScaleTransform scaler = ((TransformGroup)(sprite.AnimationElement.RenderTransform)).Children[1] as ScaleTransform;
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
            ScaleTransform scaler = ((TransformGroup)(sprite.AnimationElement.RenderTransform)).Children[1] as ScaleTransform;
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
            if (sprite.AnimationElement == null) { return; }
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
            if (sprite.AnimationElement == null) { return; }
            RotateTransform rotater = ((TransformGroup)(sprite.AnimationElement.RenderTransform)).Children[2] as RotateTransform;
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
            if (sprite.AnimationElement == null) { return; }
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
            Storyboard.SetTarget(doubleAni, sprite.AnimationElement);
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
            DoubleAnimation da = new DoubleAnimation(0, jumpDelta, duration)
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            if (acc >= 0)
            {
                da.AccelerationRatio = acc;
            }
            else
            {
                da.DecelerationRatio = -acc;
            }
            Storyboard.SetTarget(da, sprite.AnimationElement);
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
        public static bool IsAnyAnimation => SpriteAnimation.aniDict.Count > 0;

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
        /// 正在进行的动画字典
        /// </summary>
        private static readonly Dictionary<Storyboard, YuriSprite> aniDict = new Dictionary<Storyboard, YuriSprite>();
    }
}
