using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
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
        /// 获得指定名称的场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>场景实例</returns>
        public Scene GetScene(string sceneName)
        {
            if (this.sceneTable.ContainsKey(sceneName))
            {
                return this.sceneTable[sceneName];
            }
            return null;
        }

        /// <summary>
        /// 获得所有场景实例
        /// </summary>
        /// <returns>场景实例向量</returns>
        public List<Scene> GetAllScene()
        {
            List<Scene> resVec = new List<Scene>();
            foreach (var sc in this.sceneTable)
            {
                resVec.Add(sc.Value);
            }
            return resVec;
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
                sprite.Init(sourceName, rtype, ms);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_PICTUREASSETS, DevURI, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    Uri bg = new Uri(furi, UriKind.RelativeOrAbsolute);
                    sprite.Init(sourceName, rtype, bg);
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
        /// 根据PST向量载入资源到字典
        /// </summary>
        /// <param name="threadID">线程ID</param>
        private void InitDictionaryByPST(object threadID)
        {
            int tid = (int)threadID;
            while (true)
            {
                string pstPath = "";
                lock (this.pendingPst)
                {
                    if (this.pendingPst.Count != 0)
                    {
                        pstPath = this.pendingPst.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
                if (pstPath == "")
                {
                    continue;
                }
                DebugUtils.AsyncConsoleLine(String.Format("Loading PST Resource From \"{0}\" At thread {1}", pstPath, tid), "ResourceManager", this.consoleMutex, OutputStyle.Normal);
                // 开始处理文件
                FileStream fs = new FileStream(pstPath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                // 读取头部信息
                string header = sr.ReadLine();
                string[] headerItem = header.Split('@');
                if (headerItem.Length != GlobalDataContainer.PackHeaderItemNum && headerItem[0] != GlobalDataContainer.PackHeader)
                {
                    DebugUtils.AsyncConsoleLine(String.Format("Ignored Pack (Bad Header): {0}", pstPath), "ResourceManager", this.consoleMutex, OutputStyle.Warning);
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
                        DebugUtils.AsyncConsoleLine(String.Format("Ignored Pack (Key Failed): {0}", pstPath), "ResourceManager", this.consoleMutex, OutputStyle.Warning);
                        continue;
                    }
                    else if (Convert.ToDouble(version) < Convert.ToDouble(GlobalDataContainer.GAME_VERSION))
                    {
                        DebugUtils.AsyncConsoleLine(String.Format("Ignored Pack (Version Is Elder): {0}", pstPath), "ResourceManager", this.consoleMutex, OutputStyle.Warning);
                        continue;
                    }
                    // 通过检验的包才载入资源字典
                    this.AddResouceTable(resourceType);
                    int lineEncounter = 0;
                    while (lineEncounter < fileCount)
                    {
                        lineEncounter++;
                        string[] lineitem = sr.ReadLine().Split(':');
                        if (lineitem[0] == GlobalDataContainer.PackEOF)
                        {
                            DebugUtils.AsyncConsoleLine(String.Format("Occured EOF: {0}", pstPath), "ResourceManager", this.consoleMutex, OutputStyle.Warning);
                            break;
                        }
                        if (lineitem.Length != 3)
                        {
                            DebugUtils.AsyncConsoleLine(String.Format("Igonred line(Bad lineitem): {0}, In file: {1}", lineEncounter, pstPath), "ResourceManager", this.consoleMutex, OutputStyle.Warning);
                            continue;
                        }
                        string srcName = lineitem[0];
                        long srcOffset = Convert.ToInt64(lineitem[1]);
                        long srcLength = Convert.ToInt64(lineitem[2]);
                        this.AddResource(resourceType, srcName, srcOffset, srcLength);
                    }
                    DebugUtils.AsyncConsoleLine(String.Format("Finish Dictionary Init From \"{0}\" At thread {1}", pstPath, tid), "ResourceManager", this.consoleMutex, OutputStyle.Normal);
                }
                catch (Exception ex)
                {
                    DebugUtils.AsyncConsoleLine(ex.ToString(), "ResourceManager / CLR", this.consoleMutex, OutputStyle.Error);                    
                }
                sr.Close();
                fs.Close();
            }
            // 递增回到等待
            this.threadFinishCounter++;
            DebugUtils.AsyncConsoleLine(String.Format("At ResMana thread {0}, Waiting for callback", tid), "ResourceManager", this.consoleMutex, OutputStyle.Important);
        }

        /// <summary>
        /// 在根目录下搜索资源信息文件
        /// </summary>
        /// <returns>资源信息文件的路径队列</returns>
        private Queue<string> SearchPST()
        {
            Queue<string> resContainer = new Queue<string>();
            DirectoryInfo rootDirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            foreach (FileInfo file in rootDirInfo.GetFiles())
            {
                if (file.Extension == ".pst")
                {
                    resContainer.Enqueue(file.FullName);
                }
            }
            return resContainer;
        }

        /// <summary>
        /// 为资源表增加一个资源
        /// </summary>
        /// <param name="typeKey">资源在表中的类型</param>
        /// <param name="resourceKey">资源名称</param>
        /// <param name="offset">资源在包中的偏移</param>
        /// <param name="length">资源在包中的长度</param>
        /// <returns>操作成功与否</returns>
        private bool AddResource(string typeKey, string resourceKey, long offset, long length)
        {
            if (this.resourceTable.ContainsKey(typeKey))
            {
                if (!this.resourceTable[typeKey].ContainsKey(resourceKey))
                {
                    this.resourceTable[typeKey][resourceKey] = new KeyValuePair<long, long>(offset, length);
                    return true;
                }
                else
                {
                    DebugUtils.ConsoleLine(String.Format("Resource already exist, type: {0}, name: {1}", typeKey, resourceKey), "ResourceManager", OutputStyle.Warning);
                }
            }
            return false;
        }

        /// <summary>
        /// 为资源字典添加一个资源表
        /// </summary>
        /// <param name="resTableName">资源表名称</param>
        /// <returns>操作成功与否</returns>
        private bool AddResouceTable(string resTableName)
        {
            lock (this.resourceTable)
            {
                if (!this.resourceTable.ContainsKey(resTableName))
                {
                    this.resourceTable.Add(resTableName, new Dictionary<string, KeyValuePair<long, long>>());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 开启多线程将PST文件加载到资源字典中
        /// </summary>
        /// <param name="threadNum">线程数量</param>
        private void InitDictionary(int threadNum = 4)
        {
            // 控制线程数量
            if (threadNum < 1 || threadNum > 8)
            {
                threadNum = 4;
            }
            this.pendingPst = this.SearchPST();
            // 开始线程处理
            List<Thread> threadPool = new List<Thread>();
            this.threadFinishCounter = 0;
            for (int t = 0; t < threadNum; t++)
            {
                threadPool.Add(new Thread(new ParameterizedThreadStart(this.InitDictionaryByPST)));
                threadPool[t].IsBackground = true;
                threadPool[t].Start(t);
            }
            // 等待线程回调
            while (this.threadFinishCounter < threadNum);
        }

        /// <summary>
        /// 把场景文件恢复为实例
        /// </summary>
        private void InitScenario()
        {
            List<Scene> sceneList = Lyyneheym.LyyneheymCore.ILPackage.ILConvertor.GetInstance().Dash(GlobalDataContainer.DevURI_RT_SCENARIO);
            foreach (Scene sc in sceneList)
            {
                if (this.sceneTable.ContainsKey(sc.scenario))
                {
                    DebugUtils.ConsoleLine(String.Format("Scene already exist: {0}, new one will replace the elder one", sc.scenario),
                        "ResourceManager", OutputStyle.Warning);
                }
                this.sceneTable[sc.scenario] = sc;
            }
            DebugUtils.ConsoleLine(String.Format("Finish Load Scenario, Total: {0}", sceneList.Count), "ResourceManager", OutputStyle.Normal);
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>资源管理器的唯一实例</returns>
        public static ResourceManager GetInstance()
        {
            return null == synObject ? synObject = new ResourceManager() : synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ResourceManager()
        {
            this.resourceTable = new Dictionary<string, Dictionary<string, KeyValuePair<long, long>>>();
            this.sceneTable = new Dictionary<string, Scene>();
            this.InitDictionary();
            this.InitScenario();
        }

        /// <summary>
        /// 唯一实例量
        /// </summary>
        private static ResourceManager synObject = null;

        /// <summary>
        /// 封包资源字典
        /// </summary>
        private Dictionary<string, Dictionary<string, KeyValuePair<long, long>>> resourceTable = null;

        /// <summary>
        /// 场景字典
        /// </summary>
        private Dictionary<string, Scene> sceneTable = null;

        /// <summary>
        /// 等待处理的pst队列
        /// </summary>
        private Queue<string> pendingPst;

        /// <summary>
        /// 进程等待回调计数
        /// </summary>
        private int threadFinishCounter;

        /// <summary>
        /// 控制台互斥量
        /// </summary>
        private Mutex consoleMutex = new Mutex();
    }

    /// <summary>
    /// 枚举：资源类型
    /// </summary>
    public enum ResourceType
    {
        Unknown,
        Pictures,
        Stand,
        Background,
        BGM,
        BGS,
        SE,
        VOCAL,
        MessageLayerBackground
    }
}
