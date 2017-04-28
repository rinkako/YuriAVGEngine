namespace Yuri.Yuriri
{
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
        act_deletebutton,
        // 场景镜头
        act_scamera,
        // 通知
        act_notify,
        // 发送系统消息
        act_yurimsg,
        // 信号系统
        act_semaphore,
        // 章节设置
        act_chapter,
    }
}