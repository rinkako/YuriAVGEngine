using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Lyyneheym.LyyneheymCore.Utils;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>资源管理器类：负责维护游戏的资源</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// 获得一张指定背景图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetBackground(string sourceName)
        {
            return this.GetGraphicSprite(sourceName, ResourceType.Background);
        }

        /// <summary>
        /// 获得一张指定立绘图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetCharacterStand(string sourceName)
        {
            return this.GetGraphicSprite(sourceName, ResourceType.Stand);
        }

        /// <summary>
        /// 获得一张指定图片的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetPicture(string sourceName)
        {
            return this.GetGraphicSprite(sourceName, ResourceType.Pictures);
        }

        /// <summary>
        /// 获得一个指定BGM音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle, long> GetBGM(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.BGM);
        }

        /// <summary>
        /// 获得一个指定BGS音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle, long> GetBGS(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.BGS);
        }

        /// <summary>
        /// 获得一个指定SE音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle, long> GetSE(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.SE);
        }

        /// <summary>
        /// 获得一个指定Vocal音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle, long> GetVocal(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.VOCAL);
        }

        /// <summary>
        /// 从资源文件中获取图片资源并返回精灵对象
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <returns>该资源的精灵</returns>
        private MySprite GetGraphicSprite(string sourceName, ResourceType rtype)
        {
            MySprite sprite = new MySprite();
            string DevURI = null, PackURI = null;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.Background:
                    DevURI = GlobalDataContainer.DevURI_PA_BACKGROUND;
                    PackURI = GlobalDataContainer.PackURI_PA_BACKGROUND;
                    break;
                case ResourceType.Stand:
                    DevURI = GlobalDataContainer.DevURI_PA_CHARASTAND;
                    PackURI = GlobalDataContainer.PackURI_PA_CHARASTAND;
                    break;
                case ResourceType.Pictures:
                    DevURI = GlobalDataContainer.DevURI_PA_PICTURES;
                    PackURI = GlobalDataContainer.PackURI_PA_PICTURES;
                    break;
                default:
                    return null;
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[DevURI][sourceName];
                byte[] ob = PackageUtils.getObjectBytes(IOUtils.ParseURItoURL(PackURI + GlobalDataContainer.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(ms);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_PICTUREASSETS, DevURI, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    Uri bg = new Uri(furi, UriKind.RelativeOrAbsolute);
                    sprite.Init(bg);
                }
                else
                {
                    throw new Exception("文件不存在：" + sourceName);
                }
            }
            return sprite;
        }

        /// <summary>
        /// 从资源文件中获取声音资源并返回句柄
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        private KeyValuePair<GCHandle, long> GetMusicGCHandleLengthKVP(string sourceName, ResourceType rtype)
        {
            string DevURI = null, PackURI = null;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.BGM:
                    DevURI = GlobalDataContainer.DevURI_SO_BGM;
                    PackURI = GlobalDataContainer.PackURI_SO_BGM;
                    break;
                case ResourceType.BGS:
                    DevURI = GlobalDataContainer.DevURI_SO_BGS;
                    PackURI = GlobalDataContainer.PackURI_SO_BGS;
                    break;
                case ResourceType.SE:
                    DevURI = GlobalDataContainer.DevURI_SO_SE;
                    PackURI = GlobalDataContainer.PackURI_SO_SE;
                    break;
                case ResourceType.VOCAL:
                    DevURI = GlobalDataContainer.DevURI_SO_VOCAL;
                    PackURI = GlobalDataContainer.PackURI_SO_VOCAL;
                    break;
                default:
                    throw new Exception("调用了音乐获取方法，但却不是获取音乐资源");
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[DevURI][sourceName];
                GCHandle ptr = PackageUtils.getObjectIntPtr(IOUtils.ParseURItoURL(PackURI + GlobalDataContainer.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                return new KeyValuePair<GCHandle, long>(ptr, sourceLocation.Value);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_SOUND, DevURI, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    byte[] bytes = File.ReadAllBytes(IOUtils.ParseURItoURL(furi));
                    return new KeyValuePair<GCHandle, long>(GCHandle.Alloc(bytes, GCHandleType.Pinned), bytes.Length);
                }
                else
                {
                    throw new Exception("文件不存在：" + sourceName);
                }
            }
        }

        /// <summary>
        /// 初始化资源字典
        /// </summary>
        /// <returns>操作成功与否</returns>
        public bool initDictionary()
        {
            return true;
        }

        private void InitDictionaryByPST(List<string> pstList)
        {
            foreach (string pstPath in pstList)
            {
                FileStream fs = new FileStream(pstPath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                // 读取头部信息
                string header = sr.ReadLine();
                string[] headerItem = header.Split('@');
                if (headerItem.Length != GlobalDataContainer.PackHeaderItemNum && headerItem[0] != GlobalDataContainer.PackHeader)
                {
                    Console.WriteLine("Ignore pack: " + pstPath);
                    continue;
                }
                int fileCount = 0;
                string resourceType = "";
                string key = "";
                string version = "";
                try
                {
                    fileCount = Convert.ToInt32(headerItem[1]);
                    resourceType = headerItem[2];
                    string[] keyItem = headerItem[3].Split('?');
                    if (keyItem.Length != 2)
                    {
                        version = "0";
                    }
                    key = keyItem[0];
                    version = keyItem[1];
                    if (key != GlobalDataContainer.GAME_KEY)
                    {
                        Console.WriteLine("Ignore pack(key failure): " + pstPath);
                        continue;
                    }
                }

            }

            sr.Close();
            fs.Close();
        }

        /// <summary>
        /// 在根目录下搜索资源信息文件
        /// </summary>
        /// <returns>资源信息文件的路径向量</returns>
        private List<string> SearchPST()
        {
            List<string> resContainer = new List<string>();
            DirectoryInfo rootDirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            foreach (FileInfo file in rootDirInfo.GetFiles())
            {
                if (file.Extension == ".pst")
                {
                    resContainer.Add(file.FullName);
                }
            }
            return resContainer;
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>资源管理器的唯一实例</returns>
        public static ResourceManager getInstance()
        {
            return null == synObject ? synObject = new ResourceManager() : synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ResourceManager()
        {
            resourceTable = new Dictionary<string, Dictionary<string, KeyValuePair<long, long>>>();
        }

        // 唯一实例量
        private static ResourceManager synObject = null;

        // 资源字典（之后要变成私有）
        public Dictionary<string, Dictionary<string, KeyValuePair<long, long>>> resourceTable = null;
    }

    /// <summary>
    /// 枚举：资源类型
    /// </summary>
    internal enum ResourceType
    {
        Unknown,
        Pictures,
        Stand,
        Background,
        BGM,
        BGS,
        SE,
        VOCAL
    }
}
