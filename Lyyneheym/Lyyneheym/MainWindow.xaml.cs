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
using System.IO;

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
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.BO_MessageBoxLayer.Visibility == System.Windows.Visibility.Hidden)
            {
                this.BO_MainName.Visibility = this.BO_MainText.Visibility = 
                    this.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Visible;
                
            }
            else
            {
                this.BO_MainName.Visibility = this.BO_MainText.Visibility =
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
            img.Width = myBitmapImage.Width * 2;
            img.Height = myBitmapImage.Height * 2;
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
            this.BO_MainName.Text = "【蓬莱山辉夜】";
            this.BO_MainText.Text = "Erin Erin" + Environment.NewLine + "助けてErin～！";
        }


        MediaPlayer mp = null;
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            mp = new MediaPlayer();
            mp.Open(new Uri(@"Sound\bgm\bgm01.mp3", UriKind.RelativeOrAbsolute));
            mp.Play();
        }

        bool playflag = false;
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (playflag == false)
            {
                mp.Pause();
                playflag = true;
            }
            else
            {
                mp.Play();
                playflag = false;
            }
            
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            //mp.Stop();
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(@"Sound\se\se01.wav");
            sp.Play();
        }
    }
}
