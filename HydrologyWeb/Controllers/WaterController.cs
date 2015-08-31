using HydrologyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydrologyWeb.Controllers
{
    public class WaterController : Controller
    {
        //
        // GET: /Water/

        public ActionResult Index(int WaterObject = -1)
        {

            Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();
            ViewWater theViewData = new ViewWater();
            theViewData.SelectedWaterObject = WaterObject;
            theViewData.theWaterObjectCollection = theHydro.GetWaterObjects(false);

            if (theViewData.SelectedWaterObject == -1)
            {
                theViewData.SelectedWaterObject = theViewData.theWaterObjectCollection[0].Id;
            }

            theViewData.theSites = theHydro.GetSitesByWaterObject(theViewData.SelectedWaterObject);
            
            return View(theViewData);
        }

    }
}
