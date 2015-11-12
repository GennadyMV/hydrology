using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class ViewLevel
    {
        public List<ViewSite> theSiteList;
        public int SelectedSiteCode;
        public List<Level> theLevelList;
        public List<Level> theHydroPostLevelList;
        public ViewLevel()
        {
            theLevelList = new List<Level>();
            theHydroPostLevelList = new List<Level>();
        }
        public class Level
        {
            public int ID;
            public float Value;
            public int YYYY;
            public int MM;
            public int DD;
            public int HH;
            public int MI;
            public int SS;

            public DateTime _Date
            {
                get
                {
                    return new DateTime(this.YYYY, this.MM, this.DD, this.HH, this.MI, this.SS);
                }
            }
        }
    }
}