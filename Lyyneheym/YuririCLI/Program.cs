using System;
using System.Linq;

namespace Yuri.YuririCLI
{
    /// <summary>
    /// <para>YuririCLI：为Yuriri编译器提供CLI和GUI</para>
    /// <para>CLI命令行参数</para>
    /// <para>  -d:... 输入文件夹 （可选，默认根目录）</para>
    /// <para>  -o:... 输出文件名 （可选，默认根目录下生成main.sil）</para>
    /// <para>  -p:... 工程名字 （可选，默认值YuriProject）</para>
    /// <para>  -key:... 工程密钥 （可选，默认值yurayuri）</para>
    /// <para>  -e:[True/False] 是否加密 （可选，默认值True）</para>
    /// </summary>
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // CLI
            if (args.Length > 0)
            {
                CLIAdapter.ParseArgs(args);
                var beginStamp = DateTime.Now;
                CLIAdapter.BeginCompile(
                    Program.NullServer(CLIAdapter.Get("p"), "YuriProject"),
                    Program.ParseURItoURL(Program.NullServer(CLIAdapter.Get("d"), String.Empty)),
                    Program.ParseURItoURL(Program.NullServer(CLIAdapter.Get("o"), "main.sil")),
                    Program.NullServer(CLIAdapter.Get("key"), "yurayuri"),
                    Program.NullServer(CLIAdapter.Get("e"), "true").ToLower() == "true"
                );
                Console.WriteLine();
                Console.WriteLine(@"Time Cost: {0} ms", (DateTime.Now - beginStamp).TotalMilliseconds);
            }
            // GUI
            else
            {
                new CPMainForm().ShowDialog();
            }
        }

        /// <summary>
        /// 空串服务
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="ns">空串的替换串</param>
        /// <returns>转换结果</returns>
        public static string NullServer(string str, string ns)
        {
            return String.IsNullOrEmpty(str) ? ns : str;
        }

        /// <summary>
        /// 将程序目录下的相对路径转化为绝对路径
        /// </summary>
        /// <param name="uris">相对路径项目</param>
        /// <returns>绝对路径串</returns>
        public static string ParseURItoURL(params string[] uris)
        {
            return uris.Aggregate(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), (curl, uri) => curl + $"\\{uri}");
        }
    }
}
