using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 选择项按钮描述类
    /// </summary>
    public class BranchButtonDescriptor
    {
        public int Id { get; set; }

        public string JumpTarget { get; set; }

        public string Text { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int Z { get; set; }

        public SpriteDescriptor normalDescriptor { get; set; }

        public SpriteDescriptor overDescriptor { get; set; }

        public SpriteDescriptor onDescriptor { get; set; }
    }
}
