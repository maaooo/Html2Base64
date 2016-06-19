using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Html2Base64
{
    public class fileData
    {
        public fileData(string path)
        {
            filePath = path;
            ExtName = Path.GetExtension(path).ToLower();
            findPath = path.Substring(Program.SourceDir.Length+1, path.Length- Program.SourceDir.Length-1);
            replaceFXG();
            replaceText = Program.ReadFileforUtf8(path);

          //  Console.WriteLine(findPath + "--->" + replaceText) ;
        }
        public string filename;
        public string ExtName;
        public string filePath;
        public string findPath;
        public string replaceText;
        
        public void  replaceFXG()
        {
            findPath=Program.myReplace(findPath, "\\", "/");
        }
    }


    public class Program
    {
        static List<fileData> fileDataLsit=new List<fileData>();

        static string MainhtmlStr;
        static void Main(string[] args)
        {
            // var str = ReadRootHtml("C:\\PC_Fanke\\login.html");C:\PC_Fanke\login.html
            //  Console.WriteLine(str);
            //  var test=str.IndexOf("星云鼠标跟随");
            // Console.WriteLine("\n");
            //    Console.WriteLine(test);


            Console.WriteLine("请输入html文件路径。。。");
          tag1:
            string srtpath = Console.ReadLine();
            if (!File.Exists(srtpath))
            {
                Console.WriteLine("文件错误-->不是html文件,请输入正确的文件路径：");
                goto tag1;
            }
            MainhtmlStr = ReadFileforGb2312(srtpath);//用于查找
            SourceDir =Path.GetDirectoryName(srtpath);
            try
            {
                ListFiles(new DirectoryInfo(SourceDir));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            for(int i=0;i< fileDataLsit.Count;i++)
            {
                string base64 = string2Base64(MainhtmlStr, fileDataLsit[i].ExtName);
                string test = myReplace(MainhtmlStr, fileDataLsit[i].findPath, base64);//忽略大小写的替换函数
                Console.WriteLine(test);
            }
            
            
            //var htmlStr = ReadRootHtml(srtpath);
        }

        public static string ReadFileforUtf8(string path)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(path); 
            System.IO.StreamReader br = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            string str = br.ReadToEnd();
            byte[] b = Encoding.Default.GetBytes(str);
            
           // string str = System.Text.Encoding.Default.GetString(bt);
            br.Close();
            fs.Close();
           // byte[] src = Encoding.UTF8.GetBytes(str);
            //byte[] des = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), src);
            
            return Convert.ToBase64String(b);
        }
        public static string ReadFileforGb2312(string path)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(path);
            System.IO.StreamReader br = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            string str = br.ReadToEnd();
            //string str = System.Text.Encoding.Default.GetString(bt);
            br.Close();
            fs.Close();
            byte[] src = Encoding.UTF8.GetBytes(str);
            byte[] des = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), src);

            return System.Text.Encoding.ASCII.GetString(des);
        }
        public static string TargetDir = "c:\\base64_file";
        public static string SourceDir = "";

        static void CreateTargetDir()
        {
            if(Directory.Exists(TargetDir))
                    Directory.CreateDirectory(TargetDir);
        }
 

        static string string2Base64(string str ,string fileExtName)
        {
            //System.IO.FileStream fs = System.IO.File.OpenRead(path);
            //System.IO.BinaryReader br = new BinaryReader(fs);
            //byte[] bt = br.ReadBytes(Convert.ToInt32(fs.Length));
            string base64String="";
            if (fileExtName.Equals(".css"))
            {
                base64String = "data:text/css;base64," + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(str));
            }
            if (fileExtName.Equals(".js"))
            {
                base64String = "data:text/javascript;base64," + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(str));
            }
            if(base64String.Equals(""))
                Console.WriteLine("警告--->string2Base64f 返回空");
            return base64String;
            //br.Close();
            //fs.Close();
           // return base64String;
        }
        /// <summary>
        /// 过滤文件类型
        /// </summary>
        /// <param name="filepath"></param>
        public static void FilterFile(string filepath)
        {
            string str=  Path.GetExtension(filepath).ToLower();
            if(str.Equals(".css"))
            {
                var fd = new fileData(filepath);
                fileDataLsit.Add(fd);
                //Console.WriteLine(str);
            }
           
        }
        /// <summary>
        /// 忽略大小写的替换函数
        /// </summary>
        /// <param name="strSource">源文本</param>
        /// <param name="strRe">在文本中要替换的内容</param>
        /// <param name="strTo">替换值</param>
        /// <returns></returns>
        public static string myReplace(string strSource, string strRe, string strTo)
        {
            string strSl, strRl;
            strSl = strSource.ToLower();
            strRl = strRe.ToLower();
            int start = strSl.IndexOf(strRl);
            if (start != -1)
            {
                strSource = strSource.Substring(0, start) + strTo
                + myReplace(strSource.Substring(start + strRe.Length), strRe, strTo);
            }
            return strSource;
        }

        /// <summary>
        /// 递归目录所有文件
        /// </summary>
        /// <param name="info"></param>
        public static void ListFiles(FileSystemInfo info)
        {
            if (!info.Exists) return;
            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录   
            if (dir == null) return;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件   
                if (file != null)
                {
                    FilterFile(file.FullName);
                }
                    
                //对于子目录，进行递归调用   
                else
                    ListFiles(files[i]);
            }
        }
    }
}
