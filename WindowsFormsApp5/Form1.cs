using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.MemoryMappedFiles;
using System.Drawing.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text;
using Microsoft.Web.WebView2.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
                MessageBox.Show("파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //File.Delete(Application.StartupPath + @"\WindowsFormsApp5.exe.WebView2\\EBWebView\\Default\\Network\\cookies");
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();

            string queryText = textBox1.Text;

            string script = $@"
(async function() {{
    try {{
        const response = await fetch('https://my.visualstudio.com/_apis/AzureSearch/Search?upn=', {{
            method: 'POST',
            headers: {{
                'Accept': 'application/json;api-version=1.0',
                'User-Agent': 'ELinks (textmode)',
                'Content-Type': 'application/json',
                'Host': 'my.visualstudio.com'
            }},
            mode: 'cors'
            body: JSON.stringify({{
                getAllResults:true,
                searchText:{queryText},
                subscriptionLevel:''
        }});
        const data = await response.json();
        document.body.innerHTML = '<pre>' + JSON.stringify(data, null, 2) + '</pre>';
    }} catch (error) {{
        document.body.innerHTML = '<p>오류 발생: ' + error.message + '</p>';
    }}
}})();";

         string jsonResult = await webView21.CoreWebView2.ExecuteScriptAsync(script);
        }



        // 자바스크립트 실행 후 결과 수신
        //string jsonResult = await webView21.CoreWebView2.ExecuteScriptAsync(script);
        //jsonResult = jsonResult.Trim('"').Replace("\\\"", "\"");

        //if (!string.IsNullOrEmpty(jsonResult))
        //{
        //    try
        //    {
        //        // System.Text.Json으로 파싱
        //        var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResult);

        //        listBox1.Items.Clear();
        //        foreach (var item in resultData)
        //        {
        //            listBox1.Items.Add($"{item.Key}: {item.Value}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"결과 처리 오류: {ex.Message}");
        //    }
        //}
    }
}