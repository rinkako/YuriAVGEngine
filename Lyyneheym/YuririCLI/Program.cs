using System;
using Yuri.YuriInterpreter;
using Yuri.Yuriri;

namespace Yuri.YuririCLI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                CLIAdapter.ParseArgs(args);
            }
            else
            {
                new CPMainForm().ShowDialog();
            }
        }
    }
}
