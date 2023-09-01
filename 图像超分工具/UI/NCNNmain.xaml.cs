using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        void ShowLog(string msg)
        {
            Debug.AppendText(msg + "\r");
        }

        void Cheked_Foder()
        {
            var files = model.ToolsUsed.Get_Folder(Properties.Settings.Default.SrcFolder);
            List<model.ImgList> img_list = new();
            if (files != null)
            {
                int counter = 0;
                foreach (var file_name in files)
                {
                    //ShowLog(file_name);
                    img_list.Add(new model.ImgList() { ID = counter, Name = System.IO.Path.GetFileName(file_name), ImgPath = file_name });
                    counter++;
                }
                ImgListview.ItemsSource = img_list;
                ImgListview.SelectedIndex = 0;
            }
            else
            {
                ShowLog("路径下没有找到合适的图片文件，请添加jpg或png图片文件后再点击检查按钮。");
            }
        }

        public NCNNmain()
        {
            InitializeComponent();
            Cheked_Foder();
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
                //清除输入文件夹
                model.ToolsUsed.ClearFoderFile(Properties.Settings.Default.SrcFolder);
                //转移文件数据
                List<string> selectedImagePaths = new List<string>(openFileDialog.FileNames);
                List<model.ImgList> img_list = new();
                int counter = 0;
                foreach (string imagePath in selectedImagePaths)
                {
                    string name = System.IO.Path.GetFileName(imagePath);
                    //复制并重命名图片
                    //ShowLog("输入：" + imagePath);
                    string out_putimg = System.IO.Path.Combine(Properties.Settings.Default.SrcFolder, counter.ToString() + System.IO.Path.GetExtension(imagePath));
                    out_putimg = System.IO.Path.GetFullPath(out_putimg);
                    //ShowLog("输出文件：" + out_putimg);
                    model.ToolsUsed.CopyAndRename(imagePath, out_putimg);
                    img_list.Add(new model.ImgList() { ID = counter, Name = name, ImgPath = imagePath });
                    counter++;
                }
                ImgListview.ItemsSource = img_list;
                ImgListview.SelectedIndex = 0;
            }
        }

        //检查文件
        private void CheckList_clik(object sender, RoutedEventArgs e)
        {
            Cheked_Foder();
        }
        /// <summary>
        /// 开始超分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Begingo_Click(object sender, RoutedEventArgs e)
        {
            //获取输入文件夹下的所有文件
            var files = model.ToolsUsed.Get_Folder(Properties.Settings.Default.SrcFolder);
            List<string> files_name = new List<string>();
            if (files != null)
            {
                ShowLog("开始超分。");
                foreach (var file in files)
                {
                    //转化为绝对路径，以确保异步线程路径正确
                    files_name.Add(System.IO.Path.GetFullPath(file));
                }
                //清空输出文件夹：
                model.ToolsUsed.ClearFoderFile(Properties.Settings.Default.DrtFolder);
            }
            else
            {
                ShowLog("没有找到图片文件");
                return;
            }

            if (files_name != null)
            {
                var arg = new model.GAN_Func_Class
                {
                    files = files_name,
                    models = GAN_models.SelectedItem.ToString(),
                    other_arg = OtherArg.Text.Trim(),
                    model_name = model.GAN_Model_Name.NCNN_Name[ncnnModelSeleced.SelectedIndex],
                    output_folder = System.IO.Path.GetFullPath(Properties.Settings.Default.DrtFolder),
                    tb = Debug
                };
                foreach (var file in files)
                {
                    Console.WriteLine("列表：" + file);
                }
                //后台线程
                Thread t = new Thread(model.ToolsUsed.Running_GAN) { IsBackground = true };
                t.Start(arg);
            }
        }
        //点击并显示图片
        private void ImgListSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (ImgListview.SelectedItem != null)
            {
                model.ImgList selections = (model.ImgList)ImgListview.SelectedItem;
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                //将图片加载到内存中,以防止本地图片资源被占用
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(System.IO.Path.GetFullPath(selections.ImgPath));
                bitmapImage.EndInit();
                ImgSrc.Source = bitmapImage;
                bitmapImage.UriSource = null;

                //ShowLog(selections.ImgPath);
            }
        }

        private void Debug_TextChange(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if(Debug.LineCount > 1001)
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
        //在资源管理器中打开输如文件夹
        private void OpenSrcFoder_Click(object sender, RoutedEventArgs e)
        {
            model.ToolsUsed.OpenFolder(Properties.Settings.Default.SrcFolder);
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
    }
}
