using System;
using System.Collections.Generic;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 栈机类：负责游戏流程的调度
    /// </summary>
    [Serializable]
    public class StackMachine
    {
        /// <summary>
        /// 构造函数：建立一个新的栈机
        /// </summary>
        public StackMachine()
        {
            this.Reset();
        }

        /// <summary>
        /// 将栈机恢复到初始状态
        /// </summary>
        public void Reset()
        {
            coreStack = new Stack<StackMachineFrame>();
        }

        /// <summary>
        /// 向栈机提交一个场景调用
        /// </summary>
        /// <param name="sc">被调用的场景Scene实例</param>
        /// <param name="offset">IP偏移</param>
        public void Submit(Scene sc, SceneAction offset = null)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Interpreting,
                scriptName = sc.scenario,
                PC = 0,
                IP = offset == null ? sc.mainSa : offset,
                argv = null,
                bindingSceneName = sc.scenario,
                bindingFunctionName = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = ""
            };
            this.coreStack.Push(smf);
            this.EBP = this.coreStack.Peek();
        }

        /// <summary>
        /// 向栈机提交一个函数调用
        /// </summary>
        /// <param name="sf">被调用函数的SceneFuction实例</param>
        /// <param name="args">实参数列表</param>
        /// <param name="offset">PC偏移量</param>
        public void Submit(SceneFunction sf, List<object> args, int offset = 0)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.FunctionCalling,
                scriptName = sf.callname,
                PC = offset,
                IP = sf.sa,
                argv = args,
                bindingFunctionName = sf.globalName,
                bindingSceneName = sf.globalName,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = ""
            };
            this.coreStack.Push(smf);
            this.EBP = this.coreStack.Peek();
        }

        /// <summary>
        /// 向栈机提交一个中断调用
        /// </summary>
        /// <param name="ntr">中断</param>
        public void Submit(Interrupt ntr)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Interrupt,
                scriptName = null,
                PC = 0,
                IP = ntr.interruptSA,
                argv = null,
                bindingFunctionName = null,
                bindingSceneName = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = ntr.returnTarget
            };
            this.coreStack.Push(smf);
        }

        /// <summary>
        /// 向栈机提交一个延时调用
        /// </summary>
        /// <param name="causeBy">发起延时的SA名</param>
        /// <param name="begin">开始计时的时刻</param>
        /// <param name="sleepTimeSpan">延时时间间隔</param>
        public void Submit(string causeBy, DateTime begin, TimeSpan sleepTimeSpan)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Await,
                scriptName = null,
                PC = 0,
                IP = null,
                argv = null,
                bindingFunctionName = null,
                bindingSceneName = null,
                timeStamp = begin,
                delay = sleepTimeSpan,
                aTag = String.Format("ThreadSleepCausedBy:{0}({1} ms)", causeBy, sleepTimeSpan.Milliseconds)
            };
            this.coreStack.Push(smf);
        }

        /// <summary>
        /// 向栈机提交一个等待用户的调用
        /// </summary>
        /// <param name="causeBy">发起等待的SA名</param>
        /// <param name="detail">备注</param>
        public void Submit(string causeBy, string detail)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.WaitUser,
                scriptName = null,
                PC = 0,
                IP = null,
                argv = null,
                bindingFunctionName = null,
                bindingSceneName = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = String.Format("WaitingFor:{0}#Detail:{1}", causeBy, detail)
            };
            this.coreStack.Push(smf);
        }

        /// <summary>
        /// 向栈机提交一个等待动画完成的调用
        /// </summary>
        /// <param name="causeBy">原因</param>
        public void Submit(string causeBy)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.WaitAnimation,
                scriptName = null,
                PC = 0,
                IP = null,
                argv = null,
                bindingFunctionName = null,
                bindingSceneName = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = String.Format("WaitingAnimationFor:{0}", causeBy)
            };
            this.coreStack.Push(smf);
        }

        /// <summary>
        /// 向栈机提交一个栈帧
        /// </summary>
        /// <param name="mySMF">自定义栈帧</param>
        public void Submit(StackMachineFrame mySMF)
        {
            this.coreStack.Push(mySMF);
        }

        /// <summary>
        /// 将调用栈栈顶取出
        /// </summary>
        /// <returns>调用栈帧</returns>
        public StackMachineFrame Consume()
        {
            return this.coreStack.Pop();
        }

        /// <summary>
        /// 返回调用栈中的项目计数
        /// </summary>
        /// <returns>调用栈计数</returns>
        public int Count()
        {
            return this.coreStack.Count;
        }

        /// <summary>
        /// 栈顶指针
        /// </summary>
        public StackMachineFrame ESP
        {
            get
            {
                if (this.coreStack.Count > 0)
                {
                    return this.coreStack.Peek();
                }
                return null;
            }
        }

        /// <summary>
        /// 中断前指针
        /// </summary>
        public StackMachineFrame EBP
        {
            get
            {
                if (this.ebp != null && this.coreStack.Contains(this.ebp))
                {
                    return this.ebp;
                }
                return this.ebp = this.ESP;
            }
            private set
            {
                this.ebp = value;
            }
        }

        /// <summary>
        /// 当前jump指令是否有效
        /// </summary>
        public bool isAbleJMP
        {
            get
            {
                return this.ESP.state != GameStackMachineState.FunctionCalling;
            }
        }

        /// <summary>
        /// 调用前指针
        /// </summary>
        private StackMachineFrame ebp = null;

        /// <summary>
        /// 调用堆栈
        /// </summary>
        private Stack<StackMachineFrame> coreStack = null;
    }
}
