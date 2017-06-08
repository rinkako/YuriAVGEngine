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
            lock (this)
            {
                this.observers.Add(ob);
            }
        }

        /// <summary>
        /// 将订阅者ob从该订阅号的通知列表中移除
        /// </summary>
        /// <param name="ob">订阅者</param>
        public virtual void Detach(YuriObserver ob)
        {
            lock (this)
            {
                this.observers.Remove(ob);
            }
        }

        /// <summary>
        /// 获取当前订阅号的订阅者个数
        /// </summary>
        /// <returns>订阅者数量</returns>
        public virtual int CountObserver()
        {
            return this.observers.Count;
        }

        /// <summary>
        /// 清空所有的订阅者
        /// </summary>
        public virtual void ClearObserver()
        {
            lock (this)
            {
                this.observers.Clear();
            }
        }

        /// <summary>
        /// 广播更新通知
        /// </summary>
        public virtual void NotifyAll()
        {
            lock (this)
            {
                this.observers.ForEach(ob => ob.Notified(this));
            }
        }

        /// <summary>
        /// 将更新通知分发给指定group的订阅者
        /// </summary>
        /// <param name="group">订阅者的group</param>
        public virtual void NotifyGroup(string group)
        {
            lock (this)
            {
                foreach (var ob in this.observers)
                {
                    if (ob.ObGroup == group)
                    {
                        ob.Notified(this);
                    }
                }
            }
        }

        /// <summary>
        /// 将更新通知分发给指定tag的订阅者
        /// </summary>
        /// <param name="tag">订阅者的tag</param>
        public virtual void Notify(object tag)
        {
            lock (this)
            {
                foreach (var ob in this.observers)
                {
                    if (ob.ObserverTag.Equals(tag))
                    {
                        ob.Notified(this);
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个订阅号
        /// </summary>
        /// <param name="tag">订阅号的Tag</param>
        protected YuriObservable(object tag)
        {
            this.ObservableTag = tag;
        }

        /// <summary>
        /// 获取或设置该订阅号的Tag
        /// </summary>
        public object ObservableTag { get; set; } = null;

        /// <summary>
        /// 获取最后一次更新的附加信息
        /// </summary>
        public object UpdateInfo { get; private set; } = null;

        /// <summary>
        /// 订阅者列表
        /// </summary>
        protected List<YuriObserver> observers = new List<YuriObserver>();
    }
}
