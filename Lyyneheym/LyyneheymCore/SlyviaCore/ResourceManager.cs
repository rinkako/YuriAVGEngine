using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 资源管理器类：负责维护游戏的资源
    /// 她是一个单例类，只有唯一实例
    /// </summary>
    public class ResourceManager
    {

        public static ResourceManager getInstance()
        {
            return null == synObject ? synObject = new ResourceManager() : synObject;
        }

        private ResourceManager()
        {

        }

        private static ResourceManager synObject = null;
    }
}
