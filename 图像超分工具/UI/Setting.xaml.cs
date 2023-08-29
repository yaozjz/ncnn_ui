using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 图像超分工具.UI
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {
        public Setting()
        {
            InitializeComponent();
            init_ctrl();
        }
        void init_ctrl()
        {
            IsShowDot.IsChecked = Properties.Settings.Default.ShowDot;
        }

        private async void Save_config_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowDot = (bool)IsShowDot.IsChecked;

            Properties.Settings.Default.Save();
            SaveDone_msg.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SaveDone_msg.Visibility = Visibility.Collapsed;
                });
            });
        }
    }
}
