using System;
using System.Collections.Generic;
using System.Text;
using Yuri.YuriInterpreter;
using Yuri.Yuriri;

namespace Yuri.YuririCLI
{
    internal static class CLIAdapter
    {
        public static void ParseArgs(string[] args)
        {
            foreach (var arg in args)
            {
                var argItem = arg.Split(':');
                if (argItem.Length == 2)
                {
                    CLIAdapter.ArgDict[argItem[0]] = argItem[1];
                }
            }
        }

        public static void BeginCompile(string projectName, string dir, string spath, string key = "yurayuri")
        {
            try
            {
                Interpreter pile = new Interpreter(projectName, dir, key);
                pile.Dash(YuriInterpreter.YuriILEnum.InterpreterType.RELEASE_WITH_IL);
                pile.GenerateIL(spath);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }

        public static string Get(string key)
        {
            return CLIAdapter.ArgDict.ContainsKey(key) ? CLIAdapter.ArgDict[key] : null;
        }

        public static readonly Dictionary<string, string> ArgDict = new Dictionary<string, string>();
    }
}
