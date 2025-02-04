using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class fileinfo
    {
        public string category { get; set; }
        public string Pname { get; set; }
        public string Fname { get; set; }
        public string lang { get; set; }
        public string sha1 { get; set; }
        public fileinfo(string category, string Pname, string Fname, string lang, string sha1)
        {
            this.category = category;
            this.Pname = Pname;
            this.Fname = Fname;
            this.lang = lang;
            this.sha1 = sha1;
        }
        static List<fileinfo> GetfileinfoList()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://docs.google.com/spreadsheets/d/e/2PACX-1vStb17vwMPReuTx6lmvdkDuNZGyi2sIHfhNQxEgW5_t738VzCLXQArOXeks7-bqRQS8NTvACVAoOw-L/pub?output=csv");
            StreamReader reader = new StreamReader(stream);

            List<fileinfo> fileinfos = new List<fileinfo>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fields = line.Split(',');
                // 차레대로 카테고리, 제품 이름, 파일 이름, 언어, sha1값
                fileinfos.Add(new fileinfo(fields[0], fields[1], fields[2], fields[3], fields[4]));                
            }
            return fileinfos;
        }
        static public fileinfo Searchfileinfo(string filename)
        {
            List<fileinfo> fileinfos = GetfileinfoList();
            fileinfo result = fileinfos.Find(fi => fi.Fname == filename);
            return result;
        }
    }
}