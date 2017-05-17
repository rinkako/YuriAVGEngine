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
        public RTBPage()
        {
            InitializeComponent();

            this.UpdateRTBStyle();
            
            DataObject.AddPastingHandler(this.RichTextBox_TextArea, new DataObjectPastingEventHandler(OnRichTextBoxPaste));

            this.RichTextBox_TextArea.TextChanged += this.RichTextBox_TextArea_TextChanged;
        }

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
                if (isFirst) { isFirst = false; }
                else { sb.AppendLine(); }
                if (block is Paragraph)
                {
                    foreach (var inline in ((Paragraph) block).Inlines)
                    {
                        if (inline is Run) { sb.Append(((Run)inline).Text); }
                        else if (inline is LineBreak) { sb.AppendLine();}
                    }
                }
            }
            return sb.ToString();
        }
        
        private void OnRichTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            // 不是字符串的粘贴都取消
            if (e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                //重新获取字符串并设置DataObject
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

        internal HArticle ArticalRef;
        public static string[] CFontColorItem;
        public static double CFontSize;
        public static string CFontName;
        public static double CZeOpacity;
        public static double CLineHeight;
        
        private void RichTextBox_TextArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange tx = new TextRange(this.RichTextBox_FlowDocument.ContentStart, this.RichTextBox_FlowDocument.ContentEnd);
            var cc = tx.Text.Count(t => !Char.IsWhiteSpace(t));
            RTBPage.core.mainWndRef.TextBlock_StateBar.Text = String.Format("Ln: {0}\tCol: {1}\tLen: {2}", 0, 0, cc);
        }
        
        private static readonly Controller core = Controller.GetInstance();
    }
}
