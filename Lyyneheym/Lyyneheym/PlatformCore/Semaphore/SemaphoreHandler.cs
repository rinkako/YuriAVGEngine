using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// 信号处理机类
    /// </summary>
    internal sealed class SemaphoreHandler : YuriObserver
    {
        /// <summary>
        /// 创建一个信号处理机
        /// </summary>
        /// <param name="group">观察者分组名</param>
        /// <param name="tag">观察者的Tag</param>
        public SemaphoreHandler(string group, object tag) : base(group, tag) { }

        /// <summary>
        /// 接受信号变更通知并执行处理函数
        /// </summary>
        /// <param name="notifier">通知者</param>
        public override void Notified(YuriObservable notifier)
        {
            SemaphoreDispatcher.Schedule(notifier as YuriSemaphore, this);
        }

        /// <summary>
        /// 获取信号处理机的描述文本
        /// </summary>
        /// <returns>信号处理机描述</returns>
        public override string ToString()
        {
            return String.Format("{0} <- {1}", this.Type, this.Handler.GlobalName);
        }

        /// <summary>
        /// 获取或设置绑定的处理函数
        /// </summary>
        public SceneFunction Handler { get; set; }

        /// <summary>
        /// 获取或设置信号处理机类型
        /// </summary>
        public SemaphoreHandlerType Type { get; set; } = SemaphoreHandlerType.ScheduleOnce;
    }

    /// <summary>
    /// 枚举：信号处理机类型
    /// </summary>
    internal enum SemaphoreHandlerType
    {
        /// <summary>
        /// 在信号被激活时执行一次处理函数
        /// </summary>
        ScheduleOnce,
        /// <summary>
        /// 在信号被激活时循环执行处理函数直到函数主动退出
        /// </summary>
        ScheduleForever,
        /// <summary>
        /// 在信号被激活时循环执行处理函数直到信号被熄灭
        /// </summary>
        ScheduleWhenActivated
    }
}
