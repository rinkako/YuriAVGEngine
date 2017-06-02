using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Yuri.Utils
{
    /// <summary>
    /// 日志相关的静态方法集
    /// </summary>
    internal static class LogUtils
    {
        /// <summary>
        /// 提供将运行时环境信息输出到控制台的方法
        /// </summary>
        /// <param name="information">信息</param>
        /// <param name="causer">触发者</param>
        /// <param name="oStyle">输出的类型</param>
        public static void LogLine(string information, string causer, LogLevel oStyle)
        {
            Console.ResetColor();
            switch (oStyle)
            {
                case LogLevel.Normal:
                    Console.WriteLine(@"[Information]");
                    Console.WriteLine(@"触发器：{0}", causer);
                    Console.WriteLine(@"时间戳：{0}", DateTime.Now);
                    Console.WriteLine(@"工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine(@"信  息：{0}", information);
                    break;
                case LogLevel.Important:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(@"[Important]");
                    Console.WriteLine(@"触发器：{0}", causer);
                    Console.WriteLine(@"时间戳：{0}", DateTime.Now);
                    Console.WriteLine(@"工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine(@"信  息：{0}", information);
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(@"[Warning]");
                    Console.WriteLine(@"触发器：{0}", causer);
                    Console.WriteLine(@"时间戳：{0}", DateTime.Now);
                    Console.WriteLine(@"工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine(@"信  息：{0}", information);
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(@"[Error]");
                    Console.WriteLine(@"触发器：{0}", causer);
                    Console.WriteLine(@"时间戳：{0}", DateTime.Now);
                    Console.WriteLine(@"工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine(@"信  息：{0}", information);
                    MessageBox.Show(@"At: " + causer + Environment.NewLine + information, @"YuriError");
                    break;
                case LogLevel.Simple:
                default:
                    Console.WriteLine(information);
                    break;
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// 提供异步输出运行时信息的方法
        /// </summary>
        /// <param name="information">信息</param>
        /// <param name="causer">触发者</param>
        /// <param name="consoleMutex">互斥量</param>
        public static void AsyncLogLine(string information, string causer, Mutex consoleMutex, LogLevel oStyle)
        {
            lock (consoleMutex)
            {
                LogUtils.LogLine(information, causer, oStyle);
            }
        }
    }

    /// <summary>
    /// 枚举：信息显示风格
    /// </summary>
    internal enum LogLevel
    {
        /// <summary>
        /// 正常输出
        /// </summary>
        Normal,
        /// <summary>
        /// 只输出信息
        /// </summary>
        Simple,
        /// <summary>
        /// 重要信息，以蓝色显示
        /// </summary>
        Important,
        /// <summary>
        /// 警告信息，以黄色显示
        /// </summary>
        Warning,
        /// <summary>
        /// 错误信息，以红色显示
        /// </summary>
        Error
    }
}
