using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Yuri.Utils;
using Yuri.PlatformCore;

namespace Yuri.ILPackage
{
    /// <summary>
    /// <para>中间语言转换类：将SIL语言转换为场景动作序列</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal class ILConvertor
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
            var resList = new List<Scene>();
            foreach (var sapKvp in this.ilPackageContainer)
            {
                string sceneName = sapKvp.Key;
                var sapPool = sapKvp.Value;
                var saHeaderList = new List<SceneAction>();
                var yuriDict = new Dictionary<string, SceneAction>();
                var labelDictList = new List<Dictionary<string, SceneAction>>();
                foreach (var SAPPair in sapPool)
                {
                    var labelDict = new Dictionary<string, SceneAction>();
                    string nodename = SAPPair.Key;
                    SceneActionPackage sap = SAPPair.Value;
                    // 不脏的项目才入队展开
                    if (sap.dirtyBit == false)
                    {
                        saHeaderList.Add(this.iResContainer[sceneName][nodename]);
                        var openSet = new Queue<string>();
                        openSet.Enqueue(nodename);
                        // 广度优先遍历
                        while (openSet.Count != 0)
                        {
                            // 标记脏位
                            SceneActionPackage currentSAP = this.ilPackageContainer[sceneName][openSet.Dequeue()];
                            currentSAP.dirtyBit = true;
                            // 处理label字典
                            if (currentSAP.aType == SActionType.act_label)
                            {
                                labelDict[currentSAP.aTag] = this.iResContainer[sceneName][currentSAP.saNodeName];
                            }
                            // 处理next
                            if (!string.IsNullOrEmpty(currentSAP.next))
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].Next =
                                    this.iResContainer[sceneName][currentSAP.next];
                            }
                            // 处理trueRouting
                            if (currentSAP.trueRouting.Count > 0)
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].TrueRouting = new List<SceneAction>();
                                foreach (string trueSaName in currentSAP.trueRouting)
                                {
                                    this.iResContainer[sceneName][currentSAP.saNodeName].TrueRouting.Add(
                                        this.iResContainer[sceneName][trueSaName]);
                                    openSet.Enqueue(trueSaName);
                                }
                            }
                            // 处理falseRouting
                            if (currentSAP.falseRouting.Count > 0)
                            {
                                this.iResContainer[sceneName][currentSAP.saNodeName].FalseRouting = new List<SceneAction>();
                                foreach (string falseSaName in currentSAP.falseRouting)
                                {
                                    this.iResContainer[sceneName][currentSAP.saNodeName].FalseRouting.Add(
                                        this.iResContainer[sceneName][falseSaName]);
                                    openSet.Enqueue(falseSaName);
                                }
                            }
                            // 记录到Yuri向量
                            yuriDict.Add(currentSAP.saNodeName, this.iResContainer[sceneName][currentSAP.saNodeName]);
                        }
                        // 处理标签字典
                        labelDictList.Add(labelDict);
                    }
                }
                CommonUtils.ConsoleLine(String.Format("Finished SAP Relation Recovery: {0}", sceneName), "YuriIL Convertor", OutputStyle.Normal);
                Scene parseScene = null;
                if (saHeaderList.Count > 0)
                {
                    SceneAction mainSa = saHeaderList[0];
                    List<SceneFunction> funcVec = new List<SceneFunction>();
                    for (int fc = 1; fc < saHeaderList.Count; fc++)
                    {
                        SceneAction fsa = saHeaderList[fc];
                        var rsf = this.ParseSaToSF(fsa, sceneName);
                        rsf.LabelDictionary = labelDictList[fc];
                        funcVec.Add(rsf);
                    }
                    parseScene = new Scene(sceneName, mainSa, funcVec, labelDictList[0])
                    {
                        YuriDict = yuriDict
                    };
                }
                resList.Add(parseScene);
                CommonUtils.ConsoleLine(String.Format("Finished SAP Function Recovery: {0}", sceneName), "YuriIL Convertor", OutputStyle.Normal);
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
            string signature = funcSa.ArgsDict["sign"];
            var signItem = signature.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var funcParas = new List<string>();
            // 如果没有参数就跳过参数遍历
            if (signItem.Length > 1)
            {
                var varItem = signItem[1].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                funcParas.AddRange(varItem);
            }
            return new SceneFunction(signItem[0].Trim(), sceneName, funcSa) { Param = funcParas };
        }

        /// <summary>
        /// 将IL解析为SAP
        /// </summary>
        private void ProcessIL()
        {
            this.ilPackageContainer = new Dictionary<string, Dictionary<string, SceneActionPackage>>();
            this.iResContainer = new Dictionary<string, Dictionary<string, SceneAction>>();
            string currentSceneKey = String.Empty;
            foreach (string lineitem in this.splitContianer)
            {
                // 处理头部
                if (lineitem.StartsWith(">>>"))
                {
                    string mycommand = lineitem.Substring(3).Replace("\r\n", String.Empty);
                    // EOF标记
                    if (mycommand == "YuriEOF")
                    {
                        break;
                    }
                    // 场景标记
                    if (mycommand.StartsWith("YuriIL?"))
                    {
                        var commandItem = mycommand.Split('?');
                        currentSceneKey = commandItem[1];
                        this.ilPackageContainer.Add(currentSceneKey, new Dictionary<string,SceneActionPackage>());
                        this.iResContainer.Add(currentSceneKey, new Dictionary<string,SceneAction>());
                    }
                    else if (mycommand.StartsWith("YuriAEIL"))
                    {
                        var commandItem = mycommand.Split('?');
                        //GlobalDataContainer.GAME_PROJECT_NAME = commandItem[1];
                        //GlobalDataContainer.GAME_TITLE_NAME = commandItem[1];
                    }
                }
                else
                {
                    // sap
                    SceneActionPackage sap = this.ParseSceneActionPackage(lineitem);
                    this.ilPackageContainer[currentSceneKey].Add(sap.saNodeName, sap);
                    // sa
                    SceneAction sa = new SceneAction(sap);
                    this.iResContainer[currentSceneKey].Add(sa.NodeName, sa);
                }
            }
            CommonUtils.ConsoleLine("Finished Convert IL to SAP", "YuriIL Convertor", OutputStyle.Normal);
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
                    CommonUtils.ConsoleLine(String.Format("Ignored file: {0}", finfo.FullName), "YuriIL Convertor", OutputStyle.Warning);
                    continue;
                }
                // 分割文件为行
                CommonUtils.ConsoleLine(String.Format("Spliting file: {0}", finfo.FullName), "YuriIL Convertor", OutputStyle.Normal);
                var fs = new FileStream(finfo.FullName, FileMode.Open);
                var sr = new StreamReader(fs);
                // 跳过头部
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string body;
                    if ((body = sr.ReadLine()) != ">>>YuriEOF" && body != String.Empty)
                    {
                        var deb = YuriEncryptor.DecryptString(body, GlobalConfigContext.GAME_KEY);
                        this.splitContianer.Add(deb);
                    }
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
            var lineitem = oneline.Split('^');
            if (lineitem.Length == IL_LINEITEM_NUM)
            {
                sa = new SceneActionPackage
                {
                    saNodeName = lineitem[0],
                    aType = (SActionType) Enum.Parse(typeof(SActionType), lineitem[0].Split('@')[1], false),
                    argsDict = this.DispatchArgs(lineitem[1]),
                    condPolish = this.DecodeString(lineitem[2]),
                    next = lineitem[3],
                    trueRouting = this.DispatchRouting(lineitem[4]),
                    falseRouting = this.DispatchRouting(lineitem[5]),
                    isBelongFunc = lineitem[6] == "1",
                    funcName = lineitem[7],
                    aTag = this.DecodeString(lineitem[8])
                };
            }
            else
            {
                CommonUtils.ConsoleLine("IL已损坏", "ILConvertor", OutputStyle.Error);
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
            if (string.IsNullOrEmpty(origin))
            {
                return String.Empty;
            }
            var br = new byte[(int)(origin.Length / 3)];
            string rawSb = String.Empty;
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
                    rawSb = String.Empty;
                }
                rawSb += origin[i];
            }
            return isUTF8 ? Encoding.UTF8.GetString(br) : Encoding.Unicode.GetString(br);
        }

        /// <summary>
        /// 解析参数对
        /// </summary>
        /// <param name="argstr">参数对字符串</param>
        /// <returns>参数字典</returns>
        private Dictionary<string, string> DispatchArgs(string argstr)
        {
            var argItem = argstr.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            return argItem.Select(argpair => argpair.Split(new char[] {'@'}, StringSplitOptions.None)).
                ToDictionary(argkvp => argkvp[0], argkvp => this.DecodeString(argkvp[1]));
        }

        /// <summary>
        /// 解析路径向量
        /// </summary>
        /// <param name="routingstr">路径字符串</param>
        /// <returns>路径向量</returns>
        private List<string> DispatchRouting(string routingstr)
        {
            return routingstr.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>SIL语言解释器</returns>
        public static ILConvertor GetInstance()
        {
            return ILConvertor.syncObject ?? (ILConvertor.syncObject = new ILConvertor());
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
        private static ILConvertor syncObject = null;

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
        public string sceneDirectory = String.Empty;

        /// <summary>
        /// IL分割行的容器
        /// </summary>
        private List<string> splitContianer = null;

        /// <summary>
        /// IL行的split数量
        /// </summary>
        private const int IL_LINEITEM_NUM = 9;
    }
}
