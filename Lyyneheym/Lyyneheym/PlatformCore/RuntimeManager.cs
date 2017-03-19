using System;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yuri.ILPackage;
using Yuri.Utils;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>环境管理器：维护运行时环境的所有信息</para>
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    internal class RuntimeManager
    {
        /// <summary>
        /// 获取当前调用堆栈顶部状态
        /// </summary>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>栈顶状态</returns>
        public StackMachineState GameState(StackMachine vsm)
        {
            if (vsm.ESP == null)
            {
                return StackMachineState.NOP;
            }
            else
            {
                return vsm.ESP.State;
            }
        }

        /// <summary>
        /// 取下一动作指令并暂存当前执行的动作
        /// </summary>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>动作实例</returns>
        public SceneAction MoveNext(StackMachine vsm)
        {
            SceneAction fetched = this.FetchNextInstruction(vsm);
            if (fetched != null && vsm.ESP.State == StackMachineState.Interpreting)
            {
                this.DashingPureSa = fetched.Clone(true);
                if (this.DashingPureSa.aType == SActionType.act_dialog ||
                    this.DashingPureSa.aType == SActionType.act_branch)
                {
                    RollbackManager.SteadyForward(false, this.DashingPureSa, this.PlayingBGM);
                }
            }
            return fetched;
        }

        /// <summary>
        /// 在指定的调用堆栈上做递归寻指
        /// </summary>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>动作实例</returns>
        private SceneAction FetchNextInstruction(StackMachine vsm)
        {
            // 调用栈已经为空时预备退出
            if (vsm.Count() == 0)
            {
                return null;
            }
            // 取出当前要执行的指令
            if (vsm.ESP.State != StackMachineState.Interpreting &&
                vsm.ESP.State != StackMachineState.FunctionCalling)
            {
                return null;
            }
            SceneAction ret = vsm.ESP.IP;
            // 如果没有下一指令就弹栈
            if (ret == null)
            {
                this.ExitCall(vsm);
                // 递归寻指
                return this.FetchNextInstruction(vsm);
            }
            // 如果没有条件子句
            if (ret.condPolish == String.Empty)
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
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.trueRouting[0];
                            break;
                        }
                        // falseRouting
                        else if (ret.falseRouting != null && ret.falseRouting.Count > 0)
                        {
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.falseRouting[0];
                            break;
                        }
                        // next
                        else
                        {
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.next;
                            break;
                        }
                    case SActionType.act_endfor:
                        // endfor直接跳过
                        vsm.ESP.PC++;
                        ret = vsm.ESP.IP = ret.next;
                        break;
                }
                // 移动下一指令指针，为下次处理做准备
                if (ret.aType != SActionType.act_for)
                {
                    vsm.ESP.PC++;
                    vsm.ESP.IP = ret.next;
                }
                // 返回当前要执行的指令实例
                return ret;
            }
            // 条件子句不为空时
            else
            {
                // 计算条件真值
                bool condBoolean = this.CalculateBooleanPolish(ret.condPolish, vsm);
                // 处理控制流程
                switch (ret.aType)
                {
                    // IF语句
                    case SActionType.act_if:
                        // 条件为真且有真分支
                        if (condBoolean == true && ret.trueRouting != null && ret.trueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.trueRouting[0];
                        }
                        // 条件为假且有假分支
                        else if (condBoolean == false && ret.falseRouting != null && ret.falseRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入falseRouting
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.falseRouting[0];
                        }
                        // 没有执行的语句时，移动指令指针到next节点
                        else
                        {
                            // 跳过if语句
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.next;
                        }
                        // 再移动一次指针，为下次处理做准备
                        vsm.ESP.PC++;
                        vsm.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                    // FOR语句
                    case SActionType.act_for:
                        // 如果条件为真就进入真分支
                        if (condBoolean == true && ret.trueRouting != null && ret.trueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.trueRouting[0];
                        }
                        // 如果条件为假直接跳过for语句
                        else
                        {
                            // 跳过if语句
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.next;
                        }
                        // 再移动一次指针，为下次处理做准备
                        vsm.ESP.PC++;
                        vsm.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                    // 除此以外，带了cond的语句，为真才执行
                    default:
                        if (condBoolean == false)
                        {
                            // 跳过当前语句
                            vsm.ESP.PC++;
                            ret = vsm.ESP.IP = ret.next;
                        }
                        // 移动下一指令指针，为下次处理做准备
                        vsm.ESP.PC++;
                        vsm.ESP.IP = ret.next;
                        // 返回当前要执行的指令实例
                        return ret;
                }
            }
        }

        /// <summary>
        /// 停止所有的并行处理，该操作将导致所有并行调用堆栈强制弹空，只能在对话回滚时使用
        /// </summary>
        public void StopParallel()
        {
            if (this.ParallelDispatcherList != null)
            {
                // 强制并行堆栈里的弹空所有调用
                foreach (var vm in this.ParallelVMList)
                {
                    while (vm.Count() != 0)
                    {
                        vm.Consume();
                    }
                }
                // 关闭计时器
                foreach (var pdt in this.ParallelDispatcherList)
                {
                    pdt.Stop();
                }
            }
        }

        /// <summary>
        /// 从一个并行状态变换到另一个并行状态
        /// </summary>
        /// <param name="fromState">变化前的状态描述子</param>
        /// <param name="toState">目标状态描述子</param>
        public void BackTraceParallelState(Dictionary<string, bool> fromState, Dictionary<string, bool> toState)
        {
            // 找出要移除的并行堆栈
            List<string> removeList = new List<string>();
            if (toState != null)
            {
                foreach (var vmKvp in toState)
                {
                    if (fromState.ContainsKey(vmKvp.Key) == false)
                    {
                        removeList.Add(vmKvp.Key);
                    }
                }
            }
            // 停止这些并行计时器，移除并行堆栈
            foreach (var r in removeList)
            {
                var removeIdx = this.ParallelVMList.FindIndex((x) => x.StackName == r);
                this.ParallelDispatcherList[removeIdx].Stop();
                this.ParallelVMList[removeIdx].Clear();
                this.ParallelDispatcherList.RemoveAt(removeIdx);
                this.ParallelVMList.RemoveAt(removeIdx);
            }
            // 恢复上一个状态的并行
            if (toState != null)
            {
                foreach (var vmKvp in toState)
                {
                    if (vmKvp.Value == true)
                    {
                        var activeIdx = this.ParallelVMList.FindIndex((x) => x.StackName == vmKvp.Key);
                        this.ParallelDispatcherList[activeIdx].Start();
                    }
                }
            }
        }

        /// <summary>
        /// 场景调用
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <param name="target">目标标签</param>
        public void CallScene(Scene scene, SceneAction target = null)
        {
            CommonUtils.ConsoleLine(String.Format("Call Scene: {0} , with target: {1}", scene.Scenario, target == null ? "null" : target.saNodeName),
                    "RuntimeManager", OutputStyle.Important);
            // 基础调用
            this.CallStack.Submit(scene, target);
            // 如果当前有并行，而又调用了带有并行的场景，那么就要暂停现在的并行
            Dictionary<string, bool> activeDict = new Dictionary<string, bool>();
            if (this.ParallelStack.Count != 0)
            {
                var curParaStateDict = this.ParallelStack.Peek();
                foreach (var kvp in curParaStateDict)
                {
                    // 只关闭开着的并行堆栈
                    if (kvp.Value == true)
                    {
                        var vmIdx = this.ParallelVMList.FindIndex((x) => x.StackName == kvp.Key);
                        if (vmIdx != -1)
                        {
                            this.ParallelDispatcherList[vmIdx].Stop();
                        }
                    }
                    // 完整拷贝上一状态
                    activeDict[kvp.Key] = false;
                }
            }
            // 处理场景的并行函数
            if (scene.ParallellerContainer.Count > 0)
            {
                int counter = 0;
                foreach (var psf in scene.ParallellerContainer)
                {
                    DispatcherTimer dt = new DispatcherTimer();
                    dt.Interval = TimeSpan.FromTicks((long)GlobalDataContext.DirectorTimerInterval);
                    dt.Tick += this.ParallelHandler;
                    this.ParallelDispatcherList.Add(dt);
                    var pvm = new StackMachine();
                    pvm.SetMachineName("VM#" + psf.GlobalName);
                    pvm.Submit(psf, new List<object>());
                    this.ParallelVMList.Add(pvm);
                    ParallelDispatcherArgsPackage pdap = new ParallelDispatcherArgsPackage()
                    {
                        Index = counter++,
                        Render = new UpdateRender(pvm),
                        BindingSF = psf
                    };
                    dt.Tag = pdap;
                    dt.Start();
                    activeDict[pvm.StackName] = true;
                }
            }
            // 压并行状态栈
            this.ParallelStack.Push(activeDict);
        }

        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="function">函数模板实例</param>
        /// <param name="args">参数列表</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        public void CallFunction(SceneFunction function, List<object> args, StackMachine vsm)
        {
            CommonUtils.ConsoleLine(String.Format("Call Function: {0}", function.GlobalName),
                    "RuntimeManager", OutputStyle.Important);
            // 为模板创建一个分支实例
            var callForker = function.Fork(true);
            vsm.Submit(callForker, args);
            // 处理参数传递
            var funcSymbols = callForker.Symbols;
            for (int i = 0; i < args.Count; i++)
            {
                funcSymbols[callForker.Param[i].Substring(1)] = args[i];
            }
        }

        /// <summary>
        /// 弹空栈顶的所有用户等待
        /// </summary>
        public void ExitUserWait()
        {
            while (this.CallStack.Count() > 0 && this.CallStack.ESP.State == StackMachineState.WaitUser)
            {
                this.ExitCall(this.CallStack);
            }
        }

        /// <summary>
        /// 弹空整个调用堆栈
        /// </summary>
        public void ExitAll()
        {
            while (this.CallStack.Count() != 0)
            {
                this.ExitCall(this.CallStack);
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
        /// <param name="svm">要作用的调用堆栈</param>
        public void ExitCall(StackMachine svm)
        {
            // 弹调用堆栈
            var consumed = svm.Consume();
            // 如果弹出的是主堆栈上的场景，就要恢复到上一个并行栈帧的状态
            if (svm == this.CallStack && consumed.State == StackMachineState.Interpreting)
            {
                var fromParallelState = this.ParallelStack.Pop();
                Dictionary<string, bool> toParallelState = null;
                if (this.ParallelStack.Count != 0)
                {
                    toParallelState = this.ParallelStack.Peek();
                }
                this.BackTraceParallelState(fromParallelState, toParallelState);
            }
        }

        /// <summary>
        /// 左值运算一个变量
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">右值逆波兰式</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        public void Assignment(string varname, string valuePolish, StackMachine vsm)
        {
            // 处理局部变量
            if (varname.StartsWith("$"))
            {
                // 非函数调用
                if (this.GameState(vsm) != StackMachineState.FunctionCalling)
                {
                    this.Symbols.Assign(vsm.EBP.ScriptName, varname.Replace("$", ""), this.CalculatePolish(valuePolish, vsm));
                }
                // 函数调用
                else
                {
                    var functionFrame = vsm.ESP.BindingFunction; //ResourceManager.GetInstance().GetScene(this.CallStack.ESP.BindingSceneName).FuncContainer.Find((x) => x.Callname == this.CallStack.ESP.ScriptName);
                    functionFrame.Symbols[varname.Replace("$", "")] = this.CalculatePolish(valuePolish, vsm);
                }
            }
            // 处理全局变量
            else if (varname.StartsWith("&"))
            {
                this.Symbols.GlobalAssign(varname.Replace("&", ""), this.CalculatePolish(valuePolish, vsm));
            }
        }

        /// <summary>
        /// 取一个变量作右值
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>变量的引用</returns>
        public object Fetch(string varName, StackMachine vsm)
        {
            // 处理局部变量
            if (varName.StartsWith("$"))
            {
                // 非函数调用
                if (this.GameState(vsm) != StackMachineState.FunctionCalling)
                {
                    return this.Symbols.Fetch(ResourceManager.GetInstance().GetScene(vsm.EBP.ScriptName), varName.Replace("$", ""));
                }
                // 函数调用
                else
                {
                    var funFrame = vsm.ESP.BindingFunction;
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
        /// 预备存档，保存时必须先调用此方法
        /// </summary>
        /// <returns>不要保存到稳定储存器上的内容的打包</returns>
        public PreviewSaveDataStoringPackage PreviewSave()
        {
            // 处理主调用堆栈上的栈帧
            PreviewSaveDataStoringPackage savePak = new PreviewSaveDataStoringPackage();
            savePak.saveTraceBackStack = new Stack<StackMachineFrame>();
            while (this.CallStack.ESP != this.CallStack.SAVEP)
            {
                savePak.saveTraceBackStack.Push(this.CallStack.Consume());
            }
            // 处理并行句柄引用
            savePak.ParallelHandlerStore = this.ParallelHandler;
            this.ParallelHandler = null;
            // 处理并行器向量的引用
            savePak.ParallelDispatcherListStore = this.ParallelDispatcherList;
            this.ParallelDispatcherList = null;
            // 返回封装包
            return savePak;
        }

        /// <summary>
        /// 完成存档，保存后必须调用此方法
        /// </summary>
        /// <param name="savePack">调用PreviewSave时的返回值</param>
        public void FinishedSave(PreviewSaveDataStoringPackage savePack)
        {
            // 恢复主调用堆栈上的栈帧
            while (savePack.saveTraceBackStack.Count > 0)
            {
                this.CallStack.Submit(savePack.saveTraceBackStack.Pop());
            }
            // 恢复并行句柄引用
            this.ParallelHandler = savePack.ParallelHandlerStore;
            // 恢复并行器向量的引用
            this.ParallelDispatcherList = savePack.ParallelDispatcherListStore;
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
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>表达式的真值</returns>
        public bool CalculateBooleanPolish(string polish, StackMachine vsm)
        {
            return Convert.ToBoolean(this.CalculatePolish(polish, vsm));
        }

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="polish">表达式的逆波兰式</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>计算结果的值（Double/字符串）</returns>
        public object CalculatePolish(string polish, StackMachine vsm)
        {
            List<PolishItem> calcList = this.GetPolishItemList(polish, vsm);
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
        /// 修改主调用堆栈的引用
        /// </summary>
        /// <param name="vm">新的主调用堆栈</param>
        public void ResetCallstackObject(StackMachine vm)
        {
            this.CallStack = vm;
        }

        /// <summary>
        /// 将逆波兰式转化为可计算的项
        /// </summary>
        /// <param name="polish">逆波兰式字符串</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>可计算项目向量</returns>
        private List<PolishItem> GetPolishItemList(string polish, StackMachine vsm)
        {
            List<PolishItem> resVec = new List<PolishItem>();
            string[] polishItem = polish.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in polishItem)
            {
                PolishItem poi = null;
                Regex floatRegEx = new Regex(@"^(\-|\+)?\d+(\.\d+)?$");
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
                    object varRef = this.Fetch(item, vsm);
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
            this.CallStack.SetMachineName("Yuri");
            this.Symbols = SymbolTable.GetInstance();
            this.Screen = null;
            this.PlayingBGM = String.Empty;
            this.ParallelStack = new Stack<Dictionary<string, bool>>();
            this.ParallelDispatcherList = new List<DispatcherTimer>();
            this.ParallelVMList = new List<StackMachine>();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public RuntimeManager()
        {
            this.Reset();
        }
        
        /// <summary>
        /// 获取主调用堆栈
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
        /// 获取并行堆栈向量
        /// </summary>
        public List<StackMachine> ParallelVMList
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取并行计时器向量
        /// </summary>
        /// <remarks>在序列化RuntimeManager时务必保证该字段为null值</remarks>
        public List<DispatcherTimer> ParallelDispatcherList
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置并行堆栈的处理函数
        /// </summary>
        /// <remarks>在序列化RuntimeManager时务必保证该字段为null值</remarks>
        public EventHandler ParallelHandler
        {
            get;
            set;
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
        /// 并行状态栈，记录并行调用堆栈的活性
        /// </summary>
        public Stack<Dictionary<string, bool>> ParallelStack
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
        /// 获取或设置正在演出的章节名
        /// 该字段仅用在存档标记，和调用堆栈无关
        /// </summary>
        public string PerformingChapter
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
            set;
        }
    }

    /// <summary>
    /// 并行参数包装类
    /// </summary>
    internal sealed class ParallelDispatcherArgsPackage
    {
        /// <summary>
        /// 获取或设置在并行向量里的下标
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 获取或设置绑定的渲染器
        /// </summary>
        public UpdateRender Render { get; set; }

        /// <summary>
        /// 获取或设置绑定的函数入口
        /// </summary>
        public SceneFunction BindingSF { get; set; }
    }

    /// <summary>
    /// 打包暂存不要保存到磁盘的临时包装类
    /// </summary>
    internal sealed class PreviewSaveDataStoringPackage
    {
        /// <summary>
        /// 并行处理器句柄
        /// </summary>
        public EventHandler ParallelHandlerStore
        {
            get;
            set;
        }

        /// <summary>
        /// 并行计时器向量
        /// </summary>
        public List<DispatcherTimer> ParallelDispatcherListStore
        {
            get;
            set;
        }

        /// <summary>
        /// 存档状态保存栈
        /// </summary>
        public Stack<StackMachineFrame> saveTraceBackStack
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 游戏的总体状态
    /// </summary>
    internal enum GameState
    {
        /// <summary>
        /// 游戏剧情进行时
        /// </summary>
        Performing,
        /// <summary>
        /// 等待用户操作
        /// </summary>
        WaitForUserInput,
        /// <summary>
        /// 中断
        /// </summary>
        Interrupt,
        /// <summary>
        /// 系统执行中
        /// </summary>
        Waiting,
        /// <summary>
        /// 等待动画中
        /// </summary>
        WaitAni,
        /// <summary>
        /// 准备退出程序
        /// </summary>
        Exit
    }
}
