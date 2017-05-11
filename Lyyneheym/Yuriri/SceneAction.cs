using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.Yuriri
{
    /// <summary>
    /// 场景动作类：语义分析器输出的中间代码类
    /// </summary>
    [Serializable]
    public sealed class SceneAction
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; } = null;
        
        /// <summary>
        /// 节点动作
        /// </summary>
        public SActionType Type { get; set; } = SActionType.NOP;
        
        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> ArgsDict { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// 条件从句逆波兰表达
        /// </summary>
        public string CondPolish { get; set; } = null;
        
        /// <summary>
        /// 下一节点
        /// </summary>
        public SceneAction Next { get; set; } = null;

        /// <summary>
        /// 真节点向量
        /// </summary>
        public List<SceneAction> TrueRouting { get; set; } = null;

        /// <summary>
        /// 假节点向量
        /// </summary>
        public List<SceneAction> FalseRouting { get; set; } = null;

        /// <summary>
        /// 是否依存函数
        /// </summary>
        public bool IsBelongFunc { get; set; } = false;

        /// <summary>
        /// 依存函数名
        /// </summary>
        public string ReliedFuncName { get; set; } = null;

        /// <summary>
        /// 附加值
        /// </summary>
        public string Tag { get; set; } = null;

        /// <summary>
        /// 对话合并脏位
        /// </summary>
        public bool DialogDirtyBit { get; set; } = false;

        /// <summary>
        /// 带SAP项的构造函数
        /// </summary>
        /// <param name="sap">SceneActionPackage项目</param>
        public SceneAction(SceneActionPackage sap)
        {
            this.NodeName = sap.saNodeName;
            this.Type = (SActionType)Enum.Parse(typeof(SActionType), sap.saNodeName.Split('@')[1]);
            this.ArgsDict = new Dictionary<string, string>(sap.argsDict);
            this.CondPolish = sap.condPolish;
            this.IsBelongFunc = sap.isBelongFunc;
            this.ReliedFuncName = sap.funcName;
            this.Tag = sap.aTag;
        }

        /// <summary>
        /// 取下一动作
        /// </summary>
        /// <param name="rhs">自增符号的作用对象</param>
        /// <returns>该作用对象动作的下一动作</returns>
        public static SceneAction operator++(SceneAction rhs)
        {
            return rhs?.Next;
        }

        /// <summary>
        /// 将动作转化为可序列化字符串
        /// </summary>
        /// <returns>IL字符串</returns>
        public string ToIL(bool isClearText = false)
        {
            if (!isClearText)
            {
                return this.ToEncodeIL();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(this.NodeName + "^");
            string args = this.ArgsDict.Aggregate(String.Empty, (x, y) => x + ":#:" + y.Key + ":@:" + y.Value);
            sb.Append(args.Length > 0 ? args.Substring(3) + "^" : "^");
            if (this.Type != SActionType.act_else && this.Type != SActionType.act_endif && this.Type != SActionType.act_endfor
                && this.Type != SActionType.act_function && this.Type != SActionType.act_endfunction && this.Type != SActionType.act_label)
            {
                sb.Append(this.CondPolish + "^");
            }
            else
            {
                sb.Append("^");
            }
            sb.Append(this.Next != null ? this.Next.NodeName + "^" : "^");
            if (this.TrueRouting != null)
            {
                string trues = this.TrueRouting.Aggregate(String.Empty, (x, y) => x + "#" + y.NodeName);
                sb.Append(trues.Substring(1) + "^");
            }
            else
            {
                sb.Append("^");
            }
            if (this.FalseRouting != null)
            {
                string falses = this.FalseRouting.Aggregate(String.Empty, (x, y) => x + "#" + y.NodeName);
                sb.Append(falses.Substring(1) + "^");
            }
            else
            {
                sb.Append("^");
            }
            sb.Append(this.IsBelongFunc ? "1^" : "0^");
            sb.Append(this.ReliedFuncName + "^");
            sb.Append(Tag?.Replace(@"\", @"\\").Replace(@",", @"\,").Replace(@"^", @"\^").Replace("\r\n", @"\$") ?? String.Empty);
            return sb.ToString();
        }

        /// <summary>
        /// 将动作转化为可序列化的已编码字符串
        /// </summary>
        /// <returns>编码完毕的IL字符串</returns>
        private string ToEncodeIL()

        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.NodeName + "^");
            string args = this.ArgsDict.Aggregate(String.Empty, (x, y) => x + "#" + y.Key + "@" + this.EncodeString(y.Value));
            sb.Append(args.Length > 0 ? args.Substring(1) + "^" : "^");
            sb.Append(this.EncodeString(this.CondPolish) + "^");
            sb.Append(this.Next != null ? this.Next.NodeName + "^" : "^");
            if (this.TrueRouting != null)
            {
                string trues = this.TrueRouting.Aggregate(String.Empty, (x, y) => x + "#" + y.NodeName);
                sb.Append(trues.Length > 0 ? trues.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            if (this.FalseRouting != null)
            {
                string falses = this.FalseRouting.Aggregate(String.Empty, (x, y) => x + "#" + y.NodeName);
                sb.Append(falses.Length > 0 ? falses.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            sb.Append(this.IsBelongFunc ? "1^" : "0^");
            sb.Append(this.ReliedFuncName + "^");
            sb.Append(this.EncodeString(this.Tag));
            return sb.ToString();
        }

        /// <summary>
        /// 把一个字符串做编码
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        /// <param name="isUTF8">标志位，true编码UTF-8，false编码Unicode</param>
        /// <returns>编码完毕的字符串</returns>
        private string EncodeString(string str, bool isUTF8 = true)
        {
            if (str == null) { return null; }
            var br = isUTF8 ? Encoding.UTF8.GetBytes(str) : Encoding.Unicode.GetBytes(str);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in br)
            {
                sb.Append(String.Format("{0:D3}", b));
            }
            return sb.ToString();
        }


        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SceneAction()
        {

        }

        /// <summary>
        /// 为当前动作创建一个副本
        /// </summary>
        /// <param name="pureClone">是否保留关系</param>
        /// <returns>原动作的深拷贝副本</returns>
        public SceneAction Clone(bool pureClone)
        {
            SceneAction resSa = new SceneAction { ArgsDict = new Dictionary<string, string>() };
            foreach (var kv in this.ArgsDict)
            {
                resSa.ArgsDict.Add(kv.Key, kv.Value);
            }
            resSa.Tag = this.Tag;
            resSa.Type = this.Type;
            resSa.ReliedFuncName = this.ReliedFuncName;
            resSa.IsBelongFunc = this.IsBelongFunc;
            resSa.NodeName = this.NodeName;
            if (pureClone)
            {
                return resSa;
            }
            resSa.CondPolish = this.CondPolish;
            resSa.Next = this.Next;
            resSa.NodeName = this.NodeName;
            resSa.TrueRouting = new List<SceneAction>();
            foreach (var tr in this.TrueRouting)
            {
                resSa.TrueRouting.Add(tr);
            }
            resSa.FalseRouting = new List<SceneAction>();
            foreach (var fr in this.FalseRouting)
            {
                resSa.FalseRouting.Add(fr);
            }
            return resSa;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString() => this.NodeName;
    }
}
