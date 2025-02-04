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
using Microsoft.Web.WebView2.Core;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;

namespace WindowsFormsApp5
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            webView21.Source = new Uri("https://my.visualstudio.com");
        }
        private static DateTime Delay(int ms)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            inputID();
            Delay(3000);
            inputPW();
            Delay(3000);
            webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('idSIButton9').click()");
            Delay(5000);
            MessageBox.Show("로그인 성공!");
            this.Close();            
        }
        private void inputID()
        {
            string id = "ko.dataFor(document.getElementById('i0116')).usernameTextbox.value('" + textBox1.Text + "');";
            webView21.CoreWebView2.ExecuteScriptAsync(id);
            webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('idSIButton9').click()");
        }
        private void inputPW()
        {
            string pwd = "ko.dataFor(document.getElementById('i0118')).passwordTextbox.value('" + textBox2.Text + "');";
            webView21.CoreWebView2.ExecuteScriptAsync(pwd);
            webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('idSIButton9').click()");
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            
        }
    }
}