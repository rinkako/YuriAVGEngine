using System;
using System.Collections.Generic;
using System.IO;
using Yuri.Utils;

namespace Yuri.PlatformCore.Audio
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
            return NAudioPlayer.syncObject;
        }

        /// <summary>
        /// 请求一个播放通道
        /// </summary>
        /// <returns>该通道在播放器中的句柄</returns>
        public int InvokeChannel()
        {
            try
            {
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
                this.channelDict[reHandle] = new NAudioChannelPlayer();
                return reHandle;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Invoke Channel Failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
                return 0;
            }
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
                    CommonUtils.ConsoleLine("Play audio in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                this.channelDict[handle].Init(ms, vol / 1000.0f, loop, () => this.channelDict.Remove(handle));
                this.channelDict[handle].Play();
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Play audio failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                    CommonUtils.ConsoleLine("Stop audio in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                this.channelDict[handle].StopAndRelease();
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Stop and dispose audio failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                    CommonUtils.ConsoleLine("Pause audio in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                this.channelDict[handle].Pause();
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Pause audio failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                    CommonUtils.ConsoleLine("Resume play audio in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                this.channelDict[handle].Play();
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Resume play audio failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                    CommonUtils.ConsoleLine("Set volume in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                this.channelDict[handle].Volume = vol / 1000.0f;
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Set volume failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                    CommonUtils.ConsoleLine("Get playing state in empty channel:" + handle, "NAudioPlayer", OutputStyle.Error);
                    return false;
                }
                return this.channelDict[handle].IsPlaying;
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine("Get playing state failed." + ex.ToString(), "NAudioPlayer", OutputStyle.Error);
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
                ah.Value?.DisposeWithoutCallback();
            }
            this.channelDict.Clear();
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
        private readonly Random handleGenerator;

        /// <summary>
        /// 通道字典
        /// </summary>
        private readonly Dictionary<int, NAudioChannelPlayer> channelDict;
        
        /// <summary>
        /// 音频播放器的唯一实例
        /// </summary>
        private static readonly NAudioPlayer syncObject = new NAudioPlayer();
    }
}
