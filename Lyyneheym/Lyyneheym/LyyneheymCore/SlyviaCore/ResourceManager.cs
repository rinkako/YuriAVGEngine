using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Lyyneheym.LyyneheymCore.Utils;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// <para>资源管理器类：负责维护游戏的资源</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    public class ResourceManager
    {

        /// <summary>
        /// 获得一张指定背景图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetBackgroundImage(string sourceName)
        {
            MySprite sprite = new MySprite();
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(GlobalDataContainer.DevURI_PA_BACKGROUND) &&
                this.resourceTable[GlobalDataContainer.DevURI_PA_BACKGROUND].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[GlobalDataContainer.DevURI_PA_BACKGROUND][sourceName];
                byte[] ob = PackageUtils.getObjectBytes(IOUtils.ParseURItoURL(GlobalDataContainer.PackURI_PA_BACKGROUND + GlobalDataContainer.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(ms);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_PICTUREASSETS, GlobalDataContainer.DevURI_PA_BACKGROUND, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    Uri bg = new Uri(furi, UriKind.RelativeOrAbsolute);
                    sprite.Init(bg);
                }
                else
                {
                    throw new Exception("文件不存在：" + sourceName);
                }
            }
            return sprite;
        }

        /// <summary>
        /// 获得一张指定立绘图的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetCharacterStandImage(string sourceName)
        {
            MySprite sprite = new MySprite();
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(GlobalDataContainer.DevURI_PA_CHARASTAND) &&
                this.resourceTable[GlobalDataContainer.DevURI_PA_CHARASTAND].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[GlobalDataContainer.DevURI_PA_CHARASTAND][sourceName];
                byte[] ob = PackageUtils.getObjectBytes(IOUtils.ParseURItoURL(GlobalDataContainer.PackURI_PA_CHARASTAND + GlobalDataContainer.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(ms);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_PICTUREASSETS, GlobalDataContainer.DevURI_PA_CHARASTAND, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    Uri bg = new Uri(furi, UriKind.RelativeOrAbsolute);
                    sprite.Init(bg);
                }
                else
                {
                    throw new Exception("文件不存在：" + sourceName);
                }
            }
            return sprite;
        }

        /// <summary>
        /// 获得一张指定图片的精灵
        /// </summary>
        /// <param name="sourceName">资源名称</param>
        /// <returns>该资源的精灵</returns>
        public MySprite GetPictureSprite(string sourceName)
        {
            MySprite sprite = new MySprite();
            // 总是先查看是否有为封包的数据
            if (this.resourceTable.ContainsKey(GlobalDataContainer.DevURI_PA_PICTURES) &&
                this.resourceTable[GlobalDataContainer.DevURI_PA_PICTURES].ContainsKey(sourceName))
            {
                KeyValuePair<long, long> sourceLocation = this.resourceTable[GlobalDataContainer.DevURI_PA_PICTURES][sourceName];
                byte[] ob = PackageUtils.getObjectBytes(IOUtils.ParseURItoURL(GlobalDataContainer.PackURI_PA_PICTURES + GlobalDataContainer.PackPostfix),
                    sourceName, sourceLocation.Key, sourceLocation.Value);
                MemoryStream ms = new MemoryStream(ob);
                sprite.Init(ms);
            }
            // 没有封包数据再搜索开发目录
            else
            {
                string furi = IOUtils.JoinPath(GlobalDataContainer.DevURI_RT_PICTUREASSETS, GlobalDataContainer.DevURI_PA_PICTURES, sourceName);
                if (File.Exists(IOUtils.ParseURItoURL(furi)))
                {
                    Uri bg = new Uri(furi, UriKind.RelativeOrAbsolute);
                    sprite.Init(bg);
                }
                else
                {
                    throw new Exception("文件不存在：" + sourceName);
                }
            }
            return sprite;
        }











        

        

        /// <summary>
        /// 初始化资源字典
        /// </summary>
        /// <returns>操作成功与否</returns>
        public bool initDictionary()
        {
            return true;
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
