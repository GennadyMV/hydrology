using HydrologyBorshchForecastCore;
using HydrologyBorshchForecastEntity.Models;
using HydrologyCore.Agk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrologyConsole
{
    public static class MessageLoopWorker
    {
        public static async Task<object> Run(Func<object[], Task<object>> worker, params object[] args)
        {
            var tcs = new TaskCompletionSource<object>();

            var thread = new Thread(() =>
            {
                EventHandler idleHandler = null;

                idleHandler = async (s, e) =>
                {
                    // handle Application.Idle just once
                    Application.Idle -= idleHandler;

                    // return to the message loop
                    await Task.Yield();

                    // and continue asynchronously
                    // propogate the result or exception
                    try
                    {
                        var result = await worker(args);
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }

                    // signal to exit the message loop
                    // Application.Run will exit at this point
                    Application.ExitThread();
                };

                // handle Application.Idle just once
                // to make sure we're inside the message loop
                // and SynchronizationContext has been correctly installed
                Application.Idle += idleHandler;
                Application.Run();
            });

            // set STA model for the new thread
            thread.SetApartmentState(ApartmentState.STA);

            // start the thread and await for the task
            thread.Start();
            try
            {
                return await tcs.Task;
            }
            finally
            {
                thread.Join();
            }
        }
    }
    class Program
    {
        static public DateTime parseDate;
        static void Main(string[] args)
        {
            parseDate = DateTime.Now;
           // ExportToExcel();
           // UpdateSchema();
          //  Support03();
           // Support02();
           // Support01();
            Support04();
            Console.WriteLine("Ok");
            Console.ReadKey();
        }

        static void Support04()
        {
            HydrologyCore.Logger.ILogger theLogger = new HydrologyCore.Logger.LoggerNLog();
            IExport theExport = new ExportFromHydroService(theLogger);
            theExport.Run();
        }
        static void ExportToExcel()
        {
            Microsoft.Office.Interop.Excel.Workbook MyBook = null; 
            Microsoft.Office.Interop.Excel.Application MyApp = null;
            Microsoft.Office.Interop.Excel.Worksheet MySheet = null;

            MyApp = new Microsoft.Office.Interop.Excel.Application();
            MyApp.Visible = false;

            string DB_PATH = "C:\\test.xls";

            MyBook = MyApp.Workbooks.Add(); //Open(DB_PATH);
            MySheet = (Microsoft.Office.Interop.Excel.Worksheet)MyBook.Worksheets.get_Item(2);
            MySheet.Cells[1, 1] = "Sheet 2 content";
           // MySheet = (Microsoft.Office.Interop.Excel.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
           

            MyBook.SaveAs(DB_PATH);

        }

        
        static void UpdateSchema()
        {
            
            HydrologyBorshchForecastEntity.Common.NHibernateHelper.UpdateSchema();
        
        }
        static void Support03()
        {
            try
            {
                // download each page and dump the content
                var task = MessageLoopWorker.Run(DoWorkAsync,
                    "http://hydro.meteoinfo.ru/new/login.aspx?ReturnUrl=%2fnew");
                task.Wait();
                Console.WriteLine("DoWorkAsync completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DoWorkAsync failed: " + ex.Message);
            }

            Console.WriteLine("Press Enter to exit.");
        }
        static async Task<object> DoWorkAsync(object[] args)
        {
            string STATUS = "Loading";
            Console.WriteLine("Start working.");

            using (var wb = new WebBrowser())
            {
               // wb.ScriptErrorsSuppressed = true;

                TaskCompletionSource<bool> tcs = null;
                WebBrowserDocumentCompletedEventHandler documentCompletedHandler = (s, e) =>
                    {

                        var br = s as WebBrowser;
                        switch (STATUS)
                        {
                            case "Loading":
                                STATUS = "Logining";
                                HtmlElement logBut = br.Document.GetElementById("Login1_login");
                                br.Document.GetElementById("Login1_UserName").InnerText = "user";
                                br.Document.GetElementById("Login1_Password").InnerText = "amur";
                                logBut.InvokeMember("Click");
                                break;
                            case "Logining":
                                STATUS = "River";
                                if (br.Url == e.Url)
                                {                
                                    string dateText = parseDate.ToString("yyyy-MM-dd");
                                    Uri uri = new Uri("http://hydro.meteoinfo.ru/new/proxy/proxy.ashx?http://hydro.meteoinfo.ru/arcgis/rest/services/project/oper_tables/MapServer/0/query?f=json&where=dateTimeProd%20%3D%20%27" + dateText + "%2008%3A00%3A00%27&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=HydroPostCode%2CGauge%2CFloodPlaneMark%2CAdverseFact%2CDangerFact%2CLevel_obs%2CLevel_for1%2CLevel_for2%2CLevel_for3%2CLevel_for4%2CLevel_for5%2CLevel_for6%2CHeight");
                                    Log("Natigated to " + uri.ToString());
                                    br.Navigate(uri);
                                }
                                break;
                            case "River":
                                STATUS = "Reservoir";
                                if (br.Url == e.Url)
                                {
                                    string dateText = parseDate.ToString("yyyy-MM-dd");
                                    Uri uri = new Uri("http://hydro.meteoinfo.ru/new/proxy/proxy.ashx?http://hydro.meteoinfo.ru/arcgis/rest/services/project/oper_tables/MapServer/1/query?f=json&where=dateTimeData%20%3D%20%27" + dateText + "%2000%3A00%3A00%27&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=MeteoModel%2CInflow_obs_WB%2CInflow_obs_HM%2CInflow_for1%2CInflow_for2%2CInflow_for3%2CInflow_for4%2CInflow_for5");

                                    Log("Natigated to " + uri.ToString());

                                    Log(br.Document.Body.OuterText);
                                    BorshchForecastRiver(br.Document.Body.OuterText);;
                                    br.Navigate(uri);
                                }
                                break;
                            case "Reservoir":
                                if (br.Url == e.Url)
                                {
                                    Log(br.Document.Body.OuterText);
                                    BorshchForecastReservoir(br.Document.Body.OuterText);
                                    //Application.ExitThread();   // Stops the thread
                                    tcs.TrySetResult(true);

                                }
                                break;
                            default:
                                 tcs.TrySetResult(true);
                                break;
                        }
                    };
                WebBrowserDocumentCompletedEventHandler documentRiverHandle = (s, e) =>
                    {

                    };
                WebBrowserDocumentCompletedEventHandler documentLoginHandler = (s, e) =>
                    {

                        var br = s as WebBrowser;

                        HtmlElement logBut = br.Document.GetElementById("Login1_login");
                        br.Document.GetElementById("Login1_UserName").InnerText = "user";
                        br.Document.GetElementById("Login1_Password").InnerText = "amur";
                        //br.DocumentCompleted -= documentLoginHandler;
                        br.DocumentCompleted += documentCompletedHandler;
                        logBut.InvokeMember("Click");


                        //HtmlDocument doc = br.Document;
                        //HtmlElement username = doc.GetElementById("Login1_UserName");
                        //HtmlElement password = doc.GetElementById("Login1_Password");
                        //HtmlElement submit = doc.GetElementById("Login1_Login");
                        //username.SetAttribute("value", "user");
                        //password.SetAttribute("value", "amur");
                        //submit.InvokeMember("Click");
                    };

                

                // navigate to each URL in the list
                foreach (var url in args)
                {
                    tcs = new TaskCompletionSource<bool>();
                    //wb.DocumentCompleted += documentCompletedHandler;
                    wb.DocumentCompleted += documentCompletedHandler;
                    try
                    {
                        wb.Navigate(url.ToString());
                        // await for DocumentCompleted
                        await tcs.Task;
                    }
                    finally
                    {
                        wb.DocumentCompleted -= documentCompletedHandler;
                    }
                    // the DOM is ready
                    Console.WriteLine(url.ToString());
                    Console.WriteLine(wb.Document.Body.OuterHtml);
                }
            }

            Console.WriteLine("End working.");
            return null;
        }


        static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
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
                    Log("Natigated to " + e.Url);
                    // Application.ExitThread();   // Stops the thread

                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }

            }
        }

        static void browser_LoginCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                
                string dateText = parseDate.ToString("yyyy-MM-dd");
                Uri uri = new Uri("http://hydro.meteoinfo.ru/new/proxy/proxy.ashx?http://hydro.meteoinfo.ru/arcgis/rest/services/project/oper_tables/MapServer/0/query?f=json&where=dateTimeProd%20%3D%20%27" + dateText + "%2008%3A00%3A00%27&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=HydroPostCode%2CGauge%2CFloodPlaneMark%2CAdverseFact%2CDangerFact%2CLevel_obs%2CLevel_for1%2CLevel_for2%2CLevel_for3%2CLevel_for4%2CLevel_for5%2CLevel_for6%2CHeight");
                Log("Natigated to " + uri.ToString());
                br.DocumentCompleted -= browser_LoginCompleted;
                br.DocumentCompleted += browser_RiverCompleted;
                br.Navigate(uri);
            }
        }

        static void browser_RiverCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                string dateText = parseDate.ToString("yyyy-MM-dd");
                Uri uri = new Uri("http://hydro.meteoinfo.ru/new/proxy/proxy.ashx?http://hydro.meteoinfo.ru/arcgis/rest/services/project/oper_tables/MapServer/1/query?f=json&where=dateTimeData%20%3D%20%27" + dateText + "%2000%3A00%3A00%27&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=MeteoModel%2CInflow_obs_WB%2CInflow_obs_HM%2CInflow_for1%2CInflow_for2%2CInflow_for3%2CInflow_for4%2CInflow_for5");

                Log("Natigated to " + uri.ToString());

                Log(br.Document.Body.OuterText);
                BorshchForecastRiver(br.Document.Body.OuterText);

                br.DocumentCompleted -= browser_RiverCompleted;
                br.DocumentCompleted += browser_ReservoirCompleted;
                br.Navigate(uri);
            }
        }

        static void browser_ReservoirCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                Log(br.Document.Body.OuterText);
                BorshchForecastReservoir(br.Document.Body.OuterText);
               //Application.ExitThread();   // Stops the thread
                //tcs.TrySetResult(true);

            }
        }

        static void BorshchForecastRiver(string strJson)
        {
            JObject objJson = JObject.Parse(strJson);
            IList<JToken> tokenList = objJson["features"].Children().ToList();
            foreach (var item in tokenList)
            {

                int HydroPostCode = Convert.ToInt32(item["attributes"]["HydroPostCode"].ToString());

                River theRiver = River.GetByDate(parseDate.Date, HydroPostCode);
                if (theRiver == null)
                {
                    theRiver = new River();
                }
                theRiver.forecasted_at = parseDate.Date;
                theRiver.HydroPostCode = Convert.ToInt32(item["attributes"]["HydroPostCode"].ToString());
                theRiver.Gauge = item["attributes"]["Gauge"].ToString();
                if (item["attributes"]["FloodPlaneMark"].ToString() != "")
                {
                    theRiver.FloodPlaneMark = Convert.ToInt32(item["attributes"]["FloodPlaneMark"].ToString());
                }
                if (item["attributes"]["AdverseFact"].ToString() != "")
                {
                    theRiver.AdverseFact = Convert.ToInt32(item["attributes"]["AdverseFact"].ToString());
                }
                if (item["attributes"]["DangerFact"].ToString() != "")
                {
                    theRiver.DangerFact = Convert.ToInt32(item["attributes"]["DangerFact"].ToString());
                }

                if (item["attributes"]["Level_obs"].ToString() != "")
                {
                    theRiver.Level_obs = Convert.ToInt32(item["attributes"]["Level_obs"].ToString());
                }

                if (item["attributes"]["Level_for1"].ToString() != "")
                {
                    theRiver.Level_for1 = Convert.ToInt32(item["attributes"]["Level_for1"].ToString());
                }

                if (item["attributes"]["Level_for2"].ToString() != "")
                {
                    theRiver.Level_for2 = Convert.ToInt32(item["attributes"]["Level_for2"].ToString());
                }

                if (item["attributes"]["Level_for3"].ToString() != "")
                {
                    theRiver.Level_for3 = Convert.ToInt32(item["attributes"]["Level_for3"].ToString());
                }

                if (item["attributes"]["Level_for4"].ToString() != "")
                {
                    theRiver.Level_for4 = Convert.ToInt32(item["attributes"]["Level_for4"].ToString());
                }

                if (item["attributes"]["Level_for5"].ToString() != "")
                {
                    theRiver.Level_for5 = Convert.ToInt32(item["attributes"]["Level_for5"].ToString());
                }

                if (item["attributes"]["Level_for6"].ToString() != "")
                {
                    theRiver.Level_for6 = Convert.ToInt32(item["attributes"]["Level_for6"].ToString());
                }

                if (item["attributes"]["Height"].ToString() != "")
                {
                    theRiver.Height = Convert.ToDouble(item["attributes"]["Height"].ToString());
                }

                if (theRiver.ID > 0)
                {
                    theRiver.Update();
                }
                else
                {
                    theRiver.Save();
                }


            }
        }

        static void BorshchForecastReservoir(string strJson)
        {
            try
            {
                JObject objJson = JObject.Parse(strJson);
                IList<JToken> tokenList = objJson["features"].Children().ToList();
                foreach (var item in tokenList)
                {

                    string MeteoModel = item["attributes"]["MeteoModel"].ToString();
                    Reservoir theReservoir = Reservoir.GetByDate(parseDate.Date, MeteoModel);
                    if (theReservoir == null)
                    {
                        theReservoir = new Reservoir();
                    }
                    theReservoir.forecasted_at = parseDate.Date;
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

                    if (theReservoir.ID > 0)
                    {
                        theReservoir.Update();
                    }
                    else
                    {
                        theReservoir.Save();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static private void Log(string msg)
        {
            Console.WriteLine(msg);
        }

    
        static void Support02()
        {
            ILogger logger = new LoggerConsole();
            IBot theBot = new BotWebRequest(logger);
            theBot.Parser(DateTime.Now);
        }
        static void Support01()
        {
            ILogger logger = new LoggerConsole();
            IBot theBot = new Bot(logger);
            theBot.Parser(DateTime.Now);
        }
    }
}
