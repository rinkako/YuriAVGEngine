using System;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 游戏设置类
    /// </summary>
    [Serializable]
    internal sealed class ConfigPackage
    {
        /// <summary>
        /// 工程：游戏名称
        /// </summary>
        public string GameProjName = "testProject";

        /// <summary>
        /// 工程：游戏版本
        /// </summary>
        public string GameProjVersion = "0";

        /// <summary>
        /// 工程：游戏密钥
        /// </summary>
        public string GameProjKey = "yurayuri";

        /// <summary>
        /// 可视：窗口宽度
        /// </summary>
        public int GameViewWindowWidth = 1024;

        /// <summary>
        /// 可视：窗口高度
        /// </summary>
        public int GameViewWindowHeight = 576;

        /// <summary>
        /// 可视：允许用户改变窗体尺寸
        /// </summary>
        public bool GameViewWindowResizeable = false;

        /// <summary>
        /// 可视：立绘相对位置-左X
        /// </summary>
        public int GameViewCStandLeftX = 10;

        /// <summary>
        /// 可视：立绘相对位置-左Y
        /// </summary>
        public int GameViewCStandLeftY = 60;

        /// <summary>
        /// 可视：立绘相对位置-左中X
        /// </summary>
        public int GameViewCStandMidleftX = 125;

        /// <summary>
        /// 可视：立绘相对位置-左中Y
        /// </summary>
        public int GameViewCStandMidleftY = 60;

        /// <summary>
        /// 可视：立绘相对位置-中X
        /// </summary>
        public int GameViewCStandMidX = 255;

        /// <summary>
        /// 可视：立绘相对位置-中Y
        /// </summary>
        public int GameViewCStandMidY = 60;

        /// <summary>
        /// 可视：立绘相对位置-右中X
        /// </summary>
        public int GameViewCStandMidrightX = 375;

        /// <summary>
        /// 可视：立绘相对位置-右中Y
        /// </summary>
        public int GameViewCStandMidrightY = 60;

        /// <summary>
        /// 可视：立绘相对位置-右X
        /// </summary>
        public int GameViewCStandRightX = 525;

        /// <summary>
        /// 可视：立绘相对位置-右Y
        /// </summary>
        public int GameViewCStandRightY = 60;

        /// <summary>
        /// 可视：图片层数量
        /// </summary>
        public int GameViewPicturesCount = 50;

        /// <summary>
        /// 可视：图片层Z基
        /// </summary>
        public int GameViewPicturesZ = 101;

        /// <summary>
        /// 可视：按钮层数量
        /// </summary>
        public int GameViewButtonCount = 50;

        /// <summary>
        /// 可视：按钮层Z基
        /// </summary>
        public int GameViewButtonZ = 201;

        /// <summary>
        /// 可视：背景层Z基
        /// </summary>
        public int GameViewBackgroundZ = 0;

        /// <summary>
        /// 可视：立绘层Z基
        /// </summary>
        public int GameViewCStandZ = 11;

        /// <summary>
        /// 文本：层数量
        /// </summary>
        public int GameMsgLayerCount = 10;

        /// <summary>
        /// 文本：默认位置X
        /// </summary>
        public int GameMsgLayerX = 0;

        /// <summary>
        /// 文本：默认位置Y
        /// </summary>
        public int GameMsgLayerY = 410;

        /// <summary>
        /// 文本：默认尺寸W
        /// </summary>
        public int GameMsgLayerW = 1024;

        /// <summary>
        /// 文本：默认尺寸H
        /// </summary>
        public int GameMsgLayerH = 170;

        /// <summary>
        /// 文本：默认边距左
        /// </summary>
        public int GameMsgLayerL = 60;

        /// <summary>
        /// 文本：默认边距上
        /// </summary>
        public int GameMsgLayerU = 45;

        /// <summary>
        /// 文本：默认边距右
        /// </summary>
        public int GameMsgLayerR = 0;

        /// <summary>
        /// 文本：默认边距下
        /// </summary>
        public int GameMsgLayerB = 0;

        /// <summary>
        /// 文本：等待图资源名
        /// </summary>
        public string GameMsgLayerTriaName = "MessageTria.png";

        /// <summary>
        /// 文本：等待图X
        /// </summary>
        public int GameMsgLayerTriaX = 960;

        /// <summary>
        /// 文本：等待图Y
        /// </summary>
        public int GameMsgLayerTriaY = 530;

        /// <summary>
        /// 文本：字号
        /// </summary>
        public int GameMsgLayerFontSize = 16;

        /// <summary>
        /// 文本：行距
        /// </summary>
        public int GameMsgLayerFontLineheight = 22;

        /// <summary>
        /// 文本：字色
        /// </summary>
        public string GameMsgLayerFontColor = "0,0,0";

        /// <summary>
        /// 文本：字体
        /// </summary>
        public string GameMsgLayerFontName = "黑体";

        /// <summary>
        /// 文本：文字投影
        /// </summary>
        public bool GameMsgLayerFontShadow = false;

        /// <summary>
        /// 文本：打字速度
        /// </summary>
        public int GameMsgLayerTypeSpeed = 60;

        /// <summary>
        /// 文本：默认背景图
        /// </summary>
        public string GameMsgLayerBackgroundName = "originMessageBox2.png";

        /// <summary>
        /// 文本：Z坐标基
        /// </summary>
        public int GameMsgLayerZ = 51;

        /// <summary>
        /// 选项：层数量
        /// </summary>
        public int GameBranchCount = 8;

        /// <summary>
        /// 选项：Z坐标基
        /// </summary>
        public int GameBranchZ = 251;

        /// <summary>
        /// 选项：默认图
        /// </summary>
        public string GameBranchBackgroundNormal = "branchItemNormal.png";
        
        /// <summary>
        /// 选项：选中图
        /// </summary>
        public string GameBranchBackgroundOver = "branchItemSelect.png";

        /// <summary>
        /// 选项：默认尺寸W
        /// </summary>
        public int GameBranchW = 400;

        /// <summary>
        /// 选项：默认尺寸H
        /// </summary>
        public int GameBranchH = 40;

        /// <summary>
        /// 选项：字体大小
        /// </summary>
        public int GameBranchFontSize = 22;

        /// <summary>
        /// 选项：字体名称
        /// </summary>
        public string GameBranchFontName = "黑体";

        /// <summary>
        /// 选项：字体颜色
        /// </summary>
        public string GameBranchFontColor = "255,255,255";

        /// <summary>
        /// 选项：字体上边距
        /// </summary>
        public int GameBranchPadTop = 7;

        /// <summary>
        /// 音频：默认BGM音量
        /// </summary>
        public int GameMusicBGMVol = 800;

        /// <summary>
        /// 音频：默认BGS音量
        /// </summary>
        public int GameMusicBGSVol = 800;

        /// <summary>
        /// 音频：默认SE音量
        /// </summary>
        public int GameMusicSEVol = 1000;

        /// <summary>
        /// 音频：默认Vocal音量
        /// </summary>
        public int GameMusicVocalVol = 1000;

        /// <summary>
        /// 音频：BGS轨道数量
        /// </summary>
        public int GameMusicBgsCount = 5;

        /// <summary>
        /// 音频：语音文件后缀名
        /// </summary>
        public string GameMusicVocalPostfix = ".mp3";

        /// <summary>
        /// 杂项：开关数量
        /// </summary>
        public int GameMaxSwitchCount = 100;

        // TODO 完善全局设置的功能
        public bool GameFullScreen = false;
        public int GamePerformance = 0;
        public bool GameEnableSCamera = true;
        public bool GameMute = false;
        public int GameRClickMode = 0;
        public int GameScrollingMode = 1;
        public bool GameAutoPointer = false;
        public int GameViewportWidth = 1280;
        public int GameViewportHeight = 720;
        public bool Game3DStage = true;
    }
}
