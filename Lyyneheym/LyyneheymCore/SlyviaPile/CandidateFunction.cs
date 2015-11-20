using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    using iHandle = Func<SyntaxTreeNode, CFunctionType, SyntaxType, string, SyntaxTreeNode>;
    
    /// <summary>
    /// 候选式类：指导编译路径的最小单元
    /// </summary>
    public sealed class CandidateFunction
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="proc">处理函数委托</param>
        /// <param name="type">候选式类型</param>
        public CandidateFunction(iHandle proc, CFunctionType type)
        {
            this.SetProc(proc, type);
        }

        /// <summary>
        /// 设置候选式处理器
        /// </summary>
        /// <param name="proc">处理函数委托</param>
        /// <param name="type">候选式类型</param>
        public void SetProc(iHandle proc, CFunctionType type)
        {
            this.candidateProcessor = proc;
            this.candidateType = type;
        }

        /// <summary>
        /// 获得产生式的类型
        /// </summary>
        /// <returns>该候选式的类型</returns>
        public CFunctionType GetCFType()
        {
            return this.candidateType;
        }

        /// <summary>
        /// 调用产生式处理函数
        /// </summary>
        /// <param name="subroot">匹配树根节点</param>
        /// <param name="syntaxer">语法类型</param>
        /// <param name="detail">节点信息</param>
        /// <returns>产生式的处理函数</returns>
        public SyntaxTreeNode Call(SyntaxTreeNode subroot, SyntaxType syntaxer, string detail)
        {
            return this.candidateProcessor(subroot, this.candidateType, syntaxer, detail);
        }

        // 处理器指针
        private iHandle candidateProcessor = null;
        // 产生式类型
        private CFunctionType candidateType = CFunctionType.None;
    }

    /// <summary>
    /// 枚举：候选式类型
    /// </summary>
    public enum CFunctionType
    {
        // null
        None,
        // <disjunct> -> <conjunct> <disjunct_pi>
        deri___disjunct__conjunct__disjunct_pi_35,
        // <disjunct_pi> -> "||" <conjunct> <disjunct_pi>
        deri___disjunct_pi__conjunct__disjunct_pi_36,
        // <disjunct_pi> -> epsilon
        deri___disjunct_pi__epsilon_37,
        // <conjunct> -> <bool> <conjunct_pi>
        deri___conjunct__bool__conjunct_pi_38,
        // <conjunct_pi> -> "&&" <bool> <conjunct_pi>
        deri___conjunct_pi__bool__conjunct_pi_39,
        // <conjunct_pi> -> epsilon
        deri___conjunct_pi__epsilon_40,
        // <bool> -> "!" <bool>
        deri___bool__not_bool_42,
        // <bool> -> <comp>
        deri___bool__comp_43,
        // <comp> -> <wexpr> <rop> <wexpr>
        deri___comp__wexpr__rop__wexpr_44,
        // <rop> -> "<>"
        deri___rop__lessgreater_58,
        // <rop> -> "=="
        deri___rop__equalequal_59,
        // <rop> -> ">"
        deri___rop__greater_60,
        // <rop> -> "<"
        deri___rop__less_61,
        // <rop> -> ">="
        deri___rop__greaterequal_62,
        // <rop> -> "<="
        deri___rop__lessequal_63,
        // <rop> -> epsilon
        deri___rop__epsilon_80,
        // <wexpr> -> <wmulti> <wexpr_pi>
        deri___wexpr__wmulti__wexpr_pi_45,
        // <wexpr> -> epsilon
        deri___wexpr__epsilon_81,
        // <wplus> -> "+" <wmulti>
        deri___wplus__plus_wmulti_46,
        // <wplus> -> "-" <wmulti>
        deri___wplus__minus_wmulti_47,
        // <wexpr_pi> -> <wplus> <wexpr_pi>
        deri___wexpr_pi__wplus__wexpr_pi_72,
        // <wexpr_pi> -> epsilon
        deri___wexpr_pi__epsilon_73,
        // <wmulti> -> <wunit> <wmultiOpt>
        deri___wmulti__wunit__wmultiOpt_49,
        // <wmultiOpt> -> "*" <wunit> <wmultiOpt>
        deri___wmultiOpt__multi_wunit__wmultiOpt_50,
        // <wmultiOpt> -> "/" <wunit> <wmultiOpt>
        deri___wmultiOpt__div_wunit__wmultiOpt_51,
        // <wmultiOpt> -> epsilon
        deri___wmultiOpt__epsilon_52,
        // <wunit> -> number
        deri___wunit__number_53,
        // <wunit> -> iden
        deri___wunit__iden_54,
        // <wunit> -> "-" <wunit>
        deri___wunit__minus_wunit_55,
        // <wunit> -> "+" <wunit>
        deri___wunit__plus_wunit_56,
        // <wunit> -> "(" <disjunct> ")"
        deri___wunit__brucket_disjunct_57,
        // BOUNDARY
        DERI_UMI_BOUNDARY,
        // LeavePoint identifier
        umi_iden,
        // LeavePoint null
        umi_epsilon,
        // LeavePoint "="
        umi_equality_,
        // LeavePoint "+"
        umi_plus_,
        // LeavePoint "-"
        umi_minus_,
        // LeavePoint "*"
        umi_multiply_,
        // LeavePoint "/"
        umi_divide_,
        // LeavePoint number
        umi_number,
        // LeavePoint "||"
        umi_or_Or_,
        // LeavePoint "&&"
        umi_and_And_,
        // LeavePoint "!"
        umi_not_,
        // LeavePoint "<>"
        umi_lessThan_GreaterThan_,
        // LeavePoint "=="
        umi_equality_Equality_,
        // LeavePoint ">"
        umi_greaterThan_,
        // LeavePoint "<"
        umi_lessThan_,
        // LeavePoint ">="
        umi_greaterThan_Equality_,
        // LeavePoint "<="
        umi_lessThan_Equality_,
        // LeavePoint #
        umi_startEnd,
        // ERROR
        umi_errorEnd
    }
}
