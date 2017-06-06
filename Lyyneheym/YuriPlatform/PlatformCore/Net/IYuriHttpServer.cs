using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


namespace Yuri.PlatformCore.Net
{
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
