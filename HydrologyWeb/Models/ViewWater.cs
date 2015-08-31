using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class ViewWater
    {
        public Hydro.WaterObjectCollection theWaterObjectCollection;
        public int SelectedWaterObject;
        public Hydro.Site[] theSites;
    }
}