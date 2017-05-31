using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Yuri.PlatformCore.Graphic3D
{
    /// <summary>
    /// 3D粒子发射器类
    /// </summary>
    internal sealed class ParticleEmitter3D
    {
        /// <summary>
        /// 新建一个3D粒子发射器
        /// </summary>
        /// <param name="maxCount">最大粒子数</param>
        /// <param name="color">发射器所发射</param>
        public ParticleEmitter3D(int maxCount, Color color)
        {
            // 初始化
            this.MaxParticleCount = maxCount;
            this.particleList = new List<ParticleDescriptor3D>();
            this.particleModel = new GeometryModel3D
            {
                Geometry = new MeshGeometry3D()
            };
            RadialGradientBrush b = new RadialGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, color.R, color.G, color.B), 0.25));
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, color.R, color.G, color.B), 1.0));
            Ellipse e = new Ellipse
            {
                Width = 32.0,
                Height = 32.0,
                Fill = b
            };
            e.Measure(new Size(32, 32));
            e.Arrange(new Rect(0, 0, 32, 32));
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(e);
            renderTarget.Freeze();
            Brush brush = new ImageBrush(renderTarget);
            DiffuseMaterial material = new DiffuseMaterial(brush);
            this.particleModel.Material = material;
            this.rand = new Random(brush.GetHashCode());
        }
        
        /// <summary>
        /// 发射粒子
        /// </summary>
        /// <param name="position">起始位置</param>
        /// <param name="speed">速率</param>
        /// <param name="size">起始半径</param>
        /// <param name="life">生命周期</param>
        public void SpawnParticle(Point3D position, double speed, double size, double life)
        {
            // 不能超过最大粒子数
            if (this.particleList.Count > this.MaxParticleCount) { return; }
            // 建立粒子描述子
            ParticleDescriptor3D p = new ParticleDescriptor3D
            {
                Position = position,
                StartLife = life,
                Life = life,
                StartSize = size,
                Size = size
            };
            // 根据速率构造速度向量
            float x = 1.0f - (float) rand.NextDouble() * 2.0f;
            float z = 1.0f - (float) rand.NextDouble() * 2.0f;
            Vector3D v = new Vector3D(x, z, 0.0);
            v.Normalize();
            v *= ((float) rand.NextDouble() + 0.25f) * (float) speed;
            p.Velocity = new Vector3D(v.X, v.Y, v.Z);
            p.Decay = 1.0f;
            // 添加到渲染列表
            this.particleList.Add(p);
        }

        /// <summary>
        /// 更新该发射器的状态
        /// </summary>
        /// <param name="elapsed">已消逝的计数</param>
        public void Update(double elapsed)
        {
            // 更新粒子描述子
            var deadList = new List<ParticleDescriptor3D>();
            foreach (ParticleDescriptor3D p in this.particleList)
            {
                p.Position += p.Velocity * elapsed;
                p.Life -= p.Decay * elapsed;
                p.Size = p.StartSize * (p.Life / p.StartLife);
                if (p.Life <= 0.0)
                    deadList.Add(p);
            }
            // 移除已经消逝的粒子
            foreach (ParticleDescriptor3D p in deadList)
            {
                this.particleList.Remove(p);
            }
            // 渲染前端
            this.UpdateGeometry();
        }

        /// <summary>
        /// 依照更新完的描述子渲染前端
        /// </summary>
        private void UpdateGeometry()
        {
            Point3DCollection positions = new Point3DCollection();
            Int32Collection indices = new Int32Collection();
            PointCollection texcoords = new PointCollection();
            for (int i = 0; i < this.particleList.Count; ++i)
            {
                int positionIndex = i * 4;
                ParticleDescriptor3D p = this.particleList[i];
                Point3D p1 = new Point3D(p.Position.X, p.Position.Y, p.Position.Z);
                Point3D p2 = new Point3D(p.Position.X, p.Position.Y + p.Size, p.Position.Z);
                Point3D p3 = new Point3D(p.Position.X + p.Size, p.Position.Y + p.Size, p.Position.Z);
                Point3D p4 = new Point3D(p.Position.X + p.Size, p.Position.Y, p.Position.Z);
                positions.Add(p1);
                positions.Add(p2);
                positions.Add(p3);
                positions.Add(p4);
                texcoords.Add(renderDirectionT1);
                texcoords.Add(renderDirectionT2);
                texcoords.Add(renderDirectionT3);
                texcoords.Add(renderDirectionT4);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
            }
            ((MeshGeometry3D)this.particleModel.Geometry).Positions = positions;
            ((MeshGeometry3D)this.particleModel.Geometry).TriangleIndices = indices;
            ((MeshGeometry3D)this.particleModel.Geometry).TextureCoordinates = texcoords;
        }

        /// <summary>
        /// 获取或设置最大粒子数
        /// </summary>
        public int MaxParticleCount { get; set; }

        /// <summary>
        /// 获取当前粒子数
        /// </summary>
        public int Count => this.particleList.Count;

        /// <summary>
        /// 获取当前粒子发射器在3D世界的模型对象
        /// </summary>
        public Model3D ParticleModel => this.particleModel;

        /// <summary>
        /// 粒子渲染列表
        /// </summary>
        private readonly List<ParticleDescriptor3D> particleList;

        /// <summary>
        /// 发射器材质
        /// </summary>
        private readonly GeometryModel3D particleModel;

        /// <summary>
        /// 该发射器的随机数生成器
        /// </summary>
        private readonly Random rand;

        /// <summary>
        /// 材质渲染方向标记点1
        /// </summary>
        private static readonly Point renderDirectionT1 = new Point(0.0, 0.0);

        /// <summary>
        /// 材质渲染方向标记点2
        /// </summary>
        private static readonly Point renderDirectionT2 = new Point(0.0, 1.0);

        /// <summary>
        /// 材质渲染方向标记点3
        /// </summary>
        private static readonly Point renderDirectionT3 = new Point(1.0, 1.0);

        /// <summary>
        /// 材质渲染方向标记点4
        /// </summary>
        private static readonly Point renderDirectionT4 = new Point(1.0, 0.0);
    }
}
