using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastCore
{
    public class LoggerConsole: ILogger
    {
        public LoggerConsole()
        {

        }
        void ILogger.Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
