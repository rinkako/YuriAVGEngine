using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Yuri.YuriLauncher.Utils
{
    /// <summary>
    /// 设置包装类
    /// </summary>
    internal sealed class LauncherConfigPackage
    {
        /// <summary>
        /// 从外部配置文件读入配置信息
        /// </summary>
        public void ReadConfigData()
        {
            FileStream fs = new FileStream(IOUtils.ParseURItoURL("YuriConfig.dat"), FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            typeVec = new List<string>();
            while (sr.EndOfStream == false)
            {
                string aline = sr.ReadLine();
                var lineitems = aline?.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                if (lineitems?.Length == 3)
                {
                    configDict.Add(lineitems[0], lineitems[1]);
                    typeVec.Add(lineitems[2]);
                }
            }
            sr.Close();
            fs.Close();
        }

        private List<string> typeVec;

        /// <summary>
        /// 将配置信息写到稳定储存器
        /// </summary>
        public void WriteSteady()
        {
            try
            {
                FileStream fs = new FileStream(IOUtils.ParseURItoURL("YuriConfig.dat"), FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                int ctr = 0;
                foreach (var kvp in this.configDict)
                {
                    sw.WriteLine("{0}=>{1}=>{2}", kvp.Key, kvp.Value, this.typeVec[ctr++]);
                }
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"保存配置信息失败" + Environment.NewLine + ex);
            }
        }
        
        /// <summary>
        /// 注册一个设置
        /// </summary>
        /// <param name="propertyName">设置名</param>
        /// <param name="propertyValue">设置的值</param>
        public void Register(string propertyName, string propertyValue)
        {
            configDict[propertyName] = propertyValue;
        }
        
        /// <summary>
        /// 获取一个设置
        /// </summary>
        /// <param name="propertyName">设置名</param>
        /// <returns>设置的值</returns>
        public string Fetch(string propertyName)
        {
            return configDict.ContainsKey(propertyName) ? configDict[propertyName] : String.Empty;
        }

        /// <summary>
        /// 获取或设置一个设置项
        /// </summary>
        /// <param name="propertyName">设置名</param>
        /// <returns>设置的值</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.configDict[propertyName];
            }
            set
            {
                this.configDict[propertyName] = value;
            }
        }

        /// <summary>
        /// 设置信息的键值对字典
        /// </summary>
        private readonly Dictionary<string, string> configDict = new Dictionary<string, string>();
    }
}
