using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.ILPackage;

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
        public static void SteadyForward(SceneAction saPtr, string playingBGM)
        {
            // 回滚后返回，移栈并演绎
            if (RollbackManager.IsRollingBack)
            {
                // 取上一状态
                var recentStep = RollbackManager.backwardStack.Last();
                RollbackManager.backwardStack.RemoveAt(RollbackManager.backwardStack.Count - 1);
                RollbackManager.forwardStack.Add(recentStep);
                // 重演绎
                RollbackManager.GotoSteadyState(recentStep);
            }
            // 非回滚状态时才重新构造
            else
            {
                // 构造当前状态的拷贝
                StepStatePackage ssp = new StepStatePackage()
                {
                    TimeStamp = DateTime.Now,
                    MusicRef = playingBGM,
                    VMRef = Director.RunMana.CallStack.Clone() as StackMachine,
                    SymbolRef = SymbolTable.GetInstance().Clone() as SymbolTable,
                    ScreenStateRef = ScreenManager.GetInstance().Clone() as ScreenManager
                };
                // 如果栈中容量溢出就剔掉最早进入的那个
                if (RollbackManager.forwardStack.Count >= GlobalDataContainer.MaxRollbackStep)
                {
                    RollbackManager.forwardStack.RemoveAt(0);
                }
                // 入栈
                RollbackManager.forwardStack.Add(ssp);
            }
        }

        /// <summary>
        /// 把系统回滚到上一个状态并演绎
        /// </summary>
        /// <param name="rm">要作用的运行时环境</param>
        public static void SteadyBackward(RuntimeManager rm)
        {
            // 还有得回滚时才滚
            if (RollbackManager.forwardStack.Count > 0)
            {
                // 取上一状态
                var lastStep = RollbackManager.forwardStack.Last();
                RollbackManager.forwardStack.RemoveAt(RollbackManager.forwardStack.Count - 1);
                RollbackManager.backwardStack.Add(lastStep);
                // 重演绎
                RollbackManager.GotoSteadyState(lastStep);
            }
        }
        
        /// <summary>
        /// 重演绎稳定状态
        /// </summary>
        /// <param name="ssp">要演绎的状态包装</param>
        public static void GotoSteadyState(StepStatePackage ssp)
        {
            SymbolTable.ResetSynObject(ssp.SymbolRef);
            ScreenManager.ResetSynObject(ssp.ScreenStateRef);
            Director.RunMana.ResetCallstackObject(ssp.VMRef);
            Director.RunMana.PlayingBGM = ssp.MusicRef;
            // 重新演绎
            Director.GetInstance().RefreshMainRenderVMReference();
            // 重绘整个画面
            ViewManager.GetInstance().ReDraw();
            // 恢复背景音乐
            UpdateRender render = Director.GetInstance().updateRender;
            render.Bgm(Director.RunMana.PlayingBGM, GlobalDataContainer.GAME_SOUND_BGMVOL);
            // 清空字符串缓冲
            render.dialogPreStr = String.Empty;
            render.pendingDialogQueue.Clear();
            // 弹空全部等待，复现保存最后一个动作
            Director.RunMana.ExitUserWait();
            Interrupt reactionNtr = new Interrupt()
            {
                type = InterruptType.LoadReaction,
                detail = "Reaction for load data",
                interruptSA = Director.RunMana.DashingPureSa,
                interruptFuncSign = "",
                returnTarget = null,
                pureInterrupt = true
            };
            // 提交中断到主调用堆栈
            Director.RunMana.CallStack.Submit(reactionNtr);
        }

        /// <summary>
        /// 清除整个状态回滚栈
        /// </summary>
        public static void Clear()
        {
            RollbackManager.backwardStack.Clear();
            RollbackManager.forwardStack.Clear();
            Yuri.Utils.CommonUtils.ConsoleLine("Rollback Manager already reset",
                "Rollback Manager", Utils.OutputStyle.Important);
        }

        /// <summary>
        /// 获取当前系统是否正在回滚
        /// </summary>
        public static bool IsRollingBack
        {
            get
            {
                return RollbackManager.backwardStack.Count > 0;
            }
        }

        /// <summary>
        /// 前进状态栈
        /// </summary>
        private readonly static List<StepStatePackage> forwardStack = new List<StepStatePackage>();

        /// <summary>
        /// 回滚状态栈
        /// </summary>
        private readonly static List<StepStatePackage> backwardStack = new List<StepStatePackage>();
    }

    /// <summary>
    /// 提供系统回滚的状态包装类
    /// </summary>
    internal sealed class StepStatePackage
    {
        /// <summary>
        /// 调用堆栈的拷贝
        /// </summary>
        public StackMachine VMRef
        {
            get;
            set;
        }

        /// <summary>
        /// 屏幕状态的拷贝
        /// </summary>
        public ScreenManager ScreenStateRef
        {
            get;
            set;
        }

        /// <summary>
        /// 符号表的拷贝
        /// </summary>
        public SymbolTable SymbolRef
        {
            get;
            set;
        }

        /// <summary>
        /// 音乐状态的拷贝
        /// </summary>
        public string MusicRef
        {
            get;
            set;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp
        {
            get;
            set;
        }
    }
}
