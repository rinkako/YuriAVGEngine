using System;
using System.Collections.Generic;
using System.Text;

namespace Yuri.Yuriri
{
    /// <summary>
    /// <para>场景函数类：封装场景里的函数</para>
    /// <para>在发生函数调用时，场景函数将作为模板产生一个拷贝对象提交给调用堆栈，它负责维护自身的符号表</para>
    /// </summary>
    [Serializable]
    public class SceneFunction : RunnableYuriri
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SceneFunction(string callname, string parent, SceneAction sa = null)
        {
            this.Ctor = sa;
            this.Callname = callname;
            this.ParentSceneName = parent;
            this.Symbols = new Dictionary<string, object>();
            this.LabelDictionary = new Dictionary<string, SceneAction>();
        }

        /// <summary>
        /// 复制当前函数实例
        /// </summary>
        /// <param name="pureFork">是否不要复制符号表</param>
        /// <returns>新的符号实例</returns>
        public SceneFunction Fork(bool pureFork)
        {
            // 拷贝对象共享代码和标签字典
            SceneFunction nsf = new SceneFunction(this.Callname, this.ParentSceneName, this.Ctor)
            {
                Param = this.Param,
                LabelDictionary = this.LabelDictionary
            };
            // 使用新的上下文
            if (pureFork) { return nsf; }
            foreach (var svar in this.Symbols)
            {
                nsf.Symbols.Add(svar.Key, svar.Value);
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
            string paraStr = String.Empty;
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
        public string GlobalName => String.Format("__YuriFunc@{0}?{1}", this.Callname, this.ParentSceneName);
        
        /// <summary>
        /// 函数名
        /// </summary>
        public string Callname { get; set; }

        /// <summary>
        /// 形参列表
        /// </summary> 
        public List<string> Param { get; set; } = null;

        /// <summary>
        /// 场景名称
        /// </summary>
        public string ParentSceneName { get; set; }

        /// <summary>
        /// 绑定符号表
        /// </summary>
        public Dictionary<string, object> Symbols { get; set; }
    }
}
