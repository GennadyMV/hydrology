using HydrologyBorshchForecastEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class BorshchForecast
    {
        public BorshchForecast(DateTime forecastDate)
        {
            theRiverList = new List<River>();
            this.forecastDate = forecastDate;
            tab = "river";
        }
        public string tab;
        public DateTime forecastDate;
        public List<River> theRiverList;
        public Reservoir theReservoirCOSMO;
        public Reservoir theReservoirNCEP;
        public Reservoir theReservoirUKMO;
        public Reservoir theReservoirJMA;

    }
}