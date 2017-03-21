using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class Musician
    {
        /// <summary>
        /// <para>播放背景音乐：从内存读入资源</para>
        /// <para>背景音乐在同一时刻只能播放一个资源</para>
        /// </summary>
        /// <param name="resourceName">资源名</param>
        /// <param name="ms">托管内存流</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayBGM(string resourceName, MemoryStream ms, float vol)
        {
            // 如果有BGM在播放就截断
            if (this.IsBgmPlaying || this.IsBgmPaused)
            {
                this.StopAndReleaseBGM();
            }
            if (ms == null)
            {
                return;
            }
            int handle = this.audioEngine.InvokeChannel();
            this.BgmHandleContainer = new KeyValuePair<string, int>(resourceName, handle);
            this.bgmVolume = vol;
            this.audioEngine.InitAndPlay(handle, ms, vol, true);
            this.IsBgmPlaying = this.IsBgmLoaded = true;
            this.IsBgmPaused = false;
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            if (this.IsBgmLoaded && this.IsBgmPlaying)
            {
                this.audioEngine.Pause(this.BgmHandleContainer.Value);
                this.IsBgmPlaying = false;
                this.IsBgmPaused = true;
            }
        }

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            if (this.IsBgmLoaded && this.IsBgmPaused)
            {
                this.audioEngine.ResumePlay(this.BgmHandleContainer.Value);
                this.IsBgmPlaying = true;
                this.IsBgmPaused = false;
            }
        }

        /// <summary>
        /// 停止BGM并释放资源
        /// </summary>
        public void StopAndReleaseBGM()
        {
            if (this.IsBgmLoaded)
            {
                this.audioEngine.StopAndRelease(this.BgmHandleContainer.Value);
                this.IsBgmLoaded = this.IsBgmPlaying = false;
                this.BgmHandleContainer = new KeyValuePair<string, int>(null, 0);
            }
        }

        /// <summary>
        /// <para>播放背景音效：从内存读入资源</para>
        /// <para>背景声效可以多个声音资源同时播放，并且可以与BGM同存</para>
        /// </summary>
        /// <param name="ms">托管内存流</param>
        /// <param name="vol">音量（1-1000）</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(MemoryStream ms, float vol, int track = 0)
        {
            if (track >= 0 && track < GlobalDataContext.GAME_MUSIC_BGSTRACKNUM && ms != null)
            {
                var handle = this.audioEngine.InvokeChannel();
                this.BgsHandleContainer[track] = new KeyValuePair<int, float>(handle, vol);
                this.audioEngine.InitAndPlay(handle, ms, vol, true);
            }
        }

        /// <summary>
        /// 停止BGS并释放资源
        /// </summary>
        /// <param name="track">要停止的BGS轨道，缺省值-1表示全部停止</param>
        public void StopBGS(int track = -1)
        {
            if (track >= 0 && track < GlobalDataContext.GAME_MUSIC_BGSTRACKNUM && this.BgsHandleContainer[track].Key != 0)
            {
                this.audioEngine.StopAndRelease(this.BgsHandleContainer[track].Key);
            }
            else
            {
                for (int i = 0; i < this.BgsHandleContainer.Count; i++)
                {
                    if (this.BgsHandleContainer[i].Key != 0)
                    {
                        int handle = this.BgsHandleContainer[i].Key;
                        this.audioEngine.StopAndRelease(handle);
                        this.BgsHandleContainer[i] = new KeyValuePair<int, float>(0, this.BGSDefaultVolume);
                    }
                }
            }
        }

        /// <summary>
        /// 变更BGM的音量
        /// </summary>
        /// <param name="vol"></param>
        public void SetBGMVolume(float vol)
        {
            if (this.IsBgmLoaded)
            {
                this.BGMVolume = vol;
            }
        }

        /// <summary>
        /// 变更指定轨道的BGS音量
        /// </summary>
        /// <param name="vol">音量</param>
        /// <param name="track">轨道（-1为全部变更）</param>
        public void SetBGSVolume(int vol, int track = 0)
        {
            if (track >= 0 && track < GlobalDataContext.GAME_MUSIC_BGSTRACKNUM)
            {
                this.audioEngine.SetVolume(this.BgsHandleContainer[0].Key, vol);
            }
            else
            {
                foreach (var t in this.BgsHandleContainer)
                {
                    if (t.Key != 0)
                    {
                        this.audioEngine.SetVolume(t.Key, vol);
                    }
                }
            }
        }
        
        /// <summary>
        /// <para>播放声效：从内存读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="ms">托管内存流</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlaySE(MemoryStream ms, float vol)
        {
            if (ms == null)
            {
                return;
            }
            int handle = this.audioEngine.InvokeChannel();
            this.audioEngine.InitAndPlay(handle, ms, vol, false);
        }

        /// <summary>
        /// <para>播放语音：从内存读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="ms">托管内存流</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayVocal(MemoryStream ms, float vol)
        {
            if (ms == null)
            {
                return;
            }
            this.StopAndReleaseVocal();
            this.VocalHandle = this.audioEngine.InvokeChannel();
            this.audioEngine.InitAndPlay(this.VocalHandle, ms, vol, false);
        }

        /// <summary>
        /// 停止语音并释放资源
        /// </summary>
        public void StopAndReleaseVocal()
        {
            if (this.VocalHandle != 0 && this.audioEngine.IsPlaying(this.VocalHandle))
            {
                this.audioEngine.StopAndRelease(this.VocalHandle);
            }
            this.VocalHandle = 0;
        }

        /// <summary>
        /// 复位音乐管理器
        /// </summary>
        public void Reset()
        {
            this.StopAndReleaseBGM();
            this.StopAndReleaseVocal();
            this.BgmHandleContainer = new KeyValuePair<string, int>(null, 0);
            if (this.BgsHandleContainer == null)
            {
                this.BgsHandleContainer = new List<KeyValuePair<int, float>>();
            }
            else
            {
                this.BgsHandleContainer.Clear();
            }
            for (int i = 0; i < GlobalDataContext.GAME_MUSIC_BGSTRACKNUM; i++)
            {
                this.BgsHandleContainer.Add(new KeyValuePair<int, float>(0, this.BGSDefaultVolume));
            }
            this.IsBgmLoaded = this.IsBgmPaused = this.IsBgmPlaying = this.IsMute = false;
        }

        /// <summary>
        /// 释放音乐播放相关的资源
        /// </summary>
        public void Dispose()
        {
            this.audioEngine?.Dispose();
        }

        /// <summary>
        /// 工厂方法：获得音乐管理器类的唯一实例
        /// </summary>
        /// <returns>音乐管理器</returns>
        public static Musician GetInstance()
        {
            return Musician.synObject == null ? Musician.synObject = new Musician() : Musician.synObject;
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
                this.audioEngine.SetVolume(this.BgmHandleContainer.Value, this.bgmVolume);
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
        public bool IsBgmPlaying
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BGM是否已经加载
        /// </summary>
        public bool IsBgmLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BGM是否已经暂停
        /// </summary>
        public bool IsBgmPaused
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前BGM名字
        /// </summary>
        public string CurrentBgm
        {
            get
            {
                return this.BgmHandleContainer.Key;
            }
        }

        /// <summary>
        /// 获取是否有BGS在播放
        /// </summary>
        public bool IsAnyBgs
        {
            get
            {
                return this.BgsHandleContainer.TrueForAll((x) => x.Key == 0) == false;
            }
        }

        /// <summary>
        /// 获取或设置是否静音
        /// </summary>
        public bool IsMute
        {
            get;
            set;
        }

        /// <summary>
        /// 当前语音句柄
        /// </summary>
        private int VocalHandle
        {
            get;
            set;
        }

        /// <summary>
        /// BGM音量值
        /// </summary>
        private float bgmVolume;

        /// <summary>
        /// BGS音量值
        /// </summary>
        private float bgsVolume;

        /// <summary>
        /// SE音量值
        /// </summary>
        private float seVolume;

        /// <summary>
        /// Vocal音量值
        /// </summary>
        private float vocalVolume;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Musician synObject = null;

        /// <summary>
        /// BGM句柄容器
        /// </summary>
        private KeyValuePair<string, int> BgmHandleContainer;
        
        /// <summary>
        /// 背景声效容器
        /// </summary>
        private List<KeyValuePair<int, float>> BgsHandleContainer;

        /// <summary>
        /// 音频引擎实例
        /// </summary>
        private readonly NAudioPlayer audioEngine;
    
        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Musician()
        {
            this.audioEngine = NAudioPlayer.GetInstance();
            this.bgmVolume = GlobalDataContext.GAME_SOUND_BGMVOL;
            this.bgsVolume = GlobalDataContext.GAME_SOUND_BGSVOL;
            this.seVolume = GlobalDataContext.GAME_SOUND_SEVOL;
            this.vocalVolume = GlobalDataContext.GAME_SOUND_VOCALVOL;
            this.Reset();
        }
    }
}
