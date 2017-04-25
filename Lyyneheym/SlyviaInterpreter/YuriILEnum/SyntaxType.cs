namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：语法节点类型
    /// </summary>
    public enum SyntaxType
    {
        // 段落
        synr_dialog,
        // 段落结束符
        synr_dialogTerminator,
        // 显示文本
        synr_a,
        // 显示背景
        synr_bg,
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
        // 播放bgs
        synr_bgs,
        // 停止音乐
        synr_stopbgm,
        // 停止BGS
        synr_stopbgs,
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
        synr_label,
        // 标签跳转
        synr_jump,
        // 函数调用
        synr_call,
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
        // 函数声明
        synr_function,
        // 函数结束
        synr_endfunction,
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
        // 标志回归点
        synr_titlepoint,
        // 准备渐变
        synr_freeze,
        // 执行渐变
        synr_trans,
        // 中断事件处理
        synr_return,
        // 按钮
        synr_button,
        // 对话样式
        synr_style,
        // 切换文字层
        synr_msglayer,
        // 修改层属性
        synr_msglayeropt,
        // 描绘字符串
        synr_draw,
        // 等待用户操作
        synr_waituser,
        // 等待动画完成
        synr_waitani,
        // 移除按钮
        synr_deletebutton,
        // 场景镜头
        synr_scamera,
        // 通知
        synr_notify,
        // 发送系统消息
        synr_yurimsg,
        // 信号系统
        synr_semaphore,
        // 参数：类型
        para_type,
        // 参数：函数签名
        para_sign,
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
        // 参数：加速度
        para_acc,
        // 参数：x加速度
        para_xacc,
        // 参数：y加速度
        para_yacc,
        // 参数：透明度
        para_opacity,
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
        // 参数：开关状态
        para_state,
        // 参数：作用目标
        para_target,
        // 参数：按钮正常
        para_normal,
        // 参数：按钮悬停
        para_over,
        // 参数：按钮按下
        para_on,
        // 参数：激活函数
        para_activator,
        // 参数：反激活函数
        para_deactivator,
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