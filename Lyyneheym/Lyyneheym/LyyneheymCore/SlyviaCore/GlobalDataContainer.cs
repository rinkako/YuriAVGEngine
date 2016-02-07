using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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
        public static string GAME_TITLE_NAME = "SlyviaGame";
        // 游戏密钥
        public static string GAME_KEY = "testkey";
        // 游戏版本
        public static string GAME_VERSION = "1";
        #endregion

        #region 个性化设置信息
        // 窗体分辨率：宽度
        public static int GAME_WINDOW_WIDTH = 640;
        // 窗体分辨率：高度
        public static int GAME_WINDOW_HEIGHT = 480;
        // 文本展示：模式
        public static MessageLayerType GAME_MESSAGE_MODE = MessageLayerType.Dialog;
        // 文本层：文本层数量
        public static int GAME_MESSAGELAYER_COUNT = 1;
        // 文本层：文本层默认位置
        public static Point GAME_MESSAGELAYER_POSITION = new Point(0, 0);
        // 文本层：文本层默认上边距
        public static int GAME_MESSAGELAYER_MARGIN_TOP = 5;
        // 文本层：文本层默认下边距
        public static int GAME_MESSAGELAYER_MARGIN_BOTTOM = 5;
        // 文本层：文本层默认左边距
        public static int GAME_MESSAGELAYER_MARGIN_LEFT = 5;
        // 文本层：文本层默认右边距
        public static int GAME_MESSAGELAYER_MARGIN_RIGHT = 5;
        // 文本展示：是否打字模式
        public static bool GAME_MSG_ISTYPING = true;
        // 文本展示：打字模式延迟
        public static int GAME_MSG_TYPING_DELAY = 10;
        // 文本展示：打字模式过段延迟
        public static int GAME_MSG_PASSAGE_DELAY = 10;
        // 文本展示：是否已读快进
        public static bool GAME_MSG_SKIP = false;
        // 图像层：图像层数量
        public static int GAME_IMAGELAYER_COUNT = 50;
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
        // 包标记
        public static readonly string PackHeader = "___SlyviaLyyneheym";
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

        #region 系统变量名称
        // 背景音轨字典键名
        public static readonly string CVarName_BGMTrack = "CVar_BGMTrackNum";
        // 音效音轨字典键名
        public static readonly string CVarName_SETrack = "CVar_SETrackNum";
        // 语音音轨字典键名
        public static readonly string CVarName_VocalTrack = "CVar_VocalTrackNum";
        #endregion

        #region 枚举类型
        // 资源的类型
        public enum ResourceType
        {
            // 演出->剧本
            Res_Scenario,
            // 图像->背景
            Res_background,
            // 图像->立绘
            Res_character,
            // 图像->图片
            Res_pictures,
            // 声效->音乐
            Res_bgm,
            // 声效->音效
            Res_se,
            // 声效->语音
            Res_vocal
        }

        // 用户变量类型
        public enum UserVarType
        {
            // 未定式
            UVT_VOID,
            // 整数型
            UVT_INT,
            // 浮点数形
            UVT_FLOAT,
            // 字符串
            UVT_STRING,
            // 布尔
            UVT_BOOLEAN
        }

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
