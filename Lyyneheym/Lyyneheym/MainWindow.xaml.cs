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
using System.Windows.Media.Animation;
using System.IO;

using Lyyneheym.LyyneheymCore.Utils;
using Lyyneheym.LyyneheymCore;
using Lyyneheym.SlyviaInterpreter;

using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Slyvia core = Slyvia.getInstance();

        public MainWindow()
        {
            InitializeComponent();
            this.testFontEffect(this.BO_MainText);


            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"PictureAssets\pictures\exitmenu3.png", UriKind.RelativeOrAbsolute);
            myBitmapImage.EndInit();
            mytestbutton.Width = myBitmapImage.Width;
            mytestbutton.Height = myBitmapImage.Height;
            mytestbutton.Source = myBitmapImage;
            mytestbutton.Margin = new Thickness(0, 0, 0, 0);
            mytestbutton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            mytestbutton.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.BO_MessageBoxLayer.Visibility == System.Windows.Visibility.Hidden)
            {
                this.BO_MainName.Visibility = this.BO_MainText.Visibility = this.BO_MsgTria.Visibility =
                    this.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Visible;
                
            }
            else
            {
                this.BO_MainName.Visibility = this.BO_MainText.Visibility = this.BO_MsgTria.Visibility =
                    this.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        bool flag = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag == false)
            {
                this.BO_MainGrid.Background = new ImageBrush(core.testBitmapImage("bg1.png"));
                flag = true;
            }
            else
            {
                this.BO_MainGrid.Background = new ImageBrush(core.testBitmapImage("bg2.png"));
                flag = false;
            }
            
        }


        private void testLexer()
        {
            Interpreter ip = new Interpreter("TestProj", @"C:\Users\Kako\Desktop\testDir");
            ip.Dash(InterpreterType.RELEASE_WITH_IL, 8);
            ip.GetILFile(@"C:\Users\Kako\Desktop\Res\mylog.sil");

            ILConvertor ilc = ILConvertor.GetInstance();
            ilc.Dash(@"C:\Users\Kako\Desktop\Res");
        }


        private void testFontEffect(TextBlock label)
        {
            //LinearGradientBrush brush = new LinearGradientBrush();

            //GradientStop gradientStop1 = new GradientStop();
            //gradientStop1.Offset = 0;
            //gradientStop1.Color = Color.FromArgb(255, 251, 100, 17);
            //brush.GradientStops.Add(gradientStop1);

            //GradientStop gradientStop2 = new GradientStop();
            //gradientStop2.Offset = 1;
            //gradientStop2.Color = Color.FromArgb(255, 247, 238, 52);
            //brush.GradientStops.Add(gradientStop2);

            
            //brush.StartPoint = new Point(0.5, 0);
            //brush.EndPoint = new Point(0.5, 1);
            //label.Foreground = brush;
            System.Windows.Media.Effects.DropShadowEffect ds = new System.Windows.Media.Effects.DropShadowEffect();
            ds.ShadowDepth = 2;
            ds.Opacity = 0.5;
            label.Effect = ds;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //BitmapImage myBitmapImage = new BitmapImage();
            //myBitmapImage.BeginInit();
            //myBitmapImage.UriSource = new Uri(@"PictureAssets\character\CA01.png", UriKind.RelativeOrAbsolute);
            //myBitmapImage.EndInit();
            BitmapImage myBitmapImage = core.testCharaStand("CA01.png");
            this.BO_LeftChara.Width = myBitmapImage.PixelWidth;
            this.BO_LeftChara.Height = myBitmapImage.PixelHeight;
            this.BO_LeftChara.Source = myBitmapImage;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //BitmapImage myBitmapImage = new BitmapImage();
            //myBitmapImage.BeginInit();
            //myBitmapImage.UriSource = new Uri(@"PictureAssets\character\CA02.png", UriKind.RelativeOrAbsolute);
            //myBitmapImage.EndInit();
            BitmapImage myBitmapImage = core.testCharaStand("CA02.png");
            this.BO_RightChara.Width = myBitmapImage.PixelWidth;
            this.BO_RightChara.Height = myBitmapImage.PixelHeight;
            this.BO_RightChara.Source = myBitmapImage;
        }

        bool flag2 = false;
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (flag2 == false)
            {
                Panel.SetZIndex(this.BO_MessageBoxLayer, 91);
                flag2 = true;
            }
            else
            {
                Panel.SetZIndex(this.BO_MessageBoxLayer, -1);
                flag2 = false;
            }
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Image img = new Image();
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"PictureAssets\pictures\uuz.jpg", UriKind.RelativeOrAbsolute);
            myBitmapImage.EndInit();
            img.Width = myBitmapImage.Width;
            img.Height = myBitmapImage.Height;
            img.Source = myBitmapImage;
            img.Margin = new Thickness(0, 0, 0, 0);
            img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            img.Name = "UBO_PIC_01";
            this.BO_MainGrid.Children.Add(img);
            
        }


        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            foreach (UIElement c in this.BO_MainGrid.Children)
            {
                if (c is Image)
                {
                    Image obc = (Image)c;
                    if (obc.Name == "UBO_PIC_01")
                    {
                        obc.Visibility = obc.Visibility == System.Windows.Visibility.Collapsed ?
                            System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                        this.BO_MainGrid.Children.Remove(c);
                        break;
                    }
                }
            }

        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            TypewriteTextblock("测试文本测试文本测试文本测试文本", this.BO_MainText, 30);
        }


        MediaPlayer mp = null;
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //mp = new MediaPlayer();
            //mp.Open(new Uri(@"Sound\bgm\bgm01.mp3", UriKind.RelativeOrAbsolute));
            //mp.Play();
            this.testLexer();
        }

        bool playflag = false;
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (playflag == false && mp != null)
            {
                mp.Pause();
                playflag = true;
            }
            else
            {
                if (mp == null)
                {
                    mp = new MediaPlayer();
                    mp.Open(new Uri(@"Sound\bgm\bgm01.mp3", UriKind.RelativeOrAbsolute));
                    mp.Play();
                }
                else
                {
                    mp.Play();
                    playflag = false;
                }
            }
            
        }

        private void TypewriteTextblock(string textToAnimate, TextBlock txt, int timeSpan)
        {
            this.BO_MsgTria.Visibility = System.Windows.Visibility.Hidden;
            Storyboard story = new Storyboard();
            story.FillBehavior = FillBehavior.HoldEnd;
            DiscreteStringKeyFrame discreteStringKeyFrame;
            StringAnimationUsingKeyFrames stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames();
            stringAnimationUsingKeyFrames.Duration = new Duration(TimeSpan.FromMilliseconds(timeSpan * textToAnimate.Length));
            string tmp = string.Empty;
            foreach (char c in textToAnimate)
            {
                discreteStringKeyFrame = new DiscreteStringKeyFrame();
                discreteStringKeyFrame.KeyTime = KeyTime.Paced;
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }
            Storyboard.SetTargetName(stringAnimationUsingKeyFrames, txt.Name);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            story.Children.Add(stringAnimationUsingKeyFrames);
            story.Completed += new EventHandler(callback_typing);
            story.Begin(txt);
        }

        private void callback_typing(object sender, EventArgs e)
        {
            this.BO_MsgTria.Visibility = Visibility.Visible;
            this.BO_MsgTria.RenderTransform = new TranslateTransform();
            this.ApplyUpDownAnimation(this.BO_MsgTria.Name);
            
        }

        private void ApplyUpDownAnimation(string dependence)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(0, 10, new Duration(TimeSpan.FromMilliseconds(500)));
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            da.AccelerationRatio = 0.8;
            Storyboard.SetTargetName(da, dependence);
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
                Image.RenderTransformProperty,
                TranslateTransform.YProperty,
            };
            Storyboard.SetTargetProperty(da, new PropertyPath("(0).(1)", propertyChain));
            sb.Children.Add(da);
            sb.Begin(this);
        }


        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            //mp.Stop();
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(@"Sound\se\se01.wav");
            sp.Play();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Point p = e.MouseDevice.GetPosition((Image)sender);

   

            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"PictureAssets\pictures\exitmenu3.png", UriKind.RelativeOrAbsolute);
            myBitmapImage.EndInit();

            Color hitC = this.GetPixelColor(myBitmapImage, (int)p.X, (int)p.Y);

            if (hitC.A > 10)
            {
                MessageBox.Show(p.ToString() + " == " + hitC.ToString());
            }
            
        }

        public Color GetPixelColor(BitmapSource source, int x, int y)
        {
            Color c = Colors.White;
            if (source != null)
            {
                try
                {
                    CroppedBitmap cb = new CroppedBitmap(source, new Int32Rect(x, y, 1, 1));
                    var pixels = new byte[4];
                    cb.CopyPixels(pixels, 4, 0);
                    c = Color.FromArgb(pixels[3] ,pixels[2], pixels[1], pixels[0]);
                }
                catch (Exception) { }
            }
            return c;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            this.mytestbutton.RenderTransform = new TranslateTransform();
            this.ApplyUpDownAnimation(this.mytestbutton.Name);
        }
    }
}
