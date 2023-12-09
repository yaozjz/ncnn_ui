using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 图像超分工具.UI
{

    /// <summary>
    /// NCNNmain.xaml 的交互逻辑
    /// </summary>
    public partial class NCNNmain : Page
    {
        //列表显示
        private ObservableCollection<string> fileList = new ObservableCollection<string>();
        void ShowLog(string msg)
        {
            Debug.AppendText(msg + "\r");
        }

        public NCNNmain()
        {
            InitializeComponent();
            ImgListview.ItemsSource = fileList;
            initNCNNList(model.GAN_Model_Name.NCNN_Arg[0]);
        }

        //初始化
        void initNCNNList(string[] GAN_model_name)
        {
            List<string> models_name = new List<string>();
            foreach (string model in GAN_model_name)
            {
                models_name.Add(model);
            }
            if (models_name != null)
            {
                GAN_models.ItemsSource = models_name;
                GAN_models.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 批量导入文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenImg_clik(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                //点击确认键后开始清除原有文件
                ImgSrc.Source = null;
                GC.Collect();
                //转移文件数据
                List<string> selectedImagePaths = new List<string>(openFileDialog.FileNames);
                foreach (string imagePath in selectedImagePaths)
                {
                    if (!fileList.Contains(imagePath))
                        fileList.Add(imagePath);
                }
            }
        }

        /// <summary>
        /// 开始超分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Begingo_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.Count > 0)
            {
                //清空输出文件夹：
                //model.ToolsUsed.ClearFoderFile(Properties.Settings.Default.DrtFolder);
                //初始化参数
                string model_name = model.GAN_Model_Name.NCNN_Name[ncnnModelSeleced.SelectedIndex];//执行文件名称
                model.RunTerminal run = new model.RunTerminal() { EXEPath = model_name };
                string output_folder = System.IO.Path.GetFullPath(Properties.Settings.Default.DrtFolder);   //输出文件夹
                string models = GAN_models.SelectedItem.ToString().Trim(); //使用的模组的名称
                int counter = 0;
                List<string> args = new List<string>();
                foreach (string file in fileList)
                {
                    string outputs = System.IO.Path.Combine(output_folder, $"{counter}.png");
                    string arg = $"-i \"{file}\" -o \"{outputs}\"";
                    if (models != string.Empty)
                        arg += $" {models}";
                    args.Add(arg);
                    counter++;
                }
                run.BatchStartAndStreamOut(args, Debug);
            }
            else
            {
                ShowLog("没有找到图片文件");
                return;
            }
        }
        //点击并显示图片
        private void ImgListSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (ImgListview.SelectedItem != null)
            {
                string selections = ImgListview.SelectedItem.ToString();
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                //将图片加载到内存中,以防止本地图片资源被占用
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(selections);
                bitmapImage.EndInit();
                ImgSrc.Source = bitmapImage;
                bitmapImage.UriSource = null;
            }
        }

        private void Debug_TextChange(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (Debug.LineCount > 3001)
            {
                Debug.Text = Debug.Text.Substring(Debug.GetLineText(0).Length + 1);
            }
            //跟随滚动
            Debug.ScrollToEnd();
        }
        //在资源管理器中打开输出文件夹
        private void OpenDrtFoder_Click(object sender, RoutedEventArgs e)
        {
            model.ToolsUsed.OpenFolder(Properties.Settings.Default.DrtFolder);
        }

        //选择的超分模型发生变化
        private void ncnnSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (ncnnModelSeleced.SelectedIndex > -1 && GAN_models != null)
            {
                initNCNNList(model.GAN_Model_Name.NCNN_Arg[ncnnModelSeleced.SelectedIndex]);
            }
        }
        //清除文本记录
        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Debug.Text = "";
        }
        //拖拽行为
        private void fileListView_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void fileListView_Drop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // 添加文件路径到列表
            foreach (var file in files)
            {
                string extension = System.IO.Path.GetExtension(file).ToLowerInvariant();
                if (!fileList.Contains(file) && model.GAN_Model_Name.ImgFormat.Contains(extension))
                {
                    fileList.Add(file);
                }
            }
        }
        /// <summary>
        /// 移除当前选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ImgListview.SelectedItem != null)
            {
                string selectedFile = ImgListview.SelectedItem.ToString();
                fileList.Remove(selectedFile);
            }
        }
        /// <summary>
        /// 清除列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListClear_Click(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
        }
    }
}
