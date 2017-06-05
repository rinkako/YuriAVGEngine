//#define NOTIME
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Yuri.PlatformCore.Audio;
using Yuri.PlatformCore.Evaluator;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.Graphic3D;
using Yuri.PlatformCore.Semaphore;
using Yuri.PlatformCore.VM;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>控制器类：管理整个游戏生命周期的类</para>
    /// <para>她是一个单例类</para>
    /// </summary>
    internal class Director
    {
        #region 初次进入时的初始化相关函数
        /// <summary>
        /// 初始化游戏设置并载入持久化数据
        /// </summary>
        private void InitConfig()
        {
            try
            {
                // 读取游戏设置
                ConfigParser.ConfigParse();
                // 第一次打开游戏就创建持久性上下文
                if (System.IO.File.Exists(GlobalConfigContext.PersistenceFileName) == false)
                {
                    PersistContextDAO.Assign("___YURIRI@ACCDURATION___", 0);
                    PersistContextDAO.Assign("___YURIRI@FIRSTPLAYTIMESTAMP___", DateTime.Now.ToString());
                    PersistContextDAO.SaveToSteadyMemory();
                }
                // 非第一次打开游戏就读取持久性上下文
                else
                {
                    PersistContextDAO.LoadFromSteadyMemory();
                    Director.LastGameTimeAcc = TimeSpan.Parse(PersistContextDAO.Exist("___YURIRI@ACCDURATION___") ?
                        PersistContextDAO.Fetch("___YURIRI@ACCDURATION___").ToString() : "0");
                }
                Director.StartupTimeStamp = DateTime.Now;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("No config file is detected, use defualt value." + ex, "Director", LogLevel.Error);
            }
        }

        /// <summary>
        /// 初始化运行时环境，并指定脚本的入口
        /// </summary>
        private void InitRuntime()
        {
            var mainScene = this.resMana.GetScene(GlobalConfigContext.Script_Main);
            if (mainScene == null)
            {
                LogUtils.LogLine(String.Format("No Entry Point Scene: {0}, Program will exit.", GlobalConfigContext.Script_Main),
                    "Director", LogLevel.Error);
                this.updateRender.Shutdown();
            }
            this.resMana.GetAllScene().ForEach(t => Director.RunMana.Symbols.AddSceneSymbolContext(t));
            Director.RunMana.CallScene(mainScene);
            SemaphoreDispatcher.InitSystemSemaphore();
        }
        #endregion

        #region 前端更新后台相关函数
        /// <summary>
        /// 提供由前端更新后台键盘按键信息的方法
        /// </summary>
        /// <param name="e">键盘事件</param>
        /// <param name="isDown">是否按下</param>
        public void UpdateKeyboard(KeyEventArgs e, bool isDown)
        {
            this.updateRender.SetKeyboardState(e, isDown);
            LogUtils.LogLine(String.Format("Keyboard event: {0} <- {1}", e.Key, e.KeyStates),
                "Director", LogLevel.Normal);
        }

        /// <summary>
        /// 提供由前端更新后台鼠标按键信息的方法
        /// </summary>
        /// <param name="e">鼠标事件</param>
        public void UpdateMouse(MouseButtonEventArgs e)
        {
            this.updateRender.SetMouseButtonState(e.ChangedButton, e.ButtonState);
        }

        /// <summary>
        /// 提供由前端更新后台鼠标滚轮信息的方法
        /// </summary>
        /// <param name="delta">鼠标滚轮差分，上滚为正，下滚为负</param>
        public void UpdateMouseWheel(int delta)
        {
            this.updateRender.SetMouseWheelDelta(delta);
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 设置运行时环境管理器，用于读取保存的信息
        /// </summary>
        /// <param name="rm">反序列化后的RM实例</param>
        public static void ResumeFromSaveData(RuntimeManager rm)
        {
            // 停止消息循环
            Director.PauseUpdateContext();
            // 清空回滚器
            RollbackManager.Clear();
            // 清空画面并停下BGM
            ViewManager.GetInstance().RemoveView(ResourceType.Unknown);
            // 检查是否需要回滚当前的并行处理
            Director.RunMana.StopAllParallel();
            // 变更运行时环境
            Director.RunMana = rm;
            Director.RunMana.ParallelHandler = Director.GetInstance().ParallelUpdateContext;
            LogUtils.LogLine("RuntimeManager is replaced", "Director", LogLevel.Important);
            // 缓存指令指针
            var irname = rm.CallStack.ESP.IR;
            var isname = rm.CallStack.ESP.BindingSceneName;
            rm.CallStack.ESP.MircoStep(Director.GetInstance().resMana.GetScene(isname).YuriDict[irname]);
            // 变更屏幕管理器
            ScreenManager.ResetSynObject(Director.RunMana.Screen);
            LogUtils.LogLine("ScreenManager is replaced", "Director", LogLevel.Important);
            // 重绘整个画面
            ViewManager.GetInstance().ReDraw();
            // 重新绑定渲染器的作用堆栈
            UpdateRender render = Director.GetInstance().updateRender;
            render.VsmReference = Director.RunMana.CallStack;
            // 恢复背景音乐
            render.Bgm(Director.RunMana.Musics.PlayingBGM, GlobalConfigContext.GAME_SOUND_BGMVOL);
            // 清空字符串缓冲
            render.dialogPreStr = String.Empty;
            render.pendingDialogQueue.Clear();
            // 弹空全部等待，复现保存最后一个动作
            Director.RunMana.ExitUserWait();
            Interrupt reactionNtr = new Interrupt()
            {
                Type = InterruptType.LoadReaction,
                Detail = "Reaction for load data",
                InterruptSA = Director.RunMana.DashingPureSa,
                InterruptFuncSign = String.Empty,
                ReturnTarget = null,
                PureInterrupt = true
            };
            // 提交中断
            Director.RunMana.CallStack.Submit(reactionNtr);
            // 重启并行
            var sc = ResourceManager.GetInstance().GetScene(Director.RunMana.CallStack.EBP.BindingSceneName);
            Director.RunMana.ParallelExecutorStack = new Stack<List<ParallelExecutor>>();
            Director.RunMana.ConstructParallel(sc);
            Director.RunMana.RestartParallel();
            Director.RunMana.LastScenario = sc.Scenario;
            // 重启信号系统
            SemaphoreDispatcher.ReBinding(sc, Director.RunMana.SemaphoreBindings);
            // 重启消息循环
            Director.ResumeUpdateContext();
        }

        /// <summary>
        /// 刷新主渲染器的调用堆栈绑定
        /// </summary>
        public void RefreshMainRenderVMReference()
        {
            this.updateRender.VsmReference = Director.RunMana.CallStack;
        }

        /// <summary>
        /// 向运行时环境发出中断
        /// </summary>
        /// <param name="ntr">中断</param>
        public void SubmitInterrupt(StackMachine vsm, Interrupt ntr)
        {
            vsm.Submit(ntr);
        }

        /// <summary>
        /// 向运行时环境发出等待要求
        /// </summary>
        /// <param name="waitSpan">等待的时间间隔</param>
        public void SubmitWait(TimeSpan waitSpan)
        {
            Director.RunMana.Delay("Director", DateTime.Now, waitSpan);
        }

        /// <summary>
        /// 从屏幕移除按钮，用于按钮自我消除
        /// </summary>
        /// <param name="id">按钮id</param>
        public void RemoveButton(int id)
        {
            this.updateRender.Deletebutton(id);
        }

        /// <summary>
        /// 从屏幕上移除所有选择项，用户选择项按钮按下后的回调
        /// </summary>
        public void RemoveAllBranchButton()
        {
            this.updateRender.RemoveAllBranchButton();
        }
        #endregion

        #region 消息循环
        /// <summary>
        /// 处理消息循环
        /// </summary>
        private void UpdateContext(object sender, EventArgs e)
        {
            bool resumeFlag = true;
            this.timer.Stop();
            while (true)
            {
                // 取得调用堆栈顶部状态
                StackMachineState stackState = Director.RunMana.GameState(Director.RunMana.CallStack);
                switch (stackState)
                {
                    case StackMachineState.Interpreting:
                    case StackMachineState.FunctionCalling:
                        this.curState = GameState.Performing;
                        resumeFlag = false;
                        break;
                    case StackMachineState.WaitUser:
                        this.curState = GameState.WaitForUserInput;
                        resumeFlag = true;
                        break;
                    case StackMachineState.WaitAnimation:
                        this.curState = GameState.WaitAni;
                        resumeFlag = true;
                        break;
                    case StackMachineState.Await:
                        this.curState = GameState.Waiting;
                        resumeFlag = true;
                        break;
                    case StackMachineState.Interrupt:
                        this.curState = GameState.Interrupt;
                        resumeFlag = false;
                        break;
                    case StackMachineState.NOP:
                        this.curState = GameState.Exit;
                        resumeFlag = true;
                        break;
                }
                // 根据调用堆栈顶部更新系统
                switch (this.curState)
                {
                    // 等待状态
                    case GameState.Waiting:
                        // 计算已经等待的时间（这里，不考虑并行处理）
                        if (DateTime.Now - Director.RunMana.CallStack.ESP.TimeStamp >
                            Director.RunMana.CallStack.ESP.Delay)
                        {
                            Director.RunMana.ExitCall(Director.RunMana.CallStack);
                        }
                        break;
                    // 等待动画
                    case GameState.WaitAni:
                        if (SpriteAnimation.IsAnyAnimation == false && SCamera2D.IsAnyAnimation == false
                            && SCamera3D.IsAnyAnimation == false)
                        {
                            Director.RunMana.ExitCall(Director.RunMana.CallStack);
                        }
                        break;
                    // 等待用户操作
                    case GameState.WaitForUserInput:
                        break;
                    // 中断
                    case GameState.Interrupt:
                        var interruptSa = Director.RunMana.CallStack.ESP.IP;
                        var interruptExitPoint = Director.RunMana.CallStack.ESP.Tag;
                        // 退出中断
                        var pureInt = Director.RunMana.CallStack.ESP.BindingInterrupt.PureInterrupt;
                        var interruptFuncCalling = Director.RunMana.CallStack.ESP.BindingInterrupt.InterruptFuncSign;
                        var needExitWait = Director.RunMana.CallStack.ESP.BindingInterrupt.ExitWait;
                        Director.RunMana.ExitCall(Director.RunMana.CallStack);
                        // 处理中断优先动作
                        if (interruptSa != null)
                        {
                            var iterSa = interruptSa;
                            while (iterSa != null)
                            {
                                this.updateRender.Execute(interruptSa);
                                iterSa = iterSa.Next;
                            }
                        }
                        // 判断中断是否需要处理后续动作
                        if (pureInt)
                        {
                            break;
                        }
                        // 跳出所有用户等待
                        if (needExitWait || interruptExitPoint != String.Empty)
                        {
                            Director.RunMana.ExitUserWait();
                        }
                        // 处理跳转（与中断调用互斥）
                        if (interruptExitPoint != String.Empty)
                        {
                            RunnableYuriri curRunnable;
                            if (Director.RunMana.CallStack.EBP.BindingFunction == null)
                            {
                                curRunnable = this.resMana.GetScene(Director.RunMana.CallStack.EBP.BindingSceneName);
                            }
                            else
                            {
                                curRunnable = Director.RunMana.CallStack.EBP.BindingFunction;
                            }
                            if (!curRunnable.LabelDictionary.ContainsKey(interruptExitPoint))
                            {
                                LogUtils.LogLine(String.Format("Ignored Interrupt jump Instruction (target not exist): {0}",
                                        interruptExitPoint), "Director", LogLevel.Error);
                                break;
                            }
                            Director.RunMana.CallStack.EBP.MircoStep(curRunnable.LabelDictionary[interruptExitPoint]);
                        }
                        // 处理中断函数调用
                        else if (interruptFuncCalling != String.Empty)
                        {
                            var ifcItems = interruptFuncCalling.Split('(');
                            var funPureName = ifcItems[0];
                            var funParas = "(" + ifcItems[1];
                            this.FunctionCalling(funPureName, funParas, Director.RunMana.CallStack);
                        }
                        break;
                    // 演绎脚本
                    case GameState.Performing:
                        // 取下一动作
                        var nextInstruct = Director.RunMana.MoveNext(Director.RunMana.CallStack);
                        // 如果指令空了就立即迭代本次消息循环
                        if (nextInstruct == null)
                        {
                            this.timer.Start();
                            return;
                        }
                        // 处理影响调用堆栈的动作
                        if (nextInstruct.Type == SActionType.act_wait)
                        {
                            double waitMs = nextInstruct.ArgsDict.ContainsKey("time")
                                ? (double)PolishEvaluator.Evaluate(nextInstruct.ArgsDict["time"], Director.RunMana.CallStack)
                                : 0;
                            Director.RunMana.Delay(nextInstruct.NodeName, DateTime.Now,
                                TimeSpan.FromMilliseconds(waitMs));
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_waitani)
                        {
                            Director.RunMana.AnimateWait(nextInstruct.NodeName);
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_waituser)
                        {
                            Director.RunMana.UserWait("Director", nextInstruct.NodeName);
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_jump)
                        {
                            var jumpToScene = nextInstruct.ArgsDict["filename"];
                            var jumpToTarget = nextInstruct.ArgsDict["target"];
                            // 场景内跳转
                            if (jumpToScene == String.Empty)
                            {
                                if (stackState == StackMachineState.Interpreting)
                                {
                                    var currentScene = this.resMana.GetScene(Director.RunMana.CallStack.ESP.BindingSceneName);
                                    if (!currentScene.LabelDictionary.ContainsKey(jumpToTarget))
                                    {
                                        LogUtils.LogLine(
                                            String.Format("Ignored Jump Instruction (target not exist): {0}",
                                                jumpToTarget),
                                            "Director", LogLevel.Error);
                                        break;
                                    }
                                    Director.RunMana.CallStack.ESP.MircoStep(currentScene.LabelDictionary[jumpToTarget]);
                                }
                                else if (stackState == StackMachineState.FunctionCalling)
                                {
                                    var currentFunc = Director.RunMana.CallStack.ESP.BindingFunction;
                                    if (!currentFunc.LabelDictionary.ContainsKey(jumpToTarget))
                                    {
                                        LogUtils.LogLine(
                                            String.Format("Ignored Jump Instruction (target not exist): {0}",
                                                jumpToTarget),
                                            "Director", LogLevel.Error);
                                        break;
                                    }
                                    Director.RunMana.CallStack.ESP.MircoStep(currentFunc.LabelDictionary[jumpToTarget]);
                                }
                            }
                            // 跨场景跳转
                            else
                            {
                                var jumpScene = this.resMana.GetScene(jumpToScene);
                                if (jumpScene == null)
                                {
                                    LogUtils.LogLine(
                                        String.Format("Ignored Jump Instruction (scene not exist): {0}", jumpToScene),
                                        "Director", LogLevel.Error);
                                    break;
                                }
                                if (jumpToTarget != String.Empty && !jumpScene.LabelDictionary.ContainsKey(jumpToTarget))
                                {
                                    LogUtils.LogLine(
                                        String.Format("Ignored Jump Instruction (target not exist): {0} -> {1}",
                                            jumpToScene, jumpToTarget),
                                        "Director", LogLevel.Error);
                                    break;
                                }
                                Director.RunMana.ExitCall(Director.RunMana.CallStack);
                                Director.RunMana.CallScene(jumpScene,
                                    jumpToTarget == String.Empty ? jumpScene.Ctor : jumpScene.LabelDictionary[jumpToTarget]);
                            }
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_call)
                        {
                            var callFunc = nextInstruct.ArgsDict["name"];
                            var signFunc = nextInstruct.ArgsDict["sign"];
                            this.FunctionCalling(callFunc, signFunc, Director.RunMana.CallStack);
                            break;
                        }
                        // 处理常规动作
                        this.updateRender.Execute(nextInstruct);
                        break;
                    // 退出
                    case GameState.Exit:
                        this.updateRender.Shutdown();
                        break;
                }
                // 处理IO
                this.updateRender.UpdateForMouseState();
                // 是否恢复消息循环
                if (resumeFlag)
                {
                    break;
                }
            }
            if (resumeFlag)
            {
                this.timer.Start();
            }
        }

        /// <summary>
        /// 处理并行调用和信号函数的消息循环
        /// </summary>
        private void ParallelUpdateContext(object sender, EventArgs e)
        {
            // 如果主消息循环暂停那么就直接迭代掉
            if (Director.IsContextUpdatePaused)
            {
                return;
            }
            // 获取绑定的调用堆栈
            var dispatcher = sender as DispatcherTimer;
            dispatcher.Stop();
            ParallelDispatcherArgsPackage pdap;
            try
            {
                pdap = dispatcher.Tag as ParallelDispatcherArgsPackage;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Parallel Failed " + ex, "Director", LogLevel.Warning);
                dispatcher.Start();
                return;
            }
            if (pdap == null)
            {
                LogUtils.LogLine("Parallel Failed.", "Director", LogLevel.Warning);
                dispatcher.Start();
                return;
            }
            var paraVM = pdap.IsSemaphore ? pdap.SemaphoreStack : Director.RunMana.ParallelExecutorStack.Peek()[pdap.Index].Executor;

            bool resumeFlag = true;
            while (true)
            {
                // 取得调用堆栈顶部状态
                StackMachineState stackState = Director.RunMana.GameState(paraVM);
                GameState paraGameState = GameState.Exit;
                switch (stackState)
                {
                    case StackMachineState.Interpreting:
                    case StackMachineState.FunctionCalling:
                        resumeFlag = false;
                        paraGameState = GameState.Performing;
                        break;
                    case StackMachineState.WaitUser:
                        dispatcher.Start();
                        return;
                    case StackMachineState.WaitAnimation:
                        // 并行器里不应该出现等待动画结束，立即结束本次迭代
                        return;
                    case StackMachineState.Await:
                        paraGameState = GameState.Waiting;
                        resumeFlag = true;
                        break;
                    case StackMachineState.Interrupt:
                        LogUtils.LogLine(
                            "There is a interrupt in parallel function, which may cause system pause",
                            "Director", LogLevel.Warning);
                        paraGameState = GameState.Interrupt;
                        resumeFlag = false;
                        break;
                    case StackMachineState.NOP:
                        paraGameState = GameState.Exit;
                        resumeFlag = true;
                        break;
                }
                // 根据调用堆栈顶部更新系统
                switch (paraGameState)
                {
                    // 等待状态
                    case GameState.Waiting:
                        // 计算已经等待的时间
                        if (DateTime.Now - paraVM.ESP.TimeStamp > paraVM.ESP.Delay)
                        {
                            paraVM.Consume();
                        }
                        break;
                    // 等待动画
                    case GameState.WaitAni:
                        if (SpriteAnimation.IsAnyAnimation == false)
                        {
                            paraVM.Consume();
                        }
                        break;
                    // 演绎脚本
                    case GameState.Performing:
                        // 取下一动作
                        var nextInstruct = Director.RunMana.MoveNext(paraVM);
                        // 如果指令空了就立即迭代本次消息循环
                        if (nextInstruct == null)
                        {
                            dispatcher.Start();
                            return;
                        }
                        // 处理影响调用堆栈的动作
                        if (nextInstruct.Type == SActionType.act_wait)
                        {
                            double waitMs = nextInstruct.ArgsDict.ContainsKey("time")
                                ? (double) PolishEvaluator.Evaluate(nextInstruct.ArgsDict["time"], paraVM)
                                : 0;
                            paraVM.Submit("Parallel Time Waiting", DateTime.Now, TimeSpan.FromMilliseconds(waitMs));
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_waitani)
                        {
                            // 并行器里不应该出现等待动画结束，立即结束本次迭代
                            LogUtils.LogLine(
                                "There is a animation wait in parallel function, which may cause system pause",
                                "Director", LogLevel.Warning);
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_waituser)
                        {
                            LogUtils.LogLine(
                                "There is a user wait in parallel function, which may cause system pause",
                                "Director", LogLevel.Warning);
                            paraVM.Submit("Director", nextInstruct.NodeName);
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_jump)
                        {
                            var jumpToScene = nextInstruct.ArgsDict["filename"];
                            var jumpToTarget = nextInstruct.ArgsDict["target"];
                            // 场景内跳转
                            if (jumpToScene == String.Empty)
                            {
                                if (stackState == StackMachineState.Interpreting)
                                {
                                    var currentScene = this.resMana.GetScene(paraVM.ESP.BindingSceneName);
                                    if (!currentScene.LabelDictionary.ContainsKey(jumpToTarget))
                                    {
                                        LogUtils.LogLine(
                                            String.Format("Ignored Jump Instruction (target not exist): {0}",
                                                jumpToTarget),
                                            "Director", LogLevel.Error);
                                        break;
                                    }
                                    paraVM.ESP.MircoStep(currentScene.LabelDictionary[jumpToTarget]);
                                }
                                else if (stackState == StackMachineState.FunctionCalling)
                                {
                                    var currentFunc = paraVM.ESP.BindingFunction;
                                    if (!currentFunc.LabelDictionary.ContainsKey(jumpToTarget))
                                    {
                                        LogUtils.LogLine(
                                            String.Format("Ignored Jump Instruction (target not exist): {0}",
                                                jumpToTarget),
                                            "Director", LogLevel.Error);
                                        break;
                                    }
                                    paraVM.ESP.MircoStep(currentFunc.LabelDictionary[jumpToTarget]);
                                }
                            }
                            // 跨场景跳转
                            else
                            {
                                LogUtils.LogLine(
                                    "There is a jump across scene in parallel function, it will be ignored",
                                    "Director", LogLevel.Warning);
                            }
                            break;
                        }
                        else if (nextInstruct.Type == SActionType.act_call)
                        {
                            var callFunc = nextInstruct.ArgsDict["name"];
                            var signFunc = nextInstruct.ArgsDict["sign"];
                            this.FunctionCalling(callFunc, signFunc, paraVM);
                            break;
                        }
                        // 处理常规动作
                        pdap.Render.Execute(nextInstruct);
                        break;
                    // 系统中断
                    case GameState.Interrupt:
                        var interruptSa = paraVM.ESP.IP;
                        var interruptExitPoint = paraVM.ESP.Tag;
                        // 退出中断
                        var pureInt = paraVM.ESP.BindingInterrupt.PureInterrupt;
                        var interruptFuncCalling = paraVM.ESP.BindingInterrupt.InterruptFuncSign;
                        var needExitWait = paraVM.ESP.BindingInterrupt.ExitWait;
                        Director.RunMana.ExitCall(paraVM);
                        // 处理中断优先动作
                        if (interruptSa != null)
                        {
                            var iterSa = interruptSa;
                            while (iterSa != null)
                            {
                                pdap.Render.Execute(interruptSa);
                                iterSa = iterSa.Next;
                            }
                        }
                        // 判断中断是否需要处理后续动作
                        if (pureInt)
                        {
                            break;
                        }
                        // 跳出所有用户等待
                        if (needExitWait || interruptExitPoint != String.Empty)
                        {
                            while (paraVM.Count() > 0 && paraVM.ESP.State == StackMachineState.WaitUser)
                            {
                                Director.RunMana.ExitCall(paraVM);
                            }
                        }
                        // 处理跳转（与中断调用互斥）
                        if (interruptExitPoint != String.Empty)
                        {
                            RunnableYuriri curRunnable;
                            if (paraVM.EBP.BindingFunction == null)
                            {
                                curRunnable = this.resMana.GetScene(paraVM.EBP.BindingSceneName);
                            }
                            else
                            {
                                curRunnable = paraVM.EBP.BindingFunction;
                            }
                            if (!curRunnable.LabelDictionary.ContainsKey(interruptExitPoint))
                            {
                                LogUtils.LogLine(String.Format("Ignored parallel Interrupt jump Instruction (target not exist): {0}",
                                    interruptExitPoint), "Director", LogLevel.Error);
                                break;
                            }
                            paraVM.EBP.MircoStep(curRunnable.LabelDictionary[interruptExitPoint]);
                        }
                        // 处理中断函数调用
                        else if (interruptFuncCalling != String.Empty)
                        {
                            var ifcItems = interruptFuncCalling.Split('(');
                            var funPureName = ifcItems[0];
                            var funParas = "(" + ifcItems[1];
                            this.FunctionCalling(funPureName, funParas, paraVM);
                        }
                        break;
                    // 退出（其实就是执行完毕了一轮，应该重新开始）
                    case GameState.Exit:
                        if (pdap.IsSemaphore == false)
                        {
                            paraVM.Submit(pdap.BindingSF, new List<object>());
                        }
                        else
                        {
                            switch (pdap.SemaphoreType)
                            {
                                case SemaphoreHandlerType.ScheduleOnce:
                                    return;
                                case SemaphoreHandlerType.ScheduleForever:
                                    paraVM.Submit(pdap.BindingSF, new List<object>());
                                    break;
                                case SemaphoreHandlerType.ScheduleWhenActivated:
                                    throw new NotImplementedException();
                            }
                        }
                        break;
                }
                if (resumeFlag) { break; }
            }
            if (resumeFlag)
            {
                dispatcher.Start();
            }
        }

        /// <summary>
        /// 处理调用堆栈上的函数调用
        /// </summary>
        /// <param name="callFunc">函数的全局名字</param>
        /// <param name="signFunc">参数签名</param>
        /// <param name="vsm">关于哪个虚拟机做动作</param>
        public void FunctionCalling(string callFunc, string signFunc, StackMachine vsm)
        {
            if (signFunc != String.Empty && (!signFunc.StartsWith("(") || !signFunc.EndsWith(")")))
            {
                LogUtils.LogLine(String.Format("Ignored Function calling (sign not valid): {0} -> {1}", callFunc, signFunc),
                    "Director", LogLevel.Error);
                return;
            }
            var callFuncItems = callFunc.Split('@');
            List<SceneFunction> sceneFuncContainer;
            IEnumerable<SceneFunction> sceneFuncList;
            if (callFuncItems.Length > 1)
            {
                sceneFuncContainer = this.resMana.GetScene(callFuncItems[1]).FuncContainer;
                sceneFuncList = from f in sceneFuncContainer where f.Callname == callFuncItems[0] select f;
            }
            else
            {
                sceneFuncContainer = this.resMana.GetScene(vsm.ESP.BindingSceneName).FuncContainer;
                sceneFuncList = from f in sceneFuncContainer where f.Callname == callFunc select f;
                LogUtils.LogLine(String.Format("Function calling for current Scene (Scene not explicit): {0}", callFunc),
                    "Director", LogLevel.Warning);
            }
            if (!sceneFuncList.Any())
            {
                LogUtils.LogLine(String.Format("Ignored Function calling (function not exist): {0}", callFunc),
                    "Director", LogLevel.Error);
                return;
            }
            var sceneFunc = sceneFuncList.First();
            var signItem = signFunc.Replace("(", String.Empty).Replace(")", String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sceneFunc.Param.Count != signItem.Length)
            {
                LogUtils.LogLine(String.Format("Ignored Function calling (in {0}, require args num: {1}, but actual:{2})", callFunc, sceneFunc.Param.Count, signItem.Length),
                    "Director", LogLevel.Error);
                return;
            }
            // 处理参数列表
            var argsVec = new List<object>();
            foreach (var s in signItem)
            {
                string trimedPara = s.Trim();
                object varref;
                if (trimedPara.StartsWith("$") || trimedPara.StartsWith("&"))
                {
                    varref = Director.RunMana.Fetch(trimedPara, vsm);
                }
                else if (trimedPara.StartsWith("\"") && trimedPara.EndsWith("\""))
                {
                    varref = trimedPara;
                }
                else
                {
                    varref = Convert.ToDouble(trimedPara);
                }
                argsVec.Add(varref);
            }
            Director.RunMana.CallFunction(sceneFunc, argsVec, vsm);
        }

        /// <summary>
        /// 暂停消息循环
        /// </summary>
        public static void PauseUpdateContext()
        {
            Director.IsContextUpdatePaused = true;
            Director.GetInstance().timer.Stop();
            LogUtils.LogLine("Context Update Dispatcher is stopped", "Director", LogLevel.Important);
        }

        /// <summary>
        /// 恢复消息循环
        /// </summary>
        public static void ResumeUpdateContext()
        {
            Director.GetInstance().timer.Start();
            Director.IsContextUpdatePaused = false;
            LogUtils.LogLine("Context Update Dispatcher is resumed", "Director", LogLevel.Important);
        }

        /// <summary>
        /// 获取当前是否正在呼叫右键菜单
        /// </summary>
        public static bool IsRClicking
        {
            get
            {
                var ebpFunc = Director.RunMana.CallStack.EBP.BindingFunction;
                return ebpFunc != null && ebpFunc.GlobalName == "__YuriFunc@rclick?main";
            }
        }

        /// <summary>
        /// 获取或设置当前是否正在点击按钮
        /// </summary>
        public static bool IsButtonClicking { get; set; } = false;
        
        /// <summary>
        /// 获取或设置当前是否处于全屏态
        /// </summary>
        public static bool IsFullScreen { get; set; } = false;

        /// <summary>
        /// 获取或设置主消息循环是否已经暂停
        /// </summary>
        public static bool IsContextUpdatePaused { get; set; } = true;

        /// <summary>
        /// 获取程序启动的时刻
        /// </summary>
        public static DateTime StartupTimeStamp { get; private set; }

        /// <summary>
        /// 获取程序在本次启动之前的累计使用时间
        /// </summary>
        public static TimeSpan LastGameTimeAcc { get; private set; }
        
        /// <summary>
        /// 获取程序在本次启动到目前为止的时间间隔
        /// </summary>
        public static TimeSpan CurrentTimeAcc => DateTime.Now - Director.StartupTimeStamp;
        
        /// <summary>
        /// 当前游戏的状态
        /// </summary>
        private GameState curState;
        #endregion

        #region 导演类自身资源相关
        /// <summary>
        /// 设置主舞台页面的引用
        /// </summary>
        /// <param name="sp">主舞台对象</param>
        public void SetStagePageReference(Page sp)
        {
            ViewPageManager.RegisterPage(GlobalConfigContext.FirstViewPage, sp);
            ViewPageManager.InitFirstPage(sp);
        }
        
        /// <summary>
        /// 在游戏即将结束时释放所有资源
        /// </summary>
        public static void CollapseWorld()
        {
            Director.IsCollapsing = true;
            var collaTimeStamp = DateTime.Now;
            LogUtils.LogLine("Yuri world began to collapse at " + collaTimeStamp, "Director", LogLevel.Important);
            PersistContextDAO.Assign("___YURIRI@LASTPLAYTIMESTAMP___", collaTimeStamp.ToString());
            PersistContextDAO.Assign("___YURIRI@ACCDURATION___", Director.LastGameTimeAcc + (collaTimeStamp - Director.StartupTimeStamp));
            PersistContextDAO.SaveToSteadyMemory();
            LogUtils.LogLine("Save persistence context OK", "Director", LogLevel.Important);
            MusicianThreadHandler.TerminalFlag = true;
            Musician.GetInstance().Dispose();
            LogUtils.LogLine("Dispose resource OK, program will shutdown soon", "Director", LogLevel.Important);
            Environment.Exit(0);
        }

        public static bool IsCollapsing { get; set; } = false;

        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>导演类唯一实例</returns>
        public static Director GetInstance()
        {
            return Director.synObject ?? (Director.synObject = new Director());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private Director()
        {
            LogUtils.LogLine("======================", "Director", LogLevel.Simple);
            LogUtils.LogLine("Game is launched", "Director", LogLevel.Normal);
            LogUtils.LogLine("CurrentDirectory is: " + Environment.CurrentDirectory, "Director", LogLevel.Simple);
            LogUtils.LogLine("BaseDirectory is: " + AppDomain.CurrentDomain.BaseDirectory, "Director", LogLevel.Simple);
            this.InitConfig();
            this.resMana = ResourceManager.GetInstance();
            Director.RunMana = new RuntimeManager();
            this.updateRender = new UpdateRender(Director.RunMana.CallStack);
            Director.RunMana.SetScreenManager(ScreenManager.GetInstance());
            Director.RunMana.ParallelHandler = this.ParallelUpdateContext;
            Director.RunMana.PerformingChapter = "Prelogue";
            MusicianThreadHandler.Init();
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromTicks((long) GlobalConfigContext.DirectorTimerInterval)
            };
            this.timer.Tick += this.UpdateContext;
#if NOTIME
#else
            this.InitRuntime();
#endif
        }

        /// <summary>
        /// 程序启动时的根目录
        /// </summary>
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 运行时环境
        /// </summary>
        public static RuntimeManager RunMana;

        /// <summary>
        /// 屏幕管理器
        /// </summary>
        public static ScreenManager ScrMana
        {
            get => Director.RunMana.Screen;
            set => Director.RunMana.Screen = value;
        }
        
        /// <summary>
        /// 获取主渲染器
        /// </summary>
        /// <returns>与主调用堆栈绑定的渲染器</returns>
        public UpdateRender GetMainRender() => this.updateRender;
        
        /// <summary>
        /// 消息循环计时器
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private readonly ResourceManager resMana;

        /// <summary>
        /// 画音渲染器
        /// </summary>
        private readonly UpdateRender updateRender;
        
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Director synObject = null;
        #endregion
    }
}
