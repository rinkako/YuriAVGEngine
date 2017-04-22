using System.Windows;

namespace Yuri.YuriHalation.YuriForms
{
    /// <summary>
    /// SCamera3DForm.xaml 的交互逻辑
    /// </summary>
    public partial class SCamera3DForm : Window
    {
        public SCamera3DForm()
        {
            InitializeComponent();
            this.T_Frame.Content = new Stage3D();
        }

        private void Action_Radio_4_Checked(object sender, RoutedEventArgs e)
        {
            this.Groupbox_Position.IsEnabled = true;
        }

        private void Action_Radio_4_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Groupbox_Position.IsEnabled = false;
        }

        private void Action_Radio_5_Checked(object sender, RoutedEventArgs e)
        {
            this.Groupbox_Focusing.IsEnabled = true;
            this.Groupbox_Position.IsEnabled = true;
        }

        private void Action_Radio_5_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Groupbox_Focusing.IsEnabled = false;
            this.Groupbox_Position.IsEnabled = false;
        }
    }
}
