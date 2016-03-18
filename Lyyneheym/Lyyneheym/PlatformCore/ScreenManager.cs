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
    internal class ScreenManager
    {
        /// <summary>
        /// 为屏幕增加一个背景精灵描述子
        /// </summary>
        /// <param name="id">背景层的类型：0-背景 1-前景</param>
        /// <param name="source">资源名称</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="Z">Z坐标</param>
        /// <param name="Angle">角度</param>
        /// <param name="Opacity">不透明度</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="cut">纹理切割矩</param>
        public void AddBackground(int id, string source, double X, double Y, int Z, double Angle, double Opacity, double ScaleX, double ScaleY, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                Id = id,
                ResourceType = ResourceType.Background,
                ResourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_BACKGROUND,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                Angle = Angle,
                Opacity = Opacity,
                AnchorType = anchor,
                CutRect = cut
            };
            this.backgroundDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个立绘精灵描述子
        /// </summary>
        /// <param name="id">立绘位置id号</param>
        /// <param name="source">资源名称</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="Z">Z坐标</param>
        /// <param name="Angle">角度</param>
        /// <param name="Opacity">不透明度</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="cut">纹理切割矩</param>
        public void AddCharacterStand(int id, string source, double X, double Y, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                Id = id,
                ResourceType = ResourceType.Stand,
                ResourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_CHARACTERSTAND,
                Angle = Angle,
                Opacity = Opacity,
                AnchorType = anchor,
                CutRect = cut
            };
            this.characterDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个立绘精灵描述子
        /// </summary>
        /// <param name="id">立绘位置id号</param>
        /// <param name="source">资源名称</param>
        /// <param name="cst">立绘位置枚举</param>
        /// <param name="Z">Z坐标</param>
        /// <param name="Angle">角度</param>
        /// <param name="Opacity">不透明度</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="cut">纹理切割矩</param>
        public void AddCharacterStand(int id, string source, CharacterStandType cst, int Z, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = null;
            switch (cst)
            {
                case CharacterStandType.Left:
                    sd = new SpriteDescriptor()
                    {
                        Id = id,
                        ResourceType = ResourceType.Stand,
                        ResourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_LEFT_Y,
                        Z = Z + GlobalDataContainer.GAME_Z_CHARACTERSTAND,
                        Angle = Angle,
                        Opacity = Opacity,
                        AnchorType = anchor,
                        CutRect = cut
                    };
                    break;
                case CharacterStandType.MidLeft:
                    sd = new SpriteDescriptor()
                    {
                        Id = id,
                        ResourceType = ResourceType.Stand,
                        ResourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MIDLEFT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MIDLEFT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        AnchorType = anchor,
                        CutRect = cut
                    };
                    break;
                case CharacterStandType.Mid:
                    sd = new SpriteDescriptor()
                    {
                        Id = id,
                        ResourceType = ResourceType.Stand,
                        ResourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MID_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MID_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        AnchorType = anchor,
                        CutRect = cut
                    };
                    break;
                case CharacterStandType.MidRight:
                    sd = new SpriteDescriptor()
                    {
                        Id = id,
                        ResourceType = ResourceType.Stand,
                        ResourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_MIDRIGHT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_MIDRIGHT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        AnchorType = anchor,
                        CutRect = cut
                    };
                    break;
                case CharacterStandType.Right:
                    sd = new SpriteDescriptor()
                    {
                        Id = id,
                        ResourceType = ResourceType.Stand,
                        ResourceName = source,
                        X = GlobalDataContainer.GAME_CHARACTERSTAND_RIGHT_X,
                        Y = GlobalDataContainer.GAME_CHARACTERSTAND_RIGHT_Y,
                        Z = Z,
                        Angle = Angle,
                        Opacity = Opacity,
                        AnchorType = anchor,
                        CutRect = cut
                    };
                    break;
            }
            this.characterDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个图片精灵描述子
        /// </summary>
        /// <param name="id">图片的id</param>
        /// <param name="source">资源名称</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="Z">Z坐标</param>
        /// <param name="Angle">角度</param>
        /// <param name="Opacity">不透明度</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="cut">纹理切割矩</param>
        public void AddPicture(int id, string source, double X, double Y, int Z, double ScaleX, double ScaleY, double Angle, double Opacity, SpriteAnchorType anchor, Int32Rect cut)
        {
            SpriteDescriptor sd = new SpriteDescriptor()
            {
                Id = id,
                ResourceType = ResourceType.Pictures,
                ResourceName = source,
                X = X,
                Y = Y,
                Z = Z + GlobalDataContainer.GAME_Z_PICTURES,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                Angle = Angle,
                Opacity = Opacity,
                AnchorType = anchor,
                CutRect = cut
            };
            this.pictureDescVec[id] = sd;
        }

        /// <summary>
        /// 为屏幕增加一个按钮描述子
        /// </summary>
        /// <param name="id">按钮的id</param>
        /// <param name="enable">是否可点击</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="jumpTarget">按下后要跳转的标签名</param>
        /// <param name="funcCallSign">函数调用签名</param>
        /// <param name="type">按钮存续类型</param>
        /// <param name="normalDesc">正常时的精灵描述子</param>
        /// <param name="overDesc">鼠标悬停时的精灵描述子</param>
        /// <param name="onDesc">鼠标按下时的精灵描述子</param>
        public void AddButton(int id, bool enable, double X, double Y, string jumpTarget, string funcCallSign, string type, SpriteDescriptor normalDesc, SpriteDescriptor overDesc = null, SpriteDescriptor onDesc = null)
        {
            SpriteButtonDescriptor sbd = new SpriteButtonDescriptor()
            {
                Enable = enable,
                X = X,
                Y = Y,
                Z = GlobalDataContainer.GAME_Z_BUTTON + id,
                JumpLabel = jumpTarget,
                InterruptFuncSign = funcCallSign,
                NormalDescriptor = normalDesc,
                OverDescriptor = overDesc,
                OnDescriptor = onDesc,
                Opacity = 1.0,
                Eternal = type == "once" ? false : true
            };
            this.buttonDescVec[id] = sbd;
        }

        /// <summary>
        /// 为屏幕增加一个选择支描述子
        /// </summary>
        /// <param name="id">选择支按钮id</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="jumpTarget">按下后要跳转的标签名</param>
        /// <param name="text">选择支按钮上的文本</param>
        /// <param name="normalDesc">正常时的精灵描述子</param>
        /// <param name="overDesc">鼠标悬停时的精灵描述子</param>
        /// <param name="onDesc">鼠标按下时的精灵描述子</param>
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
                NormalDescriptor = normalDesc,
                OverDescriptor = overDesc,
                OnDescriptor = onDesc
            };
            this.branchDescVec[id] = bbd;
        }

        /// <summary>
        /// 为屏幕增加一个文字层描述子
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <param name="source">文字层背景图资源名</param>
        /// <param name="Visible">文字层可见性</param>
        /// <param name="W">层宽度</param>
        /// <param name="H">层高度</param>
        /// <param name="Padding">文字在层内与层边框的间距</param>
        /// <param name="X">左上角在画面的X坐标</param>
        /// <param name="Y">左上角在画面的Y坐标</param>
        /// <param name="Z">Z坐标</param>
        /// <param name="Opacity">不透明度</param>
        /// <param name="FontName">字体名称</param>
        /// <param name="FontColor">字体颜色</param>
        /// <param name="FontSize">字号</param>
        /// <param name="Ha">层在屏幕上横向对齐属性</param>
        /// <param name="Va">层在屏幕上纵向对齐属性</param>
        /// <param name="LineHeight">行距</param>
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
            this.msgLayerDescVec[id] = mld;
        }

        /// <summary>
        /// 从屏幕上移除一个精灵描述子
        /// </summary>
        /// <param name="spriteId">精灵id</param>
        /// <param name="rType">资源类型</param>
        public void RemoveSprite(int spriteId, ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Background:
                    this.backgroundDescVec[spriteId] = null;
                    break;
                case ResourceType.Stand:
                    this.characterDescVec[spriteId] = null;
                    break;
                case ResourceType.Pictures:
                    this.pictureDescVec[spriteId] = null;
                    break;
            }
        }

        /// <summary>
        /// 从屏幕上移除一个文字层
        /// </summary>
        /// <param name="layerId">文字层id</param>
        public void RemoveMsgLayer(int layerId)
        {
            this.msgLayerDescVec[layerId] = null;
        }

        /// <summary>
        /// 从屏幕上移除一个按钮
        /// </summary>
        /// <param name="id">按钮id</param>
        public void RemoveButton(int id)
        {
            this.buttonDescVec[id] = null;
        }

        /// <summary>
        /// 从屏幕上移除一个选择支
        /// </summary>
        /// <param name="id">选择支id</param>
        public void RemoveBranchButton(int id)
        {
            this.branchDescVec[id] = null;
        }

        /// <summary>
        /// 获取一个精灵的描述子
        /// </summary>
        /// <param name="id">精灵id</param>
        /// <param name="rType">资源类型</param>
        /// <returns>描述子实例</returns>
        public SpriteDescriptor GetSpriteDescriptor(int id, ResourceType rType)
        {
            try
            {
                switch (rType)
                {
                    case ResourceType.Background:
                        return this.backgroundDescVec[id];
                    case ResourceType.Stand:
                        return this.characterDescVec[id];
                    case ResourceType.Pictures:
                        return this.pictureDescVec[id];
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                CommonUtils.ConsoleLine(ex.ToString(), "ScreenManager / CLR", OutputStyle.Error);
                return null;
            }
        }

        /// <summary>
        /// 获取一个文字层描述子
        /// </summary>
        /// <param name="id">文字层id</param>
        /// <returns>描述子实例</returns>
        public MessageLayerDescriptor GetMsgLayerDescriptor(int id)
        {
            return this.msgLayerDescVec[id];
        }

        /// <summary>
        /// 获取一个按钮描述子
        /// </summary>
        /// <param name="id">按钮id</param>
        /// <returns>描述子实例</returns>
        public SpriteButtonDescriptor GetButtonDescriptor(int id)
        {
            return this.buttonDescVec[id];
        }

        /// <summary>
        /// 获取一个选择支描述子
        /// </summary>
        /// <param name="bbId">选择支id</param>
        /// <returns描述子实例></returns>
        public BranchButtonDescriptor GetBranchButtonDescriptor(int id)
        {
            return this.branchDescVec[id];
        }

        /// <summary>
        /// 交换背景的前后层
        /// </summary>
        public void Backlay()
        {
            CommonUtils.Swap<SpriteDescriptor>(this.backgroundDescVec, 0, 1);
        }

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
            this.msgLayerDescVec.Add(mainMsgLayer);
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
                this.msgLayerDescVec.Add(mld);
            }
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ScreenManager()
        {
            this.backgroundDescVec = new List<SpriteDescriptor>();
            this.characterDescVec = new List<SpriteDescriptor>();
            this.pictureDescVec = new List<SpriteDescriptor>();
            this.branchDescVec = new List<BranchButtonDescriptor>();
            this.buttonDescVec = new List<SpriteButtonDescriptor>();
            this.msgLayerDescVec = new List<MessageLayerDescriptor>();
            for (int i = 0; i < GlobalDataContainer.GAME_BACKGROUND_COUNT; i++)
            {
                this.backgroundDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_CHARACTERSTAND_COUNT; i++)
            {
                this.characterDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_IMAGELAYER_COUNT; i++)
            {
                this.pictureDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_BRANCH_COUNT; i++)
            {
                this.branchDescVec.Add(null);
            }
            for (int i = 0; i < GlobalDataContainer.GAME_BUTTON_COUNT; i++)
            {
                this.buttonDescVec.Add(null);
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

        /// <summary>
        /// 背景描述向量
        /// </summary>
        private List<SpriteDescriptor> backgroundDescVec;

        /// <summary>
        /// 立绘描述向量
        /// </summary>
        private List<SpriteDescriptor> characterDescVec;

        /// <summary>
        /// 图片描述向量
        /// </summary>
        private List<SpriteDescriptor> pictureDescVec;

        /// <summary>
        /// 选择项描述向量
        /// </summary>
        private List<BranchButtonDescriptor> branchDescVec;

        /// <summary>
        /// 文字层描述向量
        /// </summary>
        private List<MessageLayerDescriptor> msgLayerDescVec;

        /// <summary>
        /// 按钮层描述向量
        /// </summary>
        private List<SpriteButtonDescriptor> buttonDescVec;
    }

    /// <summary>
    /// 枚举：立绘位置
    /// </summary>
    internal enum CharacterStandType
    {
        Left,
        MidLeft,
        Mid,
        MidRight,
        Right,
        All
    }
}
