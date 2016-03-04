using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 动作包装类
    /// </summary>
    [Serializable]
    internal class ActionPackage
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string nodeName = "";

        /// <summary>
        /// 节点行号
        /// </summary>
        public int line = 0;

        /// <summary>
        /// 对齐偏移量
        /// </summary>
        public int indent = 0;

        /// <summary>
        /// 动作的类型
        /// </summary>
        public ActionPackageType nodeType = ActionPackageType.NOP;

        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, ArgumentPackage> argsDict = new Dictionary<string, ArgumentPackage>();

        /// <summary>
        /// 获取动作的名称
        /// </summary>
        public string GetActionName()
        {
            if (this.nodeType != ActionPackageType.NOP)
            {
                return ((ActionName)this.nodeType).ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取动作名和参数之间的分割
        /// </summary>
        public string GetFlag()
        {
            if (this.nodeType != ActionPackageType.NOP && this.nodeType != ActionPackageType.act_else
                && this.nodeType != ActionPackageType.act_endif && this.nodeType != ActionPackageType.act_endfor)
            {
                return "◆";
            }
            return ":";
        }

        /// <summary>
        /// 获取动作的参数描述
        /// </summary>
        public string GetParaDescription()
        {
            StringBuilder desSb = new StringBuilder();
            switch (this.nodeType)
            {
                case ActionPackageType.notation:
                case ActionPackageType.act_dialog:
                    desSb.Append(String.Format("{0} ", this.argsDict["context"].valueExp));
                    break;
                case ActionPackageType.act_a:
                    desSb.Append(String.Format("名字:{0} ", this.argsDict["name"].valueExp));
                    desSb.Append(String.Format("表情:{0} ", this.argsDict["face"].valueExp));
                    desSb.Append(String.Format("位置:{0} ", this.argsDict["loc"].valueExp));
                    desSb.Append(String.Format("语音:{0} ", this.argsDict["vid"].valueExp));
                    break;
                case ActionPackageType.act_draw:
                    desSb.Append(String.Format("\"{0}\" ", this.argsDict["dash"].valueExp));
                    break;
                case ActionPackageType.act_branch:
                    desSb.Append(String.Format("{0} ", this.argsDict["link"].valueExp));
                    break;
                case ActionPackageType.act_msglayer:
                    desSb.Append(String.Format("目标层：{0} ", this.argsDict["id"].valueExp));
                    break;
                case ActionPackageType.act_bgm:
                case ActionPackageType.act_bgs:
                case ActionPackageType.act_se:
                    desSb.Append(String.Format("{0} ", this.argsDict["filename"].valueExp));
                    desSb.Append(String.Format("音量：{0} ", this.argsDict["vol"].valueExp));
                    break;
                case ActionPackageType.act_label:
                    desSb.Append(String.Format("{0} ", this.argsDict["name"].valueExp));
                    break;
                case ActionPackageType.act_jump:
                    desSb.Append(String.Format("场景：{0} ", this.argsDict["filename"].valueExp));
                    if (this.argsDict["target"].valueExp != "")
                    {
                        desSb.Append(String.Format("标签：{0} ", this.argsDict["target"].valueExp));
                    }
                    if (this.argsDict["cond"].valueExp != "")
                    {
                        desSb.Append(String.Format("条件：{0} ", this.argsDict["cond"].valueExp));
                    }
                    break;
                case ActionPackageType.act_switch:
                    desSb.Append(String.Format("[{0}:{1}] 切换到 {2} ", this.argsDict["id"].valueExp,
                        Halation.project.SwitchDescriptorList[Convert.ToInt32(this.argsDict["id"].valueExp)], this.argsDict["dash"].valueExp));
                    break;
                case ActionPackageType.act_wait:
                    desSb.Append(String.Format("{0} ms ", this.argsDict["time"].valueExp));
                    break;

            }
            return desSb.ToString();
        }
    }

    /// <summary>
    /// 枚举：动作类型
    /// </summary>
    public enum ActionPackageType
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
        // 注释
        notation
    }

    /// <summary>
    /// 枚举：动作类型的显示名称
    /// </summary>
    public enum ActionName
    {
        // 无动作
        NOP,
        // 段落
        显示对话,
        // 段落结束符
        act_dialogTerminator,
        // 显示文本
        角色状态,
        // 显示背景
        显示背景,
        // 显示图片
        显示图片,
        // 移动图片
        移动图片,
        // 消去图片
        消去图片,
        // 显示立绘
        显示立绘,
        // 消去立绘
        消去立绘,
        // 播放声效
        播放声效,
        // 播放音乐
        播放音乐,
        // 播放bgs
        播放背景音效,
        // 停止音乐
        停止音乐,
        // 停止背景声效
        停止背景声效,
        // 播放语音
        播放语音,
        // 停止语音
        停止语音,
        // 返回标题
        返回标题,
        // 调用菜单
        act_menu,
        // 调用存档
        act_save,
        // 调用读档
        act_load,
        // 标签
        标签,
        // 标签跳转
        标签跳转,
        // 循环（头）
        循环,
        // 循环（尾）
        以上反复,
        // 条件（头）
        条件分支,
        // 条件（分支）
        除此以外的情况,
        // 条件（尾）
        分支结束,
        // 函数声明（头）
        act_function,
        // 函数声明（尾）
        act_endfunction,
        // 剧本跳转
        act_scene,
        // 开关操作
        开关操作,
        // 变量操作
        变量操作,
        // 退出循环
        中断循环,
        // 退出程序
        结束程序,
        // 中断事件处理
        退出当前场景,
        // 等待
        延时等待,
        // 选择支
        选择支,
        // 函数调用
        函数调用,
        // 回归点
        act_titlepoint,
        // 准备渐变
        act_freeze,
        // 执行渐变
        执行渐变,
        // 按钮
        放置按钮,
        // 对话样式
        act_style,
        // 切换文字层
        切换文字层,
        // 修改层属性
        修改层属性,
        // 等待用户操作
        等待用户操作,
        // 等待动画结束
        等待动画结束,
        // 描绘字符串
        描绘字符串,
        // 移除按钮
        移除按钮,
        // 注释
        注释
    }
}
