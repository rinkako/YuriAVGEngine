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

        public UpdateRender GameUpdater;
        





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

        public MySprite testBitmapImage(string filename)
        {
            return this.ResMana.GetBackground(filename);
        }

        public MySprite testCharaStand(string filename)
        {
            return this.ResMana.GetCharacterStand(filename);
        }


        public void testBGM(string sourceName)
        {
            Musician m = Musician.getInstance();
            var r = this.ResMana.GetBGM(sourceName);
            m.PlayBGM(sourceName, r.Key, r.Value, 1000);

        }

        public void testVocal(string vocalName)
        {
            Musician m = Musician.getInstance();
            var r = this.ResMana.GetVocal(vocalName);
            m.PlayVocal(r.Key, r.Value, 1000);
        }

        public void DisposeResource()
        {
            BassPlayer.GetInstance().Dispose();
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


        }



        private static Slyvia synObject = null;
    }
}
