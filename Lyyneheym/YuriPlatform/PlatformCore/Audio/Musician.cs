using System;
using System.Collections.Generic;
using System.IO;

namespace Yuri.PlatformCore.Audio
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class Musician
    {
        /// <summary>
        /// 根据音频状态描述子重演绎前端的音频
        /// </summary>
        /// <param name="mdescriptor">音频状态描述子</param>
        public void RePerform(MusicianDescriptor mdescriptor)
        {
            // 重演绎背景音乐
            if (mdescriptor.PlayingBGM == String.Empty)
            {
                this.StopAndReleaseBGM();
            }
            else if (mdescriptor.PlayingBGM == this.CurrentBgm)
            {
                this.SetBGMVolume(mdescriptor.BGMVol * (float) this.bgmVolumeRatio);
            }
            else
            {
                this.PlayBGM(mdescriptor.PlayingBGM, ResourceManager.GetInstance().GetBGM(mdescriptor.PlayingBGM),
                    mdescriptor.BGMVol * (float) this.bgmVolumeRatio);
            }
            // 重演绎背景音效
            for (int i = 0; i < mdescriptor.PlayingBGS.Count; i++)
            {
                if (mdescriptor.PlayingBGS[i] == String.Empty)
                {
                    this.StopBGS(i);
                }
                else if (mdescriptor.PlayingBGS[i] == this.BgsHandleContainer[i].Item2)
                {
                    this.SetBGSVolume(mdescriptor.BGSVol[i] * (float) this.bgsVolumeRatio, i);
                }
                else
                {
                    this.PlayBGS(mdescriptor.PlayingBGS[i],
                        ResourceManager.GetInstance().GetBGS(mdescriptor.PlayingBGS[i]),
                        mdescriptor.BGSVol[i] * (float) this.bgsVolumeRatio, i);
                }
            }
        }

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
            this.StopAndReleaseBGM();
            if (ms == null)
            {
                return;
            }
            int handle = this.audioEngine.InvokeChannel();
            this.BgmHandleContainer = new KeyValuePair<string, int>(resourceName, handle);
            var actualVol = this.IsMute ? 0 : vol * this.bgmVolumeRatio;
            this.audioEngine.InitAndPlay(handle, ms, (float)actualVol, true);
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
        /// <param name="resourceName">BGS资源名</param>
        /// <param name="ms">托管内存流</param>
        /// <param name="vol">音量（1-1000）</param>
        /// <param name="track">播放的轨道</param>
        public void PlayBGS(string resourceName, MemoryStream ms, float vol, int track = 0)
        {
            if (track >= 0 && track < GlobalConfigContext.GAME_MUSIC_BGSTRACKNUM && ms != null)
            {
                var handle = this.audioEngine.InvokeChannel();
                if (resourceName == this.BgsHandleContainer[track]?.Item2)
                {
                    this.BgsHandleContainer[track] = new Tuple<int, string, double>(handle, resourceName,
                        this.IsMute ? 0 : vol * this.bgsVolumeRatio);
                    this.SetBGSVolume(this.IsMute ? 0 : (float) (vol * this.bgsVolumeRatio), track);
                }
                else
                {
                    this.BgsHandleContainer[track] = new Tuple<int, string, double>(handle, resourceName,
                        this.IsMute ? 0 : vol * this.bgsVolumeRatio);
                    this.audioEngine.InitAndPlay(handle, ms, this.IsMute ? 0 : (float) (vol * this.bgsVolumeRatio), true);
                }
            }
        }

        /// <summary>
        /// 停止BGS并释放资源
        /// </summary>
        /// <param name="track">要停止的BGS轨道，缺省值-1表示全部停止</param>
        public void StopBGS(int track = -1)
        {
            if (track >= 0 && track < GlobalConfigContext.GAME_MUSIC_BGSTRACKNUM && this.BgsHandleContainer[track].Item1 != 0)
            {
                this.audioEngine.StopAndRelease(this.BgsHandleContainer[track].Item1);
                this.BgsHandleContainer[track] = new Tuple<int, string, double>(0, String.Empty, 1000 * this.BGSDefaultVolumeRatio);
            }
            else if (track == -1)
            {
                for (int i = 0; i < this.BgsHandleContainer.Count; i++)
                {
                    if (this.BgsHandleContainer[i].Item1 != 0)
                    {
                        int handle = this.BgsHandleContainer[i].Item1;
                        this.audioEngine.StopAndRelease(handle);
                        this.BgsHandleContainer[i] = new Tuple<int, string, double>(0, String.Empty, 1000 * this.BGSDefaultVolumeRatio);
                    }
                }
            }
        }

        /// <summary>
        /// 变更BGM的音量
        /// </summary>
        /// <param name="vol">音量值，值域[0, 1000]</param>
        public void SetBGMVolume(float vol)
        {
            if (this.IsBgmLoaded && this.IsMute == false)
            {
                this.audioEngine.SetVolume(this.BgmHandleContainer.Value, (float)(vol * this.bgmVolumeRatio));
            }
        }

        /// <summary>
        /// 变更指定轨道的BGS音量
        /// </summary>
        /// <param name="vol">音量</param>
        /// <param name="track">轨道（-1为全部变更）</param>
        public void SetBGSVolume(float vol, int track = 0)
        {
            if (track >= 0 && track < GlobalConfigContext.GAME_MUSIC_BGSTRACKNUM)
            {
                this.audioEngine.SetVolume(this.BgsHandleContainer[0].Item1, this.IsMute ? 0 : (float)(vol * this.bgsVolumeRatio));
            }
            else
            {
                foreach (var t in this.BgsHandleContainer)
                {
                    if (t.Item1 != 0)
                    {
                        this.audioEngine.SetVolume(t.Item1, this.IsMute ? 0 : (float)(vol * this.bgsVolumeRatio));
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
            this.audioEngine.InitAndPlay(handle, ms, this.IsMute ? 0 : (float)(vol * this.seVolumeRatio), false);
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
            Musician.IsVoicePlaying = true;
            this.audioEngine.InitAndPlay(this.VocalHandle, ms, this.IsMute ? 0 : (float)(vol * this.vocalVolumeRatio), false, true);
        }

        /// <summary>
        /// 淡入淡出BGM到指定音量
        /// </summary>
        /// <param name="vol">目标音量</param>
        /// <param name="ms">淡入淡出毫秒数</param>
        public void FadeBgm(float vol, int ms)
        {
            if (this.IsBgmLoaded && this.IsMute == false)
            {
                this.audioEngine.Fading(this.BgmHandleContainer.Value, (float)(vol * this.bgmVolumeRatio), ms);
            }
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
            Musician.IsVoicePlaying = false;
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
                this.BgsHandleContainer = new List<Tuple<int, string, double>>();
            }
            else
            {
                this.BgsHandleContainer.Clear();
            }
            for (int i = 0; i < GlobalConfigContext.GAME_MUSIC_BGSTRACKNUM; i++)
            {
                this.BgsHandleContainer.Add(new Tuple<int, string, double>(0, String.Empty, this.BGSDefaultVolumeRatio));
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
            return Musician.synObject == null
                ? Musician.synObject = new Musician()
                : Musician.synObject;
        }

        /// <summary>
        /// 获取或设置BGM音量
        /// </summary>
        public double BGMVolumeRatio
        {
            get => this.bgmVolumeRatio;
            set => this.bgmVolumeRatio = Math.Max(0, Math.Min(value, 1));
        }

        /// <summary>
        /// 获取或设置BGS默认音量
        /// </summary>
        public double BGSDefaultVolumeRatio
        {
            get => this.bgsVolumeRatio;
            set => this.bgsVolumeRatio = Math.Max(0, Math.Min(value, 1));
        }
        
        /// <summary>
        /// 获取或设置SE默认音量
        /// </summary>
        public double SEDefaultVolumeRatio
        {
            get => this.seVolumeRatio;
            set => this.seVolumeRatio = Math.Max(0, Math.Min(value, 1));
        }

        /// <summary>
        /// 获取或设置Vocal默认音量
        /// </summary>
        public double VocalDefaultVolumeRatio
        {
            get => this.vocalVolumeRatio;
            set => this.vocalVolumeRatio = Math.Max(0, Math.Min(value, 1));
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
        /// 获取语音是否正在播放
        /// </summary>
        public static bool IsVoicePlaying
        {
            get;
            set;
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
        public string CurrentBgm => this.BgmHandleContainer.Key;

        /// <summary>
        /// 获取是否有BGS在播放
        /// </summary>
        public bool IsAnyBgs => this.BgsHandleContainer.TrueForAll((x) => x.Item1 == 0) == false;

        /// <summary>
        /// 获取或设置是否静音
        /// </summary>
        public bool IsMute
        {
            get;
            set;
        } = false;

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
        private double bgmVolumeRatio;

        /// <summary>
        /// BGS音量值
        /// </summary>
        private double bgsVolumeRatio;

        /// <summary>
        /// SE音量值
        /// </summary>
        private double seVolumeRatio;

        /// <summary>
        /// Vocal音量值
        /// </summary>
        private double vocalVolumeRatio;

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
        private List<Tuple<int, string, double>> BgsHandleContainer;

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
            this.bgmVolumeRatio = GlobalConfigContext.GAME_SOUND_BGMVOL / 1000.0;
            this.bgsVolumeRatio = GlobalConfigContext.GAME_SOUND_BGSVOL / 1000.0;
            this.seVolumeRatio = GlobalConfigContext.GAME_SOUND_SEVOL / 1000.0;
            this.vocalVolumeRatio = GlobalConfigContext.GAME_SOUND_VOCALVOL / 1000.0;
            this.Reset();
        }
    }
}
