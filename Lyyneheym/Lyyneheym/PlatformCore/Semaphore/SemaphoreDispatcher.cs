using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// 信号分发处理调度器
    /// </summary>
    internal static class SemaphoreDispatcher
    {
        /// <summary>
        /// 调度一个信号处理机
        /// </summary>
        /// <param name="selphine">信号量</param>
        /// <param name="handler">处理机</param>
        public static void Schedule(YuriSemaphore selphine, SemaphoreHandler handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 为场景注册信号调度服务
        /// </summary>
        /// <param name="scene">场景实例</param>
        public static void RegisterSemaphoreService(Scene scene)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 为场景注销信号调度服务
        /// </summary>
        /// <param name="scene">场景实例</param>
        public static void UnregisterSemaphoreService(Scene scene)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 激活一个命名信号量
        /// </summary>
        /// <param name="semaphoreName">信号的名字</param>
        public static void Activate(string semaphoreName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 熄灭一个命名信号量
        /// </summary>
        /// <param name="semaphoreName">信号的名字</param>
        public static void Deactivate(string semaphoreName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 熄灭所有的信号量
        /// </summary>
        public static void DeactivateAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止分组中的所有信号处理机
        /// </summary>
        /// <param name="group">分组名</param>
        public static void AbortHandlerGroup(string group)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止所有的信号处理机
        /// </summary>
        public static void AbortHandlerAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取或设置是否启用信号调度服务
        /// </summary>
        public static bool EnableDispatcher { get; set; } = true;
    }
}
