using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Lyyneheym.LyyneheymCore.Utils;
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

        #region debug用
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
        #endregion

        /// <summary>
        /// 初始化游戏设置
        /// </summary>
        private void InitConfig()
        {

        }

        /// <summary>
        /// 初始化运行时环境，并指定脚本的入口
        /// </summary>
        private void InitRuntime()
        {
            this.RunMana.CallScene(this.ResMana.GetScene(GlobalDataContainer.Script_Main));
        }











        /// <summary>
        /// 提供由前端更新后台键盘按键信息的方法
        /// </summary>
        /// <param name="e">键盘事件</param>
        public void UpdateKeyboard(KeyEventArgs e)
        {
            this.updateRender.SetKeyboardStatus(e.Key, e.KeyStates);
            DebugUtils.ConsoleLine(String.Format("Keyboard event: {0} <- {1}", e.Key.ToString(), e.KeyStates.ToString()),
                "Director", OutputStyle.Normal);
        }

        /// <summary>
        /// 提供由前端更新后台鼠标按键信息的方法
        /// </summary>
        /// <param name="e">鼠标事件</param>
        public void UpdateMouse(MouseEventArgs e)
        {

        }



        /// <summary>
        /// 在游戏结束时释放所有资源
        /// </summary>
        public void DisposeResource()
        {
            DebugUtils.ConsoleLine(String.Format("Begin dispose resource"), "Director", OutputStyle.Important);
            BassPlayer.GetInstance().Dispose();
            DebugUtils.ConsoleLine(String.Format("Finished dispose resource, program will shutdown"), "Director", OutputStyle.Important);
        }

        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>导演类唯一实例</returns>
        public static Slyvia getInstance()
        {
            return null == synObject ? synObject = new Slyvia() : synObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private Slyvia()
        {
            this.ResMana = ResourceManager.getInstance();
            this.RunMana = new RuntimeManager();
            this.updateRender = new UpdateRender();
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(GlobalDataContainer.DirectorTimerInterval);
            this.timer.Tick += timer_Tick;
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.updateRender.GetKeyboardStatus(Key.Z) == KeyStates.Down ||
                this.updateRender.GetKeyboardStatus(Key.Z) == (KeyStates.Down | KeyStates.Toggled))
            {
                Musician.getInstance().PauseBGM();
            }
            if (this.updateRender.GetKeyboardStatus(Key.X) == KeyStates.Down ||
                this.updateRender.GetKeyboardStatus(Key.X) == (KeyStates.Down | KeyStates.Toggled))
            {
                Musician.getInstance().ResumeBGM();
            }
        }

        private DispatcherTimer timer;

        /// <summary>
        /// 运行时环境
        /// </summary>
        private RuntimeManager RunMana;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager ResMana;

        /// <summary>
        /// 画面刷新器
        /// </summary>
        public UpdateRender updateRender;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static Slyvia synObject = null;
    }
}
