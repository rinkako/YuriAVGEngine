using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 可复制的状态类
    /// </summary>
    [Serializable]
    internal class ForkableState
    {
        /// <summary>
        /// 为该状态生成一个分支
        /// </summary>
        /// <returns>状态的分支，它的所有成员变量会被递归地深拷贝</returns>
        public ForkableState Fork()
        {
            return ForkableState.DeepCopyBySerialization<ForkableState>(this);
        }

        /// <summary>
        /// 将某个可复制状态生成一份分支
        /// </summary>
        /// <typeparam name="T">要分支的类型</typeparam>
        /// <param name="obj">分支实例</param>
        /// <returns>新的分支</returns>
        public static T DeepCopyBySerialization<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
    }
}
