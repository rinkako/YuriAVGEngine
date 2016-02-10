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
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    public class RuntimeManager
    {
        /// <summary>
        /// 获取当前调用堆栈顶部状态
        /// </summary>
        /// <returns></returns>
        public GameStackMachineState GameState()
        {
            if (this.CallStack.ESP == null)
            {
                return GameStackMachineState.NOP;
            }
            else
            {
                return this.CallStack.ESP.state;
            }
        }

        /// <summary>
        /// 取下一动作指令
        /// </summary>
        /// <returns>动作实例</returns>
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

        /// <summary>
        /// 等待用户操作
        /// </summary>
        /// <param name="causedBy">触发的原因</param>
        public void UserWait(string causedBy)
        {
            this.CallStack.Submit(causedBy);
        }

        /// <summary>
        /// 延时
        /// </summary>
        /// <param name="causedBy">触发的原因</param>
        /// <param name="timespan">等待时间间隔</param>
        public void Delay(string causedBy, TimeSpan timespan)
        {
            this.CallStack.Submit(causedBy, timespan);
        }

        /// <summary>
        /// 立即结束本次调用
        /// </summary>
        public void ExitCall()
        {
            this.CallStack.Consume();
        }

        /// <summary>
        /// 场景调用
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <param name="target">目标标签</param>
        public void CallScene(Scene scene, SceneAction target = null)
        {
            this.CallStack.Submit(scene, target);
        }

        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="function">函数实例</param>
        /// <param name="args">参数列表</param>
        public void CallFunction(SceneFunction function, List<object> args)
        {
            this.CallStack.Submit(function, args);
            // 处理参数传递
            var funcSymbols = this.Symbols.CallFunctionSymbolTable(function);
            for (int i = 0; i < args.Count; i++)
            {
                funcSymbols.Add(function.param[i], args[i]);
            }
        }

        /// <summary>
        /// 左值运算一个变量
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">右值逆波兰式</param>
        public void Assignment(string varname, string valuePolish)
        {
            this.Symbols.assign(this.CallStack.ESP.scriptName, varname, this.CalculatePolish(valuePolish));
        }

        /// <summary>
        /// 取一个变量作右值
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <returns>变量的引用</returns>
        public object Fetch(string varname)
        {
            return this.Symbols.signal(ResourceManager.getInstance().GetScene(this.CallStack.ESP.scriptName), varname);
        }

        /// <summary>
        /// 计算表达式的真值
        /// </summary>
        /// <param name="polish">表达式的逆波兰式</param>
        /// <returns>表达式的真值</returns>
        public bool CalculateBooleanPolish(string polish)
        {
            return Convert.ToBoolean(this.CalculatePolish(polish));
        }

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="polish">表达式的逆波兰式</param>
        /// <returns>计算结果的值（Double/字符串）</returns>
        public object CalculatePolish(string polish)
        {
            List<PolishItem> calcList = this.GetPolishItemList(polish);
            if (calcList.Count == 0)
            {
                return null;
            }
            Stack<PolishItem> calcStack = new Stack<PolishItem>();
            foreach (PolishItem poi in calcList)
            {
                // 操作数压栈
                if (poi.itemType < PolishItemType.CAL_PLUS)
                {
                    calcStack.Push(poi);
                    continue;
                }
                // 下面开始是运算符
                if (poi.itemType == PolishItemType.CAL_NOT && calcStack.Count >= 1)
                {
                    PolishItem peeker = calcStack.Peek();
                    if (peeker.itemType == PolishItemType.CONSTANT || peeker.itemType == PolishItemType.VAR_NUM)
                    {
                        calcStack.Pop();
                        PolishItem np = new PolishItem()
                        {
                            number = Math.Abs(peeker.number) < 1e-15 ? 1 : 0
                        };
                        calcStack.Push(np);
                        continue;
                    }
                    else if (peeker.itemType == PolishItemType.STRING || peeker.itemType == PolishItemType.VAR_STRING)
                    {
                        calcStack.Pop();
                        PolishItem np = new PolishItem()
                        {
                            number = peeker.cluster == "" ? 1 : 0
                        };
                        calcStack.Push(np);
                        continue;
                    }
                }
                if (calcStack.Count >= 2)
                {
                    PolishItem operand2 = calcStack.Pop();
                    PolishItem operand1 = calcStack.Pop();
                    if (PolishItem.isOperatable(operand1, operand2) == true)
                    {
                        PolishItem newPoi = null;
                        double tempDouble = 0;
                        string tempString = "";
                        switch (poi.itemType)
                        {
                            case PolishItemType.CAL_PLUS:
                                if (operand1.reference is string)
                                {
                                    tempString = (string)operand1.reference + (string)operand2.reference;
                                    newPoi = new PolishItem()
                                    {
                                        cluster = tempString,
                                        reference = tempString
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (double)operand1.reference + (double)operand2.reference;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_MINUS:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference - (double)operand2.reference;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：减法");
                                }
                                break;
                            case PolishItemType.CAL_MULTI:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference * (double)operand2.reference;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：乘法");
                                }
                                break;
                            case PolishItemType.CAL_DIV:
                                if (operand1.reference is double)
                                {
                                    if (Math.Abs((double)operand2.reference) < 0)
                                    {
                                        throw new Exception("除零错误");
                                    }
                                    tempDouble = (double)operand1.reference / (double)operand2.reference;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：除法");
                                }
                                break;
                            case PolishItemType.CAL_ANDAND:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.reference) > 0 && Math.Abs((double)operand2.reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：&&");
                                }
                                break;
                            case PolishItemType.CAL_OROR:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.reference) > 0 || Math.Abs((double)operand2.reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：||");
                                }
                                break;
                            case PolishItemType.CAL_EQUAL:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference == (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.reference == (string)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_NOTEQUAL:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference != (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.reference != (string)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIG:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference > (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.reference, (string)operand2.reference) > 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIGEQUAL:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference >= (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.reference, (string)operand2.reference) >= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALL:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference < (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.reference, (string)operand2.reference) < 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALLEQUAL:
                                if (operand1.reference is double)
                                {
                                    tempDouble = (double)operand1.reference <= (double)operand2.reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.reference, (string)operand2.reference) <= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        number = tempDouble,
                                        reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (calcStack.Count != 1)
            {
                throw new Exception("表达式有错误");
            }
            return calcStack.Peek().reference;
        }

        /// <summary>
        /// 将逆波兰式转化为可计算的项
        /// </summary>
        /// <param name="polish">逆波兰式字符串</param>
        /// <returns>可计算项目向量</returns>
        private List<PolishItem> GetPolishItemList(string polish)
        {
            List<PolishItem> resVec = new List<PolishItem>();
            string[] polishItem = polish.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in polishItem)
            {
                PolishItem poi = null;
                // 常数项
                if (item.All((x) => x >= '0' && x <= '9'))
                {
                    double numitem = Convert.ToDouble(item);
                    poi = new PolishItem()
                    {
                        number = numitem,
                        cluster = null,
                        itemType = PolishItemType.CONSTANT,
                        reference = numitem
                    };
                }
                // 字符串
                else if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = item,
                        itemType = PolishItemType.STRING,
                        reference = item
                    };
                }
                // 变量时
                else if ((item.StartsWith("&") || item.StartsWith("$")) && item.Length > 1)
                {
                    string varPureName = item.Substring(1);
                    object varRef = this.Fetch(varPureName);
                    if (varRef is double)
                    {
                        poi = new PolishItem()
                        {
                            number = (double)varRef,
                            cluster = null,
                            itemType = PolishItemType.VAR_NUM,
                            reference = varRef
                        };
                    }
                    else
                    {
                        poi = new PolishItem()
                        {
                            number = 0.0f,
                            cluster = Convert.ToString(varRef),
                            itemType = PolishItemType.VAR_STRING,
                            reference = varRef
                        };
                    }
                }
                else if (item == "+")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_PLUS,
                        reference = null
                    };
                }
                else if (item == "-")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_MINUS,
                        reference = null
                    };
                }
                else if (item == "*")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_MULTI,
                        reference = null
                    };
                }
                else if (item == "/")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_DIV,
                        reference = null
                    };
                }
                else if (item == "!")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_NOT,
                        reference = null
                    };
                }
                else if (item == "&&")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_ANDAND,
                        reference = null
                    };
                }
                else if (item == "||")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_OROR,
                        reference = null
                    };
                }
                else if (item == "<>")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_NOTEQUAL,
                        reference = null
                    };
                }
                else if (item == "==")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_EQUAL,
                        reference = null
                    };
                }
                else if (item == ">")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_BIG,
                        reference = null
                    };
                }
                else if (item == "<")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_SMALL,
                        reference = null
                    };
                }
                else if (item == ">=")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_BIGEQUAL,
                        reference = null
                    };
                }
                else if (item == "<=")
                {
                    poi = new PolishItem()
                    {
                        number = 0.0f,
                        cluster = null,
                        itemType = PolishItemType.CAL_SMALLEQUAL,
                        reference = null
                    };
                }
                if (poi != null)
                {
                    resVec.Add(poi);
                }
            }
            return resVec;
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
        /// 构造器
        /// </summary>
        public RuntimeManager()
        {
            this.Reset();
        }

        /// <summary>
        /// 获取调用堆栈
        /// </summary>
        public StackMachine CallStack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取符号表
        /// </summary>
        public SymbolTable Symbols
        {
            get;
            private set;
        }
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
        // 中断
        Interrupt,
        // 系统执行中
        Waiting,
        // 准备退出程序
        Exit
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
