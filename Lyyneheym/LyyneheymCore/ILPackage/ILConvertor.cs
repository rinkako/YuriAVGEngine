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
        /// 进行编译
        /// </summary>
        /// <param name="dir">剧本目录（以后要去掉）</param>
        /// <param name="threadNum">进程数</param>
        public void Dash(string dir, int threadNum = 1)
        {
            this.sceneDirectory = dir;
            this.LoadAndSplit();
            this.ProcessIL();

        }


        private void ProcessIL()
        {
            this.ilPackageContainer = new Dictionary<string, List<SceneActionPackage>>();
            foreach (string lineitem in this.splitContianer)
            {
                // 处理头部
                if (lineitem.StartsWith(">>>"))
                {
                    string mycommand = lineitem.Substring(3);
                    // EOF标记
                    if (mycommand == "SlyviaEOF")
                    {
                        break;
                    }
                    // 场景标记
                    if (mycommand.StartsWith("SlyviaIL?"))
                    {
                        string[] commandItem = mycommand.Split('?');
                        this.currentSceneKey = commandItem[1];
                        this.ilPackageContainer.Add(this.currentSceneKey, new List<SceneActionPackage>());
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
                    this.ilPackageContainer[this.currentSceneKey].Add(this.ParseSceneActionPackage(lineitem));
                }
            }
        }
        
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
                    this.splitContianer.Add(sr.ReadLine() + Environment.NewLine);
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
            string[] lineitem = oneline.Split(',');
            if (lineitem.Length == this.IL_LINEITEM_NUM)
            {
                sa = new SceneActionPackage();
                sa.saNodeName = lineitem[0];
                sa.aType = (SActionType)Enum.Parse(typeof(SActionType), lineitem[0].Split('@')[1], false);
                sa.argsDict = this.DispatchArgs(lineitem[1]);
                sa.condPolish = lineitem[2];
                sa.next = lineitem[3];
                sa.trueRouting = this.DispatchRouting(lineitem[4]);
                sa.falseRouting = this.DispatchRouting(lineitem[5]);
                sa.isBelongFunc = lineitem[6] == "1";
                sa.funcName = lineitem[7];
                sa.aTag = lineitem[8].Replace(@"\$", Environment.NewLine).Replace(@"\,", @",").Replace(@"\\", @"\");
            }
            else
            {
                throw new Exception("IL损坏");
            }
            return sa;
        }

        /// <summary>
        /// 解析参数对
        /// </summary>
        /// <param name="argstr">参数对字符串</param>
        /// <returns>参数字典</returns>
        private Dictionary<string, string> DispatchArgs(string argstr)
        {
            Dictionary<string, string> resDict = new Dictionary<string, string>();
            string[] argItem = argstr.Split(new string[] { ":#:" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string argpair in argItem)
            {
                string[] argkvp = argstr.Split(new string[] { ":@:" }, StringSplitOptions.None);
                resDict.Add(argkvp[0], argkvp[1]);
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

        private Dictionary<string, List<SceneActionPackage>> ilPackageContainer;
        private string currentSceneKey = "";

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
