using System;

namespace Yuri.PlatformCore.Router
{
    /// <summary>
    /// 路由消息类：为引擎模块间通讯提供消息包装
    /// </summary>
    public class YuriRoutedEvent
    {
        /// <summary>
        /// 委托：消息被路由接收并处理时
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public delegate void OnRouterAcceptHandler(IRouteable sender, EventArgs e);

        /// <summary>
        /// 委托：消息被路由即将开始转发给子路由时
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public delegate void OnRouterRoutedHandler(IRouteable sender, EventArgs e);
        
        /// <summary>
        /// 委托：路由器吞下消息时
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public delegate void OnRouterSwallowedHandler(IRouteable sender, EventArgs e);

        /// <summary>
        /// 委托：路由器处理消息完毕时的委托
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public delegate void OnProcessedHandler(IRouteable sender, EventArgs e);

        /// <summary>
        /// 消息被路由接收并处理时发生
        /// </summary>
        public event OnRouterAcceptHandler OnRouterAccept;

        /// <summary>
        /// 消息被路由即将开始转发给下一路由时发生
        /// </summary>
        public event OnRouterRoutedHandler OnRouterRouted;

        /// <summary>
        /// 路由器吞下消息时发生
        /// </summary>
        public event OnRouterSwallowedHandler OnRouterSwallowed;

        /// <summary>
        /// 路由器处理消息完毕时发生
        /// </summary>
        public event OnProcessedHandler OnProcessed;

        /// <summary>
        /// 引发被路由接收并处理时事件
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public void RaiseOnRouterAcceptEvent(IRouteable sender, EventArgs e)
        {
            this.OnRouterAccept?.Invoke(sender, e);
        }

        /// <summary>
        /// 引发被路由即将开始转发给子路由时事件
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public void RaiseOnRouterRoutedEvent(IRouteable sender, EventArgs e)
        {
            this.OnRouterRouted?.Invoke(sender, e);
        }

        /// <summary>
        /// 引发路由器吞下消息时事件
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public void RaiseRouterSwallowedEvent(IRouteable sender, EventArgs e)
        {
            this.OnRouterSwallowed?.Invoke(sender, e);
        }

        /// <summary>
        /// 引发路由器处理消息完毕时的事件
        /// </summary>
        /// <param name="sender">触发事件的路由器</param>
        /// <param name="e">参数包装</param>
        public void RaiseProcessedEvent(IRouteable sender, EventArgs e)
        {
            this.OnProcessed?.Invoke(sender, e);
        }

        /// <summary>
        /// 获取或设置消息的路由类型
        /// </summary>
        public YuriRoutedType Type { get; set; }

        /// <summary>
        /// 获取或设置消息的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置消息的唯一标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置消息的附加值
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 获取或设置消息发出路由器的名字
        /// </summary>
        public string FromRouter { get; set; }

        /// <summary>
        /// 获取或设置消息最终目的地路由器的名字
        /// </summary>
        public string ToRouter { get; set; }

        /// <summary>
        /// 获取或设置上一路由器的名字
        /// </summary>
        public string LastRouter { get; set; }

        /// <summary>
        /// 获取或设置消息是否已经被处理
        /// </summary>
        public bool IsHandled { get; set; } = false;

        /// <summary>
        /// 获取或设置消息是否已经被取消
        /// </summary>
        public bool IsCanceled { get; set; } = false;

        /// <summary>
        /// 获取消息创建的时间戳
        /// </summary>
        public DateTime CreateTimestamp { get; } = DateTime.Now;
    }
}
