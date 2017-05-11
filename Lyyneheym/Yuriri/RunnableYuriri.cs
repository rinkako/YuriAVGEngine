using System;
using System.Collections.Generic;

namespace Yuri.Yuriri
{
    /// <summary>
    /// 可执行的Yuriri代码块包装
    /// </summary>
    [Serializable]
    public abstract class RunnableYuriri
    {
        /// <summary>
        /// 构造序列
        /// </summary>
        public SceneAction Ctor { get; set; }

        /// <summary>
        /// 标签字典
        /// </summary>
        public Dictionary<string, SceneAction> LabelDictionary { get; set; }
    }
}
