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
        #region 辅助函数
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
            return Convert.ToDouble(this.runMana.CalculatePolish(polish));
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
            return (int)(Convert.ToDouble(this.runMana.CalculatePolish(polish)));
        }
        #endregion

        #region 键位按钮状态
        /// <summary>
        /// 获取键盘上某个按键当前状态
        /// </summary>
        public KeyStates GetKeyboardState(Key key)
        {
            if (UpdateRender.KS_KEY_Dict.ContainsKey(key) == false)
            {
                UpdateRender.KS_KEY_Dict.Add(key, KeyStates.None);
                return KeyStates.None;
            }
            return UpdateRender.KS_KEY_Dict[key];
        }

        /// <summary>
        /// 设置键盘上某个按键当前状态
        /// </summary>
        public void SetKeyboardState(Key key, KeyStates state)
        {
            UpdateRender.KS_KEY_Dict[key] = state;
        }

        /// <summary>
        /// 获取鼠标某个按钮的当前状态
        /// </summary>
        public MouseButtonState GetMouseButtonState(MouseButton key)
        {
            return UpdateRender.KS_MOUSE_Dict[key];
        }

        /// <summary>
        /// 设置鼠标某个按钮的当前状态
        /// </summary>
        public void SetMouseButtonState(MouseButton key, MouseButtonState state)
        {
            UpdateRender.KS_MOUSE_Dict[key] = state;
        }

        /// <summary>
        /// 获取鼠标滚轮滚过的距离
        /// </summary>
        public int GetMouseWheelDelta()
        {
            return UpdateRender.KS_MOUSE_WHEEL_DELTA;
        }

        /// <summary>
        /// 设置鼠标滚轮滚过的距离
        /// </summary>
        public void SetMouseWheelDelta(int delta)
        {
            UpdateRender.KS_MOUSE_WHEEL_DELTA = delta;
        }

        /// <summary>
        /// 鼠标滚轮差值
        /// </summary>
        public static int KS_MOUSE_WHEEL_DELTA = 0;

        /// <summary>
        /// 鼠标按钮状态字典
        /// </summary>
        private static Dictionary<MouseButton, MouseButtonState> KS_MOUSE_Dict = new Dictionary<MouseButton, MouseButtonState>();

        /// <summary>
        /// 键盘按钮状态字典
        /// </summary>
        private static Dictionary<Key, KeyStates> KS_KEY_Dict = new Dictionary<Key, KeyStates>();
        #endregion

        #region 周期性调用
        /// <summary>
        /// 导演类周期性调用的更新函数：根据鼠标状态更新游戏，它的优先级低于精灵按钮
        /// </summary>
        public void UpdateForMouseState()
        {
            // 按下了鼠标左键
            if (UpdateRender.KS_MOUSE_Dict[MouseButton.Left] == MouseButtonState.Pressed)
            {
                // 正在显示对话则向前推进一个趟
                if (this.IsShowingDialog)
                {
                    this.DrawDialogRunQueue();
                }
            }
            // 按下了鼠标右键
            if (UpdateRender.KS_MOUSE_Dict[MouseButton.Right] == MouseButtonState.Pressed)
            {
                // 正在显示对话则隐藏对话
                if (this.IsShowingDialog)
                {
                    if (this.viewMana.GetMessageLayer(0).displayBinding.Visibility == Visibility.Hidden)
                    {
                        
                    }
                    else
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 导演类周期性调用的更新函数：根据键盘状态更新游戏，它的优先级低于精灵按钮
        /// </summary>
        public void UpdateForKeyboardState()
        {
            
        }



        #endregion

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


        #region 文字层相关
        /// <summary>
        /// 把文字描绘到指定的文字层上
        /// </summary>
        /// <param name="msglayId">文字层ID</param>
        /// <param name="str">要描绘的字符串</param>
        public void DrawStringToMsgLayer(int msglayId, string str)
        {
            this.IsShowingDialog = true;
            string[] strRuns = this.DialogToRuns(str);
            foreach (string run in strRuns)
            {
                this.pendingDialogQueue.Enqueue(run);
            }
            // 主动调用一次显示
            this.DrawDialogRunQueue();
            this.runMana.UserWait("UpdateRender", "DialogWaitForClick");
        }

        /// <summary>
        /// 将对话队列里的一趟显示出来，如果显示完毕后队列已空则结束对话状态
        /// </summary>
        private void DrawDialogRunQueue()
        {
            if (this.pendingDialogQueue.Count != 0)
            {
                string currentRun = this.pendingDialogQueue.Dequeue();
                this.TypeWriter(this.dialogPreStr, currentRun, this.viewMana.GetMessageLayer(0).displayBinding, GlobalDataContainer.GAME_MSG_TYPING_DELAY);
                // 出队后判断是否已经完成全部趟的显示
                if (this.pendingDialogQueue.Count == 0)
                {
                    // 弹掉用户等待状态
                    this.runMana.ExitCall();
                    this.IsShowingDialog = false;
                }
                else
                {
                    this.dialogPreStr += currentRun;
                }
            }
        }

        /// <summary>
        /// 当前已经显示的文本内容
        /// </summary>
        private string dialogPreStr = String.Empty;

        /// <summary>
        /// 待显示的文本趟队列
        /// </summary>
        private Queue<string> pendingDialogQueue = new Queue<string>();

        /// <summary>
        /// 将文字直接描绘到文字层上而不等待
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="text">要描绘的字符串</param>
        private void DrawTextDirectly(int id, string text)
        {
            TextBlock t = this.viewMana.GetMessageLayer(id).displayBinding;
            t.Text = text;
        }

        /// <summary>
        /// 将文本处理转义并分割为趟
        /// </summary>
        /// <param name="dialogStr">要显示的文本</param>
        /// <returns>趟数组</returns>
        private string[] DialogToRuns(string dialogStr)
        {
            return dialogStr.Split(new string[] { "\\|" }, StringSplitOptions.None);
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
            List<int> removeList = new List<int>();
            foreach (var ani in this.MsgStoryboardDict)
            {
                if (ani.Value.GetCurrentTime() == ani.Value.Duration)
                {
                    removeList.Add(ani.Key);
                }
            }
            foreach (var ri in removeList)
            {
                this.MsgStoryboardDict.Remove(ri);
                // 只有主文字层需要作用小三角
                if (ri == 0)
                {
                    this.ShowMessageTria();
                    this.BeginMessageTriaUpDownAnimation();
                }
            }
        }

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
        /// 隐藏对话小三角
        /// </summary>
        public void HideMessageTria()
        {
            // 只有主文字层需要作用小三角
            if (this.currentMsgLayer == 0)
            {
                this.MainMsgTriangleSprite.displayBinding.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 显示对话小三角
        /// </summary>
        /// <param name="opacity">透明度</param>
        public void ShowMessageTria(double opacity = 1.0f)
        {
            this.MainMsgTriangleSprite.displayOpacity = opacity;
            this.MainMsgTriangleSprite.displayBinding.Visibility = Visibility.Visible;
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

        /// <summary>
        /// 当前正在操作的文字层
        /// </summary>
        private int currentMsgLayer = 0;

        /// <summary>
        /// 是否正在显示对话
        /// </summary>
        private bool IsShowingDialog = false;

        /// <summary>
        /// 等待点击信号量
        /// </summary>
        private bool MsgClickFlag = false;

        /// <summary>
        /// 主文字层背景精灵
        /// </summary>
        private MySprite MainMsgTriangleSprite;

        private Dictionary<int, Storyboard> MsgStoryboardDict = new Dictionary<int, Storyboard>();
        #endregion

        #region 渲染器类自身相关方法和引用
        /// <summary>
        /// 为更新器设置作用窗体
        /// </summary>
        /// <param name="mw">窗体引用</param>
        public void SetMainWindow(MainWindow mw)
        {
            if (mw != null)
            {
                this.view = mw;
                this.ViewLoadedInit();
            }
        }

        /// <summary>
        /// 在主视窗加载后的初始化动作
        /// </summary>
        private void ViewLoadedInit()
        {
            // 为视窗管理设置引用
            this.viewMana.SetMainWndReference(this.view);
            // 初始化小三角
            this.InitMsgLayerTria();
            // 初始化文本层
            this.viewMana.InitMessageLayer();
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
        #endregion

        #region 演绎函数
        /// <summary>
        /// 接受一个场景动作并演绎她
        /// </summary>
        /// <param name="action">场景动作实例</param>
        public void Accept(SceneAction action)
        {
            switch (action.aType)
            {
                case SActionType.act_bgm:
                    this.Bgm(action.argsDict["filename"], this.ParseDouble(action.argsDict["vol"], 1000));
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
            this.pendingDialog += dialogStr;
        }

        /// <summary>
        /// 演绎函数：结束本节对话并清空文字层
        /// </summary>
        private void DialogTerminator()
        {
            this.DrawStringToMsgLayer(this.currentMsgLayer, this.pendingDialog);
            this.pendingDialog = string.Empty;
        }

        /// <summary>
        /// 演绎函数：直接描绘文字
        /// </summary>
        private void Drawtext(int id, string text)
        {
            if (id != 0)
            {
                this.DrawTextDirectly(id, text);
            }
            else
            {
                DebugUtils.ConsoleLine(String.Format("Drawtext cannot apply on MessageLayer0 (Main MsgLayer): {0}", text), 
                    "UpdateRender", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 待显示的字符串
        /// </summary>
        private string pendingDialog = String.Empty;

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
        /// 演绎函数：显示背景
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
        /// 演绎函数：显示图片
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
        /// 演绎函数：移动图片
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
        /// 演绎函数：移除图片
        /// </summary>
        private void Deletepicture(int id, ResourceType rType)
        {
            this.viewMana.RemoveSprite(id, rType);
        }

        /// <summary>
        /// 演绎函数：显示立绘
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
        /// 演绎函数：显示立绘
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
        /// 演绎函数：移除立绘
        /// </summary>
        private void Deletecstand(CharacterStandType cst)
        {
            this.viewMana.RemoveSprite(Convert.ToInt32(cst), ResourceType.Stand);
        }

        /// <summary>
        /// 演绎函数：播放音效
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

        /// <summary>
        /// 演绎函数：选定要操作的文字层
        /// </summary>
        /// <param name="id">文字层ID</param>
        private void MsgLayer(int id)
        {
            this.currentMsgLayer = id;
        }

        private void MsgLayerOpt()
        {

        }
        #endregion

    }
}
