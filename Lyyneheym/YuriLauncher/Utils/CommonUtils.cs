using System;
using System.Threading;
using System.Collections.Generic;

namespace Yuri.YuriLauncher.Utils
{
    /// <summary>
    /// 开发辅助相关的静态方法
    /// </summary>
    internal static class CommonUtils
    {
        /// <summary>
        /// 提供将运行时环境信息输出到控制台的方法
        /// </summary>
        /// <param name="information">信息</param>
        /// <param name="causer">触发者</param>
        /// <param name="oStyle">输出的类型</param>
        public static void ConsoleLine(string information, string causer, OutputStyle oStyle)
        {
            Console.ResetColor();
            switch (oStyle)
            {
                case OutputStyle.Normal:
                    Console.WriteLine("[Information]");
                    Console.WriteLine("触发器：{0}", causer);
                    Console.WriteLine("时间戳：{0}", DateTime.Now.ToString());
                    Console.WriteLine("工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine("信  息：{0}", information);
                    break;
                case OutputStyle.Important:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("[Important]");
                    Console.WriteLine("触发器：{0}", causer);
                    Console.WriteLine("时间戳：{0}", DateTime.Now.ToString());
                    Console.WriteLine("工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine("信  息：{0}", information);
                    break;
                case OutputStyle.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Warning]");
                    Console.WriteLine("触发器：{0}", causer);
                    Console.WriteLine("时间戳：{0}", DateTime.Now.ToString());
                    Console.WriteLine("工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine("信  息：{0}", information);
                    break;
                case OutputStyle.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Error]");
                    Console.WriteLine("触发器：{0}", causer);
                    Console.WriteLine("时间戳：{0}", DateTime.Now.ToString());
                    Console.WriteLine("工作集：{0:F3} MB", Environment.WorkingSet / 1048576.0);
                    Console.WriteLine("信  息：{0}", information);
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
                CommonUtils.ConsoleLine(information, causer, oStyle);
            }
        }

        /// <summary>
        /// 交换两个对象的引用
        /// </summary>
        /// <typeparam name="T">T是要交换的类型</typeparam>
        /// <param name="a">交换变量</param>
        /// <param name="b">交换变量</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T swaper = a;
            a = b;
            b = swaper;
        }

        /// <summary>
        /// 交换两个对象在容器中的引用
        /// </summary>
        /// <typeparam name="T">T是要交换的类型</typeparam>
        /// <param name="container">交换容器</param>
        /// <param name="aId">交换变量下标</param>
        /// <param name="bId">交换变量下标</param>
        public static void Swap<T>(List<T> container, int aId, int bId)
        {
            var exchange = container[aId];
            container[aId] = container[bId];
            container[bId] = exchange;
        }
    }

    /// <summary>
    /// 枚举：信息显示风格
    /// </summary>
    internal enum OutputStyle
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
