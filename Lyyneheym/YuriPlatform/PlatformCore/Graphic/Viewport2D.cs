using System.Windows.Media;
using System.Windows.Controls;

namespace Yuri.PlatformCore.Graphic
{
    /// <summary>
    /// 2D视窗封装类
    /// </summary>
    internal sealed class Viewport2D
    {
        /// <summary>
        /// 获取或设置视窗类型
        /// </summary>
        public ViewportType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗所绑定的视窗对象
        /// </summary>
        public Viewbox ViewboxBinding
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗所绑定的画布
        /// </summary>
        public Canvas CanvasBinding
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗所绑定的平移变换器
        /// </summary>
        public TranslateTransform TranslateTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗所绑定的缩放变换器
        /// </summary>
        public ScaleTransform ScaleTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置视窗所绑定的旋转变换器
        /// </summary>
        public RotateTransform RotateTransformer
        {
            get;
            set;
        }
    }
}
