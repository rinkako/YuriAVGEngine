using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.SlyviaCore
{
    /// <summary>
    /// 运行时管理器：维护运行时的所有信息
    /// 她是一个单例类，只有唯一实例，并且可以序列化
    /// </summary>
    [Serializable]
    public class RuntimeManager
    {

        public static RuntimeManager getInstance()
        {
            return null == synObject ? synObject = new RuntimeManager() : synObject;
        }

        private RuntimeManager()
        {

        }

        private static RuntimeManager synObject = null;
    }
}
