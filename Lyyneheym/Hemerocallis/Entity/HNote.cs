using System;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 备忘录实体
    /// </summary>
    [Serializable]
    internal sealed class HNote
    {
        /// <summary>
        /// 获取或设置备忘录唯一标识ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置备忘录内容
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 获取或设置备忘录创建时间戳
        /// </summary>
        public DateTime CreateTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置备忘录最后修改时间戳
        /// </summary>
        public DateTime LastEditTimeStamp { get; set; }
    }
}
