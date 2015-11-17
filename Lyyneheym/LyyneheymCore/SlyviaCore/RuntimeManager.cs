using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyyneheymCore.SlyviaCore;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>运行时管理器：维护运行时的所有信息</para>
    /// <para>她是一个单例类，只有唯一实例，并且可以序列化</para>
    /// </summary>
    [Serializable]
    public class RuntimeManager
    {





        /// <summary>
        /// 增加一个用户变量到运行时变量哈希表
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">变量的值</param>
        /// <returns>操作成功与否</returns>
        public bool addUserVariable(string varname, object value)
        {
            if (this.UserVarHashTable.ContainsKey(varname) == true)
            {
                this.UserVarHashTable.Add(varname, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改一个用户变量的值
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">新的变量值</param>
        /// <returns>操作成功与否</returns>
        public bool updateUserVariable(string varname, object value)
        {
            if (this.UserVarHashTable.ContainsKey(varname) == true)
            {
                this.UserVarHashTable[varname] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改一个系统变量的值
        /// </summary>
        /// <param name="varname">变量名</param>
        /// <param name="value">新的变量值</param>
        /// <returns>操作成功与否</returns>
        public bool updateCoreVariable(string varname, object value)
        {
            if (this.CoreVarHashTable.ContainsKey(varname) == true)
            {
                this.CoreVarHashTable[varname] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static RuntimeManager getInstance()
        {
            return null == synObject ? synObject = new RuntimeManager() : synObject;
        }

        private RuntimeManager()
        {
            this.CoreVarHashTable = new Hashtable();
            this.UserVarHashTable = new Hashtable();
        }

        private Hashtable CoreVarHashTable = null;
        private Hashtable UserVarHashTable = null;

        private static RuntimeManager synObject = null;
    }
}
