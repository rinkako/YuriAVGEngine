using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 设置描述子
    /// </summary>
    [Serializable]
    internal sealed class ConfigDescriptor
    {
        /// <summary>
        /// 获取或设置正文字体名
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 获取或设置正文字号
        /// </summary>
        public string FontSize { get; set; }

        /// <summary>
        /// 获取或设置是否开启晕染效果
        /// </summary>
        public bool IsEnableZe { get; set; }

        /// <summary>
        /// 获取或设置晕染不透明度
        /// </summary>
        public double ZeOpacity { get; set; }

        /// <summary>
        /// 获取或设置行距
        /// </summary>
        public double LineHeight { get; set; }

        /// <summary>
        /// 获取或设置背景样式
        /// </summary>
        public AppearanceBackgroundType BgType { get; set; }

        /// <summary>
        /// 获取或设置当前背景样式下的描述文本
        /// </summary>
        public string BgTag { get; set; }
    }

    /// <summary>
    /// 枚举：背景样式
    /// </summary>
    [Serializable]
    internal enum AppearanceBackgroundType
    {
        /// <summary>
        /// 默认背景
        /// </summary>
        Default,
        /// <summary>
        /// 纯色背景
        /// </summary>
        Pure,
        /// <summary>
        /// 图片背景
        /// </summary>
        Picture
    }
}
