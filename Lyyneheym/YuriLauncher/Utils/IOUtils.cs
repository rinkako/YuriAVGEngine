using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Yuri.YuriLauncher.Utils
{
    /// <summary>
    /// 输入输出类：管理整个游戏环境的IO动作
    /// </summary>
    internal static class IOUtils
    {
        /// <summary>
        /// 把一个相对URI转化为绝对路径
        /// </summary>
        /// <param name="uri">相对程序运行目录的相对路径</param>
        /// <returns>绝对路径</returns>
        public static string ParseURItoURL(string uri)
        {
            return AppDomain.CurrentDomain.BaseDirectory + uri;
        }

        /// <summary>
        /// 把字符串用反斜杠组合成Windows风格的路径字符串
        /// </summary>
        /// <param name="uriObj">路径项目</param>
        /// <returns>组合完毕的路径字符串</returns>
        public static string JoinPath(params string[] uriObj)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < uriObj.Length - 1; i++)
            {
                sb.Append(uriObj[i] + "\\");
            }
            sb.Append(uriObj.Last());
            return sb.ToString();
        }
        
        /// <summary>
        /// 将一个剧本文件读入为一个字符串
        /// </summary>
        /// <param name="path">剧本文件的路径</param>
        /// <returns>一个以行为单位的字符串向量</returns>
        public static List<string> ReadScenarioFromFile(string path)
        {
            List<string> resVec = new List<string>();
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                resVec.Add(sr.ReadLine() + Environment.NewLine);
            }
            sr.Close();
            fs.Close();
            return resVec;
        }

        /// <summary>
        /// 获得一个文件的字节序列
        /// </summary>
        /// <param name="resourceUrl">资源的URL</param>
        /// <returns>资源的字节序列</returns>
        public static byte[] GetObjectBytes(Uri resourceUrl)
        {
            FileStream pakFs = new FileStream(resourceUrl.LocalPath, FileMode.Open);
            byte[] buffer = new byte[pakFs.Length];
            if (pakFs.Length >= Int32.MaxValue)
            {
                BinaryReader pakBr = new BinaryReader(pakFs);
                for (long i = 0; i < pakFs.Length; i++)
                {
                    buffer[i] = pakBr.ReadByte();
                }
                pakBr.Close();
            }
            else
            {
                pakFs.Read(buffer, 0, (int)pakFs.Length);
            }
            pakFs.Close();
            return buffer;
        }

        /// <summary>
        /// 把一个实例序列化
        /// </summary>
        /// <param name="instance">类的实例</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>操作成功与否</returns>
        public static bool Serialization(object instance, string savePath)
        {
            try
            {
                Stream myStream = File.Open(savePath, FileMode.Create);
                var bf = new BinaryFormatter();
                bf.Serialize(myStream, instance);
                myStream.Close();
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Serialization failed. " + ex.ToString(), "IOUtils", OutputStyle.Error);
                throw;
            }
            return true;
        }

        /// <summary>
        /// 把二进制文件反序列化
        /// </summary>
        /// <param name="loadPath">二进制文件路径</param>
        /// <returns>类的实例</returns>
        public static object Unserialization(string loadPath)
        {
            try
            {
                Stream s = File.Open(loadPath, FileMode.Open);
                var bf = new BinaryFormatter();
                var ob = bf.Deserialize(s);
                s.Close();
                return ob;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Unserialization failed. " + ex.ToString(), "IOUtils", OutputStyle.Error);
                return null;
            }
        }
    }
}
