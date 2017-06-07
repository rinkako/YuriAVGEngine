using System;
using System.Net;


namespace Yuri.PlatformCore.Net
{
    /// <summary>
    /// 为引擎Http服务提供接口
    /// </summary>
    public interface IYuriHttpServer
    {
        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="response">HTTP响应</param>
        void OnGet(HttpListenerRequest request, HttpListenerResponse response);

        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="context">HTTP报文上下文</param>
        void OnPost(HttpListenerContext context);
    }
}
