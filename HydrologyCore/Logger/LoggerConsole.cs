using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyCore.Logger
{
    public class LoggerConsole: ILogger
    {
        void ILogger.Log(string msg)
        {
            Console.WriteLine(msg);
        }
        void ILogger.Error(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
