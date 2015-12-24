using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyyneheymCore.SlyviaCore;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// <para>符号表类：维护编译过程和运行时环境的用户符号</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public sealed class SymbolTable
    {
        /// <summary>
        /// 重置符号表
        /// </summary>
        /// <param name="clearSystemVar">是否清楚系统变量</param>
        public void Reset(bool clearSystemVar = false)
        {
            this.userSymbolTableContainer.Clear();
            this.systemSymbolTable.Clear();
            this.InitSystemVars();
        }

        /// <summary>
        /// <para>使用一个变量</para>
        /// <para>如果这个变量从未使用过，将被注册；否则，返回她在运行时环境的引用</para>
        /// </summary>
        /// <param name="SALName">动作序列名称</param>
        /// <param name="name">变量名</param>
        /// <returns>该变量的真实引用</returns>
        internal object signal(string SALName, string name)
        {
            Dictionary<string, object> table = this.FindSymbolTable(SALName);
            // 如果查无此键就注册
            if (!table.ContainsKey(name))
            {
                table.Add(name, null);
            }
            return table[name];
        }

        /// <summary>
        /// 向符号表注册一个用户变量
        /// </summary>
        /// <param name="SALName">当前动作序列名字</param>
        /// <param name="name">变量名</param>
        /// <param name="value">变量的值</param>
        /// <returns>操作成功与否</returns>
        internal bool sign(string SALName, string name, object value)
        {
            Dictionary<string, object> table = this.FindSymbolTable(SALName);
            // 如果被注册过了就返回失败
            if (table.ContainsKey(name))
            {
                return false;
            }
            // 否则注册这个变量
            table.Add(name, value);
            return true;
        }
        
        /// <summary>
        /// 向符号表撤销一个用户变量
        /// </summary>
        /// <param name="SALName">当前动作序列名字</param>
        /// <param name="name">变量名</param>
        /// <returns>操作成功与否</returns>
        internal bool unsign(string SALName, string name)
        {
            Dictionary<string, object> table = this.FindSymbolTable(SALName);
            // 如果没有这个变量就返回失败
            if (!table.ContainsKey(name))
            {
                return false;
            }
            // 否则消除她
            table.Remove(name);
            return true;
        }

        /// <summary>
        /// 寻找当前动作序列作用域绑定的符号表
        /// </summary>
        /// <param name="currentSAL">当前动作序列的名称</param>
        /// <returns>符号表</returns>
        internal Dictionary<string, object> FindSymbolTable(string SALName)
        {
            if (this.userSymbolTableContainer.ContainsKey(SALName) == false)
            {
                return null;
            }
            else
            {
                return this.userSymbolTableContainer[SALName];
            }

        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>符号表管理器</returns>
        public static SymbolTable getInstance()
        {
            return synObject == null ? synObject = new SymbolTable() : synObject;
        }

        /// <summary>
        /// 初始化系统变量
        /// </summary>
        private void InitSystemVars()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private SymbolTable()
        {
            this.systemSymbolTable = new Dictionary<string, object>();
            this.userSymbolTableContainer = new Dictionary<string, Dictionary<string, object>>();
            this.InitSystemVars();
        }

        // 唯一实例
        private static SymbolTable synObject = null;
        // 用户符号字典（K：动作序列名 - V：符号表对象）
        private Dictionary<string, Dictionary<string, object>> userSymbolTableContainer = null;
        // 系统符号字典
        private Dictionary<string, object> systemSymbolTable = null;
    }
}