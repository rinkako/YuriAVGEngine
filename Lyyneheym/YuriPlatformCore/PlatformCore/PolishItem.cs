using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 运算栈项目
    /// </summary>
    internal sealed class PolishItem
    {
        /// <summary>
        /// 获取或设置Polish项的类型
        /// </summary>
        public PolishItemType itemType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置Polish项的数值
        /// </summary>
        public double number
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置Polish项的字符串
        /// </summary>
        public string cluster
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置Polish项在运行时环境的变量引用
        /// </summary>
        public object reference
        {
            get;
            set;
        }

        /// <summary>
        /// 获取两个polish计算项是否可以进行操作
        /// </summary>
        /// <param name="p1">操作数1</param>
        /// <param name="p2">操作数2</param>
        /// <returns>是否可以作为操作符的双目</returns>
        public static bool isOperatable(PolishItem p1, PolishItem p2)
        {
            if (p1.reference != null && p2.reference != null && p1.reference.GetType() == p2.reference.GetType()) { return true; }
            if (p1.itemType == PolishItemType.CONSTANT && p2.itemType == PolishItemType.CONSTANT)     { return true; }
            if (p1.itemType == PolishItemType.CONSTANT && p2.itemType == PolishItemType.VAR_NUM)      { return true; }
            if (p1.itemType == PolishItemType.VAR_NUM && p2.itemType == PolishItemType.VAR_NUM)       { return true; }
            if (p1.itemType == PolishItemType.VAR_NUM && p2.itemType == PolishItemType.CONSTANT)      { return true; }
            if (p1.itemType == PolishItemType.STRING && p2.itemType == PolishItemType.STRING)         { return true; }
            if (p1.itemType == PolishItemType.STRING && p2.itemType == PolishItemType.VAR_STRING)     { return true; }
            if (p1.itemType == PolishItemType.VAR_STRING && p2.itemType == PolishItemType.VAR_STRING) { return true; }
            if (p1.itemType == PolishItemType.VAR_STRING && p2.itemType == PolishItemType.STRING)     { return true; }
            return false;
        }
    }

    /// <summary>
    /// 枚举：逆波兰式项类型
    /// </summary>
    internal enum PolishItemType
    {
        NONE,
        CONSTANT,
        STRING,
        VAR,
        VAR_NUM,
        VAR_STRING,
        CAL_PLUS,
        CAL_MINUS,
        CAL_MULTI,
        CAL_DIV,
        CAL_ANDAND,
        CAL_OROR,
        CAL_NOT,
        CAL_EQUAL,
        CAL_NOTEQUAL,
        CAL_BIG,
        CAL_SMALL,
        CAL_BIGEQUAL,
        CAL_SMALLEQUAL
    }
}
