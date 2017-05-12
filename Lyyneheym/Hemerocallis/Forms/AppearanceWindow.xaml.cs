using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hemerocallis;
using MahApps.Metro.Controls;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// AppearanceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppearanceWindow : MetroWindow
    {
        public AppearanceWindow()
        {
            InitializeComponent();
            this.RadioButton_Bg_1.IsChecked = true;
            this.RadioButton_Bg_2.IsChecked = false;
            this.RadioButton_Bg_3.IsChecked = false;
            this.CheckBox_Font_Ze.IsChecked = true;
            this.curFontName = this.RichTextBox_Font_Preview.FontFamily.FamilyNames.Last().Value;
            this.curFontSize = (float)this.RichTextBox_Font_Preview.FontSize;
        }

        private void CheckBox_Font_Ze_Checked(object sender, RoutedEventArgs e)
        {
            this.Slider_Font_ZeRadius.Value = 3;
            this.Slider_Font_ZeRadius.IsEnabled = true;
        }

        private void CheckBox_Font_Ze_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Slider_Font_ZeRadius.Value = 0;
            this.Slider_Font_ZeRadius.IsEnabled = false;
        }

        private void RadioButton_Bg_2_Checked(object sender, RoutedEventArgs e)
        {
            this.TextBox_Bg_R.IsEnabled = this.TextBox_Bg_G.IsEnabled = this.TextBox_Bg_B.IsEnabled = true;
        }

        private void RadioButton_Bg_2_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TextBox_Bg_R.IsEnabled = this.TextBox_Bg_G.IsEnabled = this.TextBox_Bg_B.IsEnabled = false;
        }

        private void RadioButton_Bg_3_Checked(object sender, RoutedEventArgs e)
        {
            this.Button_Bg_SelectPic.IsEnabled = true;
        }

        private void RadioButton_Bg_3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Button_Bg_SelectPic.IsEnabled = false;
        }

        private void Button_Default_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("真的要恢复默认设置吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.RadioButton_Bg_1.IsChecked = true;
                this.RadioButton_Bg_2.IsChecked = false;
                this.RadioButton_Bg_3.IsChecked = false;
                this.CheckBox_Font_Ze.IsChecked = true;
                this.Slider_Font_LineHeight.Value = 10;
                this.Slider_Font_ZeRadius.Value = 3;
                this.TextBox_Bg_R.Text = this.TextBox_Bg_G.Text = this.TextBox_Bg_B.Text = "0";
                this.Label_Bg_PicName.Content = "N/A";
                this.curFontName = "微软雅黑";
                this.curFontSize = 22;
                this.RichTextBox_Font_Preview.FontFamily = new System.Windows.Media.FontFamily(curFontName);
                this.RichTextBox_Font_Preview.FontSize = curFontSize;
            }
        }

        private string curFontName;

        private double curFontSize;
        
        private void Slider_Font_ZeRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.RichTextBox_Preview_DropShadowEffect != null)
            {
                this.RichTextBox_Preview_DropShadowEffect.Opacity = this.Slider_Font_ZeRadius.Value / 10.0;
            }
        }

        private void Slider_Font_LineHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.RichTextBox_Preview_DropShadowEffect != null)
            {
                this.RichTextBox_Preview_FlowDocument.LineHeight = this.Slider_Font_LineHeight.Value;
            }
        }
        
        private void Button_Font_Select_Click(object sender, RoutedEventArgs e)
        {
            var cFont = new Font(this.RichTextBox_Font_Preview.FontFamily.FamilyNames.Last().Value,
                (float)this.RichTextBox_Font_Preview.FontSize);
            var fontDialog = new System.Windows.Forms.FontDialog
            {
                AllowVerticalFonts = false,
                AllowScriptChange = false,
                MinSize = 8,
                ShowEffects = false,
                ShowColor = false,
                ShowHelp = false,
                ShowApply = false,
                FontMustExist = true,
                Font = cFont
            };
            fontDialog.ShowDialog();
            if (fontDialog.Font.Name != cFont.Name || Math.Abs(fontDialog.Font.Size - cFont.Size) >= 1)
            {
                this.RichTextBox_Font_Preview.FontFamily = new System.Windows.Media.FontFamily(this.curFontName = fontDialog.Font.Name);
                this.curFontSize = this.RichTextBox_Font_Preview.FontSize = fontDialog.Font.Size;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.RadioButton_Bg_2.IsChecked == true)
            {
                if (!(Int32.TryParse(this.TextBox_Bg_R.Text, out int tr) &&
                    Int32.TryParse(this.TextBox_Bg_G.Text, out int tg) &&
                    Int32.TryParse(this.TextBox_Bg_B.Text, out int tb) &&
                    tr >= 0 && tr <= 255 &&
                    tg >= 0 && tg <= 255 &&
                    tb >= 0 && tb <= 255))
                {
                    MessageBox.Show("RGB值必须是0到255的整数值");
                    return;
                }
            }
            else if (this.RadioButton_Bg_3.IsChecked == true)
            {
                if (this.bgPicPath == String.Empty)
                {
                    MessageBox.Show("必须选择背景图片");
                    return;
                }
                try
                {
                    if (File.Exists(this.bgPicPath))
                    {
                        var pureName = this.bgPicPath.Split('\\').Last();
                        File.Copy(this.bgPicPath, App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory, pureName), true);
                    }
                    else
                    {
                        MessageBox.Show("背景图片不存在");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置自定义图片背景失败" + Environment.NewLine + ex);
                    return;
                }
            }
        }

        private void Button_Bg_SelectPic_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FileDialog fd = new System.Windows.Forms.OpenFileDialog();
            fd.Filter = @"支持的图片类型|*.jpg;*.png";
            fd.ShowDialog();
            if (fd.FileName != String.Empty)
            {
                bgPicPath = fd.FileName;
                this.Label_Bg_PicName.Content = fd.FileName.Split('\\').Last();
            }
        }

        private string bgPicPath = String.Empty;
    }
}
