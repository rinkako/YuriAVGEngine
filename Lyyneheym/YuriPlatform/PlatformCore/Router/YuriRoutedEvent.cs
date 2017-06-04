using System;

namespace Yuri.PlatformCore.Router
{
    /// <summary>
    /// 路由事件类：为引擎模块间通讯提供消息包装
    /// </summary>
    public class YuriRoutedEvent
    {
        /// <summary>
        /// 获取或设置事件的路由类型
        /// </summary>
        public YuriRoutedType Type { get; set; }

        /// <summary>
        /// 获取或设置事件的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置事件的唯一标识符
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 获取或设置事件的附加值
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 获取或设置事件发出路由器的名字
        /// </summary>
        public string FromRouter { get; set; }

        /// <summary>
        /// 获取或设置事件最终目的地路由器的名字
        /// </summary>
        public string ToRouter { get; set; }

        /// <summary>
        /// 获取或设置上一路由器的名字
        /// </summary>
        public string LastRouter { get; set; }
        
        /// <summary>
        /// 获取或设置事件是否已经被处理
        /// </summary>
        public bool IsHandled { get; set; } = false;

        /// <summary>
        /// 获取或设置事件是否已经被取消
        /// </summary>
        public bool IsCanceled { get; set; } = false;
    }
}
