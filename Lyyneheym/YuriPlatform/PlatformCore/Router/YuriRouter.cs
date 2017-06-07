using System;
using System.Collections.Generic;

namespace Yuri.PlatformCore.Router
{
    /// <summary>
    /// Yuri基础路由器类
    /// </summary>
    public class YuriRouter : IRouteable
    {
        /// <summary>
        /// 冒泡触发
        /// </summary>
        public void TriggerBubbling(YuriRoutedEvent evt)
        {
            evt.RaiseOnRouterAcceptEvent(this, null);
            evt.RaiseProcessedEvent(this, null);
            if (this.IsSwallow)
            {
                evt.RaiseRouterSwallowedEvent(this, null);
            }
            else
            {
                this.Parent?.TriggerBubbling(evt);
            }
            evt.RaiseOnRouterRoutedEvent(this, null);
        }

        /// <summary>
        /// 隧道触发
        /// </summary>
        public void TriggerTunneling(YuriRoutedEvent evt)
        {
            evt.RaiseOnRouterAcceptEvent(this, null);
            evt.RaiseProcessedEvent(this, null);
            if (this.IsSwallow)
            {
                evt.RaiseRouterSwallowedEvent(this, null);
            }
            else
            {
                this.Children?.ForEach(cr => cr.TriggerTunneling(evt));
            }
            evt.RaiseOnRouterRoutedEvent(this, null);
        }

        /// <summary>
        /// 直接触发
        /// </summary>
        public void TriggerDirect(YuriRoutedEvent evt)
        {
            evt.RaiseOnRouterAcceptEvent(this, null);
            evt.RaiseProcessedEvent(this, null);
            evt.RaiseOnRouterRoutedEvent(this, null);
        }

        /// <summary>
        /// 获取或设置路由的唯一标识符名字
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// 获取或设置路由是否吞下经过的事件
        /// </summary>
        public bool IsSwallow { get; set; } = false;

        /// <summary>
        /// 获取或设置路由的上级路由
        /// </summary>
        public IRouteable Parent { get; set; } = null;

        /// <summary>
        /// 获取或设置路由的子级路由
        /// </summary>
        public List<IRouteable> Children { get; set; } = new List<IRouteable>();
    }
}
