using System.Windows.Media.Media3D;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Graphic3D
{
    /// <summary>
    /// 3D粒子描述子类
    /// </summary>
    internal sealed class ParticleDescriptor3D : CloneableDescriptor
    {
        /// <summary>
        /// 当前世界坐标
        /// </summary>
        public Point3D Position { get; set; }

        /// <summary>
        /// 速度向量
        /// </summary>
        public Vector3D Velocity { get; set; }

        /// <summary>
        /// 当前剩余寿命
        /// </summary>
        public double Life { get; set; }

        /// <summary>
        /// 衰弱速度
        /// </summary>
        public double Decay { get; set; }

        /// <summary>
        /// 生命周期
        /// </summary>
        public double StartLife { get; set; }

        /// <summary>
        /// 当前半径
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// 起始半径
        /// </summary>
        public double StartSize { get; set; }
    }
}
