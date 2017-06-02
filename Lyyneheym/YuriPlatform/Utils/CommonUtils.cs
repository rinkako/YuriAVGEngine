using System;
using System.Collections.Generic;

namespace Yuri.Utils
{
    /// <summary>
    /// 开发辅助相关的静态方法集
    /// </summary>
    internal sealed class CommonUtils
    {
        /// <summary>
        /// 交换两个对象的引用
        /// </summary>
        /// <typeparam name="T">T是要交换的类型</typeparam>
        /// <param name="a">交换变量</param>
        /// <param name="b">交换变量</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T swaper = a;
            a = b;
            b = swaper;
        }

        /// <summary>
        /// 交换两个对象在容器中的引用
        /// </summary>
        /// <typeparam name="T">T是要交换的类型</typeparam>
        /// <param name="container">交换容器</param>
        /// <param name="aId">交换变量下标</param>
        /// <param name="bId">交换变量下标</param>
        public static void Swap<T>(List<T> container, int aId, int bId)
        {
            var exchange = container[aId];
            container[aId] = container[bId];
            container[bId] = exchange;
        }
    }
}
