using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyyneheymCore.SlyviaCore;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// <para>符号表类：维护编译过程的用户符号</para>
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
        }

        /// <summary>
        /// <para>使用一个变量</para>
        /// <para>如果这个变量从未使用过，将被注册；否则，返回她在运行时环境的引用</para>
        /// </summary>
        /// <param name="stn">语法块在树上的节点</param>
        /// <param name="name">变量名</param>
        /// <returns>该变量的真实引用</returns>
        internal object signal(SyntaxTreeNode stn, string name)
        {
            Dictionary<string, object> table = this.FindSymbolTable(stn);
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
        /// <param name="stn">当前节点</param>
        /// <param name="name">变量名</param>
        /// <param name="value">变量的值</param>
        /// <returns>操作成功与否</returns>
        internal bool sign(SyntaxTreeNode stn, string name, object value)
        {
            Dictionary<string, object> table = this.FindSymbolTable(stn);
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
        /// <param name="stn">当前节点</param>
        /// <param name="name">变量名</param>
        /// <returns>操作成功与否</returns>
        internal bool unsign(SyntaxTreeNode stn, string name)
        {
            Dictionary<string, object> table = this.FindSymbolTable(stn);
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
        /// 寻找当前节点作用域绑定的符号表
        /// </summary>
        /// <param name="currentNode">当前节点</param>
        /// <returns>符号表</returns>
        internal Dictionary<string, object> FindSymbolTable(SyntaxTreeNode currentNode)
        {
            SyntaxTreeNode iterNode = currentNode.parent;
            if (iterNode == null)
            {
                return null;
            }
            // 找不是自己但距离自己最近的符号表
            while (iterNode != null && iterNode.symbols == null)
            {
                iterNode = iterNode.parent;
            }
            if (iterNode != null)
            {
                return iterNode.symbols;
            }
            return null;
        }

        /// <summary>
        /// 追加一张绑定在指定节点作用域上的符号表，如果已经存在将直接返回
        /// </summary>
        /// <param name="node">节点引用</param>
        /// <returns>符号表的引用</returns>
        internal Dictionary<string, object> AddTable(SyntaxTreeNode node)
        {
            return this.userSymbolTableContainer.ContainsKey(node) ? this.userSymbolTableContainer[node]
                : this.userSymbolTableContainer[node] = node.symbols = new Dictionary<string, object>();
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
        /// 构造函数
        /// </summary>
        private SymbolTable()
        {
            this.systemSymbolTable = new Dictionary<string, object>();
            this.userSymbolTableContainer = new Dictionary<SyntaxTreeNode, Dictionary<string, object>>();
        }

        // 唯一实例
        private static SymbolTable synObject = null;
        // 用户符号字典
        private Dictionary<SyntaxTreeNode, Dictionary<string, object>> userSymbolTableContainer = null;
        // 系统符号字典
        private Dictionary<string, object> systemSymbolTable = null;
    }
}
