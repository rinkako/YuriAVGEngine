using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>常量类：维护整个游戏环境中的系统级变量</para>
    /// <para>她是一个静态类</para>
    /// </summary>
    public static class GlobalDataContainer
    {
        #region 游戏工程数据
        // 工程名称
        public static string GAME_PROJECT_NAME = "SlyviaProject";
        // 游戏名称
        public static string GAME_TITLE_NAME = "TestProj";
        // 游戏密钥
        public static string GAME_KEY = "testkey";
        // 游戏版本
        public static string GAME_VERSION = "0";
        #endregion

        #region 个性化设置信息
        // 窗体分辨率：宽度
        public static int GAME_WINDOW_WIDTH = 1024;
        // 窗体分辨率：高度
        public static int GAME_WINDOW_HEIGHT = 608;
        // 文本展示：模式
        public static MessageLayerType GAME_MESSAGE_MODE = MessageLayerType.Dialog;
        // 文本层：文本层数量
        public static int GAME_MESSAGELAYER_COUNT = 1;
        // 文本层：文本层默认位置X
        public static double GAME_MESSAGELAYER_X = 0;
        // 文本层：文本层默认位置Y
        public static double GAME_MESSAGELAYER_Y = 410;
        // 文本层：文本层默认宽度
        public static double GAME_MESSAGELAYER_W = 1024;
        // 文本层：文本层默认高度
        public static double GAME_MESSAGELAYER_H = 170;
        // 文本层：文本层默认边距
        public static Thickness GAME_MESSAGELAYER_PADDING = new Thickness(30, 30, 30, 30);
        // 文本层：对话小三角文件名
        public static string GAME_MESSAGELAYER_TRIA_FILENAME = "MessageTria.png";
        // 文本层：对话背景文件名
        public static string GAME_MESSAGELAYER_BACKGROUNDFILENAME = "originMessageBox2.png";
        // 文本层：对话文字投影
        public static bool GAME_MESSAGELAYER_SHADOW = false;
        // 文字层：对话小三角X坐标
        public static double GAME_MESSAGELAYER_TRIA_X = 950;
        // 文字层：对话小三角Y坐标
        public static double GAME_MESSAGELAYER_TRIA_Y = 500;
        // 文本展示：是否打字模式
        public static bool GAME_MSG_ISTYPING = true;
        // 文本展示：打字模式延迟
        public static int GAME_MSG_TYPING_DELAY = 60;
        // 文本展示：打字模式过段延迟
        public static int GAME_MSG_PASSAGE_DELAY = 120;
        // 文本展示：是否已读快进
        public static bool GAME_MSG_SKIP = false;
        // 图像层：图像层数量
        public static int GAME_IMAGELAYER_COUNT = 50;
        // 图像层：最大立绘数
        public static int GAME_CHARACTERSTAND_COUNT = 5;
        // 图像层：背景层数量
        public static int GAME_BACKGROUND_COUNT = 2;
        // 图像层：默认位置
        public static Point GAME_IMAGELAYER_POSITION = new Point(0, 0);
        // 图像层：图像层默认上边距
        public static int GAME_IMAGELAYER_MARGIN_TOP = 0;
        // 图像层：图像层默认下边距
        public static int GAME_IMAGELAYER_MARGIN_DOWN = 0;
        // 图像层：图像层默认左边距
        public static int GAME_IMAGELAYER_MARGIN_LEFT = 0;
        // 图像层：图像层默认右边距
        public static int GAME_IMAGELAYER_MARGIN_RIGHT = 0;
        // 图像层：左立绘X
        public static double GAME_CHARACTERSTAND_LEFT_X = 10;
        // 图像层：左立绘Y
        public static double GAME_CHARACTERSTAND_LEFT_Y = 60;
        // 图像层：左中立绘X
        public static double GAME_CHARACTERSTAND_MIDLEFT_X = 125;
        // 图像层：左中立绘Y
        public static double GAME_CHARACTERSTAND_MIDLEFT_Y = 60;
        // 图像层：中立绘X
        public static double GAME_CHARACTERSTAND_MID_X = 255;
        // 图像层：中立绘Y
        public static double GAME_CHARACTERSTAND_MID_Y = 60;
        // 图像层：右中立绘X
        public static double GAME_CHARACTERSTAND_MIDRIGHT_X = 375;
        // 图像层：右中立绘Y
        public static double GAME_CHARACTERSTAND_MIDRIGHT_Y = 60;
        // 图像层：右立绘X
        public static double GAME_CHARACTERSTAND_RIGHT_X = 500;
        // 图像层：右立绘Y
        public static double GAME_CHARACTERSTAND_RIGHT_Y = 60;
        // 全局：背景Z坐标
        public static int GAME_Z_BACKGROUND = 0;
        // 全局：立绘Z坐标最小值
        public static int GAME_Z_CHARACTERSTAND = 11;
        // 全局：文字层Z坐标最小值
        public static int GAME_Z_MESSAGELAYER = 51;
        // 全局：图片Z坐标最小值
        public static int GAME_Z_PICTURES = 101;
        // 音频：语音文件后缀
        public static string GAME_VOCAL_POSTFIX = ".mp3";
        // 存档：截图存档
        public static bool GAME_SAVE_SCRPRINT = true;
        // 存档：最大存档数
        public static int GAME_SAVE_MAX = 99;
        // 存档：存档目录名
        public static string GAME_SAVE_DIR = "save";
        // 字体：字体名称
        public static string GAME_FONT_NAME = "宋体";
        // 字体：颜色
        public static System.Windows.Media.Color GAME_FONT_COLOR = Colors.Black;
        // 字体：行距
        public static int GAME_FONT_LINEHEIGHT = 36;
        // 字体：字号
        public static int GAME_FONT_FONTSIZE = 12;
        // 音乐：BGS轨道数
        public static int GAME_MUSIC_BGSTRACKNUM = 5;
        // 开发：控制台输出
        public static bool GAME_DEBUG_CONSOLE = true;
        // 开发：日志输出
        public static bool GAME_DEBUG_LOG = true;
        #endregion

        #region 目录和字典常量
        // 图像资源目录名
        public static readonly string DevURI_RT_PICTUREASSETS = "PictureAssets";
        // 场景资源目录名
        public static readonly string DevURI_RT_SCENARIO = "Scenario";
        // 声效资源目录名
        public static readonly string DevURI_RT_SOUND = "Sound";
        // 图像->背景资源目录名
        public static readonly string DevURI_PA_BACKGROUND = "background";
        // 图像->立绘资源目录名
        public static readonly string DevURI_PA_CHARASTAND = "character";
        // 图像->图片资源目录名
        public static readonly string DevURI_PA_PICTURES = "pictures";
        // 声效->音乐资源目录名
        public static readonly string DevURI_SO_BGM = "bgm";
        // 声效->音效资源目录名
        public static readonly string DevURI_SO_BGS = "bgs";
        // 声效->声效资源目录名
        public static readonly string DevURI_SO_SE = "se";
        // 声效->语音资源目录名
        public static readonly string DevURI_SO_VOCAL = "vocal";
        #endregion

        #region 封装包名字常量
        // 包后缀
        public static readonly string PackPostfix = ".dat";
        // 包开头
        public static readonly string PackHeader = "___SlyviaLyyneheym";
        // 包结束
        public static readonly string PackEOF = "___SlyviaLyyneheymEOF";
        // 包头部项目数
        public static readonly int PackHeaderItemNum = 4;
        // 图像->背景资源目录名
        public static readonly string PackURI_PA_BACKGROUND = "SLPBG";
        // 图像->立绘资源目录名
        public static readonly string PackURI_PA_CHARASTAND = "SLPCS";
        // 图像->图片资源目录名
        public static readonly string PackURI_PA_PICTURES = "SLPPC";
        // 声效->音乐资源目录名
        public static readonly string PackURI_SO_BGM = "SLBGM";
        // 声效->声效资源目录名
        public static readonly string PackURI_SO_BGS = "SLBGS";
        // 声效->声效资源目录名
        public static readonly string PackURI_SO_SE = "SLSOUND";
        // 声效->语音资源目录名
        public static readonly string PackURI_SO_VOCAL = "SLVOCAL";
        #endregion

        #region 系统信息
        // 脚本入口
        public static readonly string Script_Main = "main";
        // 刷新频率
        public static readonly double DirectorTimerInterval = 1000.0 / 60.0;
        #endregion

        #region 枚举类型
        // 文本展示类型
        public enum MessageLayerType
        {
            // 隐藏
            Disposed,
            // 对话框
            Dialog,
            // 全屏文本
            Novel,
            // 对话气泡
            Bubble,
            // 全透明
            Transparent
        }
        #endregion
    }
}
