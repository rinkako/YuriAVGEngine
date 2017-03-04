using System;
using System.Collections.Generic;
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
        /// 深拷贝一份自身并返回
        /// </summary>
        /// <returns>一份深拷贝</returns>
        public ActionPackage Clone()
        {
            ActionPackage ap = new ActionPackage();
            ap.nodeName = this.nodeName;
            ap.indent = this.indent;
            ap.nodeType = this.nodeType;
            ap.argsDict = new Dictionary<string, ArgumentPackage>();
            foreach (var kvp in this.argsDict)
            {
                ArgumentPackage argp = new ArgumentPackage()
                {
                    aType = kvp.Value.aType,
                    valueExp = kvp.Value.valueExp
                };
                ap.argsDict.Add(kvp.Key, argp);
            }
            return ap;
        }

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
        /// 获取参数对齐空格
        /// </summary>
        public string GetSpace()
        {
            StringBuilder sb = new StringBuilder();
            if (this.nodeType != ActionPackageType.NOP)
            {
                var chName = ((ActionName)this.nodeType);
                var chChar = chName.ToString().Length * 2;
                var delta = 12 - chChar;
                for (int i = 0; i < delta; i++)
                {
                    sb.Append(' ');
                }
                return sb.ToString();
            }
            else
            {
                return "    ";
            }
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
                case ActionPackageType.script:
                case ActionPackageType.act_dialog:
                    string displayStr = this.argsDict["context"].valueExp.Replace(Environment.NewLine, " ");
                    desSb.Append(String.Format("{0} ", displayStr.Length > 40 ? displayStr.Substring(0, 40) + "..." : displayStr));
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
                    desSb.Append(String.Format("目标层:{0} ", this.argsDict["id"].valueExp));
                    break;
                case ActionPackageType.act_bgm:
                case ActionPackageType.act_bgs:
                case ActionPackageType.act_se:
                    desSb.Append(String.Format("{0} ", this.argsDict["filename"].valueExp));
                    desSb.Append(String.Format("音量:{0} ", this.argsDict["vol"].valueExp));
                    break;
                case ActionPackageType.act_label:
                    desSb.Append(String.Format("{0} ", this.argsDict["name"].valueExp));
                    break;
                case ActionPackageType.act_jump:
                    desSb.Append(String.Format("场景:{0} ", this.argsDict["filename"].valueExp));
                    if (this.argsDict["target"].valueExp != "")
                    {
                        desSb.Append(String.Format("标签:{0} ", this.argsDict["target"].valueExp));
                    }
                    if (this.argsDict["cond"].valueExp != "")
                    {
                        desSb.Append(String.Format("条件:{0} ", this.argsDict["cond"].valueExp));
                    }
                    break;
                case ActionPackageType.act_switch:
                    desSb.Append(String.Format("[{0}:{1}] 切换到 {2} ", this.argsDict["id"].valueExp,
                        Halation.project.SwitchDescriptorList[Convert.ToInt32(this.argsDict["id"].valueExp)], this.argsDict["dash"].valueExp));
                    break;
                case ActionPackageType.act_wait:
                    desSb.Append(String.Format("{0} ms ", this.argsDict["time"].valueExp));
                    break;
                case ActionPackageType.act_deletebutton:
                case ActionPackageType.act_deletecstand:
                case ActionPackageType.act_deletepicture:
                    desSb.Append(String.Format("{0} ", this.argsDict["id"].valueExp));
                    break;
                case ActionPackageType.act_trans:
                    desSb.Append(String.Format("样式:{0} ", this.argsDict["name"].valueExp));
                    break;
                case ActionPackageType.act_msglayeropt:
                    desSb.Append(String.Format("Target:{0} ", this.argsDict["target"].valueExp));
                    if (this.argsDict["dash"].valueExp != "")
                    {
                        desSb.Append(String.Format("目标值:{0} ", this.argsDict["dash"].valueExp));
                    }
                    break;
                case ActionPackageType.act_move:
                    desSb.Append(String.Format("对象:[{0}:{1}] ", this.argsDict["name"].valueExp, this.argsDict["id"].valueExp));
                    desSb.Append(String.Format("时间:{0}ms ", this.argsDict["time"].valueExp));
                    desSb.Append(String.Format("属性:{0} ", this.argsDict["target"].valueExp));
                    desSb.Append(String.Format("目标值:{0} ", this.argsDict["dash"].valueExp));
                    desSb.Append(String.Format("加速度:{0} ", this.argsDict["acc"].valueExp));
                    break;
                case ActionPackageType.act_bg:
                    desSb.Append(String.Format("图层:{0} ", this.argsDict["id"].valueExp == "0" ? "背景" : "前景"));
                    desSb.Append(String.Format("文件:{0} ", this.argsDict["filename"].valueExp));
                    break;
                case ActionPackageType.act_cstand:
                    desSb.Append(String.Format("图层:{0} ", this.argsDict["id"].valueExp));
                    desSb.Append(String.Format("角色:{0} ", this.argsDict["name"].valueExp));
                    desSb.Append(String.Format("表情:{0} ", this.argsDict["face"].valueExp));
                    if (this.argsDict["loc"].valueExp == "")
                    {
                        desSb.Append(String.Format("X:{0},Y:{1} ", this.argsDict["x"].valueExp, this.argsDict["y"].valueExp));
                    }
                    else
                    {
                        desSb.Append(String.Format("相对位置:{0} ", this.argsDict["loc"].valueExp));
                    }
                    break;
                case ActionPackageType.act_button:
                    desSb.Append(String.Format("ID:{0} ", this.argsDict["id"].valueExp));
                    desSb.Append(String.Format("位置:{0},{1} ", this.argsDict["x"].valueExp, this.argsDict["y"].valueExp));
                    desSb.Append(String.Format("标签:{0} ", this.argsDict["target"].valueExp));
                    desSb.Append(String.Format("类型:{0} ", this.argsDict["type"].valueExp));
                    desSb.Append(String.Format("图形:{0}->{1}->{2} ", this.argsDict["normal"].valueExp, this.argsDict["over"].valueExp, this.argsDict["on"].valueExp));
                    break;
                case ActionPackageType.act_picture:
                    desSb.Append(String.Format("[{0}:{1}] ", this.argsDict["id"].valueExp, this.argsDict["filename"].valueExp));
                    desSb.Append(String.Format("位置:{0},{1} ", this.argsDict["x"].valueExp, this.argsDict["y"].valueExp));
                    desSb.Append(String.Format("比例:{0}%,{1}% ", this.argsDict["xscale"].valueExp, this.argsDict["yscale"].valueExp));
                    desSb.Append(String.Format("角度:{0} ", this.argsDict["ro"].valueExp));
                    desSb.Append(String.Format("不透明度:{0}% ", this.argsDict["opacity"].valueExp));
                    break;
                case ActionPackageType.act_call:
                    desSb.Append(String.Format("{0} ", this.argsDict["name"].valueExp));
                    if (this.argsDict["sign"].valueExp.Length > 0)
                    {
                        desSb.Append(String.Format("({0})", this.argsDict["sign"].valueExp));
                    }
                    break;
                case ActionPackageType.act_var:
                    desSb.Append(String.Format("{0} ", this.argsDict["opLeft"].valueExp));
                    desSb.Append(String.Format("{0} ", this.argsDict["op"].valueExp));
                    string[] varRightItems = this.argsDict["opRight"].valueExp.Split('#');
                    switch (varRightItems[0])
                    {
                        case "1":
                            desSb.Append(String.Format("常数[{0}] ", varRightItems[1]));
                            break;
                        case "2":
                            desSb.Append(String.Format("字符串[{0}]  ", varRightItems[1]));
                            break;
                        case "3":
                            desSb.Append(String.Format("&{0} ", varRightItems[1]));
                            break;
                        case "4":
                            desSb.Append(String.Format("${0} ", varRightItems[1]));
                            break;
                        case "5":
                            string[] raItems = varRightItems[1].Split(':');
                            desSb.Append(String.Format("随机区间:[{0},{1}] ", raItems[0], raItems[1]));
                            break;
                        default:
                            desSb.Append(String.Format("表达式:[{0}] ", varRightItems[1]));
                            break;
                    }
                    break;
                case ActionPackageType.act_if:
                    if (this.argsDict["expr"].valueExp == "")
                    {
                        string[] ifLeftItems = this.argsDict["op1"].valueExp.ToString().Split('#');
                        switch (ifLeftItems[0])
                        {
                            case "1":
                                desSb.Append(String.Format("&{0} ", ifLeftItems[1]));
                                break;
                            case "2":
                                desSb.Append(String.Format("${0} ", ifLeftItems[1]));
                                break;
                            case "3":
                                desSb.Append(String.Format("开关[{0}:{1}] ", ifLeftItems[1], Halation.project.SwitchDescriptorList[Convert.ToInt32(ifLeftItems[1])]));
                                break;
                        }
                        desSb.Append(String.Format("{0} ", this.argsDict["opr"].valueExp));
                        string[] ifRightItems = this.argsDict["op2"].valueExp.Split('#');
                        switch (ifRightItems[0])
                        {
                            case "1":
                                desSb.Append(String.Format("常数[{0}] ", ifRightItems[1]));
                                break;
                            case "2":
                                desSb.Append(String.Format("字符串[{0}]  ", ifRightItems[1]));
                                break;
                            case "3":
                                desSb.Append(String.Format("&{0} ", ifRightItems[1]));
                                break;
                            case "4":
                                desSb.Append(String.Format("${0} ", ifRightItems[1]));
                                break;
                            case "5":
                                desSb.Append(String.Format("开关状态:[{0}] ", ifRightItems[1]));
                                break;
                        }
                    }
                    else
                    {
                        desSb.Append(String.Format("表达式:[{0}]为真时 ", this.argsDict["expr"].valueExp));
                    }
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
        notation,
        // 代码片段
        script
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
        执行过渡,
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
        注释,
        // 代码片段
        代码片段
    }
}
