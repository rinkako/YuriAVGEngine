using System;
using System.Collections.Generic;
using System.Text;
using Yuri.ILPackage;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 函数调用类：处理场景里的函数
    /// </summary>
    [Serializable]
    internal class SceneFunction
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SceneFunction(string callname, string parent, SceneAction sa = null)
        {
            this.ParentSceneName = parent;
            this.Callname = callname;
            this.Sa = sa;
            this.LabelDictionary = new Dictionary<string, SceneAction>();
        }

        /// <summary>
        /// 复制当前函数实例
        /// </summary>
        /// <param name="pureFork">是否不要复制符号表</param>
        /// <returns>新的符号实例</returns>
        public SceneFunction Fork(bool pureFork)
        {
            SceneFunction nsf = new SceneFunction(this.Callname, this.ParentSceneName, this.Sa);
            nsf.Param = this.Param;
            if (!pureFork)
            {
                foreach (var svar in this.Symbols)
                {
                    nsf.Symbols.Add(svar.Key, svar.Value);
                }
            }
            return nsf;
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
            return String.Format("Function: {0}({1})", this.Callname, paraStr);
        }

        /// <summary>
        /// 获取或设置函数的全局名称
        /// </summary>
        public string GlobalName
        {
            get
            {
                return String.Format("__YuriFunc@{0}?{1}", this.Callname, this.ParentSceneName);
            }
        }

        /// <summary>
        /// 绑定动作序列
        /// </summary>
        public SceneAction Sa = null;

        /// <summary>
        /// 函数名
        /// </summary>
        public string Callname = null;

        /// <summary>
        /// 形参列表
        /// </summary>
        public List<string> Param = null;

        /// <summary>
        /// 场景名称
        /// </summary>
        public string ParentSceneName = null;

        /// <summary>
        /// 绑定符号表
        /// </summary>
        public Dictionary<string, object> Symbols = new Dictionary<string,object>();

        /// <summary>
        /// 场景标签字典
        /// </summary>
        public Dictionary<string, SceneAction> LabelDictionary { get; set; }
    }
}
