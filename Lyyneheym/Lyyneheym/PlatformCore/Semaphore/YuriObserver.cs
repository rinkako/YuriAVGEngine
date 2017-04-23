using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// 抽象观察者类
    /// </summary>
    public abstract class YuriObserver
    {
        /// <summary>
        /// 创建一个观察者
        /// </summary>
        /// <param name="group">观察者分组名</param>
        /// <param name="tag">观察者的Tag</param>
        protected YuriObserver(string group, object tag)
        {
            this.Group = group;
            this.Tag = tag;
        }

        /// <summary>
        /// 接受通知
        /// </summary>
        /// <param name="notifier">通知者</param>
        public abstract void Notified(YuriObservable notifier);

        /// <summary>
        /// 该观察者的Group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 该观察者的Tag
        /// </summary>
        public object Tag { get; set; }
    }
}
