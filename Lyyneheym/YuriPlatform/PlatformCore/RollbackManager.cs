using System;
using System.Collections.Generic;
using System.Linq;
using Yuri.PlatformCore.Audio;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.Graphic3D;
using Yuri.PlatformCore.Semaphore;
using Yuri.PlatformCore.VM;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 回滚类：控制系统向前回滚的动作
    /// </summary>
    internal static class RollbackManager
    {
        /// <summary>
        /// 系统向前进一个状态
        /// 她只有在对话推进和选择项出现时才被触发
        /// </summary>
        public static void SteadyForward(bool fromWheel, SceneAction saPtr, MusicianDescriptor mdescriptor)
        {
            // 回滚后返回，移栈并演绎
            if (fromWheel && RollbackManager.IsRollingBack)
            {
                // 取上一状态
                var recentStep = RollbackManager.backwardStack.Last();
                RollbackManager.backwardStack.RemoveLast();
                RollbackManager.forwardStack.AddLast(recentStep);
                // 重演绎
                RollbackManager.GotoSteadyState(recentStep);
            }
            // 非回滚状态时才重新构造
            else if (fromWheel == false)
            {
                // 构造当前状态的拷贝
                var vm = Director.RunMana.CallStack.Fork() as StackMachine;
                vm.SetMachineName(String.Format("Yuri!Forked?{0}?{1}", DateTime.Now.Ticks, rand.Next(0, int.MaxValue)));
                RollbackableSnapshot ssp = new RollbackableSnapshot()
                {
                    TimeStamp = DateTime.Now,
                    IsBranchingRefer = Director.GetInstance().GetMainRender().IsBranching,
                    MusicRef = ForkableState.DeepCopyBySerialization(mdescriptor),
                    ReactionRef = saPtr,
                    VMRef = vm,
                    globalDao = SymbolTable.GetInstance().GlobalCtxDao.Fork() as GlobalContextDAO,
                    sceneDao = SymbolTable.GetInstance().SceneCtxDao.Fork() as SceneContextDAO,
                    ScreenStateRef = ScreenManager.GetInstance().Fork() as ScreenManager,
                    SemaphoreDict = ForkableState.DeepCopyBySerialization(Director.RunMana.SemaphoreBindings)
                };
                // 如果栈中容量溢出就剔掉最早进入的那个
                if (RollbackManager.forwardStack.Count >= GlobalConfigContext.MaxRollbackStep)
                {
                    RollbackManager.forwardStack.RemoveFirst();
                }
                // 入栈
                RollbackManager.forwardStack.AddLast(ssp);
            }
        }

        /// <summary>
        /// 把系统回滚到上一个状态并演绎
        /// </summary>
        public static void SteadyBackward()
        {
            // 还有得回滚且不在动画时才滚
            if (RollbackManager.forwardStack.Count > 0 &&
                Director.RunMana.GameState(Director.RunMana.CallStack) != StackMachineState.WaitAnimation)
            {
                // 如果还未回滚过就要将自己先移除
                if (RollbackManager.IsRollingBack == false && RollbackManager.forwardStack.Count > 1)
                {
                    var selfStep = RollbackManager.forwardStack.Last();
                    RollbackManager.forwardStack.RemoveLast();
                    RollbackManager.backwardStack.AddLast(selfStep);
                    RollbackManager.IsRollingBack = true;
                }
                // 取上一状态
                if (RollbackManager.IsRollingBack && RollbackManager.forwardStack.Count > 0)
                {
                    var lastStep = RollbackManager.forwardStack.Last();
                    RollbackManager.forwardStack.RemoveLast();
                    RollbackManager.backwardStack.AddLast(lastStep);
                    // 重演绎
                    RollbackManager.GotoSteadyState(lastStep);
                    NotificationManager.SystemMessageNotify("正在回滚", 800);
                }
            }
        }
        
        /// <summary>
        /// 将系统跳转到指定的稳定状态
        /// </summary>
        /// <param name="ssp">要演绎的状态包装</param>
        public static void GotoSteadyState(RollbackableSnapshot ssp)
        {
            // 停止消息循环
            Director.PauseUpdateContext();
            // 结束全部动画
            SpriteAnimation.ClearAnimateWaitingDict();
            // 检查是否需要回滚当前的并行处理和信号绑定
            bool needRepara = false;
            if (ssp.VMRef.ESP.BindingSceneName != Director.RunMana.CallStack.SAVEP.BindingSceneName)
            {
                Director.RunMana.PauseParallel();
                needRepara = true;
            }
            // 退到SSP所描述的状态
            SymbolTable.GetInstance().SetDAO(ssp.sceneDao.Fork() as SceneContextDAO, ssp.globalDao.Fork() as GlobalContextDAO);
            ScreenManager.ResetSynObject(ssp.ScreenStateRef.Fork() as ScreenManager);
            Director.RunMana.ResetCallstackObject(ssp.VMRef.Fork() as StackMachine);
            Director.RunMana.Musics = ForkableState.DeepCopyBySerialization(ssp.MusicRef);
            Director.RunMana.DashingPureSa = ssp.ReactionRef.Clone(true);
            Director.RunMana.SemaphoreBindings = ForkableState.DeepCopyBySerialization(ssp.SemaphoreDict);
            Director.ScrMana = ScreenManager.GetInstance();
            // 刷新主渲染器上的堆栈绑定
            Director.GetInstance().RefreshMainRenderVMReference();
            Director.GetInstance().GetMainRender().IsBranching = ssp.IsBranchingRefer;
            // 重绘整个画面
            ViewManager.GetInstance().ReDraw();
            // 恢复背景音乐
            UpdateRender render = Director.GetInstance().GetMainRender();
            render.Bgm(Director.RunMana.Musics.PlayingBGM, GlobalConfigContext.GAME_SOUND_BGMVOL);
            // 清空字符串缓冲
            render.dialogPreStr = String.Empty;
            render.pendingDialogQueue.Clear();
            // 弹空全部等待，复现保存最后一个动作
            Director.RunMana.ExitUserWait();
            Interrupt reactionNtr = new Interrupt()
            {
                Type = InterruptType.LoadReaction,
                Detail = "Reaction for rollback",
                InterruptSA = ssp.ReactionRef,
                InterruptFuncSign = String.Empty,
                ReturnTarget = null,
                PureInterrupt = true
            };
            // 提交中断到主调用堆栈
            Director.RunMana.CallStack.Submit(reactionNtr);
            // 重启并行处理和信号系统
            if (needRepara)
            {
                var sc = ResourceManager.GetInstance().GetScene(ssp.VMRef.EBP.BindingSceneName);
                Director.RunMana.ConstructParallelForRollingBack(sc);
                Director.RunMana.BackTraceParallel();
                Director.RunMana.LastScenario = sc.Scenario;
                SemaphoreDispatcher.ReBinding(sc, Director.RunMana.SemaphoreBindings);
            }
            // 重启消息循环
            Director.ResumeUpdateContext();
        }

        /// <summary>
        /// 清除整个状态回滚栈
        /// </summary>
        public static void Clear()
        {
            RollbackManager.backwardStack.Clear();
            RollbackManager.forwardStack.Clear();
            Utils.LogUtils.LogLine("Rollback Manager already reset", "Rollback Manager", Utils.LogLevel.Important);
        }

        /// <summary>
        /// 获取当前系统是否正在回滚
        /// </summary>
        public static bool IsRollingBack
        {
            get
            {
                return RollbackManager.rollingFlag;
            }
            set
            {
                RollbackManager.rollingFlag = value;
                if (value == false)
                {
                    if (RollbackManager.backwardStack.Count > 0)
                    {
                        var b = RollbackManager.backwardStack.Last();
                        RollbackManager.forwardStack.AddLast(b);
                        RollbackManager.backwardStack.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 回滚标志变量
        /// </summary>
        private static bool rollingFlag = false;

        /// <summary>
        /// 随机数发生器
        /// </summary>
        private static readonly Random rand = new Random();

        /// <summary>
        /// 前进状态栈
        /// </summary>
        private static readonly LinkedList<RollbackableSnapshot> forwardStack = new LinkedList<RollbackableSnapshot>();

        /// <summary>
        /// 回滚状态栈
        /// </summary>
        private static readonly LinkedList<RollbackableSnapshot> backwardStack = new LinkedList<RollbackableSnapshot>();
    }
}
