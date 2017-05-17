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
using MahApps.Metro.Controls;

namespace Yuri.Hemerocallis.Forms
{
    /// <summary>
    /// AppearanceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppearanceWindow : MetroWindow
    {
        private readonly Controller core = Controller.GetInstance();

        public AppearanceWindow()
        {
            InitializeComponent();
            var cfg = core.ConfigDesc;
            switch (cfg.BgType)
            {
                case Entity.AppearanceBackgroundType.Picture:
                    this.RadioButton_Bg_1.IsChecked = false;
                    this.RadioButton_Bg_2.IsChecked = false;
                    this.RadioButton_Bg_3.IsChecked = true;
                    this.Label_Bg_PicName.Content = cfg.BgTag;
                    this.bgPicPath = App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory, cfg.BgTag);
                    break;
                case Entity.AppearanceBackgroundType.Pure:
                    this.RadioButton_Bg_1.IsChecked = false;
                    this.RadioButton_Bg_2.IsChecked = true;
                    this.RadioButton_Bg_3.IsChecked = false;
                    var rgbItems = cfg.BgTag.Split(',');
                    this.TextBox_Bg_R.Text = rgbItems[0];
                    this.TextBox_Bg_G.Text = rgbItems[1];
                    this.TextBox_Bg_B.Text = rgbItems[2];
                    break;
                default:
                    this.RadioButton_Bg_1.IsChecked = true;
                    this.RadioButton_Bg_2.IsChecked = false;
                    this.RadioButton_Bg_3.IsChecked = false;
                    break;
            }
            this.CheckBox_Font_Ze.IsChecked = cfg.IsEnableZe;
            this.RichTextBox_Font_Preview.FontFamily = new System.Windows.Media.FontFamily(this.curFontName = cfg.FontName);
            this.RichTextBox_Font_Preview.FontSize = this.curFontSize = cfg.FontSize;
            this.Slider_Font_LineHeight.Value = cfg.LineHeight;
            this.Slider_Font_ZeRadius.Value = cfg.ZeOpacity * 10;
            var ftcItems = cfg.FontColor.Split(',');
            this.TextBox_Ft_R.Text = ftcItems[0];
            this.TextBox_Ft_G.Text = ftcItems[1];
            this.TextBox_Ft_B.Text = ftcItems[2];
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
                this.TextBox_Bg_R.Text = this.TextBox_Bg_G.Text = this.TextBox_Bg_B.Text = "255";
                this.TextBox_Ft_R.Text = "44";
                this.TextBox_Ft_G.Text = "63";
                this.TextBox_Ft_B.Text = "81";
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
                AllowScriptChange = true,
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
            this.Button_Apply_Click(sender, e);
            this.Close();
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

        private void TextBox_Ft_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Ellipse_Font != null)
            {
                if (Byte.TryParse(this.TextBox_Ft_R.Text, out byte fr) &&
                    Byte.TryParse(this.TextBox_Ft_G.Text, out byte fg) &&
                    Byte.TryParse(this.TextBox_Ft_B.Text, out byte fb))
                {
                    this.Ellipse_Font.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(fr, fg, fb));
                }
            }
        }

        private void TextBox_Bg_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Ellipse_Bg != null)
            {
                if (Byte.TryParse(this.TextBox_Bg_R.Text, out byte br) &&
                    Byte.TryParse(this.TextBox_Bg_G.Text, out byte bg) &&
                    Byte.TryParse(this.TextBox_Bg_B.Text, out byte bb))
                {
                    this.Ellipse_Bg.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(br, bg, bb));
                }
            }
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            if (this.RadioButton_Bg_1.IsChecked == true)
            {
                this.core.ConfigDesc.BgType = Entity.AppearanceBackgroundType.Default;
                this.core.mainWndRef.MainAreaBrush = this.core.mainWndRef.DefaultBgBrush;
                if (this.core.mainWndRef.CurrentActivePage != null)
                {
                    this.core.mainWndRef.Grid_MainArea.Background = this.core.mainWndRef.MainAreaBrush;
                }
            }
            else if (this.RadioButton_Bg_2.IsChecked == true)
            {
                if (!(Byte.TryParse(this.TextBox_Bg_R.Text, out byte tr) &&
                    Byte.TryParse(this.TextBox_Bg_G.Text, out byte tg) &&
                    Byte.TryParse(this.TextBox_Bg_B.Text, out byte tb)))
                {
                    MessageBox.Show("RGB值必须是0到255的整数值");
                    return;
                }
                this.core.ConfigDesc.BgType = Entity.AppearanceBackgroundType.Pure;
                this.core.ConfigDesc.BgTag = $"{tr},{tg},{tb}";
                this.core.mainWndRef.MainAreaBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(tr, tg, tb));
                if (this.core.mainWndRef.CurrentActivePage != null)
                {
                    this.core.mainWndRef.Grid_MainArea.Background = this.core.mainWndRef.MainAreaBrush;
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
                        var destPath = App.ParseURIToURL(App.AppDataDirectory, App.AppearanceDirectory, pureName);
                        if (destPath != this.bgPicPath)
                        {
                            File.Copy(this.bgPicPath, destPath, true);
                        }
                        this.bgPicPath = destPath;
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
                this.core.ConfigDesc.BgType = Entity.AppearanceBackgroundType.Picture;
                this.core.ConfigDesc.BgTag = this.bgPicPath.Split('\\').Last();
                var bmp = new BitmapImage(new Uri(this.bgPicPath));
                this.core.mainWndRef.MainAreaBrush = new ImageBrush(bmp)
                {
                    TileMode = TileMode.Tile,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight)
                };
                if (this.core.mainWndRef.CurrentActivePage != null)
                {
                    this.core.mainWndRef.Grid_MainArea.Background = this.core.mainWndRef.MainAreaBrush;
                }
            }
            if (!(Byte.TryParse(this.TextBox_Ft_R.Text, out byte fr) &&
                  Byte.TryParse(this.TextBox_Ft_G.Text, out byte fg) &&
                  Byte.TryParse(this.TextBox_Ft_B.Text, out byte fb)))
            {
                MessageBox.Show("RGB值必须是0到255的整数值");
                return;
            }
            this.core.ConfigDesc.FontName = this.curFontName;
            this.core.ConfigDesc.FontSize = this.curFontSize;
            this.core.ConfigDesc.LineHeight = this.Slider_Font_LineHeight.Value;
            this.core.ConfigDesc.ZeOpacity = this.Slider_Font_ZeRadius.Value / 10.0;
            this.core.ConfigDesc.IsEnableZe = this.CheckBox_Font_Ze.IsChecked == true;
            this.core.ConfigDesc.FontColor = $"{fr},{fg},{fb}";
            RTBPage.CFontColorItem = this.core.ConfigDesc.FontColor.Split(',');
            RTBPage.CFontSize = this.core.ConfigDesc.FontSize;
            RTBPage.CFontName = this.core.ConfigDesc.FontName;
            RTBPage.CZeOpacity = this.core.ConfigDesc.ZeOpacity;
            RTBPage.CLineHeight = this.core.ConfigDesc.LineHeight;
            foreach (var tp in this.core.mainWndRef.RTBPageCacheDict)
            {
                tp.Value.UpdateRTBStyle();
            }
            this.core.WriteConfigToSteady();
        }
    }
}
