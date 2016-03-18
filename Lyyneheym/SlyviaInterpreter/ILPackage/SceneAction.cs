using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.YuriInterpreter.ILPackage
{
    /// <summary>
    /// 场景动作类：语义分析器输出的中间代码类
    /// </summary>
    [Serializable]
    internal sealed class SceneAction
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName = null;

        /// <summary>
        /// 节点动作类型
        /// </summary>
        public SActionType Type = SActionType.NOP;

        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> ArgsDict = new Dictionary<string, string>();

        /// <summary>
        /// 条件从句逆波兰表达
        /// </summary>
        public string CondPolish = null;

        /// <summary>
        /// 下一节点
        /// </summary>
        public SceneAction Next = null;

        /// <summary>
        /// 下一真节点向量
        /// </summary>
        public List<SceneAction> TrueRouting = null;

        /// <summary>
        /// 下一假节点向量
        /// </summary>
        public List<SceneAction> FalseRouting = null;

        /// <summary>
        /// 是否依存函数
        /// </summary>
        public bool IsBelongFunc = false;

        /// <summary>
        /// 依存函数名
        /// </summary>
        public string FuncName = null;

        /// <summary>
        /// 附加值
        /// </summary>
        public string Tag = null;

        /// <summary>
        /// 对话合并脏位
        /// </summary>
        public bool DialogDirtyBit = false;

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
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.NodeName + "^");
                string args = this.ArgsDict.Aggregate("", (x, y) => x + ":#:" + y.Key + ":@:" + y.Value);
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
                    string trues = this.TrueRouting.Aggregate("", (x, y) => x + "#" + y.NodeName);
                    sb.Append(trues.Substring(1) + "^");
                }
                else
                {
                    sb.Append("^");
                }
                if (this.FalseRouting != null)
                {
                    string falses = this.TrueRouting.Aggregate("", (x, y) => x + "#" + y.NodeName);
                    sb.Append(falses.Substring(1) + "^");
                }
                else
                {
                    sb.Append("^");
                }
                sb.Append(this.IsBelongFunc ? "1^" : "0^");
                sb.Append(this.FuncName + "^");
                sb.Append(this.Tag != null ? this.Tag.Replace(@"\", @"\\").Replace(@",", @"\,").Replace(@"^", @"\^").Replace("\r\n", @"\$") : "");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 将动作转化为可序列化的已编码字符串
        /// </summary>
        /// <returns>编码完毕的IL字符串</returns>
        private string ToEncodeIL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.NodeName + "^");
            string args = this.ArgsDict.Aggregate("", (x, y) => x + "#" + y.Key + "@" + this.EncodeString(y.Value));
            sb.Append(args.Length > 0 ? args.Substring(1) + "^" : "^");
            sb.Append(this.EncodeString(this.CondPolish) + "^");
            sb.Append(this.Next != null ? this.Next.NodeName + "^" : "^");
            if (this.TrueRouting != null)
            {
                string trues = this.TrueRouting.Aggregate("", (x, y) => x + "#" + y.NodeName);
                sb.Append(trues.Length > 0 ? trues.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            if (this.FalseRouting != null)
            {
                string falses = this.FalseRouting.Aggregate("", (x, y) => x + "#" + y.NodeName);
                sb.Append(falses.Length > 0 ? falses.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            sb.Append(this.IsBelongFunc ? "1^" : "0^");
            sb.Append(this.FuncName + "^");
            sb.Append(this.EncodeString((string)this.Tag));
            return sb.ToString();
        }

        /// <summary>
        /// 把一个字符串做编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isUTF8">标志位，true编码UTF-8，false编码Unicode</param>
        /// <returns>编码完毕的字符串</returns>
        private string EncodeString(string str, bool isUTF8 = true)
        {
            if (str == null) { return null; }
            byte[] br = null;
            if (isUTF8)
            {
                br = Encoding.UTF8.GetBytes(str);
            }
            else
            {
                br = Encoding.Unicode.GetBytes(str);
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in br)
            {
                sb.Append(String.Format("{0:D3}", b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 为当前动作创建一个副本
        /// </summary>
        /// <param name="pureClone">是否保留关系</param>
        /// <returns>原动作的深拷贝副本</returns>
        public SceneAction Clone(bool pureClone)
        {
            SceneAction resSa = new SceneAction();
            resSa.ArgsDict = new Dictionary<string, string>();
            foreach (var kv in this.ArgsDict)
            {
                resSa.ArgsDict.Add(kv.Key, kv.Value);
            }
            resSa.Tag = this.Tag;
            resSa.Type = this.Type;
            resSa.FuncName = this.FuncName;
            resSa.IsBelongFunc = this.IsBelongFunc;
            resSa.NodeName = this.NodeName;
            if (pureClone == false)
            {
                resSa.CondPolish = this.CondPolish;
                resSa.DialogDirtyBit = this.DialogDirtyBit;
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
            }
            return resSa;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString()
        {
            return this.NodeName;
        }
    }

    /// <summary>
    /// 枚举：动作节点类型
    /// </summary>
    public enum SActionType
    {
        // 无动作
        NOP,
        // 段落
        act_dialog,
        // 段落结束符
        act_dialogTerminator,
        // 显示文本
        act_a,
        // 显示背景
        act_bg,
        // 显示图片
        act_picture,
        // 移动图片
        act_move,
        // 消去图片
        act_deletepicture,
        // 显示立绘
        act_cstand,
        // 消去立绘
        act_deletecstand,
        // 播放声效
        act_se,
        // 播放音乐
        act_bgm,
        // 播放bgs
        act_bgs,
        // 停止音乐
        act_stopbgm,
        // 停止bgs
        act_stopbgs,
        // 播放语音
        act_vocal,
        // 停止语音
        act_stopvocal,
        // 返回标题
        act_title,
        // 调用菜单
        act_menu,
        // 调用存档
        act_save,
        // 调用读档
        act_load,
        // 标签
        act_label,
        // 标签跳转
        act_jump,
        // 循环（头）
        act_for,
        // 循环（尾）
        act_endfor,
        // 条件（头）
        act_if,
        // 条件（分支）
        act_else,
        // 条件（尾）
        act_endif,
        // 函数声明（头）
        act_function,
        // 函数声明（尾）
        act_endfunction,
        // 剧本跳转
        act_scene,
        // 开关操作
        act_switch,
        // 变量操作
        act_var,
        // 退出循环
        act_break,
        // 退出程序
        act_shutdown,
        // 中断事件处理
        act_return,
        // 等待
        act_wait,
        // 选择支
        act_branch,
        // 函数调用
        act_call,
        // 回归点
        act_titlepoint,
        // 准备渐变
        act_freeze,
        // 执行渐变
        act_trans,
        // 按钮
        act_button,
        // 对话样式
        act_style,
        // 切换文字层
        act_msglayer,
        // 修改层属性
        act_msglayeropt,
        // 等待用户操作
        act_waituser,
        // 等待动画结束
        act_waitani,
        // 描绘字符串
        act_draw,
        // 移除按钮
        act_deletebutton
    }
}
