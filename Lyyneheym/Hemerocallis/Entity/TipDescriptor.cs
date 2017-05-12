using System;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 屏幕贴纸描述子实体
    /// </summary>
    [Serializable]
    internal sealed class TipDescriptor
    {
        /// <summary>
        /// 获取或设置贴纸的Id号，它也将作为贴纸的Z坐标
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置贴纸的X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 获取或设置贴纸的Y坐标
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// 获取或设置贴纸内容的类型
        /// </summary>
        public TipType Type { get; set; }

        /// <summary>
        /// 获取或设置贴纸内容
        /// </summary>
        public object Tag { get; set; }
    }

    /// <summary>
    /// 枚举：贴纸内容类型
    /// </summary>
    [Serializable]
    internal enum TipType
    {
        /// <summary>
        /// 简单文本框
        /// </summary>
        Text,
        /// <summary>
        /// 引用备忘录
        /// </summary>
        Note,
        /// <summary>
        /// 外部图片
        /// </summary>
        Picture
    }
}
