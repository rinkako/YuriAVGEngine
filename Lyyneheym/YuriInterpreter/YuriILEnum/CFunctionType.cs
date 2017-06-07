namespace Yuri.YuriInterpreter.YuriILEnum
{
    /// <summary>
    /// 枚举：候选式类型
    /// </summary>
    public enum CFunctionType
    {
        /// <summary>
        /// null
        /// </summary>
        None,
        /// <summary>
        /// (disjunct) -> (conjunct) (disjunct_pi)
        /// </summary>
        deri___disjunct__conjunct__disjunct_pi_35,
        /// <summary>
        /// (disjunct_pi) -> "||" (conjunct) (disjunct_pi)
        /// </summary>
        deri___disjunct_pi__conjunct__disjunct_pi_36,
        /// <summary>
        /// (disjunct_pi) -> epsilon
        /// </summary>
        deri___disjunct_pi__epsilon_37,
        /// <summary>
        /// (conjunct) -> (bool) (conjunct_pi)
        /// </summary>
        deri___conjunct__bool__conjunct_pi_38,
        /// <summary>
        /// (conjunct_pi) -> "&amp;&amp;" (bool) (conjunct_pi)
        /// </summary>
        deri___conjunct_pi__bool__conjunct_pi_39,
        /// <summary>
        /// (conjunct_pi) -> epsilon
        /// </summary>
        deri___conjunct_pi__epsilon_40,
        /// <summary>
        /// (bool) -> "!" (bool)
        /// </summary>
        deri___bool__not_bool_42,
        /// <summary>
        /// (bool) -> (comp)
        /// </summary>
        deri___bool__comp_43,
        /// <summary>
        /// (comp) -> (wexpr) (rop) (wexpr)
        /// </summary>
        deri___comp__wexpr__rop__wexpr_44,
        /// <summary>
        /// (rop) -> "&lt;&gt;"
        /// </summary>
        deri___rop__lessgreater_58,
        /// <summary>
        /// (rop) -> "=="
        /// </summary>
        deri___rop__equalequal_59,
        /// <summary>
        /// (rop) -> "&gt;"
        /// </summary>
        deri___rop__greater_60,
        /// <summary>
        /// (rop) -> "&lt;"
        /// </summary>
        deri___rop__less_61,
        /// <summary>
        /// (rop) -> "&gt;="
        /// </summary>
        deri___rop__greaterequal_62,
        /// <summary>
        /// (rop) -> "&lt;="
        /// </summary>
        deri___rop__lessequal_63,
        /// <summary>
        /// (rop) -> epsilon
        /// </summary>
        deri___rop__epsilon_80,
        /// <summary>
        /// (wexpr) -> (wmulti) (wexpr_pi)
        /// </summary>
        deri___wexpr__wmulti__wexpr_pi_45,
        /// <summary>
        /// (wexpr) -> epsilon
        /// </summary>
        deri___wexpr__epsilon_81,
        /// <summary>
        /// (wplus) -> "+" (wmulti)
        /// </summary>
        deri___wplus__plus_wmulti_46,
        /// <summary>
        /// (wplus) -> "-" (wmulti)
        /// </summary>
        deri___wplus__minus_wmulti_47,
        /// <summary>
        /// (wexpr_pi) -> (wplus) (wexpr_pi)
        /// </summary>
        deri___wexpr_pi__wplus__wexpr_pi_72,
        /// <summary>
        /// (wexpr_pi) -> epsilon
        /// </summary>
        deri___wexpr_pi__epsilon_73,
        /// <summary>
        /// (wmulti) -> (wunit) (wmultiOpt)
        /// </summary>
        deri___wmulti__wunit__wmultiOpt_49,
        /// <summary>
        /// (wmultiOpt) -> "*" (wunit) (wmultiOpt)
        /// </summary>
        deri___wmultiOpt__multi_wunit__wmultiOpt_50,
        /// <summary>
        /// (wmultiOpt) -> "/" (wunit) (wmultiOpt)
        /// </summary>
        deri___wmultiOpt__div_wunit__wmultiOpt_51,
        /// <summary>
        /// (wmultiOpt) -> epsilon
        /// </summary>
        deri___wmultiOpt__epsilon_52,
        /// <summary>
        /// (wunit) -> number
        /// </summary>
        deri___wunit__number_53,
        /// <summary>
        /// (wunit) -> iden
        /// </summary>
        deri___wunit__iden_54,
        /// <summary>
        /// (wunit) -> "-" (wunit)
        /// </summary>
        deri___wunit__minus_wunit_55,
        /// <summary>
        /// (wunit) -> "+" (wunit)
        /// </summary>
        deri___wunit__plus_wunit_56,
        /// <summary>
        /// (wunit) -> "(" (disjunct) ")"
        /// </summary>
        deri___wunit__brucket_disjunct_57,
        /// <summary>
        /// BOUNDARY
        /// </summary>
        DERI_UMI_BOUNDARY,
        /// <summary>
        /// LeavePoint identifier
        /// </summary>
        umi_iden,
        /// <summary>
        /// LeavePoint null
        /// </summary>
        umi_epsilon,
        /// <summary>
        /// LeavePoint "("
        /// </summary>
        umi_leftParentheses_,
        /// <summary>
        /// LeavePoint ")"
        /// </summary>
        umi_rightParentheses_,
        /// <summary>
        /// LeavePoint "="
        /// </summary>
        umi_equality_,
        /// <summary>
        /// LeavePoint "+"
        /// </summary>
        umi_plus_,
        /// <summary>
        /// LeavePoint "-"
        /// </summary>
        umi_minus_,
        /// <summary>
        /// LeavePoint "*"
        /// </summary>
        umi_multiply_,
        /// <summary>
        /// LeavePoint "/"
        /// </summary>
        umi_divide_,
        /// <summary>
        /// LeavePoint number
        /// </summary>
        umi_number,
        /// <summary>
        /// LeavePoint "||"
        /// </summary>
        umi_or_Or_,
        /// <summary>
        /// LeavePoint "&amp;&amp;"
        /// </summary>
        umi_and_And_,
        /// <summary>
        /// LeavePoint "!"
        /// </summary>
        umi_not_,
        /// <summary>
        /// LeavePoint "&lt;&gt;"
        /// </summary>
        umi_lessThan_GreaterThan_,
        /// <summary>
        /// LeavePoint "=="
        /// </summary>
        umi_equality_Equality_,
        /// <summary>
        /// LeavePoint "&gt;"
        /// </summary>
        umi_greaterThan_,
        /// <summary>
        /// LeavePoint "&lt;"
        /// </summary>
        umi_lessThan_,
        /// <summary>
        /// LeavePoint "&gt;="
        /// </summary>
        umi_greaterThan_Equality_,
        /// <summary>
        /// LeavePoint "&lt;="
        /// </summary>
        umi_lessThan_Equality_,
        /// <summary>
        /// LeavePoint #
        /// </summary>
        umi_startEnd,
        /// <summary>
        /// ERROR
        /// </summary>
        umi_errorEnd
    }
}