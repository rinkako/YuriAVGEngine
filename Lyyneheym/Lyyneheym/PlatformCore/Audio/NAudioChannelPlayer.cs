using System;
using System.IO;
using NAudio.Wave;

namespace Yuri.PlatformCore.Audio
{
    /// <summary>
    /// NAudio单音轨音频播放器
    /// </summary>
    internal sealed class NAudioChannelPlayer
    {
        /// <summary>
        /// 初始化通道
        /// </summary>
        public void Init(MemoryStream playStream, float volume, bool loop, Action stopCallback)
        {
            this.wavePlayer = new WaveOut();
            this.playingStream = new StreamMediaFoundationReader(this.BindingStream = playStream);
            this.volumeProvider = new VolumeWaveProvider16(playingStream) { Volume = volume };
            this.wavePlayer.Init(this.volumeProvider);
            this.IsLoop = loop;
            this.stopCallback = stopCallback;
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
        /// 释放通道的资源并处理结束播放回调
        /// </summary>
        public void Dispose()
        {
            this.DisposeWithoutCallback();
            this.stopCallback?.Invoke();
        }

        /// <summary>
        /// 释放通道资源
        /// </summary>
        public void DisposeWithoutCallback()
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
                return this.volumeProvider.Volume;
            }
            set
            {
                if (this.volumeProvider != null)
                {
                    this.volumeProvider.Volume = value;
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
        /// 播放结束时触发回调
        /// </summary>
        private Action stopCallback = null;

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
