using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimSlyvia
{
    class MySprite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="device"></param>
        public MySprite(Device device)
        {
            this.sprite = new Sprite(device);
        }

        /// <summary>
        /// 声明渲染开始
        /// </summary>
        /// <param name="flags">渲染参数</param>
        public void Begin(SpriteFlags flags)
        {
            this.sprite.Begin(flags);
        }

        /// <summary>
        /// 声明渲染结束
        /// </summary>
        public void End()
        {
            this.sprite.End();
        }

        /// <summary>
        /// 释放这个精灵
        /// </summary>
        public void Dispose()
        {
            this.sprite.Dispose();
        }

        public void Draw2D(TextureObject textureObject, float scaleX, float scaleY, float rotationAngle, PointF position, Color color, bool mirrored)
        {
            this.Draw2D(textureObject, scaleX, scaleY, rotationAngle, position, textureObject.RoratingCenter, color, mirrored);
        }

        public void Draw2D(TextureObject textureObject, float scale, float rotationAngle, PointF position, int transparent)
        {
            this.Draw2D(textureObject, scale, rotationAngle, position, Color.FromArgb(transparent, Color.White));
        }

        public void Draw2D(TextureObject textureObject, float scale, float rotationAngle, PointF position, Color color)
        {
            this.Draw2D(textureObject, scale, rotationAngle, position, textureObject.RoratingCenter, color);
        }

        public void Draw2D(TextureObject textureObject, float scale, float rotationAngle, PointF position, PointF rotationCenter, Color color)
        {
            this.Draw2D(textureObject, scale, rotationAngle, position, rotationCenter, color, false);
        }

        public void Draw2D(TextureObject textureObject, float scale, float rotationAngle, PointF position, PointF rotationCenter, Color color, bool mirrored)
        {
            this.Draw2D(textureObject, scale, scale, rotationAngle, position, rotationCenter, color, mirrored);
        }

        /// <summary>
        /// 在精灵上描绘一个纹理
        /// </summary>
        /// <param name="textureObject">纹理切片</param>
        /// <param name="scaleX">横比例</param>
        /// <param name="scaleY">纵比例</param>
        /// <param name="rotationAngle">旋转角度</param>
        /// <param name="position">贴图几何中心的位置</param>
        /// <param name="rorationCenter">旋转中心</param>
        /// <param name="color">透明度和颜色</param>
        /// <param name="mirrored">镜像翻转</param>
        public void Draw2D(TextureObject textureObject, float scaleX, float scaleY, float rotationAngle, PointF position, PointF rorationCenter, Color color, bool mirrored)
        {
            Vector3 vector = new Vector3(rorationCenter.X, rorationCenter.Y, 0f);
            Vector3 vector2 = new Vector3(position.X, position.Y, 0f);
            Color4 color2 = new Color4(color);
            Vector2 vector3 = new Vector2(rorationCenter.X, rorationCenter.Y);
            Vector2 scalingCenter = new Vector2(position.X, position.Y);
            Vector2 scaling = mirrored ? new Vector2(-scaleX, scaleY) : new Vector2(scaleX, scaleY);
            this.sprite.Transform = Matrix.Transformation2D(scalingCenter, 0f, scaling, scalingCenter, rotationAngle, new Vector2(0f, 0f));
            this.sprite.Draw(textureObject.TXTure, new Rectangle?(textureObject.PosRect), new Vector3?(vector), new Vector3?(vector2), color2);
            this.sprite.Transform = Matrix.Identity;
        }

        /// <summary>
        /// Slim的精灵实例
        /// </summary>
        public Sprite sprite { get; set; }

        /// <summary>
        /// 返回该实例是否已经释放
        /// </summary>
        public bool Disposed
        {
            get
            {
                return this.sprite.Disposed;
            }
        }
    }


    public class TextureObject
    {
        public int Height;
        public int Width
        {
            get
            {
                return this.PosRect.Width;
            }
        }
        public PointF LeftTop
        {
            get
            {
                return new PointF(0f, 0f);
            }
        }
        public PointF RightTop
        {
            get
            {
                return new PointF((float)this.Width, 0f);
            }
        }
        public PointF RoratingCenter
        {
            get
            {
                return new PointF((float)((this.PosRect.Width / 2) + this.OffsetX), (float)((this.PosRect.Height / 2) + this.OffsetY));
            }
        }
        public string Name { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public Rectangle PosRect { get; set; }
        public int SrcHeight { get; set; }
        public int SrcWidth { get; set; }
        public Texture TXTure { get; set; }

        public void Dispose()
        {
            if (!this.TXTure.Disposed)
            {
                this.TXTure.Dispose();
            }
        }
    }
}
