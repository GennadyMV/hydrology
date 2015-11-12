using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class ViewSite
    {
        public int ID;
        public int Code;
        public string Name;
        public int TypeID;
        public string TypeNameFull;
        public string TypeNameShort;

        public static List<ViewSite> GetAll(int TYPE = -1)
        {
            List<ViewSite> theList = new List<ViewSite>();
            Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();
            foreach (var theType in theHydro.GetSiteTypes())
            {
                if (TYPE != -1)
                {
                    if (TYPE != theType.Id)
                    {
                        continue;
                    }
                }

                Hydro.Site[] theSites = theHydro.GetSiteList(theType.Id);
                if (theSites != null) {
                    foreach (var theSite in theHydro.GetSiteList(theType.Id))
                    {
                        ViewSite theViewSite = new ViewSite();
                        theViewSite.ID = theSite.SiteId;
                        theViewSite.Code = Convert.ToInt32(theSite.SiteCode);
                        theViewSite.Name = theSite.Name;
                        theViewSite.TypeID = theType.Id;
                        theViewSite.TypeNameFull = theType.Name;
                        theViewSite.TypeNameShort = theType.ShortName;

                        theList.Add(theViewSite);
                    }
                }
                
            }
            return theList;
        }
    }
}