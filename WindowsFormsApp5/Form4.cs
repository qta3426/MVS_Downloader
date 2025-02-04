using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace WindowsFormsApp5
{
    public partial class Form4 : Form
    {
        public Form4(string filename)
        {
            InitializeComponent();
            fileinfo result = fileinfo.Searchfileinfo(filename);
            label6.Text = result.category;
            label7.Text = result.Pname;
            label8.Text = result.Fname;
            label9.Text = result.lang;
            label10.Text = result.sha1;
        }
    }
}