using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;

namespace Yuri.YuriHalation.HalationCore
{    
    /// <summary>
    /// NAudio音频播放器
    /// </summary>
    internal sealed class NAudioPlayer
    {
        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>音频播放器的唯一实例</returns>
        public static NAudioPlayer GetInstance()
        {
            return NAudioPlayer.syncObject == null
                ? NAudioPlayer.syncObject = new NAudioPlayer()
                : NAudioPlayer.syncObject;
        }

        /// <summary>
        /// 请求一个播放通道
        /// </summary>
        /// <param name="ms">内存流</param>
        /// <returns>该通道在播放器中的句柄</returns>
        public int InvokeChannel()
        {
            NAudioChannelPlayer ncp = new NAudioChannelPlayer();
            int reHandle = this.handleGenerator.Next(Int32.MinValue, Int32.MaxValue);
            int encounter = 0;
            while (this.channelDict.ContainsKey(reHandle) || reHandle == 0)
            {
                reHandle = this.handleGenerator.Next(Int32.MinValue, Int32.MaxValue);
                if (encounter++ >= 10000)
                {
                    throw new OutOfMemoryException();
                }
            }
            this.channelDict[reHandle] = ncp;
            return reHandle;
        }

        /// <summary>
        /// 初始化并播放通道
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <param name="ms">绑定到通道的内存流</param>
        /// <param name="vol">音量，值域[0, 1000]</param>
        /// <param name="loop">是否循环</param>
        /// <returns>动作是否成功</returns>
        public bool InitAndPlay(int handle, MemoryStream ms, float vol, bool loop)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Play audio in empty channel:" + handle);
                    return false;
                }
                this.channelDict[handle].Init(ms, vol / 1000.0f, loop);
                this.channelDict[handle].Play();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Play audio failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 停止通道并释放资源
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <returns>动作是否成功</returns>
        public bool StopAndRelease(int handle)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Stop audio in empty channel:" + handle);
                    return false;
                }
                this.channelDict[handle].StopAndRelease();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop and dispose audio failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 暂停通道的播放
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <returns>动作是否成功</returns>
        public bool Pause(int handle)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Pause audio in empty channel:" + handle);
                    return false;
                }
                this.channelDict[handle].Pause();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pause audio failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 恢复通道的播放
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <returns>动作是否成功</returns>
        public bool ResumePlay(int handle)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Resume play audio in empty channel:" + handle);
                    return false;
                }
                this.channelDict[handle].Play();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Resume play audio failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 调整通道的音量
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <param name="vol">音量，值域[0, 1000]</param>
        /// <returns>动作是否成功</returns>
        public bool SetVolume(int handle, float vol)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Set volume in empty channel:" + handle);
                    return false;
                }
                this.channelDict[handle].Volume = vol / 1000.0f;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set volume failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获取通道是否处于播放状态
        /// </summary>
        /// <param name="handle">通道句柄</param>
        /// <returns>是否处于播放状态，如果通道为空则返回false</returns>
        public bool IsPlaying(int handle)
        {
            try
            {
                if (this.channelDict.ContainsKey(handle) == false)
                {
                    Console.WriteLine("Get playing state in empty channel:" + handle);
                    return false;
                }
                return this.channelDict[handle].IsPlaying;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get playing state failed." + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var ah in this.channelDict)
            {
                ah.Value?.Dispose();
            }
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private NAudioPlayer()
        {
            this.channelDict = new Dictionary<int, NAudioChannelPlayer>();
            this.handleGenerator = new Random();
        }

        /// <summary>
        /// 句柄生成器
        /// </summary>
        private Random handleGenerator;

        /// <summary>
        /// 通道字典
        /// </summary>
        private Dictionary<int, NAudioChannelPlayer> channelDict;

        /// <summary>
        /// 音频播放器的唯一实例
        /// </summary>
        private static NAudioPlayer syncObject = null;
    }

    /// <summary>
    /// NAudio单音轨音频播放器
    /// </summary>
    internal sealed class NAudioChannelPlayer
    {
        /// <summary>
        /// 初始化通道
        /// </summary>
        public void Init(MemoryStream playStream, float volume, bool loop)
        {
            this.wavePlayer = new WaveOut();
            this.playingStream = new StreamMediaFoundationReader(this.BindingStream = playStream);
            this.volumeProvider = new VolumeWaveProvider16(playingStream) { Volume = volume };
            this.wavePlayer.Init(this.volumeProvider);
            this.IsLoop = loop;
            if (loop)
            {
                this.wavePlayer.PlaybackStopped += this.PlaybackLoopCallback;
            }
            else
            {
                this.wavePlayer.PlaybackStopped += this.PlaybackStopCallback;
            }
        }

        /// <summary>
        /// 播放播放该通道
        /// </summary>
        public void Play()
        {
            if (!this.IsPlaying)
            {
                this.wavePlayer?.Play();
                this.IsPlaying = true;
            }
        }

        /// <summary>
        /// 暂停通道
        /// </summary>
        public void Pause()
        {
            if (this.IsPlaying)
            {
                this.wavePlayer.Pause();
                this.IsPlaying = false;
            }
        }

        /// <summary>
        /// 停止播放该通道并释放资源
        /// </summary>
        public void StopAndRelease()
        {
            if (this.IsPlaying)
            {
                this.wavePlayer.Stop();
            }
            this.Dispose();
        }

        /// <summary>
        /// 释放通道的资源
        /// </summary>
        public void Dispose()
        {
            if (this.IsPlaying)
            {
                this.wavePlayer.Stop();
            }
            if (this.playingStream != null)
            {
                this.playingStream.Dispose();
                this.playingStream = null;
            }
            if (this.wavePlayer != null)
            {
                this.wavePlayer.Dispose();
                this.wavePlayer = null;
            }
            this.IsPlaying = false;
        }

        /// <summary>
        /// 播放结束回调：循环播放
        /// </summary>
        private void PlaybackLoopCallback(object sender, StoppedEventArgs e)
        {
            this.playingStream.Position = 0;
            this.wavePlayer.Play();
        }

        /// <summary>
        /// 播放结束回调：单次播放
        /// </summary>
        private void PlaybackStopCallback(object sender, StoppedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// 获取或设置通道音量
        /// </summary>
        public float Volume
        {
            get
            {
                return volumeProvider.Volume;
            }
            set
            {
                if (value >= 0 && value <= 1f && this.volumeProvider != null)
                {
                    volumeProvider.Volume = value;
                }
            }
        }

        /// <summary>
        /// 获取该通道是否正在播放音乐
        /// </summary>
        public bool IsPlaying { get; private set; } = false;

        /// <summary>
        /// 获取该通道是否循环播放
        /// </summary>
        public bool IsLoop { get; private set; } = true;

        /// <summary>
        /// 获取通道绑定的内存流
        /// </summary>
        public MemoryStream BindingStream { get; set; } = null;

        /// <summary>
        /// 获取通道播放的位置戳
        /// </summary>
        public TimeSpan CurrentTime => this.playingStream?.CurrentTime ?? TimeSpan.Zero;

        /// <summary>
        /// 获取通道音乐总长度
        /// </summary>
        public TimeSpan TotalTime => this.playingStream?.TotalTime ?? TimeSpan.Zero;

        /// <summary>
        /// 音轨播放器
        /// </summary>
        private IWavePlayer wavePlayer = null;

        /// <summary>
        /// 音轨播放器的流
        /// </summary>
        private StreamMediaFoundationReader playingStream = null;

        /// <summary>
        /// 可控制流音量的波形提供器
        /// </summary>
        private VolumeWaveProvider16 volumeProvider = null;
    }
}