using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 语法节点类：构成语法树的最小单元
    /// </summary>
    public sealed class SyntaxTreeNode
    {
        // 构造函数
        public SyntaxTreeNode(SyntaxType type = SyntaxType.Unknown)
        {
            this.nodeSyntaxType = type;
        }
        // 绑定处理函数
        public CandidateFunction candidateFunction = null;
        // 子树向量
        public List<SyntaxTreeNode> children = null;
        // 父指针
        public SyntaxTreeNode parent = null;
        // 命中语法结构类型
        public SyntaxType nodeSyntaxType = SyntaxType.Unknown;
        // 命中token附加值
        public string nodeValue = null;
        // 命中产生式类型
        public CFunctionType nodeType = CFunctionType.None;
        // 节点名字
        public string nodeName = "";
        // 附加值
        public object aTag = null;
        // 错误位
        public bool errorBit = false;
        // 是否为不推导节点
        public bool isParaRoot = false;
        // 不推导节点参数孩子字典
        public Dictionary<string, SyntaxTreeNode> paramDict = null;
        // 不推导节点参数Token子流
        public List<Token> paramTokenStream = null;

        /// <summary>
        /// 树的递归遍历文本化
        /// </summary>
        /// <returns>表示树的字符串</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }

    /// <summary>
    /// 枚举：语法节点类型
    /// </summary>
    public enum SyntaxType
    {
        // 段落
        synr_dialog,
        // 显示文本
        synr_a,
        // 显示图片
        synr_picture,
        // 移动图片
        synr_move,
        // 消去图片
        synr_deletepicture,
        // 显示立绘
        synr_cstand,
        // 消去立绘
        synr_deletecstand,
        // 播放声效
        synr_se,
        // 播放音乐
        synr_bgm,
        // 停止音乐
        synr_stopbgm,
        // 播放语音
        synr_vocal,
        // 停止语音
        synr_stopvocal,
        // 返回标题
        synr_title,
        // 调用菜单
        synr_menu,
        // 调用存档
        synr_save,
        // 调用读档
        synr_load,
        // 标签
        synr_lable,
        // 标签跳转
        synr_jump,
        // 循环（头）
        synr_for,
        // 循环（尾）
        synr_endfor,
        // 条件（头）
        synr_if,
        // 条件（分支）
        synr_else,
        // 条件（尾）
        synr_endif,
        // 剧本跳转
        synr_scene,
        // 开关操作
        synr_switch,
        // 变量操作
        synr_var,
        // 退出循环
        synr_break,
        // 退出程序
        synr_shutdown,
        // 等待
        synr_wait,
        // 选择支
        synr_branch,
        // 参数：选择支链
        para_link,
        // 参数：名称
        para_name,
        // 参数：语音id
        para_vid,
        // 参数：立绘表情
        para_face,
        // 参数：序号
        para_id,
        // 参数：x坐标
        para_x,
        // 参数：y坐标
        para_y,
        // 参数：z坐标
        para_z,
        // 参数：x加速度
        para_xacc,
        // 参数：y加速度
        para_yacc,
        // 参数：透明度
        para_capacity,
        // 参数：x轴缩放比
        para_xscale,
        // 参数：y轴缩放比
        para_yscale,
        // 参数：时间
        para_time,
        // 参数：文件名
        para_filename,
        // 参数：音轨号
        para_track,
        // 参数：条件子句
        para_cond,
        // 参数：表达式
        para_dash,
        // 参数：位置
        para_loc,
        // 参数：角度
        para_ro,
        // 参数：音量
        para_vol,
        // 参数：开光状态
        para_state,
        // 参数值：左边
        para_left,
        // 参数值：中间
        para_mid,
        // 参数值：右边
        para_right,
        // 根节点
        case_kotori,
        // <disjunct> ::= <conjunct> <disjunct_pi>;
        case_disjunct,
        // <disjunct_pi> ::= "||" <conjunct> <disjunct_pi> | null;
        case_disjunct_pi,
        // <conjunct> ::= <bool> <conjunct_pi>;
        case_conjunct,
        // <conjunct_pi> ::= "&&" <bool> <conjunct_pi> | null;
        case_conjunct_pi,
        // <bool> ::= "(" <disjunct> ")" | "!" <bool> | <comp>;
        case_bool,
        // <comp> ::= <wexpr> <rop> <wexpr>;
        case_comp,
        // <rop> ::= "<>" | "==" | ">" | "<" | ">=" | "<=" | null;
        case_rop,
        // <wexpr> ::= <wmulti> <wexpr_pi>;
        case_wexpr,
        // <wexpr> ::= <wplus> <wexpr_pi> | null;
        case_wexpr_pi,
        // <wplus> ::= "+" <wmulti> | "-" <wmulti>;
        case_wplus,
        // <wmulti> ::= <wunit> <wmultiOpt>;
        case_wmulti,
        // <wmultiOpt> ::= "*" <wunit> | "/" <wunit> | null;
        case_wmultiOpt,
        // <wunit> ::= number | identifier | "-" <wunit> | "+" <wunit> | "(" <wexpr> ")";
        case_wunit,
        // 未知的语法结点符号
        Unknown,
        // identifier
        tail_idenLeave,
        // "("
        tail_leftParentheses_Leave,
        // ")"
        tail_rightParentheses_Leave,
        // ";"
        tail_semicolon_Leave,
        // ","
        tail_comma_Leave,
        // null
        epsilonLeave,
        // "="
        tail_equality_Leave,
        // "+"
        tail_plus_Leave,
        // "-"
        tail_minus_Leave,
        // "*"
        tail_multiply_Leave,
        // "/"
        tail_divide_Leave,
        // number
        numberLeave,
        // cluster
        clusterLeave,
        // "||"
        tail_or_Or_Leave,
        // "&&"
        tail_and_And_Leave,
        // "!"
        tail_not_Leave,
        // "<>"
        tail_lessThan_GreaterThan_Leave,
        // "=="
        tail_equality_Equality_Leave,
        // ">"
        tail_greaterThan_Leave,
        // "<"
        tail_lessThan_Leave,
        // ">="
        tail_greaterThan_Equality_Leave,
        // "<="
        tail_lessThan_Equality_Leave,
        // #
        tail_startEndLeave
    }
}
