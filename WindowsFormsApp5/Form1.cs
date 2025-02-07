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
        private SearchResult searchResult;
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
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string filename = listBox1.SelectedItem.ToString();
        //        string downloadlink = "https://my.visualstudio.com/_apis/Download/GetLink?friendlyFileName=" + listBox1.SelectedItem + "&&upn=&&productId=2168";
        //        Form2 outForm = new Form2(downloadlink, ppath, filename);
        //        outForm.Show();
        //    }
        //    catch (NullReferenceException)
        //    {
        //        MessageBox.Show("파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string filename = listBox1.SelectedItem.ToString();
        //        Form4 outForm = new Form4(filename);
        //        outForm.Show();
        //    }
        //    catch (NullReferenceException)
        //    {
        //        MessageBox.Show("파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        public void Form1_Closing(object sender, FormClosedEventArgs e)
        {
            //File.Delete(Application.StartupPath + @"\WindowsFormsApp5.exe.WebView2\\EBWebView\\Default\\Network\\cookies");
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            string url = "https://my.visualstudio.com";  // 쿠키를 가져올 URL
            Uri uri = null;

            try
            {
                // WebView2 초기화가 완료될 때까지 대기
                await webView21.EnsureCoreWebView2Async();

                uri = new Uri(url); // string을 Uri 객체로 변환
                string urlString = uri.ToString(); // Uri를 string으로 변환

                // WebView2가 초기화되었으므로 쿠키 가져오기
                if (webView21.CoreWebView2 != null)
                {
                    // Uri 대신 string으로 쿠키를 가져오기
                    var cookies = await webView21.CoreWebView2.CookieManager.GetCookiesAsync(urlString);

                    // 쿠키를 HttpClient용으로 변환
                    var cookieContainer = new CookieContainer();
                    foreach (var cookie in cookies)
                    {
                        cookieContainer.Add(uri, new Cookie(cookie.Name, cookie.Value));
                    }

                    // HttpClientHandler에 쿠키를 설정
                    var handler = new HttpClientHandler();
                    handler.CookieContainer = cookieContainer;

                    var client = new HttpClient(handler);

                    client.DefaultRequestHeaders.Add("Accept", "application/json;api-version=1.0");
                    client.DefaultRequestHeaders.Add("Host", "my.visualstudio.com");
                    client.DefaultRequestHeaders.Add("User-Agent","ELinks (textmode)");

                    string apiUrl = "https://my.visualstudio.com/_apis/AzureSearch/Search?upn=";
                    string body = $"{{ \"getAllResults\": true, \"searchText\": \"{textBox1.Text}\", \"subscriptionLevel\": \"\" }}";

                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    // POST 요청 보내기
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        // JSON 파싱
                        var searchResult = JsonSerializer.Deserialize<SearchResult>(responseData);

                        // ListBox에 기존 항목 지우기
                        listView1.Items.Clear();
                        listView1.Columns.Clear();
                        listView1.Columns.Add("Product ID", 80);
                        listView1.Columns.Add("Product Name", 500);
                        listView1.FullRowSelect = true;
                        listView1.View = View.Details;

                        // searchResultsGroupByProduct 데이터를 ListBox에 추가
                        foreach (var product in searchResult.searchResultsGroupByProduct)
                        {
                            var item = new ListViewItem(product.productId.ToString());  // 첫 번째 열: ProductId
                            item.SubItems.Add(product.productName);  // 두 번째 열: ProductName

                            listView1.Items.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"요청 실패: {response.StatusCode}");
                    }
                }
                else
                {
                    MessageBox.Show("WebView2가 초기화되지 않았습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }

        private async void listView1_SelectedIndexChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // 선택된 항목이 있을 때만 작업 수행
            if (e.IsSelected)
            {
                // 선택된 항목의 첫 번째 열(ProductId)을 가져옴
                int productId = int.Parse(e.Item.Text); // 첫 번째 열에 ProductId가 있음

                // API 호출을 통해 해당 productId에 대한 파일 정보 가져오기
                await GetFilesForProductId(productId);
            }
        }

        private async Task GetFilesForProductId(int productId)
        {
            string apiUrl = $"https://my.visualstudio.com/_apis/AzureSearch/GetfilesForListOfProducts?upn=&mkt=";

            // 요청 본문에 productId 포함
            var requestBody = new { productIds = new[] { productId } };
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        // 파일 이름들 파싱
                        var fileNames = JsonSerializer.Deserialize<List<string>>(responseData);

                        // listBox2에 기존 항목 제거
                        listBox2.Items.Clear();

                        // 파일 이름을 listBox2에 추가
                        foreach (var fileName in fileNames)
                        {
                            listBox2.Items.Add(fileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"요청 실패: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}");
                }
            }
        }


    }
}