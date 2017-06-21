using System;
using System.Collections.Generic;
using Yuri.PlatformCore.VM;

namespace Yuri.PlatformCore.Audio
{
    /// <summary>
    /// 游戏音频描述子
    /// </summary>
    [Serializable]
    internal class MusicianDescriptor : CloneableDescriptor
    {
        /// <summary>
        /// 获取或设置正在播放的BGM资源名
        /// </summary>
        public string PlayingBGM { get; set; } = String.Empty;

        /// <summary>
        /// 获取或设置正在播放的BGM的相对音量
        /// </summary>
        public float BGMVol { get; set; } = GlobalConfigContext.GAME_SOUND_BGMVOL;

        /// <summary>
        /// 获取或设置正在播放的BGS资源名向量
        /// </summary>
        public List<string> PlayingBGS { get; set; } = new List<string>();

        /// <summary>
        /// 获取或设置正在播放的BGS的相对音量向量
        /// </summary>
        public List<float> BGSVol { get; set; } = new List<float>();
    }
}
