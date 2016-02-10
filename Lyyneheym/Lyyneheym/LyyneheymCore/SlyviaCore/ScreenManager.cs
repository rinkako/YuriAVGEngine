using System;
using System.Collections.Generic;
using System.Windows;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>屏幕管理器：保存屏幕上显示的信息，供画面重绘和保存用</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    [Serializable]
    public class ScreenManager
    {

        public void AddBackground(int id, string source, double X, double Y, double Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescription sd = new SpriteDescription()
            {
                id = id,
                resType = ResourceType.Background,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.BackgroundDescVec[0] = sd;
        }

        public void AddCharacterStand(int id, string source, double X, double Y, double Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescription sd = new SpriteDescription()
            {
                id = id,
                resType = ResourceType.Stand,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.CharacterDescVec[id] = sd;
        }

        public void AddCharacterStand(int id, string source, CharacterStandType cst, double Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescription sd = null;
            switch (cst)
            {
                case CharacterStandType.Left:
                    sd = new SpriteDescription()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
                case CharacterStandType.Mid:
                    sd = new SpriteDescription()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MID_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MID_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
                case CharacterStandType.Right:
                    sd = new SpriteDescription()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_RIGHT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_RIGHT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
            }
            this.CharacterDescVec[id] = sd;
        }

        public void AddPicture(int id, string source, double X, double Y, double Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescription sd = new SpriteDescription()
            {
                id = id,
                resType = ResourceType.Pictures,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.CharacterDescVec[id] = sd;
        }

        /// <summary>
        /// 背景描述向量
        /// </summary>
        private List<SpriteDescription> BackgroundDescVec;

        /// <summary>
        /// 立绘描述向量
        /// </summary>
        private List<SpriteDescription> CharacterDescVec;

        /// <summary>
        /// 图片描述向量
        /// </summary>
        private List<SpriteDescription> PictureDescVec;
        
        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ScreenManager()
        {
            this.BackgroundDescVec = new List<SpriteDescription>();
            this.CharacterDescVec = new List<SpriteDescription>();
            this.PictureDescVec = new List<SpriteDescription>();
            for (int i = 0; i < GlobalDataContainer.GAME_BACKGROUND_COUNT; i++)
            {
                this.BackgroundDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_CHARACTERSTAND_COUNT; i++)
            {
                this.CharacterDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_IMAGELAYER_COUNT; i++)
            {
                this.PictureDescVec.Add(null);
            }
        }

        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>屏幕管理器</returns>
        public static ScreenManager GetInstance()
        {
            return ScreenManager.synObject == null ? ScreenManager.synObject = new ScreenManager() : ScreenManager.synObject;
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static ScreenManager synObject = null;
    }

    /// <summary>
    /// 枚举：立绘位置
    /// </summary>
    public enum CharacterStandType
    {
        Left,
        Right,
        Mid
    }
}
