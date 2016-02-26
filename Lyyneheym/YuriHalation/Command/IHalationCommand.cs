using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.YuriHalation.Command
{
    /// <summary>
    /// Halation命令类
    /// </summary>
    internal interface IHalationCommand
    {
        /// <summary>
        /// 执行本命令
        /// </summary>
        void Dash();

        /// <summary>
        /// 撤销本命令
        /// </summary>
        void Undo();
    }
}
