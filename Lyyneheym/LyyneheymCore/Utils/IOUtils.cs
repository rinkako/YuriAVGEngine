using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.Utils
{
    /// <summary>
    /// 输入输出类：管理整个游戏环境的IO动作
    /// </summary>
    public class IOUtils
    {

        public static string parseURItoURL(string uri)
        {
            return Environment.CurrentDirectory + uri;
        }
    }
}
