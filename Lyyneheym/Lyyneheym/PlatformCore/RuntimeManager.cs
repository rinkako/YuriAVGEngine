using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>运行时管理器：维护运行时的所有信息</para>
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    internal class RuntimeManager
    {
        /// <summary>
        /// 获取当前调用堆栈顶部状态
        /// </summary>
        /// <returns>栈顶状态</returns>
        public StackMachineState GameState()
        {
            if (this.CallStack.ESP == null)
            {
                return StackMachineState.NOP;
            }
            else
            {
                return this.CallStack.ESP.State;
            }
        }

        /// <summary>
        /// 取下一动作指令并暂存当前执行的动作
        /// </summary>
        /// <returns>动作实例</returns>
        public SceneAction MoveNext()
        {
            SceneAction fetched = this.FetchNextInstruction();
            if (fetched != null && this.CallStack.ESP.State == StackMachineState.Interpreting)
            {
                this.DashingPureSa = fetched.Clone(true);
            }
            return fetched;
        }

        /// <summary>
        /// 递归寻指
        /// </summary>
        /// <returns>动作实例</returns>
        private SceneAction FetchNextInstruction()
        {
            // 调用栈已经为空时预备退出
            if (this.CallStack.Count() == 0)
            {
                return null;
            }
            // 取出当前要执行的指令
            if (this.CallStack.ESP.State != StackMachineState.Interpreting &&
                this.CallStack.ESP.State != StackMachineState.FunctionCalling)
            {
                return null;
            }
            SceneAction ret = this.CallStack.ESP.IP;
            // 如果没有下一指令就弹栈
            if (ret == null)
            {
                this.CallStack.Consume();
                // 递归寻指
                return this.FetchNextInstruction();
            }
            // 如果没有条件子句
            if (ret.condPolish == "")
            {
                // 处理控制流程
                switch (ret.aType)
                {
                    case SActionType.NOP:
                    case SActionType.act_function:
                    case SActionType.act_for:
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
                if (ret.aType != SActionType.act_for)
                {
                    this.CallStack.ESP.PC++;
                    this.CallStack.ESP.IP = ret.next;
                }
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
        /// <param name="causedBy">触发者</param>
        /// <param name="detail">触发的原因</param>
        public void UserWait(string causedBy, string detail = null)
        {
            this.CallStack.Submit(causedBy, detail);
        }

        /// <summary>
        /// 延时
        /// </summary>
        /// <param name="causedBy">触发的原因</param>
        /// <param name="begin">开始计时的时刻</param>
        /// <param name="timespan">等待时间间隔</param>
        public void Delay(string causedBy, DateTime begin, TimeSpan timespan)
        {
            this.CallStack.Submit(causedBy, begin, timespan);
        }

        /// <summary>
        /// 等待动画完成
        /// </summary>
        /// <param name="causedBy">触发的原因</param>
        public void AnimateWait(string causedBy)
        {
            this.CallStack.Submit(causedBy);
        }

        /// <summary>
        /// 立即结束本次调用
        /// </summary>
        public void ExitCall()
        {
            this.CallStack.Consume();
        }

        /// <summary>
        /// 弹空栈顶的所有用户等待
        /// </summary>
        public void ExitUserWait()
        {
            while (this.CallStack.Count() > 0 && this.CallStack.ESP.State == StackMachineState.WaitUser)
            {
                this.CallStack.Consume();
            }
        }

        /// <summary>
        /// 弹空整个调用堆栈
        /// </summary>
        public void ExitAll()
        {
            while (this.CallStack.Count() != 0)
            {
                this.CallStack.Consume();
            }
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
        /// <param name="function">函数模板实例</param>
        /// <param name="args">参数列表</param>
        public void CallFunction(SceneFunction function, List<object> args)
        {
            // 为模板创建一个分支实例
            var callForker = function.Fork(true);
            this.CallStack.Submit(callForker, args);
            // 处理参数传递
            var funcSymbols = callForker.Symbols;
            for (int i = 0; i < args.Count; i++)
            {
                funcSymbols[callForker.Param[i].Substring(1)] = args[i];
            }
        }

        /// <summary>
        /// 左值运算一个变量
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">右值逆波兰式</param>
        public void Assignment(string varname, string valuePolish)
        {
            // 处理局部变量
            if (varname.StartsWith("$"))
            {
                // 非函数调用
                if (this.GameState() != StackMachineState.FunctionCalling)
                {
                    this.Symbols.Assign(this.CallStack.EBP.ScriptName, varname.Replace("$", ""), this.CalculatePolish(valuePolish));
                }
                // 函数调用
                else
                {
                    var functionFrame = ResourceManager.GetInstance().GetScene(this.CallStack.ESP.BindingSceneName).FuncContainer.Find((x) => x.Callname == this.CallStack.ESP.ScriptName);
                    functionFrame.Symbols[varname.Replace("$", "")] = this.CalculatePolish(valuePolish);
                }
            }
            // 处理全局变量
            else if (varname.StartsWith("&"))
            {
                this.Symbols.GlobalAssign(varname.Replace("&", ""), this.CalculatePolish(valuePolish));
            }
        }

        /// <summary>
        /// 取一个变量作右值
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量的引用</returns>
        public object Fetch(string varName)
        {
            // 处理局部变量
            if (varName.StartsWith("$"))
            {
                // 非函数调用
                if (this.GameState() != StackMachineState.FunctionCalling)
                {
                    return this.Symbols.Fetch(ResourceManager.GetInstance().GetScene(this.CallStack.EBP.ScriptName), varName.Replace("$", ""));
                }
                // 函数调用
                else
                {
                    var funFrame = this.CallStack.ESP.BindingFunction;
                    return funFrame.Symbols[varName.Replace("$", "")];
                }
            }
            // 处理全局变量
            else if (varName.StartsWith("&"))
            {
                return this.Symbols.GlobalFetch(varName.Replace("&", ""));
            }
            return null;
        }

        /// <summary>
        /// 预备存档
        /// </summary>
        public void PreviewSave()
        {
            this.saveTraceBackStack = new Stack<StackMachineFrame>();
            while (this.CallStack.ESP != this.CallStack.SAVEP)
            {
                this.saveTraceBackStack.Push(this.CallStack.Consume());
            }
        }

        /// <summary>
        /// 完成存档动作
        /// </summary>
        public void FinishedSave()
        {
            while (this.saveTraceBackStack.Count > 0)
            {
                this.CallStack.Submit(this.saveTraceBackStack.Pop());
            }
        }

        /// <summary>
        /// 设置场景管理器引用
        /// </summary>
        public void SetScreenManager(ScreenManager scr)
        {
            this.Screen = scr;
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
                if (poi.ItemType < PolishItemType.CAL_PLUS)
                {
                    calcStack.Push(poi);
                    continue;
                }
                // 下面开始是运算符
                if (poi.ItemType == PolishItemType.CAL_NOT && calcStack.Count >= 1)
                {
                    PolishItem peeker = calcStack.Peek();
                    if (peeker.ItemType == PolishItemType.CONSTANT || peeker.ItemType == PolishItemType.VAR_NUM)
                    {
                        calcStack.Pop();
                        double notres = Math.Abs(peeker.Number) < 1e-15 ? 1.0 : 0.0;
                        PolishItem np = new PolishItem()
                        {
                            Number = notres,
                            Reference = notres
                        };
                        calcStack.Push(np);
                        continue;
                    }
                    else if (peeker.ItemType == PolishItemType.STRING || peeker.ItemType == PolishItemType.VAR_STRING)
                    {
                        calcStack.Pop();
                        double notres = peeker.Cluster == "" ? 1.0 : 0.0;

                        PolishItem np = new PolishItem()
                        {
                            Number = notres,
                            Reference = notres
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
                        switch (poi.ItemType)
                        {
                            case PolishItemType.CAL_PLUS:
                                if (operand1.Reference is string)
                                {
                                    tempString = (string)operand1.Reference + (string)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Cluster = tempString,
                                        Reference = tempString
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (double)operand1.Reference + (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_MINUS:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference - (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：减法");
                                }
                                break;
                            case PolishItemType.CAL_MULTI:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference * (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：乘法");
                                }
                                break;
                            case PolishItemType.CAL_DIV:
                                if (operand1.Reference is double)
                                {
                                    if (Math.Abs((double)operand2.Reference) < 0)
                                    {
                                        throw new Exception("除零错误");
                                    }
                                    tempDouble = (double)operand1.Reference / (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：除法");
                                }
                                break;
                            case PolishItemType.CAL_ANDAND:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.Reference) > 0 && Math.Abs((double)operand2.Reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：&&");
                                }
                                break;
                            case PolishItemType.CAL_OROR:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.Reference) > 0 || Math.Abs((double)operand2.Reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：||");
                                }
                                break;
                            case PolishItemType.CAL_EQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference == (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.Reference == (string)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_NOTEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference != (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.Reference != (string)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIG:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference > (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) > 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIGEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference >= (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) >= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference < (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) < 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALLEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference <= (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) <= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
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
            return calcStack.Peek().Reference;
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
                Regex floatRegEx = new Regex("^(\\d*\\.)?\\d+$");
                // 常数项
                //if (item.All((x) => x >= '0' && x <= '9'))
                if (floatRegEx.IsMatch(item))
                {
                    double numitem = Convert.ToDouble(item);
                    poi = new PolishItem()
                    {
                        Number = numitem,
                        Cluster = null,
                        ItemType = PolishItemType.CONSTANT,
                        Reference = numitem
                    };
                }
                // 字符串
                else if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    string trimItem = item.Substring(1, item.Length - 2);
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = trimItem,
                        ItemType = PolishItemType.STRING,
                        Reference = trimItem
                    };
                }
                // 变量时
                else if ((item.StartsWith("&") || item.StartsWith("$")) && item.Length > 1)
                {
                    object varRef = this.Fetch(item);
                    if (varRef is double)
                    {
                        poi = new PolishItem()
                        {
                            Number = (double)varRef,
                            Cluster = null,
                            ItemType = PolishItemType.VAR_NUM,
                            Reference = varRef
                        };
                    }
                    else
                    {
                        poi = new PolishItem()
                        {
                            Number = 0.0f,
                            Cluster = Convert.ToString(varRef),
                            ItemType = PolishItemType.VAR_STRING,
                            Reference = varRef
                        };
                    }
                }
                else if (item == "+")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_PLUS,
                        Reference = null
                    };
                }
                else if (item == "-")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_MINUS,
                        Reference = null
                    };
                }
                else if (item == "*")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_MULTI,
                        Reference = null
                    };
                }
                else if (item == "/")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_DIV,
                        Reference = null
                    };
                }
                else if (item == "!")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_NOT,
                        Reference = null
                    };
                }
                else if (item == "&&")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_ANDAND,
                        Reference = null
                    };
                }
                else if (item == "||")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_OROR,
                        Reference = null
                    };
                }
                else if (item == "<>")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_NOTEQUAL,
                        Reference = null
                    };
                }
                else if (item == "==")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_EQUAL,
                        Reference = null
                    };
                }
                else if (item == ">")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_BIG,
                        Reference = null
                    };
                }
                else if (item == "<")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_SMALL,
                        Reference = null
                    };
                }
                else if (item == ">=")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_BIGEQUAL,
                        Reference = null
                    };
                }
                else if (item == "<=")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_SMALLEQUAL,
                        Reference = null
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
            this.Symbols = SymbolTable.GetInstance();
            this.Screen = null;
            this.PlayingBGM = null;
            //this.TitlePoint = new KeyValuePair<string, SceneAction>(null, null);
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

        /// <summary>
        /// 获取屏幕管理器
        /// </summary>
        public ScreenManager Screen
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置正在播放的BGM
        /// </summary>
        public string PlayingBGM
        {
            get;
            set;
        }

        /// <summary>
        /// 当前正在执行的动作的无关系副本
        /// </summary>
        public SceneAction DashingPureSa
        {
            get;
            private set;
        }

        /// <summary>
        /// 存档状态保存栈
        /// </summary>
        private Stack<StackMachineFrame> saveTraceBackStack;
    }

    /// <summary>
    /// 游戏的总体状态
    /// </summary>
    internal enum GameState
    {
        // 游戏剧情进行时
        Performing,
        // 等待用户操作
        WaitForUserInput,
        // 中断
        Interrupt,
        // 系统执行中
        Waiting,
        // 等待动画中
        WaitAni,
        // 准备退出程序
        Exit
    }
}
