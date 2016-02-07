using System;
using System.Threading;
using System.Collections.Generic;

namespace Lyyneheym.LyyneheymCore.Utils
{
    /// <summary>
    /// 开发辅助相关的静态方法
    /// </summary>
    public static class DebugUtils
    {
        /// <summary>
        /// 提供将运行时环境信息输出到控制台的方法
        /// </summary>
        /// <param name="information">信息</param>
        /// <param name="causer">触发者</param>
        public static void ConsoleLine(string information, string causer, OutputStyle oStyle)
        {
            Console.ResetColor();
            switch (oStyle)
            {
                case OutputStyle.Normal:
                    Console.WriteLine("[Information]");
                    Console.WriteLine(String.Format("触发器：{0}", causer));
                    Console.WriteLine(String.Format("时间戳：{0}", DateTime.Now.ToString()));
                    Console.WriteLine(String.Format("信  息：{0}", information));
                    break;
                case OutputStyle.Important:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("[Important]");
                    Console.WriteLine(String.Format("触发器：{0}", causer));
                    Console.WriteLine(String.Format("时间戳：{0}", DateTime.Now.ToString()));
                    Console.WriteLine(String.Format("信  息：{0}", information));
                    break;
                case OutputStyle.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Warning]");
                    Console.WriteLine(String.Format("触发器：{0}", causer));
                    Console.WriteLine(String.Format("时间戳：{0}", DateTime.Now.ToString()));
                    Console.WriteLine(String.Format("信  息：{0}", information));
                    break;
                case OutputStyle.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Error]");
                    Console.WriteLine(String.Format("触发器：{0}", causer));
                    Console.WriteLine(String.Format("时间戳：{0}", DateTime.Now.ToString()));
                    Console.WriteLine(String.Format("信  息：{0}", information));
                    break;
                case OutputStyle.Simple:
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
        public static void AsyncConsoleLine(string information, string causer, Mutex consoleMutex, OutputStyle oStyle)
        {
            lock (consoleMutex)
            {
                DebugUtils.ConsoleLine(information, causer, oStyle);
            }
        }
    }

    /// <summary>
    /// 枚举：信息显示风格
    /// </summary>
    public enum OutputStyle
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
