using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastCore
{
    public interface IBot
    {
        void Parser(DateTime parserDate);
        bool isNew();
    }
}
