using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastCore
{
    public class LoggerNLog:ILogger
    {
        protected Logger logger;
        public LoggerNLog()
        {
            logger = LogManager.GetLogger("log");
        }
        void ILogger.Log(string msg)
        {
            logger.Debug(msg);
        }
    }
}
