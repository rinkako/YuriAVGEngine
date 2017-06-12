using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.VM;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>资源管理器类：负责维护游戏的资源</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class ResourceManager
    {
        /// <summary>
        /// 获得一张指定背景图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="cutRect">纹理切割矩（X值-1代表取全图）</param>
        /// <returns>该资源的精灵</returns>
        public YuriSprite GetBackground(string sourceName, Int32Rect cutRect)
        {
            return cutRect.X == -1
                ? this.GetGraphicSprite(sourceName, ResourceType.Background, null)
                : this.GetGraphicSprite(sourceName, ResourceType.Background, cutRect);
        }

        /// <summary>
        /// 获得一张指定立绘图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="cutRect">纹理切割矩（X值-1代表取全图）</param>
        /// <returns>该资源的精灵</returns>
        public YuriSprite GetCharacterStand(string sourceName, Int32Rect cutRect)
        {
            return cutRect.X == -1 
                ? this.GetGraphicSprite(sourceName, ResourceType.Stand, null)
                : this.GetGraphicSprite(sourceName, ResourceType.Stand, cutRect);
        }

        /// <summary>
        /// 获得一张指定图片的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="cutRect">纹理切割矩（X值-1代表取全图）</param>
        /// <returns>该资源的精灵</returns>
        public YuriSprite GetPicture(string sourceName, Int32Rect cutRect)
        {
            return cutRect.X == -1
                ? this.GetGraphicSprite(sourceName, ResourceType.Pictures, null)
                : this.GetGraphicSprite(sourceName, ResourceType.Pictures, cutRect);
        }

        /// <summary>
        /// 获得一个指定BGM音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该音频的托管内存流</returns>
        public MemoryStream GetBGM(string sourceName)
        {
            return this.GetMusicMemoryStream(sourceName, ResourceType.BGM);
        }

        /// <summary>
        /// 获得一个指定BGS音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该音频的托管内存流</returns>
        public MemoryStream GetBGS(string sourceName)
        {
            return this.GetMusicMemoryStream(sourceName, ResourceType.BGS);
        }

        /// <summary>
        /// 获得一个指定SE音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该音频的托管内存流</returns>
        public MemoryStream GetSE(string sourceName)
        {
            return this.GetMusicMemoryStream(sourceName, ResourceType.SE);
        }

        /// <summary>
        /// 获得一个指定Vocal音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该音频的托管内存流</returns>
        public MemoryStream GetVocal(string sourceName)
        {
            return this.GetMusicMemoryStream(sourceName, ResourceType.VOCAL);
        }

        /// <summary>
        /// 获得一个指定BGM音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle?, long> GetBGMGCHandle(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.BGM);
        }

        /// <summary>
        /// 获得一个指定BGS音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle?, long> GetBGSGCHandle(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.BGS);
        }

        /// <summary>
        /// 获得一个指定SE音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle?, long> GetSEGCHandle(string sourceName)
        {
            return this.GetMusicGCHandleLengthKVP(sourceName, ResourceType.SE);
        }

        /// <summary>
        /// 获得一个指定Vocal音频资源的内存数组
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        public KeyValuePair<GCHandle?, long> GetVocalGCHandle(string sourceName)
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
            return this.sceneTable.ContainsKey(sceneName) ? this.sceneTable[sceneName] : null;
        }

        /// <summary>
        /// 获得所有场景实例
        /// </summary>
        /// <returns>场景实例向量</returns>
        public List<Scene> GetAllScene()
        {
            return this.sceneTable.Select(sc => sc.Value).ToList();
        }

        /// <summary>
        /// 从资源文件中获取图片资源并返回精灵对象
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <param name="cutRect">纹理切割矩</param>
        /// <returns>该资源的精灵</returns>
        private YuriSprite GetGraphicSprite(string sourceName, ResourceType rtype, Int32Rect? cutRect)
        {
            if (sourceName == String.Empty) { return null; }
            YuriSprite sprite = new YuriSprite();
            string DevURI, PackURI;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.Background:
                    DevURI = GlobalConfigContext.DevURI_PA_BACKGROUND;
                    PackURI = GlobalConfigContext.PackURI_PA_BACKGROUND;
                    break;
                case ResourceType.Stand:
                    DevURI = GlobalConfigContext.DevURI_PA_CHARASTAND;
                    PackURI = GlobalConfigContext.PackURI_PA_CHARASTAND;
                    break;
                case ResourceType.Pictures:
                    DevURI = GlobalConfigContext.DevURI_PA_PICTURES;
                    PackURI = GlobalConfigContext.PackURI_PA_PICTURES;
                    break;
                default:
                    return null;
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                // 检查缓冲
                var ob = ResourceCachePool.Refer(rtype.ToString() + "->" + sourceName, ResourceCacheType.Eden);
                if (ob == null)
                {
                    var sourceSlot = this.resourceTable[DevURI][sourceName];
                    ob = PackageUtils.GetObjectBytes(sourceSlot.BindingFile, sourceName, sourceSlot.Position, sourceSlot.Length);
                    ResourceCachePool.Register(rtype.ToString() + "->" + sourceName, ob, ResourceCacheType.Eden);
                }
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(sourceName, rtype, ms, cutRect);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                // 检查缓冲
                byte[] ob = ResourceCachePool.Refer(rtype.ToString() + "->" + sourceName, ResourceCacheType.Eden);
                if (ob == null)
                {
                    string furi = IOUtils.JoinPath(GlobalConfigContext.DevURI_RT_PICTUREASSETS, DevURI, sourceName);
                    if (File.Exists(IOUtils.ParseURItoURL(furi)))
                    {
                        Uri bg = new Uri(IOUtils.ParseURItoURL(furi), UriKind.RelativeOrAbsolute);
                        ob = IOUtils.GetObjectBytes(bg);
                        ResourceCachePool.Register(rtype.ToString() + "->" + sourceName, ob, ResourceCacheType.Eden);
                    }
                    else
                    {
                        MessageBox.Show("[错误] 资源文件不存在：" + sourceName);
                        Director.GetInstance().GetMainRender().Shutdown();
                        return null;
                    }
                }
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(sourceName, rtype, ms, cutRect);
            }
            return sprite;
        }

        /// <summary>
        /// 测试某个资源是否存在
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <returns>资源是否存在</returns>
        public bool IsResourceExist(string sourceName, ResourceType rtype)
        {
            if (sourceName == String.Empty) { return false; }
            string DevURI, PackURI, SearchURI;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.Background:
                    DevURI = GlobalConfigContext.DevURI_PA_BACKGROUND;
                    PackURI = GlobalConfigContext.PackURI_PA_BACKGROUND;
                    SearchURI = GlobalConfigContext.DevURI_RT_PICTUREASSETS;
                    break;
                case ResourceType.Stand:
                    DevURI = GlobalConfigContext.DevURI_PA_CHARASTAND;
                    PackURI = GlobalConfigContext.PackURI_PA_CHARASTAND;
                    SearchURI = GlobalConfigContext.DevURI_RT_PICTUREASSETS;
                    break;
                case ResourceType.Pictures:
                    DevURI = GlobalConfigContext.DevURI_PA_PICTURES;
                    PackURI = GlobalConfigContext.PackURI_PA_PICTURES;
                    SearchURI = GlobalConfigContext.DevURI_RT_PICTUREASSETS;
                    break;
                case ResourceType.BGM:
                    DevURI = GlobalConfigContext.DevURI_SO_BGM;
                    PackURI = GlobalConfigContext.PackURI_SO_BGM;
                    SearchURI = GlobalConfigContext.DevURI_RT_SOUND;
                    break;
                case ResourceType.BGS:
                    DevURI = GlobalConfigContext.DevURI_SO_BGS;
                    PackURI = GlobalConfigContext.PackURI_SO_BGS;
                    SearchURI = GlobalConfigContext.DevURI_RT_SOUND;
                    break;
                case ResourceType.SE:
                    DevURI = GlobalConfigContext.DevURI_SO_SE;
                    PackURI = GlobalConfigContext.PackURI_SO_SE;
                    SearchURI = GlobalConfigContext.DevURI_RT_SOUND;
                    break;
                case ResourceType.VOCAL:
                    DevURI = GlobalConfigContext.DevURI_SO_VOCAL;
                    PackURI = GlobalConfigContext.PackURI_SO_VOCAL;
                    SearchURI = GlobalConfigContext.DevURI_RT_SOUND;
                    break;
                default:
                    return false;
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                return true;
            }
            // 没有封包数据再搜索开发目录
            string furi = IOUtils.JoinPath(SearchURI, DevURI, sourceName);
            var url = IOUtils.ParseURItoURL(furi);
            return File.Exists(url);
        }

        /// <summary>
        /// 获取存档屏幕截图的精灵
        /// </summary>
        /// <param name="sourceName">文件路径</param>
        /// <returns>精灵实例</returns>
        public YuriSprite GetSaveSnapshot(string sourceName)
        {
            YuriSprite sprite = new YuriSprite();
            if (File.Exists(IOUtils.ParseURItoURL(sourceName)))
            {
                Uri bg = new Uri(IOUtils.ParseURItoURL(sourceName), UriKind.RelativeOrAbsolute);
                sprite.Init(sourceName, ResourceType.SaveSnapshot, bg);
            }
            else
            {
                MessageBox.Show("[错误] 资源文件不存在：" + sourceName);
                Director.GetInstance().GetMainRender().Shutdown();
            }
            return sprite;
        }

        /// <summary>
        /// 从资源文件中获取声音资源并返回句柄
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <returns>该音频的托管内存流</returns>
        private MemoryStream GetMusicMemoryStream(string sourceName, ResourceType rtype)
        {
            if (sourceName == String.Empty)
            {
                return null;
            }
            string DevURI, PackURI;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.BGM:
                    DevURI = GlobalConfigContext.DevURI_SO_BGM;
                    PackURI = GlobalConfigContext.PackURI_SO_BGM;
                    break;
                case ResourceType.BGS:
                    DevURI = GlobalConfigContext.DevURI_SO_BGS;
                    PackURI = GlobalConfigContext.PackURI_SO_BGS;
                    break;
                case ResourceType.SE:
                    DevURI = GlobalConfigContext.DevURI_SO_SE;
                    PackURI = GlobalConfigContext.PackURI_SO_SE;
                    break;
                case ResourceType.VOCAL:
                    DevURI = GlobalConfigContext.DevURI_SO_VOCAL;
                    PackURI = GlobalConfigContext.PackURI_SO_VOCAL;
                    break;
                default:
                    throw new Exception("调用了音乐获取方法，但却不是获取音乐资源");
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                var sourceSlot = this.resourceTable[DevURI][sourceName];
                var ptr = PackageUtils.GetObjectBytes(sourceSlot.BindingFile, sourceName, sourceSlot.Position, sourceSlot.Length);
                return new MemoryStream(ptr);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalConfigContext.DevURI_RT_SOUND, DevURI, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    var ptr = File.ReadAllBytes(IOUtils.ParseURItoURL(furi));
                    return new MemoryStream(ptr);
                }
                else
                {
                    MessageBox.Show("[错误] 资源文件不存在：" + sourceName);
                    return null;
                }
            }
        }


        /// <summary>
        /// 从资源文件中获取声音资源并返回句柄
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <param name="rtype">资源类型</param>
        /// <returns>一个键值对：该音频的内存托管句柄 - 内存长度</returns>
        private KeyValuePair<GCHandle?, long> GetMusicGCHandleLengthKVP(string sourceName, ResourceType rtype)
        {
            if (sourceName == String.Empty) { return new KeyValuePair<GCHandle?, long>(null, 0); }
            string DevURI = null, PackURI = null;
            // 处理路径
            switch (rtype)
            {
                case ResourceType.BGM:
                    DevURI = GlobalConfigContext.DevURI_SO_BGM;
                    PackURI = GlobalConfigContext.PackURI_SO_BGM;
                    break;
                case ResourceType.BGS:
                    DevURI = GlobalConfigContext.DevURI_SO_BGS;
                    PackURI = GlobalConfigContext.PackURI_SO_BGS;
                    break;
                case ResourceType.SE:
                    DevURI = GlobalConfigContext.DevURI_SO_SE;
                    PackURI = GlobalConfigContext.PackURI_SO_SE;
                    break;
                case ResourceType.VOCAL:
                    DevURI = GlobalConfigContext.DevURI_SO_VOCAL;
                    PackURI = GlobalConfigContext.PackURI_SO_VOCAL;
                    break;
                default:
                    throw new Exception("调用了音乐获取方法，但却不是获取音乐资源");
            }
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(DevURI) &&
                this.resourceTable[DevURI].ContainsKey(sourceName))
            {
                var slot = this.resourceTable[DevURI][sourceName];
                var sourceLocation = new KeyValuePair<long, long>(slot.Position, slot.Length);
                GCHandle ptr = PackageUtils.GetObjectManagedHandle(IOUtils.ParseURItoURL(PackURI + GlobalConfigContext.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                return new KeyValuePair<GCHandle?, long>(ptr, sourceLocation.Value);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalConfigContext.DevURI_RT_SOUND, DevURI, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    byte[] bytes = File.ReadAllBytes(IOUtils.ParseURItoURL(furi));
                    return new KeyValuePair<GCHandle?, long>(GCHandle.Alloc(bytes, GCHandleType.Pinned), bytes.Length);
                }
                else
                {
                    MessageBox.Show("[错误] 资源文件不存在：" + sourceName);
                    Director.GetInstance().GetMainRender().Shutdown();
                    throw new FileNotFoundException();
                }
            }
        }

        /// <summary>
        /// 在根目录下搜索资源信息文件
        /// </summary>
        /// <returns>资源信息文件的路径队列</returns>
        private Queue<string> SearchPST()
        {
            var resContainer = new Queue<string>();
            DirectoryInfo rootDirInfo = new DirectoryInfo(Director.BasePath);
            foreach (FileInfo file in rootDirInfo.GetFiles())
            {
                if (file.Extension == ".pst")
                {
                    resContainer.Enqueue(file.FullName);
                }
            }
            LogUtils.LogLine("Total PST: " + resContainer.Count, "ResourceManager", LogLevel.Important);
            return resContainer;
        }

        /// <summary>
        /// 为资源表增加一个资源
        /// </summary>
        /// <param name="typeKey">资源类型</param>
        /// <param name="slot">资源信息块</param>
        /// <returns>操作成功与否</returns>
        private bool AddResource(string typeKey, ResourceSlot slot)
        {
            lock (this.resourceTable)
            {
                if (this.resourceTable.ContainsKey(typeKey))
                {
                    if (!this.resourceTable[typeKey].ContainsKey(slot.ResourceName))
                    {
                        this.resourceTable[typeKey][slot.ResourceName] = slot;
                        return true;
                    }
                    if (
                        String.Compare(this.resourceTable[typeKey][slot.ResourceName].Version, slot.Version,
                            StringComparison.Ordinal) < 0)
                    {
                        this.resourceTable[typeKey][slot.ResourceName] = slot;
                        LogUtils.LogLine(
                            String.Format(
                                "Resource already exist with later version, to be replaced, type: {0}, name: {1}",
                                typeKey, slot.ResourceName), "ResourceManager", LogLevel.Warning);
                        return true;
                    }
                    LogUtils.LogLine(
                        String.Format(
                            "Resource already exist without later version, to be ignored, type: {0}, name: {1}",
                            typeKey, slot.ResourceName), "ResourceManager", LogLevel.Warning);
                    return false;
                }
                LogUtils.LogLine(String.Format("Resource type not exist, type: {0}", typeKey),
                    "ResourceManager", LogLevel.Warning);
                return false;
            }
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
                    this.resourceTable.Add(resTableName, new Dictionary<string, ResourceSlot>());
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
            var threadPool = new List<Thread>();
            this.threadFinishCounter = 0;
            for (int t = 0; t < threadNum; t++)
            {
                threadPool.Add(new Thread(new ParameterizedThreadStart(this.InitDictionaryByPST)));
                threadPool[t].IsBackground = true;
                threadPool[t].Start(t);
            }
            // 等待线程回调
            while (this.threadFinishCounter < threadNum) { }
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
                string pstPath;
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
                LogUtils.AsyncLogLine(String.Format("Loading PST Resource From \"{0}\" At thread {1}", pstPath, tid),
                    "ResourceManager", this.consoleMutex, LogLevel.Normal);
                // 开始处理文件
                FileStream fs = new FileStream(pstPath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                // 读取头部信息
                string header = sr.ReadLine();
                if (header == null)
                {
                    LogUtils.AsyncLogLine(String.Format("Jump null header PST Resource From \"{0}\" At thread {1}", pstPath, tid),
                    "ResourceManager", this.consoleMutex, LogLevel.Normal);
                    continue;
                }
                var headerItem = header.Split('@');
                if (headerItem.Length != GlobalConfigContext.PackHeaderItemNum && headerItem[0] != GlobalConfigContext.PackHeader)
                {
                    LogUtils.AsyncLogLine(String.Format("Ignored Pack (Bad Header): {0}", pstPath), "ResourceManager", this.consoleMutex, LogLevel.Warning);
                    continue;
                }
                try
                {
                    var fileCount = Convert.ToInt32(headerItem[1]);
                    var resourceType = headerItem[2];
                    var keyItem = headerItem[3].Split('?');
                    var version = keyItem.Length != 2 ? "0" : keyItem[1];
                    var key = keyItem[0];
                    if (key != GlobalConfigContext.GAME_KEY)
                    {
                        LogUtils.AsyncLogLine(String.Format("Ignored Pack (Key Failed): {0}", pstPath), "ResourceManager", this.consoleMutex, LogLevel.Warning);
                        continue;
                    }
                    // 通过检验的包才载入资源字典
                    this.AddResouceTable(resourceType);
                    int lineEncounter = 0;
                    while (lineEncounter < fileCount)
                    {
                        lineEncounter++;
                        var lineitem = sr.ReadLine()?.Split(':');
                        if (lineitem == null)
                        {
                            LogUtils.AsyncLogLine(String.Format("Ignored Pack of null body: {0}", pstPath), "ResourceManager", this.consoleMutex, LogLevel.Warning);
                            continue;
                        }
                        if (lineitem[0] == GlobalConfigContext.PackEOF)
                        {
                            LogUtils.AsyncLogLine(String.Format("Stop PST caching because encountered EOF: {0}", pstPath), "ResourceManager", this.consoleMutex, LogLevel.Warning);
                            break;
                        }
                        if (lineitem.Length != 3)
                        {
                            LogUtils.AsyncLogLine(String.Format("Igonred line(Bad lineitem): {0}, In file: {1}", lineEncounter, pstPath), "ResourceManager", this.consoleMutex, LogLevel.Warning);
                            continue;
                        }
                        string srcName = lineitem[0];
                        long srcOffset = Convert.ToInt64(lineitem[1]);
                        long srcLength = Convert.ToInt64(lineitem[2]);
                        ResourceSlot resSlot = new ResourceSlot()
                        {
                            ResourceName = srcName,
                            BindingFile = pstPath.Substring(0, pstPath.Length - GlobalConfigContext.PackPostfix.Length),
                            Position = srcOffset,
                            Length = srcLength,
                            Version = version
                        };
                        this.AddResource(resourceType, resSlot);
                    }
                    LogUtils.AsyncLogLine(String.Format("Finish Dictionary Init From \"{0}\" At thread {1}", pstPath, tid), "ResourceManager", this.consoleMutex, LogLevel.Normal);
                }
                catch (Exception ex)
                {
                    LogUtils.AsyncLogLine(ex.ToString(), "ResourceManager / CLR", this.consoleMutex, LogLevel.Error);
                }
                sr.Close();
                fs.Close();
            }
            // 递增回到等待
            ++this.threadFinishCounter;
            LogUtils.AsyncLogLine(String.Format("At ResMana thread {0}, Waiting for callback", tid), "ResourceManager", this.consoleMutex, LogLevel.Important);
        }

        /// <summary>
        /// 把场景文件恢复为实例
        /// </summary>
        private void InitScenario()
        {
            List<Scene> sceneList = ILConvertor.GetInstance().Dash(IOUtils.ParseURItoURL(GlobalConfigContext.DevURI_RT_SCENARIO));
            foreach (Scene sc in sceneList)
            {
                if (this.sceneTable.ContainsKey(sc.Scenario))
                {
                    LogUtils.LogLine(String.Format("Scene already exist: {0}, new one will replace the elder one", sc.Scenario),
                        "ResourceManager", LogLevel.Warning);
                }
                this.sceneTable[sc.Scenario] = sc;
            }
            LogUtils.LogLine(String.Format("Finish Load Scenario, Total: {0}", sceneList.Count), "ResourceManager", LogLevel.Normal);
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>资源管理器的唯一实例</returns>
        public static ResourceManager GetInstance()
        {
            return ResourceManager.synObject ?? (ResourceManager.synObject = new ResourceManager());
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ResourceManager()
        {
            this.resourceTable = new Dictionary<string, Dictionary<string, ResourceSlot>>();
            this.sceneTable = new Dictionary<string, Scene>();
            this.InitDictionary();
            this.InitScenario();
        }

        /// <summary>
        /// 全图切割矩
        /// </summary>
        public static readonly Int32Rect FullImageRect = new Int32Rect(-1, 0, 0, 0);

        /// <summary>
        /// 唯一实例量
        /// </summary>
        private static ResourceManager synObject = null;

        /// <summary>
        /// 封包资源字典
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, ResourceSlot>> resourceTable = null;

        /// <summary>
        /// 场景字典
        /// </summary>
        private readonly Dictionary<string, Scene> sceneTable = null;

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
        private readonly Mutex consoleMutex = new Mutex();

        /// <summary>
        /// 封包资源信息块
        /// </summary>
        internal sealed class ResourceSlot
        {
            /// <summary>
            /// 获取或设置资源名
            /// </summary>
            public string ResourceName { get; set; }

            /// <summary>
            /// 获取或设置资源所在文件名
            /// </summary>
            public string BindingFile { get; set; }

            /// <summary>
            /// 获取或设置资源在文件中的起始位置
            /// </summary>
            public long Position { get; set; }

            /// <summary>
            /// 获取或设置资源的长度
            /// </summary>
            public long Length { get; set; }

            /// <summary>
            /// 获取或设置资源的版本
            /// </summary>
            public string Version { get; set; }
        }
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
        Frontier,
        BGM,
        BGS,
        SE,
        VOCAL,
        MessageLayerBackground,
        Button,
        BranchButton,
        SaveSnapshot
    }
}
