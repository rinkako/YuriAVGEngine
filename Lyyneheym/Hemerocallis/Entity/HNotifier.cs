using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 提醒事项实体类
    /// </summary>
    [Serializable]
    internal sealed class HNotifier
    {
        /// <summary>
        /// 获取或设置提醒事项的唯一标示ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置提醒事项的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取或设置提醒事项的详情
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 获取或设置从该时间戳开始的第一次启动程序后推送该提醒
        /// </summary>
        public DateTime NotifyTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置是否已经通知过
        /// </summary>
        public bool IsNotified { get; set; }
    }
}
