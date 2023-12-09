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
            //创建输出文件夹
            _ = model.ConfigFileCreate.CreateDir(Properties.Settings.Default.DrtFolder);
            MainContent.Content = new UI.NCNNmain();

            //读取上一次窗口状态
            Width = Properties.Settings.Default.WinWidth;
            Height = Properties.Settings.Default.WinHeight;
            WindowState = Properties.Settings.Default.SavedWindowState;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.Setting();
        }

        private void NCNN_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.NCNNmain();
        }
        //窗口关闭

        private void MainWin_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.SavedWindowState = WindowState;
            Properties.Settings.Default.WinHeight = Height;
            Properties.Settings.Default.WinWidth = Width;

            Properties.Settings.Default.Save();
        }
    }
}
