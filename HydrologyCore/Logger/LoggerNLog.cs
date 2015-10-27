using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyCore.Logger
{
    public class LoggerNLog:ILogger
    {
        NLog.Logger logger;
        NLog.Logger error;
        public LoggerNLog()
        {
            logger = LogManager.GetLogger("log");
            error = LogManager.GetLogger("error");
        }
        void ILogger.Log(string msg)
        {
            logger.Debug(msg);
        }
        void ILogger.Error(string msg)
        {
            error.Debug(msg);
        }
    }
}
