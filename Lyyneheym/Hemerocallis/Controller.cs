using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Yuri.Hemerocallis.Utils;
using Yuri.Hemerocallis.Forms;
using Yuri.Hemerocallis.Entity;

namespace Yuri.Hemerocallis
{
    /// <summary>
    /// 控制器类
    /// </summary>
    internal sealed class Controller
    {
        /// <summary>
        /// 获取或设置程序的配置信息
        /// </summary>
        public ConfigDescriptor ConfigDesc { get; set; }

        /// <summary>
        /// 获取或设置程序主窗体的引用
        /// </summary>
        public MainWindow mainWndRef { get; set; }

        /// <summary>
        /// 将设置写入稳定储存器
        /// </summary>
        public void WriteConfigToSteady()
        {
            IOUtil.Serialization(this.ConfigDesc, App.ParseURIToURL(App.AppDataDirectory, App.AppConfigFilename));
        }

        /// <summary>
        /// 将设置从稳定储存器读入内存
        /// </summary>
        public void ReadConfigToMemory()
        {
            try
            {
                this.ConfigDesc = (ConfigDescriptor)IOUtil.Unserialization(App.ParseURIToURL(App.AppDataDirectory, App.AppConfigFilename));
            }
            catch (Exception ex)
            {
                this.ResetConfig();
                this.WriteConfigToSteady();
            }
        }
        
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public void ResetConfig()
        {
            this.ConfigDesc = new ConfigDescriptor()
            {
                ZeOpacity = 0.3,
                IsEnableZe = true,
                LineHeight = 10,
                BgType = AppearanceBackgroundType.Default,
                BgTag = String.Empty,
                FontName = "微软雅黑",
                FontSize = 22,
                FontColor = "44,63,81"
            };
        }
        
        /// <summary>
        /// 工厂方法，获得类的唯一实例
        /// </summary>
        /// <returns>控制器类的唯一实例</returns>
        public static Controller GetInstance()
        {
            return Controller.synObject != null ? Controller.synObject : Controller.synObject = new Controller();
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private Controller()
        {
            this.ResetConfig();
        }

        /// <summary>
        /// 唯一实例对象
        /// </summary>
        private static Controller synObject;
    }
}
