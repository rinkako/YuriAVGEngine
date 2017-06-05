using System;
using System.Reflection;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 可复制的描述子类
    /// </summary>
    [Serializable]
    internal class CloneableDescriptor
    {
        /// <summary>
        /// 为该描述子创建一份深拷贝
        /// </summary>
        /// <returns>描述子的深拷贝，它的成员变量会被递归地深拷贝</returns>
        public CloneableDescriptor Clone()
        {
            return CloneableDescriptor.DeepCopyByReflection<CloneableDescriptor>(this);
        }

        /// <summary>
        /// 递归深拷贝对象
        /// </summary>
        /// <typeparam name="T">要拷贝的类型</typeparam>
        /// <param name="obj">拷贝母版</param>
        /// <returns>深拷贝的对象</returns>
        public static T DeepCopyByReflection<T>(T obj)
        {
            // 如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType)
            {
                return obj;
            }
            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}
