using System;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
{
    using iHandle = Func<SyntaxTreeNode, CFunctionType, SyntaxType, Token, SyntaxTreeNode>;

    /// <summary>
    /// 候选式类：指导编译路径的最小单元
    /// </summary>
    internal sealed class CandidateFunction
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
        public SyntaxTreeNode Call(SyntaxTreeNode subroot, SyntaxType syntaxer, Token detail)
        {
            return this.candidateProcessor(subroot, this.candidateType, syntaxer, detail);
        }

        /// <summary>
        /// 处理器指针
        /// </summary>
        private iHandle candidateProcessor = null;

        /// <summary>
        /// 产生式类型
        /// </summary>
        private CFunctionType candidateType = CFunctionType.None;
    }
}
