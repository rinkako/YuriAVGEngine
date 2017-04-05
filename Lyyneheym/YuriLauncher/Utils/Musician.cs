using System;
using System.Collections.Generic;
using System.IO;

namespace Yuri.YuriLauncher.Utils
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
        /// <param name="vol">音量（1-1000）</param>
        public void PlayBGM(string resourceName, string filename, float vol)
        {
            // 如果有BGM在播放就截断
            if (this.IsBGMPlaying || this.isBGMPaused)
            {
                this.StopAndReleaseBGM();
            }
            int handle = this.audioEngine.InvokeChannel();
            BgmHandleContainer = new KeyValuePair<string, int>(resourceName, handle);
            this.bgmVolume = vol;
            this.audioEngine.InitAndPlay(handle, new MemoryStream(File.ReadAllBytes(filename)), this.bgmVolume, true);
            this.IsBGMPlaying = this.IsBGMLoaded = true;
            this.isBGMPaused = false;
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            if (this.IsBGMLoaded && this.IsBGMPlaying)
            {
                this.audioEngine.Pause(this.BgmHandleContainer.Value);
                this.IsBGMPlaying = false;
                this.isBGMPaused = true;
            }
        }

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            if (this.IsBGMLoaded && this.isBGMPaused)
            {
                this.audioEngine.ResumePlay(this.BgmHandleContainer.Value);
                this.IsBGMPlaying = true;
                this.isBGMPaused = false;
            }
        }

        /// <summary>
        /// 停止BGM并释放资源
        /// </summary>
        public void StopAndReleaseBGM()
        {
            if (this.IsBGMLoaded)
            {
                this.audioEngine.StopAndRelease(this.BgmHandleContainer.Value);
                this.IsBGMLoaded = this.IsBGMPlaying = false;
                this.BgmHandleContainer = new KeyValuePair<string, int>(null, 0);
            }
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
            if (track >= 0 && track < this.BgsHandleContainer.Count)
            {
                int handle = this.audioEngine.InvokeChannel();
                this.audioEngine.InitAndPlay(handle, new MemoryStream(File.ReadAllBytes(filename)), vol, true);
                this.BgsHandleContainer[track] = new KeyValuePair<int, float>(handle, vol);
            }
        }

        /// <summary>
        /// 停止BGS并释放资源
        /// </summary>
        /// <param name="track">要停止的BGS轨道，缺省值-1表示全部停止</param>
        public void StopBGS(int track = -1)
        {
            if (track >= 0 && track < this.BgsHandleContainer.Count && this.BgsHandleContainer[track].Key != 0)
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
            if (this.IsBGMLoaded)
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
            if (track >= 0 && track < this.BgsHandleContainer.Count)
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
        /// <para>播放声效：从文件读入资源</para>
        /// <para>声效可以多个声音资源同时播放</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlaySE(string filename, float vol)
        {
            int handle = this.audioEngine.InvokeChannel();
            this.audioEngine.InitAndPlay(handle, new MemoryStream(File.ReadAllBytes(filename)), vol, false);
        }

        /// <summary>
        /// <para>播放语音：从文件读入资源</para>
        /// <para>语音在同一时刻只能播放一个资源，但可以与BGM和BGS共存</para>
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="vol">音量（1-1000）</param>
        public void PlayVocal(string filename, float vol)
        {
            this.StopAndReleaseVocal();
            this.vocalHandle = this.audioEngine.InvokeChannel();
            this.audioEngine.InitAndPlay(this.vocalHandle, new MemoryStream(File.ReadAllBytes(filename)), vol, false);
        }

        /// <summary>
        /// 停止语音并释放资源
        /// </summary>
        public void StopAndReleaseVocal()
        {
            if (this.vocalHandle != 0 && this.audioEngine.IsPlaying(this.vocalHandle))
            {
                this.audioEngine.StopAndRelease(this.vocalHandle);
            }
            this.vocalHandle = 0;
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
            for (int i = 0; i < 4; i++)
            {
                this.BgsHandleContainer.Add(new KeyValuePair<int, float>(0, this.BGSDefaultVolume));
            }
            this.IsBGMLoaded = this.isBGMPaused = this.IsBGMPlaying = this.IsMute = false;
        }
        
        /// <summary>
        /// 工厂方法：获得音乐管理器类的唯一实例
        /// </summary>
        /// <returns>音乐管理器</returns>
        public static Musician GetInstance()
        {
            return synObject ?? (synObject = new Musician());
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
        public bool IsBGMPlaying
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取BGM是否已经加载
        /// </summary>
        public bool IsBGMLoaded
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
        public string CurrentBGM
        {
            get
            {
                return this.BgmHandleContainer.Key;
            }
        }

        /// <summary>
        /// 获取是否有BGS在播放
        /// </summary>
        public bool IsAnyBGS
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
        private int vocalHandle
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
        private NAudioPlayer audioEngine;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Musician()
        {
            this.audioEngine = NAudioPlayer.GetInstance();
            this.bgsVolume = 800;
            this.bgmVolume = 800;
            this.vocalVolume = 1000;
            this.seVolume = 1000;
            this.Reset();
        }
    }
}
