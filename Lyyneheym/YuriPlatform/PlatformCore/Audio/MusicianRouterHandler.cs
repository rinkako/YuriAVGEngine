using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Yuri.Utils;
using Yuri.PlatformCore.Router;

namespace Yuri.PlatformCore.Audio
{
    /// <summary>
    /// 为音乐线程提供服务
    /// </summary>
    internal sealed class MusicianRouterHandler
    {
        /// <summary>
        /// 初始化音乐控制线程
        /// </summary>
        /// <returns>是否初始化成功</returns>
        public static bool Init()
        {
            if (MusicianRouterHandler.IsInitialized)
            {
                LogUtils.LogLine("Duplicated init musician thread", "MusicianThreadHandler", LogLevel.Warning);
                return false;
            }
            var musicianRouter = new YuriRouter()
            {
                Name = MusicianRouterHandler.MusicianRouterName
            };
            RouterManager.SetRouter(musicianRouter);
            Thread mt = new Thread(new ThreadStart(MusicianRouterHandler.MusicianHandler))
            {
                IsBackground = true
            };
            mt.Start();
            return true;
        }

        /// <summary>
        /// 入队一个音乐处理消息
        /// </summary>
        /// <param name="msg">消息对象</param>
        public static void EnqueueMessage(MusicianMessage msg)
        {
            lock (MusicianRouterHandler.syncObject)
            {
                MusicianRouterHandler.messageQueue.Enqueue(msg);
            }
        }

        /// <summary>
        /// 音乐控制线程的入口
        /// </summary>
        private static void MusicianHandler()
        {
            var musician = Musician.GetInstance();
            while (true)
            {
                if (MusicianRouterHandler.TerminalFlag)
                {
                    MusicianRouterHandler.IsInitialized = false;
                    break;
                }
                MusicianMessage mmsg = null;
                lock (MusicianRouterHandler.syncObject)
                {
                    if (MusicianRouterHandler.messageQueue.Any())
                    {
                        mmsg = MusicianRouterHandler.messageQueue.Dequeue();
                    }
                }
                if (mmsg == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                switch (mmsg.Type)
                {
                    case MusicianOperation.PlayBGM:
                        musician.PlayBGM(
                            mmsg.Args["resourceName"] as string,
                            mmsg.Args["ms"] as MemoryStream,
                            (float) mmsg.Args["vol"]
                        );
                        break;
                    case MusicianOperation.PauseBGM:
                        musician.PauseBGM();
                        break;
                    case MusicianOperation.ResumeBGM:
                        musician.ResumeBGM();
                        break;
                    case MusicianOperation.StopAndReleaseBGM:
                        musician.StopAndReleaseBGM();
                        break;
                    case MusicianOperation.PlayBGS:
                        musician.PlayBGS(
                            mmsg.Args["resourceName"] as string,
                            mmsg.Args["ms"] as MemoryStream,
                            (float) mmsg.Args["vol"],
                            (int) mmsg.Args["track"]
                        );
                        break;
                    case MusicianOperation.StopBGS:
                        musician.StopBGS();
                        break;
                    case MusicianOperation.SetBGMVolume:
                        musician.SetBGMVolume(
                            (float) mmsg.Args["vol"]
                        );
                        break;
                    case MusicianOperation.SetBGSVolume:
                        musician.SetBGSVolume(
                            (float) mmsg.Args["vol"],
                            (int) mmsg.Args["track"]
                        );
                        break;
                    case MusicianOperation.PlaySE:
                        musician.PlaySE(
                            mmsg.Args["ms"] as MemoryStream,
                            (float) mmsg.Args["vol"]
                        );
                        break;
                    case MusicianOperation.PlayVocal:
                        musician.PlayVocal(
                            mmsg.Args["ms"] as MemoryStream,
                            (float)mmsg.Args["vol"]
                        );
                        break;
                    case MusicianOperation.FadeBgm:
                        musician.FadeBgm(
                            (float)mmsg.Args["vol"],
                            (int)mmsg.Args["ms"]
                        );
                        break;
                    case MusicianOperation.StopAndReleaseVocal:
                        musician.StopAndReleaseVocal();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            MusicianRouterHandler.IsCollapsed = true;
        }
        
        /// <summary>
        /// 音乐消息队列
        /// </summary>
        private static readonly Queue<MusicianMessage> messageQueue = new Queue<MusicianMessage>();

        /// <summary>
        /// 获取音乐线程是否已经坍塌
        /// </summary>
        public static bool IsCollapsed { get; private set; } = false;

        /// <summary>
        /// 获取音乐线程是否已经初始化
        /// </summary>
        public static bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// 设置是否停止音乐线程
        /// </summary>
        public static bool TerminalFlag { private get; set; } = false;

        /// <summary>
        /// Musician路由器的名称
        /// </summary>
        public const string MusicianRouterName = "MusicianRouter";

        /// <summary>
        /// 同步量
        /// </summary>
        private static readonly object syncObject = new object();
    }

    internal sealed class MusicianMessage
    {
        public MusicianOperation Type { get; set; }
        public Dictionary<string, object> Args { get; } = new Dictionary<string, object>();
    }

    internal enum MusicianOperation
    {
        PlayBGM,
        PauseBGM,
        ResumeBGM,
        StopAndReleaseBGM,
        PlayBGS,
        StopBGS,
        SetBGMVolume,
        SetBGSVolume,
        PlaySE,
        PlayVocal,
        FadeBgm,
        StopAndReleaseVocal
    }
}
