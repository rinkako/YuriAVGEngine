using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 单词类：负责装填词法分析阶段的结果的最小单元
    /// </summary>
    public sealed class Token
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
        // 词类型
        public TokenType aType = TokenType.Token_NOP;
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
        Token_LeftBrace,
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
        Token_a,
        // 显示图片
        Token_picture,
        // 移动图片
        Token_move,
        // 消去图片
        Token_deletepicture,
        // 显示立绘
        Token_cstand,
        // 消去立绘
        Token_deletecstand,
        // 播放声效
        Token_se,
        // 播放音乐
        Token_bgm,
        // 停止音乐
        Token_stopbgm,
        // 播放语音
        Token_vocal,
        // 停止语音
        Token_stopvocal,
        // 返回标题
        Token_title,
        // 调用菜单
        Token_menu,
        // 调用存档
        Token_save,
        // 调用读档
        Token_load,
        // 标签
        Token_lable,
        // 标签跳转
        Token_jump,
        // 循环（头）
        Token_for,
        // 循环（尾）
        Token_endfor,
        // 条件（头）
        Token_if,
        // 条件（分支）
        Token_else,
        // 条件（尾）
        Token_endif,
        // 剧本跳转
        Token_scene,
        // 开关操作
        Token_switch,
        // 变量操作
        Token_var,
        // 退出循环
        Token_break,
        // 退出程序
        Token_shutdown,
        // 等待
        Token_wait,
        // 参数：名称
        Token_name,
        // 参数：语音id
        Token_vid,
        // 参数：立绘表情
        Token_face,
        // 参数：序号
        Token_id,
        // 参数：x坐标
        Token_x,
        // 参数：y坐标
        Token_y,
        // 参数：z坐标
        Token_z,
        // 参数：x加速度
        Token_xacc,
        // 参数：y加速度
        Token_yacc,
        // 参数：透明度
        Token_capacity,
        // 参数：x轴缩放比
        Token_xscale,
        // 参数：y轴缩放比
        Token_yscale,
        // 参数：时间
        Token_time,
        // 参数：文件名
        Token_filename,
        // 参数：音轨号
        Token_track,
        // 参数：条件子句
        Token_cond,
        // 参数：表达式
        Token_dash,
        // 参数：开关状态
        Token_state,
        // 参数：音量
        Token_vol,
        // 参数：位置
        Token_loc,
        // 参数：角度
        Token_ro,
        // 参数值：左
        Token_left,
        // 参数值：右
        Token_right,
        // 参数值：中
        Token_mid
    }
}
