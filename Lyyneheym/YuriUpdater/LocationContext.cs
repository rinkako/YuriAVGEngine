using System;

namespace YuriUpdater
{
    /// <summary>
    /// 路径上下文
    /// </summary>
    internal class LocationContext
    {
        /// <summary>
        /// 版本文件的相对地址
        /// </summary>
        public static readonly string VersionFilePath = "version.yuri";

        /// <summary>
        /// 更新缓存目录
        /// </summary>
        public static readonly string UpdateDirectoryPath = "update";

        /// <summary>
        /// 版本获取路径
        /// </summary>
        public static readonly string VersionFetchingURL = "http://211.159.166.251:10533/version/get/";

        /// <summary>
        /// 把一个相对URI转化为绝对路径
        /// </summary>
        /// <param name="uri">相对程序运行目录的相对路径</param>
        /// <returns>绝对路径</returns>
        public static string ParseURItoURL(string uri)
        {
            return AppDomain.CurrentDomain.BaseDirectory + uri;
        }
    }
}
