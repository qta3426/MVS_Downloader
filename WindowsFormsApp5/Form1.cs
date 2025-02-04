using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.MemoryMappedFiles;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        string path;
        string ppath;
        IniFile ini = new IniFile();
        public Form1()
        {            
            InitializeComponent();    
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form3 outForm = new Form3();
            outForm.Show();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (MessageBox.Show("Visual Studio에서 Visual Studio Dev Essentials 구독 가입이 필요합니다.\n\nVisual Studio Dev Essentials에 가입 하시려면 아니오를 누르십시오.\n\n아니오를 누르면 프로그램이 종료되고 Visual Studio 사이트로 이동 합니다.(로그인 필요)", "MVS Downloader", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                System.Diagnostics.Process.Start("http://my.visualstudio.com");
                this.Close();
            }
            else
            {
                // mvs.ini 파일이 존재할 경우
                string exFile = Application.StartupPath + @"\mvs.ini";
                bool fileExist = File.Exists(exFile);
                if (fileExist)
                {
                    // mvs.ini 파일에 경로만 추가하기
                    path = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads\\";
                    ini["MVSSetting"]["Path"] = path;
                }
                else
                {
                    // 없으면 mvs.ini 파일 만들기
                    // 경로 string
                    path = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads\\";

                    // ini 파일 생성
                    // 다운로드 경로 저장
                    ini["MVSSetting"]["Path"] = path;
                    // 프로그램이 실행되는 경로에 mvs.ini 생성
                    ini.Save(Application.StartupPath + @"\mvs.ini");
                }
                ini.Load(Application.StartupPath + "\\mvs.ini");
                ppath = ini["MVSSetting"]["Path"].ToString();

                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://docs.google.com/spreadsheets/d/e/2PACX-1vStb17vwMPReuTx6lmvdkDuNZGyi2sIHfhNQxEgW5_t738VzCLXQArOXeks7-bqRQS8NTvACVAoOw-L/pub?output=csv");
                StreamReader reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    listBox1.Items.Add(fields[2]);
                }
            }
            this.FormClosed += Form1_Closing;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = listBox1.SelectedItem.ToString();
                string downloadlink = "https://my.visualstudio.com/_apis/Download/GetLink?friendlyFileName=" + listBox1.SelectedItem + "&&upn=&&productId=2168";
                Form2 outForm = new Form2(downloadlink, ppath, filename);
                outForm.Show();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("파일을 선택해주세요.","오류",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = listBox1.SelectedItem.ToString();
                Form4 outForm = new Form4(filename);
                outForm.Show();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Form1_Closing(object sender, FormClosedEventArgs e)
        {
            File.Delete(Application.StartupPath + @"\WindowsFormsApp5.exe.WebView2\\EBWebView\\Default\\Network\\cookies");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form5 outForm= new Form5();
            outForm.Show();
        }
    }
}