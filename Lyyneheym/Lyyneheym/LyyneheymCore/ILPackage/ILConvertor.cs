using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Lyyneheym.LyyneheymCore.SlyviaCore;

namespace Lyyneheym.LyyneheymCore.ILPackage
{
    /// <summary>
    /// <para>中间语言转换类：将SIL语言转换为场景动作序列</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class ILConvertor
    {
        /// <summary>
        /// 进行编译，返回IL对应的场景实例向量
        /// </summary>
        /// <param name="dir">剧本目录（以后要去掉）</param>
        /// <returns>场景实例向量</returns>
        public List<Scene> Dash(string dir)
        {
            this.sceneDirectory = dir;
            this.LoadAndSplit();
            this.ProcessIL();
            return this.ProcessSARelation();
        }

        /// <summary>
        /// 处理动作项目为动作序列，封装成场景实例
        /// </summary>
        private List<Scene> ProcessSARelation()
        {
            List<Scene> resList = new List<Scene>();
            foreach (KeyValuePair<string, Dictionary<string, SceneActionPackage>> sapKvp in this.ilPackageContainer)
            {
                string sceneName = sapKvp.Key;
                Dictionary<string, SceneActionPackage> sapPool = sapKvp.Value;
                List<SceneAction> saHeaderList = new List<SceneAction>();
                foreach (KeyValuePair<string, SceneActionPackage> SAPPair in sapPool)
                {
                    string nodename = SAPPair.Key;
                    SceneActionPackage sap = SAPPair.Value;
                    // 不脏的项目才入队展开
                    if (sap.dirtyBit == false)
                    {
                        saHeaderList.Add(this.iResContainer[sceneName][nodename]);
                        Queue<string> openSet = new Queue<string>();
                        openSet.Enqueue(nodename);
                        // 广度优先遍历
                        while (openSet.Count != 0)
                        {
                            // 标记脏位
                            SceneActionPackage currentSAP = this.ilPackageContainer[sceneName][openSet.Dequeue()];
                            currentSAP.dirtyBit = true;
                            // 处理next
                            if (currentSAP.next != "" && currentSAP.next != null)
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].next =
                                    this.iResContainer[sceneName][currentSAP.next];
                            }
                            // 处理trueRouting
                            if (currentSAP.trueRouting.Count > 0)
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].trueRouting = new List<SceneAction>();
                                foreach (string trueSaName in currentSAP.trueRouting)
                                {
                                    this.iResContainer[sceneName][currentSAP.saNodeName].trueRouting.Add(
                                        this.iResContainer[sceneName][trueSaName]);
                                    openSet.Enqueue(trueSaName);
                                }
                            }
                            // 处理falseRouting
                            if (currentSAP.falseRouting.Count > 0)
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].falseRouting = new List<SceneAction>();
                                foreach (string falseSaName in currentSAP.falseRouting)
                                {
                                    this.iResContainer[sceneName][currentSAP.saNodeName].falseRouting.Add(
                                        this.iResContainer[sceneName][falseSaName]);
                                    openSet.Enqueue(falseSaName);
                                }
                            }
                        }
                    }
                }
                Scene parseScene = null;
                if (saHeaderList.Count > 0)
                {
                    SceneAction mainSa = saHeaderList[0];
                    List<SceneFunction> funcVec = new List<SceneFunction>();
                    for (int fc = 1; fc < saHeaderList.Count; fc++)
                    {
                        SceneAction fsa = saHeaderList[fc];
                        funcVec.Add(this.ParseSaToSF(fsa, sceneName));
                    }
                    parseScene = new Scene(sceneName, mainSa, funcVec);
                }
                resList.Add(parseScene);
            }
            return resList;
        }

        /// <summary>
        /// 将动作序列绑定到一个新的场景函数
        /// </summary>
        /// <param name="funcSa">动作序列</param>
        /// <param name="sceneName">场景名称</param>
        /// <returns>场景函数</returns>
        private SceneFunction ParseSaToSF(SceneAction funcSa, string sceneName)
        {
            // 获得函数签名
            string signature = funcSa.argsDict["sign"];
            string[] signItem = signature.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> funcParas = new List<string>();
            // 如果没有参数就跳过参数遍历
            if (signItem.Length > 1)
            {
                string[] varItem = signItem[1].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ivar in varItem)
                {
                    funcParas.Add(ivar);
                }
            }
            SceneFunction nsf = new SceneFunction(signItem[0].Trim(), sceneName, funcSa);
            nsf.param = funcParas;
            return nsf;
        }

        /// <summary>
        /// 将IL解析为SAP
        /// </summary>
        private void ProcessIL()
        {
            this.ilPackageContainer = new Dictionary<string, Dictionary<string, SceneActionPackage>>();
            this.iResContainer = new Dictionary<string, Dictionary<string, SceneAction>>();
            string currentSceneKey = "";
            foreach (string lineitem in this.splitContianer)
            {
                // 处理头部
                if (lineitem.StartsWith(">>>"))
                {
                    string mycommand = lineitem.Substring(3).Replace("\r\n", "");
                    // EOF标记
                    if (mycommand == "SlyviaEOF")
                    {
                        break;
                    }
                    // 场景标记
                    if (mycommand.StartsWith("SlyviaIL?"))
                    {
                        string[] commandItem = mycommand.Split('?');
                        currentSceneKey = commandItem[1];
                        this.ilPackageContainer.Add(currentSceneKey, new Dictionary<string,SceneActionPackage>());
                        this.iResContainer.Add(currentSceneKey, new Dictionary<string,SceneAction>());
                    }
                    else if (mycommand.StartsWith("SlyviaAEIL"))
                    {
                        string[] commandItem = mycommand.Split('?');
                        GlobalDataContainer.GAME_PROJECT_NAME = commandItem[1];
                        GlobalDataContainer.GAME_TITLE_NAME = commandItem[1];
                    }
                }
                else
                {
                    // sap
                    SceneActionPackage sap = this.ParseSceneActionPackage(lineitem);
                    this.ilPackageContainer[currentSceneKey].Add(sap.saNodeName, sap);
                    // sa
                    SceneAction sa = new SceneAction(sap);
                    this.iResContainer[currentSceneKey].Add(sa.saNodeName, sa);
                }
            }
        }
        
        /// <summary>
        /// 把文件分割为行项目
        /// </summary>
        private void LoadAndSplit()
        {
            this.splitContianer = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(this.sceneDirectory);
            foreach (FileInfo finfo in dirInfo.GetFiles())
            {
                if (finfo.Extension != ".sil")
                {
                    Console.WriteLine("Ignored: " + finfo.Name);
                    continue;
                }
                // 分割文件为行
                FileStream fs = new FileStream(finfo.FullName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    this.splitContianer.Add(sr.ReadLine());
                }
                sr.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 把IL文件的一行解析为SceneActionPackage
        /// </summary>
        /// <param name="oneline">IL文件的行</param>
        /// <returns>动作序列包</returns>
        private SceneActionPackage ParseSceneActionPackage(string oneline)
        {
            SceneActionPackage sa = null;
            string[] lineitem = oneline.Split('^');
            if (lineitem.Length == this.IL_LINEITEM_NUM)
            {
                sa = new SceneActionPackage();
                sa.saNodeName = lineitem[0];
                sa.aType = (SActionType)Enum.Parse(typeof(SActionType), lineitem[0].Split('@')[1], false);
                sa.argsDict = this.DispatchArgs(lineitem[1]);
                sa.condPolish = this.DecodeString(lineitem[2]);
                sa.next = lineitem[3];
                sa.trueRouting = this.DispatchRouting(lineitem[4]);
                sa.falseRouting = this.DispatchRouting(lineitem[5]);
                sa.isBelongFunc = lineitem[6] == "1";
                sa.funcName = lineitem[7];
                sa.aTag = this.DecodeString(lineitem[8]);
            }
            else
            {
                throw new Exception("IL损坏");
            }
            return sa;
        }

        /// <summary>
        /// 把已编码字符串解码
        /// </summary>
        /// <param name="origin">编码完毕的字符串</param>
        /// <param name="isUTF8">标志位，true解码UTF-8，false解码Unicode</param>
        /// <returns>解码后的字符串</returns>
        private string DecodeString(string origin, bool isUTF8 = true)
        {
            if (origin == "" || origin == null) { return ""; }
            byte[] br = new byte[(int)(origin.Length / 3)];
            string rawSb = "";
            for (int i = 0; i < origin.Length + 1; i++)
            {
                // 如果是最后一次就要清空缓冲
                if (i == origin.Length)
                {
                    br[(int)(i / 3) - 1] = Convert.ToByte(rawSb);
                    break;
                }
                if (i % 3 == 0 && i != 0)
                {
                    br[(int)(i / 3) - 1] = Convert.ToByte(rawSb);
                    rawSb = "";
                }
                rawSb += origin[i];
            }
            if (isUTF8)
            {
                return Encoding.UTF8.GetString(br);
            }
            else
            {
                return Encoding.Unicode.GetString(br);
            }
        }

        /// <summary>
        /// 解析参数对
        /// </summary>
        /// <param name="argstr">参数对字符串</param>
        /// <returns>参数字典</returns>
        private Dictionary<string, string> DispatchArgs(string argstr)
        {
            Dictionary<string, string> resDict = new Dictionary<string, string>();
            string[] argItem = argstr.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string argpair in argItem)
            {
                string[] argkvp = argpair.Split(new char[] { '@' }, StringSplitOptions.None);
                resDict.Add(argkvp[0], this.DecodeString(argkvp[1]));
            }
            return resDict;
        }

        /// <summary>
        /// 解析路径向量
        /// </summary>
        /// <param name="routingstr">路径字符串</param>
        /// <returns>路径向量</returns>
        private List<string> DispatchRouting(string routingstr)
        {
            List<string> resList = new List<string>();
            string[] routeItem = routingstr.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in routeItem)
            {
                resList.Add(s);
            }
            return resList;
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>SIL语言解释器</returns>
        public static ILConvertor GetInstance()
        {
            return instance == null ? instance = new ILConvertor() : instance;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ILConvertor()
        {
            
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static ILConvertor instance = null;

        /// <summary>
        /// SAP容器
        /// </summary>
        private Dictionary<string, Dictionary<string, SceneActionPackage>> ilPackageContainer;

        /// <summary>
        /// SA动作序列容器
        /// </summary>
        private Dictionary<string, Dictionary<string, SceneAction>> iResContainer;

        /// <summary>
        /// 剧本文件的目录
        /// </summary>
        public string sceneDirectory = "";

        /// <summary>
        /// IL分割行的容器
        /// </summary>
        private List<string> splitContianer = null;

        /// <summary>
        /// IL行的split数量
        /// </summary>
        private readonly int IL_LINEITEM_NUM = 9;
    }
}
