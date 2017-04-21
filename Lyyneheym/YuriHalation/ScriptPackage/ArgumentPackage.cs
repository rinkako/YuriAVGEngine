using System;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 参数包装类
    /// </summary>
    [Serializable]
    internal class ArgumentPackage
    {
        /// <summary>
        /// 参数的类型
        /// </summary>
        public ArgType aType = ArgType.unknown;

        /// <summary>
        /// 参数的值表达式
        /// </summary>
        public string valueExp = String.Empty;
    }

    /// <summary>
    /// 枚举：参数类型
    /// </summary>
    public enum ArgType
    {
        // 未知
        unknown,
        // 参数：类型
        Arg_type,
        // 参数：函数签名
        Arg_sign,
        // 参数：名称
        Arg_name,
        // 参数：语音id
        Arg_vid,
        // 参数：立绘表情
        Arg_face,
        // 参数：序号
        Arg_id,
        // 参数：x坐标
        Arg_x,
        // 参数：y坐标
        Arg_y,
        // 参数：z坐标
        Arg_z,
        // 参数：加速度
        Arg_acc,
        // 参数：x加速度
        Arg_xacc,
        // 参数：y加速度
        Arg_yacc,
        // 参数：透明度
        Arg_opacity,
        // 参数：x轴缩放比
        Arg_xscale,
        // 参数：y轴缩放比
        Arg_yscale,
        // 参数：时间
        Arg_time,
        // 参数：文件名
        Arg_filename,
        // 参数：音轨号
        Arg_track,
        // 参数：条件子句
        Arg_cond,
        // 参数：表达式
        Arg_dash,
        // 参数：开关状态
        Arg_state,
        // 参数：音量
        Arg_vol,
        // 参数：位置
        Arg_loc,
        // 参数：角度
        Arg_ro,
        // 参数：选择支链
        Arg_link,
        // 参数：宽度
        Arg_width,
        // 参数：高度
        Arg_height,
        // 参数：字体
        Arg_font,
        // 参数：尺寸
        Arg_size,
        // 参数：颜色
        Arg_color,
        // 参数：作用目标
        Arg_target,
        // 参数：正常按钮
        Arg_normal,
        // 参数：鼠标悬停按钮
        Arg_over,
        // 参数：鼠标按下按钮
        Arg_on
    }

}
