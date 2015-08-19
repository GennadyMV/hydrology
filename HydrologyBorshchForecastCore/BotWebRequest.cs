using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastCore
{
    public class BotWebRequest : IBot
    {
         private ILogger theLogger;
        public BotWebRequest(ILogger logger)
        {
            theLogger = logger;
        }
        bool IBot.isNew()
        {
            return true;
        }
        void IBot.Parser(DateTime parseDate)
        {
            this.Log("Parser Start");
            Uri uri = new Uri("http://hydro.meteoinfo.ru/new/login.aspx?ReturnUrl=%2fnew");            

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Log(responseString);

           
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
