using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 常量类：维护整个游戏环境的常量
    /// 她是一个静态类
    /// </summary>
    public static class Consta
    {
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
        public static readonly string DevURI_SO_SE = "se";
        // 声效->语音资源目录名
        public static readonly string DevURI_SO_VOCAL = "vocal";
        #endregion

        #region 封装包名字常量
        // 包后缀
        public static readonly string PackPostfix = ".dat";
        // 图像->背景资源目录名
        public static readonly string PackURI_PA_BACKGROUND = "SLPBG";
        // 图像->立绘资源目录名
        public static readonly string PackURI_PA_CHARASTAND = "SLPCS";
        // 图像->图片资源目录名
        public static readonly string PackURI_PA_PICTURES = "SLPPC";
        // 声效->音乐资源目录名
        public static readonly string PackURI_SO_BGM = "SLBGM";
        // 声效->音效资源目录名
        public static readonly string PackURI_SO_SE = "SLSOUND";
        // 声效->语音资源目录名
        public static readonly string PackURI_SO_VOCAL = "SLVOCAL";
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
        #endregion
    }
}
