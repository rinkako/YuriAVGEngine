using System;
using System.Collections.Generic;
using System.Text;

namespace Yuri.YuriInterpreter.ILPackage
{
    /// <summary>
    /// 函数调用类：处理场景里的函数
    /// </summary>
    internal sealed class SceneFunction
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SceneFunction(string callname, string parent, SceneAction sa = null)
        {
            this.ParentSceneName = parent;
            this.Callname = callname;
            this.Param = null;
            this.Sa = sa;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该函数的签名</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string paraStr = "";
            foreach (string arg in this.Param)
            {
                sb.Append(arg + ", ");
            }
            if (sb.Length > 0)
            {
                paraStr = sb.ToString().Substring(0, sb.Length - 2);
            }
            return String.Format("SlyviaFunction: {0}({1})", this.Callname, paraStr);
        }

        /// <summary>
        /// 绑定动作序列
        /// </summary>
        public SceneAction Sa { get; set; }

        /// <summary>
        /// 函数名
        /// </summary>
        public string Callname { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<string> Param { get; set; }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string ParentSceneName { get; set; }
    }
}
