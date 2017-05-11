using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.Yuriri
{
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
