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
        /// 获取或设置里程碑类型
        /// </summary>
        public MilestoneType Type { get; set; }

        /// <summary>
        /// 获取或设置要达到的目标总字数
        /// </summary>
        public long Destination { get; set; }

        /// <summary>
        /// 获取或设置该里程碑是否已经完成
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        /// 获取或设置该里程碑是否已经通知过
        /// </summary>
        public bool IsNotified { get; set; }

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

        /// <summary>
        /// 获取或设置该里程碑完成时刻
        /// </summary>
        public DateTime FinishTimeStamp { get; set; }
    }

    /// <summary>
    /// 枚举：里程碑的类型
    /// </summary>
    [Serializable]
    internal enum MilestoneType
    {
        /// <summary>
        /// 书籍里程碑
        /// </summary>
        Book,
        /// <summary>
        /// 文章里程碑
        /// </summary>
        Aritical
    }
}
