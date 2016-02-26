using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.YuriHalation.HalationCore
{
    /// <summary>
    /// 文件管理器：管理Halation的文件IO动作
    /// </summary>
    class FileManager
    {
        public void CreateInitFolder(string path)
        {
            // 建立根目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 建立第一级目录
            if (!Directory.Exists(path + "\\" + DevURI_RT_PICTUREASSETS))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_PICTUREASSETS);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SCENARIO))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SCENARIO);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SOUND))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SOUND);
            }
            // 建立第二级目录
            if (!Directory.Exists(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_BACKGROUND))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_BACKGROUND);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_CHARASTAND))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_CHARASTAND);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_PICTURES))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_PICTUREASSETS + "\\" + DevURI_PA_PICTURES);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_BGM))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_BGM);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_BGS))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_BGS);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_SE))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_SE);
            }
            if (!Directory.Exists(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_VOCAL))
            {
                Directory.CreateDirectory(path + "\\" + DevURI_RT_SOUND + "\\" + DevURI_SO_VOCAL);
            }
        }


        private FileManager() { }
        public static readonly FileManager Instance = new FileManager();


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
    }
}
