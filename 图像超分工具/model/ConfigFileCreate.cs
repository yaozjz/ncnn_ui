using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace 图像超分工具.model
{
    class ConfigFileCreate
    {
        /// <summary>
        /// 文件夹创建函数
        /// </summary>
        /// <param name="path">目标文件你家路径</param>
        /// <returns>创建是否成功</returns>
        public static bool CreateDir(string path)
        {
            //检查文件夹结构
            if (!Directory.Exists(path))
            {
                try
                {
                    //不存在文件夹则创建
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
