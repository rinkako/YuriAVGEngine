using System;
using System.Windows.Threading;
using System.Collections.Generic;
using Yuri.PlatformCore.Evaluator;
using Yuri.PlatformCore.Semaphore;
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
                            return vsm.ESP.MircoStep(ret.TrueRouting[0]);
                            
                        }
                        // falseRouting
                        if (ret.FalseRouting != null && ret.FalseRouting.Count > 0)
                        {
                            return vsm.ESP.MircoStep(ret.FalseRouting[0]);
                            
                        }
                        // next
                        return vsm.ESP.MacroStep(ret);
                        
                    case SActionType.act_endfor:
                        // endfor直接跳过
                        return vsm.ESP.MacroStep(ret);
                        
                }
                // 移动下一指令指针，为下次处理做准备
                //if (ret.Type != SActionType.act_for)
                //{
                return vsm.ESP.MacroStep(ret);
                //}
                // 返回当前要执行的指令实例
                //return ret;
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
                        // ret = vsm.ESP.MacroStep(ret);
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
                        // ret = vsm.ESP.MacroStep(ret);
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
                        //ret = vsm.ESP.MacroStep(ret);
                        // 返回当前要执行的指令实例
                        return ret;
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
            CommonUtils.ConsoleLine(String.Format("Call Scene: {0} , with target: {1}", scene.Scenario, target == null ? "null" : target.NodeName),
                    "RuntimeManager", OutputStyle.Important);
            // 基础调用
            this.CallStack.Submit(scene, target);
            // 如果当前有并行，而又调用了带有并行的场景，那么就要暂停现在的并行
            if (this.LastScenario != scene.Scenario)
            {
                // fix:这块本来在外面的
                if (this.ParallelExecutorStack.Count != 0)
                {
                    var parasBeforeCalling = this.ParallelExecutorStack.Peek();
                    parasBeforeCalling.ForEach(t => t.Dispatcher.Stop());
                }
                // 处理场景的并行函数
                this.ConstructParallel(scene);
            }
            // 更新场景名字记录
            this.LastScenario = scene.Scenario;
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
            // 弹出的是主堆栈上的场景
            if (svm == this.CallStack && consumed.State == StackMachineState.Interpreting)
            {
                // 恢复到上一个并行栈帧的状态
                this.BackTraceParallel();
                // 关闭当前的信号量订阅者
                SemaphoreDispatcher.UnregisterSemaphoreService();
            }
        }

        /// <summary>
        /// 为回滚构造场景的并行处理
        /// </summary>
        /// <param name="scene">需要构造的场景实例</param>
        public void ConstructParallelForRollingBack(Scene scene)
        {
            var reverseStack = new Stack<List<ParallelExecutor>>();
            while (this.ParallelExecutorStack.Count > 0)
            {
                reverseStack.Push(this.ParallelExecutorStack.Pop());
            }
            this.ConstructParallel(scene);
            while (reverseStack.Count > 0)
            {
                this.ParallelExecutorStack.Push(reverseStack.Pop());
            }
        }

        /// <summary>
        /// 构造场景的并行处理
        /// </summary>
        /// <param name="scene">需要构造的场景实例</param>
        public void ConstructParallel(Scene scene)
        {
            var execList = new List<ParallelExecutor>();
            if (scene.ParallellerContainer.Count > 0)
            {
                int counter = 0;
                foreach (var psf in scene.ParallellerContainer)
                {
                    ParallelExecutor pExec = new ParallelExecutor()
                    {
                        Scenario = scene.Scenario
                    };
                    DispatcherTimer dt = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromTicks((long)GlobalConfigContext.DirectorTimerInterval)
                    };
                    dt.Tick += this.ParallelHandler;
                    pExec.Dispatcher = dt;
                    var pvm = new StackMachine();
                    pvm.SetMachineName("VM#" + psf.GlobalName);
                    pvm.Submit(psf, new List<object>());
                    pExec.Executor = pvm;
                    ParallelDispatcherArgsPackage pdap = new ParallelDispatcherArgsPackage()
                    {
                        Index = counter++,
                        Render = new UpdateRender(pvm),
                        BindingSF = psf,
                        IsSemaphore = false
                    };
                    dt.Tag = pdap;
                    dt.Start();
                    execList.Add(pExec);
                }
            }
            this.ParallelExecutorStack.Push(execList);
        }
        
        /// <summary>
        /// 回滚并行堆栈的状态
        /// </summary>
        public void BackTraceParallel()
        {
            // 游戏结束的情况
            if (this.ParallelExecutorStack.Count == 0)
            {
                return;
            }
            // 弹空上一并行堆栈的内容
            var lastParaExecList = this.ParallelExecutorStack.Pop();
            foreach (var lexec in lastParaExecList)
            {
                while (lexec.Executor.Count() != 0)
                {
                    lexec.Executor.Consume();
                }
                lexec.Dispatcher.Stop();
            }
            // 如果已经没有并行堆栈需要重启就直接返回
            if (this.ParallelExecutorStack.Count == 0)
            {
                return;
            }
            // 重启当前并行状态栈顶的并行处理器
            this.ParallelExecutorStack.Peek().ForEach(e => e.Dispatcher.Start());
        }

        /// <summary>
        /// 停止并清空并行堆栈
        /// </summary>
        public void StopAllParallel()
        {
            while (this.ParallelExecutorStack.Count > 0)
            {
                this.ParallelExecutorStack.Pop().ForEach(t =>
                {
                    t.Executor.Clear();
                    t.Dispatcher.Stop();
                });
            }
        }

        /// <summary>
        /// 暂停并行处理
        /// </summary>
        public void PauseParallel()
        {
            if (this.ParallelExecutorStack.Count == 0)
            {
                return;
            }
            this.ParallelExecutorStack.Peek().ForEach(t => t.Dispatcher.Stop());
        }

        /// <summary>
        /// 恢复并行处理
        /// </summary>
        public void RestartParallel()
        {
            if (this.ParallelExecutorStack.Count == 0)
            {
                return;
            }
            this.ParallelExecutorStack.Peek().ForEach(t => t.Dispatcher.Start());
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
                    this.Symbols.SceneCtxDao.Assign(vsm.EBP.ScriptName, varname.Replace("$", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
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
                this.Symbols.GlobalCtxDao.GlobalAssign(varname.Replace("&", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
            }
            // 处理持久化变量
            else if (varname.StartsWith("%"))
            {
                PersistContextDAO.Assign(varname.Replace("%", String.Empty), PolishEvaluator.Evaluate(valuePolish, vsm));
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
                    return this.Symbols.SceneCtxDao.Fetch(ResourceManager.GetInstance().GetScene(vsm.EBP.ScriptName), varName.Replace("$", String.Empty));
                }
                // 函数调用
                var funFrame = vsm.ESP.BindingFunction;
                return funFrame.Symbols[varName.Replace("$", String.Empty)];
            }
            // 处理全局变量
            if (varName.StartsWith("&"))
            {
                return this.Symbols.GlobalCtxDao.GlobalFetch(varName.Replace("&", String.Empty));
            }
            // 处理持久化变量
            if (varName.StartsWith("%"))
            {
                return PersistContextDAO.Fetch(varName.Replace("%", String.Empty));
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
            savePak.ParallelExecutorStore = this.ParallelExecutorStack;
            this.ParallelExecutorStack = null;
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
            this.ParallelExecutorStack = savePack.ParallelExecutorStore;
        }

        /// <summary>
        /// 获取当前是否处于右键菜单调用状态
        /// </summary>
        /// <returns>栈顶是否为右键菜单栈帧</returns>
        public bool GetRclickingState()
        {
            return this.CallStack.Count() > 0 && this.CallStack.EBP.BindingFunction != null &&
                   this.CallStack.EBP.BindingFunction.GlobalName == "__YuriFunc@rclick?main";
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
            this.ParallelExecutorStack = new Stack<List<ParallelExecutor>>();
            this.SemaphoreBindings = new Dictionary<string, List<Tuple<string, string>>>();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public RuntimeManager()
        {
            this.Reset();
        }
        
        /// <summary>
        /// 获取或设置并行执行器堆栈
        /// </summary>
        /// <remarks>在序列化RuntimeManager时务必保证该字段为null值</remarks>
        public Stack<List<ParallelExecutor>> ParallelExecutorStack
        {
            get;
            set;
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
        /// 获取或设置信号量绑定信息
        /// </summary>
        /// <remarks>数据结构：键-信号量名字，值-一个二元组向量，第一元激活函数名，第二元反激活函数名</remarks>
        public Dictionary<string, List<Tuple<string, string>>> SemaphoreBindings
        {
            get;
            set;
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
        /// 获取或设置上一个场景的名字
        /// </summary>
        public string LastScenario
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前是否允许右键菜单
        /// </summary>
        public bool EnableRClick { get; set; } = true;
        
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

        /// <summary>
        /// 是否为信号量处理并行包装
        /// </summary>
        public bool IsSemaphore { get; set; } = false;

        /// <summary>
        /// 信号量处理器的堆栈
        /// </summary>
        public StackMachine SemaphoreStack { get; set; } = null;

        /// <summary>
        /// 信号量调度类型
        /// </summary>
        public SemaphoreHandlerType SemaphoreType { get; set; } = SemaphoreHandlerType.ScheduleOnce;
    }

    /// <summary>
    /// 并行执行器包装
    /// </summary>
    internal sealed class ParallelExecutor
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string Scenario { get; set; }

        /// <summary>
        /// 并行堆栈
        /// </summary>
        public StackMachine Executor { get; set; }

        /// <summary>
        /// 并行线程分发器
        /// </summary>
        public DispatcherTimer Dispatcher { get; set; }
    }

    /// <summary>
    /// 打包暂存不要保存到磁盘的临时包装类
    /// </summary>
    internal sealed class PreviewSaveDataStoringPackage
    {
        /// <summary>
        /// 临时上下文缓存
        /// </summary>
        public SimpleContext TempCtx
        {
            get;
            set;
        }

        /// <summary>
        /// 指令指针缓存
        /// </summary>
        public SceneAction CacheIP
        {
            get;
            set;
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
        /// 并行执行器向量
        /// </summary>
        public Stack<List<ParallelExecutor>> ParallelExecutorStore
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

        /// <summary>
        /// 并行活性栈
        /// </summary>
        public Stack<Dictionary<string, bool>> ParallelActivationDict
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
