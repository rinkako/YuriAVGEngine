using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore
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
        /// <para>使用一个变量作为右值</para>
        /// <para>如果这个变量从未使用过，将抛出错误</para>
        /// </summary>
        /// <param name="sceneObject">场景实例/param>
        /// <param name="varName">变量名</param>
        internal object signal(object sceneObject, string varName)
        {
            Dictionary<string, object> table = this.FindSymbolTable(sceneObject);
            // 如果查无此键
            if (table.ContainsKey(varName) == false)
            {
                throw new Exception("变量 " + varName + " 在作为左值之前被引用");
            }
            return table[varName];
        }

        /// <summary>
        /// 将一个变量赋值，如果变量不存在，将被注册后再赋值
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名称</param>
        /// <param name="value">变量的值</param>
        internal void assign(string sceneName, string varName, object value)
        {
            Scene sc = ResourceManager.GetInstance().GetScene(sceneName);
            Dictionary<string, object> table = this.FindSymbolTable(sc);
            // 如果查无此键就注册
            if (table.ContainsKey(varName) == false)
            {
                table.Add(varName, null);
            }
            // 为变量赋值
            table[varName] = value;
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
        /// <param name="sceneObject">场景实例</param>
        /// <returns>符号表</returns>
        internal Dictionary<string, object> FindSymbolTable(object sceneObject)
        {
            if (this.userSymbolTableContainer.ContainsKey(sceneObject) == false)
            {
                return null;
            }
            else
            {
                return this.userSymbolTableContainer[sceneObject];
            }
        }

        /// <summary>
        /// 为函数调用新建符号表
        /// </summary>
        /// <param name="sf">函数实例</param>
        /// <returns>符号表实例</returns>
        internal Dictionary<string, object> CallFunctionSymbolTable(SceneFunction sf)
        {
            this.userSymbolTableContainer.Add(sf, new Dictionary<string, object>());
            sf.symbolsRef = this.userSymbolTableContainer[sf];
            return sf.symbolsRef;
        }

        /// <summary>
        /// 移除函数调用的符号表引用
        /// </summary>
        /// <param name="sf">函数实例</param>
        /// <returns>操作成功与否</returns>
        internal bool RemoveCallFunctionSymbolTable(SceneFunction sf)
        {
            if (this.userSymbolTableContainer.ContainsKey(sf))
            {
                sf.symbolsRef = null;
                return this.userSymbolTableContainer.Remove(sf);
            }
            return false;
        }

        /// <summary>
        /// 为场景添加符号表
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>操作成功与否</returns>
        internal bool AddSymbolTable(Scene scene)
        {
            if (this.userSymbolTableContainer.ContainsKey(scene))
            {
                return false;
            }
            this.userSymbolTableContainer.Add(scene, new Dictionary<string, object>());
            return true;
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
            this.userSymbolTableContainer = new Dictionary<object, Dictionary<string, object>>();
            this.InitSystemVars();
        }

        // 唯一实例
        private static SymbolTable synObject = null;
        // 用户符号字典（K：场景或函数实例 - V：符号表对象）
        private Dictionary<object, Dictionary<string, object>> userSymbolTableContainer = null;
        // 系统符号字典
        private Dictionary<string, object> systemSymbolTable = null;
    }
}