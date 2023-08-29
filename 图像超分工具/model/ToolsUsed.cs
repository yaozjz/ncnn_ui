using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Interop;

namespace 图像超分工具.model
{
    class ToolsUsed
    {
        static public IEnumerable<string>? Get_Folder(string FilePath)
        {
            if (Directory.Exists(FilePath))
            {

                var imageFile = Directory.GetFiles(FilePath).Where(file => Regex.IsMatch(file, @"^.+\.(png|jpg)$"));
                return imageFile;
            }
            else
            {
                MessageBox.Show("文件夹不存在!");
                return null;
            }
        }
        /// <summary>
        /// 清除文件夹下的所有文件
        /// </summary>
        /// <param name="FilePath"></param>
        static public bool ClearFoderFile(string folderPath)
        {
            try
            {
                // 检查文件夹是否存在
                if (Directory.Exists(folderPath))
                {
                    // 获取所有文件
                    string[] files = Directory.GetFiles(folderPath);

                    // 删除
                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        File.Delete(file);
                        Console.WriteLine($"Deleted file: {file}");
                    }

                    Console.WriteLine("文件夹删除成功。");
                    return true;
                }
                else
                {
                    Console.WriteLine("不存在目标文件夹！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误：" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 复制并重命名文件到输入文件夹
        /// </summary>
        /// <param name="input_path">输入的图片</param>
        /// <param name="output_path">输出的图片路径</param>
        /// <returns></returns>
        static public bool CopyAndRename(string input_path, string output_path)
        {
            try
            {
                string output_foder = System.IO.Path.GetDirectoryName(output_path);
                string filename = Path.GetFileName(input_path);
                File.Copy(input_path, Path.Combine(output_foder, filename), true);
                File.Move(System.IO.Path.Combine(output_foder, filename), output_path);

                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 开始超分
        /// </summary>
        /// <param name="model_name">超分工具名称</param>
        /// <param name="files">输入文件名</param>
        /// <param name="outputs">输出文件名</param>
        /// <param name="model">模型</param>
        /// <returns></returns>
        static public void Running_GAN(object arr)
        {
            var msg = arr as model.GAN_Func_Class;
            List<string> files = msg.files;
            foreach (string file in files)
            {
                try
                {
                    string file_name = Path.GetFileName(file);
                    string model_name = msg.model_name;
                    string inputs = file;
                    string outputs = Path.Combine(msg.output_folder, file_name);
                    Process p = new();
                    p.StartInfo.FileName = model_name;
                    string arg = string.Format("-i {0} -o {1}", inputs, outputs);
                    if (msg.other_arg != "")
                    {
                        arg += string.Format(" {0}", msg.other_arg);
                    }
                    if (msg.models != null)
                    {
                        arg += string.Format(" {0}", msg.models);
                    }
                    p.StartInfo.Arguments = arg;
                    //是否开启Dot窗口
                    if (!Properties.Settings.Default.ShowDot)
                    {
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                    }
                    p.Start();
                    string output = "";
                    if (!Properties.Settings.Default.ShowDot)
                    {
                        output = p.StandardOutput.ReadToEnd();
                    }
                    p.WaitForExit();
                    p.Close();

                    Console.WriteLine(output);

                    Application.Current.Dispatcher.InvokeAsync(delegate { msg.tb.AppendText(file_name + " 已完成超分。\r"); });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        msg.tb.AppendText("错误：" + ex.Message + "\r");
                    });
                }
            }
            Application.Current.Dispatcher.InvokeAsync(delegate { msg.tb.AppendText("全部超分完毕!\r"); });
        }

        static public void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", Path.GetFullPath(folderPath));
        }
    }

}
