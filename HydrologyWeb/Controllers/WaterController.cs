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
            theViewData.theSiteList = ViewSite.GetAll();
            return View(theViewData);
        }

        public ActionResult Change(int SiteCode, int SiteType, int WaterObject, bool isChecked)
        {
            Hydro.Site theSite = null;
            try
            {
                Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();
                theSite = theHydro.GetSite(SiteCode.ToString(), SiteType);
              

                if (isChecked)
                {
                    Hydro.WaterObject theWO = null;
                    
                    foreach (var item in theHydro.GetWaterObjects(false))
                    {
                        if (item.Id == WaterObject)
                        {
                            theWO = item;
                            break;
                        }
                    }

                    theHydro.SaveSiteWithRelation(theSite, theWO);
                }
                else
                {
                    
                    theHydro.SaveSiteWithRelation(theSite, null);
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("Станция " + theSite.Name + " успешно обновлена");
        }

    }
}
