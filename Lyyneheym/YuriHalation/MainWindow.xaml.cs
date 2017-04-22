using System.Windows;
using Yuri.YuriHalation.YuriForms;

namespace Yuri.YuriHalation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mf = new MainForm();
            mf.ShowDialog();
            this.Close();
        }

        public static MainForm mf;
    }

}
