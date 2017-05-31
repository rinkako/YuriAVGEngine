using System;
using System.Text;
using Yuri.YuriInterpreter.YuriILEnum;

namespace Yuri.YuriInterpreter
{
    /// <summary>
    /// 异常类：编译异常
    /// </summary>
    [Serializable]
    internal class InterpreterException : Exception
    {
        /// <summary>
        /// 用户脚本编译错误
        /// </summary>
        public InterpreterException()
        {
            //this.Message = "用户脚本编译错误";
            //try
            //{
            //    System.Windows.Forms.MessageBox.Show(this.ToStringImpl());
            //}
            //catch
            //{
            //    Console.WriteLine("Compile Error");
            //}
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
        /// 重写字符串化方法
        /// </summary>
        /// <returns>错误信息</returns>
        public override string ToString()
        {
            return this.ToStringImpl();
        }

        /// <summary>
        /// 错误实例字符串化
        /// </summary>
        /// <returns>错误信息</returns>
        private string ToStringImpl()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("在编译过程中发生了错误：");
            sb.AppendLine(String.Format("At:    {0} -> {1}, {2}", this.SceneFileName, this.HitLine + 1, this.HitColumn + 1));
            sb.AppendLine(String.Format("Phase: {0}", this.HitPhase));
            sb.AppendLine(String.Format("Info:  {0}", this.Message));
            sb.AppendLine(String.Format("             {0}", DateTime.Now.ToLocalTime()));
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
