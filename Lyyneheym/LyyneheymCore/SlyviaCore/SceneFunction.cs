using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyyneheymCore.SlyviaPile;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 函数调用类：处理场景里的函数
    /// </summary>
    public class SceneFunction
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SceneFunction(string callname, string parent, SceneAction sa = null)
        {
            this.parentSceneName = parent;
            this.callname = callname;
            this.sa = sa;
        }

        // 在变量字典中的名字
        public string varDictName
        {
            get
            {
                return String.Format("__SlyviaFunc@{0}?{1}", this.callname, this.parentSceneName);
            }
        }
        // 绑定动作序列
        public SceneAction sa = null;
        // 函数名
        public string callname = null;
        // 参数列表
        public List<string> param = null;
        // 场景名称
        public string parentSceneName = null;
    }
}
