using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrologyBorshchForecastCore
{
    public class BotWebBrowser : IBot
    {
        private ILogger theLogger;
        private WebBrowser theBrowser;
        private bool completed;
        public BotWebBrowser(ILogger logger)
        {
            theLogger = logger;
            theBrowser = new WebBrowser();
            completed = false;
        }
        bool IBot.isNew()
        {
            return true;
        }
        void IBot.Parser(DateTime parseDate)
        {
            this.Log("Parser Start");

            //Uri uri = new Uri("http://hydro.meteoinfo.ru/new/login.aspx?ReturnUrl=%2fnew");
            //Uri uri = new Uri("http://hydro.meteoinfo.ru/new/");
            Uri uri = new Uri("http://ya.ru");

            theBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            this.theBrowser.ScriptErrorsSuppressed = true;

            this.theBrowser.Navigate("http://www.google.com");
            while (!completed)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
             
        }
        private void runBrowserThread(Uri url)
        {
            //var th = new Thread(() =>
            //{
            //    var br = new WebBrowser();
            //    br.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            //    br.ScriptErrorsSuppressed = true;
            //    Application.Run();
            //    br.Navigate(url);
                
            //});
            ////th.SetApartmentState(ApartmentState.STA);
            //th.Start();
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Log("browser_DocumentCompleted");
            completed = true;
        }

       
        private void Log(string msg)
        {
            if (theLogger != null)
            {
                theLogger.Log(msg);
            }
        }
    }
}
