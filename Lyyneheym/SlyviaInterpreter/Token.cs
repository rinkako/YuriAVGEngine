using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.SlyviaInterpreter
{
    /// <summary>
    /// 单词类：负责装填词法分析阶段的结果的最小单元
    /// </summary>
    internal sealed class Token
    {
        // 命中行
        public int aLine = -1;
        // 命中列
        public int aColumn = -1;
        // 位置戳
        public int indexOfCode = -1;
        // 配对长
        public int length = 0;
        // 附加值
        public object aTag = null;
        // 原字串
        public string detail = "";
        // 错误位
        public bool errorBit = false;
        // 变量位
        public bool isVar = false;
        // 作用域
        public bool isGlobal = false;
        // 词类型
        public TokenType aType = TokenType.Token_NOP;

        public override string ToString()
        {
            return "Token: " + aType.ToString() + " (" + this.detail + ") -> " + 
                (this.aTag == null ? "null" : this.aTag.ToString());
        }
    }

    /// <summary>
    /// 枚举：单词类型
    /// </summary>
    public enum TokenType
    {
        // 未知
        unknown,
        // 标识符
        identifier,
        // 字符串
        cluster,
        // 剧本字符串
        scenecluster,
        // 剧本段落终结符
        sceneterminator,
        // 整数数字
        number,
        // 起始终止标记
        startend,
        // 符号：#
        Token_Sharp,
        // 符号：左方括号[
        Token_LeftBracket,
        // 符号：右方括号]
        Token_RightBracket,
        // 符号：左花括号{
        //Token_LeftBrace,
        // 符号：右花括号}
        Token_RightBrace,
        // 符号：单引号'
        Token_Quotation,
        // 符号：双引号"
        Token_DoubleQuotation,
        // 符号：等号=
        Token_Equality,
        // 符号：艾特符@
        Token_At,
        // 符号：朵拉符$
        Token_Dollar,
        // 符号：地址符&
        Token_Address,
        // 符号：左括弧(
        Token_LeftParentheses,
        // 符号：右括弧)
        Token_RightParentheses,
        // 符号：加+
        Token_Plus,
        // 符号：减-
        Token_Minus,
        // 符号：乘*
        Token_Multiply,
        // 符号：除/
        Token_Divide,
        // 符号：不等号<>
        Token_LessThan_GreaterThan,
        // 符号：等于号==
        Token_Equality_Equality,
        // 符号：大于号>
        Token_GreaterThan,
        // 符号：小于号<
        Token_LessThan,
        // 符号：大于等于号>=
        Token_GreaterThan_Equality,
        // 符号：小于等于号<=
        Token_LessThan_Equality,
        // 符号：逻辑或||
        Token_Or_Or,
        // 符号：逻辑与&&
        Token_And_And,
        // 符号：逻辑否!
        Token_Not,
        // 空操作
        Token_NOP,
        // 显示文本
        Token_o_a,
        // 显示背景
        Token_o_bg,
        // 显示图片
        Token_o_picture,
        // 移动图片
        Token_o_move,
        // 消去图片
        Token_o_deletepicture,
        // 显示立绘
        Token_o_cstand,
        // 消去立绘
        Token_o_deletecstand,
        // 播放声效
        Token_o_se,
        // 播放音乐
        Token_o_bgm,
        // 停止音乐
        Token_o_stopbgm,
        // 播放语音
        Token_o_vocal,
        // 停止语音
        Token_o_stopvocal,
        // 返回标题
        Token_o_title,
        // 调用菜单
        Token_o_menu,
        // 调用存档
        Token_o_save,
        // 调用读档
        Token_o_load,
        // 标签
        Token_o_lable,
        // 标签跳转
        Token_o_jump,
        // 循环（头）
        Token_o_for,
        // 循环（尾）
        Token_o_endfor,
        // 条件（头）
        Token_o_if,
        // 条件（分支）
        Token_o_else,
        // 条件（尾）
        Token_o_endif,
        // 剧本跳转
        Token_o_scene,
        // 开关操作
        Token_o_switch,
        // 变量操作
        Token_o_var,
        // 退出循环
        Token_o_break,
        // 退出程序
        Token_o_shutdown,
        // 等待
        Token_o_wait,
        // 选择支
        Token_o_branch,
        // 函数定义头
        Token_o_function,
        // 函数定义尾
        Token_o_endfunction,
        // 函数调用
        Token_o_call,
        // 标志回归点
        Token_o_titlepoint,
        // 准备渐变
        Token_o_freeze,
        // 执行渐变
        Token_o_trans,
        // 按钮
        Token_o_button,
        // 对话样式
        Token_o_style,
        // 切换文字层
        Token_o_msglayer,
        // 修改层属性
        Token_o_msglayeropt,
        // 等待用户操作
        Token_o_waituser,
        // 等待动画完成
        Token_o_waitani,
        // 描绘字符串
        Token_o_draw,
        // 参数：函数签名
        Token_p_sign,
        // 参数：名称
        Token_p_name,
        // 参数：语音id
        Token_p_vid,
        // 参数：立绘表情
        Token_p_face,
        // 参数：序号
        Token_p_id,
        // 参数：x坐标
        Token_p_x,
        // 参数：y坐标
        Token_p_y,
        // 参数：z坐标
        Token_p_z,
        // 参数：加速度
        Token_p_acc,
        // 参数：x加速度
        Token_p_xacc,
        // 参数：y加速度
        Token_p_yacc,
        // 参数：透明度
        Token_p_opacity,
        // 参数：x轴缩放比
        Token_p_xscale,
        // 参数：y轴缩放比
        Token_p_yscale,
        // 参数：时间
        Token_p_time,
        // 参数：文件名
        Token_p_filename,
        // 参数：音轨号
        Token_p_track,
        // 参数：条件子句
        Token_p_cond,
        // 参数：表达式
        Token_p_dash,
        // 参数：开关状态
        Token_p_state,
        // 参数：音量
        Token_p_vol,
        // 参数：位置
        Token_p_loc,
        // 参数：角度
        Token_p_ro,
        // 参数：选择支链
        Token_p_link,
        // 参数：宽度
        Token_p_width,
        // 参数：高度
        Token_p_height,
        // 参数：字体
        Token_p_font,
        // 参数：尺寸
        Token_p_size,
        // 参数：颜色
        Token_p_color,
        // 参数：作用目标
        Token_p_target,
        // 参数：正常按钮
        Token_p_normal,
        // 参数：鼠标悬停按钮
        Token_p_over,
        // 参数：鼠标按下按钮
        Token_p_on
    }
}
