using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>运行时管理器：维护运行时的所有信息</para>
    /// <para>她是一个单例类，只有唯一实例，并且可以序列化</para>
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    public class RuntimeManager
    {
        public SceneAction MoveNext()
        {
            // 调用栈已经为空时预备退出
            if (this.CallStack.Count() == 0)
            {
                return null;
            }
            // 取出当前要执行的指令
            SceneAction ret = this.CallStack.ESP.IP;
            // 如果没有下一指令就弹栈
            if (ret == null)
            {
                this.CallStack.Consume();
                // 递归寻指
                return this.MoveNext();
            }
            // 如果没有条件子句
            if (ret.condPolish == "")
            {
                // 处理控制流程
                switch (ret.aType)
                {
                    case SActionType.NOP:
                        // 优先进入trueRouting
                        if (ret.trueRouting != null && ret.trueRouting.Count > 0)
                        {
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.trueRouting[0];
                            break;
                        }
                        // falseRouting
                        else if (ret.falseRouting != null && ret.falseRouting.Count > 0)
                        {
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.falseRouting[0];
                            break;
                        }
                        // next
                        else
                        {
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.next;
                            break;
                        }
                    case SActionType.act_endfor:
                        // endfor直接跳过
                        this.CallStack.ESP.PC++;
                        ret = this.CallStack.ESP.IP = ret.next;
                        break;
                }
                // 移动下一指令指针，为下次处理做准备
                this.CallStack.ESP.PC++;
                this.CallStack.ESP.IP = ret.next;
                // 返回当前要执行的指令实例
                return ret;
            }
            // 条件子句不为空时
            else
            {
                // 计算条件真值
                bool condBoolean = this.CalculateBooleanPolish(ret.condPolish);
                // 处理控制流程
                switch (ret.aType)
                {
                    // IF语句
                    case SActionType.act_if:
                        // 条件为真且有真分支
                        if (condBoolean == true && ret.trueRouting != null && ret.trueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.trueRouting[0];
                        }
                        // 条件为假且有假分支
                        else if (condBoolean == false && ret.falseRouting != null && ret.falseRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入falseRouting
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.falseRouting[0];
                        }
                        // 没有执行的语句时，移动指令指针到next节点
                        else
                        {
                            // 跳过if语句
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.next;
                        }
                        // 再移动一次指针，为下次处理做准备
                        this.CallStack.ESP.PC++;
                        this.CallStack.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                    // FOR语句
                    case SActionType.act_for:
                        // 如果条件为真就进入真分支
                        if (condBoolean == true && ret.trueRouting != null && ret.trueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.trueRouting[0];
                        }
                        // 如果条件为假直接跳过for语句
                        else
                        {
                            // 跳过if语句
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.next;
                        }
                        // 再移动一次指针，为下次处理做准备
                        this.CallStack.ESP.PC++;
                        this.CallStack.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                    // 除此以外，带了cond的语句，为真才执行
                    default:
                        if (condBoolean == false)
                        {
                            // 跳过当前语句
                            this.CallStack.ESP.PC++;
                            ret = this.CallStack.ESP.IP = ret.next;
                        }
                        // 移动下一指令指针，为下次处理做准备
                        this.CallStack.ESP.PC++;
                        this.CallStack.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                }
            }
        }

        public bool CalculateBooleanPolish(string polish)
        {
            return true;
        }

        /// <summary>
        /// 获取游戏调用堆栈
        /// </summary>
        public StackMachine CallStack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取游戏符号表
        /// </summary>
        public SymbolTable Symbols
        {
            get;
            private set;
        }

        /// <summary>
        /// 将运行时环境恢复最初状态
        /// </summary>
        public void Reset()
        {
            this.CallStack = new StackMachine();
            this.Symbols = SymbolTable.getInstance();
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>运行时管理器</returns>
        public static RuntimeManager getInstance()
        {
            return null == synObject ? synObject = new RuntimeManager() : synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private RuntimeManager()
        {
            this.Reset();
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static RuntimeManager synObject = null;
    }

    /// <summary>
    /// 游戏的总体状态
    /// </summary>
    public enum GameState
    {
        // 游戏剧情进行时
        Performing,
        // 用户操作界面
        UserPanel,
        // 系统执行中
        Loading
    }

    /// <summary>
    /// 游戏的稳态
    /// </summary>
    public enum GameStableState
    {
        // 等待用户操作的稳态
        Stable,
        // 用户等待系统的不稳态
        Unstable,
        // 当前状态不明确
        Unknown
    }

}
