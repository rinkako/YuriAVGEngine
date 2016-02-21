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

namespace Yuri.PlatformCore
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
            // 重绘精灵
            for (int i = 0; i < this.BackgroundSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.BackgroundSpriteVec, ResourceType.Background, this.scrMana.GetSpriteDescriptor(i, ResourceType.Background), false);
            }
            for (int i = 0; i < this.CharacterStandSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.CharacterStandSpriteVec, ResourceType.Stand, this.scrMana.GetSpriteDescriptor(i, ResourceType.Stand), false);
            }
            for (int i = 0; i < this.PictureSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.PictureSpriteVec, ResourceType.Pictures, this.scrMana.GetSpriteDescriptor(i, ResourceType.Pictures), false);
            }
            // 重绘文字层
            for (int i = 0; i < this.MessageLayerVec.Count; i++)
            {
                this.ReDrawMessageLayer(i, this.scrMana.GetMsgLayerDescriptor(i), false);
            }
            // 重绘按钮
            for (int i = 0; i < this.ButtonLayerVec.Count; i++)
            {
                this.ReDrawButton(i, this.scrMana.GetButtonDescriptor(i));
            }
            // 重绘选择项
            for (int i = 0; i < this.BranchButtonVec.Count; i++)
            {
                this.ReDrawBranchButton(i, this.scrMana.GetBranchButtonDescriptor(i));
            }
        }

        /// <summary>
        /// 将描述子转化为相应前端项目并显示到前端
        /// </summary>
        /// <param name="id">控件id</param>
        /// <param name="rType">资源类型</param>
        public void Draw(int id, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    this.ReDrawSprite(id, this.BackgroundSpriteVec, ResourceType.Background, this.scrMana.GetSpriteDescriptor(id, ResourceType.Background), true);
                    break;
                case ResourceType.Stand:
                    this.ReDrawSprite(id, this.CharacterStandSpriteVec, ResourceType.Stand, this.scrMana.GetSpriteDescriptor(id, ResourceType.Stand), true);
                    break;
                case ResourceType.Pictures:
                    this.ReDrawSprite(id, this.PictureSpriteVec, ResourceType.Pictures, this.scrMana.GetSpriteDescriptor(id, ResourceType.Pictures), true);
                    break;
                case ResourceType.MessageLayerBackground:
                    this.ReDrawMessageLayer(id, this.scrMana.GetMsgLayerDescriptor(id), true);
                    break;
                case ResourceType.Button:
                    this.ReDrawButton(id, this.scrMana.GetButtonDescriptor(id));
                    break;
                case ResourceType.BranchButton:
                    this.ReDrawBranchButton(id, this.scrMana.GetBranchButtonDescriptor(id));
                    break;
            }
        }
        
        /// <summary>
        /// 重绘精灵
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="vector">精灵所在向量</param>
        /// <param name="rType">资源类型</param>
        /// <param name="descriptor">精灵描述子</param>
        /// <param name="forceReload">是否强制重新载入资源文件</param>
        private void ReDrawSprite(int id, List<YuriSprite> vector, ResourceType rType, SpriteDescriptor descriptor, bool forceReload)
        {
            YuriSprite sprite = vector[id], newSprite = null;
            // 强制重新载入或资源名称不同时重新加载资源文件
            if (sprite == null ||
                sprite.resourceName != descriptor.resourceName ||
                forceReload)
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetBackground(descriptor.resourceName, descriptor.cutRect);
                        break;
                    case ResourceType.Stand:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetCharacterStand(descriptor.resourceName, descriptor.cutRect);
                        break;
                    case ResourceType.Pictures:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetPicture(descriptor.resourceName, descriptor.cutRect);
                        break;
                }
            }
            // 重绘精灵
            this.RemoveSprite(sprite);
            this.DrawSprite(newSprite, descriptor);
        }

        /// <summary>
        /// 重绘文字层
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="descriptor">文字层描述子</param>
        /// <param name="forceReload">是否强制重新载入背景图资源文件</param>
        private void ReDrawMessageLayer(int id, MessageLayerDescriptor descriptor, bool forceReload)
        {
            MessageLayer msglay = this.MessageLayerVec[id];
            if (msglay == null ||
                msglay.backgroundSprite.resourceName != descriptor.BackgroundResourceName ||
                forceReload)
            {
                YuriSprite bgSprite = ResourceManager.GetInstance().GetPicture(descriptor.BackgroundResourceName, new Int32Rect(-1, 0, 0, 0));
                MessageLayer newLayer = new MessageLayer();
                newLayer.backgroundSprite = bgSprite;
                newLayer.Id = id;
                this.MessageLayerVec[id] = msglay = newLayer;
            }
            // 重绘文本层
            this.RemoveMessageLayer(msglay);
            this.DrawMessageLayer(msglay, descriptor);
        }

        /// <summary>
        /// 重绘按钮
        /// </summary>
        /// <param name="id">按钮id</param>
        /// <param name="descriptor">按钮描述子</param>
        private void ReDrawButton(int id, SpriteButtonDescriptor descriptor)
        {
            SpriteButton oldButton = this.ButtonLayerVec[id];
            SpriteButton sbutton = new SpriteButton(id);
            sbutton.ImageNormal = descriptor.normalDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.normalDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOver = descriptor.overDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.overDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOn = descriptor.onDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.onDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            this.ButtonLayerVec[id] = sbutton;
            // 重绘
            this.RemoveButton(oldButton);
            this.DrawButton(sbutton, descriptor);
        }

        /// <summary>
        /// 重绘选择支
        /// </summary>
        /// <param name="id">选择支id</param>
        /// <param name="descriptor">选择支描述子</param>
        private void ReDrawBranchButton(int id, BranchButtonDescriptor descriptor)
        {
            BranchButton oldButton = this.BranchButtonVec[id];
            BranchButton sbutton = new BranchButton(id);
            sbutton.ImageNormal = descriptor.normalDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.normalDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOver = descriptor.overDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.overDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOn = descriptor.onDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.onDescriptor.resourceName, new Int32Rect(-1, 0, 0, 0));
            this.BranchButtonVec[id] = sbutton;
            // 重绘
            this.RemoveBranchButton(oldButton);
            this.DrawBranchButton(sbutton, descriptor);
        }

        /// <summary>
        /// 为主窗体描绘一个精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        /// <param name="descriptor">精灵描述子</param>
        private void DrawSprite(YuriSprite sprite, SpriteDescriptor descriptor)
        {
            Image spriteImage = new Image();
            BitmapImage bmp = sprite.myImage;
            spriteImage.Width = bmp.PixelWidth;
            spriteImage.Height = bmp.PixelHeight;
            spriteImage.Source = bmp;
            spriteImage.Opacity = descriptor.Opacity;
            sprite.cutRect = descriptor.cutRect;
            sprite.displayBinding = spriteImage;
            sprite.anchor = descriptor.anchorType;
            Canvas.SetLeft(spriteImage, descriptor.X);
            Canvas.SetTop(spriteImage, descriptor.Y);
            Canvas.SetZIndex(spriteImage, descriptor.Z);
            spriteImage.Visibility = Visibility.Visible;
            this.view.BO_MainGrid.Children.Add(spriteImage);
            sprite.InitAnimationRenderTransform();
            SpriteAnimation.RotateToAnimation(sprite, TimeSpan.FromMilliseconds(0), descriptor.Angle, 0);
            SpriteAnimation.ScaleToAnimation(sprite, TimeSpan.FromMilliseconds(0), descriptor.ScaleX, descriptor.ScaleY, 0, 0);
        }

        /// <summary>
        /// 为主窗体描绘一个文字层
        /// </summary>
        /// <param name="msglay">文字层</param>
        /// <param name="descriptor">文字层描述子</param>
        private void DrawMessageLayer(MessageLayer msglay, MessageLayerDescriptor descriptor)
        {
            TextBlock msgBlock = new TextBlock();
            msglay.displayBinding = msgBlock;
            if (msglay.backgroundSprite != null && msglay.backgroundSprite.myImage != null)
            {
                ImageBrush ib = new ImageBrush(msglay.backgroundSprite.myImage);
                ib.Stretch = Stretch.None;
                ib.AlignmentX = AlignmentX.Left;
                ib.AlignmentY = AlignmentY.Top;
                msgBlock.Background = ib;
            }
            msglay.Width = descriptor.Width;
            msglay.Height = descriptor.Height;
            msglay.Opacity = descriptor.Opacity;
            msglay.Padding = descriptor.Padding;
            msglay.LineHeight = descriptor.LineHeight;
            msglay.HorizontalAlignment = descriptor.HorizonAlign;
            msglay.VerticalAlignment = descriptor.VertiAlign;
            msglay.FontColor = descriptor.FontColor;
            msglay.FontSize = descriptor.FontSize;
            msglay.FontName = descriptor.FontName;
            msglay.FontShadow = descriptor.FontShadow;
            msglay.displayBinding.TextWrapping = TextWrapping.Wrap;
            msglay.displayBinding.TextAlignment = TextAlignment.Left;
            Canvas.SetLeft(msgBlock, descriptor.X);
            Canvas.SetTop(msgBlock, descriptor.Y);
            Canvas.SetZIndex(msgBlock, descriptor.Z);
            msglay.Visibility = descriptor.Visible ? Visibility.Visible : Visibility.Hidden;
            this.view.BO_MainGrid.Children.Add(msgBlock);
        }

        /// <summary>
        /// 为主窗体描绘一个按钮
        /// </summary>
        /// <param name="sbutton">按钮实例</param>
        /// <param name="descriptor">按钮描述子</param>
        private void DrawButton(SpriteButton sbutton, SpriteButtonDescriptor descriptor)
        {
            Image buttonImage = new Image();
            BitmapImage bmp = sbutton.ImageNormal.myImage;
            buttonImage.Width = bmp.PixelWidth;
            buttonImage.Height = bmp.PixelHeight;
            buttonImage.Source = bmp;
            buttonImage.Opacity = descriptor.Opacity;
            sbutton.displayBinding = buttonImage;
            sbutton.Eternal = descriptor.Eternal;
            sbutton.Enable = descriptor.Enable;
            sbutton.ntr = new Interrupt()
            {
                detail = "ButtonNTRInterrupt",
                interruptSA = null,
                type = InterruptType.ButtonJump,
                returnTarget = descriptor.jumpLabel,
            };
            Canvas.SetLeft(buttonImage, descriptor.X);
            Canvas.SetTop(buttonImage, descriptor.Y);
            Canvas.SetZIndex(buttonImage, descriptor.Z);
            buttonImage.Visibility = Visibility.Visible;
            buttonImage.MouseDown += sbutton.MouseOnHandler;
            buttonImage.MouseEnter += sbutton.MouseEnterHandler;
            buttonImage.MouseLeave += sbutton.MouseLeaveHandler;
            buttonImage.MouseUp += sbutton.MouseUpHandler;
            this.view.BO_MainGrid.Children.Add(buttonImage);
            sbutton.InitAnimationRenderTransform();
        }

        /// <summary>
        /// 为主窗体描绘一个选择支
        /// </summary>
        /// <param name="sbutton">按钮实例</param>
        /// <param name="descriptor">按钮描述子</param>
        private void DrawBranchButton(BranchButton bbutton, BranchButtonDescriptor descriptor)
        {
            TextBlock buttonTextView = new TextBlock();
            BitmapImage bmp = bbutton.ImageNormal.myImage;
            buttonTextView.Width = bmp.PixelWidth;
            buttonTextView.Height = bmp.PixelHeight;
            ImageBrush ib = new ImageBrush(bmp);
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            ib.TileMode = TileMode.None;
            ib.Stretch = Stretch.Fill;
            buttonTextView.FontSize = GlobalDataContainer.GAME_BRANCH_FONTSIZE;
            buttonTextView.Foreground = new SolidColorBrush(GlobalDataContainer.GAME_BRANCH_FONTCOLOR);
            buttonTextView.FontFamily = new FontFamily(GlobalDataContainer.GAME_BRANCH_FONTNAME);
            buttonTextView.TextAlignment = TextAlignment.Center;
            buttonTextView.Padding = new Thickness(0, GlobalDataContainer.GAME_BRANCH_TOPPAD, 0, 0);
            buttonTextView.Background = ib;
            bbutton.displayBinding = buttonTextView;
            bbutton.Eternal = false;
            bbutton.Enable = true;
            bbutton.Text = descriptor.Text;
            bbutton.ntr = new Interrupt()
            {
                detail = "BranchButtonNTRInterrupt",
                interruptSA = null,
                type = InterruptType.ButtonJump,
                returnTarget = descriptor.JumpTarget,
            };
            Canvas.SetLeft(buttonTextView, descriptor.X);
            Canvas.SetTop(buttonTextView, descriptor.Y);
            Canvas.SetZIndex(buttonTextView, descriptor.Z);
            buttonTextView.Visibility = Visibility.Visible;
            buttonTextView.MouseDown += bbutton.MouseOnHandler;
            buttonTextView.MouseEnter += bbutton.MouseEnterHandler;
            buttonTextView.MouseLeave += bbutton.MouseLeaveHandler;
            buttonTextView.MouseUp += bbutton.MouseUpHandler;
            this.view.BO_MainGrid.Children.Add(buttonTextView);
            bbutton.InitAnimationRenderTransform();
        }

        /// <summary>
        /// 将指定类型的所有项目从画面移除
        /// </summary>
        public void RemoveView(ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Button:
                    for (int bi = 0; bi < this.ButtonLayerVec.Count; bi++)
                    {
                        if (this.ButtonLayerVec[bi] != null)
                        {
                            this.RemoveButton(bi);
                        }
                    }
                    break;
                case ResourceType.Background:
                    for (int bi = 0; bi < this.BackgroundSpriteVec.Count; bi++)
                    {
                        if (this.BackgroundSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Background);
                        }
                    }
                    break;
                case ResourceType.Stand:
                    for (int bi = 0; bi < this.CharacterStandSpriteVec.Count; bi++)
                    {
                        if (this.CharacterStandSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Stand);
                        }
                    }
                    break;
                case ResourceType.Pictures:
                    for (int bi = 0; bi < this.PictureSpriteVec.Count; bi++)
                    {
                        if (this.PictureSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Pictures);
                        }
                    }
                    break;
                case ResourceType.MessageLayerBackground:
                    for (int bi = 0; bi < this.MessageLayerVec.Count; bi++)
                    {
                        if (this.MessageLayerVec[bi] != null)
                        {
                            this.RemoveMessageLayer(bi);
                        }
                    }
                    break;
                case ResourceType.BranchButton:
                    for (int bi = 0; bi < this.BranchButtonVec.Count; bi++)
                    {
                        if (this.BranchButtonVec[bi] != null)
                        {
                            this.RemoveBranchButton(bi);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 将指定精灵从画面移除并释放
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="rType">类型</param>
        public void RemoveSprite(int id, ResourceType rType)
        {
            this.scrMana.RemoveSprite(id, rType);
            YuriSprite removeOne = null;
            switch (rType)
            {
                case ResourceType.Background:
                    removeOne = this.BackgroundSpriteVec[id];
                    this.BackgroundSpriteVec[id] = null;
                    break;
                case ResourceType.Stand:
                    removeOne = this.CharacterStandSpriteVec[id];
                    this.CharacterStandSpriteVec[id] = null;
                    break;
                case ResourceType.Pictures:
                    removeOne = this.PictureSpriteVec[id];
                    this.PictureSpriteVec[id] = null;
                    break;
            }
            this.RemoveSprite(removeOne);
        }

        /// <summary>
        /// 将指定文字层从画面移除并释放
        /// </summary>
        /// <param name="id">文字层id</param>
        public void RemoveMessageLayer(int id)
        {
            this.scrMana.RemoveMsgLayer(id);
            MessageLayer removeOne = this.MessageLayerVec[id];
            this.MessageLayerVec[id] = null;
            this.RemoveMessageLayer(removeOne);
        }

        /// <summary>
        /// 将指定按钮从画面移除并释放
        /// </summary>
        /// <param name="id">按钮id</param>
        public void RemoveButton(int id)
        {
            this.scrMana.RemoveButton(id);
            SpriteButton removeOne = this.ButtonLayerVec[id];
            this.ButtonLayerVec[id] = null;
            this.RemoveButton(removeOne);
        }

        /// <summary>
        /// 将指定选择支从画面移除并释放
        /// </summary>
        /// <param name="id">选择支id</param>
        public void RemoveBranchButton(int id)
        {
            this.scrMana.RemoveBranchButton(id);
            BranchButton removeOne = this.BranchButtonVec[id];
            this.BranchButtonVec[id] = null;
            this.RemoveBranchButton(removeOne);
        }

        /// <summary>
        /// 将精灵从画面移除
        /// </summary>
        /// <param name="sprite">精灵实例</param>
        private void RemoveSprite(YuriSprite sprite)
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
        /// 将文字层从画面移除
        /// </summary>
        /// <param name="msglay">文字层实例</param>
        private void RemoveMessageLayer(MessageLayer msglay)
        {
            if (msglay != null)
            {
                TextBlock msglayView = msglay.displayBinding;
                if (msglayView != null && this.view.BO_MainGrid.Children.Contains(msglayView))
                {
                    this.view.BO_MainGrid.Children.Remove(msglayView);
                }
                msglay.displayBinding = null;
            }
        }

        /// <summary>
        /// 将按钮从画面移除
        /// </summary>
        /// <param name="sbutton">按钮实例</param>
        private void RemoveButton(SpriteButton sbutton)
        {
            if (sbutton != null)
            {
                Image buttonView = sbutton.displayBinding;
                if (buttonView != null && this.view.BO_MainGrid.Children.Contains(buttonView))
                {
                    this.view.BO_MainGrid.Children.Remove(buttonView);
                    if (sbutton.ImageNormal != null)
                    {
                        sbutton.ImageNormal.displayBinding = null;
                    }
                    if (sbutton.ImageMouseOver != null)
                    {
                        sbutton.ImageMouseOver.displayBinding = null;
                    }
                    if (sbutton.ImageMouseOn != null)
                    {
                        sbutton.ImageMouseOn.displayBinding = null;
                    }
                }
                sbutton.displayBinding = null;
            }
        }

        /// <summary>
        /// 将选择支从画面移除
        /// </summary>
        /// <param name="bbutton">选择支实例</param>
        public void RemoveBranchButton(BranchButton bbutton)
        {
            if (bbutton != null)
            {
                var bbView = bbutton.displayBinding;
                if (bbView != null && this.view.BO_MainGrid.Children.Contains(bbView))
                {
                    this.view.BO_MainGrid.Children.Remove(bbView);
                }
                bbutton.displayBinding = null;
            }
        }

        /// <summary>
        /// 获取画面上的精灵实例
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="rType">资源类型</param>
        /// <returns>精灵实例</returns>
        public YuriSprite GetSprite(int id, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    return this.BackgroundSpriteVec[id];
                case ResourceType.Stand:
                    return this.CharacterStandSpriteVec[id];
                default:
                    return this.PictureSpriteVec[id];
            }
        }

        /// <summary>
        /// 获取画面上的文字层实例
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <returns>文字层实例</returns>
        public MessageLayer GetMessageLayer(int id)
        {
            return this.MessageLayerVec[id];
        }

        /// <summary>
        /// 获取画面上的按钮实例
        /// </summary>
        /// <param name="id">按钮id</param>
        /// <returns>按钮实例</returns>
        public SpriteButton GetSpriteButton(int id)
        {
            return this.ButtonLayerVec[id];
        }

        /// <summary>
        /// 获取画面上的按钮实例
        /// </summary>
        /// <param name="id">选择支id</param>
        /// <returns>选择支实例</returns>
        public BranchButton GetBranchButton(int id)
        {
            return this.BranchButtonVec[id];
        }

        /// <summary>
        /// 为视窗管理器设置主窗体的引用
        /// </summary>
        /// <param name="mw">主窗体</param>
        public void SetMainWndReference(MainWindow mw)
        {
            this.view = mw;
        }

        /// <summary>
        /// 初始化文字层实例
        /// </summary>
        public void InitMessageLayer()
        {
            for (int i = 0; i < GlobalDataContainer.GAME_MESSAGELAYER_COUNT; i++)
            {
                this.ReDrawMessageLayer(i, this.scrMana.GetMsgLayerDescriptor(i), true);
            }
        }

        /// <summary>
        /// 背景精灵向量
        /// </summary>
        private List<YuriSprite> BackgroundSpriteVec;
        
        /// <summary>
        /// 立绘精灵向量
        /// </summary>
        private List<YuriSprite> CharacterStandSpriteVec;
        
        /// <summary>
        /// 图片精灵向量
        /// </summary>
        private List<YuriSprite> PictureSpriteVec;

        /// <summary>
        /// 文字层向量
        /// </summary>
        private List<MessageLayer> MessageLayerVec;

        /// <summary>
        /// 按钮层向量
        /// </summary>
        private List<SpriteButton> ButtonLayerVec;

        /// <summary>
        /// 选择支按钮向量
        /// </summary>
        private List<BranchButton> BranchButtonVec;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ViewManager()
        {
            this.BackgroundSpriteVec = new List<YuriSprite>();
            this.CharacterStandSpriteVec = new List<YuriSprite>();
            this.PictureSpriteVec = new List<YuriSprite>();
            this.MessageLayerVec = new List<MessageLayer>();
            this.BranchButtonVec = new List<BranchButton>();
            this.ButtonLayerVec = new List<SpriteButton>();
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
            for (int i = 0; i < GlobalDataContainer.GAME_BUTTON_COUNT; i++)
            {
                this.ButtonLayerVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_BRANCH_COUNT; i++)
            {
                this.BranchButtonVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_MESSAGELAYER_COUNT; i++)
            {
                this.MessageLayerVec.Add(null);
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
