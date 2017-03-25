using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Yuri.YuriLauncher.Utils;
using System.Windows.Media;

namespace Yuri.YuriLauncher.Forms
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 在指定的文字层绑定控件上进行打字动画
        /// </summary>
        /// <param name="appendString">要追加的字符串</param>
        /// <param name="msglayBinding">文字层的控件</param>
        /// <param name="wordTimeSpan">字符之间的打字时间间隔</param>
        private void TypeWriter(string appendString, TextBlock msglayBinding, int wordTimeSpan)
        {
            MsgLayerTypingStory = new Storyboard();
            StringAnimationUsingKeyFrames stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames();
            Duration aniDuration = new Duration(TimeSpan.FromMilliseconds(wordTimeSpan * appendString.Length + 2000));
            stringAnimationUsingKeyFrames.Duration = aniDuration;
            MsgLayerTypingStory.Duration = aniDuration;
            string tmp = String.Empty;
            int ctr = 0;
            foreach (char c in appendString)
            {
                var discreteStringKeyFrame = new DiscreteStringKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(ctr++ * wordTimeSpan))
                };
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }
            var waitFrame = new DiscreteStringKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(ctr * wordTimeSpan + 2000)
            };
            Storyboard.SetTarget(stringAnimationUsingKeyFrames, msglayBinding);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            MsgLayerTypingStory.Children.Add(stringAnimationUsingKeyFrames);
            MsgLayerTypingStory.RepeatBehavior = RepeatBehavior.Forever;
            MsgLayerTypingStory.Begin();
        }

        private Storyboard MsgLayerTypingStory;

        private void radioButton_Screen_Animation_2_Checked(object sender, RoutedEventArgs e)
        {
            if (this.radioButton_Screen_Animation_2.IsChecked == true)
            {
                this.groupBox_Screen_SCamera.IsEnabled = false;
                this.radioButton_Screen_SCamera_2.IsChecked = true;
            }
            else
            {
                this.groupBox_Screen_SCamera.IsEnabled = true;
            }
        }

        private void radioButton_Screen_Animation_3_Checked(object sender, RoutedEventArgs e)
        {
            if (this.radioButton_Screen_Animation_3.IsChecked == true)
            {
                this.groupBox_Screen_SCamera.IsEnabled = false;
                this.radioButton_Screen_SCamera_2.IsChecked = true;
            }
            else
            {
                this.groupBox_Screen_SCamera.IsEnabled = true;
            }
        }

        private void radioButton_Screen_Animation_1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.radioButton_Screen_Animation_1.IsChecked == true)
            {
                if (this.groupBox_Screen_SCamera != null)
                {
                    this.groupBox_Screen_SCamera.IsEnabled = true;
                    this.radioButton_Screen_SCamera_1.IsChecked = true;
                }
            }
        }

        private readonly string TypingStr = "伊泽塔爱菲涅。" + Environment.NewLine + "abcABC123?!";

        private void Tab_Screen_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.TypeWriter(TypingStr, this.textblock_Screen_Typing, (int)this.slider_Screen_Typing.Value);
        }

        private void radioButton_Screen_Typing_2_Checked(object sender, RoutedEventArgs e)
        {
            if (this.slider_Screen_Typing != null)
            {
                this.slider_Screen_Typing.Value = 0;
                this.slider_Screen_Typing.IsEnabled = false;
            }
        }

        private void radioButton_Screen_Typing_1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.slider_Screen_Typing != null)
            {
                this.slider_Screen_Typing.Value = 60;
                this.slider_Screen_Typing.IsEnabled = true;
            }
        }

        private void slider_Screen_Typing_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.label_Screen_Typing != null)
            {
                this.label_Screen_Typing.Content = this.slider_Screen_Typing.Value.ToString("0");
                MsgLayerTypingStory?.Stop();
                this.TypeWriter(TypingStr, this.textblock_Screen_Typing, (int)this.slider_Screen_Typing.Value);
            }
        }
        
        private void ToggleSwitch_Sound_Mute_OnIsCheckedChanged(object sender, EventArgs e)
        {
            this.groupBox_Sound_BGM.IsEnabled = this.groupBox_Sound_BGS.IsEnabled = groupBox_Sound_SE.IsEnabled
                = this.groupBox_Sound_Vocal.IsEnabled = this.toggleSwitch_Sound_Mute.IsChecked != true;
        }
        
        private Musician musicMana = Musician.GetInstance();

        private void button_Sound_BGM_Try_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.PlayBGM("bgm.mp3", IOUtils.ParseURItoURL(@"LItem\bgm.mp3"),
                (int)(this.slider_Sound_BGM.Value) * 10);
        }

        private void button_Sound_BGM_TryEnd_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.StopAndReleaseBGM();
        }

        private void slider_Sound_BGM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.label_Sound_BGM != null)
            {
                this.label_Sound_BGM.Content = this.slider_Sound_BGM.Value.ToString("0");
                if (this.musicMana.isBGMPlaying)
                {
                    this.musicMana.SetBGMVolume((int)(this.slider_Sound_BGM.Value) * 10);
                }
            }
        }

        private void button_about_reference_Click(object sender, RoutedEventArgs e)
        {
            (new LicenseForm()).ShowDialog();
        }

        private int seCounter = 1;
        private void button_Sound_SE_Try_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.PlaySE(IOUtils.ParseURItoURL(String.Format("LItem\\se{0}.mp3", seCounter++)),
                (int)(this.slider_Sound_SE.Value) * 10);
            if (seCounter == 4)
            {
                this.seCounter = 1;
            }
        }

        private void slider_Sound_SE_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.label_Sound_SE != null)
            {
                this.label_Sound_SE.Content = this.slider_Sound_SE.Value.ToString("0");
            }
        }

        private void button_Sound_BGS_Try_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.PlayBGS(IOUtils.ParseURItoURL(@"LItem\bgs.mp3"), (int)(this.slider_Sound_BGM.Value) * 10);
        }

        private void button_Sound_BGS_TryEnd_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.StopBGS();
        }

        private void slider_Sound_BGS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.label_Sound_BGS != null)
            {
                this.label_Sound_BGS.Content = this.slider_Sound_BGS.Value.ToString("0");
                if (this.musicMana.isAnyBGS)
                {
                    this.musicMana.SetBGSVolume((int)(this.slider_Sound_BGS.Value) * 10);
                }
            }
        }

        private void slider_Sound_Vocal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.label_Sound_Vocal != null)
            {
                this.label_Sound_Vocal.Content = this.slider_Sound_Vocal.Value.ToString("0");
            }
        }

        private int voiceCounter = 1;
        private void button_Sound_Vocal_Try_Click(object sender, RoutedEventArgs e)
        {
            this.musicMana.PlayVocal(IOUtils.ParseURItoURL(String.Format("LItem\\vocal{0}.wav", voiceCounter++)),
                (int)(this.slider_Sound_Vocal.Value) * 10);
            if (voiceCounter == 4)
            {
                voiceCounter = 1;
            }
        }

        private FontDialog fontDialog;

        private void Button_Screen_Typing_ChangeFont_Click(object sender, RoutedEventArgs e)
        {
            var cFont = new Font(this.textblock_Screen_Typing.FontFamily.FamilyNames.Last().Value,
                (float) this.textblock_Screen_Typing.FontSize);
            fontDialog = new FontDialog
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
                this.textblock_Screen_Typing.FontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.Name);
                this.textblock_Screen_Typing.FontSize = fontDialog.Font.Size;
            }
        }
    }
}
