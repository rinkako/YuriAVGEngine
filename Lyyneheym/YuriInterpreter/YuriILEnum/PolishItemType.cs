namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：逆波兰式项类型
    /// </summary>
    internal enum OptimizerPolishItemType
    {
        NONE,
        CONSTANT,
        STRING,
        VAR,
        VAR_NUM,
        VAR_STRING,
        CAL_PLUS,
        CAL_MINUS,
        CAL_MULTI,
        CAL_DIV,
        CAL_ANDAND,
        CAL_OROR,
        CAL_NOT,
        CAL_EQUAL,
        CAL_NOTEQUAL,
        CAL_BIG,
        CAL_SMALL,
        CAL_BIGEQUAL,
        CAL_SMALLEQUAL
    }
}