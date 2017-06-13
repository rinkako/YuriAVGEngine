using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Media;
using System.Timers;
using System.Runtime.InteropServices;

//using Un4seen.Bass;
//using Un4seen.Bass.AddOn;
//using Un4seen.Bass.AddOn.Tags;

namespace Yuri.PlatformCore.Audio
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class MusicianBass
    {
        /// <summary>
        /// 根据音频状态描述子重演绎前端的音频
        /// </summary>
        /// <param name="mdescriptor">音频状态描述子</param>
        public void RePerform(MusicianDescriptor mdescriptor)
        {
           throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放背景音乐：从文件读入资源</para>
        /// <para>背景音乐在同一时刻只能播放一个资源</para>
        /// </summary>
        /// <param name="resourceName">资源名</param>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayBGM(string resourceName, string filename, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放背景音乐：从内存读入资源</para>
        /// <para>背景音乐在同一时刻只能播放一个资源</para>
        /// </summary>
        /// <param name="resourceName">资源名</param>
        /// <param name="gch">托管的内存句柄</param>
        /// <param name="len">句柄指向内存长度</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayBGM(string resourceName, GCHandle? gch, long len, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 停止BGM并释放资源
        /// </summary>
        public void StopAndReleaseBGM()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放背景音效：从文件读入资源</para>
        /// <para>背景声效可以多个声音资源同时播放，并且可以与BGM同存</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(string filename, float vol, int track = 0)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放背景音效：从内存读入资源</para>
        /// <para>背景声效可以多个声音资源同时播放，并且可以与BGM同存</para>
        /// </summary>
        /// <param name="gch">托管的内存句柄</param>
        /// <param name="len">句柄指向内存长度</param>
        /// <param name="vol">音量（1-1000）</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(GCHandle? gch, long len, float vol, int track = 0)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 停止BGS并释放资源
        /// </summary>
        /// <param name="track">要停止的BGS轨道，缺省值-1表示全部停止</param>
        public void StopBGS(int track = -1)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 变更BGM的音量
        /// </summary>
        /// <param name="vol"></param>
        public void SetBGMVolume(float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 变更指定轨道的BGS音量
        /// </summary>
        /// <param name="vol">音量</param>
        /// <param name="track">轨道（-1为全部变更）</param>
        public void SetBGSVolume(int vol, int track = 0)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 设置BGM声道
        /// </summary>
        /// <param name="offset">声道偏移（-1到1，从左向右偏移，0为立体声）</param>
        public void SetBGMStereo(float offset)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放声效：从文件读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlaySE(string filename, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放声效：从内存读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="gch">托管的内存句柄</param>
        /// <param name="len">句柄指向内存长度</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlaySE(GCHandle? gch, long len, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放语音：从文件读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayVocal(string filename, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <para>播放语音：从内存读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="gch">托管的内存句柄</param>
        /// <param name="len">句柄指向内存长度</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayVocal(GCHandle? gch, long len, float vol)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 停止语音并释放资源
        /// </summary>
        public void StopAndReleaseVocal()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 复位音乐管理器
        /// </summary>
        public void Reset()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 更新函数：消息循环定期调用的更新方法
        /// </summary>
        private void musicianTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 工厂方法：获得音乐管理器类的唯一实例
        /// </summary>
        /// <returns>音乐管理器</returns>
        public static MusicianBass GetInstance()
        {
            return synObject == null ? synObject = new MusicianBass() : synObject;
        }

        /// <summary>
        /// 获取或设置BGM音量
        /// </summary>
        public float BGMVolume
        {
            get
            {
                return this.bgmVolume;
            }
            set
            {
                this.bgmVolume = Math.Max(0, Math.Min(value, 1000));
                //this.BassEngine.SetVolume(this.BgmHandleContainer.Value, this.bgmVolume);
            }
        }

        /// <summary>
        /// 获取或设置BGS默认音量
        /// </summary>
        public float BGSDefaultVolume
        {
            get { return this.bgsVolume; }
            set { this.bgsVolume = Math.Max(0, Math.Min(value, 1000)); }
        }

        /// <summary>
        /// 获取或设置SE默认音量
        /// </summary>
        public float SEDefaultVolume
        {
            get { return this.seVolume; }
            set { this.seVolume = Math.Max(0, Math.Min(value, 1000)); }
        }

        /// <summary>
        /// 获取或设置Vocal默认音量
        /// </summary>
        public float VocalDefaultVolume
        {
            get { return this.vocalVolume; }
            set { this.vocalVolume = Math.Max(0, Math.Min(value, 1000)); }
        }

        /// <summary>
        /// 获取BGM是否正在播放
        /// </summary>
        public bool isBGMPlaying
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BGM是否已经加载
        /// </summary>
        public bool isBGMLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BGM是否已经暂停
        /// </summary>
        public bool isBGMPaused
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前BGM名字
        /// </summary>
        public string currentBGM
        {
            get
            {
                return this.BgmHandleContainer.Key;
            }
        }

        /// <summary>
        /// 获取是否有BGS在播放
        /// </summary>
        public bool isAnyBGS
        {
            get
            {
                return this.BgsHandleContainer.TrueForAll((x) => x.Key == 0) == false;
            }
        }

        /// <summary>
        /// 获取或设置是否静音
        /// </summary>
        public bool isMute
        {
            get;
            set;
        }

        /// <summary>
        /// 当前语音句柄
        /// </summary>
        private int vocalHandle
        {
            get;
            set;
        }

        /// <summary>
        /// BGM音量值
        /// </summary>
        private float bgmVolume = GlobalConfigContext.GAME_SOUND_BGMVOL;

        /// <summary>
        /// BGS音量值
        /// </summary>
        private float bgsVolume = GlobalConfigContext.GAME_SOUND_BGSVOL;

        /// <summary>
        /// SE音量值
        /// </summary>
        private float seVolume = GlobalConfigContext.GAME_SOUND_SEVOL;

        /// <summary>
        /// Vocal音量值
        /// </summary>
        private float vocalVolume = GlobalConfigContext.GAME_SOUND_VOCALVOL;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static MusicianBass synObject = null;

        /// <summary>
        /// BGM句柄容器
        /// </summary>
        private KeyValuePair<string, int> BgmHandleContainer;

        /// <summary>
        /// 音乐管理器定时器
        /// </summary>
        private Timer musicianTimer;

        /// <summary>
        /// 背景声效容器
        /// </summary>
        private List<KeyValuePair<int, float>> BgsHandleContainer;

        /// <summary>
        /// 音频引擎实例
        /// </summary>
        private BassPlayer BassEngine;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private MusicianBass()
        {
            this.BassEngine = BassPlayer.GetInstance();
            this.musicianTimer = new Timer(1000);
            this.musicianTimer.Elapsed += musicianTimer_Elapsed;
            this.BGSDefaultVolume = 800;
            this.bgmVolume = 800;
            this.VocalDefaultVolume = 1000;
            this.seVolume = 1000;
            this.Reset();
        }
    }

    /// <summary>
    /// Bass播放器
    /// </summary>
    internal class BassPlayer : IDisposable
    {
        public static BassPlayer GetInstance()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            throw new NotSupportedException();
        }
    }
}