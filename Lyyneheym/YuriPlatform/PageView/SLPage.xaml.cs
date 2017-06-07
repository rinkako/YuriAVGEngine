using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Yuri.PlatformCore;
using Yuri.PlatformCore.Graphic;
using Yuri.Utils;

namespace Yuri.PageView
{
    /// <summary>
    /// SLPage.xaml 的交互逻辑：存读档页面
    /// </summary>
    public partial class SLPage : Page
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="isSave">是否呼唤保存模式</param>
        public SLPage(bool isSave)
        {
            InitializeComponent();
            this.isSave = isSave;
            this.InitResource();
        }

        /// <summary>
        /// 加载画面资源
        /// </summary>
        private void InitResource()
        {
            try
            {
                // 背景图
                var backGroundSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Background.png", ResourceManager.FullImageRect);
                this.SL_MainGrid.Background = new ImageBrush(backGroundSprite.SpriteBitmapImage);
                // 返回按钮
                var backButtonSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Button_BACK.png", ResourceManager.FullImageRect);
                this.SL_Button_Back.Source = backButtonSprite.SpriteBitmapImage;
                // 删除按钮
                var deleteButtonSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Button_DELETE.png", ResourceManager.FullImageRect);
                this.SL_Button_Delete.Source = deleteButtonSprite.SpriteBitmapImage;
                if (this.isSave)
                {
                    // 保存按钮
                    var saveButtonSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Button_SAVE.png", ResourceManager.FullImageRect);
                    this.SL_Button_SorL.Source = saveButtonSprite.SpriteBitmapImage;
                    // 描述子底层框
                    var descriptorBoxSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Item_Descriptor.png", ResourceManager.FullImageRect);
                    this.SL_DescriptorBox.Source = descriptorBoxSprite.SpriteBitmapImage;
                    // 消去描述子的显示框，以填写框代替
                    this.SL_Descriptor_TextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // 读取按钮
                    var loadButtonSprite = ResourceManager.GetInstance().GetPicture("UI_SaveLoad_Button_LOAD.png", ResourceManager.FullImageRect);
                    this.SL_Button_SorL.Source = loadButtonSprite.SpriteBitmapImage;
                    // 消去描述子的填写框，以显示框代替
                    this.SL_Descriptor_TextBox.Visibility = Visibility.Collapsed;
                }
                // 放置按钮
                for (int i = 0; i < GlobalConfigContext.GAME_SAVE_MAX; i++)
                {
                    Button slotButton = new Button()
                    {
                        Name = String.Format("button_{0}", i + 1),
                        Content = String.Format(" --- 空档{0} --- ", i + 1),
                        Height = 40,
                        FontSize = 18,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        Foreground = Brushes.White
                    };
                    slotButton.MouseEnter += this.button_MouseEnter;
                    slotButtonList.Add(slotButton);
                    this.SL_FileslotStackPanel.Children.Add(slotButton);
                }
                // 加载存档资源
                this.ReLoadFileInfo();
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Failed to load SLPage Resource. " + ex, "SLPage", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// 加载保存文件的信息
        /// </summary>
        public void ReLoadFileInfo()
        {
            var saveDirPathStr = IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR);
            if (!Directory.Exists(saveDirPathStr))
            {
                Directory.CreateDirectory(saveDirPathStr);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(saveDirPathStr);
            this.saveList = new List<FileInfo>();
            for (int i = 0; i < GlobalConfigContext.GAME_SAVE_MAX; i++)
            {
                this.saveList.Add(null);
            }
            foreach (var fInfo in dirInfo.GetFiles())
            {
                if (fInfo.Name.StartsWith(GlobalConfigContext.GAME_SAVE_PREFIX + "-") &&
                    String.Compare(fInfo.Extension, GlobalConfigContext.GAME_SAVE_POSTFIX, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var timeItems = fInfo.Name.Split('-');
                    var pointedId = Convert.ToInt32(timeItems[1]) - 1;
                    this.saveList[pointedId] = fInfo;
                    this.slotButtonList[pointedId].Content = String.Format("存档{0}：{1}/{2} {3}:{4}",
                        pointedId + 1, timeItems[3], timeItems[4], timeItems[5], timeItems[6]);
                }
            }
        }
        
        /// <summary>
        /// 事件：存档文件位按钮鼠标进入，刷新右侧信息
        /// </summary>
        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                // 获取当前指向的slot
                int pointId = 0;
                if (sender is int)
                {
                    pointId = (int)sender;
                }
                else
                {
                    pointId = Convert.ToInt32(((Button)sender).Name.Split('_')[1]) - 1;
                }
                // 如果该存档位不是空存档
                if (saveList[pointId] != null)
                {
                    // 显示截图
                    var shotName = GlobalConfigContext.GAME_SAVE_DIR + @"\" + GlobalConfigContext.GAME_SAVE_SNAPSHOT_PREFIX +
                        saveList[pointId].Name.Substring(GlobalConfigContext.GAME_SAVE_PREFIX.Length).Replace(GlobalConfigContext.GAME_SAVE_POSTFIX,
                        GlobalConfigContext.GAME_SAVE_SNAPSHOT_POSTFIX);
                    var fullShotName = IOUtils.ParseURItoURL(shotName);
                    if (File.Exists(fullShotName))
                    {
                        var p = ResourceManager.GetInstance().GetSaveSnapshot(shotName);
                        this.SL_SnapshotImage.Source = p.SpriteBitmapImage;
                    }
                    else
                    {
                        this.SL_SnapshotImage.Source = null;
                    }
                    // 显示描述子
                    var descName = GlobalConfigContext.GAME_SAVE_DIR + @"\" + GlobalConfigContext.GAME_SAVE_DESCRIPTOR_PREFIX +
                        saveList[pointId].Name.Substring(GlobalConfigContext.GAME_SAVE_PREFIX.Length).Replace(GlobalConfigContext.GAME_SAVE_POSTFIX,
                        GlobalConfigContext.GAME_SAVE_DESCRIPTOR_POSTFIX);
                    var fullDescName = IOUtils.ParseURItoURL(descName);
                    var chapterStr = String.Empty;
                    if (File.Exists(fullDescName))
                    {
                        FileStream fs = new FileStream(fullDescName, FileMode.Open);
                        StreamReader sr = new StreamReader(fs);
                        chapterStr = sr.ReadLine();
                        if (this.isSave)
                        {
                            this.SL_Descriptor_TextBox.Text = sr.ReadToEnd();
                        }
                        else
                        {
                            this.SL_Descriptor_TextBlock.Text = sr.ReadToEnd();
                        }
                        sr.Close();
                        fs.Close();
                    }
                    else
                    {
                        if (this.isSave)
                        {
                            this.SL_Descriptor_TextBox.Text = String.Empty;
                        }
                        else
                        {
                            this.SL_Descriptor_TextBlock.Text = String.Empty;
                        }
                    }
                    // 显示时间戳
                    var timeItems = saveList[pointId].Name.Split('-');
                    var timeStr = String.Format("{0}-{1}-{2} {3}:{4}:{5}",
                        timeItems[2], timeItems[3], timeItems[4], timeItems[5], timeItems[6], timeItems[7]);
                    this.SL_TimeStampTextBlock.Text = String.Format("{0}{1}  Time:{2}",
                        chapterStr == String.Empty ? String.Empty : "Chapter:", chapterStr, timeStr);
                    this.SL_NameTextBlock.Text = String.Format("< 存档{0} >", pointId + 1);
                    this.SL_Button_SorL.Visibility = this.SL_Button_Delete.Visibility = 
                        this.SL_TimeStampTextBlock.Visibility = this.SL_DescriptorBox.Visibility = Visibility.Visible;
                    if (this.isSave)
                    {
                        this.SL_Descriptor_TextBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.SL_Descriptor_TextBlock.Visibility = Visibility.Visible;
                    }
                }
                // 该存档位是空存档
                else
                {
                    this.SL_Button_Delete.Visibility = Visibility.Hidden;
                    this.SL_NameTextBlock.Text = String.Format("< 空存档{0} >", pointId + 1);
                    this.SL_SnapshotImage.Source = null;
                    // 保存
                    if (this.isSave)
                    {
                        this.SL_Button_SorL.Visibility = this.SL_DescriptorBox.Visibility =
                            this.SL_Descriptor_TextBox.Visibility = Visibility.Visible;
                        this.SL_Descriptor_TextBox.Text = String.Empty;
                        this.SL_TimeStampTextBlock.Text = String.Empty;
                    }
                    // 读取
                    else
                    {
                        this.SL_Button_SorL.Visibility = this.SL_TimeStampTextBlock.Visibility =
                            this.SL_DescriptorBox.Visibility = this.SL_Descriptor_TextBlock.Visibility = 
                            this.SL_Descriptor_TextBox.Visibility = Visibility.Hidden;
                    }
                    
                }
                if (lastPointed != -1)
                {
                    this.slotButtonList[lastPointed].Background = Brushes.Transparent;
                }
                this.slotButtonList[pointId].Background = Brushes.LightBlue;
                lastPointed = pointId;
            }
            catch (Exception)
            {
                // empty
            }
        }
        
        /// <summary>
        /// 事件：辅助按钮鼠标进入
        /// </summary>
        private void SL_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Opacity = 1.0;
        }

        /// <summary>
        /// 事件：辅助按钮鼠标离开
        /// </summary>
        private void SL_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Opacity = 0.7;
        }

        /// <summary>
        /// 按钮：删除
        /// </summary>
        private void SL_Button_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show(String.Format("真的要删除存档[{0}]吗？{1}这个行为不能回滚。",
                this.slotButtonList[lastPointed].Content.ToString(), Environment.NewLine),
                "确认", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No))
            {
                try
                {
                    // 消去前端相关的信息
                    this.SL_SnapshotImage.Source = null;
                    // 处理存档文件
                    File.Delete(this.saveList[this.lastPointed].FullName);
                    // 处理描述子
                    var descName = GlobalConfigContext.GAME_SAVE_DIR + @"\" + GlobalConfigContext.GAME_SAVE_DESCRIPTOR_PREFIX +
                        saveList[this.lastPointed].Name.Substring(GlobalConfigContext.GAME_SAVE_PREFIX.Length).Replace(GlobalConfigContext.GAME_SAVE_POSTFIX,
                        GlobalConfigContext.GAME_SAVE_DESCRIPTOR_POSTFIX);
                    File.Delete(IOUtils.ParseURItoURL(descName));
                    // 处理截图
                    //var shotName = GlobalDataContainer.GAME_SAVE_DIR + @"\" + GlobalDataContainer.GAME_SAVE_SNAPSHOT_PREFIX +
                    //    saveList[this.lastPointed].Name.Substring(GlobalDataContainer.GAME_SAVE_PREFIX.Length).Replace(GlobalDataContainer.GAME_SAVE_POSTFIX,
                    //    GlobalDataContainer.GAME_SAVE_SNAPSHOT_POSTFIX);
                    //File.Delete(Utils.IOUtils.ParseURItoURL(shotName));
                    MessageBox.Show("记录已抹去");
                    // 强制前端的刷新
                    this.saveList[this.lastPointed] = null;
                    this.slotButtonList[this.lastPointed].Content = String.Format(" --- 空档{0} --- ", this.lastPointed + 1);
                    this.button_MouseEnter(this.lastPointed, null);
                }
                catch (Exception ex)
                {
                    var exStr = "在删除文件时发生了异常：" + ex.ToString();
                    LogUtils.LogLine(exStr, "SLPage", LogLevel.Warning);
                    MessageBox.Show(exStr);
                }
            }
        }

        /// <summary>
        /// 按钮：返回
        /// </summary>
        private void SL_Button_Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewPageManager.GoBack();
        }

        /// <summary>
        /// 按钮：保存或读取存档文件
        /// </summary>
        /// <remarks>
        ///   存档的文件名形如：
        ///     save-1-2017-03-17-21-38-44-484.dat
        ///   其中save是存档前缀，1是存档位编码（对应存档位0），后面是时间戳和文件后缀
        ///   这个存档将绑定截图文件名形如：
        ///     ssnap-1-2017-03-17-21-38-44-484.jpg
        ///   如果存档存在描述子，那还将生成描述子文件，名字形如：
        ///     sdesc-1-2017-03-17-21-38-44-484.md
        ///   描述子文件以UTF-8编码，储存单行内容
        /// </remarks>
        private void SL_Button_SorL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 存档
            if (this.isSave)
            {
                // 是否覆盖
                if (this.saveList[this.lastPointed] != null)
                {
                    if (MessageBox.Show(String.Format("要覆盖这个存档吗？{0}该动作不能回滚。", Environment.NewLine), 
                        "提示", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        return;
                    }
                    // 移除原来的文件
                    try
                    {
                        // 处理过时存档
                        File.Delete(this.saveList[this.lastPointed].FullName);
                        // 处理过时描述子
                        var descName = GlobalConfigContext.GAME_SAVE_DIR + @"\" + GlobalConfigContext.GAME_SAVE_DESCRIPTOR_PREFIX +
                            saveList[this.lastPointed].Name.Substring(GlobalConfigContext.GAME_SAVE_PREFIX.Length).Replace(GlobalConfigContext.GAME_SAVE_POSTFIX,
                            GlobalConfigContext.GAME_SAVE_DESCRIPTOR_POSTFIX);
                        File.Delete(IOUtils.ParseURItoURL(descName));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogLine("覆盖存档时，在移除过时文件过程出现异常" + Environment.NewLine + ex, "SLPage", LogLevel.Error);
                    }
                }
                // 获得存档时间戳 
                var timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                var timeItems = timeStamp.Split('-');
                var saveIdentity = String.Format("-{0}-{1}", this.lastPointed + 1, timeStamp);
                // 不可容忍的错误段
                try
                {
                    // 构造存档文件名（不需要后缀，UR的Save方法已经封装了）
                    var fname = String.Format("{0}{1}", GlobalConfigContext.GAME_SAVE_PREFIX, saveIdentity);
                    // 保存游戏信息
                    this.core.GetMainRender().ActualSave(fname);
                    // 更新页面的信息
                    this.saveList[this.lastPointed] = new FileInfo(IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + @"\" + fname + GlobalConfigContext.GAME_SAVE_POSTFIX));
                    this.slotButtonList[this.lastPointed].Content = String.Format("存档{0}：{1}/{2} {3}:{4}",
                        this.lastPointed + 1, timeItems[1], timeItems[2], timeItems[3], timeItems[4]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存文件失败！在SLPage(in CLR)触发了：" + Environment.NewLine + ex);
                    return;
                }
                // 可容忍的错误段
                try
                {
                    // 保存截图文件
                    if (File.Exists(IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg")))
                    {
                        File.Move(IOUtils.ParseURItoURL(GlobalConfigContext.GAME_SAVE_DIR + "\\tempSnapshot.jpg"),
                            IOUtils.ParseURItoURL(String.Format("{0}\\{1}{2}{3}", GlobalConfigContext.GAME_SAVE_DIR,
                            GlobalConfigContext.GAME_SAVE_SNAPSHOT_PREFIX, saveIdentity, GlobalConfigContext.GAME_SAVE_SNAPSHOT_POSTFIX)));
                    }
                    // 保存描述子
                    var descFname = String.Format("{0}\\{1}{2}{3}", GlobalConfigContext.GAME_SAVE_DIR,
                        GlobalConfigContext.GAME_SAVE_DESCRIPTOR_PREFIX, saveIdentity, GlobalConfigContext.GAME_SAVE_DESCRIPTOR_POSTFIX);
                    FileStream fs = new FileStream(IOUtils.ParseURItoURL(descFname), FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(Director.RunMana.PerformingChapter.Trim());
                    sw.WriteLine(this.SL_Descriptor_TextBox.Text.Trim());
                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    LogUtils.LogLine("保存存档的辅助文件出现异常" + Environment.NewLine + ex, "SLPage", LogLevel.Warning);
                }
                // 保存完毕强制刷新页面
                this.button_MouseEnter(this.lastPointed, null);
            }
            // 读档
            else
            {
                // 读取文件
                try
                {
                    this.core.GetMainRender().ActualLoad(this.saveList[this.lastPointed].Name.Replace(GlobalConfigContext.GAME_SAVE_POSTFIX, String.Empty));
                    // 返回主舞台
                    ViewPageManager.GoBack();
                }
                catch (Exception ex)
                {
                    var exStr = "读取存档文件失败，存档是损坏的？" + Environment.NewLine + ex.ToString();
                    LogUtils.LogLine(exStr, "SLPage", Utils.LogLevel.Error);
                    MessageBox.Show(exStr);
                }
            }
        }

        /// <summary>
        /// 是否为保存模式
        /// </summary>
        private readonly bool isSave;

        /// <summary>
        /// 最后指向的文件id
        /// </summary>
        private int lastPointed = -1;

        /// <summary>
        /// 文件信息向量
        /// </summary>
        private List<FileInfo> saveList;
        
        /// <summary>
        /// 文件位按钮向量
        /// </summary>
        private readonly List<Button> slotButtonList = new List<Button>();

        /// <summary>
        /// 导演类的引用
        /// </summary>
        private readonly Director core = Director.GetInstance();
    }
}
