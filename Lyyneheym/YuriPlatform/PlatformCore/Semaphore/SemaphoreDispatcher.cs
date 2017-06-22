using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
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
            if (!SemaphoreDispatcher.EnableDispatcher) { return; }
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
        /// 注销现有的信号处理机并依照字典重绑定信号与处理机
        /// </summary>
        /// <param name="scene">当前场景</param>
        /// <param name="bindingDict">重绑定字典</param>
        public static void ReBinding(Scene scene, Dictionary<string, List<Tuple<string, string>>> bindingDict)
        {
            SemaphoreDispatcher.UnregisterSemaphoreService(true);
            foreach (var sema in bindingDict)
            {
                foreach (var tp in sema.Value)
                {
                    var activator = scene.FuncContainer.Find(t => t.Callname == tp.Item1);
                    var deactivator = scene.FuncContainer.Find(t => t.Callname == tp.Item2);
                    SemaphoreDispatcher.RegisterSemaphoreService(sema.Key, activator, deactivator, null, "", true);
                }
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
        /// <param name="isRebinding">是否为重绑定</param>
        public static void RegisterSemaphoreService(string semaphoreName, SceneFunction activator, SceneFunction deactivator, object tag = null, string groupName = "", bool isRebinding = false)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                semaphoreName = semaphoreName.ToUpper();
                if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName) == false)
                {
                    LogUtils.LogLine("semaphore not exist for binding to " + activator?.GlobalName + ", " + deactivator?.GlobalName,
                        "SemaphoreDispatcher", LogLevel.Error);
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
                if (isRebinding == false)
                {
                    if (Director.RunMana.SemaphoreBindings.ContainsKey(semaphoreName) == false)
                    {
                        Director.RunMana.SemaphoreBindings[semaphoreName] = new List<Tuple<string, string>>();
                    }
                    Director.RunMana.SemaphoreBindings[semaphoreName].Add(new Tuple<string, string>(
                        activator?.Callname, deactivator?.Callname));
                }
            }
        }

        /// <summary>
        /// 注销信号调度服务
        /// </summary>
        /// <param name="isRebinding">是否为重绑定</param>
        public static void UnregisterSemaphoreService(bool isRebinding = false)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                foreach (var kvp in SemaphoreDispatcher.handlerList)
                {
                    SemaphoreDispatcher.semaphoreDict[kvp.Key].Detach(kvp.Value);
                }
                SemaphoreDispatcher.handlerList.Clear();
                if (isRebinding == false)
                {
                    Director.RunMana.SemaphoreBindings.Clear();
                }
            }
        }

        /// <summary>
        /// 注册全局信号调度服务，它不会被存档所保存，必须在游戏开头做绑定动作
        /// </summary>
        /// <param name="semaphoreName">信号名</param>
        /// <param name="activator">激活函数</param>
        /// <param name="deactivator">反激活函数</param>
        /// <param name="tag">信号附加值</param>
        /// <param name="groupName">信号分组名</param>
        public static void RegisterGlobalSemaphoreService(string semaphoreName, SceneFunction activator, SceneFunction deactivator, object tag = null, string groupName = "")
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                semaphoreName = semaphoreName.ToUpper();
                if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName) == false)
                {
                    LogUtils.LogLine("global semaphore not exist for binding to " + activator?.GlobalName + ", " + deactivator?.GlobalName,
                        "SemaphoreDispatcher", LogLevel.Error);
                    return;
                }
                var hdObject = new SemaphoreHandler(groupName, tag)
                {
                    ActivateFunc = activator,
                    DeActivateFunc = deactivator,
                    Type = SemaphoreHandlerType.ScheduleOnce,
                    IsGlobal = true
                };
                SemaphoreDispatcher.semaphoreDict[semaphoreName].Attach(hdObject);
                SemaphoreDispatcher.globalHandlerList.Add(new KeyValuePair<string, SemaphoreHandler>(semaphoreName, hdObject));
            }
        }
        
        /// <summary>
        /// 注销全局某个信号调度服务
        /// </summary>
        /// <param name="semaphoreName">信号名</param>
        public static void UnregisterGlobalSemaphoreService(string semaphoreName)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                var pRmList = new List<KeyValuePair<string, SemaphoreHandler>>();
                foreach (var kvp in SemaphoreDispatcher.globalHandlerList)
                {
                    if (String.Compare(kvp.Key, semaphoreName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        SemaphoreDispatcher.semaphoreDict[kvp.Key].Detach(kvp.Value);
                        pRmList.Add(kvp);
                    }
                }
                foreach (var kvp in pRmList)
                {
                    SemaphoreDispatcher.globalHandlerList.Remove(kvp);
                }
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
            semaphoreName = semaphoreName.ToUpper();
            SemaphoreDispatcher.semaphoreDict[semaphoreName] = new YuriSemaphore(semaphoreName, initActivated, tag);
        }

        /// <summary>
        /// 移除一个信号量
        /// </summary>
        /// <param name="semaphoreName">信号量的名字</param>
        public static void RemoveSemaphore(string semaphoreName)
        {
            SemaphoreDispatcher.semaphoreDict.Remove(semaphoreName.ToUpper());
        }

        /// <summary>
        /// 获取信号量上绑定的处理机数量
        /// </summary>
        /// <param name="semaphoreName">信号量的名称</param>
        /// <returns>绑定在该信号量上的所有处理机的数量</returns>
        public static int CountBinding(string semaphoreName)
        {
            semaphoreName = semaphoreName.ToUpper();
            if (SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName))
            {
                return SemaphoreDispatcher.semaphoreDict[semaphoreName].CountObserver();
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 激活一个命名信号量，如果信号量不存在，将被初始化并激活
        /// </summary>
        /// <param name="semaphoreName">信号的名字</param>
        /// <param name="tag">信号的Tag</param>
        public static void Activate(string semaphoreName, object tag = null)
        {
            semaphoreName = semaphoreName.ToUpper();
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
                semaphoreName = semaphoreName.ToUpper();
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
        /// 获取一个信号量是否激活
        /// </summary>
        /// <param name="semaphoreName">信号量的名字</param>
        /// <returns>信号量是否激活，如果信号量不存在，则返回False</returns>
        public static bool GetSemaphoreState(string semaphoreName)
        {
            lock (SemaphoreDispatcher.syncMutex)
            {
                semaphoreName = semaphoreName.ToUpper();
                return SemaphoreDispatcher.semaphoreDict.ContainsKey(semaphoreName) 
                    && SemaphoreDispatcher.semaphoreDict[semaphoreName].Activated;
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
            SemaphoreDispatcher.handlerList.RemoveAll(t => String.Compare(group, t.Value.ObGroup, StringComparison.OrdinalIgnoreCase) == 0);
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
        /// 初始化系统默认信号量
        /// </summary>
        public static void InitSystemSemaphore()
        {
            // 键盘IO信号量
            foreach (var t in Enum.GetNames(typeof(Key)))
            {
                SemaphoreDispatcher.SetSemaphore($"System_Key_{t}");
            }
            // 鼠标IO信号量
            foreach (var t in Enum.GetNames(typeof(MouseButton)))
            {
                SemaphoreDispatcher.SetSemaphore($"System_Mouse_{t}");
            }
            // 按钮信号量
            for (int i = 0; i < GlobalConfigContext.GAME_BUTTON_COUNT; i++)
            {
                SemaphoreDispatcher.SetSemaphore($"System_Button_Over_{i}");
            }
            // 窗口关闭信号量
            SemaphoreDispatcher.SetSemaphore("System_PreviewShutdown");
        }

        /// <summary>
        /// 获取或设置是否启用信号调度服务
        /// </summary>
        public static bool EnableDispatcher { get; set; } = true;

        /// <summary>
        /// 场景信号处理机向量
        /// </summary>
        private static readonly List<KeyValuePair<string, SemaphoreHandler>> handlerList = new List<KeyValuePair<string, SemaphoreHandler>>();

        /// <summary>
        /// 全局信号处理机向量
        /// </summary>
        private static readonly List<KeyValuePair<string, SemaphoreHandler>> globalHandlerList = new List<KeyValuePair<string, SemaphoreHandler>>();

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
