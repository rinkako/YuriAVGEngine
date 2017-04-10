using System;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using Yuri.PlatformCore.Evaluator;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.VM;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>运行时上下文类：维护运行时环境的所有信息</para>
    /// <para>游戏保存的本质就是保存本实例</para>
    /// </summary>
    [Serializable]
    internal sealed class RuntimeManager
    {
        /// <summary>
        /// 获取当前调用堆栈顶部状态
        /// </summary>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>栈顶状态</returns>
        public StackMachineState GameState(StackMachine vsm) => vsm.ESP?.State ?? StackMachineState.NOP;

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
                if (this.DashingPureSa.Type == SActionType.act_dialog ||
                    this.DashingPureSa.Type == SActionType.act_branch)
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
            if (ret.CondPolish == String.Empty)
            {
                // 处理控制流程
                switch (ret.Type)
                {
                    case SActionType.NOP:
                    case SActionType.act_function:
                    case SActionType.act_for:
                        // 优先进入trueRouting
                        if (ret.TrueRouting != null && ret.TrueRouting.Count > 0)
                        {
                            ret = vsm.ESP.MircoStep(ret.TrueRouting[0]);
                            break;
                        }
                        // falseRouting
                        if (ret.FalseRouting != null && ret.FalseRouting.Count > 0)
                        {
                            ret = vsm.ESP.MircoStep(ret.FalseRouting[0]);
                            break;
                        }
                        // next
                        ret = vsm.ESP.MacroStep(ret);
                        break;
                    case SActionType.act_endfor:
                        // endfor直接跳过
                        ret = vsm.ESP.MacroStep(ret);
                        break;
                }
                // 移动下一指令指针，为下次处理做准备
                if (ret.Type != SActionType.act_for)
                {
                    vsm.ESP.MacroStep(ret);
                }
                // 返回当前要执行的指令实例
                return ret;
            }
            // 条件子句不为空时
            else
            {
                // 计算条件真值
                bool condBoolean = PolishEvaluator.EvaluateBoolean(ret.CondPolish, vsm);
                // 处理控制流程
                switch (ret.Type)
                {
                    // IF语句
                    case SActionType.act_if:
                        // 条件为真且有真分支
                        if (condBoolean == true && ret.TrueRouting != null && ret.TrueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            ret = vsm.ESP.MircoStep(ret.TrueRouting[0]);
                        }
                        // 条件为假且有假分支
                        else if (condBoolean == false && ret.FalseRouting != null && ret.FalseRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入falseRouting
                            ret = vsm.ESP.MircoStep(ret.FalseRouting[0]);
                        }
                        // 没有执行的语句时，移动指令指针到next节点
                        else
                        {
                            // 跳过if语句
                            ret = vsm.ESP.MacroStep(ret);
                        }
                        // 再移动一次指针，为下次处理做准备
                        vsm.ESP.MacroStep(ret);
                        // 返回当前要执行的指令实例
                        return ret;
                    // FOR语句
                    case SActionType.act_for:
                        // 如果条件为真就进入真分支
                        if (condBoolean == true && ret.TrueRouting != null && ret.TrueRouting.Count > 0)
                        {
                            // 移动下一指令指针，进入trueRouting
                            ret = vsm.ESP.MircoStep(ret.TrueRouting[0]);
                        }
                        // 如果条件为假直接跳过for语句
                        else
                        {
                            // 跳过if语句
                            ret = vsm.ESP.MacroStep(ret);
                        }
                        // 再移动一次指针，为下次处理做准备
                        vsm.ESP.MacroStep(ret);
                        // 返回当前要执行的指令实例
                        return ret;
                    // 除此以外，带了cond的语句，为真才执行
                    default:
                        if (condBoolean == false)
                        {
                            // 跳过当前语句
                            ret = vsm.ESP.MacroStep(ret);
                        }
                        // 移动下一指令指针，为下次处理做准备
                        vsm.ESP.MacroStep(ret);
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
                this.ParallelDispatcherList.ForEach(pdt => pdt.Stop());
            }
        }

        /// <summary>
        /// 从一个并行状态变换到另一个并行状态
        /// </summary>
        /// <param name="fromState">变化前状态的描述子</param>
        /// <param name="toState">目标状态的描述子</param>
        public void BackTraceParallelState(Dictionary<string, bool> fromState, Dictionary<string, bool> toState)
        {
            // 找出要移除的并行堆栈
            var removeList = new List<string>();
            if (toState != null)
            {
                removeList.AddRange(from removeKvp in fromState where toState.ContainsKey(removeKvp.Key) == false select removeKvp.Key);
            }
            else
            {
                removeList.AddRange(fromState.Select(removeKvp => removeKvp.Key));
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
            if (scene == null)
            {
                return;
            }
            var beforeSceneName = this.CallStack.SAVEP?.BindingSceneName;
            CommonUtils.ConsoleLine(String.Format("Call Scene: {0} , with target: {1}", scene?.Scenario, target == null ? "null" : target.NodeName),
                    "RuntimeManager", OutputStyle.Important);
            // 基础调用
            this.CallStack.Submit(scene, target);
            // 如果当前有并行，而又调用了带有并行的场景，那么就要暂停现在的并行
            var activeDict = new Dictionary<string, bool>();
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
            if (beforeSceneName != scene.Scenario)
            {
                if (scene.ParallellerContainer.Count > 0)
                {
                    int counter = 0;
                    foreach (var psf in scene.ParallellerContainer)
                    {
                        DispatcherTimer dt = new DispatcherTimer();
                        dt.Interval = TimeSpan.FromTicks((long) GlobalConfigContext.DirectorTimerInterval);
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
        /// <param name="valuePolish">右值逆波兰式</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        public void Assignment(string varname, string valuePolish, StackMachine vsm)
        {
            // 处理局部变量
            if (varname.StartsWith("$"))
            {
                // 非函数调用
                if (this.GameState(vsm) != StackMachineState.FunctionCalling)
                {
                    this.Symbols.Assign(vsm.EBP.ScriptName, varname.Replace("$", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
                }
                // 函数调用
                else
                {
                    var functionFrame = vsm.ESP.BindingFunction;
                    functionFrame.Symbols[varname.Replace("$", String.Empty)] = PolishEvaluator.Evaluate(valuePolish, vsm);
                }
            }
            // 处理全局变量
            else if (varname.StartsWith("&"))
            {
                this.Symbols.GlobalAssign(varname.Replace("&", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
            }
            // 处理持久化变量
            else if (varname.StartsWith("%"))
            {
                PersistenceContext.Assign(varname.Replace("%", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
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
                    return this.Symbols.Fetch(ResourceManager.GetInstance().GetScene(vsm.EBP.ScriptName), varName.Replace("$", String.Empty));
                }
                // 函数调用
                var funFrame = vsm.ESP.BindingFunction;
                return funFrame.Symbols[varName.Replace("$", String.Empty)];
            }
            // 处理全局变量
            if (varName.StartsWith("&"))
            {
                return this.Symbols.GlobalFetch(varName.Replace("&", String.Empty));
            }
            // 处理持久化变量
            if (varName.StartsWith("%"))
            {
                return PersistenceContext.Fetch(varName.Replace("%", String.Empty));
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
            PreviewSaveDataStoringPackage savePak = new PreviewSaveDataStoringPackage
            {
                saveTraceBackStack = new Stack<StackMachineFrame>()
            };
            // 弹空所有的函数调用和等待，只保存稳定性场景
            while (this.CallStack.ESP != this.CallStack.SAVEP)
            {
                savePak.saveTraceBackStack.Push(this.CallStack.Consume());
            }
            // 缓存指令指针，这里必须直接设null而不能用mircoStep避免IR寄存器被修改
            savePak.CacheIP = this.CallStack.ESP.IP;
            this.CallStack.ESP.IP = null;
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
            // 恢复指令指针
            this.CallStack.ESP.MircoStep(savePack.CacheIP);
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
        /// 修改主调用堆栈的引用
        /// </summary>
        /// <param name="vm">新的主调用堆栈</param>
        public void ResetCallstackObject(StackMachine vm)
        {
            this.CallStack = vm;
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
        /// 指令指针缓存
        /// </summary>
        public SceneAction CacheIP
        {
            get; set;
        }

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
