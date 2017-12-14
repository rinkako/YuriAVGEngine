using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Yuri.YuriHalation.HalationCore
{
    /// <summary>
    /// 文件管理器：管理Halation的文件IO动作
    /// </summary>
    internal static class FileManager
    {
        /// <summary>
        /// 为工程初始化目录
        /// </summary>
        /// <param name="path">要建立的根路径</param>
        public static void CreateInitFolder(string path)
        {
            // 建立根目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 复制样本工程
            FileManager.DirectoryCopy(System.Windows.Forms.Application.StartupPath + "\\SampleProject", path);
        }

        /// <summary>
        /// 递归拷贝文件夹
        /// </summary>
        /// <param name="sourceDirectory">源文件夹</param>
        /// <param name="targetDirectory">目标文件夹</param>
        private static void DirectoryCopy(string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory) || !Directory.Exists(targetDirectory))
            {
                return;
            }
            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirectory);
            FileInfo[] fileInfo = sourceInfo.GetFiles();
            foreach (FileInfo fiTemp in fileInfo)
            {
                File.Copy(sourceDirectory + "\\" + fiTemp.Name, targetDirectory + "\\" + fiTemp.Name, true);
            }
            DirectoryInfo[] diInfo = sourceInfo.GetDirectories();
            foreach (DirectoryInfo diTemp in diInfo)
            {
                string sourcePath = diTemp.FullName;
                string targetPath = diTemp.FullName.Replace(sourceDirectory, targetDirectory);
                Directory.CreateDirectory(targetPath);
                DirectoryCopy(sourcePath, targetPath);
            }
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
        public static object Unserialization(string loadPath)
        {
            try
            {
                Stream s = File.Open(loadPath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                object ob = bf.Deserialize(s);
                s.Close();
                return ob;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 为项目保存全局配置信息
        /// </summary>
        /// <param name="savePath">文件路径</param>
        /// <param name="kvpList">config包装的成员变量反射向量</param>
        public static void SaveConfigData(string savePath, List<Tuple<string, object, int>> kvpList)
        {
            FileStream fs = new FileStream(savePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var kvp in kvpList)
            {
                sw.WriteLine("{0}=>{1}=>{2}", kvp.Item1, kvp.Item2, kvp.Item3);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 为项目读取全局配置信息
        /// </summary>
        /// <param name="loadPath">文件路径</param>
        /// <returns>行分割对象向量</returns>
        public static List<Tuple<string, string, int>> LoadConfigData(string loadPath)
        {
            FileStream fs = new FileStream(loadPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            var resList = new List<Tuple<string, string, int>>();
            while (sr.EndOfStream == false)
            {
                var lineitems = sr.ReadLine().Split(new[] {"=>"}, StringSplitOptions.RemoveEmptyEntries);
                if (lineitems.Length == 3)
                {
                    resList.Add(new Tuple<string, string, int>(lineitems[0], lineitems[1], Convert.ToInt32(lineitems[2])));
                }
            }
            sr.Close();
            fs.Close();
            return resList;
        }

        /// <summary>
        /// 将向量保存为文件组
        /// </summary>
        /// <param name="savePath">保存根目录</param>
        /// <param name="postfix">文件的后缀</param>
        /// <param name="lineitems">一个键值对向量，键是文件名，值是文件内容</param>
        public static void SaveByLineItem(string savePath, string postfix, List<KeyValuePair<string, string>> lineitems)
        {
            foreach (var lineitem in lineitems)
            {
                FileStream fs = new FileStream(savePath + "\\" + lineitem.Key + postfix, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(lineitem.Value);
                sw.Close();
                fs.Close();
            }
        }

        #region 目录和字典常量
        // 图像资源目录名
        public static readonly string DevURI_RT_PICTUREASSETS = "PictureAssets";
        // 场景资源目录名
        public static readonly string DevURI_RT_SCENARIO = "Scenario";
        // 声效资源目录名
        public static readonly string DevURI_RT_SOUND = "Sound";
        // 图像->背景资源目录名
        public static readonly string DevURI_PA_BACKGROUND = "background";
        // 图像->立绘资源目录名
        public static readonly string DevURI_PA_CHARASTAND = "character";
        // 图像->图片资源目录名
        public static readonly string DevURI_PA_PICTURES = "pictures";
        // 声效->音乐资源目录名
        public static readonly string DevURI_SO_BGM = "bgm";
        // 声效->音效资源目录名
        public static readonly string DevURI_SO_BGS = "bgs";
        // 声效->声效资源目录名
        public static readonly string DevURI_SO_SE = "se";
        // 声效->语音资源目录名
        public static readonly string DevURI_SO_VOCAL = "vocal";
        #endregion
    }
}
