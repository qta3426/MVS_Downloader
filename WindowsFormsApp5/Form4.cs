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
        private FileDetailModel _fileDetail;
        public Form4(FileDetailModel fileDetail)
        {
            InitializeComponent();
            _fileDetail = fileDetail;
        }
        private void Form4_Load(object sender, EventArgs e)
        {
            // 파일의 상세 정보를 폼에 표시
            label6.Text = _fileDetail.fileName;
            label7.Text = _fileDetail.fileDescription;
            label8.Text = _fileDetail.languageName;
            label9.Text = _fileDetail.releaseDate.ToString("yyyy-MM-dd");
            label10.Text = _fileDetail.sha256 ?? "정보 없음";
        }
    }

}