using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrologyBorshchForecastForm
{
    public partial class HydrologyBorshchForecastForm : Form
    {
        public HydrologyBorshchForecastForm()
        {
            InitializeComponent();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Uri uri = new Uri("http://hydro.meteoinfo.ru/new/login.aspx?ReturnUrl=%2fnew");
            webBrowser1.Navigate(uri);
            
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                try
                {
                    HtmlElement logBut = br.Document.GetElementById("Login1_login");
                    br.Document.GetElementById("Login1_UserName").InnerText = "user";
                    br.Document.GetElementById("Login1_Password").InnerText = "amur";
                   // br.DocumentCompleted -= browser_DocumentCompleted;
                  //  br.DocumentCompleted += browser_RequestCompleted;
                    logBut.InvokeMember("Click");
                    // Application.ExitThread();   // Stops the thread

                }
                catch 
                {
                    Application.ExitThread();
                }

            }
        }

        void browser_LoginCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        void browser_RequestCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
           //     Application.ExitThread();   // Stops the thread
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1_Tick(null, null);
        }
    }
}
