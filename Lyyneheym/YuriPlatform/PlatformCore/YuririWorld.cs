using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using IronPython.Hosting;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using Yuri.Utils;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 为整个游戏运行时环境和来自外部脚本的过程调用提供服务
    /// </summary>
    internal static class YuririWorld
    {
        /// <summary>
        /// <para>执行一个Python脚本字符串，它拥有访问和修改引擎公共上下文的能力</para>
        /// <para>此方法使用互斥锁保证线程安全，因此同一时刻此方法只能被一个线程所执行</para>
        /// </summary>
        /// <param name="pyExpr">要执行的Python命令</param>
        /// <param name="ctx">上下文</param>
        /// <returns>执行的结果</returns>
        public static dynamic Execute(string pyExpr, EvaluatableContext ctx)
        {
            dynamic execResult = null;
            try
            {
                lock (YuririWorld.executionMutex)
                {
                    ScriptScope scope = YuririWorld.contextEngine.CreateScope();
                    foreach (var kvp in ctx.GetSymbols())
                    {
                        scope.SetVariable(kvp.Key, kvp.Value);
                    }
                    scope.SetVariable("_YuririReflector_", new YuririReflector());
                    ScriptSource script = YuririWorld.contextEngine.CreateScriptSourceFromString(pyExpr);
                    execResult = script.Execute(scope);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AsyncLogLine("Execute isolation python script failed. " + ex, "YuririWorld",
                    YuririWorld.consoleMutex, LogLevel.Error);
            }
            return execResult;
        }

        /// <summary>
        /// <para>执行一个Python脚本文件，它拥有访问和修改引擎公共上下文的能力</para>
        /// <para>此方法使用互斥锁保证线程安全，因此同一时刻此方法只能被一个线程所执行</para>
        /// </summary>
        /// <param name="pyPath">要执行的Python命令</param>
        /// <param name="ctx">上下文</param>
        /// <returns>执行的结果</returns>
        public static dynamic ExecuteFile(string pyPath, EvaluatableContext ctx)
        {
            dynamic execResult = null;
            try
            {
                lock (YuririWorld.executionMutex)
                {
                    ScriptScope scope = YuririWorld.contextEngine.CreateScope();
                    foreach (var kvp in ctx.GetSymbols())
                    {
                        scope.SetVariable(kvp.Key, kvp.Value);
                    }
                    scope.SetVariable("_YuririReflector_", new YuririReflector());
                    ScriptSource script = YuririWorld.contextEngine.CreateScriptSourceFromFile(IOUtils.ParseURItoURL(pyPath));
                    execResult = script.Execute(scope);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AsyncLogLine("Execute isolation python file failed. " + ex, "YuririWorld",
                    YuririWorld.consoleMutex, LogLevel.Error);
            }
            return execResult;
        }

        /// <summary>
        /// <para>为Python脚本字符串创建一个与其他脚本以及游戏运行时环境隔离性的上下文并执行它</para>
        /// </summary>
        /// <param name="pyExpr">要执行的Python命令</param>
        /// <param name="ctx">上下文</param>
        /// <returns>执行的结果</returns>
        public static dynamic ExecuteIsolation(string pyExpr, EvaluatableContext ctx)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                foreach (var kvp in ctx.GetSymbols())
                {
                    scope.SetVariable(kvp.Key, kvp.Value);
                }
                ScriptSource script = engine.CreateScriptSourceFromString(pyExpr);
                return script.Execute(scope);
            }
            catch (Exception ex)
            {
                LogUtils.AsyncLogLine("Execute isolation python script failed. " + ex, "YuririWorld",
                    YuririWorld.consoleMutex, LogLevel.Error);
            }
            return null;
        }

        /// <summary>
        /// <para>为Python脚本文件创建一个与其他脚本以及游戏运行时环境隔离性的上下文并执行它</para>
        /// </summary>
        /// <param name="pyPath">要执行的Python文件</param>
        /// <param name="ctx">上下文</param>
        /// <returns>执行的结果</returns>
        public static dynamic ExecuteFileIsolation(string pyPath, EvaluatableContext ctx)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                foreach (var kvp in ctx.GetSymbols())
                {
                    scope.SetVariable(kvp.Key, kvp.Value);
                }
                ScriptSource script = engine.CreateScriptSourceFromFile(IOUtils.ParseURItoURL(pyPath));
                return script.Execute(scope);
            }
            catch (Exception ex)
            {
                LogUtils.AsyncLogLine("Execute isolation python script failed. " + ex, "YuririWorld",
                    YuririWorld.consoleMutex, LogLevel.Error);
            }
            return null;
        }

        /// <summary>
        /// 涉及游戏上下文的执行引擎
        /// </summary>
        private static readonly ScriptEngine contextEngine = Python.CreateEngine();

        /// <summary>
        /// 涉及游戏上下文线程安全互斥量
        /// </summary>
        private static readonly Mutex executionMutex = new Mutex();

        /// <summary>
        /// 控制台线程安全互斥量
        /// </summary>
        private static readonly Mutex consoleMutex = new Mutex();

        /// <summary>
        /// 游戏引擎反射器：为外部Python脚本提供访问引擎上下文和方法的入口
        /// </summary>
        internal class YuririReflector
        {
            /// <summary>
            /// 反射执行静态方法
            /// </summary>
            /// <param name="typeName">类的名字</param>
            /// <param name="methodName">方法的名字</param>
            /// <param name="args">参数列表</param>
            /// <returns>执行的结果</returns>
            public object InvokeStatic(string typeName, string methodName, object[] args)
            {
                try
                {
                    Type typeIns = YuririReflector.YuriTypeVector.Find((ty) => String.Compare(ty.Name, typeName, true) == 0);
                    if (typeIns == null)
                    {
                        LogUtils.AsyncLogLine(String.Format("Invoke static by yuriri failed: {0}->{1} not exist.", typeName, methodName), 
                            "YuririWorld", YuririWorld.consoleMutex, LogLevel.Error);
                        return null;
                    }
                    MethodInfo methodIns = typeIns.GetMethod("Display", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] {}, null);
                    return methodIns.Invoke(null, args);
                }
                catch (Exception ex)
                {
                    LogUtils.AsyncLogLine(String.Format("Invoke static by yuriri failed: {0}->{1} , CLR: {2}", typeName, methodName, ex),
                            "YuririWorld", YuririWorld.consoleMutex, LogLevel.Error);
                    return null;
                }
            }

            /// <summary>
            /// 通过类的名字创建类的实例
            /// </summary>
            /// <param name="typeName">类的名字</param>
            /// <returns>类的实例</returns>
            public object CreateObject(string typeName)
            {
                try
                {
                    Type typeIns = YuririReflector.YuriTypeVector.Find((ty) => String.Compare(ty.Name, typeName, true) == 0);
                    if (typeIns != null)
                    {
                        return Activator.CreateInstance(typeIns);
                    }
                    LogUtils.AsyncLogLine(String.Format("Create object by yuriri failed: {0} not exist.", typeName),
                            "YuririWorld", YuririWorld.consoleMutex, LogLevel.Error);
                }
                catch (Exception ex)
                {
                    LogUtils.AsyncLogLine(String.Format("Create object by yuriri failed: {0} , CLR: {1}", typeName, ex),
                            "YuririWorld", YuririWorld.consoleMutex, LogLevel.Error);
                }
                return null;
            }

            /// <summary>
            /// 线程安全地输出内容
            /// </summary>
            /// <param name="msg">要输出的内容</param>
            public void ShowInMessageBox(string msg)
            {
                lock (YuririWorld.consoleMutex)
                {
                    Console.WriteLine(msg);
                    System.Windows.Forms.MessageBox.Show(msg);
                }
            }

            /// <summary>
            /// 静态构造器，初始化类型数组
            /// </summary>
            static YuririReflector()
            {
                YuririReflector.YuriTypeVector = Assembly.GetExecutingAssembly().GetTypes().ToList();
            }

            /// <summary>
            /// 类型向量，供反射时查找类型
            /// </summary>
            public static readonly List<Type> YuriTypeVector;
        }
    }
}
