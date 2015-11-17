using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LyyneheymCore.SlyviaPile;

namespace LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 资源管理器类：负责维护游戏的资源
    /// 她是一个单例类，只有唯一实例
    /// </summary>
    public class ResourceManager
    {














        public BitmapImage getBackgroundImage(string sourceName)
        {
            if (this.resourceTable.ContainsKey(Consta.DevURI_PA_BACKGROUND) &&
                this.resourceTable[Consta.DevURI_PA_BACKGROUND].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[Consta.DevURI_PA_BACKGROUND][sourceName];
                byte[] ob = PackageUtils.getObjectBytes(IOUtils.parseURItoURL("\\" + Consta.PackURI_PA_BACKGROUND + Consta.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                MemoryStream ms = new MemoryStream(ob);
                BitmapImage bpi = new BitmapImage();
                bpi.BeginInit();
                bpi.StreamSource = ms;
                bpi.EndInit();
                return bpi;
            }
            else
            {
                throw new Exception("缺失资源文件：" + sourceName);
            }
        }

        



        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>资源管理器的唯一实例</returns>
        public static ResourceManager getInstance()
        {
            return null == synObject ? synObject = new ResourceManager() : synObject;
        }

        /// <summary>
        /// 私有的构造器
        /// </summary>
        private ResourceManager()
        {
            resourceTable = new Dictionary<string, Dictionary<string, KeyValuePair<long, long>>>();
        }

        // 唯一实例量
        private static ResourceManager synObject = null;
        // 资源字典
        public Dictionary<string, Dictionary<string, KeyValuePair<long, long>>> resourceTable = null;
    }
}
