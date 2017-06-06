using System;
using System.Collections.Generic;

namespace Yuri.PlatformCore.Router
{
    /// <summary>
    /// 消息路由管理器：维护路由器的引用，提供消息服务
    /// </summary>
    public static class RouterManager
    {
        /// <summary>
        /// 获取一个路由器
        /// </summary>
        /// <param name="name">路由器的名字</param>
        /// <returns>路由器的引用</returns>
        public static IRouteable GetRouter(string name)
        {
            return RouterManager.ExistRouter(name) ? RouterManager.RouterTable[name] : null;
        }

        /// <summary>
        /// 添加一个路由器
        /// </summary>
        /// <param name="router">路由器的引用</param>
        /// <returns>是否为替换操作</returns>
        public static bool SetRouter(IRouteable router)
        {
            bool replaced = RouterManager.ExistRouter(router.Name);
            RouterManager.RouterTable[router.Name] = router;
            return replaced;
        }

        /// <summary>
        /// 检测一个路由是否存在
        /// </summary>
        /// <param name="name">路由的名字</param>
        /// <returns>该名字的路由是否存在路由表中</returns>
        public static bool ExistRouter(string name)
        {
            return RouterManager.RouterTable.ContainsKey(name);
        }

        /// <summary>
        /// 向一个路由器发送一个消息
        /// </summary>
        /// <param name="routerName">起始路由器名字</param>
        /// <param name="evt">路由消息</param>
        /// <returns>起始路由是否存在</returns>
        public static bool Send(string routerName, YuriRoutedEvent evt)
        {
            if (RouterManager.ExistRouter(routerName))
            {
                var router = RouterManager.RouterTable[routerName];
                switch (evt.Type)
                {
                    case YuriRoutedType.Direct:
                        router.TriggerDirect(evt);
                        break;
                    case YuriRoutedType.Bubbling:
                        router.TriggerBubbling(evt);
                        break;
                    case YuriRoutedType.Tunneling:
                        router.TriggerTunneling(evt);
                        break;
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 路由表
        /// </summary>
        private static readonly Dictionary<string, IRouteable> RouterTable = new Dictionary<string, IRouteable>();
    }
}
