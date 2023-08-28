using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace 图像超分工具
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //创建输入输出文件夹
            _ = model.ConfigFileCreate.CreateDir(Properties.Settings.Default.SrcFolder);
            _ = model.ConfigFileCreate.CreateDir(Properties.Settings.Default.DrtFolder);
            MainContent.Content = new UI.NCNNmain();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.Setting();
        }

        private void NCNN_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.NCNNmain();
        }
    }
}
