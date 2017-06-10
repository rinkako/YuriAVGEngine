namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：单词类型
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// 未知
        /// </summary>
        unknown,
        /// <summary>
        /// 标识符
        /// </summary>
        identifier,
        /// <summary>
        /// 字符串
        /// </summary>
        cluster,
        /// <summary>
        /// 剧本字符串
        /// </summary>
        scenecluster,
        /// <summary>
        /// 剧本段落终结符
        /// </summary>
        sceneterminator,
        /// <summary>
        /// 整数数字
        /// </summary>
        number,
        /// <summary>
        /// 起始终止标记
        /// </summary>
        startend,
        /// <summary>
        /// 符号：#
        /// </summary>
        Token_Sharp,
        /// <summary>
        /// 符号：左方括号[
        /// </summary>
        Token_LeftBracket,
        /// <summary>
        /// 符号：右方括号]
        /// </summary>
        Token_RightBracket,
        // 符号：左花括号{
        //Token_LeftBrace,
        /// <summary>
        /// 符号：右花括号}
        /// </summary>
        Token_RightBrace,
        /// <summary>
        /// 符号：单引号'
        /// </summary>
        Token_Quotation,
        /// <summary>
        /// 符号：双引号"
        /// </summary>
        Token_DoubleQuotation,
        /// <summary>
        /// 符号：等号=
        /// </summary>
        Token_Equality,
        /// <summary>
        /// 符号：艾特符@
        /// </summary>
        Token_At,
        /// <summary>
        /// 符号：朵拉符$
        /// </summary>
        Token_Dollar,
        /// <summary>
        /// 符号：地址符&amp;
        /// </summary>
        Token_Address,
        /// <summary>
        /// 符号：左括弧(
        /// </summary>
        Token_LeftParentheses,
        /// <summary>
        /// 符号：右括弧)
        /// </summary>
        Token_RightParentheses,
        /// <summary>
        /// 符号：加+
        /// </summary>
        Token_Plus,
        /// <summary>
        /// 符号：减-
        /// </summary>
        Token_Minus,
        /// <summary>
        /// 符号：乘*
        /// </summary>
        Token_Multiply,
        /// <summary>
        /// 符号：除/
        /// </summary>
        Token_Divide,
        /// <summary>
        /// 符号：不等号&lt;&gt;
        /// </summary>
        Token_LessThan_GreaterThan,
        /// <summary>
        /// 符号：等于号==
        /// </summary>
        Token_Equality_Equality,
        /// <summary>
        /// 符号：大于号&gt;
        /// </summary>
        Token_GreaterThan,
        /// <summary>
        /// 符号：小于号&lt;
        /// </summary>
        Token_LessThan,
        /// <summary>
        /// 符号：大于等于号&gt;=
        /// </summary>
        Token_GreaterThan_Equality,
        /// <summary>
        /// 符号：小于等于号&lt;=
        /// </summary>
        Token_LessThan_Equality,
        /// <summary>
        /// 符号：逻辑或||
        /// </summary>
        Token_Or_Or,
        /// <summary>
        /// 符号：逻辑与&amp;&amp;
        /// </summary>
        Token_And_And,
        /// <summary>
        /// 符号：逻辑否!
        /// </summary>
        Token_Not,
        /// <summary>
        /// 空操作
        /// </summary>
        Token_NOP,
        /// <summary>
        /// 显示文本
        /// </summary>
        Token_o_a,
        /// <summary>
        /// 显示背景
        /// </summary>
        Token_o_bg,
        /// <summary>
        /// 显示图片
        /// </summary>
        Token_o_picture,
        /// <summary>
        /// 移动图片
        /// </summary>
        Token_o_move,
        /// <summary>
        /// 消去图片
        /// </summary>
        Token_o_deletepicture,
        /// <summary>
        /// 显示立绘
        /// </summary>
        Token_o_cstand,
        /// <summary>
        /// 消去立绘
        /// </summary>
        Token_o_deletecstand,
        /// <summary>
        /// 播放声效
        /// </summary>
        Token_o_se,
        /// <summary>
        /// 播放音乐
        /// </summary>
        Token_o_bgm,
        /// <summary>
        /// 播放bgs
        /// </summary>
        Token_o_bgs,
        /// <summary>
        /// 停止音乐
        /// </summary>
        Token_o_stopbgm,
        /// <summary>
        /// 停止Bgs
        /// </summary>
        Token_o_stopbgs,
        /// <summary>
        /// 播放语音
        /// </summary>
        Token_o_vocal,
        /// <summary>
        /// 停止语音
        /// </summary>
        Token_o_stopvocal,
        /// <summary>
        /// 返回标题
        /// </summary>
        Token_o_title,
        /// <summary>
        /// 调用菜单
        /// </summary>
        Token_o_menu,
        /// <summary>
        /// 调用存档
        /// </summary>
        Token_o_save,
        /// <summary>
        /// 调用读档
        /// </summary>
        Token_o_load,
        /// <summary>
        /// 标签
        /// </summary>
        Token_o_label,
        /// <summary>
        /// 标签跳转
        /// </summary>
        Token_o_jump,
        /// <summary>
        /// 循环（头）
        /// </summary>
        Token_o_for,
        /// <summary>
        /// 循环（尾）
        /// </summary>
        Token_o_endfor,
        /// <summary>
        /// 条件（头）
        /// </summary>
        Token_o_if,
        /// <summary>
        /// 条件（分支）
        /// </summary>
        Token_o_else,
        /// <summary>
        /// 条件（尾）
        /// </summary>
        Token_o_endif,
        /// <summary>
        /// 剧本跳转
        /// </summary>
        Token_o_scene,
        /// <summary>
        /// 开关操作
        /// </summary>
        Token_o_switch,
        /// <summary>
        /// 变量操作
        /// </summary>
        Token_o_var,
        /// <summary>
        /// 退出循环
        /// </summary>
        Token_o_break,
        /// <summary>
        /// 退出程序
        /// </summary>
        Token_o_shutdown,
        /// <summary>
        /// 中断事件处理
        /// </summary>
        Token_o_return,
        /// <summary>
        /// 等待
        /// </summary>
        Token_o_wait,
        /// <summary>
        /// 选择支
        /// </summary>
        Token_o_branch,
        /// <summary>
        /// 函数定义头
        /// </summary>
        Token_o_function,
        /// <summary>
        /// 函数定义尾
        /// </summary>
        Token_o_endfunction,
        /// <summary>
        /// 函数调用
        /// </summary>
        Token_o_call,
        /// <summary>
        /// 标志回归点
        /// </summary>
        Token_o_titlepoint,
        /// <summary>
        /// 准备渐变
        /// </summary>
        Token_o_freeze,
        /// <summary>
        /// 执行渐变
        /// </summary>
        Token_o_trans,
        /// <summary>
        /// 按钮
        /// </summary>
        Token_o_button,
        /// <summary>
        /// 对话样式
        /// </summary>
        Token_o_style,
        /// <summary>
        /// 切换文字层
        /// </summary>
        Token_o_msglayer,
        /// <summary>
        /// 修改层属性
        /// </summary>
        Token_o_msglayeropt,
        /// <summary>
        /// 等待用户操作
        /// </summary>
        Token_o_waituser,
        /// <summary>
        /// 等待动画完成
        /// </summary>
        Token_o_waitani,
        /// <summary>
        /// 描绘字符串
        /// </summary>
        Token_o_draw,
        /// <summary>
        /// 移除按钮
        /// </summary>
        Token_o_deletebutton,
        /// <summary>
        /// 场景镜头
        /// </summary>
        Token_o_scamera,
        /// <summary>
        /// 通知
        /// </summary>
        Token_o_notify,
        /// <summary>
        /// 发送系统消息
        /// </summary>
        Token_o_yurimsg,
        /// <summary>
        /// 信号操作
        /// </summary>
        Token_o_semaphore,
        /// <summary>
        /// 章节设置
        /// </summary>
        Token_o_chapter,
        /// <summary>
        /// 弹窗
        /// </summary>
        Token_o_alert,
        /// <summary>
        /// 截图
        /// </summary>
        Token_o_snapshot,
        /// <summary>
        /// BGM音量淡入淡出
        /// </summary>
        Token_o_bgmfade,
        /// <summary>
        /// 启用禁用功能
        /// </summary>
        Token_o_enabler,
        /// <summary>
        /// 设置系统变量
        /// </summary>
        Token_o_sysset,
        /// <summary>
        /// 显示UI页
        /// </summary>
        Token_o_uipage,
        /// <summary>
        /// 参数：类型
        /// </summary>
        Token_p_type,
        /// <summary>
        /// 参数：函数签名
        /// </summary>
        Token_p_sign,
        /// <summary>
        /// 参数：名称
        /// </summary>
        Token_p_name,
        /// <summary>
        /// 参数：语音id
        /// </summary>
        Token_p_vid,
        /// <summary>
        /// 参数：立绘表情
        /// </summary>
        Token_p_face,
        /// <summary>
        /// 参数：序号
        /// </summary>
        Token_p_id,
        /// <summary>
        /// 参数：x坐标
        /// </summary>
        Token_p_x,
        /// <summary>
        /// 参数：y坐标
        /// </summary>
        Token_p_y,
        /// <summary>
        /// 参数：z坐标
        /// </summary>
        Token_p_z,
        /// <summary>
        /// 参数：加速度
        /// </summary>
        Token_p_acc,
        /// <summary>
        /// 参数：x加速度
        /// </summary>
        Token_p_xacc,
        /// <summary>
        /// 参数：y加速度
        /// </summary>
        Token_p_yacc,
        /// <summary>
        /// 参数：不透明度
        /// </summary>
        Token_p_opacity,
        /// <summary>
        /// 参数：x轴缩放比
        /// </summary>
        Token_p_xscale,
        /// <summary>
        /// 参数：y轴缩放比
        /// </summary>
        Token_p_yscale,
        /// <summary>
        /// 参数：时间
        /// </summary>
        Token_p_time,
        /// <summary>
        /// 参数：文件名
        /// </summary>
        Token_p_filename,
        /// <summary>
        /// 参数：音轨号
        /// </summary>
        Token_p_track,
        /// <summary>
        /// 参数：条件子句
        /// </summary>
        Token_p_cond,
        /// <summary>
        /// 参数：表达式
        /// </summary>
        Token_p_dash,
        /// <summary>
        /// 参数：开关状态
        /// </summary>
        Token_p_state,
        /// <summary>
        /// 参数：音量
        /// </summary>
        Token_p_vol,
        /// <summary>
        /// 参数：位置
        /// </summary>
        Token_p_loc,
        /// <summary>
        /// 参数：角度
        /// </summary>
        Token_p_ro,
        /// <summary>
        /// 参数：选择支链
        /// </summary>
        Token_p_link,
        /// <summary>
        /// 参数：宽度
        /// </summary>
        Token_p_width,
        /// <summary>
        /// 参数：高度
        /// </summary>
        Token_p_height,
        /// <summary>
        /// 参数：字体
        /// </summary>
        Token_p_font,
        /// <summary>
        /// 参数：尺寸
        /// </summary>
        Token_p_size,
        /// <summary>
        /// 参数：颜色
        /// </summary>
        Token_p_color,
        /// <summary>
        /// 参数：作用目标
        /// </summary>
        Token_p_target,
        /// <summary>
        /// 参数：正常按钮
        /// </summary>
        Token_p_normal,
        /// <summary>
        /// 参数：鼠标悬停按钮
        /// </summary>
        Token_p_over,
        /// <summary>
        /// 参数：鼠标按下按钮
        /// </summary>
        Token_p_on,
        /// <summary>
        /// 参数：激活函数
        /// </summary>
        Token_p_activator,
        /// <summary>
        /// 参数：反激活函数
        /// </summary>
        Token_p_deactivator
    }
}