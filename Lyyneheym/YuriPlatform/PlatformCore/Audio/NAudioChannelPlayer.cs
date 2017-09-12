using System;
using System.IO;
using System.Threading;
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
            this.wavePlayer = new WaveOutEvent();
            MemoryStream tms = new MemoryStream();
            playStream.CopyTo(tms);
            this.playingStream = new StreamMediaFoundationReader(this.BindingStream = tms);
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
        /// 异步地过渡音乐音量到目标值
        /// </summary>
        /// <param name="destVol">目标音量</param>
        /// <param name="ms">时延毫秒</param>
        public void AsyncFadeMusic(float destVol, int ms)
        {
            if (ms == 0)
            {
                this.Volume = destVol;
                return;
            }
            if (Math.Abs(this.Volume - destVol) < 0.05)
            {
                return;
            }
            lock (this.volumeMutex)
            {
                Thread t = new Thread(new ParameterizedThreadStart(this.FadeHandler));
                t.Start(new Tuple<float, float, int>(this.Volume, destVol, ms));
            }
        }
        
        /// <summary>
        /// 异步音量调节处理器
        /// </summary>
        /// <param name="tp">参数包装</param>
        private void FadeHandler(object tp)
        {
            var t = tp as Tuple<float, float, int>;
            var beginVol = t.Item1;
            var destVol = t.Item2;
            var fadeMs = t.Item3;
            // 计算每次变更的时间间隔
            var deltaVol = destVol - beginVol;
            var dv = deltaVol / fadeMs;
            dv = dv > 0 ? (float)Math.Max(0.0001, dv) : (float)Math.Min(-0.0001, dv);
            while (Math.Abs(this.Volume - destVol) > 0.025)
            {
                this.Volume += dv;
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 停止播放该通道并释放资源
        /// </summary>
        public void StopAndRelease()
        {
            if (this.IsPlaying)
            {
                this.wavePlayer.PlaybackStopped -= this.PlaybackLoopCallback;
                this.wavePlayer.PlaybackStopped -= this.PlaybackStopCallback;
                this.wavePlayer.Stop();
                this.IsPlaying = false;
            }
            this.DisposeWithoutCallback();
        }

        /// <summary>
        /// 停止播放该通道并释放资源
        /// </summary>
        public void StopAndReleaseWithoutCallback()
        {
            if (this.IsPlaying)
            {
                this.wavePlayer.PlaybackStopped -= this.PlaybackLoopCallback;
                this.wavePlayer.PlaybackStopped -= this.PlaybackStopCallback;
                this.wavePlayer.Stop();
                this.IsPlaying = false;
            }
            this.DisposeWithoutCallback();
        }

        /// <summary>
        /// 释放通道的资源并处理结束播放回调
        /// </summary>
        public void Dispose()
        {
            this.IsPlaying = false;
            this.DisposeWithoutCallback();
            this.stopCallback?.Invoke();
        }

        /// <summary>
        /// 释放通道资源
        /// </summary>
        public void DisposeWithoutCallback()
        {
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
            if (this.playingStream != null && !Director.IsCollapsing)
            {
                this.playingStream.Position = 0;
                this.wavePlayer.Play();
            }
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
            get => this.volumeProvider.Volume;
            set
            {
                lock (this.volumeMutex)
                {
                    if (this.volumeProvider != null)
                    {
                        this.volumeProvider.Volume = value;
                    }
                }
            }
        }

        /// <summary>
        /// 音量互斥量
        /// </summary>
        private object volumeMutex = new object();

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
