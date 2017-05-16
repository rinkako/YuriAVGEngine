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
        }

        public string ArticalId;
        public static string[] CFontColorItem;
        public static double CFontSize;
        public static string CFontName;
        public static double CZeOpacity;
        public static double CLineHeight;
    }
}
