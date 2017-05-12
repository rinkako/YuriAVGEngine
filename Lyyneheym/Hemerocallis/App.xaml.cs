using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Hemerocallis
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            App.BaseURL = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(App.BaseURL);
            var dirs = dirInfo.GetDirectories();
            bool existAppDataFlag = dirs.Any(td => String.Equals(td.Name, App.AppDataDirectory, StringComparison.OrdinalIgnoreCase));
            if (existAppDataFlag)
            {
                // TODO 在这里载入config包装
                DirectoryInfo appDataDirInfo = new DirectoryInfo(App.ParseURIToURL(App.AppDataDirectory));
                var appDirs = appDataDirInfo.GetDirectories();
                bool bgDirExistFlag = appDirs.Any(adirs => String.Equals(adirs.Name, App.AppearanceDirectory, StringComparison.OrdinalIgnoreCase));
                if (!bgDirExistFlag)
                {
                    Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory));
                    // TODO 这里要修改config包装是默认背景样式
                }
                var bkFiles = appDataDirInfo.GetFiles();
                foreach (var bk in bkFiles)
                {
                    if (String.Equals(bk.Extension, App.AppBookDataExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO 在这里载入书籍工程
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory));
                Directory.CreateDirectory(App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory));
            }
        }

        public static string ParseURIToURL(params string[] uri)
        {
            StringBuilder sb = new StringBuilder(App.BaseURL);
            foreach (var uriItem in uri)
            {
                sb.Append($"\\{uriItem}");
            }
            return sb.ToString();
        }

        public static string BaseURL;

        public static readonly string AppDataDirectory = ".YuriHemerocallis";

        public static readonly string AppearanceDirectory = ".YuriHemerocallisAppearance";

        public static readonly string AppBookDataExtension = "hemebk";
    }
}
