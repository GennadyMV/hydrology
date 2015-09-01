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
        public List<ViewSite> theSiteList;
        public bool Check(int siteCode, int siteType)
        {
            if (theSiteList != null)
            {
                if (theSites.Where(x => Convert.ToInt32(x.SiteCode) == siteCode && x.Type.Id == siteType).Count() == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}