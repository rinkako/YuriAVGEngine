using System;
using System.Collections.Generic;
using System.Linq;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 栈机类：负责游戏流程的调度
    /// </summary>
    [Serializable]
    internal sealed class StackMachine : ForkableState
    {
        /// <summary>
        /// 构造函数：建立一个新的栈机
        /// </summary>
        public StackMachine()
        {
            this.Reset();
        }

        /// <summary>
        /// 为堆栈机指定一个名称
        /// </summary>
        /// <param name="vmName">堆栈的名称</param>
        public void SetMachineName(string vmName)
        {
            this.StackName = vmName;
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
                State = StackMachineState.Interpreting,
                ScriptName = sc.Scenario,
                PC = 0,
                IP = offset ?? sc.Ctor,
                IR = sc.Ctor?.NodeName,
                Argv = null,
                BindingSceneName = sc.Scenario,
                BindingFunctionName = null,
                Delay = TimeSpan.FromMilliseconds(0),
                Tag = String.Empty
            };
            this.coreStack.Push(smf);
            this.EBP = this.SAVEP = this.coreStack.Peek();
        }

        /// <summary>
        /// 向栈机提交一个函数调用
        /// </summary>
        /// <param name="sf">被调用函数的SceneFuction实例</param>
        /// <param name="args">实参数列表</param>
        /// <param name="offset">PC偏移量</param>
        public void Submit(SceneFunction sf, List<object> args, int offset = 0)
        {
            // 处理形参顺序
            string argc = sf.Param.Aggregate(String.Empty, (current, arg) => current + ("," + arg));
            if (argc.Length > 0)
            {
                argc = argc.Substring(1);
            }
            StackMachineFrame smf = new StackMachineFrame()
            {
                State = StackMachineState.FunctionCalling,
                ScriptName = sf.Callname,
                PC = offset,
                IP = sf.Ctor,
                IR = sf.Ctor.NodeName,
                BindingFunction = sf,
                Argv = args,
                BindingFunctionName = String.Format("{0}?{1}", sf.GlobalName, this.coreStack.Count.ToString()),
                BindingSceneName = sf.ParentSceneName,
                Delay = TimeSpan.FromMilliseconds(0),
                Tag = argc
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
                State = StackMachineState.Interrupt,
                ScriptName = null,
                PC = 0,
                IP = ntr.InterruptSA,
                IR = ntr.InterruptSA?.NodeName,
                Argv = null,
                BindingFunctionName = null,
                BindingSceneName = null,
                Delay = TimeSpan.FromMilliseconds(0),
                Tag = ntr.ReturnTarget,
                BindingInterrupt = ntr
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
                State = StackMachineState.Await,
                ScriptName = null,
                PC = 0,
                IP = null,
                IR = String.Empty,
                Argv = null,
                BindingFunctionName = null,
                BindingSceneName = null,
                TimeStamp = begin,
                Delay = sleepTimeSpan,
                Tag = String.Format("ThreadSleepCausedBy:{0}({1} ms)", causeBy, sleepTimeSpan.Milliseconds)
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
                State = StackMachineState.WaitUser,
                ScriptName = null,
                PC = 0,
                IP = null,
                IR = String.Empty,
                Argv = null,
                BindingFunctionName = null,
                BindingSceneName = null,
                Delay = TimeSpan.FromMilliseconds(0),
                Tag = String.Format("WaitingFor:{0}#Detail:{1}", causeBy, detail)
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
                State = StackMachineState.WaitAnimation,
                ScriptName = null,
                PC = 0,
                IP = null,
                IR = String.Empty,
                Argv = null,
                BindingFunctionName = null,
                BindingSceneName = null,
                Delay = TimeSpan.FromMilliseconds(0),
                Tag = String.Format("WaitingAnimationFor:{0}", causeBy)
            };
            this.coreStack.Push(smf);
        }

        /// <summary>
        /// 向栈机提交一个自动播放等待
        /// </summary>
        /// <param name="autoWaitPending">自动播放延时</param>
        public void Submit(TimeSpan autoWaitPending)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                State = StackMachineState.AutoWait,
                ScriptName = null,
                PC = 0,
                IP = null,
                IR = String.Empty,
                Argv = null,
                BindingFunctionName = null,
                BindingSceneName = null,
                Delay = autoWaitPending,
                Tag = null
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
            if (this.ESP.State == StackMachineState.Interpreting)
            {
                this.SAVEP = null;
            }
            return this.coreStack.Pop();
        }

        /// <summary>
        /// 弹空堆栈
        /// </summary>
        public void Clear()
        {
            while (this.coreStack.Count != 0)
            {
                this.Consume();
            }
        }

        /// <summary>
        /// 返回调用栈中的项目计数
        /// </summary>
        /// <returns>调用栈计数</returns>
        public int Count() => this.coreStack.Count;

        /// <summary>
        /// 栈顶指针
        /// </summary>
        public StackMachineFrame ESP => this.coreStack.Count > 0 ? this.coreStack.Peek() : null;

        /// <summary>
        /// 中断或等待前指针
        /// </summary>
        public StackMachineFrame EBP
        {
            get
            {
                if (this.ebp != null && this.coreStack.Contains(this.ebp) &&
                    (this.ebp.State == StackMachineState.Interpreting || this.ebp.State == StackMachineState.FunctionCalling))
                {
                    return this.ebp;
                }
                return this.ebp = this.coreStack.First(xp => xp.State == StackMachineState.Interpreting || xp.State == StackMachineState.FunctionCalling);
                //Stack<StackMachineFrame> ebpSerachStack = new Stack<StackMachineFrame>();
                //StackMachineFrame xebp = this.ESP;
                //while (xebp.State != StackMachineState.Interpreting && xebp.State != StackMachineState.FunctionCalling)
                //{
                //    ebpSerachStack.Push(this.coreStack.Pop());
                //    xebp = this.coreStack.Peek();
                //}
                //this.ebp = xebp;
                //while (ebpSerachStack.Count > 0)
                //{
                //    this.coreStack.Push(ebpSerachStack.Pop());
                //}
                //return this.ebp;
            }
            private set
            {
                this.ebp = value;
            }
        }

        /// <summary>
        /// 场景指针
        /// </summary>
        public StackMachineFrame SAVEP
        {
            get;
            set;
        }

        /// <summary>
        /// 当前jump指令是否有效
        /// </summary>
        public bool IsAbleJMP => this.ESP.State != StackMachineState.FunctionCalling;

        /// <summary>
        /// 该堆栈的名字
        /// </summary>
        public string StackName
        {
            get;
            set;
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
