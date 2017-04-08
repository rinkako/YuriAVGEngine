using System;
using System.Collections.Generic;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 提供系统回滚的快照类
    /// </summary>
    internal sealed class RollbackableSnapshot
    {
        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该快照的唯一标识符</returns>
        public override string ToString()
        {
            return this.VMRef.StackName;
        }

        /// <summary>
        /// 调用堆栈的拷贝
        /// </summary>
        public StackMachine VMRef
        {
            get;
            set;
        }

        /// <summary>
        /// 屏幕状态的拷贝
        /// </summary>
        public ScreenManager ScreenStateRef
        {
            get;
            set;
        }

        /// <summary>
        /// 符号表的拷贝
        /// </summary>
        public SymbolTable SymbolRef
        {
            get;
            set;
        }

        /// <summary>
        /// 重现动作的拷贝
        /// </summary>
        public SceneAction ReactionRef
        {
            get;
            set;
        }

        /// <summary>
        /// 并行栈的拷贝
        /// </summary>
        public Stack<Dictionary<string, bool>> ParallelStateStackRef
        {
            get;
            set;
        }

        /// <summary>
        /// 音乐状态的拷贝
        /// </summary>
        public string MusicRef
        {
            get;
            set;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp
        {
            get;
            set;
        }
    }
}
