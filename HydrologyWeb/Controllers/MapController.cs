using HydrologyWeb.Hydro;
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

        public ActionResult EditSite(string siteCode="1007", int typeId=0, string name="", string lat="", string lon="")
        {
            string result = "";
            try
            {
                Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();
                Site theSite = theHydro.GetSite(siteCode, typeId);
                if (name != "")
                {
                    theSite.Name = name;
                }
                else
                {
                    throw new Exception("Пустое поле Наименование поста");
                }
                if (lat != "")
                {
                    theSite.Lat = Convert.ToDecimal(lat);
                }
                else
                {
                    throw new Exception("Пустое поле Lat поста");
                }
                if(lon != "") {
                    theSite.Lon = Convert.ToDecimal(lon);
                }
                else
                {
                    throw new Exception("Пустое поле Lon поста");
                }
                theHydro.SaveSiteWithRelation(theSite, null);
                result = "Ok:" + siteCode;
            }
            catch (Exception ex)
            {
                result = "Error:" + ex.Message;
            }
            

            return Content(result);
        }

    }
}
