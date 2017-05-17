using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuri.Hemerocallis.Entity;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// RTBPage.xaml 的交互逻辑
    /// </summary>
    public partial class RTBPage : Page
    {
        /// <summary>
        /// 构造一个富文本页面
        /// </summary>
        public RTBPage()
        {
            InitializeComponent();

            this.UpdateRTBStyle();

            //DataObject.AddPastingHandler(this.RichTextBox_TextArea, new DataObjectPastingEventHandler(OnRichTextBoxPaste));
            this.RichTextBox_TextArea.TextChanged += this.RichTextBox_TextArea_TextChanged;
            this.RichTextBox_TextArea.SelectionChanged += RichTextBox_TextArea_SelectionChanged;
        }

        private void RichTextBox_TextArea_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var cPos = this.RichTextBox_TextArea.CaretPosition;
            if (cPos.Parent is TextElement)
            {
                this.Col = -cPos.GetOffsetToPosition((cPos.Parent as TextElement).ContentStart);
                this.Ln = 0;
                for (int i = 0; i < this.RichTextBox_TextArea.Document.Blocks.Count; i++)
                {
                    if (this.RichTextBox_TextArea.Document.Blocks.ElementAt(i) == cPos.Paragraph)
                    {
                        this.Ln = i;
                        break;
                    }
                }
                this.Ln += 1;
                this.Col += 1;
            }
            if (this.RichTextBox_TextArea.Selection.IsEmpty)
            {
                RTBPage.core.mainWndRef.TextBlock_StateBar.Text = String.Format("Ln: {0}\tCol: {1}\tLen: {2}",
                    this.Ln, this.Col, this.WordCount);
            }
            else
            {
                this.Sel = this.RichTextBox_TextArea.Selection.Text.Count(t => !Char.IsWhiteSpace(t));
                RTBPage.core.mainWndRef.TextBlock_StateBar.Text = String.Format("Ln: {0}\tCol: {1}\tSel:{2}\tLen: {3}",
                    this.Ln, this.Col, this.Sel, this.WordCount);
            }
        }

        /// <summary>
        /// 更新富文本框的文字样式
        /// </summary>
        public void UpdateRTBStyle()
        {
            if (Byte.TryParse(CFontColorItem[0], out byte fr) &&
                Byte.TryParse(CFontColorItem[1], out byte fg) &&
                Byte.TryParse(CFontColorItem[2], out byte fb))
            {
                this.RichTextBox_FlowDocument.Foreground = new SolidColorBrush(Color.FromRgb(fr, fg, fb));
            }
            this.RichTextBox_TextArea.FontSize = RTBPage.CFontSize;
            this.RichTextBox_TextArea.FontFamily = new FontFamily(RTBPage.CFontName);
            this.RichTextBox_DropShadowEffect.Opacity = RTBPage.CZeOpacity;
            this.RichTextBox_FlowDocument.LineHeight = RTBPage.CLineHeight;
            foreach (var block in this.RichTextBox_TextArea.Document.Blocks)
            {
                block.FontSize = RTBPage.CFontSize;
                block.FontFamily = new FontFamily(RTBPage.CFontName);
                block.LineHeight = RTBPage.CLineHeight;
            }
        }

        /// <summary>
        /// 获取该页面RTB中的纯文本字符串
        /// </summary>
        /// <returns>富文本框中的文字字符串</returns>
        internal string GetText()
        {
            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var block in this.RichTextBox_TextArea.Document.Blocks)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.AppendLine();
                }
                if (block is Paragraph)
                {
                    foreach (var inline in ((Paragraph) block).Inlines)
                    {
                        if (inline is Run)
                        {
                            sb.Append(((Run) inline).Text);
                        }
                        else if (inline is LineBreak)
                        {
                            sb.AppendLine();
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 事件：粘贴发生时，过滤非文本元素
        /// </summary>
        private void OnRichTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            // 不是字符串的粘贴都取消
            if (e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                var text = e.SourceDataObject.GetData(DataFormats.UnicodeText);
                var dataObj = new DataObject();
                dataObj.SetData(DataFormats.UnicodeText, text ?? String.Empty);
                e.DataObject = dataObj;
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// 事件：富文本框内容更新
        /// </summary>
        private void RichTextBox_TextArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange tx = new TextRange(this.RichTextBox_FlowDocument.ContentStart,
                this.RichTextBox_FlowDocument.ContentEnd);
            this.WordCount = tx.Text.Count(t => !Char.IsWhiteSpace(t));
            RTBPage.core.mainWndRef.TextBlock_StateBar.Text = String.Format("Ln: {0}\tCol: {1}\tLen: {2}", this.Ln, this.Col, this.WordCount);
        }

        /// <summary>
        /// 获取或设置该页所显示的文章引用
        /// </summary>
        internal HArticle ArticalRef { get; set; }

        /// <summary>
        /// 获取或设置富文本页的字体颜色
        /// </summary>
        public static string[] CFontColorItem;

        /// <summary>
        /// 获取或设置富文本页的字号
        /// </summary>
        public static double CFontSize;

        /// <summary>
        /// 获取或设置富文本页的字体名字
        /// </summary>
        public static string CFontName;

        /// <summary>
        /// 获取或设置富文本页的晕染效果不透明度
        /// </summary>
        public static double CZeOpacity;

        /// <summary>
        /// 获取或设置富文本页的行距
        /// </summary>
        public static double CLineHeight;

        public long Ln { get; set; }

        public long Col { get; set; }

        public long Sel { get; set; }

        public long WordCount { get; set; }

        /// <summary>
        /// 后台的引用
        /// </summary>
        private static readonly Controller core = Controller.GetInstance();
    }
}
