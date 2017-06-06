using System;
using System.IO;
using System.Net;
using Yuri.Utils;

namespace Yuri.PlatformCore.Net
{
    /// <summary>
    /// 为游戏引擎提供FTP服务
    /// </summary>
    internal class YuriFtpClient
    {
        /// <summary>
        /// 将一个文件上载到FTP
        /// </summary>
        /// <remarks>该方法是同步的，线程安全的，如果需要异步请利用信号分发机制</remarks>
        /// <param name="relativeFilePath">要上传的文件相对于游戏目录的相对路径</param>
        /// <param name="targetPath">文件在目标主机上的保存路径</param>
        /// <param name="hostname">主机域名</param>
        /// <param name="username">创建FTP连接的用户名</param>
        /// <param name="password">创建FTP连接的用户名</param>
        /// <param name="bufferSize">缓冲区的字节数</param>
        /// <returns>操作是否成功</returns>
        public static bool Upload(string relativeFilePath, string targetPath, string hostname, string username, string password, int bufferSize = 2048)
        {
            var url = "ftp://" + hostname + "/" + targetPath;
            try
            {
                FileInfo fileinfo = new FileInfo(IOUtils.ParseURItoURL(relativeFilePath));
                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(url);
                ftp.Credentials = new NetworkCredential(username, password);
                ftp.KeepAlive = false;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.ContentLength = fileinfo.Length;
                var content = new byte[Math.Max(128, bufferSize)];
                using (FileStream fs = fileinfo.OpenRead())
                {
                    using (Stream rs = ftp.GetRequestStream())
                    {
                        int dataRead;
                        do
                        {
                            dataRead = fs.Read(content, 0, bufferSize);
                            rs.Write(content, 0, dataRead);
                        } while (dataRead > 0);
                        rs.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Upload to URL " + url + " failed." + Environment.NewLine + ex,
                    "YuriFtpClient", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 将一个文件从FTP下载
        /// </summary>
        /// <remarks>该方法是同步的，线程安全的，如果需要异步请利用信号分发机制</remarks>
        /// <param name="relativeFilePath">要下载的文件相对于游戏目录的保存路径</param>
        /// <param name="targetPath">文件在目标主机上的保存路径</param>
        /// <param name="hostname">主机域名</param>
        /// <param name="username">创建FTP连接的用户名</param>
        /// <param name="password">创建FTP连接的用户名</param>
        /// <returns>操作是否成功</returns>
        public static bool Download(string relativeFilePath, string targetPath, string hostname, string username, string password)
        {
            var url = "ftp://" + hostname + "/" + targetPath;
            try
            {
                FileStream outputStream = new FileStream(IOUtils.ParseURItoURL(relativeFilePath), FileMode.Create);
                var reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(url));
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                var readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Download from URL " + url + " failed." + Environment.NewLine + ex,
                    "YuriFtpClient", LogLevel.Error);
                return false;
            }
        }
    }
}
