using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using Yuri.PlatformCore.VM;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.Semaphore
{
    /// <summary>
    /// 信号分发处理调度器
    /// </summary>
    internal static class SemaphoreDispatcher
    {
        /// <summary>
        /// 调度一个信号处理机
        /// </summary>
        /// <param name="selphine">信号量</param>
        /// <param name="shandler">处理机</param>
        public static void Schedule(YuriSemaphore selphine, SemaphoreHandler shandler)
        {
            switch (shandler.Type)
            {
                case SemaphoreHandlerType.ScheduleOnce:
                    var handleFunc = selphine.Activated ? shandler.ActivateFunc : shandler.DeActivateFunc;
                    if (handleFunc == null) { return; }
                    ParallelExecutor pExec = new ParallelExecutor()
                    {
                        Scenario = handleFunc.ParentSceneName
                    };
                    DispatcherTimer dt = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromTicks((long) GlobalConfigContext.DirectorTimerInterval)
                    };
                    dt.Tick += Director.RunMana.ParallelHandler;
                    pExec.Dispatcher = dt;
                    var pvm = new StackMachine();
                    pvm.SetMachineName("SemaphoreVM#" + handleFunc.GlobalName);
                    pvm.Submit(handleFunc, new List<object>());
                    pExec.Executor = pvm;
                    ParallelDispatcherArgsPackage pdap = new ParallelDispatcherArgsPackage()
                    {
                        Index = -1,
                        Render = new UpdateRender(pvm),
                        BindingSF = handleFunc,
                        IsSemaphore = true,
                        SemaphoreStack = pvm
                    };
                    dt.Tag = pdap;
                    shandler.Dispatcher = dt;
                    dt.Start();
                    break;
                case SemaphoreHandlerType.ScheduleForever:
                    throw new NotImplementedException();
                case SemaphoreHandlerType.ScheduleWhenActivated:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 为当前场景注册信号调度服务
        /// </summary>
        /// <param name="semaphoreName">信号名</param>
        /// <param name="activator">激活函数</param>
        /// <param name="deactivator">反激活函数</param>
        /// <param name="tag">信号附加值</param>
        /// <param name="groupName">信号分组名</param>
        public static void RegisterSemaphoreService(string semaphoreName, SceneFunction activator, SceneFunction deactivator, object tag = null, string groupName = "")
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName) == false)
                {
                    CommonUtils.ConsoleLine("semaphore not exist for binding to " + activator?.GlobalName + ", " + deactivator?.GlobalName,
                        "SemaphoreDispatcher", OutputStyle.Error);
                    return;
                }
                var hdObject = new SemaphoreHandler(groupName, tag)
                {
                    ActivateFunc = activator,
                    DeActivateFunc = deactivator,
                    Type = SemaphoreHandlerType.ScheduleOnce
                };
                SemaphoreDispatcher.semaphoreDict[semaphoreName].Attach(hdObject);
                SemaphoreDispatcher.handlerList.Add(new KeyValuePair<string, SemaphoreHandler>(semaphoreName, hdObject));
            }
        }

        /// <summary>
        /// 注销信号调度服务
        /// </summary>
        public static void UnregisterSemaphoreService()
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                foreach (var kvp in SemaphoreDispatcher.handlerList)
                {
                    SemaphoreDispatcher.semaphoreDict[kvp.Key].Detach(kvp.Value);
                }
                SemaphoreDispatcher.handlerList.Clear();
            }
        }

        /// <summary>
        /// 添加一个信号量
        /// </summary>
        /// <param name="semaphoreName">信号量的名字</param>
        /// <param name="initActivated">初始化时是否处于激活状态</param>
        /// <param name="tag">附加值</param>
        public static void SetSemaphore(string semaphoreName, bool initActivated = false, object tag = null)
        {
            SemaphoreDispatcher.semaphoreDict[semaphoreName] = new YuriSemaphore(semaphoreName, initActivated, tag);
        }

        /// <summary>
        /// 移除一个信号量
        /// </summary>
        /// <param name="semaphoreName">信号量的名字</param>
        public static void RemoveSemaphore(string semaphoreName)
        {
            SemaphoreDispatcher.semaphoreDict.Remove(semaphoreName);
        }

        /// <summary>
        /// 激活一个命名信号量，如果信号量不存在，将被初始化并激活
        /// </summary>
        /// <param name="semaphoreName">信号的名字</param>
        /// <param name="tag">信号的Tag</param>
        public static void Activate(string semaphoreName, object tag = null)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName) == false)
                {
                    SemaphoreDispatcher.semaphoreDict[semaphoreName] = new YuriSemaphore(semaphoreName, false, tag);
                }
                SemaphoreDispatcher.semaphoreDict[semaphoreName].Activated = true;
            }
        }

        /// <summary>
        /// 熄灭一个命名信号量
        /// </summary>
        /// <param name="semaphoreName">信号的名字</param>
        public static void Deactivate(string semaphoreName)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName))
                {
                    SemaphoreDispatcher.semaphoreDict[semaphoreName].Activated = false;
                }
            }
        }

        /// <summary>
        /// 熄灭所有的信号量
        /// </summary>
        public static void DeactivateAll()
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                foreach (var kvp in SemaphoreDispatcher.semaphoreDict)
                {
                    kvp.Value.Activated = false;
                }
            }
        }

        /// <summary>
        /// 停止分组中的所有信号处理机并销毁
        /// </summary>
        /// <param name="group">分组名</param>
        public static void AbortHandlerGroup(string group)
        {
            foreach (var kvp in SemaphoreDispatcher.handlerList)
            {
                if (kvp.Value.ObGroup.Equals(group))
                {
                    kvp.Value.Dispatcher.Stop();
                }
            }
            SemaphoreDispatcher.handlerList.RemoveAll(t => t.Value.ObGroup.Equals(group));
        }

        /// <summary>
        /// 停止所有的信号处理机并销毁
        /// </summary>
        public static void AbortHandlerAll()
        {
            foreach (var kvp in SemaphoreDispatcher.handlerList)
            {
                kvp.Value.Dispatcher.Stop();
            }
            SemaphoreDispatcher.handlerList.Clear();
        }

        /// <summary>
        /// 获取或设置是否启用信号调度服务
        /// </summary>
        public static bool EnableDispatcher { get; set; } = true;

        /// <summary>
        /// 信号处理机向量
        /// </summary>
        private static readonly List<KeyValuePair<string, SemaphoreHandler>> handlerList = new List<KeyValuePair<string, SemaphoreHandler>>();
        
        /// <summary>
        /// 信号量字典
        /// </summary>
        private static readonly Dictionary<string, YuriSemaphore> semaphoreDict = new Dictionary<string, YuriSemaphore>();

        /// <summary>
        /// 同步互斥量
        /// </summary>
        private static readonly Mutex syncMutex = new Mutex();
    }
}
