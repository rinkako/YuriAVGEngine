using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Yuri.PlatformCore.VM
{
    using GDC = GlobalConfigContext;
    /// <summary>
    /// 设置信息语法分析器类
    /// </summary>
    internal static class ConfigParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public static void ConfigParse()
        {
            // 读入数据
            FileStream fs = new FileStream(Director.BasePath + "YuriConfig.dat", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            var configDict = new Dictionary<string, string>();
            while (sr.EndOfStream == false)
            {
                string aline = sr.ReadLine();
                var lineitems = aline?.Split(new[] {"=>"}, StringSplitOptions.RemoveEmptyEntries);
                if (lineitems?.Length == 3)
                {
                    configDict.Add(lineitems[0], lineitems[1]);
                }
            }
            sr.Close();
            fs.Close();
            // 映射设置信息
            // 工程
            GDC.GAME_PROJECT_NAME = configDict["GameProjName"];
            GDC.GAME_TITLE_NAME = configDict["GameProjName"];
            GDC.GAME_KEY = configDict["GameProjKey"];
            GDC.GAME_VERSION = configDict["GameProjVersion"];
            // 窗体
            GDC.GAME_WINDOW_WIDTH = Convert.ToInt32(configDict["GameViewWindowWidth"]);
            GDC.GAME_WINDOW_HEIGHT = Convert.ToInt32(configDict["GameViewWindowHeight"]);
            GDC.GAME_WINDOW_RESIZEABLE = configDict["GameViewWindowResizeable"] == "True";
            GDC.GAME_IS3D = configDict["Game3DStage"] == "True";
            GDC.GAME_SCROLLINGMODE = Convert.ToInt32(configDict["GameScrollingMode"]);
            GDC.GAME_RCLICKMODE = (GDC.RClickType)Convert.ToInt32(configDict["GameRClickMode"]);
            // 文本层
            GDC.GAME_MESSAGELAYER_COUNT = Convert.ToInt32(configDict["GameMsgLayerCount"]);
            GDC.GAME_MESSAGELAYER_X = Convert.ToInt32(configDict["GameMsgLayerX"]);
            GDC.GAME_MESSAGELAYER_Y = Convert.ToInt32(configDict["GameMsgLayerY"]);
            GDC.GAME_MESSAGELAYER_W = Convert.ToInt32(configDict["GameMsgLayerW"]);
            GDC.GAME_MESSAGELAYER_H = Convert.ToInt32(configDict["GameMsgLayerH"]);
            GDC.GAME_MESSAGELAYER_PADDING = new System.Windows.Thickness(Convert.ToInt32(configDict["GameMsgLayerL"]), Convert.ToInt32(configDict["GameMsgLayerU"]), Convert.ToInt32(configDict["GameMsgLayerR"]), Convert.ToInt32(configDict["GameMsgLayerB"]));
            GDC.GAME_MESSAGELAYER_TRIA_FILENAME = configDict["GameMsgLayerTriaName"];
            GDC.GAME_MESSAGELAYER_BACKGROUNDFILENAME = configDict["GameMsgLayerBackgroundName"];
            GDC.GAME_MESSAGELAYER_SHADOW = configDict["GameMsgLayerFontShadow"] == "True";
            GDC.GAME_MESSAGELAYER_TRIA_X = Convert.ToInt32(configDict["GameMsgLayerTriaX"]);
            GDC.GAME_MESSAGELAYER_TRIA_Y = Convert.ToInt32(configDict["GameMsgLayerTriaY"]);
            GDC.GAME_MSG_TYPING_DELAY = Convert.ToInt32(configDict["GameMsgLayerTypeSpeed"]);
            GDC.GAME_FONT_NAME = configDict["GameMsgLayerFontName"];
            GDC.GAME_FONT_FONTSIZE = Convert.ToInt32(configDict["GameMsgLayerFontSize"]);
            GDC.GAME_FONT_LINEHEIGHT = Convert.ToInt32(configDict["GameMsgLayerFontLineheight"]);
            GDC.GAME_FONT_COLOR = Color.FromRgb(Convert.ToByte(configDict["GameMsgLayerFontColor"].Split(',')[0]), Convert.ToByte(configDict["GameMsgLayerFontColor"].Split(',')[1]), Convert.ToByte(configDict["GameMsgLayerFontColor"].Split(',')[2]));
            GDC.GAME_Z_MESSAGELAYER = Convert.ToInt32(configDict["GameMsgLayerZ"]);
            // 图像层
            GDC.GAME_IMAGELAYER_COUNT = Convert.ToInt32(configDict["GameViewPicturesCount"]);
            GDC.GAME_BUTTON_COUNT = Convert.ToInt32(configDict["GameViewButtonCount"]);
            GDC.GAME_Z_BUTTON = Convert.ToInt32(configDict["GameViewButtonZ"]);
            GDC.GAME_Z_PICTURES = Convert.ToInt32(configDict["GameViewPicturesZ"]);
            GDC.GAME_Z_BACKGROUND = Convert.ToInt32(configDict["GameViewBackgroundZ"]);
            GDC.GAME_Z_CHARACTERSTAND = Convert.ToInt32(configDict["GameViewCStandZ"]);
            GDC.GAME_CHARACTERSTAND_LEFT_X = Convert.ToInt32(configDict["GameViewCStandLeftX"]);
            GDC.GAME_CHARACTERSTAND_LEFT_Y = Convert.ToInt32(configDict["GameViewCStandLeftY"]);
            GDC.GAME_CHARACTERSTAND_MIDLEFT_X = Convert.ToInt32(configDict["GameViewCStandMidleftX"]);
            GDC.GAME_CHARACTERSTAND_MIDLEFT_Y = Convert.ToInt32(configDict["GameViewCStandMidleftY"]);
            GDC.GAME_CHARACTERSTAND_MID_X = Convert.ToInt32(configDict["GameViewCStandMidX"]);
            GDC.GAME_CHARACTERSTAND_MID_Y = Convert.ToInt32(configDict["GameViewCStandMidY"]);
            GDC.GAME_CHARACTERSTAND_MIDRIGHT_X = Convert.ToInt32(configDict["GameViewCStandMidrightX"]);
            GDC.GAME_CHARACTERSTAND_MIDRIGHT_Y = Convert.ToInt32(configDict["GameViewCStandMidrightY"]);
            GDC.GAME_CHARACTERSTAND_RIGHT_X = Convert.ToInt32(configDict["GameViewCStandRightX"]);
            GDC.GAME_CHARACTERSTAND_RIGHT_Y = Convert.ToInt32(configDict["GameViewCStandRightY"]);
            // 选择项
            GDC.GAME_BRANCH_COUNT = Convert.ToInt32(configDict["GameBranchCount"]);
            GDC.GAME_Z_BRANCHBUTTON = Convert.ToInt32(configDict["GameBranchZ"]);
            GDC.GAME_BRANCH_BACKGROUNDNORMAL = configDict["GameBranchBackgroundNormal"];
            GDC.GAME_BRANCH_BACKGROUNDSELECT = configDict["GameBranchBackgroundOver"];
            GDC.GAME_BRANCH_WIDTH = Convert.ToInt32(configDict["GameBranchW"]);
            GDC.GAME_BRANCH_HEIGHT = Convert.ToInt32(configDict["GameBranchH"]);
            GDC.GAME_BRANCH_FONTSIZE = Convert.ToInt32(configDict["GameBranchFontSize"]);
            GDC.GAME_BRANCH_FONTNAME = configDict["GameBranchFontName"];
            GDC.GAME_BRANCH_TOPPAD = Convert.ToInt32(configDict["GameBranchPadTop"]);
            GDC.GAME_BRANCH_FONTCOLOR = Color.FromRgb(Convert.ToByte(configDict["GameBranchFontColor"].Split(',')[0]), Convert.ToByte(configDict["GameBranchFontColor"].Split(',')[1]), Convert.ToByte(configDict["GameBranchFontColor"].Split(',')[2]));
            // 音频
            GDC.GAME_SOUND_BGMVOL = Convert.ToInt32(configDict["GameMusicBGMVol"]);
            GDC.GAME_SOUND_BGSVOL = Convert.ToInt32(configDict["GameMusicBGSVol"]);
            GDC.GAME_SOUND_SEVOL = Convert.ToInt32(configDict["GameMusicSEVol"]);
            GDC.GAME_SOUND_VOCALVOL = Convert.ToInt32(configDict["GameMusicVocalVol"]);
            GDC.GAME_MUSIC_BGSTRACKNUM = Convert.ToInt32(configDict["GameMusicBgsCount"]);
            GDC.GAME_VOCAL_POSTFIX = configDict["GameMusicVocalPostfix"];
            // 杂项
            GDC.GAME_SWITCH_COUNT = Convert.ToInt32(configDict["GameMaxSwitchCount"]);
            GDC.GAME_WINDOW_FULLSCREEN = configDict["GameFullScreen"] == "True";
            GDC.GAME_PERFORMANCE_TYPE = (GDC.PerformanceType)Convert.ToInt32(configDict["GamePerformance"]);
            GDC.GAME_SCAMERA_ENABLE = configDict["GameEnableSCamera"] == "True";
            GDC.GAME_SOUND_MUTE = configDict["GameMute"] == "True";
            GDC.GAME_AUTO_POINTER = configDict["GameAutoPointer"] == "True";
            GDC.GAME_VIEWPORT_WIDTH = Convert.ToInt32(configDict["GameViewportWidth"]);
            GDC.GAME_VIEWPORT_HEIGHT = Convert.ToInt32(configDict["GameViewportHeight"]);
        }
    }
}
