using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using SlimDX.Windows;

namespace SlimSlyvia
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            try
            {
                GameCore gcore = GameCore.GetInstance();
                gcore.Init();
                gcore.LoadResourceTexture(gcore.myLoad("mytest.jpg"), "mytest");
                gcore.LoadResourceTexture(gcore.myLoad("MenuItems2.png"), "flowers", new Rectangle(187, 2, 226, 226));
                if (gcore.initSuccess)
                {
                    MessagePump.Run(gcore.mainForm, new MainLoop(gcore.Processor));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
