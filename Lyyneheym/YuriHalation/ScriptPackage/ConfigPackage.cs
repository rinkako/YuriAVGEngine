using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 游戏设置类
    /// </summary>
    [Serializable]
    class ConfigPackage
    {
        /// <summary>
        /// 开关数量
        /// </summary>
        public int MaxSwitchCount = 100;

        /// <summary>
        /// 文字层数量
        /// </summary>
        public int MessageLayerCount = 10;

        /// <summary>
        /// 图片层数量
        /// </summary>
        public int PictureLayerCount = 50;

        /// <summary>
        /// 按钮层数量
        /// </summary>
        public int ButtonLayerCount = 20;

        /// <summary>
        /// BGS轨道数量
        /// </summary>
        public int BgsCount = 10;
    }
}
