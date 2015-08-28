using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class ViewMap
    {
        public Hydro.SiteTypeCollection theSiteTypeCollection;
        public int SelectedSiteType;
        public Hydro.Site[] theSites;
    }
}