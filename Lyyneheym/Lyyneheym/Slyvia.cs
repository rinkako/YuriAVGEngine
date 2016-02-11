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
            Musician m = Musician.GetInstance();
            var r = this.ResMana.GetBGM(sourceName);
            m.PlayBGM(sourceName, r.Key, r.Value, 1000);

        }

        public void testVocal(string vocalName)
        {
            Musician m = Musician.GetInstance();
            var r = this.ResMana.GetVocal(vocalName);
            m.PlayVocal(r.Key, r.Value, 1000);
        }
        #endregion

        #region 初次进入时的初始化相关函数
        /// <summary>
        /// 初始化游戏设置
        /// </summary>
        private void InitConfig()
        {
            // 读入游戏设定INI
            
            // 把设定写到GlobalDataContainer

            // 应用设置到窗体

        }

        /// <summary>
        /// 初始化运行时环境，并指定脚本的入口
        /// </summary>
        private void InitRuntime()
        {
            var mainScene = this.ResMana.GetScene(GlobalDataContainer.Script_Main);
            if (mainScene == null)
            {
                DebugUtils.ConsoleLine(String.Format("No Entry Point Scene: {0}, Program will exit.", GlobalDataContainer.Script_Main),
                    "Director", OutputStyle.Error);
                Environment.Exit(0);
            }
            foreach (var sc in this.ResMana.GetAllScene())
            {
                this.RunMana.Symbols.AddSymbolTable(sc);
            }
            this.RunMana.CallScene(mainScene);
        }
        #endregion

        #region 前端更新后台相关函数
        /// <summary>
        /// 提供由前端更新后台键盘按键信息的方法
        /// </summary>
        /// <param name="e">键盘事件</param>
        public void UpdateKeyboard(KeyEventArgs e)
        {
            this.updateRender.SetKeyboardState(e.Key, e.KeyStates);
            DebugUtils.ConsoleLine(String.Format("Keyboard event: {0} <- {1}", e.Key.ToString(), e.KeyStates.ToString()),
                "Director", OutputStyle.Normal);
        }

        /// <summary>
        /// 提供由前端更新后台鼠标按键信息的方法
        /// </summary>
        /// <param name="e">鼠标事件</param>
        public void UpdateMouse(MouseButtonEventArgs e)
        {
            this.updateRender.SetMouseButtonState(e.ChangedButton, e.ButtonState);
        }

        /// <summary>
        /// 提供由前端更新后台鼠标滚轮信息的方法
        /// </summary>
        /// <param name="delta">鼠标滚轮差分，上滚为正，下滚为负</param>
        public void UpdateMouseWheel(int delta)
        {
            this.updateRender.SetMouseWheelDelta(delta);
        }
        #endregion

        /// <summary>
        /// 向运行时环境发出中断
        /// </summary>
        /// <param name="ntr">中断</param>
        public void SubmitInterrupt(Interrupt ntr)
        {
            this.RunMana.CallStack.Submit(ntr);
        }

        /// <summary>
        /// 向运行时环境发出等待要求
        /// </summary>
        /// <param name="waitSpan">等待的时间间隔</param>
        public void SubmitWait(TimeSpan waitSpan)
        {
            this.RunMana.Delay("Director", waitSpan);
            this.waitingVector.Add(new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, waitSpan));
        }

        /// <summary>
        /// 处理消息循环
        /// </summary>
        private void UpdateContext(object sender, EventArgs e)
        {
            // 取得调用堆栈顶部状态
            GameStackMachineState stackState = this.RunMana.GameState();
            switch (stackState)
            {
                case GameStackMachineState.Interpreting:
                case GameStackMachineState.FunctionCalling:
                    this.curState = GameState.Performing;
                    break;
                case GameStackMachineState.WaitUser:
                    this.curState = GameState.UserPanel;
                    break;
                case GameStackMachineState.Await:
                    this.curState = GameState.Waiting;
                    break;
                case GameStackMachineState.Interrupt:
                    this.curState = GameState.Interrupt;
                    break;
                case GameStackMachineState.NOP:
                    this.curState = GameState.Exit;
                    break;
            }
            // 根据状态更新信息
            switch (this.curState)
            {
                // 等待状态
                case GameState.Waiting:
                    // 计算已经等待的时间（这里，不考虑并行处理）
                    if (this.waitingVector.Count > 0)
                    {
                        var waitKVP = this.waitingVector[0];
                        var beginTimestamp = waitKVP.Key;
                        var waitSpan = waitKVP.Value;
                        // 如果已经等待结束，就弹调用栈
                        if ((DateTime.Now - waitKVP.Key) >= waitSpan)
                        {
                            this.RunMana.ExitCall();
                        }
                    }
                    break;
                // 等待用户操作
                case GameState.UserPanel:
                    break;
                // 中断
                case GameState.Interrupt:
                    var interruptSa = this.RunMana.CallStack.ESP.IP;
                    var interruptExitPoint = this.RunMana.CallStack.ESP.aTag;
                    // 处理可选表达式计算
                    if (interruptSa != null)
                    {
                        this.updateRender.Accept(interruptSa);
                    }
                    // 处理跳转
                    this.RunMana.ExitCall();
                    if (interruptExitPoint != null)
                    {
                        var curScene = this.ResMana.GetScene(this.RunMana.CallStack.ESP.bindingSceneName);
                        if (!curScene.labelDictionary.ContainsKey(interruptExitPoint))
                        {
                            DebugUtils.ConsoleLine(String.Format("Ignored Button jump Instruction (target not exist): {0}", interruptExitPoint),
                                        "Director", OutputStyle.Error);
                            break;
                        }
                        this.RunMana.CallStack.ESP.IP = curScene.labelDictionary[interruptExitPoint];
                    }
                    break;
                // 演绎脚本
                case GameState.Performing:
                    // 取下一动作
                    var nextInstruct = this.RunMana.MoveNext();
                    // 处理影响调用堆栈的动作
                    if (nextInstruct.aType == SActionType.act_wait)
                    {
                        double waitMs = nextInstruct.argsDict.ContainsKey("time") ?
                                (double)this.RunMana.CalculatePolish(nextInstruct.argsDict["time"]) : 0;
                        this.RunMana.Delay(nextInstruct.saNodeName, TimeSpan.FromMilliseconds(waitMs));
                        break;
                    }
                    else if (nextInstruct.aType == SActionType.act_waituser)
                    {
                        this.RunMana.UserWait("Director", nextInstruct.saNodeName);
                        break;
                    }
                    else if (nextInstruct.aType == SActionType.act_jump)
                    {
                        var jumpToScene = nextInstruct.argsDict["filename"];
                        var jumpToTarget = nextInstruct.argsDict["name"];
                        // 场景内跳转
                        if (jumpToScene == "")
                        {
                            var currentScene = this.ResMana.GetScene(this.RunMana.CallStack.ESP.bindingSceneName);
                            if (!currentScene.labelDictionary.ContainsKey(jumpToTarget))
                            {
                                DebugUtils.ConsoleLine(String.Format("Ignored Jump Instruction (target not exist): {0}", jumpToTarget),
                                    "Director", OutputStyle.Error);
                                break;
                            }
                            this.RunMana.CallStack.ESP.IP = currentScene.labelDictionary[jumpToTarget];
                        }
                        // 跨场景跳转
                        else
                        {
                            var jumpScene = this.ResMana.GetScene(jumpToScene);
                            if (jumpScene == null)
                            {
                                DebugUtils.ConsoleLine(String.Format("Ignored Jump Instruction (scene not exist): {0}", jumpToScene),
                                    "Director", OutputStyle.Error);
                                break;
                            }
                            if (!jumpScene.labelDictionary.ContainsKey(jumpToTarget))
                            {
                                DebugUtils.ConsoleLine(String.Format("Ignored Jump Instruction (target not exist): {0} -> {1}", jumpToScene, jumpToTarget),
                                    "Director", OutputStyle.Error);
                                break;
                            }
                            this.RunMana.ExitCall();
                            this.RunMana.CallScene(jumpScene, jumpScene.labelDictionary[jumpToTarget]);
                        }
                        break;
                    }
                    else if (nextInstruct.aType == SActionType.act_call)
                    {
                        var callFunc = nextInstruct.argsDict["name"];
                        var signFunc = nextInstruct.argsDict["sign"];
                        if (!signFunc.StartsWith("(") && !signFunc.EndsWith(")"))
                        {
                            DebugUtils.ConsoleLine(String.Format("Ignored Function calling (sign not valid): {0} -> {1}", callFunc, signFunc),
                                "Director", OutputStyle.Error);
                            break;
                        }
                        var sceneFuncContainer = this.ResMana.GetScene(this.RunMana.CallStack.ESP.bindingSceneName).funcContainer;
                        var sceneFuncList = from f in sceneFuncContainer where f.callname == callFunc select f;
                        if (sceneFuncList.Count() == 0)
                        {
                            DebugUtils.ConsoleLine(String.Format("Ignored Function calling (function not exist): {0}", callFunc),
                                "Director", OutputStyle.Error);
                            break;
                        }
                        var sceneFunc = sceneFuncList.First();
                        var signItem = signFunc.Split(',');
                        if (sceneFunc.param.Count != signItem.Length)
                        {
                            DebugUtils.ConsoleLine(String.Format("Ignored Function calling (in {0}, require args num: {1}, but actual:{2})", callFunc, sceneFunc.param.Count, signItem.Length),
                                "Director", OutputStyle.Error);
                            break;
                        }
                        // 处理参数列表
                        List<object> argsVec = new List<object>();
                        foreach (var s in signItem)
                        {
                            string trimedPara = s.Trim();
                            object varref = null;
                            if (trimedPara.StartsWith("$") || trimedPara.StartsWith("&"))
                            {
                                varref = this.RunMana.Fetch(trimedPara.Substring(1));
                            }
                            else if (trimedPara.StartsWith("\"") && trimedPara.EndsWith("\""))
                            {
                                varref = (string)trimedPara;
                            }
                            else
                            {
                                varref = Convert.ToDouble(trimedPara);
                            }
                            argsVec.Add(varref);
                        }
                        this.RunMana.CallFunction(sceneFunc, argsVec);
                        break;
                    }
                    // 处理常规动作
                    this.updateRender.Accept(nextInstruct);
                    break;
                // 退出
                case GameState.Exit:
                    this.updateRender.Shutdown();
                    break;
            }
        }

        private GameState curState;


        private List<KeyValuePair<DateTime, TimeSpan>> waitingVector;



        #region 导演类自身资源相关
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
            this.ResMana = ResourceManager.GetInstance();
            this.RunMana = new RuntimeManager();
            this.updateRender = new UpdateRender();
            this.waitingVector = new List<KeyValuePair<DateTime, TimeSpan>>();
            this.updateRender.SetRuntimeManagerReference(this.RunMana);
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(GlobalDataContainer.DirectorTimerInterval);
            this.timer.Tick += UpdateContext;
            this.timer.Start();
            this.InitRuntime();
        }

        /// <summary>
        /// 消息循环计时器
        /// </summary>
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
        #endregion
    }
}
