using System.Collections.Generic;
using System.Windows.Controls;
using Yuri.Utils;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 前端页面管理器类：为游戏窗体提供对前端页的通用接口
    /// </summary>
    internal static class ViewPageManager
    {
        /// <summary>
        /// 静态构造器
        /// </summary>
        static ViewPageManager()
        {
            ViewPageManager.pageDict = new Dictionary<string, Page>();
        }

        /// <summary>
        /// 在页面管理器中注册一个页面
        /// </summary>
        /// <param name="pageId">页面唯一标识符</param>
        /// <param name="pageRef">页引用</param>
        /// <returns>是否发生了覆盖</returns>
        public static bool RegisterPage(string pageId, Page pageRef)
        {
            CommonUtils.ConsoleLine("Register Page: " + pageId, "ViewPage Manager", OutputStyle.Important);
            bool rFlag = ViewPageManager.pageDict.ContainsKey(pageId);
            ViewPageManager.pageDict[pageId] = pageRef;
            return rFlag;
        }

        /// <summary>
        /// 通过页面的唯一标识符获取页面的引用
        /// </summary>
        /// <param name="pageId">页面唯一标识符</param>
        /// <returns>页引用</returns>
        public static Page RetrievePage(string pageId)
        {
            return ViewPageManager.pageDict.ContainsKey(pageId) ? ViewPageManager.pageDict[pageId] : null;
        }

        /// <summary>
        /// 清空页面管理器中储存的页引用
        /// </summary>
        public static void Clear()
        {
            ViewPageManager.pageDict.Clear();
        }

        /// <summary>
        /// 前端页引用字典
        /// </summary>
        private static readonly Dictionary<string, Page> pageDict;
    }
}
