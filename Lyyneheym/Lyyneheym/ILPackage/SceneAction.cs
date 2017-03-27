using System;
using System.CodeDom;
using System.Collections.Generic;
using Yuri.Utils;

namespace Yuri.ILPackage
{
    /// <summary>
    /// 场景动作类：语义分析器输出的中间代码类
    /// </summary>
    [Serializable]
    internal class SceneAction
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; } = null;
        
        /// <summary>
        /// 节点动作
        /// </summary>
        public SActionType Type { get; set; } = SActionType.NOP;
        
        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> ArgsDict { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// 条件从句逆波兰表达
        /// </summary>
        public string CondPolish { get; set; } = null;
        
        /// <summary>
        /// 下一节点
        /// </summary>
        public SceneAction Next { get; set; } = null;

        /// <summary>
        /// 真节点向量
        /// </summary>
        public List<SceneAction> TrueRouting { get; set; } = null;

        /// <summary>
        /// 假节点向量
        /// </summary>
        public List<SceneAction> FalseRouting { get; set; } = null;

        /// <summary>
        /// 是否依存函数
        /// </summary>
        public bool IsBelongFunc { get; set; } = false;

        /// <summary>
        /// 依存函数名
        /// </summary>
        public string ReliedFuncName { get; set; } = null;

        /// <summary>
        /// 附加值
        /// </summary>
        public string Tag { get; set; } = null;

        /// <summary>
        /// 带SAP项的构造函数
        /// </summary>
        /// <param name="sap">SceneActionPackage项目</param>
        public SceneAction(SceneActionPackage sap)
        {
            this.NodeName = sap.saNodeName;
            this.Type = (SActionType)Enum.Parse(typeof(SActionType), sap.saNodeName.Split('@')[1]);
            this.ArgsDict = new Dictionary<string, string>(sap.argsDict);
            this.CondPolish = sap.condPolish;
            this.IsBelongFunc = sap.isBelongFunc;
            this.ReliedFuncName = sap.funcName;
            this.Tag = sap.aTag;
        }

        /// <summary>
        /// 取下一动作
        /// </summary>
        /// <param name="rhs">自增符号的作用对象</param>
        /// <returns>该作用对象动作的下一动作</returns>
        public static SceneAction operator++(SceneAction rhs)
        {
            if (rhs != null)
            {
                return rhs.Next;
            }
            CommonUtils.ConsoleLine("++ applied on an action but it is null", "SceneAction", OutputStyle.Error);
            return null;
        } 

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SceneAction()
        {

        }

        /// <summary>
        /// 为当前动作创建一个副本
        /// </summary>
        /// <param name="pureClone">是否保留关系</param>
        /// <returns>原动作的深拷贝副本</returns>
        public SceneAction Clone(bool pureClone)
        {
            SceneAction resSa = new SceneAction { ArgsDict = new Dictionary<string, string>() };
            foreach (var kv in this.ArgsDict)
            {
                resSa.ArgsDict.Add(kv.Key, kv.Value);
            }
            resSa.Tag = this.Tag;
            resSa.Type = this.Type;
            resSa.ReliedFuncName = this.ReliedFuncName;
            resSa.IsBelongFunc = this.IsBelongFunc;
            resSa.NodeName = this.NodeName;
            if (pureClone != false)
            {
                return resSa;
            }
            resSa.CondPolish = this.CondPolish;
            resSa.Next = this.Next;
            resSa.NodeName = this.NodeName;
            resSa.TrueRouting = new List<SceneAction>();
            foreach (var tr in this.TrueRouting)
            {
                resSa.TrueRouting.Add(tr);
            }
            resSa.FalseRouting = new List<SceneAction>();
            foreach (var fr in this.FalseRouting)
            {
                resSa.FalseRouting.Add(fr);
            }
            return resSa;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString() => this.NodeName;
    }

    [Serializable]
    internal class SceneActionPackage
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string saNodeName = null;
        
        /// <summary>
        /// 节点动作
        /// </summary>
        public SActionType aType = SActionType.NOP;
        
        /// <summary>
        /// 参数字典
        /// </summary>
        public Dictionary<string, string> argsDict = new Dictionary<string, string>();
        
        /// <summary>
        /// 条件从句逆波兰表达
        /// </summary>
        public string condPolish = null;
        
        /// <summary>
        /// 下一节点
        /// </summary>
        public string next = null;
        
        /// <summary>
        /// 下一真节点向量
        /// </summary>
        public List<string> trueRouting = null;
        
        /// <summary>
        /// 下一假节点向量
        /// </summary>
        public List<string> falseRouting = null;

        /// <summary>
        /// 是否依存函数
        /// </summary>
        public bool isBelongFunc = false;
        
        /// <summary>
        /// 依存函数名
        /// </summary>
        public string funcName = null;

        /// <summary>
        /// 附加值
        /// </summary>
        public string aTag = null;

        /// <summary>
        /// 脏位
        /// </summary>
        public bool dirtyBit = false;

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
        act_deletebutton
    }
}
