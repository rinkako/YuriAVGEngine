using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

using Lyyneheym.LyyneheymCore.Utils;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 渲染类：负责将场景动作转化为前端事物的类
    /// </summary>
    public class UpdateRender
    {
        /// <summary>
        /// 渲染类构造器
        /// </summary>
        public UpdateRender()
        {
            // 初始化鼠标键位
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Left, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Middle, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Right, MouseButtonState.Released);
        }
        


        /// <summary>
        /// 设置运行时环境引用
        /// </summary>
        /// <param name="rm">运行时环境</param>
        public void SetRuntimeManagerReference(RuntimeManager rm)
        {
            this.runMana = rm;
        }

        /// <summary>
        /// 调用运行时环境计算表达式
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <returns>表达式的值</returns>
        private object CalculatePolish(string polish)
        {
            return this.runMana.CalculatePolish(polish);
        }

        /// <summary>
        /// <para>将逆波兰式计算为等价的Double类型实例</para>
        /// <para>如果逆波兰式为空，则返回参数nullValue的值</para>
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <param name="nullValue">默认值</param>
        /// <returns>Double实例</returns>
        private double ParseDouble(string polish, double nullValue)
        {
            if (polish == "")
            {
                return nullValue;
            }
            return Convert.ToDouble(this.CalculatePolish(polish));
        }

        /// <summary>
        /// <para>将逆波兰式计算为等价的Int32类型实例</para>
        /// <para>如果逆波兰式为空，则返回参数nullValue的值</para>
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <param name="nullValue">默认值</param>
        /// <returns>Int32实例</returns>
        private int ParseInt(string polish, int nullValue)
        {
            if (polish == "")
            {
                return nullValue;
            }
            return (int)(Convert.ToDouble(this.CalculatePolish(polish)));
        }

        /// <summary>
        /// 接受一个场景动作并演绎她
        /// </summary>
        /// <param name="action">场景动作实例</param>
        public void Accept(SceneAction action)
        {
            switch (action.aType)
            {
                case SActionType.act_bgm:
                    this.Bgm(action.argsDict["filename"], Convert.ToDouble(this.CalculatePolish(action.argsDict["vol"])));
                    break;
                case SActionType.act_stopbgm:
                    this.Stopbgm();
                    break;
                case SActionType.act_var:
                    this.Var(action.argsDict["name"], action.argsDict["dash"]);
                    break;
                case SActionType.act_cstand:
                    this.Cstand(
                        this.ParseInt(action.argsDict["id"], 0),
                        String.Format("{0}_{1}.png", action.argsDict["name"], action.argsDict["face"]),
                        this.ParseDouble(action.argsDict["x"], 0),
                        this.ParseDouble(action.argsDict["y"], 0),
                        this.ParseDouble(action.argsDict["opacity"], 1),
                        this.ParseDouble(action.argsDict["xscale"], 1),
                        this.ParseDouble(action.argsDict["yscale"], 1),
                        this.ParseDouble(action.argsDict["ro"], 0),
                        action.argsDict["anchor"] == "" ? (action.argsDict["anchor"] == "center" ? SpriteAnchorType.Center : SpriteAnchorType.LeftTop) : SpriteAnchorType.Center,
                        new Int32Rect(0, 0, 0, 0));
                    break;
                case SActionType.act_a:
                    this.A(action.argsDict["name"],
                        this.ParseInt(action.argsDict["vid"], -1),
                        action.argsDict["face"],
                        action.argsDict["loc"]
                        );
                    break;
                    







            }
        }

        public KeyStates GetKeyboardState(Key key)
        {
            if (UpdateRender.KS_KEY_Dict.ContainsKey(key) == false)
            {
                UpdateRender.KS_KEY_Dict.Add(key, KeyStates.None);
                return KeyStates.None;
            }
            return UpdateRender.KS_KEY_Dict[key];
        }

        public void SetKeyboardState(Key key, KeyStates state)
        {
            UpdateRender.KS_KEY_Dict[key] = state;
        }

        public MouseButtonState GetMouseButtonState(MouseButton key)
        {
            return UpdateRender.KS_MOUSE_Dict[key];
        }

        public void SetMouseButtonState(MouseButton key, MouseButtonState state)
        {
            UpdateRender.KS_MOUSE_Dict[key] = state;
        }

        public int GetMouseWheelDelta()
        {
            return UpdateRender.KS_MOUSE_WHEEL_DELTA;
        }

        public void SetMouseWheelDelta(int delta)
        {
            UpdateRender.KS_MOUSE_WHEEL_DELTA = delta;
        }

        /// <summary>
        /// 处理游戏窗体的鼠标按下信息
        /// </summary>
        /// <param name="e"></param>
        public void WMouseDownEventHandler(MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (this.view.BO_MessageBoxLayer.Visibility == System.Windows.Visibility.Hidden)
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            
        }

        public void WMouseUpEventHandler(MouseButtonEventArgs e)
        {

        }





        #region 文字层相关
        /// <summary>
        /// 初始化文字小三角
        /// </summary>
        private void InitMsgLayerTria()
        {
            this.MainMsgTriangleSprite = ResourceManager.GetInstance().GetPicture(GlobalDataContainer.GAME_MESSAGELAYER_TRIA_FILENAME);
            Image TriaView = new Image();
            BitmapImage bmp = MainMsgTriangleSprite.myImage;
            this.MainMsgTriangleSprite.displayBinding = TriaView;
            TriaView.Width = bmp.PixelWidth;
            TriaView.Height = bmp.PixelHeight;
            TriaView.Source = bmp;
            TriaView.RenderTransform = new TranslateTransform();
            Canvas.SetLeft(TriaView, GlobalDataContainer.GAME_MESSAGELAYER_TRIA_X);
            Canvas.SetTop(TriaView, GlobalDataContainer.GAME_MESSAGELAYER_TRIA_Y);
            Canvas.SetZIndex(TriaView, GlobalDataContainer.GAME_Z_PICTURES - 1);
            this.view.BO_MainGrid.Children.Add(this.MainMsgTriangleSprite.displayBinding);
        }

        /// <summary>
        /// 在指定的文字层绑定控件上进行打字动画
        /// </summary>
        /// <param name="orgString">原字符串</param>
        /// <param name="appendString">要追加的字符串</param>
        /// <param name="msglayBinding">文字层的控件</param>
        /// <param name="wordTimeSpan">字符之间的打字时间间隔</param>
        private void TypeWriter(string orgString, string appendString, TextBlock msglayBinding, int wordTimeSpan)
        {
            this.HideMessageTria();
            Storyboard story = new Storyboard();
            story.FillBehavior = FillBehavior.HoldEnd;
            DiscreteStringKeyFrame discreteStringKeyFrame;
            StringAnimationUsingKeyFrames stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames();
            stringAnimationUsingKeyFrames.Duration = new Duration(TimeSpan.FromMilliseconds(wordTimeSpan * appendString.Length));
            string tmp = orgString;
            foreach (char c in appendString)
            {
                discreteStringKeyFrame = new DiscreteStringKeyFrame();
                discreteStringKeyFrame.KeyTime = KeyTime.Paced;
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }
            Storyboard.SetTarget(stringAnimationUsingKeyFrames, msglayBinding);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            story.Children.Add(stringAnimationUsingKeyFrames);
            story.Completed += new EventHandler(this.TypeWriterAnimationCompletedCallback);
            story.Begin(msglayBinding);
        }

        /// <summary>
        /// 打字动画完成回调
        /// </summary>
        private void TypeWriterAnimationCompletedCallback(object sender, EventArgs e)
        {
            this.ShowMessageTria();
            this.BeginMessageTriaUpDownAnimation();
        }

        /// <summary>
        /// 隐藏对话小三角
        /// </summary>
        public void HideMessageTria()
        {
            this.view.BO_MsgTria.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 显示对话小三角
        /// </summary>
        /// <param name="opacity">透明度</param>
        public void ShowMessageTria(double opacity = 1.0f)
        {
            this.view.BO_MsgTria.Opacity = opacity;
            this.view.BO_MsgTria.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 为对话小三角施加跳动动画
        /// </summary>
        public void BeginMessageTriaUpDownAnimation()
        {
            SpriteAnimation.UpDownRepeatAnimation(this.MainMsgTriangleSprite, TimeSpan.FromMilliseconds(500), 10, 0.8);
        }

        /// <summary>
        /// 设置小三角位置
        /// </summary>
        /// <param name="X">目标X坐标</param>
        /// <param name="Y">目标Y坐标</param>
        public void SetMessageTriaPosition(double X, double Y)
        {
            Canvas.SetLeft(this.view.BO_MsgTria, X);
            Canvas.SetTop(this.view.BO_MsgTria, Y);
        }

        private int currentMsgLayer = 0;

        private bool MsgClickFlag = false;
        #endregion

        /// <summary>
        /// 为更新器设置作用窗体
        /// </summary>
        /// <param name="mw">窗体引用</param>
        public void SetMainWindow(MainWindow mw)
        {
            this.view = mw;
            // 为视窗管理设置引用
            this.viewMana.SetMainWndReference(this.view);
            // 初始化小三角
            this.InitMsgLayerTria();
        }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        private MainWindow view = null;

        /// <summary>
        /// 运行时环境引用
        /// </summary>
        private RuntimeManager runMana = null;

        /// <summary>
        /// 音乐引擎
        /// </summary>
        private Musician musician = Musician.GetInstance();

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager resMana = ResourceManager.GetInstance();

        /// <summary>
        /// 屏幕管理器
        /// </summary>
        private ScreenManager scrMana = ScreenManager.GetInstance();

        /// <summary>
        /// 视窗管理器
        /// </summary>
        private ViewManager viewMana = ViewManager.GetInstance();

        #region 演绎函数
        /// <summary>
        /// 结束程序
        /// </summary>
        public void Shutdown()
        {
            DebugUtils.ConsoleLine("Shutdown is called", "UpdateRender", OutputStyle.Important);
            this.view.Close();
        }

        /// <summary>
        /// 跳过所有动画
        /// </summary>
        public void SkipAllAnimation()
        {
            SpriteAnimation.SkipAllAnimation();
        }

        /// <summary>
        /// 演绎函数：显示文本
        /// </summary>
        /// <param name="dialogStr">要显示的文本</param>
        private void Dialog(string dialogStr)
        {
            string[] strRun = dialogStr.Split(new string[] { "\\|" }, StringSplitOptions.None);
            string preStr = String.Empty, desStr = strRun[0];
            TextBlock textView = this.viewMana.GetMessageLayer(this.currentMsgLayer).displayBinding;
            if (textView == null) { return; }
            for (int i = 0; i < strRun.Length; i++)
            {
                this.TypeWriter(preStr, desStr, textView, 60);
                if (i == strRun.Length - 1) { break; }
                preStr = desStr;
                desStr = strRun[i + 1];
                DateTime beginTime = DateTime.Now;
                TimeSpan ts = TimeSpan.FromMilliseconds(GlobalDataContainer.DirectorTimerInterval);
                while (this.MsgClickFlag == false)
                {
                    // 一段时间就要刷新UI
                    if (DateTime.Now - beginTime > ts)
                    {
                        this.view.DoEvent();
                        beginTime = DateTime.Now;
                    }
                }
                this.MsgClickFlag = false;
            }
        }

        /// <summary>
        /// 演绎函数：结束本节对话并清空文字层
        /// </summary>
        private void DialogTerminator()
        {
            this.viewMana.GetMessageLayer(this.currentMsgLayer).displayBinding.Text = "";
        }

        /// <summary>
        /// 演绎函数：角色状态设置
        /// </summary>
        /// <param name="name">角色名字</param>
        /// <param name="vid">语音序号</param>
        /// <param name="face">立绘表情</param>
        /// <param name="locStr">立绘位置</param>
        private void A(string name, int vid, string face, string locStr)
        {
            if (face != "")
            {
                this.Cstand(-1, String.Format("{0}_{1}.png", name, face), locStr, 1, 1, 1, 0, SpriteAnchorType.Center, new Int32Rect(0, 0, 0, 0));
            }
            if (vid != -1)
            {
                this.Vocal(name, vid, this.musician.VocalDefaultVolume);
            }
        }

        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="x">图片X坐标</param>
        /// <param name="y">图片Y坐标</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="ro">图片角度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Background(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            this.scrMana.AddBackground(id, filename, x, y, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Background);
        }

        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="x">图片X坐标</param>
        /// <param name="y">图片Y坐标</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="ro">角度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Picture(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            this.scrMana.AddPicture(id, filename, x, y, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Pictures);
        }

        /// <summary>
        /// 移动图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="rType">资源类型</param>
        /// <param name="property">改变的属性</param>
        /// <param name="fromValue">起始值</param>
        /// <param name="toValue">目标值</param>
        /// <param name="acc">加速度</param>
        /// <param name="duration">完成所需时间</param>
        private void Move(int id, ResourceType rType, string property, double fromValue, double toValue, double acc, Duration duration)
        {
            MySprite actionSprite = this.viewMana.GetSprite(id, rType);
            SpriteDescriptor descriptor = this.scrMana.GetSpriteDescriptor(id, rType);
            if (actionSprite == null)
            {
                DebugUtils.ConsoleLine(String.Format("Ignored move (sprite is null): {0}, {1}", rType.ToString(), id),
                    "UpdateRender", OutputStyle.Warning);
                return;
            }
            switch (property)
            {
                case "x":
                    SpriteAnimation.XYMoveAnimation(actionSprite, duration, fromValue, toValue, actionSprite.displayY, actionSprite.displayY, acc, 0);
                    break;
                case "y":
                    SpriteAnimation.XYMoveAnimation(actionSprite, duration, actionSprite.displayX, actionSprite.displayX, fromValue, toValue, 0, acc);
                    break;
                case "o":
                case "opacity":
                    SpriteAnimation.OpacityAnimation(actionSprite, duration, fromValue, toValue, acc);
                    break;
                case "a":
                case "angle":
                    SpriteAnimation.RotateAnimation(actionSprite, duration, fromValue, toValue, acc);
                    break;
                case "s":
                case "scale":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, fromValue, toValue, fromValue, toValue, acc, acc);
                    break;
                case "sx":
                case "scalex":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, fromValue, toValue, descriptor.ScaleY, descriptor.ScaleY, acc, 0);
                    break;
                case "sy":
                case "scaley":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, descriptor.ScaleX, descriptor.ScaleX, fromValue, toValue, 0, acc);
                    break;
                default:
                    DebugUtils.ConsoleLine(String.Format("Move instruction without valid parameters: {0}", property),
                        "UpdateRender", OutputStyle.Warning);
                    break;
            }
        }

        /// <summary>
        /// 移除图片
        /// </summary>
        private void Deletepicture(int id, ResourceType rType)
        {
            this.viewMana.RemoveSprite(id, rType);
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="locationStr">立绘位置字符串</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="ro">角度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Cstand(int id, string filename, string locationStr, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            CharacterStandType cst;
            switch (locationStr)
            {
                case "l":
                case "left":
                    cst = CharacterStandType.Left;
                    if (id == -1) { id = 0; }
                    break;
                case "ml":
                case "midleft":
                    cst = CharacterStandType.MidLeft;
                    if (id == -1) { id = 1; }
                    break;
                case "mr":
                case "midright":
                    cst = CharacterStandType.MidRight;
                    if (id == -1) { id = 3; }
                    break;
                case "r":
                case "right":
                    cst = CharacterStandType.Right;
                    if (id == -1) { id = 4; }
                    break;
                default:
                    cst = CharacterStandType.Mid;
                    if (id == -1) { id = 2; }
                    break;
            }
            this.scrMana.AddCharacterStand(id, filename, cst, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Stand);
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="ro">角度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Cstand(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            this.scrMana.AddCharacterStand(id, filename, x, y, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Stand);
        }

        /// <summary>
        /// 移除立绘
        /// </summary>
        private void Deletecstand(CharacterStandType cst)
        {
            this.viewMana.RemoveSprite(Convert.ToInt32(cst), ResourceType.Stand);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        private void Se(string resourceName, float volume)
        {
            var seKVP = this.resMana.GetSE(resourceName);
            this.musician.PlaySE(seKVP.Key, seKVP.Value, volume);
        }

        /// <summary>
        /// 演绎函数：播放BGM，如果是同一个文件将不会重新播放
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        private void Bgm(string resourceName, double volume)
        {
            // 如果当前BGM就是此BGM就只调整音量
            if (this.musician.currentBGM != resourceName)
            {
                var bgmKVP = this.resMana.GetBGM(resourceName);
                this.musician.PlayBGM(resourceName, bgmKVP.Key, bgmKVP.Value, (float)volume);
            }
            else
            {
                this.musician.SetBGMVolume((float)volume);
            }
        }

        /// <summary>
        /// 演绎函数：停止BGM
        /// </summary>
        private void Stopbgm()
        {
            this.musician.StopAndReleaseBGM();
        }

        /// <summary>
        /// 演绎函数：播放Vocal，这个动作会截断正在播放的Vocal
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="vid">语音编号</param>
        /// <param name="volume">音量</param>
        private void Vocal(string name, int vid, float volume)
        {
            this.Vocal(String.Format("{0}_{1}{2}", name, vid, GlobalDataContainer.GAME_VOCAL_POSTFIX), volume);
        }

        /// <summary>
        /// 演绎函数：播放Vocal，这个动作会截断正在播放的Vocal
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        private void Vocal(string resourceName, float volume)
        {
            if (resourceName != "")
            {
                var vocalKVP = this.resMana.GetVocal(resourceName);
                this.musician.PlayVocal(vocalKVP.Key, vocalKVP.Value, volume);
            }
        }

        /// <summary>
        /// 演绎函数：停止语音
        /// </summary>
        private void Stopvocal()
        {
            this.musician.StopAndReleaseVocal();
        }

        private void Title()
        {

        }

        private void Menu()
        {

        }

        private void Save()
        {

        }

        private void Load()
        {

        }

        /// <summary>
        /// 变量操作
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="dashPolish">表达式的等价逆波兰式</param>
        private void Var(string varname, string dashPolish)
        {
            this.runMana.Assignment(varname, dashPolish);
        }

        private void Branch()
        {

        }

        private void Titlepoint()
        {

        }

        private void MsgLayer()
        {

        }

        private void MsgLayerOpt()
        {

        }
        #endregion



        private MySprite MainMsgTriangleSprite;


        #region 键位按钮状态
        public static int KS_MOUSE_WHEEL_DELTA = 0;
        private static Dictionary<MouseButton, MouseButtonState> KS_MOUSE_Dict = new Dictionary<MouseButton, MouseButtonState>();
        private static Dictionary<Key, KeyStates> KS_KEY_Dict = new Dictionary<Key, KeyStates>();
        #endregion
    }
}
