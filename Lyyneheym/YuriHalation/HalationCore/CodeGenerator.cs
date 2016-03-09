using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Yuri.YuriHalation.ScriptPackage;

namespace Yuri.YuriHalation.HalationCore
{
    /// <summary>
    /// 代码生成器：将代码树生成为脚本代码
    /// </summary>
    class CodeGenerator
    {
        /// <summary>
        /// 开始生成代码
        /// </summary>
        /// <param name="thread">线程数量</param>
        public List<KeyValuePair<string, string>> Generate(int thread = 4)
        {
            thread = (thread > 8 || thread < 1) ? 8 : thread;
            // 将所有待翻译项目加入队列
            this.splitQueue = new Queue<ScenePackage>();
            foreach (var sc in Halation.project.GetScene())
            {
                this.splitQueue.Enqueue(sc);
            }
            // 开启处理线程
            this.finishedThread = 0;
            this.resList = new List<KeyValuePair<string, string>>();
            this.threadPool = new List<Thread>();
            for (int t = 0; t < this.threadNum; t++)
            {
                this.threadPool.Add(new Thread(new ParameterizedThreadStart(this.ParseHandler)));
                this.threadPool[t].IsBackground = true;
                this.threadPool[t].Start(t);
            }
            // 等待线程回调
            while (this.finishedThread < this.threadNum);
            return this.resList;
        }

        /// <summary>
        /// 多线程翻译脚本语言
        /// </summary>
        /// <param name="threadId">线程id</param>
        private void ParseHandler(object threadId)
        {
            while (true)
            {
                // 取队列中还未处理的场景
                ScenePackage handleSp = null;
                lock (this.splitQueue)
                {
                    if (this.splitQueue.Count > 0)
                    {
                        handleSp = this.splitQueue.Dequeue();
                    }
                }
                if (handleSp == null) { break; }
                StringBuilder sceneBuilder = new StringBuilder();
                // 处理主序列
                sceneBuilder.AppendLine(this.ParseToScript(handleSp));
                // 处理函数
                foreach (var fp in (handleSp as ScenePackage).GetFunc())
                {
                    sceneBuilder.AppendLine();
                    sceneBuilder.AppendLine(fp.functionDeclaration);
                    sceneBuilder.AppendLine(this.ParseToScript(fp));
                    sceneBuilder.AppendLine(fp.functionEndDeclaration);
                }
                // 生成代码加入结果向量
                lock (this.resList)
                {
                    this.resList.Add(new KeyValuePair<string, string>(handleSp.sceneName, sceneBuilder.ToString()));
                }
                lock (this.consoleMutex)
                {
                    Console.WriteLine(String.Format("Finish Code Generation \"{0}\" At thread {1}", handleSp.sceneName, (int)threadId));
                }
            }
            // 递增完成信号量
            lock (consoleMutex)
            {
                this.finishedThread++;
                Console.WriteLine(String.Format("Thread {0} is Finished", (int)threadId));
            }
        }

        /// <summary>
        /// 将代码包装转化为脚本语言
        /// </summary>
        /// <returns>对应脚本语言字符串</returns>
        private string ParseToScript(RunnablePackage rp)
        {
            StringBuilder codeBuilder = new StringBuilder();
            foreach (var act in rp.GetAction())
            {
                // 跳过空节点
                if (act.nodeType == ActionPackageType.NOP)
                {
                    continue;
                }
                // 处理缩进
                if (act.nodeType != ActionPackageType.act_dialog)
                {
                    for (int ind = 0; ind < act.indent; ind++)
                    {
                        codeBuilder.Append(' ');
                    }
                }
                // 处理语句
                switch (act.nodeType)
                {
                    case ActionPackageType.act_var:
                        string varCode = "@var ";
                        varCode += String.Format("name={0} ", act.argsDict["opLeft"].valueExp);
                        var varOp = act.argsDict["op"].valueExp;
                        if (varOp == "=")
                        {
                            varCode += "dash=";
                        }
                        else
                        {
                            varCode += String.Format("dash={0}{1}", act.argsDict["opLeft"].valueExp, varOp.Substring(0, 1));
                        }
                        var varRights = act.argsDict["opRight"].valueExp.Split('#');
                        switch (varRights[0])
                        {
                            case "1":
                                varCode += String.Format("{0}", varRights[1]);
                                break;
                            case "2":
                                varCode += String.Format("\"{0}\"", varRights[1]);
                                break;
                            case "3":
                                varCode += String.Format("&{0}", varRights[1]);
                                break;
                            case "4":
                                varCode += String.Format("${0}", varRights[1]);
                                break;
                            case "5":
                                string[] raItems = varRights[1].Split(':');
                                varCode += String.Format("&{0}", (new Random()).Next(Convert.ToInt32(raItems[0]), Convert.ToInt32(raItems[1])));
                                break;
                            default:
                                varCode += String.Format("{0}", varRights[1]);
                                break;
                        }
                        codeBuilder.AppendLine(varCode);
                        break;
                    case ActionPackageType.act_if:
                        string ifCode = "@if cond=";
                        if (act.argsDict["expr"].valueExp == "")
                        {
                            string[] ifLeftItems = act.argsDict["op1"].valueExp.ToString().Split('#');
                            string[] ifRightItems = act.argsDict["op2"].valueExp.Split('#');
                            switch (ifLeftItems[0])
                            {
                                case "1":
                                    ifCode += String.Format("&{0}", ifLeftItems[1]);
                                    break;
                                case "2":
                                    ifCode += String.Format("${0}", ifLeftItems[1]);
                                    break;
                                case "3":
                                    if (ifRightItems[1] == "off")
                                    {
                                        ifCode += "!";
                                    }
                                    ifCode += "&switches{" + ifLeftItems[1] + "}";
                                    break;
                            }
                            if (ifLeftItems[0] != "3")
                            {
                                ifCode += String.Format("{0} ", act.argsDict["opr"].valueExp);
                            }
                            switch (ifRightItems[0])
                            {
                                case "1":
                                    ifCode += ifRightItems[1];
                                    break;
                                case "2":
                                    ifCode += String.Format("\"{0}\"", ifRightItems[1]);
                                    break;
                                case "3":
                                    ifCode += String.Format("&{0}", ifRightItems[1]);
                                    break;
                                case "4":
                                    ifCode += String.Format("${0} ", ifRightItems[1]);
                                    break;
                            }
                        }
                        else
                        {
                            ifCode += act.argsDict["expr"].valueExp;
                        }
                        codeBuilder.AppendLine(ifCode);
                        break;
                    case ActionPackageType.act_else:
                        codeBuilder.AppendLine("@else");
                        break;
                    case ActionPackageType.act_endif:
                        codeBuilder.AppendLine("@endif");
                        break;
                    case ActionPackageType.act_for:
                        codeBuilder.AppendLine("@for");
                        break;
                    case ActionPackageType.act_endfor:
                        codeBuilder.AppendLine("@endfor");
                        break;
                    case ActionPackageType.act_a:
                        string aCode = "@a ";
                        aCode += String.Format("name=\"{0}\" ", act.argsDict["name"].valueExp);
                        aCode += String.Format("face=\"{0}\" ", act.argsDict["face"].valueExp);
                        aCode += String.Format("vid=\"{0}\" ", act.argsDict["vid"].valueExp);
                        switch (act.argsDict["loc"].valueExp)
                        {
                            case "左":
                                aCode += "loc=\"left\"";
                                break;
                            case "左中":
                                aCode += "loc=\"midleft\"";
                                break;
                            case "右中":
                                aCode += "loc=\"midright\"";
                                break;
                            case "右":
                                aCode += "loc=\"right\"";
                                break;
                            default:
                                aCode += "loc=\"mid\"";
                                break;
                        }
                        codeBuilder.AppendLine(aCode);
                        break;
                    case ActionPackageType.act_dialog:
                        codeBuilder.AppendLine("[");
                        codeBuilder.AppendLine(act.argsDict["context"].valueExp);
                        codeBuilder.AppendLine("]");
                        break;
                    case ActionPackageType.script:
                        codeBuilder.AppendLine(act.argsDict["context"].valueExp);
                        break;
                    case ActionPackageType.notation:
                        codeBuilder.AppendLine(String.Format("# {0}", act.argsDict["context"].valueExp));
                        break;
                    case ActionPackageType.act_call:
                        string callCode = "@call ";
                        callCode += String.Format("name=\"{0}\"", act.argsDict["name"].valueExp);
                        callCode += "(";
                        string[] callsignItems = act.argsDict["sign"].valueExp.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        if (callsignItems.Length > 0)
                        {
                            string signBuilder = "";
                            foreach (var cskvp in callsignItems)
                            {
                                string[] cskvpItems = cskvp.Split(':');
                                signBuilder += "," + cskvpItems[1];
                            }
                            callCode += signBuilder.Substring(1);
                        }
                        callCode += ")";
                        codeBuilder.AppendLine(callCode);
                        break;
                    default:
                        string singleCode = String.Format("@{0} ", act.nodeType.ToString().Replace("act_", ""));
                        foreach (var arkv in act.argsDict)
                        {
                            switch (arkv.Key)
                            {
                                case "opacity":
                                case "xscale":
                                case "yscale":
                                    singleCode += String.Format("{0}=\"{1}\" ", arkv.Key, (Convert.ToDouble(arkv.Value.valueExp) / 100.0));
                                    break;
                                default:
                                    singleCode += String.Format("{0}=\"{1}\" ", arkv.Key, arkv.Value.valueExp);
                                    break;
                            }
                        }
                        codeBuilder.AppendLine(singleCode);
                        break;
                }
            }
            return codeBuilder.ToString();
        }

        /// <summary>
        /// 待处理队列
        /// </summary>
        private Queue<ScenePackage> splitQueue;

        /// <summary>
        /// 翻译线程数量
        /// </summary>
        private int threadNum = 4;

        /// <summary>
        /// 已完成线程数
        /// </summary>
        private int finishedThread = 0;

        /// <summary>
        /// 线程池
        /// </summary>
        private List<Thread> threadPool;

        /// <summary>
        /// 结果向量
        /// </summary>
        private List<KeyValuePair<string, string>> resList;

        /// <summary>
        /// 显示输出互斥量
        /// </summary>
        private Mutex consoleMutex = new Mutex();

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private CodeGenerator()
        {

        }

        /// <summary>
        /// 工厂方法：获得唯一实例
        /// </summary>
        /// <returns>代码生成器实例</returns>
        public static CodeGenerator GetInstance()
        {
            return CodeGenerator.instance;
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static readonly CodeGenerator instance = new CodeGenerator();
    }
}
