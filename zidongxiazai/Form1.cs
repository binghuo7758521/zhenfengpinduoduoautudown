using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Net;

namespace zidongxiazai
{
    public partial class Form1 : Form

    {
        string downdir;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            downdir = ConfigurationManager.AppSettings["downdir"];
            if (downdir==null)
            {
                downdir = "e:\\autodown\\";
            }
            if (!Directory.Exists(downdir))
            {
                Directory.CreateDirectory(downdir);
            }
            textBox1.Text = downdir;
        }
        private string GetWebClient(string url)
        {

            string strHTML = "";

            WebClient myWebClient = new WebClient();

            Stream myStream = myWebClient.OpenRead(url);

            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));

            strHTML = sr.ReadToEnd();

            myStream.Close();

            return strHTML;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string htmtxt="";
            
            string myfilename = "";
           
            WebClient client = new WebClient();
            
            timer1.Enabled = false;
            textBox2.AppendText("1.0正在读取服务器信息...." + "\r\n");
            htmtxt = GetWebClient("http://ct.ahjcg.com/auto_down.php");
            if (htmtxt.IndexOf("无法打开文件") > 0) {
                textBox3.AppendText("文件下载失败，请检查服务器后台...." + "\r\n");
                textBox3.AppendText(htmtxt + "\r\n");
                timer1.Enabled = true;
                return;
            }
            htmtxt = htmtxt.Replace("开始zipdownval:", "");
            int i=htmtxt.IndexOf("zip_ok:<");
            int ii=htmtxt.IndexOf(".zip>");
            string zipfile = htmtxt.Substring(i+8,ii-i-4 );
           htmtxt= htmtxt.Substring(0, htmtxt.IndexOf("zip_ok:"));
           htmtxt= htmtxt.Replace("val:","|");
          


            //////
           string[] str2 = zipfile.Split('/');
           myfilename = str2[5];
           textBox2.AppendText("正在压缩并下载... . ." + myfilename + "\r\n");
           client.DownloadFile(zipfile, downdir + myfilename);
           textBox2.AppendText("下载完成." + "\r\n");
           timer1.Enabled = true;

           
        }
    }
}
