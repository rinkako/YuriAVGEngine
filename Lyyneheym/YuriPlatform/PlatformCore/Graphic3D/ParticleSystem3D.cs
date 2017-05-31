using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Yuri.PlatformCore.Graphic3D
{
    /// <summary>
    /// 3D粒子系统
    /// </summary>
    internal sealed class ParticleSystem3D
    {
        /// <summary>
        /// 创建一个粒子系统
        /// </summary>
        public ParticleSystem3D()
        {
            this.emitterDict = new Dictionary<Color, ParticleEmitter3D>();
        }

        /// <summary>
        /// 更新粒子系统状态
        /// </summary>
        /// <param name="elapsed">已消逝的计数</param>
        public void Update(float elapsed)
        {
            foreach (var ps in this.emitterDict.Values)
            {
                ps.Update(elapsed);
            }
        }

        /// <summary>
        /// 发射粒子
        /// </summary>
        /// <param name="position">起始位置</param>
        /// <param name="speed">速率</param>
        /// <param name="color">颜色</param>
        /// <param name="size">起始半径</param>
        /// <param name="life">生命周期</param>
        public void SpawnParticle(Point3D position, double speed, Color color, double size, double life)
        {
            this.emitterDict[color]?.SpawnParticle(position, speed, size, life);
        }

        /// <summary>
        /// 创造一种粒子的发射器
        /// </summary>
        /// <param name="maxCount">该发射器最大并存粒子数</param>
        /// <param name="color">发射粒子的颜色</param>
        /// <returns>发射器在3D世界的模型</returns>
        public Model3D CreateParticleEmitter(int maxCount, Color color)
        {
            var ps = new ParticleEmitter3D(maxCount, color);
            this.emitterDict.Add(color, ps);
            return ps.ParticleModel;
        }

        /// <summary>
        /// 当前粒子系统中活泼的粒子数
        /// </summary>
        public int ActiveParticleCount => this.emitterDict.Values.Sum(ps => ps.Count);

        /// <summary>
        /// 粒子发射器集合
        /// </summary>
        private readonly Dictionary<Color, ParticleEmitter3D> emitterDict;
    }
}
