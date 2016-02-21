using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;

using Yuri.Utils;
using Yuri.PlatformCore;
using Yuri.ILPackage;
using Yuri;
using Yuri.YuriInterpreter;


namespace Yuri
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Director core = Director.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            core.SetMainWindow(this);
        }


        public void DoEvent()
        {
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Background);
        }

        bool flag = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag == false)
            {

                this.BO_MainGrid.Background = new ImageBrush(core.testBitmapImage("bg1.png").myImage);
                flag = true;
            }
            else
            {
                this.BO_MainGrid.Background = new ImageBrush(core.testBitmapImage("bg2.png").myImage);
                flag = false;
            }
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

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Interpreter ip = new Interpreter("TestProj", @"C:\Users\Kako\Desktop\testDir");
            ip.Dash(InterpreterType.RELEASE_WITH_IL, 8);
            ip.GetILFile(@"Scenario\main.sil");

            ILConvertor ilc = ILConvertor.GetInstance();
            List<Scene> rS = ilc.Dash(@"Scenario");
        }




        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

            Musician m = Musician.GetInstance();
            m.PlayBGM(@"Boss01.wav", @"Sound\bgm\Boss01.wav", 1000);
            //timer.Start();
            
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            core.testBGM("车椅子の未来宇宙.mp3");
            //core.testBGM("Boss01.wav");
            
            //timer.Start();
        }



        private void callback_typing(object sender, EventArgs e)
        {
            //this.BO_MsgTria.Visibility = Visibility.Visible;
            //this.BO_MsgTria.RenderTransform = new TranslateTransform();
            //this.ApplyUpDownAnimation(this.BO_MsgTria.Name);
        }
        



        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            //mp.Stop();
            //System.Media.SoundPlayer sp = new System.Media.SoundPlayer(@"Sound\se\se01.wav");
            //sp.Play();
            core.testVocal("Alice002.mp3");
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            core.DisposeResource();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.core.UpdateKeyboard(e);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.core.UpdateKeyboard(e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.core.UpdateMouseWheel(e.Delta);
        }

        private void BO_LeftChara_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void BO_LeftChara_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void BO_LeftChara_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void BO_LeftChara_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        




    }
}
