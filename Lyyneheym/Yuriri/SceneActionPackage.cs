using System;
using System.Collections.Generic;

namespace Yuri.Yuriri
{
    /// <summary>
    /// 场景动作临时包装类：为运行时环境重新组织场景动作间关系提供临时容器
    /// </summary>
    [Serializable]
    public class SceneActionPackage
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string saNodeName = null;

        /// <summary>
        /// 节点动作
        /// </summary>
        public SActionType aType = SActionType.NOP;

        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> argsDict = new Dictionary<string, string>();

        /// <summary>
        /// 条件从句逆波兰表达
        /// </summary>
        public string condPolish = null;

        /// <summary>
        /// 下一节点
        /// </summary>
        public string next = null;

        /// <summary>
        /// 下一真节点向量
        /// </summary>
        public List<string> trueRouting = null;

        /// <summary>
        /// 下一假节点向量
        /// </summary>
        public List<string> falseRouting = null;

        /// <summary>
        /// 是否依存函数
        /// </summary>
        public bool isBelongFunc = false;

        /// <summary>
        /// 依存函数名
        /// </summary>
        public string funcName = null;

        /// <summary>
        /// 附加值
        /// </summary>
        public string aTag = null;

        /// <summary>
        /// 脏位
        /// </summary>
        public bool dirtyBit = false;

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString()
        {
            return this.saNodeName;
        }
    }
}
