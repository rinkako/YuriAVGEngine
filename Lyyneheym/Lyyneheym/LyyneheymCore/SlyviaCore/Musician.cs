using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Media;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>音乐管理器类：负责游戏所有声效的维护和处理</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class Musician
    {
        public void PlayBGM(string filename)
        {
            
        }

        public void PlayBGS(string filename, int track = 0)
        {

        }

        public void PlaySE(string filename)
        {

        }

        public void PlayME(string filename)
        {

        }

        public void PlayVocal(string filename)
        {
            
        }

        private Queue<string> BGMQueue = null;

        private MediaPlayer PlayerBGM = null;

        private MediaPlayer PlayerME = null;

        private MediaPlayer PlayerVocal = null;

        private List<MediaPlayer> PlayerBGS = null;

        /// <summary>
        /// 工厂方法：获得音乐管理器类的唯一实例
        /// </summary>
        /// <returns>音乐管理器</returns>
        public static Musician getInstance()
        {
            return synObject == null ? synObject = new Musician() : synObject;
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Musician synObject = null;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Musician()
        {
            
        }
    }
}
