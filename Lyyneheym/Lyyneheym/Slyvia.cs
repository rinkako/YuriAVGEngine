using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Lyyneheym.LyyneheymCore.SlyviaCore;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym
{
    /// <summary>
    /// <para>导演类：管理整个游戏生命周期的类</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class Slyvia
    {
        private RuntimeManager RunMana;

        private ResourceManager ResMana;

        private UpdateRender GameUpdater;
        





        public void testRef()
        {
            this.RunMana.Symbols.AddSymbolTable("testScript");
            StackMachineFrame smf = new StackMachineFrame()
            {
                scriptName = "testScript"
            };
            this.RunMana.CallStack.Submit(smf);
            this.RunMana.Assignment("myvar", "123");
        }















        //bool testflag = false;

        public BitmapImage testBitmapImage(string filename)
        {
            return this.ResMana.getBackgroundImage(filename);
        }

        public BitmapImage testCharaStand(string filename)
        {
            return this.ResMana.getCharacterStandImage(filename);
        }

        public BitmapImage testBGM(string filename)
        {
            return this.ResMana.getCharacterStandImage(filename);
        }





        public static Slyvia getInstance()
        {
            return null == synObject ? synObject = new Slyvia() : synObject;
        }

        private Slyvia()
        {
            this.ResMana = ResourceManager.getInstance();
            this.RunMana = RuntimeManager.getInstance();
            this.GameUpdater = new UpdateRender();


            // ================== DEBUG ==================
            Dictionary<string, KeyValuePair<long, long>> dtt = new Dictionary<string, KeyValuePair<long, long>>();
            dtt.Add("bg2.png", new KeyValuePair<long, long>(754513, 1149339));
            ResMana.resourceTable.Add(GlobalDataContainer.DevURI_PA_BACKGROUND, dtt);
            // ================== DEBUG ==================
        }



        private static Slyvia synObject = null;
    }
}
