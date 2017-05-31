using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 接口：运行时上下文
    /// </summary>
    internal interface IRuntimeContext
    {
        /// <summary>
        /// 在此上下文中申请一个变量来储存指定对象的引用，如果指定变量名已存在，将覆盖原有的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="varObj">要存入的对象引用</param>
        void Assign(string varName, object varObj);

        /// <summary>
        /// 从此上下文移除一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>是否移除成功（变量原本是否存在）</returns>
        bool Remove(string varName);

        /// <summary>
        /// 查找此上下文中是否存在某个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量是否存在</returns>
        bool Exist(string varName);

        /// <summary>
        /// 从此上下文中取一个变量名对应的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量代表的对象引用</returns>
        object Fetch(string varName);

        /// <summary>
        /// 清空此上下文
        /// </summary>
        void Clear();

        /// <summary>
        /// 使用指定的筛选谓词查找此上下文符合条件的变量名
        /// </summary>
        /// <param name="varNamePred">变量名筛选谓词</param>
        /// <returns>满足约束的键值对</returns>
        List<KeyValuePair<string, object>> GetSymbols(Predicate<string> varNamePred);
    }
}
