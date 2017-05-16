using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Yuri.Hemerocallis.Entity;
using Yuri.Hemerocallis.Utils;

namespace Yuri.Hemerocallis
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 程序初始化过程
        /// </summary>
        static App()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(App.BaseURL);
            var dirs = dirInfo.GetDirectories();
            bool existAppDataFlag = dirs.Any(td => String.Equals(td.Name, App.AppDataDirectory, StringComparison.OrdinalIgnoreCase));
            if (existAppDataFlag)
            {
                // 读取配置信息
                var ctr = Controller.GetInstance();
                ctr.ReadConfigToMemory();
                DirectoryInfo appDataDirInfo = new DirectoryInfo(App.ParseURIToURL(App.AppDataDirectory));
                var appDirs = appDataDirInfo.GetDirectories();
                bool bgDirExistFlag = appDirs.Any(adirs => String.Equals(adirs.Name, App.AppearanceDirectory, StringComparison.OrdinalIgnoreCase));
                if (!bgDirExistFlag)
                {
                    Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory));
                    ctr.ConfigDesc.BgType = AppearanceBackgroundType.Default;
                    ctr.WriteConfigToSteady();
                }
                // 读取书籍信息
                var bkFiles = appDataDirInfo.GetFiles();
                var openList = new Queue<HArticle>();
                foreach (var bk in bkFiles)
                {
                    if (String.Equals(bk.Extension, "." + App.AppBookDataExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var bkObj = IOUtil.Unserialization(bk.FullName) as HBook;
                            ctr.BookVector.Add(new BookCacheDescriptor(bkObj, false));
                            openList.Enqueue(bkObj.HomePage);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Unable to load book project: " + bk.FullName + Environment.NewLine + ex);
                        }
                    }
                }
                // 处理文章的引用
                while (openList.Any())
                {
                    var curNode = openList.Dequeue();
                    ctr.ArticleDict[curNode.Id] = curNode;
                    foreach (var sn in curNode.ChildrenList)
                    {
                        openList.Enqueue(sn);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory));
                Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory));
            }
        }

        /// <summary>
        /// 将相对路径映射到绝对路径
        /// </summary>
        /// <param name="uri">相对路径项</param>
        /// <returns>绝对路径字符串</returns>
        public static string ParseURIToURL(params string[] uri)
        {
            StringBuilder sb = new StringBuilder(App.BaseURL);
            foreach (var uriItem in uri)
            {
                sb.Append($"\\{uriItem}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 程序根目录
        /// </summary>
        public static readonly string BaseURL = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 程序配置文件夹名
        /// </summary>
        public static readonly string AppDataDirectory = ".YuriHemerocallis";

        /// <summary>
        /// 程序外观图片缓存文件夹名
        /// </summary>
        public static readonly string AppearanceDirectory = ".YuriHemerocallisAppearance";

        /// <summary>
        /// 书籍工程文件后缀名
        /// </summary>
        public static readonly string AppBookDataExtension = "hbk";

        /// <summary>
        /// 程序配置文件名
        /// </summary>
        public static readonly string AppConfigFilename = "AppConfig.dat";
    }
}
