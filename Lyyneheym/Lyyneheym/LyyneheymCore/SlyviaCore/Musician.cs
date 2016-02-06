using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Media;
using System.Runtime.InteropServices;

using Un4seen.Bass;
using Un4seen.Bass.AddOn;
using Un4seen.Bass.AddOn.Tags;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class Musician
    {
        /// <summary>
        /// <para>播放背景音乐：从文件读入资源</para>
        /// <para>背景音乐在同一时刻只能播放一个资源</para>
        /// </summary>
        /// <param name="resourceName">资源名</param>
        /// <param name="filename">文件路径</param>
        public void PlayBGM(string resourceName, string filename)
        {
            // 如果有BGM在播放就截断
            if (this.isBGMPlaying || this.isBGMPaused)
            {
                this.StopAndReleaseBGM();
            }
            int handle = this.BassEngine.LoadFromFile(filename);
            BgmHandleContainer = new KeyValuePair<string, int>(resourceName, handle);
            this.BassEngine.Play(handle);
            this.isBGMPlaying = this.isBGMLoaded = true;
            this.isBGMPaused = false;
        }

        /// <summary>
        /// <para>播放背景音乐：从内存读入资源</para>
        /// <para>背景音乐在同一时刻只能播放一个资源</para>
        /// </summary>
        /// <param name="resourceName">资源名</param>
        /// <param name="memory">资源内存数组</param>
        public void PlayBGM(string resourceName, GCHandle? memory, long len)
        {
            // 如果有BGM在播放就截断
            if (this.isBGMPlaying || this.isBGMPaused)
            {
                this.StopAndReleaseBGM();
            }
            int handle = this.BassEngine.LoadFromMemory((GCHandle)memory, len);
            this.BassEngine.SetVolume(handle, 1000);
            BgmHandleContainer = new KeyValuePair<string, int>(resourceName, handle);
            this.BassEngine.Play(handle);
            this.isBGMPlaying = this.isBGMLoaded = true;
            this.isBGMPaused = false;
            
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            if (this.isBGMLoaded)
            {
                this.BassEngine.Pause(this.BgmHandleContainer.Value);
                this.isBGMPlaying = false;
                this.isBGMPaused = true;
            }
        }

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            if (this.isBGMLoaded)
            {
                this.BassEngine.Play(this.BgmHandleContainer.Value);
                this.isBGMPlaying = true;
                this.isBGMPaused = false;
            }
        }

        /// <summary>
        /// 停止BGM并释放资源
        /// </summary>
        public void StopAndReleaseBGM()
        {
            if (this.isBGMLoaded)
            {
                this.BassEngine.Stop(this.BgmHandleContainer.Value);
                this.BassEngine.DisposeHandle(this.BgmHandleContainer.Value);
                this.isBGMLoaded = this.isBGMPlaying = false;
            }
        }

        /// <summary>
        /// BGM句柄容器
        /// </summary>
        private KeyValuePair<string, int> BgmHandleContainer;

        /// <summary>
        /// 获取BGM是否正在播放
        /// </summary>
        public bool isBGMPlaying = false;
        
        /// <summary>
        /// 获取BGM是否已经加载
        /// </summary>
        public bool isBGMLoaded = false;

        /// <summary>
        /// 获取BGM是否已经暂停
        /// </summary>
        public bool isBGMPaused = false;
        
        /// <summary>
        /// 音频引擎实例
        /// </summary>
        private BassPlayer BassEngine;

        /// <summary>
        /// <para>播放背景音效：从文件读入资源</para>
        /// <para>背景声效可以多个声音资源同时播放，并且可以与BGM同存</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(string filename, int track = 0)
        {
            if (track >= 0 && track < GlobalDataContainer.GAME_MUSIC_BGSTRACKNUM)
            {
                this.BgsHandleContainer[track] = this.BassEngine.LoadFromFile(filename);
                this.BassEngine.Play(this.BgsHandleContainer[track]);
            }
        }

        /// <summary>
        /// <para>播放背景音效：从内存读入资源</para>
        /// <para>背景声效可以多个声音资源同时播放，并且可以与BGM同存</para>
        /// </summary>
        /// <param name="memory">资源内存数组</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(byte[] memory, int track = 0)
        {
            if (track >= 0 && track < GlobalDataContainer.GAME_MUSIC_BGSTRACKNUM)
            {
                //this.BgsHandleContainer[track] = this.BassEngine.LoadFromMemory(memory);
                this.BassEngine.Play(this.BgsHandleContainer[track]);
            }
        }

        /// <summary>
        /// 停止BGS并释放资源
        /// </summary>
        /// <param name="track">要停止的BGS轨道，缺省值-1表示全部停止</param>
        public void StopBGS(int track = -1)
        {
            if (track >= 0 && track < GlobalDataContainer.GAME_MUSIC_BGSTRACKNUM && this.BgsHandleContainer[track] != 0)
            {
                this.BassEngine.Stop(this.BgsHandleContainer[track]);
            }
            else
            {
                for (int i = 0; i < this.BgsHandleContainer.Count; i++)
                {
                    if (this.BgsHandleContainer[i] != 0)
                    {
                        int handle = this.BgsHandleContainer[i];
                        this.BassEngine.Stop(handle);
                        this.BassEngine.DisposeHandle(handle);
                        this.BgsHandleContainer[i] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 背景声效容器
        /// </summary>
        private List<int> BgsHandleContainer;

        /// <summary>
        /// <para>播放声效：从文件读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        public void PlaySE(string filename)
        {
            int handle = this.BassEngine.LoadFromFileWithAutoRelease(filename);
            this.BassEngine.Play(handle);
        }

        /// <summary>
        /// <para>播放声效：从内存读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="memory">资源内存数组</param>
        public void PlaySE(byte[] memory)
        {
            //int handle = this.BassEngine.LoadFromMemoryWithAutoRelease(memory);
            //this.BassEngine.Play(handle);
        }

        /// <summary>
        /// <para>播放语音：从文件读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        public void PlayVocal(string filename)
        {
            this.StopAndReleaseVocal();
            this.vocalHandle = this.BassEngine.LoadFromFileWithAutoRelease(filename);
            this.BassEngine.Play(this.vocalHandle);
        }

        /// <summary>
        /// <para>播放语音：从内存读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="memory">资源内存数组</param>
        public void PlayVocal(GCHandle? memory, long len)
        {
            this.StopAndReleaseVocal();
            this.vocalHandle = this.BassEngine.LoadFromMemoryWithAutoRelease((GCHandle)memory, len);
            this.BassEngine.Play(this.vocalHandle);
        }

        /// <summary>
        /// 停止语音并释放资源
        /// </summary>
        public void StopAndReleaseVocal()
        {
            if (this.vocalHandle != 0)
            {
                this.BassEngine.Stop(this.vocalHandle);
                this.BassEngine.DisposeHandle(this.vocalHandle);
                this.vocalHandle = 0;
            }
        }

        /// <summary>
        /// 当前语音句柄
        /// </summary>
        private int vocalHandle = 0;

        /// <summary>
        /// 更新函数：消息循环定期调用的更新方法
        /// </summary>
        public void Update()
        {
            if (this.BgmHandleContainer.Value != 0)
            {
                if (this.BassEngine.IsPlaying(this.BgmHandleContainer.Value) == false)
                {
                    this.BassEngine.Play(this.BgmHandleContainer.Value);
                }
            }
        }

        /// <summary>
        /// 工厂方法：获得音乐管理器类的唯一实例
        /// </summary>
        /// <returns>音乐管理器</returns>
        public static Musician getInstance()
        {
            return synObject == null ? synObject = new Musician() : synObject;
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Musician synObject = null;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Musician()
        {
            this.BassEngine = BassPlayer.GetInstance();
            this.BgmHandleContainer = new KeyValuePair<string, int>(null, 0);
            this.BgsHandleContainer = new List<int>();
            for (int i = 0; i < GlobalDataContainer.GAME_MUSIC_BGSTRACKNUM; i++)
            {
                this.BgsHandleContainer.Add(0);
            }
        }
    }

    /// <summary>
    /// Bass播放器
    /// </summary>
    public class BassPlayer : IDisposable
    {
        #region 类自身方法
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static BassPlayer instance;

        /// <summary>
        /// 工厂方法：获得Bass音频引擎的唯一实例
        /// </summary>
        /// <returns>Bass音频引擎</returns>
        public static BassPlayer GetInstance()
        {
            return BassPlayer.instance == null ?
                BassPlayer.instance = new BassPlayer(null) : BassPlayer.instance;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        static BassPlayer()
        {
            BassNet.Registration("jiajidi@163.com", "2X1929291512622");
        }

        /// <summary>
        /// 构造器：初始化播放器设备
        /// </summary>
        /// <param name="deviceInfo">设备信息</param>
        private BassPlayer(AudioDeviceInfo? deviceInfo = null)
        {
            this.Initialize(deviceInfo);
            this.endSyncProc = new SYNCPROC(this.EndCallback);
            this.autoReleaseProc = new SYNCPROC(this.AutoReleaseCallback);
            this.playingStatusDict = new Dictionary<int, bool>();
            this.referenceDict = new Dictionary<int, GCHandle>();
        }

        /// <summary>
        /// 音频设备
        /// </summary>
        private AudioDeviceInfo? Device
        {
            get;
            set;
        }

        /// <summary>
        /// 析构器
        /// </summary>
        ~BassPlayer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放播放设备
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">哑元</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                Bass.BASS_Free();
                Bass.FreeMe();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private bool isDisposed;
        #endregion

        #region 初始化引擎
        /// <summary>
        /// 查找设备的序号
        /// </summary>
        /// <param name="device">要查找的设备</param>
        /// <param name="returnDefault">当找不到设备时，是否返回默认设备的序号</param>
        /// <returns></returns>
        private static int FindDevice(AudioDeviceInfo? device, bool returnDefault = false)
        {
            int result;
            if (device.HasValue)
            {
                int num = -1;
                BASS_DEVICEINFO[] devices = Bass.BASS_GetDeviceInfos();
                IEnumerable<int> source =
                    from d in devices
                    where d.id != null && d.id == device.Value.ID
                    select Array.IndexOf<BASS_DEVICEINFO>(devices, d);
                if (source.Count<int>() == 1)
                {
                    num = source.First<int>();
                }
                if (num == -1)
                {
                    source =
                        from d in devices
                        where d.name == device.Value.Name
                        select Array.IndexOf<BASS_DEVICEINFO>(devices, d);
                    if (source.Count<int>() == 1)
                    {
                        num = source.First<int>();
                    }
                }
                if (num == -1)
                {
                    source =
                        from d in devices
                        where d.driver == device.Value.Driver
                        select Array.IndexOf<BASS_DEVICEINFO>(devices, d);
                    if (source.Count<int>() == 1)
                    {
                        num = source.First<int>();
                    }
                }
                if (num == -1 && returnDefault)
                {
                    result = BassPlayer.FindDefaultDevice();
                }
                else
                {
                    if (num == -1)
                    {
                        throw new Exception("找不到此设备：" + device.Value.Name);
                    }
                    result = num;
                }
            }
            else
            {
                result = BassPlayer.FindDefaultDevice();
            }
            return result;
        }

        /// <summary>
        /// 返回默认设备的序号
        /// </summary>
        /// <returns></returns>
        private static int FindDefaultDevice()
        {
            BASS_DEVICEINFO[] array = Bass.BASS_GetDeviceInfos();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].IsDefault)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 初始化BassEngine
        /// </summary>
        private void Initialize(AudioDeviceInfo? device = null)
        {
            try
            {
                IntPtr win = IntPtr.Zero;
                int i = BassPlayer.FindDevice(device, true);
                if (!Bass.BASS_Init(i, this.sampleFrequency, BASSInit.BASS_DEVICE_DEFAULT, win))
                {
                    BASSError code = Bass.BASS_ErrorGetCode();
                    int num = Bass.BASS_GetDeviceCount();
                    for (i = -1; i < num; i++)
                    {
                        if (i != 0 && Bass.BASS_Init(i, this.sampleFrequency, BASSInit.BASS_DEVICE_DEFAULT, win))
                        {
                            break;
                        }
                    }
                    if (i == num)
                    {
                        throw new Exception();
                    }
                }
                if (!device.HasValue && i == BassPlayer.FindDefaultDevice())
                {
                    this.Device = null;
                }
                else
                {
                    BASS_DEVICEINFO bASS_DEVICEINFO = Bass.BASS_GetDeviceInfo(Bass.BASS_GetDevice());
                    this.Device = new AudioDeviceInfo?(new AudioDeviceInfo
                    {
                        Driver = bASS_DEVICEINFO.driver,
                        Name = bASS_DEVICEINFO.name,
                        ID = bASS_DEVICEINFO.id
                    });
                }

                Bass.LoadMe();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }

        /// <summary>
        /// 获取音频设备列表
        /// </summary>
        /// <returns>音频设备数组</returns>
        private static AudioDeviceInfo[] GetDeviceInfos()
        {
            List<AudioDeviceInfo> list = new List<AudioDeviceInfo>();
            List<BASS_DEVICEINFO> list2 = Bass.BASS_GetDeviceInfos().ToList<BASS_DEVICEINFO>();
            foreach (BASS_DEVICEINFO current in list2)
            {
                if (current.IsEnabled && !string.Equals(current.name, "No sound", StringComparison.CurrentCultureIgnoreCase) && !string.Equals(current.name, "Default", StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(new AudioDeviceInfo
                    {
                        ID = current.id,
                        Name = current.name,
                        Driver = current.driver
                    });
                }
            }
            return list.ToArray();
        }
        #endregion

        /// <summary>
        /// 将文件加载为可播放的句柄
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>句柄</returns>
        public int LoadFromFile(string filename)
        {
            int fileHandle = Bass.BASS_StreamCreateFile(filename, 0L, 0L, BASSFlag.BASS_DEFAULT);
            int callbackHandle = Bass.BASS_ChannelSetSync(fileHandle, BASSSync.BASS_SYNC_END, 0L, this.endSyncProc, IntPtr.Zero);
            if (fileHandle == 0)
            {
                throw new Exception("cannot open audio file");
            }
            return fileHandle;
        }

        /// <summary>
        /// 将一块内存区域加载为可播放的句柄
        /// </summary>
        /// <param name="memoryBuffer">内存数组</param>
        /// <returns>句柄</returns>
        public int LoadFromMemory(GCHandle memoryBuffer, long len)
        {
            //fixed (byte* pointer = memoryBuffer)
            //{
            //    IntPtr pArray = new IntPtr(pointer);
            int bufferHandle = Bass.BASS_StreamCreateFile(memoryBuffer.AddrOfPinnedObject(), 0L, len, BASSFlag.BASS_DEFAULT);
                int callbackHandle = Bass.BASS_ChannelSetSync(bufferHandle, BASSSync.BASS_SYNC_END, 0L, this.endSyncProc, IntPtr.Zero);
                if (bufferHandle == 0)
                {
                    throw new Exception("cannot open audio file");
                }
                if (this.referenceDict.ContainsKey(bufferHandle))
                {
                    this.referenceDict.Remove(bufferHandle);
                }
                this.referenceDict.Add(bufferHandle, memoryBuffer);
                return bufferHandle;
            //}
        }

        private Dictionary<int, GCHandle> referenceDict;
        
        /// <summary>
        /// 将文件加载为可播放的句柄，并在播放结束后销毁自己
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>句柄</returns>
        public int LoadFromFileWithAutoRelease(string filename)
        {
            int fileHandle = Bass.BASS_StreamCreateFile(filename, 0L, 0L, BASSFlag.BASS_DEFAULT);
            int callbackHandle = Bass.BASS_ChannelSetSync(fileHandle, BASSSync.BASS_SYNC_END, 0L, this.autoReleaseProc, IntPtr.Zero);
            if (fileHandle == 0)
            {
                throw new Exception("cannot open audio file");
            }
            return fileHandle;
        }

        /// <summary>
        /// 将一块内存区域加载为可播放的句柄，并在播放结束后销毁自己
        /// </summary>
        /// <param name="memoryBuffer">内存数组</param>
        /// <returns>句柄</returns>
        public unsafe int LoadFromMemoryWithAutoRelease(GCHandle memoryBuffer, long len)
        {
            //IntPtr pArray;
            //fixed (byte* pointer = memoryBuffer)
            //{
            //    pArray = new IntPtr(pointer);
            int bufferHandle = Bass.BASS_StreamCreateFile(memoryBuffer.AddrOfPinnedObject(), 0L, len, BASSFlag.BASS_DEFAULT);
                int callbackHandle = Bass.BASS_ChannelSetSync(bufferHandle, BASSSync.BASS_SYNC_END, 0L, this.autoReleaseProc, IntPtr.Zero);
                if (bufferHandle == 0)
                {
                    throw new Exception("cannot open audio file");
                }
                if (this.referenceDict.ContainsKey(bufferHandle))
                {
                    this.referenceDict.Remove(bufferHandle);
                }
                this.referenceDict.Add(bufferHandle, memoryBuffer);
                return bufferHandle;
            //}
        }

        /// <summary>
        /// 播放一个句柄
        /// </summary>
        /// <param name="handle">句柄</param>
        public void Play(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_ChannelPlay(handle, false);
                if (this.playingStatusDict.ContainsKey(handle))
                {
                    this.playingStatusDict.Remove(handle);
                }
                this.playingStatusDict.Add(handle, true);
            }
        }

        /// <summary>
        /// 停止一个句柄
        /// </summary>
        /// <param name="handle">句柄</param>
        public void Stop(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_ChannelStop(handle);
                Bass.BASS_ChannelSetPosition(handle, 0);
                if (this.playingStatusDict.ContainsKey(handle))
                {
                    this.playingStatusDict[handle] = false;
                }
            }
        }

        /// <summary>
        /// 释放一个句柄
        /// </summary>
        /// <param name="handle">句柄</param>
        public void DisposeHandle(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_StreamFree(handle);
                if (this.playingStatusDict.ContainsKey(handle))
                {
                    this.playingStatusDict.Remove(handle);
                }
                if (this.referenceDict.ContainsKey(handle))
                {
                    GCHandle gch = this.referenceDict[handle];
                    if (gch.IsAllocated)
                    {
                        gch.Free();
                    }
                    this.referenceDict.Remove(handle);
                }
            }
        }

        /// <summary>
        /// 暂停一个句柄
        /// </summary>
        /// <param name="handle">句柄</param>
        public void Pause(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_ChannelPause(handle);
                this.playingStatusDict[handle] = false;
            }
        }

        /// <summary>
        /// 获得一个句柄是否正在播放
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <returns>是否正在播放中</returns>
        public bool IsPlaying(int handle)
        {
            if (this.playingStatusDict.ContainsKey(handle))
            {
                return this.playingStatusDict[handle];
            }
            return false;
        }

        /// <summary>
        /// 为一个句柄设置音量
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <param name="volume">音量值（1-1000）</param>
        public void SetVolume(int handle, float volume = 1000)
        {
            if (handle != 0)
            {
                float value = this.IsMuted ? 0f : (float)(volume / 1000.0 * this.volumeScale);
                Bass.BASS_ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        /// <summary>
        /// 回调函数：句柄播放完的回调
        /// </summary>
        private void EndCallback(int handle, int channel, int data, IntPtr user)
        {
            this.Stop(channel);
        }

        /// <summary>
        /// 回调函数：自动释放句柄播放完的回调
        /// </summary>
        private void AutoReleaseCallback(int handle, int channel, int data, IntPtr user)
        {
            this.Stop(channel);
            this.DisposeHandle(channel);
        }

        /// <summary>
        /// 音量比例（0-1）
        /// </summary>
        private float volumeScale = 1.0f;

        /// <summary>
        /// 是否静音
        /// </summary>
        private bool isMuted;

        /// <summary>
        /// 当播放BGM结束时调用
        /// </summary>
        private readonly SYNCPROC endSyncProc;

        /// <summary>
        /// 当播放自动释放句柄完成时调用
        /// </summary>
        private readonly SYNCPROC autoReleaseProc;

        /// <summary>
        /// 播放码率
        /// </summary>
        private int sampleFrequency = 44100;

        /// <summary>
        /// 设置或获取全局音量比例
        /// </summary>
        public float VolumeScale
        {
            get
            {
                return this.volumeScale;
            }
            set
            {
                this.volumeScale = value;
            }
        }

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool IsMuted
        {
            get
            {
                return this.isMuted;
            }
            set
            {
                if (this.isMuted != value)
                {
                    this.isMuted = value;
                }
            }
        }

        /// <summary>
        /// 句柄播放状态字典
        /// </summary>
        private Dictionary<int, bool> playingStatusDict;

        /// <summary>
        /// 音频设备信息类
        /// </summary>
        [Serializable]
        private struct AudioDeviceInfo
        {
            public string Driver { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
        }
    }


}
