using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// <para>符号表类：维护运行时环境的用户符号</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    [Serializable]
    internal sealed class SymbolTable : ForkableState
    {
        /// <summary>
        /// 重置符号表
        /// </summary>
        /// <param name="clearSystemVar">是否清空系统变量</param>
        public void Reset(bool clearSystemVar = false)
        {
            this.userSymbolTableContainer.Clear();
            this.globalSymbolTable.Clear();
            this.InitSystemVars();
        }

        /// <summary>
        /// <para>使用一个变量作为右值</para>
        /// <para>如果这个变量从未使用过，将抛出错误</para>
        /// </summary>
        /// <param name="sceneObject">场景实例/param>
        /// <param name="varName">变量名</param>
        /// <returns>变量在运行时环境的引用</returns>
        public object Fetch(Scene sceneObject, string varName)
        {
            var table = this.FindSymbolTable(sceneObject.Scenario);
            // 如果查无此键
            if (table.ContainsKey(varName) == false)
            {
                CommonUtils.ConsoleLine("变量 " + varName + " 在作为左值之前被引用", "SymbolTable", OutputStyle.Error);
                throw new NullReferenceException("变量 " + varName + " 在作为左值之前被引用");
            }
            return table[varName];
        }

        /// <summary>
        /// 将一个变量赋值，如果变量不存在，将被注册后再赋值
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="varName">变量名称</param>
        /// <param name="value">变量的值</param>
        public void Assign(string sceneName, string varName, object value)
        {
            var table = this.FindSymbolTable(sceneName);
            table[varName] = value;
        }

        /// <summary>
        /// <para>使用一个全局变量作为右值</para>
        /// <para>如果这个变量从未使用过，将抛出错误</para>
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量在运行时环境的引用</returns>
        public object GlobalFetch(string varName)
        {
            // 处理开关操作
            if (SymbolTable.IsSwitchExpression(varName))
            {
                string[] swiItem = varName.Split(new char[] { '{', '}' });
                int fetchId = Convert.ToInt32(swiItem[1]);
                if (fetchId >= 0 && fetchId < GlobalConfigContext.GAME_SWITCH_COUNT)
                {
                    return this.globalSwitchList[fetchId] == true ? 1.0 : 0.0;
                }
                else
                {
                    CommonUtils.ConsoleLine(String.Format("Invalid Switch id: {0}, TRUE will be returned instead", fetchId),
                        "SymbolManager", OutputStyle.Error);
                    return 1.0;
                }
            }
            // 如果查无此键
            if (this.globalSymbolTable.ContainsKey(varName) == false)
            {
                CommonUtils.ConsoleLine(String.Format("Invalid Variable Fetch: {0}, which haven't been left-value yet", varName),
                        "SymbolManager", OutputStyle.Error);
                throw new Exception("变量 " + varName + " 在作为左值之前被引用");
            }
            return this.globalSymbolTable[varName];
        }

        /// <summary>
        /// 将一个全局变量赋值，如果变量不存在，将被注册后再赋值
        /// </summary>
        /// <param name="varName">变量名称</param>
        /// <param name="value">变量的值</param>
        public void GlobalAssign(string varName, object value)
        {
            // 处理开关操作
            if (SymbolTable.IsSwitchExpression(varName))
            {
                string[] swiItem = varName.Split(new char[] { '{', '}' });
                int fetchId = Convert.ToInt32(swiItem[1]);
                if (fetchId >= 0 && fetchId < GlobalConfigContext.GAME_SWITCH_COUNT)
                {
                    if (value == null)
                    {
                        this.globalSwitchList[fetchId] = false;
                    }
                    else if (value is string)
                    {
                        this.globalSwitchList[fetchId] = (string)value == String.Empty;
                    }
                    else
                    {
                        this.globalSwitchList[fetchId] = (double)value != 0;
                    }
                    return;
                }
                else
                {
                    CommonUtils.ConsoleLine(String.Format("Invalid Switch id: {0}", fetchId), "SymbolManager", OutputStyle.Error);
                    return;
                }
            }
            // 处理全局符号
            this.globalSymbolTable[varName] = value;
        }

        /// <summary>
        /// 获取一个开关的状态，如果该开关从未被使用过，将返回True
        /// </summary>
        /// <param name="id">开关id</param>
        /// <returns>返回开关的状态</returns>
        public bool SwitchFetch(int id)
        {
            if (id >= 0 && id < GlobalConfigContext.GAME_SWITCH_COUNT)
            {
                return this.globalSwitchList[id];
            }
            CommonUtils.ConsoleLine(String.Format("Invalid Switch Fetch id: {0}, TRUE will be returned instead", id), "SymbolManager", OutputStyle.Error);
            return true;
        }

        /// <summary>
        /// 设置一个开关的状态
        /// </summary>
        /// <param name="id">开关id</param>
        /// <param name="state">目标状态</param>
        public void SwitchAssign(int id, bool state)
        {
            if (id >= 0 && id < GlobalConfigContext.GAME_SWITCH_COUNT)
            {
                this.globalSwitchList[id] = state;
            }
            else
            {
                CommonUtils.ConsoleLine(String.Format("Invalid Switch id: {0}", id), "SymbolManager", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 为场景添加符号表
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>操作成功与否</returns>
        public bool AddSymbolTable(Scene scene)
        {
            if (this.userSymbolTableContainer.ContainsKey(scene.Scenario))
            {
                return false;
            }
            this.userSymbolTableContainer.Add(scene.Scenario, new Dictionary<string, object>());
            return true;
        }

        /// <summary>
        /// 测试一个符号是否为开关表达式
        /// </summary>
        /// <param name="parStr">测试字符串</param>
        /// <returns>是否是系统开关操作</returns>
        public static bool IsSwitchExpression(string parStr)
        {
            return SymbolTable.switchRegex.IsMatch(parStr);
        }

        /// <summary>
        /// 为符号表重新设置唯一实例的引用
        /// </summary>
        /// <param name="st">新符号表</param>
        public static void ResetSynObject(SymbolTable st)
        {
            SymbolTable.synObject = st;
        }

        /// <summary>
        /// 寻找当前场景作用域绑定的符号表
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns>符号表</returns>
        private Dictionary<string, object> FindSymbolTable(string sceneName)
        {
            return this.userSymbolTableContainer.ContainsKey(sceneName)
                ? this.userSymbolTableContainer[sceneName]
                : null;
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>符号表管理器</returns>
        public static SymbolTable GetInstance()
        {
            return SymbolTable.synObject ?? (SymbolTable.synObject = new SymbolTable());
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
            this.globalSwitchList = new List<bool>();
            this.globalSymbolTable = new Dictionary<string, object>();
            this.userSymbolTableContainer = new Dictionary<string, Dictionary<string, object>>();
            for (int i = 0; i < GlobalConfigContext.GAME_SWITCH_COUNT; i++)
            {
                this.globalSwitchList.Add(true);
            }
            this.InitSystemVars();
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static SymbolTable synObject = null;
        
        /// <summary>
        /// 局部符号字典（K：场景名称 - V：符号表对象）
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> userSymbolTableContainer = null;

        /// <summary>
        /// 全局符号字典
        /// </summary>
        private Dictionary<string, object> globalSymbolTable = null;

        /// <summary>
        /// 全局开关容器
        /// </summary>
        private List<bool> globalSwitchList = null;

        /// <summary>
        /// 开关正则式子
        /// </summary>
        private static readonly Regex switchRegex = new Regex(@"^switches{\d+}$");
    }
}