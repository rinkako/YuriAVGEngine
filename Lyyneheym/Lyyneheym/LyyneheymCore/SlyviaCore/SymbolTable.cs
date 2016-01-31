using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>符号表类：维护运行时环境的用户符号</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    [Serializable]
    public sealed class SymbolTable
    {
        /// <summary>
        /// 重置符号表
        /// </summary>
        /// <param name="clearSystemVar">是否清空系统变量</param>
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
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名</param>
        /// <returns>该变量的真实引用</returns>
        internal object signal(string sceneName, string varName, bool isLvalue)
        {
            Dictionary<string, object> table = this.FindSymbolTable(sceneName);
            // 如果查无此键
            if (table.ContainsKey(varName) == false)
            {
                // 不是作为左值就报错就注册
                if (isLvalue == false)
                {
                    throw new Exception("变量 " + varName + " 在作为左值之前被引用");
                }
                // 否则注册这个变量
                else
                {
                    table.Add(varName, null);
                }
            }
            return table[varName];
        }

        /// <summary>
        /// 向符号表注册一个用户变量
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名</param>
        /// <param name="value">变量的值</param>
        /// <returns>操作成功与否</returns>
        internal bool sign(string sceneName, string varName, object value)
        {
            Dictionary<string, object> table = this.FindSymbolTable(sceneName);
            // 如果被注册过了就返回失败
            if (table.ContainsKey(varName))
            {
                return false;
            }
            // 否则注册这个变量
            table.Add(varName, value);
            return true;
        }
        
        /// <summary>
        /// 向符号表撤销一个用户变量
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名</param>
        /// <returns>操作成功与否</returns>
        internal bool unsign(string sceneName, string varName)
        {
            Dictionary<string, object> table = this.FindSymbolTable(sceneName);
            // 如果没有这个变量就返回失败
            if (!table.ContainsKey(varName))
            {
                return false;
            }
            // 否则消除她
            table.Remove(varName);
            return true;
        }

        /// <summary>
        /// 寻找当前场景作用域绑定的符号表
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>符号表</returns>
        internal Dictionary<string, object> FindSymbolTable(string sceneName)
        {
            if (this.userSymbolTableContainer.ContainsKey(sceneName) == false)
            {
                return null;
            }
            else
            {
                return this.userSymbolTableContainer[sceneName];
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
        // 用户符号字典（K：场景或函数名称 - V：符号表对象）
        private Dictionary<string, Dictionary<string, object>> userSymbolTableContainer = null;
        // 系统符号字典
        private Dictionary<string, object> systemSymbolTable = null;
    }
}