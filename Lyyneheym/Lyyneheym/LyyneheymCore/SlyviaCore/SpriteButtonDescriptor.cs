using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 精灵按钮描述类
    /// </summary>
    [Serializable]
    public class SpriteButtonDescriptor
    {
        public SpriteButtonDescriptor()
        {
            this.normalDescriptor = this.overDescriptor = this.onDescriptor = null;
            this.jumpLabel = String.Empty;
            this.X = this.Y = 0;
            this.Z = GlobalDataContainer.GAME_Z_BUTTON;
            this.Opacity = 1;
            this.Enable = true;
        }

        public SpriteDescriptor normalDescriptor { get; set; }

        public SpriteDescriptor overDescriptor { get; set; }

        public SpriteDescriptor onDescriptor { get; set; }

        public string jumpLabel { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int Z { get; set; }

        public double Opacity { get; set; }

        public bool Enable { get; set; }
    }
}
