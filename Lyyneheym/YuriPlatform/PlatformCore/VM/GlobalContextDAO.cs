using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.Utils;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 全局变量上下文DAO：为游戏的全局变量上下文提供包装
    /// </summary>
    [Serializable]
    internal sealed class GlobalContextDAO : ForkableState
    {
        /// <summary>
        /// 构造并初始化一个全局变量上下文
        /// </summary>
        public GlobalContextDAO()
        {
            for (int i = 0; i < GlobalConfigContext.GAME_SWITCH_COUNT; i++)
            {
                this.GlobalSwitchList.Assign(i.ToString(), false);
            }
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
                var swiItem = varName.Split('{', '}');
                int fetchId = Convert.ToInt32(swiItem[1]);
                if (fetchId >= 0 && fetchId < GlobalConfigContext.GAME_SWITCH_COUNT)
                {
                    return (bool)this.GlobalSwitchList.Fetch(fetchId.ToString()) == true ? 1.0 : 0.0;
                }
                LogUtils.LogLine(String.Format("Invalid Switch id: {0}, TRUE will be returned instead", fetchId),
                    "SymbolManager", LogLevel.Error);
                return 1.0;
            }
            // 如果查无此键
            if (this.GlobalSymbolTable.Exist(varName) == false)
            {
                LogUtils.LogLine(String.Format("Invalid Variable Fetch: {0}, which haven't been left-value yet", varName),
                        "SymbolManager", LogLevel.Error);
                throw new Exception("变量 " + varName + " 在作为左值之前被引用");
            }
            return this.GlobalSymbolTable.Fetch(varName);
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
                var swiItem = varName.Split('{', '}');
                int fetchId = Convert.ToInt32(swiItem[1]);
                if (fetchId >= 0 && fetchId < GlobalConfigContext.GAME_SWITCH_COUNT)
                {
                    if (value == null)
                    {
                        this.GlobalSwitchList.Assign(fetchId.ToString(), false);
                    }
                    else if (value is string)
                    {
                        this.GlobalSwitchList.Assign(fetchId.ToString(), (string)value == String.Empty);
                    }
                    else
                    {
                        this.GlobalSwitchList.Assign(fetchId.ToString(), Math.Abs((double)value) > 1e-7);
                    }
                    return;
                }
                else
                {
                    LogUtils.LogLine(String.Format("Invalid Switch id: {0}", fetchId), "SymbolManager", LogLevel.Error);
                    return;
                }
            }
            // 处理全局符号
            this.GlobalSymbolTable.Assign(varName, value);
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
                return (bool)this.GlobalSwitchList.Fetch(id.ToString());
            }
            LogUtils.LogLine(String.Format("Invalid Switch Fetch id: {0}, TRUE will be returned instead", id), "SymbolManager", LogLevel.Error);
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
                this.GlobalSwitchList.Assign(id.ToString(), state);
            }
            else
            {
                LogUtils.LogLine(String.Format("Invalid Switch id: {0}", id), "SymbolManager", LogLevel.Error);
            }
        }

        /// <summary>
        /// 全局符号上下文
        /// </summary>
        public readonly SaveableContext GlobalSymbolTable = new SaveableContext("&__YuriGlobal");

        /// <summary>
        /// 全局开关上下文
        /// </summary>
        public readonly SaveableContext GlobalSwitchList = new SaveableContext("&__YuriSwitches");

    }
}
