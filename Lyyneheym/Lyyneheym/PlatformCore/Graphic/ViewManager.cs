using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Transitionals;
using Transitionals.Controls;
using Yuri.PageView;
using Yuri.PlatformCore.Graphic3D;
using Yuri.PlatformCore.VM;
using Yuri.Utils;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 视窗管理器：负责将画面管理器的内容渲染为前端2D/3D视图
    /// </summary>
    internal sealed class ViewManager
    {
        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>视窗管理器</returns>
        public static ViewManager GetInstance()
        {
            return ViewManager.synObject ?? (ViewManager.synObject = new ViewManager());
        }

        /// <summary>
        /// 重绘整个画面
        /// </summary>
        public void ReDraw()
        {
            // 先重绘可视化舞台
            if (ViewManager.Is3DStage)
            {
                var scale = SCamera3D.GetCameraScale(Director.ScrMana.SCameraScale);
                SCamera3D.FocusOn(Director.ScrMana.SCameraFocusRow, Director.ScrMana.SCameraFocusCol, scale, true);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    this.ReDrawViewport2D((ViewportType)i, Director.ScrMana.GetViewboxDescriptor((ViewportType)i));
                }
            }
            // 重绘背景
            for (int i = 0; i < this.backgroundSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.backgroundSpriteVec, ResourceType.Background, Director.ScrMana.GetSpriteDescriptor(i, ResourceType.Background), false);
            }
            // 重绘立绘
            for (int i = 0; i < this.characterStandSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.characterStandSpriteVec, ResourceType.Stand, Director.ScrMana.GetSpriteDescriptor(i, ResourceType.Stand), false);
            }
            // 重绘图片
            for (int i = 0; i < this.pictureSpriteVec.Count; i++)
            {
                this.ReDrawSprite(i, this.pictureSpriteVec, ResourceType.Pictures, Director.ScrMana.GetSpriteDescriptor(i, ResourceType.Pictures), false);
            }
            // 重绘文字层
            for (int i = 0; i < this.messageLayerVec.Count; i++)
            {
                this.ReDrawMessageLayer(i, Director.ScrMana.GetMsgLayerDescriptor(i), false);
            }
            // 重绘按钮
            for (int i = 0; i < this.buttonLayerVec.Count; i++)
            {
                this.ReDrawButton(i, Director.ScrMana.GetButtonDescriptor(i));
            }
            // 重绘选择项
            for (int i = 0; i < this.branchButtonVec.Count; i++)
            {
                this.ReDrawBranchButton(i, Director.ScrMana.GetBranchButtonDescriptor(i));
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
                    this.ReDrawSprite(id, this.backgroundSpriteVec, ResourceType.Background, Director.ScrMana.GetSpriteDescriptor(id, ResourceType.Background), true);
                    break;
                case ResourceType.Stand:
                    this.ReDrawSprite(id, this.characterStandSpriteVec, ResourceType.Stand, Director.ScrMana.GetSpriteDescriptor(id, ResourceType.Stand), true);
                    break;
                case ResourceType.Pictures:
                    this.ReDrawSprite(id, this.pictureSpriteVec, ResourceType.Pictures, Director.ScrMana.GetSpriteDescriptor(id, ResourceType.Pictures), true);
                    break;
                case ResourceType.MessageLayerBackground:
                    this.ReDrawMessageLayer(id, Director.ScrMana.GetMsgLayerDescriptor(id), true);
                    break;
                case ResourceType.Button:
                    this.ReDrawButton(id, Director.ScrMana.GetButtonDescriptor(id));
                    break;
                case ResourceType.BranchButton:
                    this.ReDrawBranchButton(id, Director.ScrMana.GetBranchButtonDescriptor(id));
                    break;
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
                    return this.backgroundSpriteVec[id];
                case ResourceType.Stand:
                    return this.characterStandSpriteVec[id];
                default:
                    return this.pictureSpriteVec[id];
            }
        }

        /// <summary>
        /// 获取画面上的文字层实例
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <returns>文字层实例</returns>
        public MessageLayer GetMessageLayer(int id) => this.messageLayerVec[id];

        /// <summary>
        /// 获取画面上的按钮实例
        /// </summary>
        /// <param name="id">按钮id</param>
        /// <returns>按钮实例</returns>
        public SpriteButton GetSpriteButton(int id) => this.buttonLayerVec[id];

        /// <summary>
        /// 获取画面上的按钮实例
        /// </summary>
        /// <param name="id">选择支id</param>
        /// <returns>选择支实例</returns>
        public BranchButton GetBranchButton(int id) => this.branchButtonVec[id];

        /// <summary>
        /// 获取画面上的2D视窗
        /// </summary>
        /// <param name="vt">视窗类型</param>
        /// <returns>视窗实例</returns>
        public Viewport2D GetViewport2D(ViewportType vt) => this.viewbox2dVec[(int)vt];

        /// <summary>
        /// 将指定类型的所有项目从画面移除
        /// </summary>
        public void RemoveView(ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Button:
                    for (int bi = 0; bi < this.buttonLayerVec.Count; bi++)
                    {
                        if (this.buttonLayerVec[bi] != null)
                        {
                            this.RemoveButton(bi);
                        }
                    }
                    return;
                case ResourceType.Background:
                    for (int bi = 0; bi < this.backgroundSpriteVec.Count; bi++)
                    {
                        if (this.backgroundSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Background);
                        }
                    }
                    return;
                case ResourceType.Stand:
                    for (int bi = 0; bi < this.characterStandSpriteVec.Count; bi++)
                    {
                        if (this.characterStandSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Stand);
                        }
                    }
                    return;
                case ResourceType.Pictures:
                    for (int bi = 0; bi < this.pictureSpriteVec.Count; bi++)
                    {
                        if (this.pictureSpriteVec[bi] != null)
                        {
                            this.RemoveSprite(bi, ResourceType.Pictures);
                        }
                    }
                    return;
                case ResourceType.MessageLayerBackground:
                    for (int bi = 0; bi < this.messageLayerVec.Count; bi++)
                    {
                        if (this.messageLayerVec[bi] != null)
                        {
                            this.RemoveMessageLayer(bi);
                        }
                    }
                    return;
                case ResourceType.BranchButton:
                    for (int bi = 0; bi < this.branchButtonVec.Count; bi++)
                    {
                        if (this.branchButtonVec[bi] != null)
                        {
                            this.RemoveBranchButton(bi);
                        }
                    }
                    return;
                default:
                    break;
            }
            for (int bi = 0; bi < this.buttonLayerVec.Count; bi++)
            {
                if (this.buttonLayerVec[bi] != null)
                {
                    this.RemoveButton(bi);
                }
            }
            for (int bi = 0; bi < this.backgroundSpriteVec.Count; bi++)
            {
                if (this.backgroundSpriteVec[bi] != null)
                {
                    this.RemoveSprite(bi, ResourceType.Background);
                }
            }
            for (int bi = 0; bi < this.characterStandSpriteVec.Count; bi++)
            {
                if (this.characterStandSpriteVec[bi] != null)
                {
                    this.RemoveSprite(bi, ResourceType.Stand);
                }
            }
            for (int bi = 0; bi < this.pictureSpriteVec.Count; bi++)
            {
                if (this.pictureSpriteVec[bi] != null)
                {
                    this.RemoveSprite(bi, ResourceType.Pictures);
                }
            }
            for (int bi = 0; bi < this.messageLayerVec.Count; bi++)
            {
                if (this.messageLayerVec[bi] != null)
                {
                    this.RemoveMessageLayer(bi);
                }
            }
            for (int bi = 0; bi < this.branchButtonVec.Count; bi++)
            {
                if (this.branchButtonVec[bi] != null)
                {
                    this.RemoveBranchButton(bi);
                }
            }
        }

        /// <summary>
        /// 将指定精灵从画面移除并释放
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="rType">类型</param>
        public void RemoveSprite(int id, ResourceType rType)
        {
            Director.ScrMana.RemoveSprite(id, rType);
            YuriSprite removeOne;
            switch (rType)
            {
                case ResourceType.Background:
                    if (ViewManager.Is3DStage)
                    {
                        removeOne = this.backgroundSpriteVec[id];
                        this.backgroundSpriteVec[id] = null;
                        this.RemoveSprite(ResourceType.Background, removeOne);
                        break;
                    }
                    else
                    {
                        // 交换前景和背景，为消除背景做准备
                        ScreenManager.GetInstance().Backlay();
                        // 执行过渡，消除背景
                        this.ApplyTransition("FadeTransition");
                    }
                    break;
                case ResourceType.Stand:
                    removeOne = this.characterStandSpriteVec[id];
                    this.characterStandSpriteVec[id] = null;
                    this.RemoveSprite(ResourceType.Stand, removeOne);
                    break;
                case ResourceType.Pictures:
                    removeOne = this.pictureSpriteVec[id];
                    this.pictureSpriteVec[id] = null;
                    this.RemoveSprite(ResourceType.Pictures, removeOne);
                    break;
            }
        }

        /// <summary>
        /// 将指定按钮从画面移除并释放
        /// </summary>
        /// <param name="id">按钮id</param>
        public void RemoveButton(int id)
        {
            Director.ScrMana.RemoveButton(id);
            SpriteButton removeOne = this.buttonLayerVec[id];
            this.buttonLayerVec[id] = null;
            this.RemoveButton(removeOne);
        }

        /// <summary>
        /// 初始化文字层实例
        /// </summary>
        public void InitMessageLayer()
        {
            for (int i = 0; i < GlobalConfigContext.GAME_MESSAGELAYER_COUNT; i++)
            {
                this.ReDrawMessageLayer(i, Director.ScrMana.GetMsgLayerDescriptor(i), true);
            }
        }

        /// <summary>
        /// 执行过渡
        /// </summary>
        /// <param name="transTypeName">过渡类型的名字</param>
        public void ApplyTransition(string transTypeName)
        {
            // 如果不是2D舞台就跳过
            if (ViewManager.Is3DStage) { return; }
            // 刷新精灵
            var backDesc = Director.ScrMana.GetSpriteDescriptor((int)BackgroundPage.Back, ResourceType.Background);
            var foreDesc = Director.ScrMana.GetSpriteDescriptor((int)BackgroundPage.Fore, ResourceType.Background);
            if (backDesc == null)
            {
                this.backgroundSpriteVec[0] = null;
            }
            if (foreDesc == null)
            {
                this.backgroundSpriteVec[1] = null;
            }
            // 获取过渡的类型
            Type transType = this.transitionTypes[0];
            foreach (var t in this.transitionTypes)
            {
                string[] nameItem = t.ToString().Split('.');
                if (nameItem[nameItem.Length - 1].ToLower() == transTypeName.ToLower())
                {
                    transType = t;
                    break;
                }
            }
            Transition transition = (Transition)Activator.CreateInstance(transType);
            // 处理真实的Backlay动作
            if (this.backgroundSpriteVec[(int)BackgroundPage.Back] != null &&
                this.backgroundSpriteVec[(int)BackgroundPage.Back].DisplayBinding != null)
            {
                this.backgroundSpriteVec[(int)BackgroundPage.Back].DisplayBinding.Visibility = Visibility.Visible;
            }
            CommonUtils.Swap<YuriSprite>(this.backgroundSpriteVec, (int)BackgroundPage.Fore, (int)BackgroundPage.Back);
            if (this.backgroundSpriteVec[(int)BackgroundPage.Fore] != null)
            {
                this.backgroundSpriteVec[(int)BackgroundPage.Fore].DisplayZ = (int)BackgroundPage.Fore + GlobalConfigContext.GAME_Z_BACKGROUND;
            }
            if (this.backgroundSpriteVec[(int)BackgroundPage.Back] != null)
            {
                this.backgroundSpriteVec[(int)BackgroundPage.Back].DisplayZ = (int)BackgroundPage.Back + GlobalConfigContext.GAME_Z_BACKGROUND;
            }
            // 交换前景和背景
            Director.ScrMana.Backlay();
            View2D.TransitionDS.ObjectInstance = transition;
            var viewBinder = this.backgroundSpriteVec[(int)BackgroundPage.Fore] == null ?
                null : this.backgroundSpriteVec[(int)BackgroundPage.Fore].DisplayBinding;
            var canvas = this.GetDrawingCanvas2D(ResourceType.Background);
            if (viewBinder != null && canvas.Children.Contains(viewBinder))
            {
                canvas.Children.Remove(viewBinder);
                Canvas.SetZIndex(View2D.TransitionBox, Canvas.GetZIndex(viewBinder));
            }
            // 执行过渡
            View2D.TransitionBox.TransitionEnded += TransitionEnded;
            View2D.TransitionBox.Content = viewBinder;
        }

        /// <summary>
        /// 获取主视窗上的过渡容器
        /// </summary>
        /// <returns>过渡容器引用</returns>
        public TransitionElement GetTransitionBox() => ViewManager.View2D.TransitionBox;

        /// <summary>
        /// 在过渡效果完成时触发
        /// </summary>
        private void TransitionEnded(object sender, TransitionEventArgs e)
        {
            // 恢复back层不可见
            if (backgroundSpriteVec[(int)BackgroundPage.Back]?.DisplayBinding != null)
            {
                this.backgroundSpriteVec[(int)BackgroundPage.Back].DisplayBinding.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 重绘视窗
        /// </summary>
        /// <param name="vt">视窗类型</param>
        /// <param name="descriptor">2D视窗重绘结果的描述子</param>
        private void ReDrawViewport2D(ViewportType vt, Viewport2DDescriptor descriptor)
        {
            // 取得前端对象
            Viewbox vb = this.viewbox2dVec[(int)vt].ViewboxBinding;
            // 重置
            if (vt == ViewportType.VTBackground)
            {
                var bindingBackgroundDescriptor = Director.ScrMana.GetSpriteDescriptor((int)BackgroundPage.Fore, ResourceType.Background);
                if (bindingBackgroundDescriptor != null)
                {
                    Canvas.SetLeft(vb, bindingBackgroundDescriptor.X - GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0);
                    Canvas.SetTop(vb, bindingBackgroundDescriptor.Y - GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0);
                    this.viewbox2dVec[(int)vt].ScaleTransformer.ScaleX = bindingBackgroundDescriptor.ScaleX;
                    this.viewbox2dVec[(int)vt].ScaleTransformer.ScaleY = bindingBackgroundDescriptor.ScaleY;
                }
            }
            else
            {
                Canvas.SetLeft(vb, descriptor.Left);
                Canvas.SetTop(vb, descriptor.Top);

                this.viewbox2dVec[(int)vt].ScaleTransformer.ScaleX = descriptor.ScaleX;
                this.viewbox2dVec[(int)vt].ScaleTransformer.ScaleY = descriptor.ScaleY;
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
            // 不需要重绘的情况
            if (descriptor == null)
            {
                if (vector[id] != null)
                {
                    switch (rType)
                    {
                        case ResourceType.Background:
                            this.RemoveSprite(id, ResourceType.Background);
                            break;
                        case ResourceType.Stand:
                            this.RemoveSprite(id, ResourceType.Stand);
                            break;
                        case ResourceType.Pictures:
                            this.RemoveSprite(id, ResourceType.Pictures);
                            break;
                    }
                }
                return;
            }
            YuriSprite sprite = vector[id], newSprite = null;
            // 强制重新载入或资源名称不同时重新加载资源文件
            if (sprite == null ||
                sprite.ResourceName != descriptor.ResourceName ||
                forceReload)
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetBackground(descriptor.ResourceName, descriptor.CutRect);
                        break;
                    case ResourceType.Stand:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetCharacterStand(descriptor.ResourceName, descriptor.CutRect);
                        break;
                    case ResourceType.Pictures:
                        vector[id] = newSprite = ResourceManager.GetInstance().GetPicture(descriptor.ResourceName, descriptor.CutRect);
                        break;
                }
            }
            else
            {
                newSprite = sprite;
            }
            // 重绘精灵
            this.RemoveSprite(rType, sprite);
            this.DrawSprite(newSprite, descriptor, rType, id);
        }

        /// <summary>
        /// 重绘文字层
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="descriptor">文字层描述子</param>
        /// <param name="forceReload">是否强制重新载入背景图资源文件</param>
        private void ReDrawMessageLayer(int id, MessageLayerDescriptor descriptor, bool forceReload)
        {
            // 不需要重绘的情况
            if (descriptor == null) { return; }
            MessageLayer msglay = this.messageLayerVec[id];
            if (msglay == null ||
                (msglay.BackgroundSprite != null && msglay.BackgroundSprite.ResourceName != descriptor.BackgroundResourceName) ||
                forceReload)
            {
                YuriSprite bgSprite = ResourceManager.GetInstance().GetPicture(descriptor.BackgroundResourceName, new Int32Rect(-1, 0, 0, 0));
                MessageLayer newLayer = new MessageLayer();
                newLayer.BackgroundSprite = bgSprite;
                newLayer.Id = id;
                this.messageLayerVec[id] = msglay = newLayer;
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
            // 不需要重绘的情况
            if (descriptor == null)
            {
                this.RemoveButton(this.buttonLayerVec[id]);
                return;
            }
            SpriteButton oldButton = this.buttonLayerVec[id];
            SpriteButton sbutton = new SpriteButton(id)
            {
                ImageNormal = descriptor.NormalDescriptor == null ? null
                        : ResourceManager.GetInstance() .GetPicture(descriptor.NormalDescriptor.ResourceName, ResourceManager.FullImageRect),
                ImageMouseOver = descriptor.OverDescriptor == null ? null
                        : ResourceManager.GetInstance() .GetPicture(descriptor.OverDescriptor.ResourceName, ResourceManager.FullImageRect),
                ImageMouseOn = descriptor.OnDescriptor == null ? null
                        : ResourceManager.GetInstance()  .GetPicture(descriptor.OnDescriptor.ResourceName, ResourceManager.FullImageRect)
            };
            sbutton.ImageNormal.Descriptor = descriptor.NormalDescriptor;
            sbutton.ImageMouseOver.Descriptor = descriptor.OverDescriptor;
            sbutton.ImageMouseOn.Descriptor = descriptor.OnDescriptor;
            this.buttonLayerVec[id] = sbutton;
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
            // 不需要重绘的情况
            if (descriptor == null)
            {
                this.RemoveBranchButton(this.branchButtonVec[id]);
                return;
            }
            BranchButton oldButton = this.branchButtonVec[id];
            BranchButton sbutton = new BranchButton(id);
            sbutton.ImageNormal = descriptor.NormalDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.NormalDescriptor.ResourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOver = descriptor.OverDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.OverDescriptor.ResourceName, new Int32Rect(-1, 0, 0, 0));
            sbutton.ImageMouseOn = descriptor.OnDescriptor == null ? null : ResourceManager.GetInstance().GetPicture(descriptor.OnDescriptor.ResourceName, new Int32Rect(-1, 0, 0, 0));
            this.branchButtonVec[id] = sbutton;
            // 重绘
            this.RemoveBranchButton(oldButton);
            this.DrawBranchButton(sbutton, descriptor);
        }

        /// <summary>
        /// 获取对象要描绘上去的2D画布
        /// </summary>
        /// <param name="rType">资源类型</param>
        /// <returns>画布的引用</returns>
        private Canvas GetDrawingCanvas2D(ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    return this.viewbox2dVec[(int)ViewportType.VTBackground].CanvasBinding;
                case ResourceType.Stand:
                    return this.viewbox2dVec[(int)ViewportType.VTCharacterStand].CanvasBinding;
                case ResourceType.Pictures:
                    return this.viewbox2dVec[(int)ViewportType.VTPictures].CanvasBinding;
                default:
                    return ViewManager.View2D.BO_MainGrid;
            }
        }
        
        /// <summary>
        /// 为主窗体描绘一个精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        /// <param name="descriptor">精灵描述子</param>
        /// <param name="rType">资源类型</param>
        /// <param name="idx">描述子在向量的下标</param>
        private void DrawSprite(YuriSprite sprite, SpriteDescriptor descriptor, ResourceType rType, int idx)
        {
            if (sprite == null) { return; }
            if (ViewManager.Is3DStage)
            {
                this.DrawSprite3D(sprite, descriptor, rType);
            }
            else
            {
                this.DrawSprite2D(sprite, descriptor, rType, idx);
            }
        }

        /// <summary>
        /// 在2D画布上描绘精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        /// <param name="descriptor">精灵描述子</param>
        /// <param name="rType">资源类型</param>
        /// <param name="idx">描述子在向量的下标</param>
        private void DrawSprite2D(YuriSprite sprite, SpriteDescriptor descriptor, ResourceType rType, int idx)
        {
            Image spriteImage = new Image();
            BitmapImage bmp = sprite.SpriteBitmapImage;
            spriteImage.Width = bmp.PixelWidth;
            spriteImage.Height = bmp.PixelHeight;
            spriteImage.Source = bmp;
            spriteImage.Opacity = descriptor.Opacity;
            sprite.CutRect = descriptor.CutRect;
            sprite.DisplayBinding = spriteImage;
            sprite.Anchor = descriptor.AnchorType;
            sprite.Descriptor = descriptor;
            Canvas.SetLeft(spriteImage, descriptor.X - bmp.PixelWidth / 2.0);
            Canvas.SetTop(spriteImage, descriptor.Y - bmp.PixelHeight / 2.0);
            Canvas.SetZIndex(spriteImage, descriptor.Z);
            // 此处不可以用descriptor的id去判断background元素的可见性，因为存在backlay
            spriteImage.Visibility = (rType == ResourceType.Background && idx == 0) ? Visibility.Hidden : Visibility.Visible;
            this.GetDrawingCanvas2D(rType).Children.Add(spriteImage);
            sprite.InitAnimationRenderTransform();
            if (rType != ResourceType.Background)
            {
                sprite.AnimationElement = sprite.DisplayBinding;
                descriptor.ToScaleX = descriptor.ScaleX;
                descriptor.ToScaleY = descriptor.ScaleY;
                SpriteAnimation.RotateToAnimation(sprite, TimeSpan.Zero, descriptor.Angle, 0);
                SpriteAnimation.ScaleToAnimation(sprite, TimeSpan.Zero, descriptor.ScaleX, descriptor.ScaleY, 0, 0);
            }
            else
            {
                sprite.AnimationElement = this.viewbox2dVec[(int)ViewportType.VTBackground].ViewboxBinding;
                // 如果前景渐变框是空就强制刷新她
                if (View2D.TransitionBox.Content == null && idx == 1)
                {
                    this.GetDrawingCanvas2D(rType).Children.Remove(spriteImage);
                    View2D.TransitionBox.Content = spriteImage;
                }
            }
        }

        /// <summary>
        /// 在3D模型组上描绘精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        /// <param name="descriptor">精灵描述子</param>
        /// <param name="rType">资源类型</param>
        private void DrawSprite3D(YuriSprite sprite, SpriteDescriptor descriptor, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    var bgModel = ViewManager.View3D.ST3D_Background_Fore;
                    var bgGeomtry = bgModel.Geometry as MeshGeometry3D;
                    if (bgGeomtry.Positions[0].Z != descriptor.Deepth3D)
                    {
                        List<Point3D> gPointList = new List<Point3D>();
                        foreach (var orgP in bgGeomtry.Positions)
                        {
                            Point3D np = new Point3D
                            {
                                X = orgP.X,
                                Y = orgP.Y,
                                Z = descriptor.Deepth3D
                            };
                            gPointList.Add(np);
                        }
                        bgGeomtry.Positions.Clear();
                        foreach (var nvP in gPointList)
                        {
                            bgGeomtry.Positions.Add(nvP);
                        }
                    }
                    var bgMaterial = bgModel.Material;
                    if (!(bgMaterial is DiffuseMaterial))
                    {
                        bgMaterial = bgModel.Material = new DiffuseMaterial();
                    }
                    var bgMaterialBrush = new ImageBrush(sprite.SpriteBitmapImage)
                    {
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        TileMode = TileMode.None
                    };
                    ((DiffuseMaterial)bgMaterial).Brush = bgMaterialBrush;
                    break;
                case ResourceType.Stand:
                    var slotModel = this.GetCharacterModel3D(descriptor.Slot3D);
                    var csGeomtry = slotModel.Geometry as MeshGeometry3D;
                    if (csGeomtry.Positions[0].Z != descriptor.Deepth3D)
                    {
                        List<Point3D> gPointList = new List<Point3D>();
                        foreach (var orgP in csGeomtry.Positions)
                        {
                            Point3D np = new Point3D
                            {
                                X = orgP.X,
                                Y = orgP.Y,
                                Z = descriptor.Deepth3D
                            };
                            gPointList.Add(np);
                        }
                        csGeomtry.Positions.Clear();
                        foreach (var nvP in gPointList)
                        {
                            csGeomtry.Positions.Add(nvP);
                        }
                    }
                    var csMaterial = slotModel.Material;
                    if (!(csMaterial is DiffuseMaterial))
                    {
                        csMaterial = slotModel.Material = new DiffuseMaterial();
                    }
                    var csMaterialBrush = new ImageBrush(sprite.SpriteBitmapImage)
                    {
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        TileMode = TileMode.None
                    };
                    ((DiffuseMaterial)csMaterial).Brush = csMaterialBrush;
                    break;
                case ResourceType.Frontier:
                    var ftModel = ViewManager.View3D.ST3D_Frontier_1;
                    var ftGeomtry = ftModel.Geometry as MeshGeometry3D;
                    if (ftGeomtry.Positions[0].Z != descriptor.Deepth3D)
                    {
                        List<Point3D> gPointList = new List<Point3D>();
                        foreach (var orgP in ftGeomtry.Positions)
                        {
                            Point3D np = new Point3D
                            {
                                X = orgP.X,
                                Y = orgP.Y,
                                Z = descriptor.Deepth3D
                            };
                            gPointList.Add(np);
                        }
                        ftGeomtry.Positions.Clear();
                        foreach (var nvP in gPointList)
                        {
                            ftGeomtry.Positions.Add(nvP);
                        }
                    }
                    var ftMaterial = ftModel.Material;
                    if (!(ftMaterial is DiffuseMaterial))
                    {
                        ftMaterial = ftModel.Material = new DiffuseMaterial();
                    }
                    var ftMaterialBrush = new ImageBrush(sprite.SpriteBitmapImage)
                    {
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        TileMode = TileMode.None
                    };
                    ((DiffuseMaterial)ftMaterial).Brush = ftMaterialBrush;
                    break;
                default:
                    Image spriteImage = new Image();
                    BitmapImage bmp = sprite.SpriteBitmapImage;
                    spriteImage.Width = bmp.PixelWidth;
                    spriteImage.Height = bmp.PixelHeight;
                    spriteImage.Source = bmp;
                    spriteImage.Opacity = descriptor.Opacity;
                    sprite.CutRect = descriptor.CutRect;
                    sprite.DisplayBinding = spriteImage;
                    sprite.Anchor = descriptor.AnchorType;
                    Canvas.SetLeft(spriteImage, descriptor.X - bmp.PixelWidth / 2.0);
                    Canvas.SetTop(spriteImage, descriptor.Y - bmp.PixelHeight / 2.0);
                    Canvas.SetZIndex(spriteImage, descriptor.Z);
                    spriteImage.Visibility = Visibility.Visible;
                    ViewManager.View3D.BO_MainGrid.Children.Add(spriteImage);
                    sprite.InitAnimationRenderTransform();
                    sprite.AnimationElement = sprite.DisplayBinding;
                    descriptor.ToScaleX = descriptor.ScaleX;
                    descriptor.ToScaleY = descriptor.ScaleY;
                    SpriteAnimation.RotateToAnimation(sprite, TimeSpan.Zero, descriptor.Angle, 0);
                    SpriteAnimation.ScaleToAnimation(sprite, TimeSpan.Zero, descriptor.ScaleX, descriptor.ScaleY, 0, 0);
                    break;
            }
            sprite.Descriptor = descriptor;
        }

        /// <summary>
        /// 获取角色立绘槽的3D模型
        /// </summary>
        /// <param name="cSlot">槽中心在屏幕分块的编号</param>
        /// <returns>模型对象</returns>
        private GeometryModel3D GetCharacterModel3D(int cSlot) => ViewManager.View3D.ST3D_Character_Group.Children[cSlot] as GeometryModel3D;

        /// <summary>
        /// 为主窗体描绘一个文字层
        /// </summary>
        /// <param name="msglay">文字层</param>
        /// <param name="descriptor">文字层描述子</param>
        private void DrawMessageLayer(MessageLayer msglay, MessageLayerDescriptor descriptor)
        {
            TextBlock msgBlock = new TextBlock();
            msglay.DisplayBinding = msgBlock;
            if (msglay.BackgroundSprite?.SpriteBitmapImage != null)
            {
                ImageBrush ib = new ImageBrush(msglay.BackgroundSprite.SpriteBitmapImage);
                BitmapImage t = ib.ImageSource as BitmapImage;
                ib.Stretch = Stretch.None;
                ib.TileMode = TileMode.None;
                ib.AlignmentX = AlignmentX.Left;
                ib.AlignmentY = AlignmentY.Top;
                msgBlock.Background = ib;
            }
            msglay.Width = descriptor.Width;
            msglay.Height = descriptor.Height;
            msglay.Opacity = descriptor.Opacity;
            msglay.Padding = (Thickness)descriptor.Padding;
            msglay.LineHeight = descriptor.LineHeight;
            msglay.HorizontalAlignment = descriptor.HorizonAlign;
            msglay.VerticalAlignment = descriptor.VertiAlign;
            msglay.FontColor = Color.FromRgb(descriptor.FontColorR, descriptor.FontColorG, descriptor.FontColorB);
            msglay.FontSize = descriptor.FontSize;
            msglay.FontName = descriptor.FontName;
            msglay.FontShadow = descriptor.FontShadow;
            msglay.DisplayBinding.TextWrapping = TextWrapping.NoWrap;
            msglay.DisplayBinding.TextAlignment = TextAlignment.Left;
            Canvas.SetLeft(msgBlock, descriptor.X);
            Canvas.SetTop(msgBlock, descriptor.Y);
            Canvas.SetZIndex(msgBlock, descriptor.Z);
            msglay.Visibility = descriptor.Visible ? Visibility.Visible : Visibility.Hidden;
            if (ViewManager.Is3DStage)
            {
                ViewManager.View3D.BO_MainGrid.Children.Add(msgBlock);
            }
            else
            {
                ViewManager.View2D.BO_MainGrid.Children.Add(msgBlock);
            }
        }

        /// <summary>
        /// 为主窗体描绘一个按钮
        /// </summary>
        /// <param name="sbutton">按钮实例</param>
        /// <param name="descriptor">按钮描述子</param>
        private void DrawButton(SpriteButton sbutton, SpriteButtonDescriptor descriptor)
        {
            Image buttonImage = new Image();
            BitmapImage bmp = sbutton.ImageNormal.SpriteBitmapImage;
            buttonImage.Width = bmp.PixelWidth;
            buttonImage.Height = bmp.PixelHeight;
            buttonImage.Source = bmp;
            buttonImage.Opacity = descriptor.Opacity;
            sbutton.DisplayBinding = buttonImage;
            sbutton.ImageNormal.DisplayBinding = buttonImage;
            sbutton.ImageMouseOver.DisplayBinding = buttonImage;
            sbutton.ImageMouseOn.DisplayBinding = buttonImage;
            sbutton.ImageNormal.AnimationElement = buttonImage;
            sbutton.ImageMouseOver.AnimationElement = buttonImage;
            sbutton.ImageMouseOn.AnimationElement = buttonImage;
            sbutton.Eternal = descriptor.Eternal;
            sbutton.Enable = descriptor.Enable;
            sbutton.Ntr = new Interrupt()
            {
                detail = "ButtonNTRInterrupt",
                interruptSA = null,
                type = InterruptType.ButtonJump,
                interruptFuncSign = descriptor.InterruptFuncSign,
                returnTarget = descriptor.JumpLabel,
                pureInterrupt = false,
                exitWait = !descriptor.Eternal
            };
            Canvas.SetLeft(buttonImage, descriptor.X - bmp.PixelWidth / 2.0);
            Canvas.SetTop(buttonImage, descriptor.Y - bmp.PixelHeight / 2.0);
            Canvas.SetZIndex(buttonImage, descriptor.Z);
            buttonImage.Visibility = Visibility.Visible;
            buttonImage.MouseDown += sbutton.MouseOnHandler;
            buttonImage.MouseEnter += sbutton.MouseEnterHandler;
            buttonImage.MouseLeave += sbutton.MouseLeaveHandler;
            buttonImage.MouseUp += sbutton.MouseUpHandler;
            if (ViewManager.Is3DStage)
            {
                ViewManager.View3D.BO_MainGrid.Children.Add(buttonImage);
            }
            else
            {
                ViewManager.View2D.BO_MainGrid.Children.Add(buttonImage);
            }
            sbutton.InitAnimationRenderTransform();
        }

        /// <summary>
        /// 为主窗体描绘一个选择支
        /// </summary>
        /// <param name="bbutton">按钮实例</param>
        /// <param name="descriptor">按钮描述子</param>
        private void DrawBranchButton(BranchButton bbutton, BranchButtonDescriptor descriptor)
        {
            TextBlock buttonTextView = new TextBlock();
            BitmapImage bmp = bbutton.ImageNormal.SpriteBitmapImage;
            buttonTextView.Width = bmp.PixelWidth;
            buttonTextView.Height = bmp.PixelHeight;
            ImageBrush ib = new ImageBrush(bmp);
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            ib.TileMode = TileMode.None;
            ib.Stretch = Stretch.Fill;
            buttonTextView.FontSize = GlobalConfigContext.GAME_BRANCH_FONTSIZE;
            buttonTextView.Foreground = new SolidColorBrush(GlobalConfigContext.GAME_BRANCH_FONTCOLOR);
            buttonTextView.FontFamily = new FontFamily(GlobalConfigContext.GAME_BRANCH_FONTNAME);
            buttonTextView.TextAlignment = TextAlignment.Center;
            buttonTextView.Padding = new Thickness(0, GlobalConfigContext.GAME_BRANCH_TOPPAD, 0, 0);
            buttonTextView.Background = ib;
            bbutton.DisplayBinding = buttonTextView;
            bbutton.Eternal = false;
            bbutton.Enable = true;
            bbutton.Text = descriptor.Text;
            bbutton.Ntr = new Interrupt()
            {
                detail = "BranchButtonNTRInterrupt",
                interruptSA = null,
                type = InterruptType.ButtonJump,
                returnTarget = descriptor.JumpTarget,
                exitWait = true
            };
            Canvas.SetLeft(buttonTextView, descriptor.X);
            Canvas.SetTop(buttonTextView, descriptor.Y);
            Canvas.SetZIndex(buttonTextView, descriptor.Z);
            buttonTextView.Visibility = Visibility.Visible;
            buttonTextView.MouseDown += bbutton.MouseOnHandler;
            buttonTextView.MouseEnter += bbutton.MouseEnterHandler;
            buttonTextView.MouseLeave += bbutton.MouseLeaveHandler;
            buttonTextView.MouseUp += bbutton.MouseUpHandler;
            if (ViewManager.Is3DStage)
            {
                ViewManager.View3D.BO_MainGrid.Children.Add(buttonTextView);
            }
            else
            {
                ViewManager.View2D.BO_MainGrid.Children.Add(buttonTextView);
            }
            bbutton.InitAnimationRenderTransform();
        }

        /// <summary>
        /// 将指定文字层从画面移除并释放
        /// </summary>
        /// <param name="id">文字层id</param>
        private void RemoveMessageLayer(int id)
        {
            Director.ScrMana.RemoveMsgLayer(id);
            MessageLayer removeOne = this.messageLayerVec[id];
            this.messageLayerVec[id] = null;
            this.RemoveMessageLayer(removeOne);
        }

        /// <summary>
        /// 将文字层从画面移除
        /// </summary>
        /// <param name="msglay">文字层实例</param>
        private void RemoveMessageLayer(MessageLayer msglay)
        {
            if (msglay != null)
            {
                TextBlock msglayView = msglay.DisplayBinding;
                Canvas workCanvas = ViewManager.Is3DStage ? ViewManager.View3D.BO_MainGrid : ViewManager.View2D.BO_MainGrid;
                if (msglayView != null && workCanvas.Children.Contains(msglayView))
                {
                    workCanvas.Children.Remove(msglayView);
                }
                msglay.DisplayBinding = null;
            }
        }

        /// <summary>
        /// 将指定选择支从画面移除并释放
        /// </summary>
        /// <param name="id">选择支id</param>
        private void RemoveBranchButton(int id)
        {
            Director.ScrMana.RemoveBranchButton(id);
            BranchButton removeOne = this.branchButtonVec[id];
            this.branchButtonVec[id] = null;
            this.RemoveBranchButton(removeOne);
        }

        /// <summary>
        /// 将选择支从画面移除
        /// </summary>
        /// <param name="bbutton">选择支实例</param>
        private void RemoveBranchButton(BranchButton bbutton)
        {
            if (bbutton != null)
            {
                var bbView = bbutton.DisplayBinding;
                Canvas workCanvas = ViewManager.Is3DStage ? ViewManager.View3D.BO_MainGrid : ViewManager.View2D.BO_MainGrid;
                if (bbView != null && workCanvas.Children.Contains(bbView))
                {
                    workCanvas.Children.Remove(bbView);
                }
                bbutton.DisplayBinding = null;
            }
        }

        /// <summary>
        /// 将精灵从画布上移除
        /// </summary>
        /// <param name="rType">资源类型</param>
        /// <param name="sprite">精灵实例</param>
        private void RemoveSprite(ResourceType rType, YuriSprite sprite)
        {
            if (sprite == null)
            {
                return;
            }
            Canvas canvas;
            if (ViewManager.Is3DStage)
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        var bgModelGeometry = ViewManager.View3D.ST3D_Background_Fore;
                        if (!(bgModelGeometry.Material is DiffuseMaterial))
                        {
                            return;
                        }
                        bgModelGeometry.Material = null;
                        return;
                    case ResourceType.Stand:
                        var slotModelGeometry = this.GetCharacterModel3D(sprite.Descriptor.Slot3D);
                        if (!(slotModelGeometry?.Material is DiffuseMaterial))
                        {
                            return;
                        }
                        slotModelGeometry.Material = null;
                        return;
                    case ResourceType.Frontier:
                        var ftModelGeometry = ViewManager.View3D.ST3D_Frontier_1;
                        if (!(ftModelGeometry.Material is DiffuseMaterial))
                        {
                            return;
                        }
                        ftModelGeometry.Material = null;
                        return;
                    default:
                        canvas = ViewManager.View3D.BO_MainGrid;
                        break;
                }
            }
            else
            {
                canvas = this.GetDrawingCanvas2D(rType);
            }
            var spriteView = sprite.DisplayBinding;
            if (spriteView != null)
            {
                if (canvas.Children.Contains(spriteView))
                {
                    canvas.Children.Remove(spriteView);
                }
            }
            sprite.DisplayBinding = null;
        }

        /// <summary>
        /// 将按钮从画面移除
        /// </summary>
        /// <param name="sbutton">按钮实例</param>
        private void RemoveButton(SpriteButton sbutton)
        {
            if (sbutton == null)
            {
                return;
            }
            Image buttonView = sbutton.DisplayBinding;
            Canvas workCanvas = ViewManager.Is3DStage ? ViewManager.View3D.BO_MainGrid : ViewManager.View2D.BO_MainGrid;
            if (buttonView != null && workCanvas.Children.Contains(buttonView))
            {
                workCanvas.Children.Remove(buttonView);
                if (sbutton.ImageNormal != null)
                {
                    sbutton.ImageNormal.DisplayBinding = null;
                }
                if (sbutton.ImageMouseOver != null)
                {
                    sbutton.ImageMouseOver.DisplayBinding = null;
                }
                if (sbutton.ImageMouseOn != null)
                {
                    sbutton.ImageMouseOn.DisplayBinding = null;
                }
            }
            sbutton.DisplayBinding = null;
        }

        /// <summary>
        /// 加载过渡样式
        /// </summary>
        /// <param name="assembly">过渡所在的程序集</param>
        private void LoadTransitions(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (transitionTypes.Contains(type)) { continue; }
                // Must not be abstract.
                if ((typeof(Transition).IsAssignableFrom(type)) && (!type.IsAbstract))
                {
                    transitionTypes.Add(type);
                }
            }
        }
        
        /// <summary>
        /// 将窗体控件转化为JPEG图片
        /// </summary>
        /// <param name="ui">控件的引用</param>
        /// <param name="filename">要保存的图片文件名</param>
        public static void RenderFrameworkElementToJPEG(FrameworkElement ui, string filename)
        {
            try
            {
                System.IO.FileStream ms = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)ui.ActualWidth, (int)ui.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                bmp.Render(ui);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(ms);
                ms.Close();
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine(String.Format("Saving Snapshot Failed. path: {0} with CLR Error: {1}", filename, ex),
                    "ViewManager", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 初始化2D视图
        /// </summary>
        public void InitViewport2D()
        {
            if (ViewManager.Is3DStage) { return; }
            // 初始化视窗向量
            this.viewbox2dVec[(int)ViewportType.VTBackground] = new Viewport2D()
            {
                Type = ViewportType.VTBackground,
                ViewboxBinding = View2D.BO_Bg_Viewbox,
                CanvasBinding = View2D.BO_Bg_Canvas
            };
            this.viewbox2dVec[(int)ViewportType.VTCharacterStand] = new Viewport2D()
            {
                Type = ViewportType.VTCharacterStand,
                ViewboxBinding = View2D.BO_Cstand_Viewbox,
                CanvasBinding = View2D.BO_Cstand_Canvas
            };
            this.viewbox2dVec[(int)ViewportType.VTPictures] = new Viewport2D()
            {
                Type = ViewportType.VTPictures,
                ViewboxBinding = View2D.BO_Pics_Viewbox,
                CanvasBinding = View2D.BO_Pics_Canvas
            };
            // 初始化变换动画
            for (int i = 0; i < 3; i++)
            {
                TransformGroup aniGroup = new TransformGroup();
                TranslateTransform XYTransformer = new TranslateTransform();
                ScaleTransform ScaleTransformer = new ScaleTransform
                {
                    CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0,
                    CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0
                };
                RotateTransform RotateTransformer = new RotateTransform
                {
                    CenterX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0,
                    CenterY = GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0
                };
                aniGroup.Children.Add(XYTransformer);
                aniGroup.Children.Add(ScaleTransformer);
                aniGroup.Children.Add(RotateTransformer);
                this.viewbox2dVec[i].ViewboxBinding.RenderTransform = aniGroup;
                this.viewbox2dVec[i].RotateTransformer = RotateTransformer;
                this.viewbox2dVec[i].TranslateTransformer = XYTransformer;
                this.viewbox2dVec[i].ScaleTransformer = ScaleTransformer;
            }
        }

        /// <summary>
        /// 初始化3D视图
        /// </summary>
        public void InitViewport3D()
        {
            if (!ViewManager.Is3DStage) { return; }
            var cbList = SCamera3D.CharacterBlockList;
            var csModelGroup = ViewManager.View3D.ST3D_Character_Group;
            for (int i = 0; i <= GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT; i++)
            {
                GeometryModel3D Geom = new GeometryModel3D();
                var sideFrontPlane = new MeshGeometry3D();
                sideFrontPlane.Positions.Add(cbList[i].Item1);
                sideFrontPlane.Positions.Add(cbList[i].Item2);
                sideFrontPlane.Positions.Add(cbList[i].Item3);
                sideFrontPlane.Positions.Add(cbList[i].Item4);
                sideFrontPlane.TriangleIndices.Add(0);
                sideFrontPlane.TriangleIndices.Add(1);
                sideFrontPlane.TriangleIndices.Add(2);
                sideFrontPlane.TriangleIndices.Add(1);
                sideFrontPlane.TriangleIndices.Add(3);
                sideFrontPlane.TriangleIndices.Add(2);
                sideFrontPlane.TextureCoordinates.Add(new Point(0, 1));
                sideFrontPlane.TextureCoordinates.Add(new Point(1, 1));
                sideFrontPlane.TextureCoordinates.Add(new Point(0, 0));
                sideFrontPlane.TextureCoordinates.Add(new Point(1, 0));
                Geom.Geometry = sideFrontPlane;
                Geom.Material = null;
                csModelGroup.Children.Add(Geom);
            }
        }

        /// <summary>
        /// 为视窗管理器设置窗体的引用并更新视窗向量
        /// </summary>
        /// <param name="wnd">主窗体的引用</param>
        public static void SetWindowReference(MainWindow wnd) => ViewManager.mWnd = wnd;

        /// <summary>
        /// 获取应用程序主窗体
        /// </summary>
        /// <returns>主窗体的引用</returns>
        public static MainWindow GetWindowReference() => ViewManager.mWnd;

        /// <summary>
        /// 获取或设置遮罩层的引用
        /// </summary>
        public static Frame MaskFrameRef { get; set; }

        /// <summary>
        /// 视窗向量
        /// </summary>
        private readonly List<Viewport2D> viewbox2dVec;

        /// <summary>
        /// 背景精灵向量
        /// </summary>
        private readonly List<YuriSprite> backgroundSpriteVec;
        
        /// <summary>
        /// 立绘精灵向量
        /// </summary>
        private readonly List<YuriSprite> characterStandSpriteVec;
        
        /// <summary>
        /// 图片精灵向量
        /// </summary>
        private readonly List<YuriSprite> pictureSpriteVec;

        /// <summary>
        /// 文字层向量
        /// </summary>
        private readonly List<MessageLayer> messageLayerVec;

        /// <summary>
        /// 按钮层向量
        /// </summary>
        private readonly List<SpriteButton> buttonLayerVec;

        /// <summary>
        /// 选择支按钮向量
        /// </summary>
        private readonly List<BranchButton> branchButtonVec;

        /// <summary>
        /// 过渡类型容器
        /// </summary>
        private readonly ObservableCollection<Type> transitionTypes = new ObservableCollection<Type>();

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ViewManager()
        {
            if (ViewManager.Is3DStage == false)
            {
                this.LoadTransitions(Assembly.GetAssembly(typeof(Transition)));
                this.viewbox2dVec = new List<Viewport2D>();
                for (int i = 0; i < 3; i++)
                {
                    this.viewbox2dVec.Add(null);
                }
            }
            this.backgroundSpriteVec = new List<YuriSprite>();
            this.characterStandSpriteVec = new List<YuriSprite>();
            this.pictureSpriteVec = new List<YuriSprite>();
            this.messageLayerVec = new List<MessageLayer>();
            this.branchButtonVec = new List<BranchButton>();
            this.buttonLayerVec = new List<SpriteButton>();
            for (int i = 0; i < GlobalConfigContext.GAME_IMAGELAYER_COUNT; i++)
            {
                this.pictureSpriteVec.Add(null);
            }
            for (int i = 0; i < GlobalConfigContext.GAME_MESSAGELAYER_COUNT; i++)
            {
                this.messageLayerVec.Add(null);
            }
            for (int i = 0; i < GlobalConfigContext.GAME_BUTTON_COUNT; i++)
            {
                this.buttonLayerVec.Add(null);
            }
            for (int i = 0; i < GlobalConfigContext.GAME_BRANCH_COUNT; i++)
            {
                this.branchButtonVec.Add(null);
            }
            for (int i = 0; i < GlobalConfigContext.GAME_BACKGROUND_COUNT; i++)
            {
                this.backgroundSpriteVec.Add(null);
            }
            var charaBorder = ViewManager.Is3DStage
                ? GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT
                : GlobalConfigContext.GAME_CHARACTERSTAND_COUNT;
            for (int i = 0; i < charaBorder; i++)
            {
                this.characterStandSpriteVec.Add(null);
            }
        }

        /// <summary>
        /// 2D主舞台页面的引用
        /// </summary>
        public static Stage2D View2D => ViewPageManager.RetrievePage(GlobalConfigContext.FirstViewPage) as Stage2D;

        /// <summary>
        /// 3D主舞台页面的引用
        /// </summary>
        public static Stage3D View3D => ViewPageManager.RetrievePage(GlobalConfigContext.FirstViewPage) as Stage3D;

        /// <summary>
        /// 获取或设置当前是否使用3D镜头系统
        /// </summary>
        public static bool Is3DStage => GlobalConfigContext.GAME_IS3D;

        /// <summary>
        /// 获取主窗体的引用
        /// </summary>
        public static MainWindow mWnd { get; private set; } = null;
        
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static ViewManager synObject = null;
    }

    /// <summary>
    /// 枚举：背景图所在的层
    /// </summary>
    internal enum BackgroundPage
    {
        /// <summary>
        /// 背景层：不可见的层，过渡效果所作用的层
        /// </summary>
        Back = 0,
        /// <summary>
        /// 前景层：可见的层
        /// </summary>
        Fore = 1
    }
}
