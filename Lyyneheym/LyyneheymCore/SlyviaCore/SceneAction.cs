using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyyneheymCore.SlyviaPile;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 场景动作类：语义分析器输出的中间代码类
    /// </summary>
    public class SceneAction
    {
        // 节点动作
        public SActionType aType = SActionType.NOP;
        // 参数字典
        public Dictionary<string, SyntaxTreeNode> argsDict = new Dictionary<string, SyntaxTreeNode>();
        // 条件从句
        public SyntaxTreeNode condPointer = null;
        // 下一节点
        public SceneAction next = null;
        // 下一真节点向量
        public List<SceneAction> trueRouting = null;
        // 下一假节点向量
        public List<SceneAction> falseRouting = null;
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
        act_lable,
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
        act_branch
    }
}
