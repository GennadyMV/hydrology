using HydrologyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydrologyWeb.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index(int SiteType = -1)
        {

            Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();
            ViewMap theViewData = new ViewMap();
            theViewData.SelectedSiteType = SiteType;
            theViewData.theSiteTypeCollection = theHydro.GetSiteTypes();

            if (theViewData.SelectedSiteType == -1)
            {
                theViewData.SelectedSiteType = theViewData.theSiteTypeCollection[0].Id;
            }

            theViewData.theSites = theHydro.GetSiteList(theViewData.SelectedSiteType);


            return View(theViewData);
        }

    }
}
