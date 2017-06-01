using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Yuri.PageView;
using Yuri.PlatformCore.Audio;
using Yuri.PlatformCore.Evaluator;
using Yuri.PlatformCore.Graphic;
using Yuri.PlatformCore.Graphic3D;
using Yuri.PlatformCore.Semaphore;
using Yuri.PlatformCore.VM;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 渲染类：将场景动作转化为前端画音
    /// </summary>
    internal class UpdateRender
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
            return polish == String.Empty
                ? nullValue
                : Convert.ToDouble(PolishEvaluator.Evaluate(polish, this.VsmReference));
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
            return polish == String.Empty
                ? nullValue
                : (int) (Convert.ToDouble(PolishEvaluator.Evaluate(polish, this.VsmReference)));
        }

        /// <summary>
        /// <para>将逆波兰式计算为等价的String类型实例</para>
        /// <para>如果逆波兰式为空，则返回参数nullValue的值</para>
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <param name="nullValue">默认值</param>
        /// <returns>String实例</returns>
        private string ParseString(string polish, string nullValue)
        {
            return polish == String.Empty
                ? nullValue
                : PolishEvaluator.Evaluate(polish, this.VsmReference).ToString();
        }

        /// <summary>
        /// <para>把逆波兰式直接看做字符串处理</para>
        /// <para>如果逆波兰式为空，则返回参数nullValue的值</para>
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <param name="nullValue">默认值</param>
        /// <returns>String实例</returns>
        private string ParseDirectString(string polish, string nullValue)
        {
            return polish == String.Empty? nullValue : polish;
        }
        #endregion

        #region 键位按钮状态
        /// <summary>
        /// 获取键盘上某个按键当前状态
        /// </summary>
        public KeyStates GetKeyboardState(Key key)
        {
            return UpdateRender.KS_KEY_Dict[key];
        }

        /// <summary>
        /// 设置键盘上某个按键当前状态
        /// </summary>
        /// <param name="e">事件对象</param>
        /// <param name="isDown">是否按下</param>
        public void SetKeyboardState(KeyEventArgs e, bool isDown)
        {
            UpdateRender.KS_KEY_Dict[e.Key] = isDown ? KeyStates.Down : KeyStates.None;
            // 触发更新事件
            this.UpdateForKeyboardState();
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
            if (state == MouseButtonState.Pressed)
            {
                SemaphoreDispatcher.Activate($"System_Mouse_{key}");
            }
            else
            {
                SemaphoreDispatcher.Deactivate($"System_Mouse_{key}");
            }
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
            // 更新变量
            UpdateRender.KS_MOUSE_WHEEL_DELTA = delta;
            // 上滚
            if (delta > 0 &&
                (!ViewManager.Is3DStage && !SCamera2D.IsAnyAnimation ||
                ViewManager.Is3DStage && !SCamera3D.IsAnyAnimation) &&
                !Director.IsRClicking)
            {
                this.Stopvocal();
                RollbackManager.SteadyBackward();
            }
            // 下滚
            else if (GlobalConfigContext.GAME_SCROLLINGMODE != 0)
            {
                //RollbackManager.SteadyForward(true, null, null);
                this.ForwardToNextSteadyState();
            }
        }

        /// <summary>
        /// 鼠标滚轮差值
        /// </summary>
        public static int KS_MOUSE_WHEEL_DELTA = 0;

        /// <summary>
        /// 鼠标按钮状态字典
        /// </summary>
        private static readonly Dictionary<MouseButton, MouseButtonState> KS_MOUSE_Dict = new Dictionary<MouseButton, MouseButtonState>();

        /// <summary>
        /// 键盘按钮状态字典
        /// </summary>
        private static readonly Dictionary<Key, KeyStates> KS_KEY_Dict = new Dictionary<Key, KeyStates>();
        #endregion

        #region 周期性调用
        /// <summary>
        /// 更新函数：根据鼠标状态更新游戏，它的优先级低于精灵按钮
        /// </summary>
        public void UpdateForMouseState()
        {
            // 按下了鼠标左键
            if (UpdateRender.KS_MOUSE_Dict[MouseButton.Left] == MouseButtonState.Pressed)
            {
                // 松开按键之后
                if (this.MouseLeftUpFlag)
                {
                    this.ForwardToNextSteadyState();
                }
                // 连续按压生效的情况下
                else
                {

                }
                // 保持按下的状态
                this.MouseLeftUpFlag = false;
            }
            // 松开了鼠标左键
            else
            {
                this.MouseLeftUpFlag = true;
            }
            // 按下了鼠标右键
            if (UpdateRender.KS_MOUSE_Dict[MouseButton.Right] == MouseButtonState.Pressed)
            {
                // 要松开才生效的情况下
                if (this.MouseRightUpFlag)
                {
                    // 可以右键的情况
                    if ((this.IsShowingDialog || this.IsBranching) && Director.RunMana.EnableRClick)
                    {
                        var mainMsgLayer = this.viewMana.GetMessageLayer(0).DisplayBinding;
                        switch (this.RclickCounter)
                        {
                            // 隐藏对话框
                            case 0:
                                this.SaveSnapshot();
                                mainMsgLayer.Visibility = Visibility.Hidden;
                                MainMsgTriangleSprite.DisplayBinding.Visibility = Visibility.Hidden;
                                break;
                            // 呼叫菜单
                            case 1:
                                if (this.IsBranching)
                                {
                                    this.SaveSnapshot();
                                    this.viewMana.DisableBranchButtonHitTest();
                                }
                                Director.GetInstance().FunctionCalling("rclick@main", "()", this.VsmReference);
                                break;
                            // 退出菜单并恢复对话框
                            case 2:
                                if (Director.RunMana.GetRclickingState())
                                {
                                    var rfunc = this.VsmReference.EBP.BindingFunction;
                                    if (rfunc.LabelDictionary.ContainsKey(GlobalConfigContext.DeConstructorName))
                                    {
                                        // 跳转到析构标签
                                        this.VsmReference.EBP.IP = this.VsmReference.EBP.BindingFunction.LabelDictionary[GlobalConfigContext.DeConstructorName];
                                        Director.RunMana.ExitUserWait();
                                    }
                                    else
                                    {
                                        while (this.VsmReference.ESP != this.VsmReference.EBP)
                                        {
                                            Director.RunMana.ExitCall(this.VsmReference);
                                        }
                                        Director.RunMana.ExitCall(this.VsmReference);
                                    }
                                }
                                if (this.IsShowingDialog)
                                {
                                    mainMsgLayer.Visibility = Visibility.Visible;
                                    MainMsgTriangleSprite.DisplayBinding.Visibility = Visibility.Visible;
                                }
                                else if (this.IsBranching)
                                {
                                    this.viewMana.EnableBranchButtonHitTest();
                                }
                                break;
                        }
                        if (++this.RclickCounter >= 3)
                        {
                            this.RclickCounter = this.IsBranching || GlobalConfigContext.GAME_RCLICKMODE == GlobalConfigContext.RClickType.RClickMenu ? 1 : 0;
                        }
                    }
                }
                // 连续按压生效的情况下
                else
                {

                }
                // 保持按下的状态
                this.MouseRightUpFlag = false;
            }
            else
            {
                this.MouseRightUpFlag = true;
            }
        }

        /// <summary>
        /// 鼠标右键计数器
        /// </summary>
        public int RclickCounter { get; set; } = GlobalConfigContext.GAME_RCLICKMODE == GlobalConfigContext.RClickType.RClickMenu ? 1 : 0;

        /// <summary>
        /// 更新函数：根据键盘状态更新游戏，它的优先级低于精灵按钮
        /// </summary>
        public void UpdateForKeyboardState()
        {
            if ((this.IsShowingDialog || this.IsBranching) && ViewPageManager.IsAtMainStage())
            {
                if (UpdateRender.KS_KEY_Dict[Key.S] == KeyStates.Down)
                {
                    Canvas mainCanvas = ViewManager.Is3DStage
                        ? ViewManager.View3D.BO_MainGrid
                        : ViewManager.View2D.BO_MainGrid;
                    ViewManager.RenderFrameworkElementToJPEG(mainCanvas,
                        IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg"));
                    SLPage p = (SLPage) ViewPageManager.RetrievePage("SavePage");
                    p.ReLoadFileInfo();
                    ViewPageManager.NavigateTo("SavePage");
                }
                if (UpdateRender.KS_KEY_Dict[Key.L] == KeyStates.Down)
                {
                    SLPage p = (SLPage) ViewPageManager.RetrievePage("LoadPage");
                    p.ReLoadFileInfo();
                    ViewPageManager.NavigateTo("LoadPage");
                }
                // quick save
                if (UpdateRender.KS_KEY_Dict[Key.F2] == KeyStates.Down)
                {
                    try
                    {
                        Director.PauseUpdateContext();
                        this.ActualSave(GlobalConfigContext.QSaveFileName);
                        Director.ResumeUpdateContext();
                        NotificationManager.SystemMessageNotify("Quick Saved.", 1000);
                    }
                    catch (Exception ex)
                    {
                        CommonUtils.ConsoleLine("Quick save failed. " + ex, "UpdateRender", OutputStyle.Error);
                        return;
                    }
                }
                // quick load
                if (UpdateRender.KS_KEY_Dict[Key.F3] == KeyStates.Down)
                {
                    try
                    {
                        var url = IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + @"\" +
                                                        GlobalConfigContext.QSaveFileName + GlobalConfigContext.GAME_SAVE_POSTFIX);
                        if (File.Exists(url))
                        {
                            this.Stopvocal();
                            Director.PauseUpdateContext();
                            this.ActualLoad(GlobalConfigContext.QSaveFileName);
                            NotificationManager.SystemMessageNotify("Quick Loaded.", 1000);
                        }
                        else
                        {
                            MessageBox.Show("当前没有快速存档可以读取");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonUtils.ConsoleLine("Quick load failed. " + ex, "UpdateRender", OutputStyle.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 推进一个稳定状态
        /// </summary>
        private void ForwardToNextSteadyState()
        {
            if (ViewManager.Is3DStage && SCamera3D.IsAnyAnimation == false ||
                ViewManager.Is3DStage == false && SCamera2D.IsAnyAnimation == false)
            {
                // 正在显示对话
                if (this.IsShowingDialog && Director.IsButtonClicking == false && Director.IsRClicking == false)
                {
                    // 如果还在播放打字动画就跳跃
                    if (this.MsgStoryboardDict.ContainsKey(0) && this.MsgStoryboardDict[0].GetCurrentProgress() < 1.0)
                    {
                        this.MsgStoryboardDict[0].SkipToFill();
                        this.MouseLeftUpFlag = false;
                        return;
                    }
                    // 判断是否已经完成全部趟的显示
                    else if (this.pendingDialogQueue.Count == 0)
                    {
                        // 弹掉用户等待状态
                        Director.RunMana.ExitCall(Director.RunMana.CallStack);
                        this.IsShowingDialog = false;
                        this.dialogPreStr = String.Empty;
                        // 非连续对话时消除对话框
                        if (this.IsContinousDialog == false)
                        {
                            this.viewMana.GetMessageLayer(0).Visibility = Visibility.Hidden;
                            this.HideMessageTria();
                        }
                        // 截断语音
                        this.Stopvocal();
                        // 标记为非回滚
                        RollbackManager.IsRollingBack = false;
                    }
                    // 正在显示对话则向前推进一个趟
                    else
                    {
                        this.viewMana.GetMessageLayer(0).Visibility = Visibility.Visible;
                        this.DrawDialogRunQueue();
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标左键是否松开标志位
        /// </summary>
        private bool MouseLeftUpFlag = true;

        /// <summary>
        /// 鼠标右键是否松开标志位
        /// </summary>
        private bool MouseRightUpFlag = true;
        #endregion

        #region 文字层相关
        /// <summary>
        /// 把文字描绘到指定的文字层上
        /// </summary>
        /// <param name="msglayId">文字层ID</param>
        /// <param name="str">要描绘的字符串</param>
        public void DrawStringToMsgLayer(int msglayId, string str)
        {
            // 清除上一次的显示缓存
            this.viewMana.GetMessageLayer(0).Text = String.Empty;
            this.dialogPreStr = String.Empty;
            // 标记显示
            this.IsShowingDialog = true;
            string[] strRuns = this.DialogToRuns(str);
            foreach (string run in strRuns)
            {
                this.pendingDialogQueue.Enqueue(run);
            }
            // 主动调用一次显示
            this.DrawDialogRunQueue();
            Director.RunMana.UserWait("UpdateRender", "DialogWaitForClick");
        }

        /// <summary>
        /// 将对话队列里指定趟数的趟显示出来，如果显示完毕后队列已空则结束对话状态
        /// </summary>
        /// <param name="runCount">合并的趟数</param>
        /// <param name="wordDelay">是否有打字效果</param>
        private void DrawDialogRunQueue(int runCount = 1, bool wordDelay = true)
        {
            if (this.pendingDialogQueue.Count != 0)
            {
                string currentRun = String.Empty;
                for (int i = 0; i < runCount; i++)
                {
                    if (this.pendingDialogQueue.Count != 0)
                    {
                        currentRun += this.pendingDialogQueue.Dequeue();
                    }
                }
                // 回滚时不打字而是直接显示
                if (RollbackManager.IsRollingBack)
                {
                    wordDelay = false;
                }
                // 打字动画
                this.TypeWriter(0, this.dialogPreStr, currentRun, this.viewMana.GetMessageLayer(0).DisplayBinding, wordDelay ? GlobalConfigContext.GAME_MSG_TYPING_DELAY : 0);
                this.dialogPreStr += currentRun;
            }
        }

        /// <summary>
        /// 将文字直接描绘到文字层上而不等待
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="text">要描绘的字符串</param>
        private void DrawTextDirectly(int id, string text)
        {
            TextBlock t = this.viewMana.GetMessageLayer(id).DisplayBinding;
            t.Text = text;
        }

        /// <summary>
        /// 将文本处理转义并分割为趟
        /// </summary>
        /// <param name="dialogStr">要显示的文本</param>
        /// <returns>趟数组</returns>
        private string[] DialogToRuns(string dialogStr)
        {
            for (int i = 0; i < dialogStr.Length; i++)
            {
                // 如果出现转义符号
                if (dialogStr[i] == '\\' && i != dialogStr.Length - 1)
                {
                    if (i + 2 < dialogStr.Length && dialogStr[i + 1] == '$' && dialogStr[i + 2] == '{')
                    {
                        int varPtr = i + 3;
                        while (varPtr < dialogStr.Length && dialogStr[varPtr++] != '}');
                        string varStr = Director.RunMana.Fetch("$" + dialogStr.Substring(i + 3, varPtr - i - 4), this.VsmReference).ToString();
                        dialogStr = dialogStr.Remove(i, varPtr - i);
                        dialogStr = dialogStr.Insert(i, varStr);
                    }
                    else if (i + 2 < dialogStr.Length && dialogStr[i + 1] == '&' && dialogStr[i + 2] == '{')
                    {
                        int varPtr = i + 3;
                        while (varPtr < dialogStr.Length && dialogStr[varPtr++] != '}') ;
                        string varStr = Director.RunMana.Fetch("&" + dialogStr.Substring(i + 3, varPtr - i - 4), this.VsmReference).ToString();
                        dialogStr = dialogStr.Remove(i, varPtr - i);
                        dialogStr = dialogStr.Insert(i, varStr);
                    }
                }
            }
            return dialogStr.Split(new string[] { "\\|" }, StringSplitOptions.None);
        }

        /// <summary>
        /// 在指定的文字层绑定控件上进行打字动画
        /// </summary>
        /// <param name="id">层id</param>
        /// <param name="orgString">原字符串</param>
        /// <param name="appendString">要追加的字符串</param>
        /// <param name="msglayBinding">文字层的控件</param>
        /// <param name="wordTimeSpan">字符之间的打字时间间隔</param>
        private void TypeWriter(int id, string orgString, string appendString, TextBlock msglayBinding, int wordTimeSpan)
        {
            this.HideMessageTria();
            Storyboard MsgLayerTypingStory = new Storyboard();
            StringAnimationUsingKeyFrames stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames();
            Duration aniDuration = new Duration(TimeSpan.FromMilliseconds(wordTimeSpan * appendString.Length));
            stringAnimationUsingKeyFrames.Duration = aniDuration;
            MsgLayerTypingStory.Duration = aniDuration;
            string tmp = orgString;
            foreach (char c in appendString)
            {
                var discreteStringKeyFrame = new DiscreteStringKeyFrame();
                discreteStringKeyFrame.KeyTime = KeyTime.Paced;
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }
            Storyboard.SetTarget(stringAnimationUsingKeyFrames, msglayBinding);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            MsgLayerTypingStory.Children.Add(stringAnimationUsingKeyFrames);
            MsgLayerTypingStory.Completed += this.TypeWriterAnimationCompletedCallback;
            MsgLayerTypingStory.Begin();
            this.MsgStoryboardDict[id] = MsgLayerTypingStory;
        }

        /// <summary>
        /// 打字动画完成回调
        /// </summary>
        private void TypeWriterAnimationCompletedCallback(object sender, EventArgs e)
        {
            if (this.MsgStoryboardDict.ContainsKey(0) && Math.Abs(this.MsgStoryboardDict[0].GetCurrentProgress() - 1.0) < 0.01)
            {
                this.ShowMessageTria();
                this.BeginMessageTriaUpDownAnimation();
            }
        }

        /// <summary>
        /// 初始化文字小三角
        /// </summary>
        private void InitMsgLayerTria()
        {
            this.MainMsgTriangleSprite = ResourceManager.GetInstance().GetPicture(
                GlobalConfigContext.GAME_MESSAGELAYER_TRIA_FILENAME, ResourceManager.FullImageRect);
            Image TriaView = new Image();
            BitmapImage bmp = MainMsgTriangleSprite.SpriteBitmapImage;
            this.MainMsgTriangleSprite.DisplayBinding = TriaView;
            this.MainMsgTriangleSprite.AnimationElement = TriaView;
            TriaView.Width = bmp.PixelWidth;
            TriaView.Height = bmp.PixelHeight;
            TriaView.Source = bmp;
            TriaView.Visibility = Visibility.Hidden;
            TriaView.RenderTransform = new TranslateTransform();
            Canvas.SetLeft(TriaView, GlobalConfigContext.GAME_MESSAGELAYER_TRIA_X);
            Canvas.SetTop(TriaView, GlobalConfigContext.GAME_MESSAGELAYER_TRIA_Y);
            Canvas.SetZIndex(TriaView, GlobalConfigContext.GAME_Z_MESSAGELAYER + 100);
            if (ViewManager.Is3DStage)
            {
                this.view3d.BO_MainGrid.Children.Add(this.MainMsgTriangleSprite.DisplayBinding);
            }
            else
            {
                this.view2d.BO_MainGrid.Children.Add(this.MainMsgTriangleSprite.DisplayBinding);
            }
        }

        /// <summary>
        /// 隐藏对话小三角
        /// </summary>
        private void HideMessageTria()
        {
            // 只有主文字层需要作用小三角
            if (this.currentMsgLayer == 0)
            {
                this.MainMsgTriangleSprite.DisplayBinding.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 显示对话小三角
        /// </summary>
        /// <param name="opacity">透明度</param>
        private void ShowMessageTria(double opacity = 1.0f)
        {
            this.MainMsgTriangleSprite.DisplayOpacity = opacity;
            if (this.viewMana.GetMessageLayer(0)?.Visibility == Visibility.Visible)
            {
                this.MainMsgTriangleSprite.DisplayBinding.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 为对话小三角施加跳动动画
        /// </summary>
        private void BeginMessageTriaUpDownAnimation()
        {
            SpriteAnimation.UpDownRepeatAnimation(this.MainMsgTriangleSprite, TimeSpan.FromMilliseconds(500), 10, 0.8);
        }

        /// <summary>
        /// 当前正在操作的文字层
        /// </summary>
        private int currentMsgLayer = 0;
        
        /// <summary>
        /// 获取当前是否正在显示对话
        /// </summary>
        public bool IsShowingDialog { get; private set; } = false;

        /// <summary>
        /// 获取或设置当前是否正在显示选择支
        /// </summary>
        public bool IsBranching {
            get => this.isBranching;
            set
            {
                if (this.isBranching = value)
                {
                    this.RclickCounter = 1;
                }
                else
                {
                    this.RclickCounter = GlobalConfigContext.GAME_RCLICKMODE == GlobalConfigContext.RClickType.StageAndMenu ? 0 : 1;
                }
            }
        }


        private bool isBranching = false;

        /// <summary>
        /// 是否下一动作仍为对话
        /// </summary>
        private bool IsContinousDialog = false;

        /// <summary>
        /// 主文字层背景精灵
        /// </summary>
        private YuriSprite MainMsgTriangleSprite;

        /// <summary>
        /// 当前已经显示的文本内容
        /// </summary>
        public string dialogPreStr = String.Empty;

        /// <summary>
        /// 待显示的文本趟队列
        /// </summary>
        public Queue<string> pendingDialogQueue = new Queue<string>();

        /// <summary>
        /// 对话故事板容器
        /// </summary>
        private Dictionary<int, Storyboard> MsgStoryboardDict = new Dictionary<int, Storyboard>();
        #endregion

        #region 渲染器类自身相关方法和引用
        /// <summary>
        /// 初始化前端显示
        /// </summary>
        public void ViewLoaded()
        {
            this.ViewLoadedInit();
        }

        /// <summary>
        /// 在主视窗加载后的初始化动作
        /// </summary>
        private void ViewLoadedInit()
        {
            // 初始化视窗
            if (GlobalConfigContext.GAME_IS3D)
            {
                this.viewMana.InitViewport3D();
            }
            else
            {
                this.viewMana.InitViewport2D();
            }
            // 初始化小三角
            this.InitMsgLayerTria();
            // 初始化文本层
            this.viewMana.InitMessageLayer();
        }

        /// <summary>
        /// 构造器：创建一个新的画音渲染器
        /// </summary>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        public UpdateRender(StackMachine vsm)
        {
            // 绑定调用堆栈
            this.VsmReference = vsm;
            // 初始化鼠标和键盘变量
            lock (UpdateRender.KS_MOUSE_Dict)
            {
                if (UpdateRender.KS_MOUSE_Dict.ContainsKey(MouseButton.Left) == false)
                {
                    UpdateRender.KS_MOUSE_Dict[MouseButton.Left] = MouseButtonState.Released;
                    UpdateRender.KS_MOUSE_Dict[MouseButton.Middle] = MouseButtonState.Released;
                    UpdateRender.KS_MOUSE_Dict[MouseButton.Right] = MouseButtonState.Released;
                    foreach (var t in Enum.GetNames(typeof(Key)))
                    {
                        UpdateRender.KS_KEY_Dict[(Key) Enum.Parse(typeof(Key), t)] = KeyStates.None;
                        //Director.RunMana.Assignment("&kb_" + t, "0", vsm);
                    }
                }
            }
        }

        /// <summary>
        /// 作用堆栈的引用
        /// </summary>
        public StackMachine VsmReference = null;

        /// <summary>
        /// 2D主舞台的引用
        /// </summary>
        private Stage2D view2d => (Stage2D)ViewPageManager.RetrievePage(GlobalConfigContext.FirstViewPage);

        /// <summary>
        /// 3D主舞台的引用
        /// </summary>
        private Stage3D view3d => (Stage3D)ViewPageManager.RetrievePage(GlobalConfigContext.FirstViewPage);

        /// <summary>
        /// 音乐引擎
        /// </summary>
        private readonly Musician musician = Musician.GetInstance();

        /// <summary>
        /// 资源管理器
        /// </summary>
        private readonly ResourceManager resMana = ResourceManager.GetInstance();

        /// <summary>
        /// 视窗管理器
        /// </summary>
        private readonly ViewManager viewMana = ViewManager.GetInstance();
        #endregion

        #region 演绎函数
        /// <summary>
        /// 接受一个场景动作并演绎她
        /// </summary>
        /// <param name="action">场景动作实例</param>
        public void Execute(SceneAction action)
        {
            switch (action.Type)
            {
                case SActionType.act_a:
                    this.A(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseInt(action.ArgsDict["vid"], -1),
                        this.ParseDirectString(action.ArgsDict["face"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["loc"], String.Empty)
                        );
                    break;
                case SActionType.act_bg:
                    this.Background(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty),
                        this.ParseDouble(action.ArgsDict["x"], 0),
                        this.ParseDouble(action.ArgsDict["y"], 0),
                        this.ParseDouble(action.ArgsDict["opacity"], 1),
                        this.ParseDouble(action.ArgsDict["xscale"], 1),
                        this.ParseDouble(action.ArgsDict["yscale"], 1),
                        this.ParseDouble(action.ArgsDict["ro"], 0),
                        SpriteAnchorType.Center,
                        new Int32Rect(-1, 0, 0, 0)
                        );
                    break;
                case SActionType.act_picture:
                    this.Picture(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty),
                        this.ParseDouble(action.ArgsDict["x"], 0),
                        this.ParseDouble(action.ArgsDict["y"], 0),
                        this.ParseDouble(action.ArgsDict["opacity"], 1),
                        this.ParseDouble(action.ArgsDict["xscale"], 1),
                        this.ParseDouble(action.ArgsDict["yscale"], 1),
                        this.ParseDouble(action.ArgsDict["ro"], 0),
                        SpriteAnchorType.Center,
                        new Int32Rect(-1, 0, 0, 0)
                        );
                    break;
                case SActionType.act_move:
                    string moveResType = action.ArgsDict["name"];
                    this.Move(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        moveResType == "picture" ? ResourceType.Pictures : (moveResType == "stand" ? ResourceType.Stand : (moveResType == "button" ? ResourceType.Button : ResourceType.Background)),
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        this.ParseDouble(action.ArgsDict["dash"], 1),
                        this.ParseDouble(action.ArgsDict["acc"], 0),
                        TimeSpan.FromMilliseconds(this.ParseDouble(action.ArgsDict["time"], 0))
                        );
                    break;
                case SActionType.act_deletepicture:
                    this.Deletepicture(
                        this.ParseInt(action.ArgsDict["id"], -1),
                        ResourceType.Pictures
                        );
                    break;
                case SActionType.act_cstand:
                    this.Cstand(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        String.Format("{0}_{1}.png", action.ArgsDict["name"], action.ArgsDict["face"]),
                        this.ParseDouble(action.ArgsDict["x"], 0),
                        this.ParseDouble(action.ArgsDict["y"], 0),
                        1,
                        this.ParseInt(action.ArgsDict["loc"], 0),
                        SpriteAnchorType.Center,
                        new Int32Rect(0, 0, 0, 0)
                        );
                    break;
                case SActionType.act_deletecstand:
                    this.Deletecstand(
                        this.ParseInt(action.ArgsDict["id"], 0)
                        );
                    break;
                case SActionType.act_bgs:
                    this.Bgs(
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty),
                        this.ParseDouble(action.ArgsDict["vol"], 1000),
                        0
                        );
                    break;
                case SActionType.act_se:
                    this.Se(
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty),
                        this.ParseDouble(action.ArgsDict["vol"], 1000)
                        );
                    break;
                case SActionType.act_bgmfade:
                    this.Bgmfade(
                        this.ParseDouble(action.ArgsDict["vol"], 1000),
                        this.ParseDouble(action.ArgsDict["time"], 0)
                    );
                    break;
                case SActionType.act_bgm:
                    this.Bgm(
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty),
                        this.ParseDouble(action.ArgsDict["vol"], 1000)
                        );
                    break;
                case SActionType.act_stopbgm:
                    this.Stopbgm();
                    break;
                case SActionType.act_stopbgs:
                    this.Stopbgs(0);
                    break;
                case SActionType.act_vocal:
                    this.Vocal(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseInt(action.ArgsDict["vid"], -1),
                        1000
                        );
                    break;
                case SActionType.act_stopvocal:
                    this.Stopvocal();
                    break;
                case SActionType.act_title:
                    this.Title();
                    break;
                case SActionType.act_save:
                    this.Save();
                    break;
                case SActionType.act_load:
                    this.Load();
                    break;
                case SActionType.act_notify:
                    this.Notify(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["filename"], String.Empty)
                        );
                    break;
                case SActionType.act_label:
                    this.Label(action.ArgsDict["name"]);
                    break;
                case SActionType.act_snapshot:
                    this.SaveSnapshot();
                    break;
                case SActionType.act_switch:
                    this.Switch(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        this.ParseDirectString(action.ArgsDict["state"], "on") == "on"
                        );
                    break;
                case SActionType.act_var:
                    this.Var(
                        this.ParseDirectString(action.ArgsDict["name"], "$__LyyneheymTempVar"),
                        this.ParseDirectString(action.ArgsDict["dash"], "1")
                        );
                    break;
                case SActionType.act_break:
                    this.Break(
                        action
                        );
                    break;
                case SActionType.act_shutdown:
                    this.Shutdown();
                    break;
                case SActionType.act_branch:
                    this.Branch(
                        this.ParseDirectString(action.ArgsDict["link"], String.Empty)
                        );
                    break;
                case SActionType.act_titlepoint:
                    break;
                case SActionType.act_enabler:
                    this.Enabler(
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["state"], String.Empty)
                        );
                    break;
                case SActionType.act_trans:
                    this.Trans(
                        this.ParseDirectString(action.ArgsDict["name"], "Fade")
                        );
                    break;
                case SActionType.act_button:
                    this.Button(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        true,
                        this.ParseDouble(action.ArgsDict["x"], 0),
                        this.ParseDouble(action.ArgsDict["y"], 0),
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["sign"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["normal"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["over"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["on"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["type"], "once")
                        );
                    break;
                case SActionType.act_deletebutton:
                    this.Deletebutton(
                        this.ParseInt(action.ArgsDict["id"], -1)
                        );
                    break;
                case SActionType.act_style:
                    break;
                case SActionType.act_sysset:
                    this.Sysset(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["dash"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["sign"], String.Empty)
                        );
                    break;
                case SActionType.act_msglayer:
                    this.MsgLayer(
                        this.ParseInt(action.ArgsDict["id"], 0)
                        );
                    break;
                case SActionType.act_msglayeropt:
                    var dashMsgoptItem = this.ParseDirectString(action.ArgsDict["dash"], String.Empty);
                    this.MsgLayerOpt(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        dashMsgoptItem ?? String.Empty
                        );
                    break;
                case SActionType.act_scamera:
                    this.Scamera(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseInt(action.ArgsDict["x"], 0),
                        this.ParseInt(action.ArgsDict["y"], GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2),
                        this.ParseDouble(action.ArgsDict["ro"], 0)
                        );
                    break;
                case SActionType.act_draw:
                    this.DrawCommand(
                        this.ParseInt(action.ArgsDict["id"], 0),
                        this.ParseDirectString(action.ArgsDict["dash"], String.Empty)
                        );
                    break;
                case SActionType.act_alert:
                    this.Alert(
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty)
                        );
                    break;
                case SActionType.act_dialog:
                    this.Dialog(
                        action.Tag.Substring(0, action.Tag.Length - 2),
                        action.Tag.Last() == '1'
                        );
                    break;
                case SActionType.act_chapter:
                    this.Chapter(
                        this.ParseDirectString(action.ArgsDict["name"], 
                        Director.RunMana.LastScenario == null ? String.Empty : Director.RunMana.LastScenario)
                        );
                    break;
                case SActionType.act_semaphore:
                    this.Semaphore(
                        this.ParseDirectString(action.ArgsDict["name"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["target"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["activator"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["deactivator"], String.Empty),
                        this.ParseDirectString(action.ArgsDict["dash"], String.Empty)
                        );
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 结束程序
        /// </summary>
        public void Shutdown()
        {
            CommonUtils.ConsoleLine("Shutdown is called", "UpdateRender", OutputStyle.Important);
            //ViewManager.GetWindowReference()?.Close();
            Director.CollapseWorld();
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
        /// <remarks>若要强行修改对话框中的内容，请使用DrawStringToMsgLayer方法</remarks>
        /// <param name="dialogStr">要显示的文本</param>
        /// <param name="continous">是否连续对话</param>
        private void Dialog(string dialogStr, bool continous)
        {
            // 清除上一次的显示缓存
            this.viewMana.GetMessageLayer(0).Text = String.Empty;
            this.dialogPreStr = String.Empty;
            // 刷新
            this.IsContinousDialog = continous;
            this.pendingDialog = dialogStr;
            this.viewMana.GetMessageLayer(0).Visibility = Visibility.Visible;
            this.DrawStringToMsgLayer(0, this.pendingDialog);
            this.pendingDialog = String.Empty;
        }

        /// <summary>
        /// 演绎函数：执行过渡
        /// </summary>
        /// <param name="transName">效果的减缩名字</param>
        private void Trans(string transName)
        {
            this.viewMana.ApplyTransition(transName + "Transition");
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
                CommonUtils.ConsoleLine(String.Format("Drawtext cannot apply on MessageLayer0 (Main MsgLayer): {0}", text), 
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
            if (face != String.Empty)
            {
                this.Cstand(-1, String.Format("{0}_{1}.png", name, face), locStr, 1, 1, 1, 0, SpriteAnchorType.Center, new Int32Rect(0, 0, 0, 0));
            }
            if (vid != -1)
            {
                this.Vocal(name, vid, 1000);
            }
        }

        /// <summary>
        /// 演绎函数：中断循环
        /// </summary>
        /// <param name="breakSa">中断循环动作实例</param>
        private void Break(SceneAction breakSa)
        {
            Director.RunMana.CallStack.ESP.MircoStep(breakSa.Next);
        }

        /// <summary>
        /// 演绎函数：标签
        /// </summary>
        /// <param name="labelName">标签的名字</param>
        private void Label(string labelName)
        {
            if (Director.IsRClicking && labelName == "~finalizer")
            {
                if (this.IsShowingDialog)
                {
                    var mainMsgLayer = this.viewMana.GetMessageLayer(0).DisplayBinding;
                    mainMsgLayer.Visibility = Visibility.Visible;
                    MainMsgTriangleSprite.DisplayBinding.Visibility = Visibility.Visible;
                }
                else if (this.IsBranching)
                {
                    this.viewMana.EnableBranchButtonHitTest();
                }
                this.RclickCounter = this.IsBranching || GlobalConfigContext.GAME_RCLICKMODE == GlobalConfigContext.RClickType.RClickMenu ? 1 : 0;
            }
        }

        /// <summary>
        /// 演绎函数：信号系统操作
        /// </summary>
        /// <param name="cmd">信号命令</param>
        /// <param name="semaphoreName">信号量名字</param>
        /// <param name="activatorName">激活函数名</param>
        /// <param name="deactivatorName">反激活函数名</param>
        /// <param name="dash">信号量操作目标值</param>
        private void Semaphore(string cmd, string semaphoreName, string activatorName, string deactivatorName, string dash)
        {
            cmd = cmd.ToLower();
            var curScene = this.resMana.GetScene(Director.RunMana.CallStack.EBP.BindingSceneName);
            switch (cmd)
            {
                case "binding":
                    var bindActivator = activatorName == String.Empty ? null : curScene.FuncContainer.Find(t => t.Callname == activatorName);
                    var bindDeactivator = deactivatorName == String.Empty ? null : curScene.FuncContainer.Find(t => t.Callname == deactivatorName);
                    SemaphoreDispatcher.RegisterSemaphoreService(semaphoreName, bindActivator, bindDeactivator, null, "UserScene");
                    break;
                case "unbind":
                    SemaphoreDispatcher.UnregisterSemaphoreService();
                    break;
                case "set":
                    SemaphoreDispatcher.SetSemaphore(semaphoreName, dash.ToLower() == "true", null);
                    break;
                case "remove":
                    SemaphoreDispatcher.RemoveSemaphore(semaphoreName);
                    break;
                case "activate":
                    SemaphoreDispatcher.Activate(semaphoreName);
                    break;
                case "deactivate":
                    SemaphoreDispatcher.Deactivate(semaphoreName);
                    break;
                case "deactivateall":
                    SemaphoreDispatcher.DeactivateAll();
                    break;
                case "globalbinding":
                    var gbindActivator = activatorName == String.Empty ? null : curScene.FuncContainer.Find(t => t.Callname == activatorName);
                    var gbindDeactivator = deactivatorName == String.Empty ? null : curScene.FuncContainer.Find(t => t.Callname == deactivatorName);
                    SemaphoreDispatcher.RegisterGlobalSemaphoreService(semaphoreName, gbindActivator, gbindDeactivator, null, "Global");
                    break;
                case "globalunbind":
                    SemaphoreDispatcher.UnregisterGlobalSemaphoreService(semaphoreName);
                    break;
            }
        }

        /// <summary>
        /// 演绎函数：启用禁用功能
        /// </summary>
        /// <param name="target">功能描述</param>
        /// <param name="state">启用与否</param>
        public void Enabler(string target, string state)
        {
            if (target == String.Empty) { return; }
            state = state == String.Empty ? "on" : state.ToLower();
            target = target.ToLower();
            switch (target)
            {
                case "rclick":
                    Director.RunMana.EnableRClick = state == "on";
                    break;
            }
        }

        /// <summary>
        /// 演绎函数：设置当前章节名字
        /// </summary>
        /// <param name="name">章节名</param>
        private void Chapter(string name)
        {
            Director.RunMana.PerformingChapter = name;
        }

        /// <summary>
        /// 演绎函数：放置按钮
        /// </summary>
        /// <param name="id">按钮id</param>
        /// <param name="enable">按钮是否可点击</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="target">跳转目标</param>
        /// <param name="funcsign">中断函数调用签名</param>
        /// <param name="normal">正常图资源名</param>
        /// <param name="over">悬停图资源名</param>
        /// <param name="on">按下图资源名</param>
        /// <param name="type">存续类型</param>
        private void Button(int id, bool enable, double x, double y, string target, string funcsign, string normal, string over, string on, string type)
        {
            SpriteDescriptor normalDesc = new SpriteDescriptor()
            {
                ResourceName = normal
            }, overDesc = null, onDesc = null;
            if (over != String.Empty)
            {
                overDesc = new SpriteDescriptor()
                {
                    ResourceName = over
                };
            }
            if (on != String.Empty)
            {
                onDesc = new SpriteDescriptor()
                {
                    ResourceName = on
                };
            }
            Director.ScrMana.AddButton(id, enable, x, y, target, funcsign, type, normalDesc, overDesc, onDesc);
            this.viewMana.Draw(id, ResourceType.Button);
            var btn = this.viewMana.GetSpriteButton(id);
            if (btn != null)
            {
                btn.InterruptVSM = this.VsmReference;
            }
        }

        /// <summary>
        /// 从画面移除一个按钮
        /// </summary>
        /// <param name="id">按钮id</param>
        public void Deletebutton(int id)
        {
            if (id == -1)
            {
                this.viewMana.RemoveView(ResourceType.Button);
            }
            else
            {
                this.viewMana.RemoveButton(id);
            }
        }

        /// <summary>
        /// 演绎函数：显示背景
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="x">[废弃的] 图片X坐标</param>
        /// <param name="y">[废弃的] 图片Y坐标</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">X缩放比</param>
        /// <param name="yscale">Y缩放比</param>
        /// <param name="ro">图片角度或深度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Background(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            if (ViewManager.Is3DStage)
            {
                Director.ScrMana.AddBackground3D(filename, ro);
            }
            else
            {
                Director.ScrMana.AddBackground2D(id, filename, GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0, GlobalConfigContext.GAME_WINDOW_HEIGHT / 2.0,
                id, ro, opacity, xscale, yscale, anchor, cut);
            }
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
            Director.ScrMana.AddPicture(id, filename, x, y, id, xscale, yscale, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Pictures);
        }

        /// <summary>
        /// 演绎函数：显示通知
        /// </summary>
        /// <param name="name">通知大标题</param>
        /// <param name="detail">通知详情</param>
        /// <param name="iconFilename">通知的图标资源名</param>
        public void Notify(string name, string detail, string iconFilename)
        {
            NotificationManager.Notify(name, detail, iconFilename);
        }

        /// <summary>
        /// 演绎函数：移动图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="rType">资源类型</param>
        /// <param name="property">改变的属性</param>
        /// <param name="toValue">目标值</param>
        /// <param name="acc">加速度</param>
        /// <param name="duration">完成所需时间</param>
        private void Move(int id, ResourceType rType, string property, double toValue, double acc, Duration duration)
        {
            if (rType == ResourceType.Stand && ViewManager.Is3DStage)
            {
                GeometryModel3D geom = this.viewMana.GetCharacterModel3D(id);
                ModelDescriptor3D descriptor3d = Director.ScrMana.GetCharacter3DDescriptor(id);
                if (descriptor3d == null)
                {
                    CommonUtils.ConsoleLine(
                        String.Format("Ignored move (target 3d model is null): {0}, {1}", rType, id),
                        "UpdateRender", OutputStyle.Warning);
                    return;
                }
                switch (property)
                {
                    case "x":
                        var orgXP = SCamera3D.GetScreenCoordination(0, descriptor3d.SlotId);
                        var newXP = SCamera3D.GetScreenCoordination(0, (int) toValue);
                        descriptor3d.ToOffsetX = SCamera3D.GetManhattanDistance(newXP, orgXP).X;
                        SpriteAnimation.XMoveToAnimation3D(geom, descriptor3d, duration, descriptor3d.ToOffsetX, acc);
                        break;
                    case "y":
                        descriptor3d.ToOffsetY = toValue;
                        SpriteAnimation.YMoveToAnimation3D(geom, descriptor3d, duration, descriptor3d.ToOffsetY, acc);
                        break;
                    case "z":
                        descriptor3d.ToOffsetZ = toValue;
                        SpriteAnimation.ZMoveToAnimation3D(geom, descriptor3d, duration, toValue, acc);
                        break;
                    case "o":
                    case "opacity":
                        descriptor3d.ToOpacity = toValue;
                        SpriteAnimation.OpacityToAnimation3D(geom, descriptor3d, duration, toValue, acc);
                        break;
                    default:
                        CommonUtils.ConsoleLine(
                            String.Format("3D Move instruction without valid parameters: {0}", property),
                            "UpdateRender", OutputStyle.Warning);
                        break;
                }
            }
            else
            {
                YuriSprite actionSprite = this.viewMana.GetSprite(id, rType);
                SpriteDescriptor descriptor = Director.ScrMana.GetSpriteDescriptor(id, rType);
                if (rType == ResourceType.Button)
                {
                    var btn = this.viewMana.GetSpriteButton(id);
                    if (btn == null)
                    {
                        return;
                    }
                    actionSprite = btn.ImageNormal;
                    descriptor = actionSprite.Descriptor;
                }
                if (actionSprite == null)
                {
                    CommonUtils.ConsoleLine(
                        String.Format("Ignored move (target sprite is null): {0}, {1}", rType.ToString(), id),
                        "UpdateRender", OutputStyle.Warning);
                    return;
                }
                switch (property)
                {
                    case "x":
                        descriptor.ToX = toValue;
                        SpriteAnimation.XMoveToAnimation(actionSprite, duration, toValue, acc);
                        break;
                    case "y":
                        descriptor.ToY = toValue;
                        SpriteAnimation.YMoveToAnimation(actionSprite, duration, toValue, acc);
                        break;
                    case "o":
                    case "opacity":
                        descriptor.ToOpacity = toValue;
                        SpriteAnimation.OpacityToAnimation(actionSprite, duration, toValue, acc);
                        break;
                    case "a":
                    case "angle":
                        descriptor.ToAngle = toValue;
                        SpriteAnimation.RotateToAnimation(actionSprite, duration, toValue, acc);
                        break;
                    case "s":
                    case "scale":
                        descriptor.ToScaleX = descriptor.ToScaleY = toValue;
                        SpriteAnimation.ScaleToAnimation(actionSprite, duration, toValue, toValue, acc, acc);
                        break;
                    case "sx":
                    case "scalex":
                        descriptor.ToScaleX = toValue;
                        SpriteAnimation.ScaleToAnimation(actionSprite, duration, toValue, descriptor.ScaleY, acc, 0);
                        break;
                    case "sy":
                    case "scaley":
                        descriptor.ToScaleY = toValue;
                        SpriteAnimation.ScaleToAnimation(actionSprite, duration, descriptor.ScaleX, toValue, 0, acc);
                        break;
                    default:
                        CommonUtils.ConsoleLine(
                            String.Format("Move instruction without valid parameters: {0}", property),
                            "UpdateRender", OutputStyle.Warning);
                        break;
                }
            }
        }

        /// <summary>
        /// 演绎函数：设置一个系统配置上下文的变量
        /// </summary>
        /// <param name="name">变量的名字</param>
        /// <param name="dash">值的字符串表示</param>
        /// <param name="sign">值的类型</param>
        private void Sysset(string name, string dash, string sign)
        {
            try
            {
                var fVec = typeof(GlobalConfigContext).GetFields(BindingFlags.Static | BindingFlags.Public);
                var fieldObj = fVec.First(t => t.Name == name);
                if (fieldObj == null)
                {
                    CommonUtils.ConsoleLine($"field reflection failed {name}", "UpdateRender", OutputStyle.Error);
                    return;
                }
                object tvalue;
                switch (sign.ToLower())
                {
                    case "int":
                        tvalue = Int32.TryParse(dash, out int tInt) ? tInt : 0;
                        break;
                    case "boolean":
                    case "bool":
                        tvalue = Boolean.TryParse(dash, out bool tBool) && tBool;
                        break;
                    case "float":
                        tvalue = Single.TryParse(dash, out float tFloat) ? tFloat : 0.0f;
                        break;
                    case "double":
                        tvalue = Double.TryParse(dash, out double tDouble) ? tDouble : 0.0;
                        break;
                    default:
                        tvalue = dash;
                        break;
                }
                fieldObj.SetValue(null, tvalue);
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine($"field reflection failed: {name} , with type signal: {sign} and value: {dash}"
                    + Environment.NewLine + ex, "UpdateRender", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 演绎函数：为保存拍摄屏幕快照
        /// </summary>
        private void SaveSnapshot()
        {
            ViewManager.RenderFrameworkElementToJPEG(
                ViewManager.Is3DStage ? ViewManager.View3D.BO_MainGrid : ViewManager.View2D.BO_MainGrid,
                IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg"));
        }

        /// <summary>
        /// 演绎函数：移除图片
        /// </summary>
        private void Deletepicture(int id, ResourceType rType)
        {
            if (id == -1)
            {
                this.viewMana.RemoveView(rType);
            }
            else
            {
                this.viewMana.RemoveSprite(id, rType);
            }
        }

        /// <summary>
        /// 演绎函数：显示立绘
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="locationStr">立绘位置字符串</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="xscale">[废弃的] X缩放比</param>
        /// <param name="yscale">[废弃的] Y缩放比</param>
        /// <param name="ro">角度</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Cstand(int id, string filename, string locationStr, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            int tloc;
            if (ViewManager.Is3DStage)
            {
                if (Int32.TryParse(locationStr, out tloc))
                {
                    Director.ScrMana.AddCharacterStand3D(tloc, filename, 0);
                }
                else
                {
                    Director.ScrMana.AddCharacterStand3D(tloc = 0, filename, 0);
                }
            }
            else
            {
                CharacterStand2DType cst;
                switch (locationStr)
                {
                    case "l":
                    case "left":
                        cst = CharacterStand2DType.Left;
                        if (id == -1) { id = 0; }
                        break;
                    case "ml":
                    case "midleft":
                        cst = CharacterStand2DType.MidLeft;
                        if (id == -1) { id = 1; }
                        break;
                    case "mr":
                    case "midright":
                        cst = CharacterStand2DType.MidRight;
                        if (id == -1) { id = 3; }
                        break;
                    case "r":
                    case "right":
                        cst = CharacterStand2DType.Right;
                        if (id == -1) { id = 4; }
                        break;
                    default:
                        cst = CharacterStand2DType.Mid;
                        if (id == -1) { id = 2; }
                        break;
                }
                Director.ScrMana.AddCharacterStand2D(id, filename, cst, id, ro, opacity, anchor, cut);
                tloc = id;
            }
            this.viewMana.Draw(tloc, ResourceType.Stand);
        }

        /// <summary>
        /// 演绎函数：显示立绘
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="filename">资源名称</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="opacity">不透明度</param>
        /// <param name="loc">横向相对位置分块号</param>
        /// <param name="anchor">锚点</param>
        /// <param name="cut">纹理切割矩</param>
        private void Cstand(int id, string filename, double x, double y, double opacity, int loc, SpriteAnchorType anchor, Int32Rect cut)
        {
            int passinId;
            if (ViewManager.Is3DStage)
            {
                Director.ScrMana.AddCharacterStand3D(passinId = loc, filename, 0);
            }
            else
            {
                Director.ScrMana.AddCharacterStand2D(passinId = id, filename, x, y, id, 0, opacity, anchor, cut);
            }
            this.viewMana.Draw(passinId, ResourceType.Stand);
        }

        /// <summary>
        /// 演绎函数：移除立绘
        /// </summary>
        private void Deletecstand(int cst)
        {
            if (cst == -1)
            {
                this.viewMana.RemoveView(ResourceType.Stand);
            }
            else
            {
                this.viewMana.RemoveSprite(cst, ResourceType.Stand);
            }
        }

        /// <summary>
        /// 演绎函数：播放音效
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        private void Se(string resourceName, double volume)
        {
            var ms = this.resMana.GetSE(resourceName);
            this.musician.PlaySE(ms, (float)volume);
        }

        /// <summary>
        /// 演绎函数：播放BGS
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        public void Bgs(string resourceName, double volume, int track)
        {
            // 空即为停止
            if (String.IsNullOrEmpty(resourceName))
            {
                this.musician.StopBGS(track);
                return;
            }
            var ms = this.resMana.GetBGS(resourceName);
            this.musician.PlayBGS(ms, (float)volume, track);
        }

        /// <summary>
        /// 演绎函数：播放BGM，如果是同一个文件将不会重新播放
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        public void Bgm(string resourceName, double volume)
        {
            // 空即为停止
            if (String.IsNullOrEmpty(resourceName))
            {
                Director.RunMana.PlayingBGM = String.Empty;
                this.musician.StopAndReleaseBGM();
            }
            // 如果当前BGM就是此BGM就只调整音量
            else if (this.musician.CurrentBgm != resourceName)
            {
                var ms = this.resMana.GetBGM(resourceName);
                Director.RunMana.PlayingBGM = resourceName;
                this.musician.PlayBGM(resourceName, ms, (float)volume);
            }
            else
            {
                this.musician.SetBGMVolume((float)volume);
            }
        }

        /// <summary>
        /// 演绎函数：淡入淡出BGM
        /// </summary>
        /// <param name="vol">目标音量</param>
        /// <param name="ms">毫秒数</param>
        public void Bgmfade(double vol, double ms)
        {
            this.musician.FadeBgm((float) vol, (int)ms);
        }

        /// <summary>
        /// 演绎函数：停止BGM
        /// </summary>
        private void Stopbgm()
        {
            this.musician.StopAndReleaseBGM();
        }

        /// <summary>
        /// 演绎函数：停止BGS
        /// </summary>
        private void Stopbgs(int track)
        {
            this.musician.StopBGS(track);
        }

        /// <summary>
        /// 演绎函数：播放Vocal，这个动作会截断正在播放的Vocal
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="vid">语音编号</param>
        /// <param name="volume">音量</param>
        private void Vocal(string name, int vid, double volume)
        {
            if (vid == -1) { return; }
            this.Vocal(String.Format("{0}_{1}{2}", name, vid, GlobalConfigContext.GAME_VOCAL_POSTFIX), (float)volume);
        }

        /// <summary>
        /// 演绎函数：播放Vocal，这个动作会截断正在播放的Vocal
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="volume">音量</param>
        private void Vocal(string resourceName, double volume)
        {
            if (resourceName != String.Empty)
            {
                var ms = this.resMana.GetVocal(resourceName);
                this.musician.PlayVocal(ms, (float)volume);
            }
        }

        /// <summary>
        /// 演绎函数：停止语音
        /// </summary>
        private void Stopvocal()
        {
            this.musician.StopAndReleaseVocal();
        }

        /// <summary>
        /// 演绎函数：回到标题，这个动作会在弹空整个调用堆栈后再施加
        /// </summary>
        private void Title()
        {
            Director.RunMana.ExitAll();
            // 没有标志回归点就从程序入口重新开始
            if (this.titlePointContainer.Key == null || this.titlePointContainer.Value == null)
            {
                var mainScene = this.resMana.GetScene(GlobalConfigContext.Script_Main);
                if (mainScene == null)
                {
                    CommonUtils.ConsoleLine(String.Format("No Entry Point Scene: {0}, Program will exit.", GlobalConfigContext.Script_Main),
                        "Director", OutputStyle.Error);
                    Environment.Exit(0);
                }
                Director.RunMana.CallScene(mainScene);
            }
            // 有回归点就调用回归点场景并把IP指针偏移到回归点动作
            else
            {
                Director.RunMana.CallScene(this.titlePointContainer.Key);
                Director.RunMana.CallStack.ESP.MircoStep(this.titlePointContainer.Value);
            }
        }

        /// <summary>
        /// 演绎函数：呼叫存档画面
        /// </summary>
        public void Save()
        {
            Director.PauseUpdateContext();
            ((SLPage)ViewPageManager.RetrievePage("SavePage")).ReLoadFileInfo();
            ViewPageManager.NavigateTo("SavePage");
        }

        /// <summary>
        /// 演绎函数：呼叫读档画面
        /// </summary>
        public void Load()
        {
            Director.PauseUpdateContext();
            ((SLPage)ViewPageManager.RetrievePage("LoadPage")).ReLoadFileInfo();
            ViewPageManager.NavigateTo("LoadPage");
        }

        /// <summary>
        /// 保存游戏到文件
        /// </summary>
        /// <param name="saveFileName">文件名</param>
        public void ActualSave(string saveFileName)
        {
            SpriteAnimation.SkipAllAnimation();
            if (this.pendingDialogQueue.Count > 0)
            {
                this.DrawDialogRunQueue(this.pendingDialogQueue.Count, false);
            }
            var sp = Director.RunMana.PreviewSave();
            PersistContextDAO.SaveToSteadyMemory();
            IOUtils.Serialization(Director.RunMana, IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\" + saveFileName + GlobalConfigContext.GAME_SAVE_POSTFIX));
            Director.RunMana.FinishedSave(sp);
        }

        /// <summary>
        /// 载入游戏到文件
        /// </summary>
        /// <param name="loadFileName">文件名</param>
        public void ActualLoad(string loadFileName)
        {
            SpriteAnimation.SkipAllAnimation();
            var rm = (RuntimeManager)IOUtils.Unserialization(IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\" + loadFileName + GlobalConfigContext.GAME_SAVE_POSTFIX));
            Director.ResumeFromSaveData(rm);
        }

        /// <summary>
        /// 弹窗通知
        /// </summary>
        /// <param name="text">要显示的表达式</param>
        public void Alert(string text)
        {
            MessageBox.Show(text);
        }

        /// <summary>
        /// 变量操作
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="dashPolish">表达式的等价逆波兰式</param>
        private void Var(string varname, string dashPolish)
        {
            Director.RunMana.Assignment(varname, dashPolish, this.VsmReference);
        }

        /// <summary>
        /// 开关操作
        /// </summary>
        /// <param name="switchId">开关id</param>
        /// <param name="toState">目标状态</param>
        private void Switch(int switchId, bool toState)
        {
            Director.RunMana.Symbols.GlobalCtxDao.SwitchAssign(switchId, toState);
        }

        /// <summary>
        /// 演绎函数：执行场景镜头动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="r">目标分区行号</param>
        /// <param name="c">目标分区列号</param>
        /// <param name="ro">缩放比，1.0代表原始尺寸</param>
        private void Scamera(string name, int r, int c, double ro)
        {
            if (GlobalConfigContext.GAME_SCAMERA_ENABLE == false && name != "resetslot")
            {
                return;
            }
            if (r < 0 || r > GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT)
            {
                r = GlobalConfigContext.GAME_SCAMERA_SCR_ROWCOUNT / 2;
            }
            if (c < 0 || c > GlobalConfigContext.GAME_SCAMERA_SCR_COLCOUNT)
            {
                c = 0;
            }
            var sname = name.Trim().ToLower();
            if (ViewManager.Is3DStage)
            {
                switch (sname)
                {
                    case "translate":
                        SCamera3D.Translate(r, c);
                        break;
                    case "focus":
                        SCamera3D.FocusOn(r, c, ro);
                        break;
                    case "reset":
                        SCamera3D.ResetFocus(false);
                        break;
                    case "blackframe":
                        SCamera3D.LeaveSceneToBlackFrame();
                        break;
                    case "outblackframe":
                        SCamera3D.ResumeBlackFrame();
                        break;
                    case "enterscene":
                        SCamera3D.PreviewEnterScene();
                        SCamera3D.PostEnterScene();
                        break;
                    case "resetslot":
                        SCamera3D.ResetAllSlot();
                        break;
                }
            }
            else
            {
                ro = Math.Max(0.5, Math.Min(2.5, ro));
                switch (sname)
                {
                    case "translate":
                        SCamera2D.Translate(r, c);
                        break;
                    case "focus":
                        SCamera2D.FocusOn(r, c, ro);
                        break;
                    case "reset":
                        SCamera2D.ResetFocus(false);
                        break;
                    case "blackframe":
                        SCamera2D.LeaveSceneToBlackFrame();
                        break;
                    case "outblackframe":
                        SCamera2D.ResumeBlackFrame();
                        break;
                    case "enterscene":
                        SCamera2D.PreviewEnterScene();
                        SCamera2D.PostEnterScene();
                        break;
                }
            }
        }
        
        /// <summary>
        /// 选择项
        /// </summary>
        /// <param name="linkStr">选择项跳转链</param>
        private void Branch(string linkStr)
        {
            // 处理跳转链
            var tagList = new List<KeyValuePair<string, string>>();
            var linkItems = linkStr.Split(';');
            foreach (var linkItem in linkItems)
            {
                var linkPair = linkItem.Split(',');
                if (linkPair.Length == 2)
                {
                    tagList.Add(new KeyValuePair<string,string>(linkPair[0].Trim(), linkPair[1].Trim()));
                }
                else
                {
                    CommonUtils.ConsoleLine(String.Format("Ignore Branch Item: {0}", linkItem),
                        "UpdateRender", OutputStyle.Error);
                }
            }
            if (tagList.Count == 0)
            {
                return;
            }
            // 处理按钮显示参数
            double GroupX = GlobalConfigContext.GAME_WINDOW_WIDTH / 2.0 - GlobalConfigContext.GAME_BRANCH_WIDTH / 2.0;
            double BeginY = GlobalConfigContext.GAME_WINDOW_ACTUALHEIGHT / 2.0 - (GlobalConfigContext.GAME_BRANCH_HEIGHT * 2.0) * (tagList.Count / 2.0);
            double DeltaY = GlobalConfigContext.GAME_BRANCH_HEIGHT;
            // 描绘按钮
            for (int i = 0; i < tagList.Count; i++)
            {
                SpriteDescriptor normalDesc = new SpriteDescriptor()
                {
                    ResourceName = GlobalConfigContext.GAME_BRANCH_BACKGROUNDNORMAL
                },
                overDesc = new SpriteDescriptor()
                {
                    ResourceName = GlobalConfigContext.GAME_BRANCH_BACKGROUNDSELECT
                },
                onDesc = new SpriteDescriptor()
                {
                    ResourceName = GlobalConfigContext.GAME_BRANCH_BACKGROUNDSELECT
                };
                Director.ScrMana.AddBranchButton(i, GroupX, BeginY + DeltaY * 2 * i, tagList[i].Value, tagList[i].Key, normalDesc, overDesc, onDesc);
                this.viewMana.Draw(i, ResourceType.BranchButton);
            }
            // 更改状态
            this.IsShowingDialog = false;
            this.IsBranching = true;
            // 追加等待
            Director.RunMana.UserWait("UpdateRender", String.Format("BranchWaitFor:{0}", linkStr));
        }

        /// <summary>
        /// 移除屏幕上所有的选择项按钮
        /// </summary>
        public void RemoveAllBranchButton()
        {
            this.viewMana.RemoveView(ResourceType.BranchButton);
        }

        /// <summary>
        /// 标记标题回归点
        /// </summary>
        private void Titlepoint()
        {
            this.titlePointContainer = new KeyValuePair<Scene, SceneAction>(
                this.resMana.GetScene(Director.RunMana.CallStack.ESP.BindingSceneName),
                Director.RunMana.CallStack.ESP.IP);
        }

        /// <summary>
        /// 标题动作节点
        /// </summary>
        private KeyValuePair<Scene, SceneAction> titlePointContainer = new KeyValuePair<Scene, SceneAction>(null, null);

        /// <summary>
        /// 演绎函数：选定要操作的文字层
        /// </summary>
        /// <param name="id">文字层ID</param>
        private void MsgLayer(int id)
        {
            this.currentMsgLayer = id;
        }

        /// <summary>
        /// 演绎函数：修改文字层的属性
        /// </summary>
        /// <param name="msglayId">层id</param>
        /// <param name="property">要修改的属性名</param>
        /// <param name="valueStr">值字符串</param>
        private void MsgLayerOpt(int msglayId, string property, string valueStr)
        {
            if (msglayId >= 0 && msglayId < GlobalConfigContext.GAME_MESSAGELAYER_COUNT)
            {
                MessageLayer ml = this.viewMana.GetMessageLayer(msglayId);
                MessageLayerDescriptor mld = Director.ScrMana.GetMsgLayerDescriptor(msglayId);
                switch (property)
                {
                    case "fs":
                    case "fontsize":
                        ml.FontSize = mld.FontSize = Convert.ToDouble(valueStr);
                        break;
                    case "fn":
                    case "fontname":
                        ml.FontName = mld.FontName = valueStr;
                        break;
                    case "fc":
                    case "fontcolor":
                        var rgbItem = valueStr.Split(',');
                        if (rgbItem.Length != 3)
                        {
                            CommonUtils.ConsoleLine("Font Color should be RGB format", "UpdateRender", OutputStyle.Error);
                            return;
                        }
                        mld.FontColorR = Convert.ToByte(rgbItem[0]);
                        mld.FontColorG = Convert.ToByte(rgbItem[1]);
                        mld.FontColorB = Convert.ToByte(rgbItem[2]);
                        ml.FontColor = Color.FromRgb(Convert.ToByte(rgbItem[0]), Convert.ToByte(rgbItem[1]), Convert.ToByte(rgbItem[2]));
                        break;
                    case "v":
                    case "visible":
                        mld.Visible = valueStr == "true";
                        ml.Visibility = mld.Visible ? Visibility.Visible : Visibility.Hidden;
                        break;
                    case "l":
                    case "lineheight":
                        ml.LineHeight = mld.LineHeight = Convert.ToDouble(valueStr);
                        break;
                    case "o":
                    case "opacity":
                        ml.Opacity = mld.Opacity = Convert.ToDouble(valueStr);
                        break;
                    case "x":
                        ml.X = mld.X = Convert.ToDouble(valueStr);
                        break;
                    case "y":
                        ml.Y = mld.Y = Convert.ToDouble(valueStr);
                        break;
                    case "z":
                        ml.Z = mld.Z = (int)Convert.ToDouble(valueStr);
                        break;
                    case "h":
                    case "height":
                        ml.Height = mld.Height = Convert.ToDouble(valueStr);
                        break;
                    case "w":
                    case "width":
                        ml.Width = mld.Width = Convert.ToDouble(valueStr);
                        break;
                    case "p":
                    case "padding":
                        var padItem = valueStr.Split(',');
                        ml.Padding = new Thickness(Convert.ToDouble(padItem[0]), Convert.ToDouble(padItem[1]), Convert.ToDouble(padItem[2]), Convert.ToDouble(padItem[3]));
                        mld.Padding = new MyThickness(ml.Padding);
                        break;
                    case "ta":
                    case "texthorizontal":
                        ml.TextHorizontalAlignment = mld.TextHorizonAlign = (TextAlignment)Enum.Parse(typeof(TextAlignment), valueStr, true);
                        break;
                    case "ha":
                    case "horizontal":
                        ml.HorizontalAlignment = mld.HorizonAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), valueStr);
                        break;
                    case "va":
                    case "vertical":
                        ml.VerticalAlignment = mld.VertiAlign = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), valueStr);
                        break;
                    case "bg":
                    case "backgroundname":
                        mld.BackgroundResourceName = valueStr;
                        this.viewMana.Draw(msglayId, ResourceType.MessageLayerBackground);
                        break;
                    case "r":
                    case "reset":
                        this.viewMana.GetMessageLayer(msglayId).Reset();
                        break;
                    case "sr":
                    case "stylereset":
                        this.viewMana.GetMessageLayer(msglayId).StyleReset();
                        break;
                }
            }
            else
            {
                CommonUtils.ConsoleLine(String.Format("msglayeropt id out of range: MsgLayer {0}", msglayId),
                    "UpdateRender", OutputStyle.Error);
            }
        }

        /// <summary>
        /// 把字符串描绘到指定的文字层上
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="str">要描绘的文本</param>
        private void DrawCommand(int id, string str)
        {
            this.Drawtext(id, str);
        }
        #endregion
    }
}
