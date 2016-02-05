using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Media;

using Un4seen.Bass;
using Un4seen.Bass.AddOn;
using Un4seen.Bass.AddOn.Tags;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class Musician
    {
        public void PlayBGM(string filename)
        {
            BassPlayer bassPlayer = BassPlayer.GetInstance();
            bassPlayer.OpenFile(filename);
            bassPlayer.Volume = 1000;
            bassPlayer.Play();
            
        }

        public void PlayBGM(byte[] memory)
        {
            BassPlayer bassPlayer = BassPlayer.GetInstance();
            bassPlayer.OpenMemory(memory);
            bassPlayer.Volume = 1000;
            bassPlayer.Play();
        }

        public void PlayBGS(string filename, int track = 0)
        {

        }

        public void PlaySE(string filename)
        {

        }

        public void PlayME(string filename)
        {

        }

        public void PlayVocal(string filename)
        {
            
        }

        public void PlayVocal(byte[] memory)
        {
            BassPlayer bassPlayer = BassPlayer.GetInstance();
            int handle = bassPlayer.OpenMemoryHandle(memory);
            bassPlayer.Play(handle);
        }



        private Queue<string> BGMQueue = null;

        private MediaPlayer PlayerBGM = null;

        private MediaPlayer PlayerME = null;

        private MediaPlayer PlayerVocal = null;

        private List<MediaPlayer> PlayerBGS = null;

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
            this.endTrackSyncProc = new SYNCPROC(this.EndTrack);
        }

        /// <summary>
        /// 音频设备
        /// </summary>
        public AudioDeviceInfo? Device
        {
            get;
            private set;
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
                this.IsPlaying = false;
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

        #region 播放动作
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filename">文件名</param>
        public void OpenFile(string filename)
        {
            this.openningFile = filename;
            this.Stop();
            this.pendingOperation = BassPlayer.BassAudioCurrentState.None;
            int num = Bass.BASS_StreamCreateFile(filename, 0L, 0L, BASSFlag.BASS_DEFAULT);
            this.tagInfo = BassTags.BASS_TAG_GetFromFile(filename);
            if (num != 0)
            {
                this.ActiveStreamHandle = num;
                this.ChannelLength = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(this.ActiveStreamHandle, Bass.BASS_ChannelGetLength(this.ActiveStreamHandle, BASSMode.BASS_POS_BYTES)));
                BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
                Bass.BASS_ChannelGetInfo(this.ActiveStreamHandle, bASS_CHANNELINFO);
                this.sampleFrequency = bASS_CHANNELINFO.freq;
                int num2 = Bass.BASS_ChannelSetSync(this.ActiveStreamHandle, BASSSync.BASS_SYNC_END, 0L, this.endTrackSyncProc, IntPtr.Zero);
                if (num2 == 0)
                {
                    throw new ArgumentException("Error establishing End Sync on file stream.", "path");
                }
                this.CanPlay = true;
            }
            else
            {
                throw new Exception("cannot open audio file");
            }
        }

        /// <summary>
        /// 打开内存流
        /// </summary>
        public unsafe void OpenMemory(byte[] memoryBuffer)
        {
            IntPtr pArray;
            fixed (byte* pointer = memoryBuffer)
            {
                pArray = new IntPtr(pointer);

                this.openningFile = "MEMORYBUFFER";
                this.Stop();
                this.pendingOperation = BassPlayer.BassAudioCurrentState.None;
                int num = Bass.BASS_StreamCreateFile(pArray, 0L, memoryBuffer.Length, BASSFlag.BASS_DEFAULT);
                this.tagInfo = null;
                if (num != 0)
                {
                    this.ActiveStreamHandle = num;
                    this.ChannelLength = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(this.ActiveStreamHandle, Bass.BASS_ChannelGetLength(this.ActiveStreamHandle, BASSMode.BASS_POS_BYTES)));
                    BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
                    Bass.BASS_ChannelGetInfo(this.ActiveStreamHandle, bASS_CHANNELINFO);
                    this.sampleFrequency = bASS_CHANNELINFO.freq;
                    int num2 = Bass.BASS_ChannelSetSync(this.ActiveStreamHandle, BASSSync.BASS_SYNC_END, 0L, this.endTrackSyncProc, IntPtr.Zero);
                    if (num2 == 0)
                    {
                        throw new ArgumentException("Error establishing End Sync on file stream.", "path");
                    }
                    this.CanPlay = true;
                }
                else
                {
                    throw new Exception("cannot open audio file");
                }
            }
        }

        /// <summary>
        /// 打开内存流
        /// </summary>
        public unsafe int OpenMemoryHandle(byte[] memoryBuffer)
        {
            IntPtr pArray;
            fixed (byte* pointer = memoryBuffer)
            {
                pArray = new IntPtr(pointer);
                int num = Bass.BASS_StreamCreateFile(pArray, 0L, memoryBuffer.Length, BASSFlag.BASS_DEFAULT);
                if (num == 0)
                {
                    throw new Exception("cannot open audio file");
                }
                return num;
            }
        }

        /// <summary>
        /// 播放当前音频
        /// </summary>
        public void Play()
        {
            if (this.CanPlay)
            {
                this.PlayCurrentStream();
                this.IsPlaying = true;
                this.CanPause = true;
                this.CanPlay = false;
                this.CanStop = true;
                this.pendingOperation = BassPlayer.BassAudioCurrentState.None;
            }
            else
            {
                this.pendingOperation = BassPlayer.BassAudioCurrentState.Play;
            }
        }

        public void Play(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_ChannelPlay(handle, true);
            }
        }

        public void Stop(int handle)
        {
            if (handle != 0)
            {
                Bass.BASS_ChannelStop(handle);
            }
        }

        /// <summary>
        /// 播放当前流
        /// </summary>
        private void PlayCurrentStream()
        {
            if (this.ActiveStreamHandle != 0 && Bass.BASS_ChannelPlay(this.ActiveStreamHandle, true))
            {
                BASS_CHANNELINFO info = new BASS_CHANNELINFO();
                Bass.BASS_ChannelGetInfo(this.ActiveStreamHandle, info);
            }
            else
            {
                Console.WriteLine("BASS_StreamPlay失败：" + Bass.BASS_ErrorGetCode());
            }
        }

        /// <summary>
        /// 播放指定的流
        /// </summary>
        private void PlayHandleStream(int handle)
        {
            if (this.ActiveStreamHandle != 0 && Bass.BASS_ChannelPlay(handle, true))
            {
                BASS_CHANNELINFO info = new BASS_CHANNELINFO();
                Bass.BASS_ChannelGetInfo(handle, info);
            }
            else
            {
                Console.WriteLine("BASS_StreamPlay失败：" + Bass.BASS_ErrorGetCode());
            }
        }


        /// <summary>
        /// 停止当前音频，并释放资源
        /// </summary>
        public void Stop()
        {
            if (this.canStop)
            {
                this.ChannelPosition = TimeSpan.Zero;
                if (this.ActiveStreamHandle != 0)
                {
                    Bass.BASS_ChannelStop(this.ActiveStreamHandle);
                    Bass.BASS_ChannelSetPosition(this.ActiveStreamHandle, this.ChannelPosition.TotalSeconds);
                }
                this.IsPlaying = false;
                this.CanStop = false;
                this.CanPlay = false;
                this.CanPause = false;
            }
            this.FreeCurrentStream();
            this.pendingOperation = BassPlayer.BassAudioCurrentState.None;
        }

        /// <summary>
        /// 暂停当前音频
        /// </summary>
        public void Pause()
        {
            if (this.IsPlaying && this.CanPause)
            {
                Bass.BASS_ChannelPause(this.ActiveStreamHandle);
                this.IsPlaying = false;
                this.CanPlay = true;
                this.CanPause = false;
                this.pendingOperation = BassPlayer.BassAudioCurrentState.None;
            }
            else
            {
                this.pendingOperation = BassPlayer.BassAudioCurrentState.Pause;
            }
        }

        /// <summary>
        /// 设置声道
        /// </summary>
        /// <param name="value">声道值</param>
        public void ChannelStereo(int value = 16)
        {
            Bass.BASS_ChannelGetLevel(value);
        }
        
        /// <summary>
        /// 释放当前流
        /// </summary>
        private void FreeCurrentStream()
        {
            if (this.ActiveStreamHandle != 0)
            {
                if (!Bass.BASS_StreamFree(this.ActiveStreamHandle))
                {
                    Console.WriteLine("BASS_StreamFree失败：" + Bass.BASS_ErrorGetCode());
                }
                this.ActiveStreamHandle = 0;
            }
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        private void SetVolume()
        {
            if (this.ActiveStreamHandle != 0)
            {
                float value = this.IsMuted ? 0f : ((float)this.Volume);
                Bass.BASS_ChannelSetAttribute(this.ActiveStreamHandle, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        /// <summary>
        /// 播放完毕回调
        /// </summary>
        private void EndTrack(int handle, int channel, int data, IntPtr user)
        {
            this.Stop();
        }
        #endregion

        #region 状态控制
        /// <summary>
        /// 当前流的句柄
        /// </summary>
        private int activeStreamHandle;
        /// <summary>
        /// 可以使用播放命令
        /// </summary>
        private bool canPlay;
        /// <summary>
        /// 可以使用暂停命令
        /// </summary>
        private bool canPause;
        /// <summary>
        /// 是否正在播放
        /// </summary>
        private bool isPlaying;
        /// <summary>
        /// 可以使用停止命令
        /// </summary>
        private bool canStop;
        /// <summary>
        /// 音频长度
        /// </summary>
        private TimeSpan channelLength = TimeSpan.Zero;
        /// <summary>
        /// 当前播放进度
        /// </summary>
        private TimeSpan currentChannelPosition = TimeSpan.Zero;
        /// <summary>
        /// 待执行的命令
        /// </summary>
        private BassPlayer.BassAudioCurrentState pendingOperation = BassPlayer.BassAudioCurrentState.None;
        /// <summary>
        /// 音量
        /// </summary>
        private double volume;
        /// <summary>
        /// 是否静音
        /// </summary>
        private bool isMuted;
        /// <summary>
        /// 文件信息
        /// </summary>
        private TAG_INFO tagInfo;
        /// <summary>
        /// 正在打开的文件的地址
        /// </summary>
        private string openningFile = null;
        /// <summary>
        /// 当播放结束时调用
        /// </summary>
        private readonly SYNCPROC endTrackSyncProc;
        /// <summary>
        /// 播放码率
        /// </summary>
        private int sampleFrequency = 44100;
        /// <summary>
        /// 获取或设置正在打开的文件
        /// </summary>
        public string OpenningFile
        {
            get { return openningFile; }
            set { openningFile = value; }
        }
        /// <summary>
        /// 获取或设置歌曲Tag标签
        /// </summary>
        public TAG_INFO TagInfo
        {
            get { return tagInfo; }
            set { tagInfo = value; }
        }
        /// <summary>
        /// 长度
        /// </summary>
        public TimeSpan ChannelLength
        {
            get
            {
                return this.channelLength;
            }
            protected set
            {
                TimeSpan t = this.channelLength;
                this.channelLength = value;
            }
        }
        /// <summary>
        /// 是否正在设置音轨
        /// </summary>
        private bool inChannelSet;
        /// <summary>
        /// 是否正在更新时计
        /// </summary>
        private bool inChannelTimerUpdate;
        /// <summary>
        /// 播放器指针位置
        /// </summary>
        public TimeSpan ChannelPosition
        {
            get
            {
                positionTimer_Tick();
                return this.currentChannelPosition;
            }
            set
            {
                if (!this.inChannelSet)
                {
                    this.inChannelSet = true;
                    TimeSpan t = this.currentChannelPosition;
                    TimeSpan t2 = value;
                    if (t2 > this.ChannelLength)
                    {
                        t2 = this.ChannelLength;
                    }
                    if (t2 < TimeSpan.Zero)
                    {
                        t2 = TimeSpan.Zero;
                    }
                    if (!this.inChannelTimerUpdate)
                    {
                        Bass.BASS_ChannelSetPosition(this.ActiveStreamHandle, Bass.BASS_ChannelSeconds2Bytes(this.ActiveStreamHandle, t2.TotalSeconds));
                    }
                    this.currentChannelPosition = t2;
                    this.inChannelSet = false;
                }
            }
        }
        /// <summary>
        /// 当前流的句柄
        /// </summary>
        public int ActiveStreamHandle
        {
            get
            {
                return this.activeStreamHandle;
            }
            protected set
            {
                int num = this.activeStreamHandle;
                this.activeStreamHandle = value;
                if (num != this.activeStreamHandle)
                {
                    if (this.activeStreamHandle != 0)
                    {
                        this.SetVolume();
                    }
                }
            }
        }
        /// <summary>
        /// 可以使用播放命令
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return this.canPlay;
            }
            protected set
            {
                bool flag = this.canPlay;
                this.canPlay = value;
            }
        }
        /// <summary>
        /// 可以使用暂停命令
        /// </summary>
        public bool CanPause
        {
            get
            {
                return this.canPause;
            }
            protected set
            {
                bool flag = this.canPause;
                this.canPause = value;
            }
        }
        /// <summary>
        /// 可以使用停止命令
        /// </summary>
        public bool CanStop
        {
            get
            {
                return this.canStop;
            }
            protected set
            {
                bool flag = this.canStop;
                this.canStop = value;
            }
        }
        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return this.isPlaying;
            }
            protected set
            {
                bool flag = this.isPlaying;
                this.isPlaying = value;
            }
        }
        /// <summary>
        /// 设置或获取音量值
        /// </summary>
        public double Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.volume = value / 1000;
                this.SetVolume();
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
                    this.SetVolume();
                }
            }
        }
        /// <summary>
        /// 更新播放进度
        /// </summary>
        private void positionTimer_Tick()
        {
            if (this.ActiveStreamHandle == 0)
            {
                this.ChannelPosition = TimeSpan.Zero;
            }
            else
            {
                this.inChannelTimerUpdate = true;
                this.ChannelPosition = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(this.ActiveStreamHandle, Bass.BASS_ChannelGetPosition(this.ActiveStreamHandle, BASSMode.BASS_POS_BYTES)));
                this.inChannelTimerUpdate = false;
            }
        }
        /// <summary>
        /// 当前的播放状态
        /// </summary>
        private enum BassAudioCurrentState
        {
            None,
            Play,
            Pause
        }
        /// <summary>
        /// 音频设备信息类
        /// </summary>
        [Serializable]
        public struct AudioDeviceInfo
        {
            public string Driver { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }


}
