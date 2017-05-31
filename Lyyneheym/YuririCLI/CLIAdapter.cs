using System;
using System.Collections.Generic;
using Yuri.YuriInterpreter;

namespace Yuri.YuririCLI
{
    /// <summary>
    /// 编译器适配器
    /// </summary>
    internal static class CLIAdapter
    {
        /// <summary>
        /// 将CLI参数列表匹配到字典
        /// </summary>
        /// <param name="args">参数列表</param>
        public static void ParseArgs(string[] args)
        {
            foreach (var arg in args)
            {
                var argItem = arg.Split(':');
                if (argItem.Length == 2)
                {
                    CLIAdapter.ArgDict[argItem[0].Substring(1)] = argItem[1];
                }
            }
        }

        /// <summary>
        /// 进行编译并生成IL文件
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="dir">输入文件夹</param>
        /// <param name="spath">输出文件</param>
        /// <param name="key">工程密钥</param>
        /// <param name="encrypted">是否加密</param>
        public static void BeginCompile(string projectName, string dir, string spath, string key = "yurayuri", bool encrypted = true)
        {
            try
            {
                CLIAdapter.Pile = new Interpreter(projectName, dir, key, encrypted);
                CLIAdapter.Pile.Dash(YuriInterpreter.YuriILEnum.InterpreterType.RELEASE_WITH_IL);
                CLIAdapter.Pile.GenerateIL(spath);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// 获取参数字典中的项
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>参数的值，不存在时为null</returns>
        public static string Get(string key)
        {
            return CLIAdapter.ArgDict.ContainsKey(key) ? CLIAdapter.ArgDict[key] : null;
        }

        /// <summary>
        /// 编译器接口
        /// </summary>
        public static Interpreter Pile;

        /// <summary>
        /// 参数字典
        /// </summary>
        private static readonly Dictionary<string, string> ArgDict = new Dictionary<string, string>();
    }
}
