namespace Yuri.Yuriri
{
    /// <summary>
    /// 枚举：动作节点类型
    /// </summary>
    public enum SActionType
    {
        /// <summary>
        /// 无动作
        /// </summary>
        NOP,
        /// <summary>
        /// 段落
        /// </summary>
        act_dialog,
        /// <summary>
        /// 段落结束符
        /// </summary>
        act_dialogTerminator,
        /// <summary>
        /// 显示文本
        /// </summary>
        act_a,
        /// <summary>
        /// 显示背景
        /// </summary>
        act_bg,
        /// <summary>
        /// 显示图片
        /// </summary>
        act_picture,
        /// <summary>
        /// 移动图片
        /// </summary>
        act_move,
        /// <summary>
        /// 消去图片
        /// </summary>
        act_deletepicture,
        /// <summary>
        /// 显示立绘
        /// </summary>
        act_cstand,
        /// <summary>
        /// 消去立绘
        /// </summary>
        act_deletecstand,
        /// <summary>
        /// 播放声效
        /// </summary>
        act_se,
        /// <summary>
        /// 播放音乐
        /// </summary>
        act_bgm,
        /// <summary>
        /// 播放bgs
        /// </summary>
        act_bgs,
        /// <summary>
        /// 停止音乐
        /// </summary>
        act_stopbgm,
        /// <summary>
        /// 停止bgs
        /// </summary>
        act_stopbgs,
        /// <summary>
        /// 播放语音
        /// </summary>
        act_vocal,
        /// <summary>
        /// 停止语音
        /// </summary>
        act_stopvocal,
        /// <summary>
        /// 返回标题
        /// </summary>
        act_title,
        /// <summary>
        /// 调用菜单
        /// </summary>
        act_menu,
        /// <summary>
        /// 调用存档
        /// </summary>
        act_save,
        /// <summary>
        /// 调用读档
        /// </summary>
        act_load,
        /// <summary>
        /// 标签
        /// </summary>
        act_label,
        /// <summary>
        /// 标签跳转
        /// </summary>
        act_jump,
        /// <summary>
        /// 循环（头）
        /// </summary>
        act_for,
        /// <summary>
        /// 循环（尾）
        /// </summary>
        act_endfor,
        /// <summary>
        /// 条件（头）
        /// </summary>
        act_if,
        /// <summary>
        /// 条件（分支）
        /// </summary>
        act_else,
        /// <summary>
        /// 条件（尾）
        /// </summary>
        act_endif,
        /// <summary>
        /// 函数声明（头）
        /// </summary>
        act_function,
        /// <summary>
        /// 函数声明（尾）
        /// </summary>
        act_endfunction,
        /// <summary>
        /// 剧本跳转
        /// </summary>
        act_scene,
        /// <summary>
        /// 开关操作
        /// </summary>
        act_switch,
        /// <summary>
        /// 变量操作
        /// </summary>
        act_var,
        /// <summary>
        /// 退出循环
        /// </summary>
        act_break,
        /// <summary>
        /// 退出程序
        /// </summary>
        act_shutdown,
        /// <summary>
        /// 中断事件处理
        /// </summary>
        act_return,
        /// <summary>
        /// 等待
        /// </summary>
        act_wait,
        /// <summary>
        /// 选择支
        /// </summary>
        act_branch,
        /// <summary>
        /// 函数调用
        /// </summary>
        act_call,
        /// <summary>
        /// 回归点
        /// </summary>
        act_titlepoint,
        /// <summary>
        /// 准备渐变
        /// </summary>
        act_freeze,
        /// <summary>
        /// 执行渐变
        /// </summary>
        act_trans,
        /// <summary>
        /// 按钮
        /// </summary>
        act_button,
        /// <summary>
        /// 对话样式
        /// </summary>
        act_style,
        /// <summary>
        /// 切换文字层
        /// </summary>
        act_msglayer,
        /// <summary>
        /// 修改层属性
        /// </summary>
        act_msglayeropt,
        /// <summary>
        /// 等待用户操作
        /// </summary>
        act_waituser,
        /// <summary>
        /// 等待动画结束
        /// </summary>
        act_waitani,
        /// <summary>
        /// 描绘字符串
        /// </summary>
        act_draw,
        /// <summary>
        /// 移除按钮
        /// </summary>
        act_deletebutton,
        /// <summary>
        /// 场景镜头
        /// </summary>
        act_scamera,
        /// <summary>
        /// 通知
        /// </summary>
        act_notify,
        /// <summary>
        /// 发送系统消息
        /// </summary>
        act_yurimsg,
        /// <summary>
        /// 信号系统
        /// </summary>
        act_semaphore,
        /// <summary>
        /// 章节设置
        /// </summary>
        act_chapter,
        /// <summary>
        /// 消息弹窗
        /// </summary>
        act_alert,
        /// <summary>
        /// 截图
        /// </summary>
        act_snapshot,
        /// <summary>
        /// 调节BGM音量
        /// </summary>
        act_bgmfade,
        /// <summary>
        /// 启用禁用功能
        /// </summary>
        act_enabler,
        /// <summary>
        /// 设置系统变量
        /// </summary>
        act_sysset,
        /// <summary>
        /// 显示UI页
        /// </summary>
        act_uipage
    }
}