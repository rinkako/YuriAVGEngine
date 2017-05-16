using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
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
        #region 公共
        /// <summary>
        /// 全部提交到稳定储存器
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool FullCommit()
        {
            try
            {
                foreach (var pKvp in this.mainWndRef.RTBPageCacheDict)
                {
                    var page = pKvp.Value;
                    TextRange st = new TextRange(page.RichTextBox_FlowDocument.ContentStart, page.RichTextBox_FlowDocument.ContentEnd);
                    MemoryStream metadata = new MemoryStream();
                    st.Save(metadata, System.Windows.DataFormats.XamlPackage);
                    var updateId = pKvp.Key.StartsWith("HBook#") ?
                        this.BookVector.Find(t => t.BookRef.Id == pKvp.Key).BookRef.HomePage.Id : pKvp.Key;
                    this.ArticleDict[updateId].DocumentMetadata = metadata;
                }
                foreach (var p in this.BookVector)
                {
                    IOUtil.Serialization(p.BookRef, App.ParseURIToURL(App.AppDataDirectory, $"{p.BookRef.Id}.{App.AppBookDataExtension}"));
                    p.DirtyBit = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"提交到磁盘时发生了错误，请手动备份数据" + Environment.NewLine +
                                @"Failed to commit data to steady memory, please backup your data" + Environment.NewLine +
                                ex);
            }
            return false;
        }

        /// <summary>
        /// 提交更改到稳定储存器
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool DirtyCommit()
        {
            try
            {
                foreach (var p in this.BookVector)
                {
                    if (p.DirtyBit)
                    {
                        IOUtil.Serialization(p.BookRef, App.ParseURIToURL(App.AppDataDirectory, $"{p.BookRef.Id}.{App.AppBookDataExtension}"));
                        p.DirtyBit = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"提交到磁盘时发生了错误，请手动备份数据" + Environment.NewLine +
                                @"Failed to commit data to steady memory, please backup your data" + Environment.NewLine +
                                ex);
            }
            return false;
        }

        /// <summary>
        /// 提交当前书籍到稳定储存器
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool PageCommit()
        {
            try
            {
                TextRange st = new TextRange(this.mainWndRef.CurrentActivePage.RichTextBox_FlowDocument.ContentStart,
                    this.mainWndRef.CurrentActivePage.RichTextBox_FlowDocument.ContentEnd);
                MemoryStream metadata = new MemoryStream();
                st.Save(metadata, System.Windows.DataFormats.XamlPackage);
                this.ArticleDict[this.mainWndRef.CurrentActivePage.ArticalRef.Id].DocumentMetadata = metadata;
                var curBB = this.BookVector.Find(t => t.BookRef.Id == this.mainWndRef.CurrentBookId);
                IOUtil.Serialization(curBB.BookRef, App.ParseURIToURL(App.AppDataDirectory, $"{curBB.BookRef.Id}.{App.AppBookDataExtension}"));
                curBB.DirtyBit = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"提交到磁盘时发生了错误，请手动备份数据" + Environment.NewLine +
                                @"Failed to commit data to steady memory, please backup your data" + Environment.NewLine +
                                ex);
            }
            return false;
        }
        #endregion

        #region 书籍操作
        /// <summary>
        /// 添加一本书籍
        /// </summary>
        /// <param name="bookName">书籍的名字</param>
        /// <returns>所添加的书籍的唯一标识符</returns>
        public string AddBook(string bookName)
        {
            // 记录时间戳
            var createTime = DateTime.Now;
            // 构造主页文章
            FlowDocument flowDoc = new FlowDocument();
            TextRange st = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            MemoryStream metadata = new MemoryStream();
            st.Save(metadata, System.Windows.DataFormats.XamlPackage);
            HArticle homePage = new HArticle()
            {
                Id = "HArticle#" + Guid.NewGuid(),
                ParentId = "HArticle#ROOT",
                Name = bookName,
                CreateTimeStamp = createTime,
                LastEditTimeStamp = createTime,
                DocumentMetadata = metadata
            };
            this.ArticleDict[homePage.Id] = homePage;
            // 构造书籍对象
            var bookId = "HBook#" + Guid.NewGuid();
            HBook hb = new HBook()
            {
                Id = bookId,
                Name = bookName,
                HomePage = homePage,
                CreateTimeStamp = createTime,
                LastEditTimeStamp = createTime,
                FileName = $"{bookId}.{App.AppBookDataExtension}"
            };
            this.BookVector.Add(new BookCacheDescriptor(hb, true));
            // 提交到稳定储存器
            this.FullCommit();
            return bookId;
        }
        
        /// <summary>
        /// 删除一本书籍
        /// </summary>
        /// <param name="bookId">书籍的唯一标识符</param>
        /// <returns>操作成功或否</returns>
        public bool DeleteBook(string bookId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重命名一本书籍
        /// </summary>
        /// <param name="bookId">书籍的唯一标识符</param>
        /// <param name="newBookName">新的书名</param>
        /// <returns>操作成功或否</returns>
        public bool RenameBook(string bookId, string newBookName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将一篇文章移动到一个新的书籍或文档的下属
        /// </summary>
        /// <param name="articalId">要移动的文章的唯一标识符</param>
        /// <param name="newParentId">要成为该文章的新下属文章或书籍的唯一标识符</param>
        /// <returns>操作成功或否</returns>
        public bool MoveArtical(string articalId, string newParentId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取出一篇文章的内容包装
        /// </summary>
        /// <param name="articalId">文章的唯一标识符</param>
        /// <param name="dataPackage">[out] 文章的数据包装内存流</param>
        /// <returns>操作成功或否</returns>
        public bool RetrieveArtical(string articalId, out MemoryStream dataPackage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存一篇文章的内容包装
        /// </summary>
        /// <param name="articalId">文章的唯一标识符</param>
        /// <param name="dataPackage">文章的数据包装内存流</param>
        /// <returns>操作成功或否</returns>
        public bool UpdateArtical(string articalId, MemoryStream dataPackage)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 便签操作
        /// <summary>
        /// 添加一个便签
        /// </summary>
        /// <param name="type">便签内容类型</param>
        /// <param name="payload">附加值</param>
        /// <returns>所添加的便签描述子的唯一标识符</returns>
        public string AddTip(TipType type, object payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除一个便签
        /// </summary>
        /// <param name="id">便签描述子的唯一标识符</param>
        /// <returns>操作成功或否</returns>
        public bool DeleteTip(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 隐藏所有便签
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool HideAllTips()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 显示所有隐藏便签
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool ShowAllTips()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重绘所有便签
        /// </summary>
        /// <returns>操作成功或否</returns>
        public bool ReDrawTips()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检索便签
        /// </summary>
        /// <param name="pred">筛选谓词</param>
        /// <param name="tipVec">检索结果向量</param>
        /// <returns>操作成功或否</returns>
        public bool RetrieveTip(Predicate<TipDescriptor> pred, out List<TipDescriptor> tipVec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检索一个便签
        /// </summary>
        /// <param name="id">便签的唯一标识符</param>
        /// <param name="descriptor">[out] 描述子对象</param>
        /// <returns>操作成功或否</returns>
        public bool RetrieveTip(string id, out TipDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 里程碑操作
        /// <summary>
        /// 添加一个里程碑
        /// </summary>
        /// <param name="type">里程碑类型</param>
        /// <param name="id">里程碑作用对象的唯一标识符</param>
        /// <param name="destination">目标字数</param>
        /// <param name="detail">里程碑备注</param>
        /// <param name="beginTime">里程碑开始时刻</param>
        /// <param name="endTime">里程碑结束时刻</param>
        /// <returns>所添加的里程碑的唯一标识符</returns>
        public string AddMilestone(MilestoneType type, string id, long destination, string detail, DateTime beginTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新一个里程碑
        /// </summary>
        /// <param name="milestoneId">里程碑的唯一标识符</param>
        /// <param name="destination">目标字数</param>
        /// <param name="detail">里程碑备注</param>
        /// <param name="beginTime">里程碑开始时刻</param>
        /// <param name="endTime">里程碑结束时刻</param>
        /// <param name="isFinished">是否已经完成</param>
        /// <param name="finishTime">完成时刻</param>
        /// <returns>操作成功或否</returns>
        public bool UpdateMilestone(string milestoneId, long destination, string detail, DateTime beginTime,
            DateTime endTime, bool isFinished, DateTime finishTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 完成一个里程碑
        /// </summary>
        /// <param name="milestoneId">里程碑的唯一标识符</param>
        /// <returns>操作成功或否</returns>
        public bool FinishMilestone(string milestoneId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除一个里程碑
        /// </summary>
        /// <param name="milestoneId">里程碑的唯一标识符</param>
        /// <returns>操作成功或否</returns>
        public bool DeleteMilestone(string milestoneId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检索里程碑
        /// </summary>
        /// <param name="pred">筛选谓词</param>
        /// <param name="msVec">[out] 结果向量</param>
        /// <returns>操作成功或否</returns>
        public bool RetrieveMilestone(Predicate<HMilestone> pred, out List<HMilestone> msVec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检索一个里程碑
        /// </summary>
        /// <param name="id">里程碑的唯一标识符</param>
        /// <param name="ms">[out] 里程碑对象</param>
        /// <returns>操作成功或否</returns>
        public bool RetrieveMilestone(string id, out HMilestone ms)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 运行时信息
        /// <summary>
        /// 获取书籍向量（对象，脏位）
        /// </summary>
        public List<BookCacheDescriptor> BookVector { get; private set; } = new List<BookCacheDescriptor>();

        /// <summary>
        /// 前端RTB对象的引用
        /// </summary>
        public List<KeyValuePair<string, RichTextBox>> ViewRTBList = new List<KeyValuePair<string, RichTextBox>>();

        /// <summary>
        /// 获取文章字典
        /// </summary>
        public Dictionary<string, HArticle> ArticleDict { get; private set; } = new Dictionary<string, HArticle>();

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
            catch
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
            return Controller.synObject;
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
        private static readonly Controller synObject = new Controller();
        #endregion
    }

    /// <summary>
    /// 书籍在内存中的脏块
    /// </summary>
    internal sealed class BookCacheDescriptor
    {
        /// <summary>
        /// 构造一个内存书籍描述脏块
        /// </summary>
        /// <param name="bref">书籍对象的引用</param>
        /// <param name="dirty">初始脏位</param>
        public BookCacheDescriptor(HBook bref, bool dirty)
        {
            this.BookRef = bref;
            this.DirtyBit = dirty;
        }

        /// <summary>
        /// 书籍对象的引用
        /// </summary>
        public HBook BookRef { get; set; }

        /// <summary>
        /// 脏位
        /// </summary>
        public bool DirtyBit { get; set; }
    }
}
