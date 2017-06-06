using System;

namespace Yuri.PlatformCore.Net
{
    /// <summary>
    /// 为引擎提供Http服务器服务
    /// </summary>
    internal static class HttpServerRouterHandler
    {
        /// <summary>
        /// 初始化Http服务器
        /// </summary>
        /// <returns>是否初始化成功</returns>
        public static bool Init()
        {
            if (!HttpServerRouterHandler.IsInitialized)
            {
                HttpServerRouterHandler.IsInitialized = true;
                HttpServerRouterHandler.serverObject = new YuriHttpServer();
                HttpServerRouterHandler.serverObject.BeginAsyncAccept();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// HTTP服务器对象
        /// </summary>
        private static YuriHttpServer serverObject;

        /// <summary>
        /// 获取Http服务器是否已经初始化
        /// </summary>
        public static bool IsInitialized { get; private set; } = false;
    }
}
