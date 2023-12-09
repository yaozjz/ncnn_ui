using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace 图像超分工具.model
{
    class GAN_Model_Name
    {
        static public string[] NCNN_Name = { "./tools/realesrgan-ncnn-vulkan/realesrgan-ncnn-vulkan.exe", "./tools/realcugan-ncnn-vulkan/realcugan-ncnn-vulkan.exe" };
        //对应模组后缀
        static private string[] RealESRGAN = { "-n realesrgan-x4plus-anime", "-n realesrgan-x4plus" };
        static private string[] RealCURAN = { "-s 2 -n 1 -x #放大二倍", "-s 3 -n 3 -x #放大三倍", "-s 4 -n 3 -x #放大四倍", "-s 2", "-s 3", "-s 4"};

        static public List<string[]> NCNN_Arg = new List<string[]>() { RealESRGAN, RealCURAN };

        static public List<string> ImgFormat = new List<string>() { ".png", ".jpg" };
    }

    /// <summary>
    /// 超分参数类
    /// </summary>
    class GAN_Func_Class
    {
        //所使用的超分工具名称
        public string model_name { get; set; }
        //输入文件列表
        public List<string> files { get; set; }
        //输出目录
        public string output_folder { get; set; }
        //附随参数
        public string models { get; set; }
        //其他附随参数
        public string other_arg { get; set; }

        public TextBox tb { get; set; }
    }
}
