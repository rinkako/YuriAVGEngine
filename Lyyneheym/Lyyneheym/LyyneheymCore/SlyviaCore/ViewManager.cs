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
    /// 视窗管理器：负责将画面管理器的内容渲染为前端视图
    /// </summary>
    public class ViewManager
    {
        /// <summary>
        /// 重绘画面
        /// </summary>
        public void ReDraw()
        {
            for (int i = 0; i < this.BackgroundSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.BackgroundSpriteVec, ResourceType.Background, this.scrMana.GetDescriptor(i, ResourceType.Background), false);
            }
            for (int i = 0; i < this.CharacterStandSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.CharacterStandSpriteVec, ResourceType.Stand, this.scrMana.GetDescriptor(i, ResourceType.Stand), false);
            }
            for (int i = 0; i < this.PictureSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.PictureSpriteVec, ResourceType.Pictures, this.scrMana.GetDescriptor(i, ResourceType.Pictures), false);
            }
        }

        /// <summary>
        /// 将精灵描述子转化为精灵并显示到前端
        /// </summary>
        /// <param name="spriteId">精灵ID</param>
        /// <param name="rType">资源类型</param>
        public void Draw(int spriteId, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    this.ReDrawSprite(spriteId, this.BackgroundSpriteVec, ResourceType.Background, this.scrMana.GetDescriptor(spriteId, ResourceType.Background), true);
                    break;
                case ResourceType.Stand:
                    this.ReDrawSprite(spriteId, this.CharacterStandSpriteVec, ResourceType.Stand, this.scrMana.GetDescriptor(spriteId, ResourceType.Stand), true);
                    break;
                case ResourceType.Pictures:
                    this.ReDrawSprite(spriteId, this.PictureSpriteVec, ResourceType.Pictures, this.scrMana.GetDescriptor(spriteId, ResourceType.Pictures), true);
                    break;
            }
        }
        
        /// <summary>
        /// 重绘精灵
        /// </summary>
        /// <param name="spriteId">精灵ID</param>
        /// <param name="vector">精灵所在向量</param>
        /// <param name="rType">资源类型</param>
        /// <param name="descriptor">精灵描述子</param>
        /// <param name="forceReload">是否强制重新载入资源文件</param>
        private void ReDrawSprite(int spriteId, List<MySprite> vector, ResourceType rType, SpriteDescriptor descriptor, bool forceReload)
        {
            MySprite sprite = vector[spriteId];
            // 强制重新载入或资源名称不同时重新加载资源文件
            if (sprite == null ||
                sprite.resourceName != descriptor.resourceName ||
                forceReload)
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        vector[spriteId] = sprite = ResourceManager.GetInstance().GetBackground(descriptor.resourceName);
                        break;
                    case ResourceType.Stand:
                        vector[spriteId] = sprite = ResourceManager.GetInstance().GetCharacterStand(descriptor.resourceName);
                        break;
                    case ResourceType.Pictures:
                        vector[spriteId] = sprite = ResourceManager.GetInstance().GetPicture(descriptor.resourceName);
                        break;
                }
            }
            // 重绘精灵
            this.RemoveSprite(sprite);
            this.DrawSprite(sprite, descriptor);
        }

        /// <summary>
        /// 为主窗体描绘一个精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        /// <param name="descriptor">精灵描述子</param>
        private void DrawSprite(MySprite sprite, SpriteDescriptor descriptor)
        {
            Image spriteImage = new Image();
            BitmapImage bmp = sprite.myImage;
            spriteImage.Width = bmp.PixelWidth;
            spriteImage.Height = bmp.PixelHeight;
            spriteImage.Source = bmp;
            spriteImage.Opacity = descriptor.Opacity;
            sprite.displayBinding = spriteImage;
            sprite.anchor = descriptor.anchorType;
            Canvas.SetLeft(spriteImage, descriptor.X);
            Canvas.SetTop(spriteImage, descriptor.Y);
            Canvas.SetZIndex(spriteImage, descriptor.Z);
            SpriteAnimation.RotateAnimation(sprite, TimeSpan.FromMilliseconds(0), descriptor.Angle, 0);
            spriteImage.Visibility = Visibility.Visible;
            this.view.BO_MainGrid.Children.Add(spriteImage);
            sprite.InitAnimationRenderTransform();
        }

        /// <summary>
        /// 将指定精灵从画面溢出并释放
        /// </summary>
        /// <param name="spriteId">精灵ID</param>
        /// <param name="rType">类型</param>
        public void RemoveSprite(int spriteId, ResourceType rType)
        {
            this.scrMana.RemoveSprite(spriteId, rType);
            MySprite removeOne = null;
            switch (rType)
            {
                case ResourceType.Background:
                    removeOne = this.BackgroundSpriteVec[spriteId];
                    this.BackgroundSpriteVec[spriteId] = null;
                    break;
                case ResourceType.Stand:
                    removeOne = this.CharacterStandSpriteVec[spriteId];
                    this.CharacterStandSpriteVec[spriteId] = null;
                    break;
                case ResourceType.Pictures:
                    removeOne = this.PictureSpriteVec[spriteId];
                    this.PictureSpriteVec[spriteId] = null;
                    break;
            }
            this.RemoveSprite(removeOne);
        }

        /// <summary>
        /// 将精灵从画面移除
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        private void RemoveSprite(MySprite sprite)
        {
            if (sprite != null)
            {
                Image spriteView = sprite.displayBinding;
                if (spriteView != null && this.view.BO_MainGrid.Children.Contains(spriteView))
                {
                    this.view.BO_MainGrid.Children.Remove(spriteView);
                }
                sprite.displayBinding = null;
            }
        }

        /// <summary>
        /// 获取画面上的精灵实例
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="rType">资源类型</param>
        /// <returns>精灵实例</returns>
        public MySprite GetSprite(int id, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    return this.BackgroundSpriteVec[id];
                case ResourceType.Stand:
                    return this.BackgroundSpriteVec[id];
                default:
                    return this.PictureSpriteVec[id];
            }
        }

        /// <summary>
        /// 为视窗管理器设置主窗体的引用
        /// </summary>
        /// <param name="mw">主窗体</param>
        public void SetMainWndReference(MainWindow mw)
        {
            this.view = mw;
        }


        private List<MySprite> BackgroundSpriteVec;
        
        private List<MySprite> CharacterStandSpriteVec;
        
        private List<MySprite> PictureSpriteVec;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ViewManager()
        {
            this.BackgroundSpriteVec = new List<MySprite>();
            this.CharacterStandSpriteVec = new List<MySprite>();
            this.PictureSpriteVec = new List<MySprite>();
            for (int i = 0; i < GlobalDataContainer.GAME_BACKGROUND_COUNT; i++)
            {
                this.BackgroundSpriteVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_CHARACTERSTAND_COUNT; i++)
            {
                this.CharacterStandSpriteVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_IMAGELAYER_COUNT; i++)
            {
                this.PictureSpriteVec.Add(null);
            }
        }

        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>视窗管理器</returns>
        public static ViewManager GetInstance()
        {
            return ViewManager.synObject == null ? ViewManager.synObject = new ViewManager() : ViewManager.synObject;
        }

        /// <summary>
        /// 画面管理器
        /// </summary>
        private ScreenManager scrMana = ScreenManager.GetInstance();

        /// <summary>
        /// 主窗体引用
        /// </summary>
        private MainWindow view = null;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static ViewManager synObject = null;

    }
}
