using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyyneheymCore.SlyviaPile
{
    /// <summary>
    /// 词法分析器类：负责把用户脚本分割为单词流的类
    /// </summary>
    internal sealed class Lexer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Lexer()
        {
            this.currentLine = RowStartNum;
            this.currentColumn = ColStartNum;
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="scenario">待分析的剧本文本</param>
        /// <param name="resetFloatPointer">是否重置游标</param>
        public void init(string scenario, bool resetFloatPointer = false)
        {
            this.finFlag = true;
            this.blockFlag = false;
            this.sourceCode = scenario;
            this.nextCharPointer = 0;
            this.resultVector = new List<Token>();
            if (resetFloatPointer)
            {
                this.currentColumn = ColStartNum;
                this.currentLine = RowStartNum;
            }
        }

        /// <summary>
        /// 启动词法分析DFA自动机
        /// </summary>
        /// <returns>词法分析完毕的的单词流</returns>
        public List<Token> Analyse()
        {
            // 如果上一轮词法分析完成，那么就要重新构造结果向量
            if (this.finFlag == true)
            {
                this.resultVector = new List<Token>();
            }
            // 分析这个句子
            bool sentenceFlag = false;
            while (this.nextCharPointer < this.sourceCode.Length)
            {
                Token nextToken;
                sentenceFlag = this.DFA(out nextToken);
                if (nextToken != null)
                {
                    this.resultVector.Add(nextToken);
                }
            }
            // 如果句子标志位还没有成功这一轮分析就没有完成
            this.finFlag = sentenceFlag;
            // 把结果向量返回给上层
            return this.resultVector;
        }

        /// <summary>
        /// 返回是否已经完成了一轮词法分析
        /// </summary>
        /// <returns>词法分析完成标志位</returns>
        public bool isComplete()
        {
            return this.finFlag;
        }

        /// <summary>
        /// 将一段文本追加到待分析字符串末尾
        /// </summary>
        /// <param name="appender">待追加的字符串</param>
        public void appendCode(string appender)
        {
            this.sourceCode += appender;
        }

        /// <summary>
        /// <para>DFA自动机</para>
        /// <para>处理当前代码指针所在的单词，并移动指针</para>
        /// <para>如果遇到未知的字符，将抛出错误</para>
        /// </summary>
        /// <param name="nextToken">当前代码指针所指单词实例</param>
        /// <returns>是否已经完成了一个句子的分析</returns>
        private bool DFA(out Token nextToken)
        {
            // 定义结果实例并初始化
            Token res = new Token();
            res.aLine = this.currentLine;
            res.aColumn = this.currentColumn;
            res.indexOfCode = this.nextCharPointer;
            int alen = this.sourceCode.Length;
            // 获取下一个字符来判断自动机路径
            bool successFlag = false;
            CharacterType cara = this.GetCharType(this.sourceCode[this.nextCharPointer]);
            switch (cara)
            {
                // 单字符token
                case CharacterType.Plus:
                case CharacterType.Minus:
                case CharacterType.Multiply:
                case CharacterType.Divide:
                case CharacterType.Not:
                case CharacterType.At:
                case CharacterType.LeftParentheses:
                case CharacterType.LeftBracket:
                case CharacterType.RightParentheses:
                case CharacterType.RightBracket:
                    successFlag = this.getSingleCharaCalculator(res);
                    break;
                // 可能双字符token
                case CharacterType.Equality:
                case CharacterType.LessThan:
                case CharacterType.GreaterThan:
                case CharacterType.And:
                case CharacterType.Or:
                    successFlag = this.getDoubleCharaCalculator(res);
                    break;
                // 关键字和标识符（实参字串的值在这里处理）
                case CharacterType.Letter:
                case CharacterType.UnderLine:
                case CharacterType.Dollar:
                    successFlag = this.getReservedCalculator(res);
                    break;
                // 字符串
                case CharacterType.Quotation:
                case CharacterType.DoubleQuotation:
                    successFlag = this.getCluster(res);
                    break;
                // 剧本对白
                case CharacterType.LeftBrace:
                case CharacterType.RightBrace:
                    successFlag = this.getSceneCluster(res);
                    break;
                // 常数
                case CharacterType.Number:
                    successFlag = this.getConstant(res);
                    break;
                // 空白
                case CharacterType.Space:
                    successFlag = this.getSpace(res);
                    break;
                // 谜
                default:
                    successFlag = this.getUnknown(res);
                    break;
            }
            // 如果成功获得了token，就返回给Lexer
            if (successFlag)
            {
                res.length = this.nextCharPointer - res.indexOfCode;
                nextToken = res;
                return blockFlag == false;
            }
            // 否则返回空
            nextToken = null;
            return false;
        }




        /// <summary>
        /// 单字符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSingleCharaCalculator(Token res)
        {
            return false;
        }

        /// <summary>
        /// 可能双字符的自动机路径（最长匹配）
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getDoubleCharaCalculator(Token res)
        {
            return false;
        }

        /// <summary>
        /// 关键字和标识符的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getReservedCalculator(Token res)
        {
            return false;
        }

        /// <summary>
        /// 剧本对白的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSceneCluster(Token res)
        {
            return false;
        }

        /// <summary>
        /// 字符串的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getCluster(Token res)
        {
            return false;
        }

        /// <summary>
        /// 常数数字的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getConstant(Token res)
        {
            return false;
        }

        /// <summary>
        /// 空白符号的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getSpace(Token res)
        {
            // 获取字符，看是否要换行
            char c = this.sourceCode[this.nextCharPointer];
            if (c == LineTerminator)
            {
                this.currentLine++;
                this.currentColumn = 0;
            }
            // 为空格跳游程
            if (c == ' ')
            {
                this.jump(1);
            }
            else
            {
                this.nextCharPointer++;
            }
            // 空token，永久为false
            return false;
        }

        /// <summary>
        /// 其余情况的自动机路径
        /// </summary>
        /// <param name="res">结果实例</param>
        /// <returns>是否命中</returns>
        private bool getUnknown(Token res)
        {
            return false;
        }

        /// <summary>
        /// 计算指针跳跃和列数的修正
        /// </summary>
        /// <param name="go"></param>
        private void jump(int go)
        {
            this.currentColumn += go;
            this.nextCharPointer += go;
        }

        /// <summary>
        /// 判断一个字符属于哪种类型
        /// </summary>
        /// <param name="c">待分析字符</param>
        /// <returns>字符的类型</returns>
        private CharacterType GetCharType(char c)
        {
            if (c <= 0x9fbb && c >= 0x4e00) { return CharacterType.Chinese; }
            if ('a' <= c && c <= 'z') { return CharacterType.Letter; }
            if ('A' <= c && c <= 'Z') { return CharacterType.Letter; }
            if ('0' <= c && c <= '9') { return CharacterType.Number; }
            if (c == ' ') { return CharacterType.Space; }
            if (c == '_') { return CharacterType.UnderLine; }
            if (c == '.') { return CharacterType.Dot; }
            if (c == ',') { return CharacterType.Comma; }
            if (c == '+') { return CharacterType.Plus; }
            if (c == '-') { return CharacterType.Minus; }
            if (c == '*') { return CharacterType.Multiply; }
            if (c == '/') { return CharacterType.Divide; }
            if (c == '%') { return CharacterType.Percent; }
            if (c == '^') { return CharacterType.Xor; }
            if (c == '&') { return CharacterType.And; }
            if (c == '|') { return CharacterType.Or; }
            if (c == '~') { return CharacterType.Reverse; }
            if (c == '$') { return CharacterType.Dollar; }
            if (c == '<') { return CharacterType.LessThan; }
            if (c == '>') { return CharacterType.GreaterThan; }
            if (c == '(') { return CharacterType.LeftParentheses; }
            if (c == ')') { return CharacterType.RightParentheses; }
            if (c == '[') { return CharacterType.LeftBracket; }
            if (c == ']') { return CharacterType.RightBracket; }
            if (c == '{') { return CharacterType.LeftBrace; }
            if (c == '}') { return CharacterType.RightBrace; }
            if (c == '!') { return CharacterType.Not; }
            if (c == '#') { return CharacterType.Pound; }
            if (c == '?') { return CharacterType.Question; }
            if (c == '"') { return CharacterType.DoubleQuotation; }
            if (c == ':') { return CharacterType.Colon; }
            if (c == ';') { return CharacterType.Semicolon; }
            if (c == '=') { return CharacterType.Equality; }
            if (c == '@') { return CharacterType.At; }
            if (c == '\\') { return CharacterType.Slash; }
            if (c == '\'') { return CharacterType.Quotation; }
            if (c == '\t') { return CharacterType.Space; }
            if (c == '\r') { return CharacterType.Space; }
            if (c == '\n') { return CharacterType.Space; }
            return CharacterType.cUnknown;
        }

        // 原剧本字串
        private string sourceCode = "";
        // 指针
        private int nextCharPointer = 0;
        // 行游标
        private int currentLine = 0;
        // 列游标
        private int currentColumn = 0;
        // 单词流向量
        private List<Token> resultVector = null;
        // 词法分析完成标志位
        private bool finFlag = true;
        // 剧本花括弧匹配标志位
        private bool blockFlag = false;
        // 列初始值
        private static readonly int ColStartNum = 1;
        // 行初始值
        private static readonly int RowStartNum = 1;
        // 换行符
        private static readonly char LineTerminator = '\n';
    }

    /// <summary>
    /// 枚举：字符类型
    /// </summary>
    internal enum CharacterType
    {
        // 未知
        cUnknown,
        // 字母
        Letter,
        // 中文
        Chinese,
        // 数字
        Number,
        // _
        UnderLine,
        // .
        Dot,
        // ,
        Comma,
        // +
        Plus,
        // -
        Minus,
        // *
        Multiply,
        // /
        Divide,
        // %
        Percent,
        // ^
        Xor,
        // &;
        And,
        // |
        Or,
        // ~
        Reverse,
        // $
        Dollar,
        // <
        LessThan,
        // >
        GreaterThan,
        // (
        LeftParentheses,
        // )
        RightParentheses,
        // [
        LeftBracket,
        // ]
        RightBracket,
        // {
        LeftBrace,
        // }
        RightBrace,
        // !
        Not,
        // #
        Pound,
        // "\\"
        Slash,
        // ?
        Question,
        // '
        Quotation,
        // "
        DoubleQuotation,
        // :
        Colon,
        // ;
        Semicolon,
        // =
        Equality,
        // @
        At,
        // space Tab \r\n
        Space
    };

    /// <summary>
    /// 枚举：单词类型
    /// </summary>
    internal enum TokenType
    {
        // 未知
        unknown,
        // 标识符
        identifier,
        // 字符串
        cluster,
        // 整数数字
        number,
        // 起始终止标记
        startend,
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
        // 符号：朵拉符$
        Token_Dollar,
        // 符号：左括弧(
        token_LeftParentheses,
        // 符号：右括弧)
        token_RightParentheses,
        // 符号：加+
        token_Plus,
        // 符号：减-
        token_Minus,
        // 符号：乘*
        token_Multiply,
        // 符号：除/
        token_Divide,
        // 符号：不等号<>
        token_LessThan_GreaterThan_,
        // 符号：等于号==
        token_Equality_Equality_,
        // 符号：大于号>
        token_GreaterThan_,
        // 符号：小于号<
        token_LessThan_,
        // 符号：大于等于号>=
        token_GreaterThan_Equality_,
        // 符号：小于等于号<=
        token_LessThan_Equality_,
        // 符号：逻辑或||
        token_Or_Or_,
        // 符号：逻辑与&&
        token_And_And_,
        // 符号：逻辑否!
        token_Not_,
        // 空操作
        Token_NOP,
        // 显示文本
        Token_a,
        // 显示图片
        Token_picture,
        // 移动图片
        Token_move,
        // 消去图片
        Token_deletepicure,
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
        Token_dash
    }
}
