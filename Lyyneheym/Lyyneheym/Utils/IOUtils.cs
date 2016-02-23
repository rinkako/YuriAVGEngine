using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Yuri.Utils
{
    /// <summary>
    /// 输入输出类：管理整个游戏环境的IO动作
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// 把一个相对URI转化为绝对路径
        /// </summary>
        /// <param name="uri">相对程序运行目录的相对路径</param>
        /// <returns>绝对路径</returns>
        public static string ParseURItoURL(string uri)
        {
            return IOUtils.JoinPath(Environment.CurrentDirectory, uri);
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
        /// 把一个实例序列化
        /// </summary>
        /// <param name="instance">类的实例</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>操作成功与否</returns>
        public static bool serialization(object instance, string savePath)
        {
            try
            {
                Stream myStream = File.Open(savePath, FileMode.Create);
                if (myStream == null)
                {
                    throw new IOException();
                }
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(myStream, instance);
                myStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// 把二进制文件反序列化
        /// </summary>
        /// <param name="loadPath">二进制文件路径</param>
        /// <returns>类的实例</returns>
        public static object unserialization(string loadPath)
        {
            try
            {
                Stream s = File.Open(loadPath, FileMode.Open);
                if (s == null)
                {
                    throw new IOException();
                }
                BinaryFormatter bf = new BinaryFormatter();
                object ob = bf.Deserialize(s);
                s.Close();
                return ob;
            }
            catch
            {
                return null;
            }
        }
    }
}
