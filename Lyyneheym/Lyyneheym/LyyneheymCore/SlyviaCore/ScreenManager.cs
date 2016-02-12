using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Lyyneheym.LyyneheymCore.Utils;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>屏幕管理器：保存屏幕上显示的信息，供画面重绘和保存用</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    [Serializable]
    public class ScreenManager
    {
        /// <summary>
        /// 为屏幕增加一个背景精灵描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="Angle"></param>
        /// <param name="Opacity"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        public void AddBackground(int id, string source, double X, double Y, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                id = id,
                resType = ResourceType.Background,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_BACKGROUND,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.BackgroundDescVec[0] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个立绘精灵描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="Angle"></param>
        /// <param name="Opacity"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        public void AddCharacterStand(int id, string source, double X, double Y, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                id = id,
                resType = ResourceType.Stand,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_CHARACTERSTAND,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.CharacterDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个立绘精灵描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="cst"></param>
        /// <param name="Z"></param>
        /// <param name="Angle"></param>
        /// <param name="Opacity"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        public void AddCharacterStand(int id, string source, CharacterStandType cst, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = null;
            switch (cst)
            {
                case CharacterStandType.Left:
                    sd = new SpriteDescriptor()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_Y,
                        Z = Z + GlobalDataContainer.GAME_Z_CHARACTERSTAND,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
                case CharacterStandType.MidLeft:
                    sd = new SpriteDescriptor()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MIDLEFT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MIDLEFT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
                case CharacterStandType.Mid:
                    sd = new SpriteDescriptor()
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
                case CharacterStandType.MidRight:
                    sd = new SpriteDescriptor()
                    {
                        id = id,
                        resType = ResourceType.Stand,
                        resourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MIDRIGHT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MIDRIGHT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        anchorType = anchor,
                        cutRect = cut
                    };
                    break;
                case CharacterStandType.Right:
                    sd = new SpriteDescriptor()
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

        /// <summary>
        /// 为屏幕增加一个图片精灵描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="Angle"></param>
        /// <param name="Opacity"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        public void AddPicture(int id, string source, double X, double Y, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                id = id,
                resType = ResourceType.Pictures,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_PICTURES,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.CharacterDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个文字层描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="Visible"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="Margin"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="Opacity"></param>
        /// <param name="FontName"></param>
        /// <param name="FontColor"></param>
        /// <param name="FontSize"></param>
        /// <param name="Ha"></param>
        /// <param name="Va"></param>
        /// <param name="LineHeight"></param>
        public void AddMsgLayer(int id, string source, bool Visible, double W, double H, Thickness Margin, double X, double Y, int Z, double Opacity, string FontName, Color FontColor, double FontSize, HorizontalAlignment Ha, VerticalAlignment Va, double LineHeight)
        {
            MessageLayerDescriptor mld = new MessageLayerDescriptor()
            {
                BackgroundResourceName = source,
                Visible = Visible,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_MESSAGELAYER,
                Opacity = Opacity,
                Margin = Margin,
                Width = W,
                Height = H,
                FontName = FontName,
                FontColor = FontColor,
                FontSize = FontSize,
                HorizonAlign = Ha,
                VertiAlign = Va,
                LineHeight = LineHeight,
                text = ""
            };
            this.MsgLayerDescVec[id] = mld;
        }

        /// <summary>
        /// 从屏幕上移除一个精灵描述子
        /// </summary>
        /// <param name="spriteId">精灵ID</param>
        /// <param name="rType">资源类型</param>
        public void RemoveSprite(int spriteId, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    this.BackgroundDescVec[spriteId] = null;
                    break;
                case ResourceType.Stand:
                    this.CharacterDescVec[spriteId] = null;
                    break;
                case ResourceType.Pictures:
                    this.PictureDescVec[spriteId] = null;
                    break;
            }
        }

        /// <summary>
        /// 从屏幕上移除一个文字层
        /// </summary>
        /// <param name="layerId">文字层ID</param>
        public void RemoveMsgLayer(int layerId)
        {
            this.MsgLayerDescVec[layerId] = null;
        }

        /// <summary>
        /// 获取一个精灵的描述子
        /// </summary>
        /// <param name="spriteId">精灵ID</param>
        /// <param name="rType">资源类型</param>
        /// <returns>描述子实例</returns>
        public SpriteDescriptor GetSpriteDescriptor(int spriteId, ResourceType rType)
        {
            try
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        return this.BackgroundDescVec[spriteId];
                    case ResourceType.Stand:
                        return this.CharacterDescVec[spriteId];
                    case ResourceType.Pictures:
                        return this.PictureDescVec[spriteId];
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                DebugUtils.ConsoleLine(ex.ToString(), "ScreenManager / CLR", OutputStyle.Error);
                return null;
            }
        }

        /// <summary>
        /// 获取一个文字层描述子
        /// </summary>
        /// <param name="layerId">文字层ID</param>
        /// <returns>描述子实例</returns>
        public MessageLayerDescriptor GetMsgLayerDescriptor(int layerId)
        {
            return this.MsgLayerDescVec[layerId];
        }

        /// <summary>
        /// 背景描述向量
        /// </summary>
        private List<SpriteDescriptor> BackgroundDescVec;

        /// <summary>
        /// 立绘描述向量
        /// </summary>
        private List<SpriteDescriptor> CharacterDescVec;

        /// <summary>
        /// 图片描述向量
        /// </summary>
        private List<SpriteDescriptor> PictureDescVec;

        /// <summary>
        /// 文字层描述向量
        /// </summary>
        private List<MessageLayerDescriptor> MsgLayerDescVec;

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ScreenManager()
        {
            this.BackgroundDescVec = new List<SpriteDescriptor>();
            this.CharacterDescVec = new List<SpriteDescriptor>();
            this.PictureDescVec = new List<SpriteDescriptor>();
            this.MsgLayerDescVec = new List<MessageLayerDescriptor>();
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
            for (int i = 0; i < GlobalDataContainer.GAME_MESSAGELAYER_COUNT; i++)
            {
                this.MsgLayerDescVec.Add(null);
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
        MidLeft,
        Mid,
        MidRight,
        Right,
    }
}
