using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using LyyneheymCore.SlyviaCore;

namespace Lyyneheym
{
    /// <summary>
    /// 导演类：管理整个游戏环境的类
    /// 她是一个单例类，只有唯一实例
    /// </summary>
    public class Slyvia
    {





        public BitmapImage testBitmapImage()
        {
            Dictionary<string, KeyValuePair<long, long>> dtt = new Dictionary<string, KeyValuePair<long, long>>();
            dtt.Add("bg2.png", new KeyValuePair<long, long>(754513, 1149339));
            resourceMana.resourceTable.Add(Consta.DevURI_PA_BACKGROUND, dtt);
            return this.resourceMana.getBackgroundImage("bg2.png");
        }

        public static Slyvia getInstance()
        {
            return null == synObject ? synObject = new Slyvia() : synObject;
        }

        private Slyvia()
        {
            this.resourceMana = ResourceManager.getInstance();
        }

        private ResourceManager resourceMana = null;

        private static Slyvia synObject = null;
    }
}
