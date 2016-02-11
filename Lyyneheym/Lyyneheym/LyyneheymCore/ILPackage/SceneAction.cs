using System;
using System.Text;
using System.Collections.Generic;

namespace Lyyneheym.LyyneheymCore.ILPackage
{
    /// <summary>
    /// 场景动作类：语义分析器输出的中间代码类
    /// </summary>
    [Serializable]
    public class SceneAction
    {
        // 节点名称
        public string saNodeName = null;
        // 节点动作
        public SActionType aType = SActionType.NOP;
        // 参数字典
        public Dictionary<string, string> argsDict = new Dictionary<string, string>();
        // 条件从句逆波兰表达
        public string condPolish = null;
        // 下一节点
        public SceneAction next = null;
        // 下一真节点向量
        public List<SceneAction> trueRouting = null;
        // 下一假节点向量
        public List<SceneAction> falseRouting = null;
        // 是否依存函数
        public bool isBelongFunc = false;
        // 依存函数名
        public string funcName = null;
        // 附加值
        public string aTag = null;

        /// <summary>
        /// 带SAP项的构造函数
        /// </summary>
        /// <param name="sap">SceneActionPackage项目</param>
        public SceneAction(SceneActionPackage sap)
        {
            this.saNodeName = sap.saNodeName;
            this.aType = (SActionType)Enum.Parse(typeof(SActionType), sap.saNodeName.Split('@')[1]);
            this.argsDict = new Dictionary<string, string>(sap.argsDict);
            this.condPolish = sap.condPolish;
            this.isBelongFunc = sap.isBelongFunc;
            this.funcName = sap.funcName;
            this.aTag = sap.aTag;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>该动作的名字</returns>
        public override string ToString()
        {
            return this.saNodeName;
        }
    }

    [Serializable]
    public class SceneActionPackage
    {
        // 节点名称
        public string saNodeName = null;
        // 节点动作
        public SActionType aType = SActionType.NOP;
        // 参数字典
        public Dictionary<string, string> argsDict = new Dictionary<string, string>();
        // 条件从句逆波兰表达
        public string condPolish = null;
        // 下一节点
        public string next = null;
        // 下一真节点向量
        public List<string> trueRouting = null;
        // 下一假节点向量
        public List<string> falseRouting = null;
        // 是否依存函数
        public bool isBelongFunc = false;
        // 依存函数名
        public string funcName = null;
        // 附加值
        public string aTag = null;
        // 脏位
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
        act_waituser
    }
}
