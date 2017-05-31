using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Yuri.YuriInterpreter.YuriILEnum;
using Yuri.Yuriri;

namespace Yuri.YuriInterpreter
{
    /// <summary>
    /// 翻译器类：控制编译工作
    /// </summary>
    public sealed class Interpreter
    {
        /// <summary>
        /// 启动解释器
        /// </summary>
        /// <param name="project">项目名称</param>
        /// <param name="scdir">项目剧本路径</param>
        /// <param name="key">密钥</param>
        /// <param name="encrypt">是否加密</param>
        public Interpreter(string project, string scdir, string key, bool encrypt = true)
        {
            this.projectName = project;
            this.sceneDirectory = scdir;
            if (key == String.Empty)
            {
                key = "yurayuri";
            }
            Pile.Encryptor = key;
            Pile.needEncryption = encrypt;
            this.parseTreeStringVec = new List<string>();
        }

        /// <summary>
        /// 进行编译
        /// </summary>
        /// <param name="itype">编译类型</param>
        /// <param name="threadN">进程数</param>
        public void Dash(InterpreterType itype, int threadN = 4)
        {
            this.compileType = itype;
            this.threadPool = new List<Thread>();
            this.LoadAndSplit();
        }

        /// <summary>
        /// 将IL储存为文件
        /// </summary>
        /// <param name="storeFile">含文件名的储存文件路径</param>
        public void GenerateIL(string storeFile)
        {
            try
            {
                FileStream fs = new FileStream(storeFile, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(">>>YuriAEIL?" + this.projectName);
                foreach (var ilp in this.ILVector)
                {
                    sw.WriteLine(ilp.Value);
                }
                sw.WriteLine(">>>YuriEOF");
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存文件出错");
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取整个项目IL的匹配树结构字符串
        /// </summary>
        /// <returns>代表匹配森林的字符串</returns>
        public string GetTreeGraphic()
        {
            return this.parseTreeStringVec.Aggregate(String.Empty, (cc, t) => cc + t + Environment.NewLine);
        }

        /// <summary>
        /// 读取文件并做行分割
        /// </summary>
        private void LoadAndSplit()
        {
            // 加载合法脚本文件到队列
            if (this.compileType == InterpreterType.DEBUG)
            {
                this.SceneVector = new List<KeyValuePair<string, Scene>>();
            }
            else
            {
                this.ILVector = new List<KeyValuePair<string, string>>();
            }
            this.splitQueue = new Queue<FileInfo>();
            DirectoryInfo dirInfo = null;
            try
            {
                dirInfo = new DirectoryInfo(this.sceneDirectory);
            }
            catch
            {
                throw new Exception("剧本文件夹加载错误");
            }
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                if (file.Extension == ".sls")
                {
                    splitQueue.Enqueue(file);
                }
                else
                {
                    Console.WriteLine("Interpreter Ignored:" + file.Name);
                }
            }
            // 开启处理线程
            this.finishedThread = 0;
            for (int t = 0; t < this.threadNum; t++)
            {
                this.threadPool.Add(new Thread(new ParameterizedThreadStart(this.SplitHandler)));
                this.threadPool[t].IsBackground = true;
                this.threadPool[t].Start(t);
            }
            // 等待线程回调
            while (this.finishedThread < this.threadNum) ;
        }

        /// <summary>
        /// 多线程切割剧本文件
        /// </summary>
        /// <param name="threadID">线程id</param>
        private void SplitHandler(object threadID)
        {
            int tid = (int)threadID;
            while (true)
            {
                FileInfo fi = null;
                lock (this.splitQueue)
                {
                    if (this.splitQueue.Count != 0)
                    {
                        fi = this.splitQueue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
                if (fi == null)
                {
                    continue;
                }
                lock (this.consoleMutex)
                {
                    Console.WriteLine("Spliting \"{0}\" At thread {1}", fi.Name, tid);
                }
                var resVec = new List<string>();
                try
                {
                    FileStream fs = new FileStream(fi.FullName, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        resVec.Add(sr.ReadLine() + Environment.NewLine);
                    }
                    sr.Close();
                    fs.Close();
                    lock (this.consoleMutex)
                    {
                        Console.WriteLine("Compiling \"{0}\" At thread {1}", fi.Name, tid);
                    }
                    if (this.compileType == InterpreterType.DEBUG)
                    {
                        Pile pile = new Pile();
                        var yuriResult = new KeyValuePair<string, Scene>(
                            fi.Name.Split('.')[0], (Scene)pile.StartDash(resVec, fi.Name.Split('.')[0], this.compileType));
                        lock (this.SceneVector)
                        {
                            this.parseTreeStringVec.Add(pile.GetParsedTree());
                            this.SceneVector.Add(yuriResult);
                        }
                    }
                    else
                    {
                        Pile pile = new Pile();
                        var yuriIL = new KeyValuePair<string, string>(
                            fi.Name.Split('.')[0], (string)pile.StartDash(resVec, fi.Name.Split('.')[0], this.compileType));
                        lock (this.ILVector)
                        {
                            this.parseTreeStringVec.Add(pile.GetParsedTree());
                            this.ILVector.Add(yuriIL);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
            // 递增完成信号量
            lock (consoleMutex)
            {
                this.finishedThread++;
                Console.WriteLine("Thread {0} is Finished", tid);
            }
        }

        /// <summary>
        /// 语法解析树字符串向量
        /// </summary>
        private List<string> parseTreeStringVec;

        /// <summary>
        /// 待处理文件队列
        /// </summary>
        private Queue<FileInfo> splitQueue;

        /// <summary>
        /// IL结果向量
        /// </summary>
        private List<KeyValuePair<string, string>> ILVector;

        /// <summary>
        /// Scene结果向量
        /// </summary>
        private List<KeyValuePair<string, Scene>> SceneVector;

        /// <summary>
        /// 线程池
        /// </summary>
        private List<Thread> threadPool;

        /// <summary>
        /// 显示输出互斥量
        /// </summary>
        private readonly Mutex consoleMutex = new Mutex();

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName;

        /// <summary>
        /// 剧本文件的目录
        /// </summary>
        public string sceneDirectory;

        /// <summary>
        /// 已完成线程数
        /// </summary>
        private int finishedThread = 0;

        /// <summary>
        /// 编译类型
        /// </summary>
        private InterpreterType compileType = InterpreterType.RELEASE_WITH_IL;

        /// <summary>
        /// 线程数量
        /// </summary>
        private int threadNum = 1;
    }
}
