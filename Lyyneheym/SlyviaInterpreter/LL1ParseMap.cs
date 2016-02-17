using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.SlyviaInterpreter
{
    using iHandle = Func<SyntaxTreeNode, CFunctionType, SyntaxType, Token, SyntaxTreeNode>;

    /// <summary>
    /// 匹配表类：候选式的二维字典容器
    /// </summary>
    internal sealed class LL1ParseMap
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="row">最大行数</param>
        /// <param name="col">最大列数</param>
        public LL1ParseMap(int row, int col)
        {
            this.iRowCount = row;
            this.iColCount = col;
            this.iParserMap = new CandidateFunction[row, col];
        }

        /// <summary>
        /// 设置行类型
        /// </summary>
        /// <param name="rowid">行号</param>
        /// <param name="st">syntax类型</param>
        public void SetRow(int rowid, SyntaxType st)
        {
            if (0 <= rowid && rowid < this.iRowCount)
            {
                iLeftNodes.Add(st, rowid);
            }
        }

        /// <summary>
        /// 设置列类型
        /// </summary>
        /// <param name="colid">列号</param>
        /// <param name="st">token类型</param>
        public void SetCol(int colid, TokenType st)
        {
            if (0 <= colid && colid < this.iColCount)
            {
                iNextLeaves.Add(st, colid);
            }
        }

        /// <summary>
        /// 设置节点分析函数
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="col">列号</param>
        /// <param name="proc">产生式</param>
        public void SetCellular(int row, int col, CandidateFunction proc)
        {
            if (0 <= row && row < this.iRowCount &&
              0 <= col && col < this.iColCount)
            {
                this.iParserMap[row, col] = proc;
            }
        }

        /// <summary>
        /// 设置节点分析函数
        /// </summary>
        /// <param name="left">语法类型</param>
        /// <param name="leave">Token类型</param>
        /// <param name="proc">产生式</param>
        public void SetCellular(SyntaxType left, TokenType leave, CandidateFunction proc)
        {
            this.SetCellular(this.iLeftNodes[left], this.iNextLeaves[leave], proc);
        }

        /// <summary>
        /// 取得节点的处理函数
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="col">列号</param>
        /// <returns>此节点的处理函数</returns>
        public CandidateFunction GetCFunciton(int row, int col)
        {
            return this.iParserMap[row, col];
        }

        /// <summary>
        /// 取得节点的处理函数
        /// </summary>
        /// <param name="left">语法类型</param>
        /// <param name="leave">Token类型</param>
        /// <param name="nilserver">空节点展开式处理函数</param>
        /// <returns>候选式实例</returns>
        public CandidateFunction GetCFunciton(SyntaxType left, TokenType leave, iHandle nilserver)
        {
            try
            {
                if (left == SyntaxType.epsilonLeave)
                {
                    return new CandidateFunction(nilserver, CFunctionType.umi_epsilon);
                }
                CandidateFunction candidator = this.GetCFunciton(this.iLeftNodes[left], this.iNextLeaves[leave]);
                return candidator == null ? new CandidateFunction(null, CFunctionType.umi_errorEnd) : candidator;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(String.Format("{0} --> {1}", left.ToString(), leave.ToString()));
                throw;
            }
        }

        // 行游标
        private int iRowCount = 0;
        // 列游标
        private int iColCount = 0;
        // 产生式左字典
        private Dictionary<SyntaxType, int> iLeftNodes = new Dictionary<SyntaxType,int>();
        // 产生式右字典
        private Dictionary<TokenType, int> iNextLeaves = new Dictionary<TokenType,int>();
        // LL1预测表
        private CandidateFunction[,] iParserMap = null;
    }
}
