using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Yuri.Utils;

namespace Yuri.PlatformCore.Net
{
    /// <summary>
    /// 为游戏引擎提供HTTP服务
    /// </summary>
    internal class YuriHttpClient
    {
        /// <summary>
        /// 对指定的URL做POST动作
        /// </summary>
        /// <remarks>该方法是同步的，线程安全的，如果需要异步请利用信号分发机制</remarks>
        /// <param name="url">要访问的URL</param>
        /// <param name="argsDict">POST的参数字典</param>
        /// <param name="result">[out] URL的反馈</param>
        /// <param name="encoding">编码器</param>
        /// <returns>操作是否成功</returns>
        public static bool PostData(string url, Dictionary<string, string> argsDict, out string result, Encoding encoding = null)
        {
            try
            {
                var client = new WebClient();
                // 提交的内容
                StringBuilder sb = new StringBuilder();
                if (argsDict != null)
                {
                    foreach (var arg in argsDict)
                    {
                        sb.Append("&" + arg.Key + "=" + arg.Value);
                    }
                }
                if (sb.Length > 0)
                {
                    sb = sb.Remove(0, 1);
                }
                // 编码
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                byte[] postData = encoding.GetBytes(sb.ToString());
                // POST
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("ContentLength", postData.Length.ToString());
                byte[] respondData = client.UploadData(url, "POST", postData);
                result = encoding.GetString(respondData);
                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Post Data to URL " + url + " failed." + Environment.NewLine + ex,
                    "YuriHttpClient", LogLevel.Error);
                result = null;
                return false;
            }
        }

        /// <summary>
        /// 访问指定的URL并下载页面中的内容为字符串
        /// </summary>
        /// <remarks>该方法是同步的，线程安全的，如果需要异步请利用信号分发机制</remarks>
        /// <param name="url">要访问的URL</param>
        /// <param name="result">[out] 获得的字符串</param>
        /// <returns>操作是否成功</returns>
        public static bool FetchString(string url, out string result)
        {
            try
            {
                var wb = new WebClient();
                result = wb.DownloadString(url);
                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Fetch String from URL " + url + " failed." + Environment.NewLine + ex,
                    "YuriHttpClient", LogLevel.Error);
                result = null;
                return false;
            }
        }

        /// <summary>
        /// 从指定的URL下载文件
        /// </summary>
        /// <remarks>这个方法是同步的，如果需要异步请利用信号分发机制</remarks>
        /// <param name="url">要下载的文件的URL</param>
        /// <param name="saveFileRelativePath">保存文件相对程序根目录的路径</param>
        /// <param name="timeout">请求超时的毫秒数</param>
        /// <param name="bufferSize">缓冲区字节数</param>
        /// <returns>操作是否成功</returns>
        public static bool Download(string url, string saveFileRelativePath, int timeout = 5000, int bufferSize = 2048)
        {
            try
            {
                using (FileStream fs = new FileStream(IOUtils.ParseURItoURL(saveFileRelativePath), FileMode.Create, FileAccess.Write))
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();
                    request.Timeout = timeout;
                    Stream responseStream = response.GetResponseStream();
                    var bufferBytes = new byte[Math.Max(128, bufferSize)];
                    int bytesRead;
                    do
                    {
                        bytesRead = responseStream.Read(bufferBytes, 0, bufferBytes.Length);
                        fs.Write(bufferBytes, 0, bytesRead);
                    } while (bytesRead > 0);
                    fs.Flush();
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Download from URL " + url + " failed." + Environment.NewLine + ex,
                    "YuriHttpClient", LogLevel.Error);
                return false;
            }
        }
    }
}
