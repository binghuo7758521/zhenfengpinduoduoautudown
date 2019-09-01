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
        int sumnum=0;
        int lianxucuo = 0;//连续失败次数
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
                downdir = "d:\\autodown\\";
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
            myWebClient.Dispose();

            return strHTML;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            timer1.Enabled = true;

        }

        void client_downok(object sender, AsyncCompletedEventArgs e)
        {
            timer1.Enabled = true;
            this.progressBar1.Value = 0;
            sumnum++;
            string aa = sumnum.ToString();
            textBox2.AppendText(aa + "下载完成." + "\r\n");
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = (int)e.TotalBytesToReceive;
            this.progressBar1.Value = (int)e.BytesReceived;
            long i= e.TotalBytesToReceive / 1024 / 1024; 

            this.label2.Text =i.ToString()+"M"; 
             
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string htmtxt="";
            
            string myfilename = "";
           
          
            
            
            timer1.Enabled = false;
            textBox2.AppendText("1.0正在读取服务器信息...." + "\r\n");
            htmtxt = GetWebClient("http://ct.ahjcg.com/auto_down.php");
            htmtxt = htmtxt.Replace("开始zipdownval:", "");
            int i=htmtxt.IndexOf("zip_ok:<");
            int ii=htmtxt.IndexOf(".zip>");
            if (htmtxt.IndexOf("无法打开文件") > 0 || i<1 ||  ii<1) {
                textBox3.AppendText("文件下载失败，请检查服务器后台...." + "\r\n");
                textBox3.AppendText(htmtxt + "\r\n");
                lianxucuo ++;

                if (lianxucuo > 10)
                {
                    timer1.Enabled = false;
                    button1.Enabled = true;
                    textBox2.AppendText("连续出错超过10次，程序停止运行...." + "\r\n");

                }
                else
                {
                    timer1.Enabled = true;
                }
                return;
               
            }


            lianxucuo = 0;

            string zipfile = htmtxt.Substring(i+8,ii-i-4 );
           htmtxt= htmtxt.Substring(0, htmtxt.IndexOf("zip_ok:"));
           htmtxt= htmtxt.Replace("val:","|");
          


            //////
           string[] str2 = zipfile.Split('/');
           myfilename = str2[5];
           textBox2.AppendText("正在压缩并下载... . ." + myfilename + "\r\n");
           WebClient client = new WebClient();
           client.DownloadFileCompleted += client_downok;
           client.DownloadProgressChanged += client_DownloadProgressChanged;            
           client.DownloadFileAsync(new Uri(zipfile), downdir + myfilename);
           client.Dispose();
             
           
           
           //timer1.Enabled = true;

           
        }
    }
}
