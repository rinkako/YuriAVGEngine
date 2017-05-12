using System;

namespace Yuri.Hemerocallis.Entity
{
    /// <summary>
    /// 里程碑实体类
    /// </summary>
    [Serializable]
    internal sealed class HMilestone
    {
        /// <summary>
        /// 获取或设置里程碑对象的唯一标示ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置要达到的目标总字数
        /// </summary>
        public long Destination { get; set; }

        /// <summary>
        /// 获取或设置该里程碑是否已经完成
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        /// 获取或设置该里程碑的详情
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 获取或设置该里程碑开始时刻
        /// </summary>
        public DateTime BeginTimeStamp { get; set; }

        /// <summary>
        /// 获取或设置该里程碑结束时刻
        /// </summary>
        public DateTime EndTimeStamp { get; set; }
    }
}
