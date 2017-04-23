using System;
using System.Collections.Generic;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// 抽象订阅号类
    /// </summary>
    public abstract class YuriObservable
    {
        /// <summary>
        /// 将订阅者ob加入该订阅号的通知列表
        /// </summary>
        /// <param name="ob">订阅者</param>
        public virtual void Attach(YuriObserver ob)
        {
            this.observers.Add(ob);
        }

        /// <summary>
        /// 将订阅者ob从该订阅号的通知列表中移除
        /// </summary>
        /// <param name="ob">订阅者</param>
        public virtual void Detach(YuriObserver ob)
        {
            this.observers.Remove(ob);
        }

        /// <summary>
        /// 广播更新通知
        /// </summary>
        public virtual void NotifyAll()
        {
            this.observers.ForEach(ob => ob.Notified(this));
        }

        /// <summary>
        /// 将更新通知分发给指定group的订阅者
        /// </summary>
        /// <param name="group">订阅者的group</param>
        public virtual void NotifyGroup(string group)
        {
            foreach (var ob in this.observers)
            {
                if (ob.Group == group)
                {
                    ob.Notified(this);
                }
            }
        }

        /// <summary>
        /// 将更新通知分发给指定tag的订阅者
        /// </summary>
        /// <param name="tag">订阅者的tag</param>
        public virtual void Notify(object tag)
        {
            foreach (var ob in this.observers)
            {
                if (ob.Tag.Equals(tag))
                {
                    ob.Notified(this);
                }
            }
        }

        /// <summary>
        /// 创建一个订阅号
        /// </summary>
        /// <param name="tag">订阅号的Tag</param>
        protected YuriObservable(object tag)
        {
            this.Tag = tag;
        }

        /// <summary>
        /// 该订阅号的Tag
        /// </summary>
        public object Tag { get; set; } = null;

        /// <summary>
        /// 获取或设置最后一次更新的附加信息
        /// </summary>
        public object UpdateInfo { get; set; } = null;

        /// <summary>
        /// 订阅者列表
        /// </summary>
        protected List<YuriObserver> observers = new List<YuriObserver>();
    }
}
