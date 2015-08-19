using HydrologyBorshchForecastEntity.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;

namespace HydrologyBorshchForecastCore
{
    public class BotReservoir : IBot
    {
        private ILogger theLogger;
        private DateTime parseDate;
        public bool isNew;
        public BotReservoir(ILogger logger)
        {
            isNew = false;
            theLogger = logger;
        }
        void IBot.Parser(DateTime parseDate)
        {
            this.parseDate = parseDate;
            this.Log("Parser Start");

            Uri uri = new Uri("http://hydro.meteoinfo.ru/new/login.aspx?ReturnUrl=%2fnew");
            
            runBrowserThread(uri);

             
        }
        private void runBrowserThread(Uri url)
        {
            var th = new Thread(() =>
            {
                var br = new WebBrowser();
                br.DocumentCompleted += browser_DocumentCompleted;
                br.ScriptErrorsSuppressed = true;
                br.Navigate(url);
                Application.Run();                
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                Log(e.Url.ToString());
                Log(br.Document.Body.OuterText);
                try
                {
                    HtmlElement logBut = br.Document.GetElementById("Login1_login");
                    br.Document.GetElementById("Login1_UserName").InnerText = "user";
                    br.Document.GetElementById("Login1_Password").InnerText = "amur";
                    br.DocumentCompleted -= browser_DocumentCompleted;
                    br.DocumentCompleted += browser_LoginCompleted;
                    logBut.InvokeMember("Click");
                    Log("Natigated to "+ e.Url);
       // Application.ExitThread();   // Stops the thread

                }
                catch(Exception ex)
                {
                    Log(ex.Message);
                }

            }
        }

        void browser_LoginCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                string dateText = this.parseDate.ToString("yyyy-MM-dd");
                Uri uri = new Uri("http://hydro.meteoinfo.ru/new/proxy/proxy.ashx?http://hydro.meteoinfo.ru/arcgis/rest/services/project/oper_tables/MapServer/1/query?f=json&where=dateTimeData%20%3D%20%27" + dateText + "%2000%3A00%3A00%27&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=MeteoModel%2CInflow_obs_WB%2CInflow_obs_HM%2CInflow_for1%2CInflow_for2%2CInflow_for3%2CInflow_for4%2CInflow_for5");
                
                Log("Natigated to " + uri.ToString());
                br.DocumentCompleted -= browser_LoginCompleted;
                br.DocumentCompleted += browser_ReservoirCompleted;
                br.Navigate(uri);
            }
        }

       
        void browser_ReservoirCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                Log(br.Document.Body.OuterText);
                BorshchForecastReservoir(br.Document.Body.OuterText);
                Application.ExitThread();   // Stops the thread
            }
        }

   
        void BorshchForecastReservoir(string strJson)
        {
            try
            {
                JObject objJson = JObject.Parse(strJson);
                IList<JToken> tokenList = objJson["features"].Children().ToList();
                foreach (var item in tokenList)
                {

                    string MeteoModel = item["attributes"]["MeteoModel"].ToString();
                    Reservoir theReservoir = Reservoir.GetByDate(this.parseDate.Date, MeteoModel);
                    if (theReservoir == null)
                    {
                        theReservoir = new Reservoir();
                    }
                    theReservoir.forecasted_at = this.parseDate.Date;
                    theReservoir.MeteoModel = item["attributes"]["MeteoModel"].ToString();

                    if (item["attributes"]["Inflow_for1"].ToString() != "")
                    {
                        theReservoir.Inflow_for1 = Convert.ToDouble(item["attributes"]["Inflow_for1"].ToString());
                    }

                    if (item["attributes"]["Inflow_for2"].ToString() != "")
                    {
                        theReservoir.Inflow_for2 = Convert.ToDouble(item["attributes"]["Inflow_for2"].ToString());
                    }

                    if (item["attributes"]["Inflow_for3"].ToString() != "")
                    {
                        theReservoir.Inflow_for3 = Convert.ToDouble(item["attributes"]["Inflow_for3"].ToString());
                    }

                    if (item["attributes"]["Inflow_for4"].ToString() != "")
                    {
                        theReservoir.Inflow_for4 = Convert.ToDouble(item["attributes"]["Inflow_for4"].ToString());
                    }

                    if (item["attributes"]["Inflow_for5"].ToString() != "")
                    {
                        theReservoir.Inflow_for5 = Convert.ToDouble(item["attributes"]["Inflow_for5"].ToString());
                    }

                    if (item["attributes"]["Inflow_obs_WB"].ToString() != "")
                    {
                        theReservoir.Inflow_obs_WB = Convert.ToDouble(item["attributes"]["Inflow_obs_WB"].ToString());
                    }

                    if (item["attributes"]["Inflow_obs_HM"].ToString() != "")
                    {
                        theReservoir.Inflow_obs_HM = Convert.ToDouble(item["attributes"]["Inflow_obs_HM"].ToString());
                    }
                    if (theReservoir.ID > 0)
                    {
                        theReservoir.Update();
                    }
                    else
                    {
                        isNew = true;
                        theReservoir.Save();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        bool IBot.isNew()
        {
            return isNew;
        }
        private void Log(string msg)
        {
            if (theLogger.ToString() != "")
            {
                theLogger.Log(msg);
            }
        }
    }
}
