using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            webView21.Source = new Uri("https://docs.google.com/spreadsheets/d/1vbyvNm7iPtgp4HnIDr4ZHRVx0cuYxUk6V4i_STXhUQg/edit?usp=sharing");
        }
    }
}
