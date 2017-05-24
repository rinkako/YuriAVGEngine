using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 文章实体类
    /// </summary>
    [Serializable]
    internal sealed class HArticle
    {
        /// <summary>
        /// 获取或设置文章实体的唯一ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置文章的名字
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置文章创建时间戳
        /// </summary>
        public DateTime CreateTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置文章最后修改时间戳
        /// </summary>
        public DateTime LastEditTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置文章的母亲节点的唯一ID
        /// </summary>
        public string ParentId { get; set; }
        
        /// <summary>
        /// 获取或设置所属书籍的唯一ID
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// 获取节点的孩子向量
        /// </summary>
        public List<HArticle> ChildrenList { get; } = new List<HArticle>();

        /// <summary>
        /// 获取或设置该实体的文章内容的元数据
        /// </summary>
        public MemoryStream DocumentMetadata { get; set; }
    }
}
