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
            return this.ResMana.GetBackgroundImage(filename);
        }

        public MySprite testCharaStand(string filename)
        {
            return this.ResMana.GetCharacterStandImage(filename);
        }


        public void testBGM(string sourceName)
        {
            Musician m = Musician.getInstance();
            m.PlayBGM(this.ResMana.GetBGMMemoryStream(sourceName));

        }

        public void testVocal(string vocalName)
        {
            Musician m = Musician.getInstance();
            m.PlayVocal(this.ResMana.GetVocalMemoryStream(vocalName));
        }

        public void DisposeResource()
        {
            BassPlayer b = BassPlayer.GetInstance();
            b.Dispose();
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

            Dictionary<string, KeyValuePair<long, long>> bgm = new Dictionary<string, KeyValuePair<long, long>>();
            bgm.Add("车椅子の未来宇宙.mp3", new KeyValuePair<long, long>(0, 8705857));
            bgm.Add("Boss01.wav", new KeyValuePair<long, long>(8705857, 13446376));
            ResMana.resourceTable.Add(GlobalDataContainer.DevURI_SO_BGM, bgm);

            Dictionary<string, KeyValuePair<long, long>> vcc = new Dictionary<string, KeyValuePair<long, long>>();
            vcc.Add("Alice001.mp3", new KeyValuePair<long, long>(0, 87757));
            vcc.Add("Alice002.mp3", new KeyValuePair<long, long>(87757, 57052));
            vcc.Add("Alice003.mp3", new KeyValuePair<long, long>(144809, 381862));
            ResMana.resourceTable.Add(GlobalDataContainer.DevURI_SO_VOCAL, vcc);

            // ================== DEBUG ==================
        }



        private static Slyvia synObject = null;
    }
}
