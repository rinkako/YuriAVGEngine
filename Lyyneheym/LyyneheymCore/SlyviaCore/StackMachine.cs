using System;
using System.Collections.Generic;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 栈机类：负责游戏流程的调度
    /// </summary>
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

        public StackMachineFrame Peek()
        {
            return this.coreStack.Peek();
        }

        public StackMachineFrame Pop()
        {
            return this.coreStack.Pop();
        }

        public int Count()
        {
            return this.coreStack.Count;
        }
        
        public void Submit(Scene sc, int offset = 0)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Interpreting,
                scriptName = sc.scenario,
                PC = offset,
                argv = null,
                bindingScene = sc,
                bindingFunction = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = ""
            };
            this.coreStack.Push(smf);
        }

        public void Submit(SceneFunction sf, List<object> args, int offset = 0)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Calling,
                scriptName = sf.callname,
                PC = offset,
                argv = args,
                bindingFunction = sf,
                bindingScene = null,
                delay = TimeSpan.FromMilliseconds(0),
                aTag = ""
            };
            this.coreStack.Push(smf);
        }

        public void Submit(string causeBy, TimeSpan sleepTimeSpan)
        {
            StackMachineFrame smf = new StackMachineFrame()
            {
                state = GameStackMachineState.Await,
                scriptName = null,
                PC = 0,
                argv = null,
                bindingFunction = null,
                bindingScene = null,
                delay = sleepTimeSpan,
                aTag = String.Format("Caused by: " + causeBy)
            };
            this.coreStack.Push(smf);
        }

        private Stack<StackMachineFrame> coreStack;
    }
}
