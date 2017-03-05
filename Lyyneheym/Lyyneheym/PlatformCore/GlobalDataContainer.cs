using System.Windows;
using System.Windows.Media;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>常量类：维护整个游戏环境中的系统级变量</para>
    /// <para>她是一个静态类</para>
    /// </summary>
    internal static class GlobalDataContainer
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
        // 窗体分辨率：宽度 1024
        public static int GAME_WINDOW_WIDTH = 1024;
        // 窗体分辨率：高度 576
        public static int GAME_WINDOW_HEIGHT = 576;
        // 窗体分辨率：实际高度（标题栏含）
        public static int GAME_WINDOW_ACTUALHEIGHT { get { return GlobalDataContainer.GAME_WINDOW_HEIGHT + 32; } }
        // 窗体分辨率：是否自由调节
        public static bool GAME_WINDOW_RESIZEABLE = true;
        // 文本展示：模式
        public static MessageLayerType GAME_MESSAGE_MODE = MessageLayerType.Dialog;
        // 文本层：文本层数量
        public static int GAME_MESSAGELAYER_COUNT = 5;
        // 文本层：文本层默认位置X
        public static double GAME_MESSAGELAYER_X = 0;
        // 文本层：文本层默认位置Y
        public static double GAME_MESSAGELAYER_Y = 410;
        // 文本层：文本层默认宽度
        public static double GAME_MESSAGELAYER_W = 1024;
        // 文本层：文本层默认高度
        public static double GAME_MESSAGELAYER_H = 170;
        // 文本层：文本层默认边距
        public static Thickness GAME_MESSAGELAYER_PADDING = new Thickness(60, 45, 60, 0);
        // 文本层：对话小三角文件名
        public static string GAME_MESSAGELAYER_TRIA_FILENAME = "MessageTria.png";
        // 文本层：对话背景文件名
        public static string GAME_MESSAGELAYER_BACKGROUNDFILENAME = "originMessageBox2.png";
        // 文本层：对话文字投影
        public static bool GAME_MESSAGELAYER_SHADOW = false;
        // 文字层：对话小三角X坐标
        public static double GAME_MESSAGELAYER_TRIA_X = 960;
        // 文字层：对话小三角Y坐标
        public static double GAME_MESSAGELAYER_TRIA_Y = 530;
        // 文本展示：是否打字模式
        public static bool GAME_MSG_ISTYPING = true;
        // 文本展示：打字模式延迟
        public static int GAME_MSG_TYPING_DELAY = 60;
        // 文本展示：打字模式过段延迟
        public static int GAME_MSG_PASSAGE_DELAY = 120;
        // 文本展示：是否已读快进
        public static bool GAME_MSG_SKIP = false;
        // 选择项：最大数量
        public static int GAME_BRANCH_COUNT = 8;
        // 选择项：正常背景图
        public static string GAME_BRANCH_BACKGROUNDNORMAL = "branchItemNormal.png";
        // 选择项：选中背景图
        public static string GAME_BRANCH_BACKGROUNDSELECT = "branchItemSelect.png";
        // 选择项：宽度
        public static int GAME_BRANCH_WIDTH = 400;
        // 选择项：高度
        public static int GAME_BRANCH_HEIGHT = 40;
        // 选择项：字体大小
        public static int GAME_BRANCH_FONTSIZE = 22;
        // 选择项：上边距
        public static int GAME_BRANCH_TOPPAD = 7;
        // 选择项：字体颜色
        public static Color GAME_BRANCH_FONTCOLOR = Colors.White;
        // 选择项：字体
        public static string GAME_BRANCH_FONTNAME = "黑体";
        // 图像层：图像层数量
        public static int GAME_IMAGELAYER_COUNT = 50;
        // 图像层：最大立绘数
        public static int GAME_CHARACTERSTAND_COUNT = 5;
        // 图像层：背景层数量
        public static int GAME_BACKGROUND_COUNT = 2;
        // 图像层：按钮层数量
        public static int GAME_BUTTON_COUNT = 50;
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
        public static double GAME_CHARACTERSTAND_RIGHT_X = 525;
        // 图像层：右立绘Y
        public static double GAME_CHARACTERSTAND_RIGHT_Y = 60;
        // 场景镜头：屏幕纵向划分块数
        public static int GAME_SCAMERA_SCR_ROWCOUNT = 5;
        // 场景镜头：屏幕横向划分块数
        public static int GAME_SCAMERA_SCR_COLCOUNT = 16;
        // 场景镜头：屏幕横向单侧出血块数
        public static int GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT = 3;
        // 场景镜头：立绘纵向划分块数
        public static int GAME_SCAMERA_CSTAND_ROWCOUNT = 12;
        // 场景镜头：立绘横向尺寸
        public static int GAME_SCAMERA_CSTAND_WIDTH = 2031;
        // 场景镜头：立绘纵向尺寸
        public static int GAME_SCAMERA_CSTAND_HEIGHT = 2952;
        // 场景镜头：立绘正常半身态缩放比
        public static double GAME_SCAMERA_CSTAND_NORMALRATIO = 0.4;
        // 全局：背景Z坐标
        public static int GAME_Z_BACKGROUND = 0;
        // 全局：立绘Z坐标最小值
        public static int GAME_Z_CHARACTERSTAND = 11;
        // 全局：文字层Z坐标最小值
        public static int GAME_Z_MESSAGELAYER = 51;
        // 全局：图片Z坐标最小值
        public static int GAME_Z_PICTURES = 101;
        // 全局：按钮Z坐标最小值
        public static int GAME_Z_BUTTON = 201;
        // 全局：选择项按钮Z坐标最小值
        public static int GAME_Z_BRANCHBUTTON = 251;
        // 音频：语音文件后缀
        public static string GAME_VOCAL_POSTFIX = ".mp3";
        // 音频：BGM默认音量
        public static float GAME_SOUND_BGMVOL = 800;
        // 音频：BGS默认音量
        public static float GAME_SOUND_BGSVOL = 800;
        // 音频：SE默认音量
        public static float GAME_SOUND_SEVOL = 1000;
        // 音频：VOCAL默认音量
        public static float GAME_SOUND_VOCALVOL = 1000;
        // 存档：截图存档
        public static bool GAME_SAVE_SCRPRINT = true;
        // 存档：最大存档数
        public static int GAME_SAVE_MAX = 99;
        // 存档：存档目录名
        public static string GAME_SAVE_DIR = "Save";
        // 存档：存档后缀名
        public static string GAME_SAVE_POSTFIX = ".dat";
        // 字体：字体名称
        public static string GAME_FONT_NAME = "黑体";
        // 字体：颜色
        public static Color GAME_FONT_COLOR = Colors.Black; 
        // 字体：行距
        public static int GAME_FONT_LINEHEIGHT = 22;
        // 字体：字号
        public static int GAME_FONT_FONTSIZE = 16;
        // 音乐：BGS轨道数
        public static int GAME_MUSIC_BGSTRACKNUM = 5;
        // 开发：全局开关总数
        public static int GAME_SWITCH_COUNT = 100;
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
        public static readonly double DirectorTimerInterval = 100;
        // 最大回滚尺度
        public static readonly int MaxRollbackStep = 100;
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
