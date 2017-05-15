using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 书籍实体类
    /// </summary>
    [Serializable]
    internal sealed class HBook
    {
        /// <summary>
        /// 获取或设置书籍的唯一标识ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置书籍的名字
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置书籍在系统缓存文件中的位置
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 获取或设置书籍的首页文章
        /// </summary>
        public HArticle HomePage { get; set; }

        /// <summary>
        /// 获取或设置文章创建时间戳
        /// </summary>
        public DateTime CreateTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置文章最后修改时间戳
        /// </summary>
        public DateTime LastEditTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置书籍的一级下属文章向量
        /// </summary>
        public List<HArticle> Children { get; set; } = new List<HArticle>();
    }
}
