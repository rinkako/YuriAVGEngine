using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using LyyneheymCore.SlyviaCore;

namespace Lyyneheym
{
    /// <summary>
    /// <para>导演类：管理整个游戏生命周期的类</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class Slyvia
    {





        bool testflag = false;

        public BitmapImage testBitmapImage(string filename)
        {
            return this.resourceMana.getBackgroundImage(filename);
        }

        public BitmapImage testCharaStand(string filename)
        {
            return this.resourceMana.getCharacterStandImage(filename);
        }

        public BitmapImage testBGM(string filename)
        {
            return this.resourceMana.getCharacterStandImage(filename);
        }





        public static Slyvia getInstance()
        {
            return null == synObject ? synObject = new Slyvia() : synObject;
        }

        private Slyvia()
        {
            this.resourceMana = ResourceManager.getInstance();


            // ================== DEBUG ==================
            Dictionary<string, KeyValuePair<long, long>> dtt = new Dictionary<string, KeyValuePair<long, long>>();
            dtt.Add("bg2.png", new KeyValuePair<long, long>(754513, 1149339));
            resourceMana.resourceTable.Add(Consta.DevURI_PA_BACKGROUND, dtt);
            // ================== DEBUG ==================
        }

        private ResourceManager resourceMana = null;

        private static Slyvia synObject = null;
    }
}
