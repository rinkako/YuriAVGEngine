using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lyyneheym.LyyneheymCore.Utils
{
    /// <summary>
    /// 输入输出类：管理整个游戏环境的IO动作
    /// </summary>
    public sealed class IOUtils
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
    }
}
