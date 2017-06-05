using System;
using System.Collections.Generic;

namespace Yuri.PlatformCore.Router
{
    /// <summary>
    /// 可消息路由项目
    /// </summary>
    public interface IRouteable
    {
        /// <summary>
        /// 冒泡触发
        /// </summary>
        void TriggerBubbling(YuriRoutedEvent evt);

        /// <summary>
        /// 隧道触发
        /// </summary>
        void TriggerTunneling(YuriRoutedEvent evt);

        /// <summary>
        /// 直接触发
        /// </summary>
        void TriggerDirect(YuriRoutedEvent evt);

        /// <summary>
        /// 获取或设置路由的唯一标识符名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 获取或设置路由是否吞下经过的事件
        /// </summary>
        bool IsSwallow { get; set; }

        /// <summary>
        /// 获取或设置路由的上级路由
        /// </summary>
        IRouteable Parent { get; set; }

        /// <summary>
        /// 获取或设置路由的子级路由
        /// </summary>
        List<IRouteable> Children { get; set; }
    }

    /// <summary>
    /// 枚举：路由类型
    /// </summary>
    public enum YuriRoutedType
    {
        /// <summary>
        /// 直接
        /// </summary>
        Direct,
        /// <summary>
        /// 冒泡：自底向上
        /// </summary>
        Bubbling,
        /// <summary>
        /// 隧道：自顶向下
        /// </summary>
        Tunneling
    }

}
