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
