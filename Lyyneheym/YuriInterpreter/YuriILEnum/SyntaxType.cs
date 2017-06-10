namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：语法节点类型
    /// </summary>
    public enum SyntaxType
    {
        /// <summary>
        /// 段落
        /// </summary>
        synr_dialog,
        /// <summary>
        /// 段落结束符
        /// </summary>
        synr_dialogTerminator,
        /// <summary>
        /// 显示文本
        /// </summary>
        synr_a,
        /// <summary>
        /// 显示背景
        /// </summary>
        synr_bg,
        /// <summary>
        /// 显示图片
        /// </summary>
        synr_picture,
        /// <summary>
        /// 移动图片
        /// </summary>
        synr_move,
        /// <summary>
        /// 消去图片
        /// </summary>
        synr_deletepicture,
        /// <summary>
        /// 显示立绘
        /// </summary>
        synr_cstand,
        /// <summary>
        /// 消去立绘
        /// </summary>
        synr_deletecstand,
        /// <summary>
        /// 播放声效
        /// </summary>
        synr_se,
        /// <summary>
        /// 播放音乐
        /// </summary>
        synr_bgm,
        /// <summary>
        /// 播放bgs
        /// </summary>
        synr_bgs,
        /// <summary>
        /// 停止音乐
        /// </summary>
        synr_stopbgm,
        /// <summary>
        /// 停止BGS
        /// </summary>
        synr_stopbgs,
        /// <summary>
        /// 播放语音
        /// </summary>
        synr_vocal,
        /// <summary>
        /// 停止语音
        /// </summary>
        synr_stopvocal,
        /// <summary>
        /// 返回标题
        /// </summary>
        synr_title,
        /// <summary>
        /// 调用菜单
        /// </summary>
        synr_menu,
        /// <summary>
        /// 调用存档
        /// </summary>
        synr_save,
        /// <summary>
        /// 调用读档
        /// </summary>
        synr_load,
        /// <summary>
        /// 标签
        /// </summary>
        synr_label,
        /// <summary>
        /// 标签跳转
        /// </summary>
        synr_jump,
        /// <summary>
        /// 函数调用
        /// </summary>
        synr_call,
        /// <summary>
        /// 循环（头）
        /// </summary>
        synr_for,
        /// <summary>
        /// 循环（尾）
        /// </summary>
        synr_endfor,
        /// <summary>
        /// 条件（头）
        /// </summary>
        synr_if,
        /// <summary>
        /// 条件（分支）
        /// </summary>
        synr_else,
        /// <summary>
        /// 条件（尾）
        /// </summary>
        synr_endif,
        /// <summary>
        /// 函数声明
        /// </summary>
        synr_function,
        /// <summary>
        /// 函数结束
        /// </summary>
        synr_endfunction,
        /// <summary>
        /// 剧本跳转
        /// </summary>
        synr_scene,
        /// <summary>
        /// 开关操作
        /// </summary>
        synr_switch,
        /// <summary>
        /// 变量操作
        /// </summary>
        synr_var,
        /// <summary>
        /// 退出循环
        /// </summary>
        synr_break,
        /// <summary>
        /// 退出程序
        /// </summary>
        synr_shutdown,
        /// <summary>
        /// 等待
        /// </summary>
        synr_wait,
        /// <summary>
        /// 选择支
        /// </summary>
        synr_branch,
        /// <summary>
        /// 标志回归点
        /// </summary>
        synr_titlepoint,
        /// <summary>
        /// 准备渐变
        /// </summary>
        synr_freeze,
        /// <summary>
        /// 执行渐变
        /// </summary>
        synr_trans,
        /// <summary>
        /// 中断事件处理
        /// </summary>
        synr_return,
        /// <summary>
        /// 按钮
        /// </summary>
        synr_button,
        /// <summary>
        /// 对话样式
        /// </summary>
        synr_style,
        /// <summary>
        /// 切换文字层
        /// </summary>
        synr_msglayer,
        /// <summary>
        /// 修改层属性
        /// </summary>
        synr_msglayeropt,
        /// <summary>
        /// 描绘字符串
        /// </summary>
        synr_draw,
        /// <summary>
        /// 等待用户操作
        /// </summary>
        synr_waituser,
        /// <summary>
        /// 等待动画完成
        /// </summary>
        synr_waitani,
        /// <summary>
        /// 移除按钮
        /// </summary>
        synr_deletebutton,
        /// <summary>
        /// 场景镜头
        /// </summary>
        synr_scamera,
        /// <summary>
        /// 通知
        /// </summary>
        synr_notify,
        /// <summary>
        /// 发送系统消息
        /// </summary>
        synr_yurimsg,
        /// <summary>
        /// 信号系统
        /// </summary>
        synr_semaphore,
        /// <summary>
        /// 章节设置
        /// </summary>
        synr_chapter,
        /// <summary>
        /// 消息弹窗
        /// </summary>
        synr_alert,
        /// <summary>
        /// 截图
        /// </summary>
        synr_snapshot,
        /// <summary>
        /// 设置系统变量
        /// </summary>
        synr_sysset,
        /// <summary>
        /// BGM音量淡入淡出
        /// </summary>
        synr_bgmfade,
        /// <summary>
        /// 启用禁用功能
        /// </summary>
        synr_enabler,
        /// <summary>
        /// 显示UI页
        /// </summary>
        synr_uipage,
        /// <summary>
        /// 参数：类型
        /// </summary>
        para_type,
        /// <summary>
        /// 参数：函数签名
        /// </summary>
        para_sign,
        /// <summary>
        /// 参数：选择支链
        /// </summary>
        para_link,
        /// <summary>
        /// 参数：名称
        /// </summary>
        para_name,
        /// <summary>
        /// 参数：语音id
        /// </summary>
        para_vid,
        /// <summary>
        /// 参数：立绘表情
        /// </summary>
        para_face,
        /// <summary>
        /// 参数：序号
        /// </summary>
        para_id,
        /// <summary>
        /// 参数：x坐标
        /// </summary>
        para_x,
        /// <summary>
        /// 参数：y坐标
        /// </summary>
        para_y,
        /// <summary>
        /// 参数：z坐标
        /// </summary>
        para_z,
        /// <summary>
        /// 参数：加速度
        /// </summary>
        para_acc,
        /// <summary>
        /// 参数：x加速度
        /// </summary>
        para_xacc,
        /// <summary>
        /// 参数：y加速度
        /// </summary>
        para_yacc,
        /// <summary>
        /// 参数：不透明度
        /// </summary>
        para_opacity,
        /// <summary>
        /// 参数：x轴缩放比
        /// </summary>
        para_xscale,
        /// <summary>
        /// 参数：y轴缩放比
        /// </summary>
        para_yscale,
        /// <summary>
        /// 参数：时间
        /// </summary>
        para_time,
        /// <summary>
        /// 参数：文件名
        /// </summary>
        para_filename,
        /// <summary>
        /// 参数：音轨号
        /// </summary>
        para_track,
        /// <summary>
        /// 参数：条件子句
        /// </summary>
        para_cond,
        /// <summary>
        /// 参数：表达式
        /// </summary>
        para_dash,
        /// <summary>
        /// 参数：位置
        /// </summary>
        para_loc,
        /// <summary>
        /// 参数：角度
        /// </summary>
        para_ro,
        /// <summary>
        /// 参数：音量
        /// </summary>
        para_vol,
        /// <summary>
        /// 参数：开关状态
        /// </summary>
        para_state,
        /// <summary>
        /// 参数：作用目标
        /// </summary>
        para_target,
        /// <summary>
        /// 参数：按钮正常
        /// </summary>
        para_normal,
        /// <summary>
        /// 参数：按钮悬停
        /// </summary>
        para_over,
        /// <summary>
        /// 参数：按钮按下
        /// </summary>
        para_on,
        /// <summary>
        /// 参数：激活函数
        /// </summary>
        para_activator,
        /// <summary>
        /// 参数：反激活函数
        /// </summary>
        para_deactivator,
        /// <summary>
        /// 根节点
        /// </summary>
        case_kotori,
        /// <summary>
        /// (disjunct) ::= (conjunct) (disjunct_pi);
        /// </summary>
        case_disjunct,
        /// <summary>
        /// (disjunct_pi) ::= "||" (conjunct) (disjunct_pi) | null;
        /// </summary>
        case_disjunct_pi,
        /// <summary>
        /// (conjunct) ::= (bool) (conjunct_pi);
        /// </summary>
        case_conjunct,
        /// <summary>
        /// (conjunct_pi) ::= "&amp;&amp;" (bool) (conjunct_pi) | null;
        /// </summary>
        case_conjunct_pi,
        /// <summary>
        /// (bool) ::= "(" (disjunct) ")" | "!" (bool) | (comp);
        /// </summary>
        case_bool,
        /// <summary>
        /// (comp) ::= (wexpr) (rop) (wexpr);
        /// </summary>
        case_comp,
        /// <summary>
        /// (rop) ::= "&lt;&gt;" | "==" | ">" | "&lt;" | ">=" | "&lt;=" | null;
        /// </summary>
        case_rop,
        /// <summary>
        /// (wexpr) ::= (wmulti) (wexpr_pi);
        /// </summary>
        case_wexpr,
        /// <summary>
        /// (wexpr) ::= (wplus) (wexpr_pi) | null;
        /// </summary>
        case_wexpr_pi,
        /// <summary>
        /// (wplus) ::= "+" (wmulti) | "-" (wmulti);
        /// </summary>
        case_wplus,
        /// <summary>
        /// (wmulti) ::= (wunit) (wmultiOpt);
        /// </summary>
        case_wmulti,
        /// <summary>
        /// (wmultiOpt) ::= "*" (wunit) | "/" (wunit) | null;
        /// </summary>
        case_wmultiOpt,
        /// <summary>
        /// (wunit) ::= number | identifier | "-" (wunit) | "+" (wunit) | "(" (wexpr) ")";
        /// </summary>
        case_wunit,
        /// <summary>
        /// 未知的语法结点符号
        /// </summary>
        Unknown,
        /// <summary>
        /// identifier
        /// </summary>
        tail_idenLeave,
        /// <summary>
        /// "("
        /// </summary>
        tail_leftParentheses_Leave,
        /// <summary>
        /// ")"
        /// </summary>
        tail_rightParentheses_Leave,
        /// <summary>
        /// ";"
        /// </summary>
        tail_semicolon_Leave,
        /// <summary>
        /// ","
        /// </summary>
        tail_comma_Leave,
        /// <summary>
        /// null
        /// </summary>
        epsilonLeave,
        /// <summary>
        /// "="
        /// </summary>
        tail_equality_Leave,
        /// <summary>
        /// "+"
        /// </summary>
        tail_plus_Leave,
        /// <summary>
        /// "-"
        /// </summary>
        tail_minus_Leave,
        /// <summary>
        /// "*"
        /// </summary>
        tail_multiply_Leave,
        /// <summary>
        /// "/"
        /// </summary>
        tail_divide_Leave,
        /// <summary>
        /// number
        /// </summary>
        numberLeave,
        /// <summary>
        /// cluster
        /// </summary>
        clusterLeave,
        /// <summary>
        /// "||"
        /// </summary>
        tail_or_Or_Leave,
        /// <summary>
        /// "&amp;&amp;"
        /// </summary>
        tail_and_And_Leave,
        /// <summary>
        /// "!"
        /// </summary>
        tail_not_Leave,
        /// <summary>
        /// "&lt;&gt;"
        /// </summary>
        tail_lessThan_GreaterThan_Leave,
        /// <summary>
        /// "=="
        /// </summary>
        tail_equality_Equality_Leave,
        /// <summary>
        /// ">"
        /// </summary>
        tail_greaterThan_Leave,
        /// <summary>
        /// "&lt;"
        /// </summary>
        tail_lessThan_Leave,
        /// <summary>
        /// ">="
        /// </summary>
        tail_greaterThan_Equality_Leave,
        /// <summary>
        /// "&lt;="
        /// </summary>
        tail_lessThan_Equality_Leave,
        /// <summary>
        /// #
        /// </summary>
        tail_startEndLeave
    }
}