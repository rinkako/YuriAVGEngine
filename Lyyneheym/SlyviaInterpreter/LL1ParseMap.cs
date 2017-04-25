using System;
using System.Collections.Generic;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
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
            this.rowCount = row;
            this.colCount = col;
            this.parserMap = new CandidateFunction[row, col];
        }

        /// <summary>
        /// 设置行类型
        /// </summary>
        /// <param name="rowid">行号</param>
        /// <param name="st">syntax类型</param>
        public void SetRow(int rowid, SyntaxType st)
        {
            if (0 <= rowid && rowid < this.rowCount)
            {
                leftNodesDict.Add(st, rowid);
            }
        }

        /// <summary>
        /// 设置列类型
        /// </summary>
        /// <param name="colid">列号</param>
        /// <param name="st">token类型</param>
        public void SetCol(int colid, TokenType st)
        {
            if (0 <= colid && colid < this.colCount)
            {
                nextLeavesDict.Add(st, colid);
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
            if (0 <= row && row < this.rowCount && 0 <= col && col < this.colCount)
            {
                this.parserMap[row, col] = proc;
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
            this.SetCellular(this.leftNodesDict[left], this.nextLeavesDict[leave], proc);
        }

        /// <summary>
        /// 取得节点的处理函数
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="col">列号</param>
        /// <returns>此节点的处理函数</returns>
        public CandidateFunction GetCFunction(int row, int col)
        {
            return this.parserMap[row, col];
        }

        /// <summary>
        /// 取得节点的处理函数
        /// </summary>
        /// <param name="left">语法类型</param>
        /// <param name="leave">Token类型</param>
        /// <param name="nilserver">空节点展开式处理函数</param>
        /// <returns>候选式实例</returns>
        public CandidateFunction GetCFunction(SyntaxType left, TokenType leave, iHandle nilserver)
        {
            try
            {
                if (left == SyntaxType.epsilonLeave)
                {
                    return new CandidateFunction(nilserver, CFunctionType.umi_epsilon);
                }
                CandidateFunction candidator = this.GetCFunction(this.leftNodesDict[left], this.nextLeavesDict[leave]);
                return candidator ?? new CandidateFunction(null, CFunctionType.umi_errorEnd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(String.Format("{0} --> {1}", left.ToString(), leave.ToString()));
                throw;
            }
        }

        /// <summary>
        /// 行游标
        /// </summary>
        private readonly int rowCount;

        /// <summary>
        /// 列游标
        /// </summary>
        private readonly int colCount;

        /// <summary>
        /// 产生式左字典
        /// </summary>
        private readonly Dictionary<SyntaxType, int> leftNodesDict = new Dictionary<SyntaxType,int>();
        
        /// <summary>
        /// 产生式右字典
        /// </summary>
        private readonly Dictionary<TokenType, int> nextLeavesDict = new Dictionary<TokenType,int>();
        
        /// <summary>
        /// LL1预测表
        /// </summary>
        private readonly CandidateFunction[,] parserMap = null;
    }
}
