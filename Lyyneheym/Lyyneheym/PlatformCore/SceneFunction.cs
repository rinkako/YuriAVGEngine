using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 函数调用类：处理场景里的函数
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该函数的C风格签名</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string paraStr = "";
            foreach (string arg in this.param)
            {
                sb.Append(arg + ", ");
            }
            if (sb.Length > 0)
            {
                paraStr = sb.ToString().Substring(0, sb.Length - 2);
            }
            return String.Format("SlyviaFunction: {0}({1})", this.callname, paraStr);
        }

        /// <summary>
        /// 获取或设置函数的全局名称
        /// </summary>
        public string globalName
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
        // 绑定符号表
        public Dictionary<string, object> symbolsRef = null;
    }
}
