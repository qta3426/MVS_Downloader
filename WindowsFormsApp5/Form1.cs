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
        public Dictionary<int, ProductFiles> productFiles;  // productFiles를 클래스 필드로 선언
        private List<FileDetailModel> productFileDetails = new List<FileDetailModel>();
        private CookieContainer _cookieContainer = new CookieContainer();
        private HttpClient _httpClient;
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
        private void button3_Click(object sender, EventArgs e)
        {
        }
        public void Form1_Closing(object sender, FormClosedEventArgs e)
        {
            //File.Delete(Application.StartupPath + @"\WindowsFormsApp5.exe.WebView2\\EBWebView\\Default\\Network\\cookies");
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            await InitializeWebViewAndExtractCookiesAsync();
            await SearchProductsAsync(textBox1.Text);
        }

        private async Task InitializeWebViewAndExtractCookiesAsync()
        {
            try
            {
                string url = "https://my.visualstudio.com";
                await webView21.EnsureCoreWebView2Async();

                if (webView21.CoreWebView2 != null)
                {
                    var cookies = await webView21.CoreWebView2.CookieManager.GetCookiesAsync(url);

                    Uri uri = new Uri(url);
                    _cookieContainer = new CookieContainer();
                    foreach (var cookie in cookies)
                    {
                        _cookieContainer.Add(uri, new Cookie(cookie.Name, cookie.Value));
                    }

                    var handler = new HttpClientHandler { CookieContainer = _cookieContainer };
                    _httpClient = new HttpClient(handler)
                    {
                        DefaultRequestHeaders =
                {
                    { "Accept", "application/json;api-version=1.0" },
                    { "Host", "my.visualstudio.com" },
                    { "User-Agent", "ELinks (textmode)" }
                }
                    };
                }
                else
                {
                    MessageBox.Show("WebView2가 초기화되지 않았습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"쿠키 초기화 중 오류 발생: {ex.Message}");
            }
        }

        private async Task SearchProductsAsync(string searchText)
        {
            try
            {
                string apiUrl = "https://my.visualstudio.com/_apis/AzureSearch/Search?upn=";
                string body = $"{{ \"getAllResults\": true, \"searchText\": \"{searchText}\", \"subscriptionLevel\": \"\" }}";

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    searchResult = JsonSerializer.Deserialize<SearchResult>(responseData);

                    listView1.Items.Clear();
                    listView1.Columns.Clear();
                    listView1.Columns.Add("Product ID", 80);
                    listView1.Columns.Add("Product Name", 500);
                    listView1.FullRowSelect = true;
                    listView1.View = View.Details;

                    foreach (var product in searchResult.searchResultsGroupByProduct)
                    {
                        var item = new ListViewItem(product.productId.ToString());
                        item.SubItems.Add(product.productName);
                        listView1.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show($"요청 실패: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"제품 검색 중 오류 발생: {ex.Message}");
            }
        }

        private async Task GetFilesForProductId(int productId)
        {
            try
            {
                string apiUrl = "https://my.visualstudio.com/_apis/AzureSearch/GetfilesForListOfProducts?upn=&mkt=";
                string jsonBody = $"[{productId}]";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var parsedResponse = JsonSerializer.Deserialize<FilesForProductsResponse>(responseData);

                    listBox2.Items.Clear();

                    if (parsedResponse?.filesForProducts != null &&
                        parsedResponse.filesForProducts.TryGetValue(productId.ToString(), out var productFiles) &&
                        productFiles?.fileDetailModels != null)
                    {
                        foreach (var fileDetail in productFiles.fileDetailModels)
                        {
                            listBox2.Items.Add(fileDetail.fileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("해당 ProductId에 파일 정보가 없습니다.");
                    }
                }
                else
                {
                    MessageBox.Show($"요청 실패: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 가져오기 중 오류 발생: {ex.Message}");
            }
        }

        private async void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                var selectedItem = listView1.SelectedItems[0];

                if (int.TryParse(selectedItem.Text, out int productId))
                {
                    await GetFilesForProductId(productId);
                }
                else
                {
                    MessageBox.Show("올바른 Product ID가 아닙니다.");
                }
            }
        }
    }
}