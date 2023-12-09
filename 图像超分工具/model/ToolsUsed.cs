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
using Microsoft.Win32;

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

        static public void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", Path.GetFullPath(folderPath));
        }
        /// <summary>
        /// 获取单个文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public static string OpenFile(string title, string filter, bool isFolder = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = title,
                Filter = filter
            };
            if (isFolder)
            {
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "选择文件夹";
            }
            if (openFileDialog.ShowDialog() == true)
            {
                if (isFolder)
                    return Path.GetDirectoryName(openFileDialog.FileName);
                return openFileDialog.FileName;
            }
            return "";
        }
    }

}
