using System.Windows;
using System.Windows.Media;
using Yuri.PlatformCore.Graphic;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>全局设置上下文类：维护整个游戏环境中的系统上下文</para>
    /// <para>她是一个静态类</para>
    /// </summary>
    internal static class GlobalConfigContext
    {
        #region 游戏工程数据
        /// <summary>
        /// 工程名称
        /// </summary>
        public static string GAME_PROJECT_NAME = "YuririProject";
        /// <summary>
        /// 游戏名称
        /// </summary>
        public static string GAME_TITLE_NAME = "Yuri";
        /// <summary>
        /// 游戏密钥
        /// </summary>
        public static string GAME_KEY = "yurayuri";
        /// <summary>
        /// 游戏版本
        /// </summary>
        public static string GAME_VERSION = "0";
        #endregion

        #region 个性化设置信息
        /// <summary>
        /// 舞台：是否3D
        /// </summary>
        public static bool GAME_IS3D = true;
        /// <summary>
        /// 舞台：特效水平
        /// </summary>
        public static PerformanceType GAME_PERFORMANCE_TYPE = PerformanceType.HighQuality;
        /// <summary>
        /// 舞台：鼠标滚轮模式
        /// </summary>
        public static int GAME_SCROLLINGMODE = 1;
        /// <summary>
        /// 舞台：窗体宽度 1280
        /// </summary>
        public static int GAME_WINDOW_WIDTH = 1280;
        /// <summary>
        /// 舞台：窗体高度 720
        /// </summary>
        public static int GAME_WINDOW_HEIGHT = 720;
        /// <summary>
        /// 舞台：窗体实际高度（含标题栏）
        /// </summary>
        public static int GAME_WINDOW_ACTUALHEIGHT => GlobalConfigContext.GAME_WINDOW_HEIGHT + 30;
        /// <summary>
        /// 舞台：启动时刻的窗体宽度
        /// </summary>
        public static int GAME_VIEWPORT_WIDTH = 1280;
        /// <summary>
        /// 舞台：启动时刻的窗体高度
        /// </summary>
        public static int GAME_VIEWPORT_HEIGHT = 720;
        /// <summary>
        /// 舞台：启动时刻的窗体实际高度（含标题栏）
        /// </summary>
        public static int GAME_VIEWPORT_ACTUALHEIGHT => GlobalConfigContext.GAME_VIEWPORT_HEIGHT + 30;
        /// <summary>
        /// 舞台：是否允许自由调节
        /// </summary>
        public static bool GAME_WINDOW_RESIZEABLE = true;
        /// <summary>
        /// 舞台：以全屏模式启动
        /// </summary>
        public static bool GAME_WINDOW_FULLSCREEN = false;
        /// <summary>
        /// 文本展示：模式
        /// </summary>
        public static MessageLayerType GAME_MESSAGE_MODE = MessageLayerType.Dialog;
        /// <summary>
        /// 文本层：文本层数量
        /// </summary>
        public static int GAME_MESSAGELAYER_COUNT = 2;
        /// <summary>
        /// 文本层：文本层默认位置X
        /// </summary>
        public static double GAME_MESSAGELAYER_X = 0;
        /// <summary>
        /// 文本层：文本层默认位置Y
        /// </summary>
        public static double GAME_MESSAGELAYER_Y = 410;
        /// <summary>
        /// 文本层：文本层默认宽度
        /// </summary>
        public static double GAME_MESSAGELAYER_W = 1024;
        /// <summary>
        /// 文本层：文本层默认高度
        /// </summary>
        public static double GAME_MESSAGELAYER_H = 170;
        /// <summary>
        /// 文本层：文本层默认边距
        /// </summary>
        public static Thickness GAME_MESSAGELAYER_PADDING = new Thickness(60, 45, 60, 0);
        /// <summary>
        /// 文本层：对话小三角文件名
        /// </summary>
        public static string GAME_MESSAGELAYER_TRIA_FILENAME = "MessageTria.png";
        /// <summary>
        /// 文本层：对话背景文件名
        /// </summary>
        public static string GAME_MESSAGELAYER_BACKGROUNDFILENAME = "originMessageBox2.png";
        /// <summary>
        /// 文本层：对话文字投影
        /// </summary>
        public static bool GAME_MESSAGELAYER_SHADOW = false;
        /// <summary>
        /// 文字层：对话小三角X坐标
        /// </summary>
        public static double GAME_MESSAGELAYER_TRIA_X = 960;
        /// <summary>
        /// 文字层：对话小三角Y坐标
        /// </summary>
        public static double GAME_MESSAGELAYER_TRIA_Y = 530;
        /// <summary>
        /// 文本展示：是否打字模式
        /// </summary>
        public static bool GAME_MSG_ISTYPING = true;
        /// <summary>
        /// 文本展示：打字模式延迟
        /// </summary>
        public static int GAME_MSG_TYPING_DELAY = 60;
        /// <summary>
        /// 文本展示：打字模式过段延迟
        /// </summary>
        public static int GAME_MSG_PASSAGE_DELAY = 120;
        /// <summary>
        /// 文本展示：是否已读快进
        /// </summary>
        public static bool GAME_MSG_SKIP = false;
        /// <summary>
        /// 选择项：最大数量
        /// </summary>
        public static int GAME_BRANCH_COUNT = 8;
        /// <summary>
        /// 选择项：正常背景图
        /// </summary>
        public static string GAME_BRANCH_BACKGROUNDNORMAL = "branchItemNormal.png";
        /// <summary>
        /// 选择项：选中背景图
        /// </summary>
        public static string GAME_BRANCH_BACKGROUNDSELECT = "branchItemSelect.png";
        /// <summary>
        /// 选择项：宽度
        /// </summary>
        public static int GAME_BRANCH_WIDTH = 400;
        /// <summary>
        /// 选择项：高度
        /// </summary>
        public static int GAME_BRANCH_HEIGHT = 40;
        /// <summary>
        /// 选择项：字体大小
        /// </summary>
        public static int GAME_BRANCH_FONTSIZE = 22;
        /// <summary>
        /// 选择项：上边距
        /// </summary>
        public static int GAME_BRANCH_TOPPAD = 7;
        /// <summary>
        /// 选择项：字体颜色
        /// </summary>
        public static Color GAME_BRANCH_FONTCOLOR = Colors.White;
        /// <summary>
        /// 选择项：字体
        /// </summary>
        public static string GAME_BRANCH_FONTNAME = "黑体";
        /// <summary>
        /// 图像层：图像层数量
        /// </summary>
        public static int GAME_IMAGELAYER_COUNT = 50;
        /// <summary>
        /// 图像层：最大立绘数
        /// </summary>
        public static int GAME_CHARACTERSTAND_COUNT = 5;
        /// <summary>
        /// 图像层：背景层数量
        /// </summary>
        public static int GAME_BACKGROUND_COUNT = 2;
        /// <summary>
        /// 图像层：按钮层数量
        /// </summary>
        public static int GAME_BUTTON_COUNT = 50;
        /// <summary>
        /// 图像层：左立绘X
        /// </summary>
        public static double GAME_CHARACTERSTAND_LEFT_X = 10;
        /// <summary>
        /// 图像层：左立绘Y
        /// </summary>
        public static double GAME_CHARACTERSTAND_LEFT_Y = 60;
        /// <summary>
        /// 图像层：左中立绘X
        /// </summary>
        public static double GAME_CHARACTERSTAND_MIDLEFT_X = 125;
        /// <summary>
        /// 图像层：左中立绘Y
        /// </summary>
        public static double GAME_CHARACTERSTAND_MIDLEFT_Y = 60;
        /// <summary>
        /// 图像层：中立绘X
        /// </summary>
        public static double GAME_CHARACTERSTAND_MID_X = 255;
        /// <summary>
        /// 图像层：中立绘Y
        /// </summary>
        public static double GAME_CHARACTERSTAND_MID_Y = 60;
        /// <summary>
        /// 图像层：右中立绘X
        /// </summary>
        public static double GAME_CHARACTERSTAND_MIDRIGHT_X = 375;
        /// <summary>
        /// 图像层：右中立绘Y
        /// </summary>
        public static double GAME_CHARACTERSTAND_MIDRIGHT_Y = 60;
        /// <summary>
        /// 图像层：右立绘X
        /// </summary>
        public static double GAME_CHARACTERSTAND_RIGHT_X = 525;
        /// <summary>
        /// 图像层：右立绘Y
        /// </summary>
        public static double GAME_CHARACTERSTAND_RIGHT_Y = 60;
        /// <summary>
        /// 场景镜头：是否开启场景镜头系统
        /// </summary>
        public static bool GAME_SCAMERA_ENABLE = true;
        /// <summary>
        /// 场景镜头：屏幕纵向划分块数
        /// </summary>
        public static int GAME_SCAMERA_SCR_ROWCOUNT = 15;
        /// <summary>
        /// 场景镜头：屏幕横向划分块数
        /// </summary>
        public static int GAME_SCAMERA_SCR_COLCOUNT = 16 * 2;
        /// <summary>
        /// 场景镜头：屏幕横向单侧出血块数
        /// </summary>
        public static int GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT = 6 * 2;
        /// <summary>
        /// 场景镜头：立绘纵向划分块数
        /// </summary>
        public static int GAME_SCAMERA_CSTAND_ROWCOUNT = 12;
        /// <summary>
        /// 场景镜头：立绘横向尺寸
        /// </summary>
        public static int GAME_SCAMERA_CSTAND_WIDTH = 2031;
        /// <summary>
        /// 场景镜头：立绘纵向尺寸
        /// </summary>
        public static int GAME_SCAMERA_CSTAND_HEIGHT = 2952;
        /// <summary>
        /// 场景镜头：立绘正常半身态缩放比
        /// </summary>
        public static double GAME_SCAMERA_CSTAND_NORMALRATIO = 0.4;
        /// <summary>
        /// 全局：自动移动指针
        /// </summary>
        public static bool GAME_AUTO_POINTER = false;
        /// <summary>
        /// 全局：背景Z坐标
        /// </summary>
        public static int GAME_Z_BACKGROUND = 0;
        /// <summary>
        /// 全局：立绘Z坐标最小值
        /// </summary>
        public static int GAME_Z_CHARACTERSTAND = 11;
        /// <summary>
        /// 全局：文字层Z坐标最小值
        /// </summary>
        public static int GAME_Z_MESSAGELAYER = 51;
        /// <summary>
        /// 全局：图片Z坐标最小值
        /// </summary>
        public static int GAME_Z_PICTURES = 101;
        /// <summary>
        /// 全局：按钮Z坐标最小值
        /// </summary>
        public static int GAME_Z_BUTTON = 201;
        /// <summary>
        /// 全局：选择项按钮Z坐标最小值
        /// </summary>
        public static int GAME_Z_BRANCHBUTTON = 251;
        /// <summary>
        /// 音频：语音文件后缀
        /// </summary>
        public static string GAME_VOCAL_POSTFIX = ".mp3";
        /// <summary>
        /// 音频：BGM默认音量
        /// </summary>
        public static float GAME_SOUND_BGMVOL = 600;
        /// <summary>
        /// 音频：BGS默认音量
        /// </summary>
        public static float GAME_SOUND_BGSVOL = 600;
        /// <summary>
        /// 音频：SE默认音量
        /// </summary>
        public static float GAME_SOUND_SEVOL = 900;
        /// <summary>
        /// 音频：VOCAL默认音量
        /// </summary>
        public static float GAME_SOUND_VOCALVOL = 1000;
        /// <summary>
        /// 音频：静音
        /// </summary>
        public static bool GAME_SOUND_MUTE = false;
        /// <summary>
        /// 存档：截图存档
        /// </summary>
        public static bool GAME_SAVE_SCRPRINT = true;
        /// <summary>
        /// 存档：最大存档数
        /// </summary>
        public static int GAME_SAVE_MAX = 99;
        /// <summary>
        /// 存档：存档目录名
        /// </summary>
        public static string GAME_SAVE_DIR = "Save";
        /// <summary>
        /// 存档：存档描述子前缀名
        /// </summary>
        public static string GAME_SAVE_DESCRIPTOR_PREFIX = "sdesc";
        /// <summary>
        /// 存档：存档描述子后缀名
        /// </summary>
        public static string GAME_SAVE_DESCRIPTOR_POSTFIX = ".md";
        /// <summary>
        /// 存档：存档截图前缀名
        /// </summary>
        public static string GAME_SAVE_SNAPSHOT_PREFIX = "ssnap";
        /// <summary>
        /// 存档：存档截图后缀名
        /// </summary>
        public static string GAME_SAVE_SNAPSHOT_POSTFIX = ".jpg";
        /// <summary>
        /// 存档：存档前缀名
        /// </summary>
        public static string GAME_SAVE_PREFIX = "save";
        /// <summary>
        /// 存档：存档后缀名
        /// </summary>
        public static string GAME_SAVE_POSTFIX = ".dat";
        /// <summary>
        /// 字体：字体名称
        /// </summary>
        public static string GAME_FONT_NAME = "黑体";
        /// <summary>
        /// 字体：颜色
        /// </summary>
        public static Color GAME_FONT_COLOR = Colors.Black;
        /// <summary>
        /// 字体：行距
        /// </summary>
        public static int GAME_FONT_LINEHEIGHT = 22;
        /// <summary>
        /// 字体：字号
        /// </summary>
        public static int GAME_FONT_FONTSIZE = 16;
        /// <summary>
        /// 音乐：BGS轨道数
        /// </summary>
        public static int GAME_MUSIC_BGSTRACKNUM = 5;
        /// <summary>
        /// 开发：全局开关总数
        /// </summary>
        public static int GAME_SWITCH_COUNT = 100;
        /// <summary>
        /// 开发：控制台输出
        /// </summary>
        public static bool GAME_DEBUG_CONSOLE = true;
        /// <summary>
        /// 开发：日志输出
        /// </summary>
        public static bool GAME_DEBUG_LOG = true;
        #endregion

        #region 目录和字典常量
        /// <summary>
        /// 图像资源目录名
        /// </summary>
        public static readonly string DevURI_RT_PICTUREASSETS = "PictureAssets";
        /// <summary>
        /// 场景资源目录名
        /// </summary>
        public static readonly string DevURI_RT_SCENARIO = "Scenario";
        /// <summary>
        /// 声效资源目录名
        /// </summary>
        public static readonly string DevURI_RT_SOUND = "Sound";
        /// <summary>
        /// 图像:背景资源目录名
        /// </summary>
        public static readonly string DevURI_PA_BACKGROUND = "background";
        /// <summary>
        /// 图像:立绘资源目录名
        /// </summary>
        public static readonly string DevURI_PA_CHARASTAND = "character";
        /// <summary>
        /// 图像:图片资源目录名
        /// </summary>
        public static readonly string DevURI_PA_PICTURES = "pictures";
        /// <summary>
        /// 声效:音乐资源目录名
        /// </summary>
        public static readonly string DevURI_SO_BGM = "bgm";
        /// <summary>
        /// 声效:音效资源目录名
        /// </summary>
        public static readonly string DevURI_SO_BGS = "bgs";
        /// <summary>
        /// 声效:声效资源目录名
        /// </summary>
        public static readonly string DevURI_SO_SE = "se";
        /// <summary>
        /// 声效:语音资源目录名
        /// </summary>
        public static readonly string DevURI_SO_VOCAL = "vocal";
        #endregion

        #region 封装包名字常量
        /// <summary>
        /// 包：后缀
        /// </summary>
        public static readonly string PackPostfix = ".dat";
        /// <summary>
        /// 包：开头
        /// </summary>
        public static readonly string PackHeader = "___SlyviaLyyneheym";
        /// <summary>
        /// 包：结束
        /// </summary>
        public static readonly string PackEOF = "___SlyviaLyyneheymEOF";
        /// <summary>
        /// 包：头部项目数
        /// </summary>
        public static readonly int PackHeaderItemNum = 4;
        /// <summary>
        /// 图像:背景资源目录名
        /// </summary>
        public static readonly string PackURI_PA_BACKGROUND = "SLPBG";
        /// <summary>
        /// 图像:立绘资源目录名
        /// </summary>
        public static readonly string PackURI_PA_CHARASTAND = "SLPCS";
        /// <summary>
        /// 图像:图片资源目录名
        /// </summary>
        public static readonly string PackURI_PA_PICTURES = "SLPPC";
        /// <summary>
        /// 声效:音乐资源目录名
        /// </summary>
        public static readonly string PackURI_SO_BGM = "SLBGM";
        /// <summary>
        /// 声效:声效资源目录名
        /// </summary>
        public static readonly string PackURI_SO_BGS = "SLBGS";
        /// <summary>
        /// 声效:声效资源目录名
        /// </summary>
        public static readonly string PackURI_SO_SE = "SLSOUND";
        /// <summary>
        /// 声效:语音资源目录名
        /// </summary>
        public static readonly string PackURI_SO_VOCAL = "SLVOCAL";
        #endregion

        #region 系统信息
        /// <summary>
        /// 脚本入口
        /// </summary>
        public static readonly string Script_Main = "main";
        /// <summary>
        /// 刷新频率（万分之一毫秒）
        /// </summary>
        public static readonly double DirectorTimerInterval = 10000;
        /// <summary>
        /// 最大回滚尺度
        /// </summary>
        public static readonly int MaxRollbackStep = 100;
        /// <summary>
        /// 最大缓存资源个数
        /// </summary>
        public static readonly int EdenResourceCacheSize = 128;
        /// <summary>
        /// 入口前端页面名称
        /// </summary>
        public static readonly string FirstViewPage = "stagePage";
        /// <summary>
        /// 持久性上下文文件名
        /// </summary>
        public static readonly string PersistenceFileName = "YuriScore.dat";
        /// <summary>
        /// 持久性上下文文件名
        /// </summary>
        public static readonly string QSaveFileName = "QSave";
        #endregion

        #region 枚举类型
        /// <summary>
        /// 枚举：特效类型
        /// </summary>
        internal enum PerformanceType
        {
            /// <summary>
            /// 全特效
            /// </summary>
            HighQuality = 0,
            /// <summary>
            /// 减弱特效
            /// </summary>
            Weaken = 1,
            /// <summary>
            /// 无特效
            /// </summary>
            NoEffect = 2
        }

        /// <summary>
        /// 枚举：右键类型
        /// </summary>
        internal enum RClickType
        {
            /// <summary>
            /// 仅右键菜单
            /// </summary>
            RClickMenu = 0,
            /// <summary>
            /// 舞台后右键菜单
            /// </summary>
            StageAndMenu = 1
        }
        #endregion
    }
}
