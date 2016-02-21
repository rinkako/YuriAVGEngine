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
        // 节点名称
        public string saNodeName = null;
        // 节点动作
        public SActionType aType = SActionType.NOP;
        // 参数字典
        public Dictionary<string, string> argsDict = new Dictionary<string, string>();
        // 条件从句逆波兰表达
        public string condPolish = null;
        // 下一节点
        public SceneAction next = null;
        // 下一真节点向量
        public List<SceneAction> trueRouting = null;
        // 下一假节点向量
        public List<SceneAction> falseRouting = null;
        // 是否依存函数
        public bool isBelongFunc = false;
        // 依存函数名
        public string funcName = null;
        // 附加值
        public string aTag = null;
        // 对话合并脏位
        public bool dialogDirtyBit = false;

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
                sb.Append(this.saNodeName + "^");
                string args = this.argsDict.Aggregate("", (x, y) => x + ":#:" + y.Key + ":@:" + y.Value);
                sb.Append(args.Length > 0 ? args.Substring(3) + "^" : "^");
                if (this.aType != SActionType.act_else && this.aType != SActionType.act_endif && this.aType != SActionType.act_endfor
                    && this.aType != SActionType.act_function && this.aType != SActionType.act_endfunction && this.aType != SActionType.act_label)
                {
                    sb.Append(this.condPolish + "^");
                }
                else
                {
                    sb.Append("^");
                }
                sb.Append(this.next != null ? this.next.saNodeName + "^" : "^");
                if (this.trueRouting != null)
                {
                    string trues = this.trueRouting.Aggregate("", (x, y) => x + "#" + y.saNodeName);
                    sb.Append(trues.Substring(1) + "^");
                }
                else
                {
                    sb.Append("^");
                }
                if (this.falseRouting != null)
                {
                    string falses = this.trueRouting.Aggregate("", (x, y) => x + "#" + y.saNodeName);
                    sb.Append(falses.Substring(1) + "^");
                }
                else
                {
                    sb.Append("^");
                }
                sb.Append(this.isBelongFunc ? "1^" : "0^");
                sb.Append(this.funcName + "^");
                sb.Append(this.aTag != null ? this.aTag.Replace(@"\", @"\\").Replace(@",", @"\,").Replace(@"^", @"\^").Replace("\r\n", @"\$") : "");
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
            sb.Append(this.saNodeName + "^");
            string args = this.argsDict.Aggregate("", (x, y) => x + "#" + y.Key + "@" + this.EncodeString(y.Value));
            sb.Append(args.Length > 0 ? args.Substring(1) + "^" : "^");
            sb.Append(this.EncodeString(this.condPolish) + "^");
            sb.Append(this.next != null ? this.next.saNodeName + "^" : "^");
            if (this.trueRouting != null)
            {
                string trues = this.trueRouting.Aggregate("", (x, y) => x + "#" + y.saNodeName);
                sb.Append(trues.Length > 0 ? trues.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            if (this.falseRouting != null)
            {
                string falses = this.falseRouting.Aggregate("", (x, y) => x + "#" + y.saNodeName);
                sb.Append(falses.Length > 0 ? falses.Substring(1) + "^" : "^");
            }
            else
            {
                sb.Append("^");
            }
            sb.Append(this.isBelongFunc ? "1^" : "0^");
            sb.Append(this.funcName + "^");
            sb.Append(this.EncodeString((string)this.aTag));
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
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString()
        {
            return this.saNodeName;
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
        // 停止音乐
        act_stopbgm,
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
