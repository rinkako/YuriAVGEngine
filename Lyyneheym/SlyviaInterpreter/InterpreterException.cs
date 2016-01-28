using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyyneheym.SlyviaInterpreter
{
    [Serializable]
    internal class InterpreterException : Exception
    {
        /// <summary>
        /// 用户脚本编译错误
        /// </summary>
        public InterpreterException()
        {
            this.Message = "用户脚本编译错误";
        }

        /// <summary>
        /// 用户脚本编译错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public InterpreterException(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// 用户脚本编译错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="inner">包含异常</param>
        public InterpreterException(string message, Exception inner)
        {
            this.Message = message;
            this.InnerException = inner;
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public new string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 此异常中包含的异常
        /// </summary>
        public new Exception InnerException
        {
            get;
            set;
        }

        /// <summary>
        /// 错误发生的脚本文件
        /// </summary>
        public string SceneFileName
        {
            get;
            set;
        }

        /// <summary>
        /// 错误在源代码中的行数
        /// </summary>
        public int HitLine
        {
            get;
            set;
        }

        /// <summary>
        /// 错误在源代码中的列数
        /// </summary>
        public int HitColumn
        {
            get;
            set;
        }

        /// <summary>
        /// 错误发生的编译阶段
        /// </summary>
        public InterpreterPhase HitPhase
        {
            get;
            set;
        }

        /// <summary>
        /// 错误实例字符串化
        /// </summary>
        /// <returns>错误信息</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("在编译过程中发生了错误：");
            sb.AppendLine(String.Format("At:    {0} -> {1}, {2}", this.SceneFileName, this.HitLine + 1, this.HitColumn + 1));
            sb.AppendLine(String.Format("Phase: {0}", this.HitPhase.ToString()));
            sb.AppendLine(String.Format("Info:  {0}", this.Message));
            sb.AppendLine(String.Format("             {0}", DateTime.Now.ToLocalTime().ToString()));
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// 枚举：编译过程
        /// </summary>
        public enum InterpreterPhase
        {
            Lexer,
            Parser,
            Sematicer,
            Optimizer,
            ILGenerator
        }
    }
}
