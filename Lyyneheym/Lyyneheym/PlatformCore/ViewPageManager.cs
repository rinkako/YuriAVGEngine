using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using Yuri.Utils;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 前端页面管理器类：为游戏窗体提供对前端页的通用接口
    /// </summary>
    internal static class ViewPageManager
    {
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
        /// 初始化第一个页面
        /// </summary>
        /// <param name="fp">首页引用</param>
        public static void InitFirstPage(Page fp)
        {
            ViewPageManager.PageCallStack.Push(fp);
        }

        /// <summary>
        /// 导航到目标页面
        /// </summary>
        /// <param name="toPageName">目标页面在页管理器里的唯一标识符</param>
        public static void NavigateTo(string toPageName)
        {
            var rp = ViewPageManager.RetrievePage(toPageName);
            if (rp != null && ViewPageManager.CurrentPage != null)
            {
                NavigationService.GetNavigationService(ViewPageManager.CurrentPage)?.Navigate(rp);
                ViewPageManager.PageCallStack.Push(rp);
            }
            else
            {
                CommonUtils.ConsoleLine(string.Format("Cannot find page: {0}, Navigation service ignored.", toPageName),
                    "ViewPageManager", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 返回导航前的页面
        /// </summary>
        public static void GoBack()
        {
            if (ViewPageManager.CurrentPage != null && ViewPageManager.CurrentPage.NavigationService != null &&
                ViewPageManager.CurrentPage.NavigationService.CanGoBack)
            {
                ViewPageManager.CurrentPage.NavigationService.GoBack();
                ViewPageManager.PageCallStack.Pop();
            }
            else
            {
                CommonUtils.ConsoleLine(string.Format("Cannot go back from page: {0}, Navigation service ignored.", ViewPageManager.CurrentPage?.Name),
                    "ViewPageManager", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 获取当前是否位于主舞台页面
        /// </summary>
        /// <returns>是否在主舞台</returns>
        public static bool IsAtMainStage()
        {
            return ViewPageManager.PageCallStack.Count > 0 && 
                (ViewPageManager.PageCallStack.Peek() is PageView.Stage3D || ViewPageManager.PageCallStack.Peek() is PageView.Stage2D);
        }

        /// <summary>
        /// 获取当前呈现在屏幕上的页面
        /// </summary>
        public static Page CurrentPage => ViewPageManager.PageCallStack.Count > 0 ? ViewPageManager.PageCallStack.Peek() : null;

        /// <summary>
        /// 页面转移栈
        /// </summary>
        private static readonly Stack<Page> PageCallStack = new Stack<Page>();

        /// <summary>
        /// 前端页引用字典
        /// </summary>
        private static readonly Dictionary<string, Page> pageDict = new Dictionary<string, Page>();
    }
}
