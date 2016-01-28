using System;
using System.Collections.Generic;
using System.Text;


namespace Lyyneheym.LyyneheymCore.ILPackage
{
    /// <summary>
    /// <para>中间语言转换类：将SIL语言转换为场景动作序列</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class ILConvertor
    {








        private void ParseSceneAction(string oneline)
        {
            string[] lineitem = oneline.Split(',');
            if (lineitem.Length == this.IL_LINEITEM_NUM)
            {
                SceneActionPackage sa = new SceneActionPackage();
                sa.saNodeName = lineitem[0];
                sa.aType = (SActionType)Enum.Parse(typeof(SActionType), lineitem[0].Split('@')[1], false);
                sa.condPolish = lineitem[2];
                sa.next = lineitem[3];
                
            }
            else
            {
                throw new Exception("IL损坏");
            }
        }


        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>SIL语言解释器</returns>
        public static ILConvertor GetInstance()
        {
            return instance == null ? instance = new ILConvertor() : instance;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ILConvertor()
        {
            
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static ILConvertor instance = null;

        private readonly int IL_LINEITEM_NUM = 9;
    }
}
