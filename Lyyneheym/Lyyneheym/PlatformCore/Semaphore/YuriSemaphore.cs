using System;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// Yuri信号类：为游戏信号调度系统提供基础设施
    /// </summary>
    internal sealed class YuriSemaphore : YuriObservable
    {
        /// <summary>
        /// 创建一个Yuri信号
        /// </summary>
        /// <param name="name">信号量的命名</param>
        /// <param name="activated">初始时是否处于激活状态</param>
        /// <param name="tag">信号量的Tag</param>
        public YuriSemaphore(string name, bool activated, object tag) : base(tag)
        {
            this.activateFlag = activated;
            this.Name = name;
        }

        /// <summary>
        /// 获取信号的名字
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置信号是否激活，
        /// 并在状态发生变化时通知信号的订阅者
        /// </summary>
        /// <remarks>该属性是线程安全的</remarks>
        public bool Activated
        {
            get
            {
                lock (this)
                {
                    return this.activateFlag;
                }
            }
            set
            {
                lock (this)
                {
                    var orgValue = this.activateFlag;
                    this.activateFlag = value;
                    if (value != orgValue)
                    {
                        base.NotifyAll();
                    }
                }
            }
        }
        
        /// <summary>
        /// 信号量是否处于激活状态
        /// </summary>
        private bool activateFlag;
    }
}
