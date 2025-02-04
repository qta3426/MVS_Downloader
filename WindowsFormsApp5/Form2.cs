using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp5
{
    public partial class Form2 : Form
    {
        private string downloadlink;
        private string ppath;
        private string filename;
        string reallink;
        string userpath;
        WebClient webClient;
        Stopwatch sw = new Stopwatch();
        IniFile ini = new IniFile();
        public Form2(string downloadlink, string ppath, string filename)
        {
            InitializeComponent();
            webView21.Source = new Uri("https://my.visualstudio.com");
            this.downloadlink = downloadlink;
            this.ppath = ppath;
            this.filename = filename;
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri(downloadlink);
            string html = await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");
            string shtml = Regex.Unescape(html);
            string[] shtmlsplit = shtml.Split('"');
            reallink = shtmlsplit[10].Replace("amp;", "");
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                Uri URL = new Uri(reallink);

                sw.Start();

                try
                {
                    webClient.DownloadFileAsync(URL, ppath+filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            label3.Text = string.Format("{0} MB/s", (e.BytesReceived / 1024d/1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            progressBar1.Value = e.ProgressPercentage;

            label4.Text = e.ProgressPercentage.ToString() + "%";

            label5.Text = string.Format("{0} MB/s / {1} MB/s",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();

            if (e.Cancelled == true)
            {
                MessageBox.Show("다운로드가 취소 되었습니다.");
            }
            else
            {
                MessageBox.Show("다운로드가 완료 되었습니다.");
                this.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog= new FolderBrowserDialog();
            dialog.ShowDialog();
            userpath=dialog.SelectedPath+"\\";
            ini["MVSSetting"]["Path"] = userpath;
            // 프로그램이 실행되는 경로에 mvs.ini 생성
            ini.Save(Application.StartupPath + @"\mvs.ini");
            ppath = userpath;
        }
    }
}