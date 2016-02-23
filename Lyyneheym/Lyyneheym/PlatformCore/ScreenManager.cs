using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Yuri.Utils;

namespace Yuri.PlatformCore
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
        public void AddBackground(int id, string source, double X, double Y, int Z, double Angle, double Opacity, double ScaleX, double ScaleY, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                id = id,
                resType = ResourceType.Background,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_BACKGROUND,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.BackgroundDescVec[id] = sd;
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
        public void AddPicture(int id, string source, double X, double Y, int Z, double ScaleX, double ScaleY, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                id = id,
                resType = ResourceType.Pictures,
                resourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_PICTURES,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                Angle = Angle,
                Opacity = Opacity,
                anchorType = anchor,
                cutRect = cut
            };
            this.PictureDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个按钮描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="jumpTarget"></param>
        /// <param name="normalDesc"></param>
        /// <param name="overDesc"></param>
        /// <param name="onDesc"></param>
        public void AddButton(int id, bool enable, double X, double Y, string jumpTarget, string type, SpriteDescriptor normalDesc, SpriteDescriptor overDesc = null, SpriteDescriptor onDesc = null)
        {
            SpriteButtonDescriptor sbd = new SpriteButtonDescriptor()
            {
                Enable = enable,
                X = X,
                Y = Y,
                Z = GlobalDataContainer.GAME_Z_BUTTON + id,
                jumpLabel = jumpTarget,
                normalDescriptor = normalDesc,
                overDescriptor = overDesc,
                onDescriptor = onDesc,
                Opacity = 1.0,
                Eternal = type == "once" ? false : true
            };
            this.ButtonDescVec[id] = sbd;
        }

        /// <summary>
        /// 为屏幕增加一个选择支描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="jumpTarget"></param>
        /// <param name="text"></param>
        /// <param name="normalDesc"></param>
        /// <param name="overDesc"></param>
        /// <param name="onDesc"></param>
        public void AddBranchButton(int id, double X, double Y, string jumpTarget, string text, SpriteDescriptor normalDesc, SpriteDescriptor overDesc = null, SpriteDescriptor onDesc = null)
        {
            BranchButtonDescriptor bbd = new BranchButtonDescriptor()
            {
                Id = id,
                JumpTarget = jumpTarget,
                X = X,
                Y = Y,
                Z = GlobalDataContainer.GAME_Z_BRANCHBUTTON + id,
                Text = text,
                normalDescriptor = normalDesc,
                overDescriptor = overDesc,
                onDescriptor = onDesc
            };
            this.BranchDescVec[id] = bbd;
        }

        /// <summary>
        /// 为屏幕增加一个文字层描述子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="Visible"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="Padding"></param>
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
        public void EditMsgLayer(int id, string source, bool Visible, double W, double H, Thickness Padding, double X, double Y, int Z, double Opacity, string FontName, Color FontColor, double FontSize, HorizontalAlignment Ha, VerticalAlignment Va, double LineHeight, bool shadow)
        {
            MessageLayerDescriptor mld = new MessageLayerDescriptor()
            {
                BackgroundResourceName = source,
                Visible = Visible,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_MESSAGELAYER,
                Opacity = Opacity,
                Padding = new MyThickness(Padding),
                Width = W,
                Height = H,
                FontName = FontName,
                FontColorR = FontColor.R,
                FontColorG = FontColor.G,
                FontColorB = FontColor.B,
                FontSize = FontSize,
                HorizonAlign = Ha,
                VertiAlign = Va,
                LineHeight = LineHeight,
                FontShadow = shadow,
                Text = ""
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
        /// 从屏幕上移除一个按钮
        /// </summary>
        /// <param name="id">按钮id</param>
        public void RemoveButton(int id)
        {
            this.ButtonDescVec[id] = null;
        }

        /// <summary>
        /// 从屏幕上移除一个选择支
        /// </summary>
        /// <param name="id">选择支id</param>
        public void RemoveBranchButton(int id)
        {
            this.BranchDescVec[id] = null;
        }

        /// <summary>
        /// 获取一个精灵的描述子
        /// </summary>
        /// <param name="id">精灵ID</param>
        /// <param name="rType">资源类型</param>
        /// <returns>描述子实例</returns>
        public SpriteDescriptor GetSpriteDescriptor(int id, ResourceType rType)
        {
            try
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        return this.BackgroundDescVec[id];
                    case ResourceType.Stand:
                        return this.CharacterDescVec[id];
                    case ResourceType.Pictures:
                        return this.PictureDescVec[id];
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
        /// <param name="id">文字层ID</param>
        /// <returns>描述子实例</returns>
        public MessageLayerDescriptor GetMsgLayerDescriptor(int id)
        {
            return this.MsgLayerDescVec[id];
        }

        /// <summary>
        /// 获取一个按钮描述子
        /// </summary>
        /// <param name="id">按钮ID</param>
        /// <returns>描述子实例</returns>
        public SpriteButtonDescriptor GetButtonDescriptor(int id)
        {
            return this.ButtonDescVec[id];
        }

        /// <summary>
        /// 获取一个选择支描述子
        /// </summary>
        /// <param name="bbId">选择支ID</param>
        /// <returns描述子实例></returns>
        public BranchButtonDescriptor GetBranchButtonDescriptor(int id)
        {
            return this.BranchDescVec[id];
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
        /// 选择项描述向量
        /// </summary>
        private List<BranchButtonDescriptor> BranchDescVec;

        /// <summary>
        /// 文字层描述向量
        /// </summary>
        private List<MessageLayerDescriptor> MsgLayerDescVec;

        /// <summary>
        /// 按钮层描述向量
        /// </summary>
        private List<SpriteButtonDescriptor> ButtonDescVec;

        /// <summary>
        /// 初始化文字层描述子
        /// </summary>
        private void InitMessageLayerDescriptors()
        {
            // 初始化主文本层
            MessageLayerDescriptor mainMsgLayer = new MessageLayerDescriptor()
            {
                Id = 0,
                BackgroundResourceName = GlobalDataContainer.GAME_MESSAGELAYER_BACKGROUNDFILENAME,
                FontColorR = GlobalDataContainer.GAME_FONT_COLOR.R,
                FontColorG = GlobalDataContainer.GAME_FONT_COLOR.G,
                FontColorB = GlobalDataContainer.GAME_FONT_COLOR.B,
                FontName = GlobalDataContainer.GAME_FONT_NAME,
                FontSize = GlobalDataContainer.GAME_FONT_FONTSIZE,
                FontShadow = GlobalDataContainer.GAME_MESSAGELAYER_SHADOW,
                LineHeight = GlobalDataContainer.GAME_FONT_LINEHEIGHT,
                HorizonAlign = HorizontalAlignment.Left,
                VertiAlign = VerticalAlignment.Bottom,
                X = GlobalDataContainer.GAME_MESSAGELAYER_X,
                Y = GlobalDataContainer.GAME_MESSAGELAYER_Y,
                Z = GlobalDataContainer.GAME_Z_MESSAGELAYER,
                Height = GlobalDataContainer.GAME_MESSAGELAYER_H,
                Width = GlobalDataContainer.GAME_MESSAGELAYER_W,
                Padding = new MyThickness(GlobalDataContainer.GAME_MESSAGELAYER_PADDING),
                Opacity = 1.0,
                Visible = false,
                Text = ""
            };
            this.MsgLayerDescVec.Add(mainMsgLayer);
            // 初始化附加文本层
            for (int i = 1; i < GlobalDataContainer.GAME_MESSAGELAYER_COUNT; i++)
            {
                MessageLayerDescriptor mld = new MessageLayerDescriptor()
                {
                    Id = i,
                    BackgroundResourceName = "",
                    FontColorR = GlobalDataContainer.GAME_FONT_COLOR.R,
                    FontColorG = GlobalDataContainer.GAME_FONT_COLOR.G,
                    FontColorB = GlobalDataContainer.GAME_FONT_COLOR.B,
                    FontName = GlobalDataContainer.GAME_FONT_NAME,
                    FontSize = GlobalDataContainer.GAME_FONT_FONTSIZE,
                    FontShadow = GlobalDataContainer.GAME_MESSAGELAYER_SHADOW,
                    LineHeight = GlobalDataContainer.GAME_FONT_LINEHEIGHT,
                    HorizonAlign = HorizontalAlignment.Left,
                    VertiAlign = VerticalAlignment.Top,
                    X = GlobalDataContainer.GAME_MESSAGELAYER_X,
                    Y = GlobalDataContainer.GAME_MESSAGELAYER_Y,
                    Z = GlobalDataContainer.GAME_Z_MESSAGELAYER + i,
                    Height = GlobalDataContainer.GAME_MESSAGELAYER_H,
                    Width = GlobalDataContainer.GAME_MESSAGELAYER_W,
                    Padding = new MyThickness(GlobalDataContainer.GAME_MESSAGELAYER_PADDING),
                    Opacity = 1.0,
                    Visible = false,
                    Text = ""
                };
                this.MsgLayerDescVec.Add(mld);
            }
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ScreenManager()
        {
            this.BackgroundDescVec = new List<SpriteDescriptor>();
            this.CharacterDescVec = new List<SpriteDescriptor>();
            this.PictureDescVec = new List<SpriteDescriptor>();
            this.BranchDescVec = new List<BranchButtonDescriptor>();
            this.ButtonDescVec = new List<SpriteButtonDescriptor>();
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
            for (int i = 0; i < GlobalDataContainer.GAME_BRANCH_COUNT; i++)
            {
                this.BranchDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_BUTTON_COUNT; i++)
            {
                this.ButtonDescVec.Add(null);
            }
            this.InitMessageLayerDescriptors();
        }

        /// <summary>
        /// 重置唯一实例，用于读取保存数据时
        /// </summary>
        /// <param name="sm">反序列化后的实例</param>
        public static void ResetSynObject(ScreenManager sm)
        {
            ScreenManager.synObject = sm;
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
        All
    }
}
