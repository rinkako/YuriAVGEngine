using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.SlyviaCore
{
    /// <summary>
    /// 导演类：管理整个游戏环境的类
    /// 她是一个单例类，只有唯一实例
    /// </summary>
    public class Slyvia
    {





        public static Slyvia getInstance()
        {
            return null == synObject ? synObject = new Slyvia() : synObject;
        }

        private Slyvia()
        {

        }

        private static Slyvia synObject = null;
    }
}
