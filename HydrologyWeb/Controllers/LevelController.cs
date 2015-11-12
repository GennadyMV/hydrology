using HydrologyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydrologyWeb.Controllers
{
    public class LevelController : Controller
    {
        //
        // GET: /Level/

        public ActionResult Index()
        {
            return View();
        }

       public ActionResult Days(int SiteCode = -1, int YYYY = -1, int MM = -1, int DD = -1)
        {
            DateTime currDate;
            try
            {
                currDate = new DateTime(YYYY, MM, DD);
            }
            catch
            {
                currDate = DateTime.Now;
            }

            ViewBag.currDate = currDate;

            Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();

            ViewLevel theView = new ViewLevel();
            const int TYPE_AGK = 6;
            theView.theSiteList = ViewSite.GetAll(TYPE_AGK);
            theView.SelectedSiteCode = SiteCode;
            if (theView.SelectedSiteCode == -1)
            {
                theView.SelectedSiteCode = theView.theSiteList[0].Code;
            }

            return View(theView);
        }

       public ActionResult Months(int SiteCode = -1, int YYYY = -1, int MM = -1, int DD = 1)
       {
           DateTime currDate;
           try
           {
               currDate = new DateTime(YYYY, MM, DD);
           }
           catch
           {
               currDate = DateTime.Now;
           }

           ViewBag.currDate = currDate;

           Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient();

           ViewLevel theView = new ViewLevel();
           const int TYPE_AGK = 6;
           theView.theSiteList = ViewSite.GetAll(TYPE_AGK);
           theView.SelectedSiteCode = SiteCode;
           if (theView.SelectedSiteCode == -1)
           {
               theView.SelectedSiteCode = theView.theSiteList[0].Code;
           }

           int SiteID = theView.theSiteList.Where(x=>x.Code == theView.SelectedSiteCode).Single().ID;

           DateTime dateBgn = new DateTime(currDate.Year, currDate.Month, 1);
           DateTime dateEnd = new DateTime(currDate.Year, currDate.Month, DateTime.DaysInMonth(currDate.Year, currDate.Month));

           const int LEVEL_CODE = 2;

           var ValueCollection = theHydro.GetDataValues(SiteID, dateBgn, dateEnd, LEVEL_CODE, null, null, null);
           if (ValueCollection == null)
           {
               return View(theView);
           }

            foreach (var item in ValueCollection)
            {
                if (item.Value > -9000)
                {
                    ViewLevel.Level theLevel = new ViewLevel.Level();
                    theLevel.Value = item.Value;
                    theLevel.YYYY = item.Date.Year;
                    theLevel.MM = item.Date.Month;
                    theLevel.DD = item.Date.Day;
                    theLevel.HH = item.Date.Hour;
                     theLevel.MI = 0;
                    theLevel.SS = 0;
                    theLevel.ID = item.Id;
                    theView.theLevelList.Add(theLevel);
                }

            }

           const int HydroPostType = 2;

           var HydroPost = theHydro.GetSite(theView.SelectedSiteCode.ToString(), HydroPostType);
           var HydroPostValueCollection = theHydro.GetDataValues(HydroPost.SiteId, dateBgn, dateEnd, LEVEL_CODE, null, null, null);
           if (HydroPostValueCollection == null)
           {
               return View(theView);
           }

           foreach (var item in HydroPostValueCollection)
           {
               if (item.Value > -9000)
               {
                   ViewLevel.Level theLevel = new ViewLevel.Level();
                   theLevel.Value = item.Value;
                   theLevel.YYYY = item.Date.Year;
                   theLevel.MM = item.Date.Month;
                   theLevel.DD = item.Date.Day;
                   theLevel.HH = item.Date.Hour;
                   theLevel.MI = 0;
                   theLevel.SS = 0;
                   theLevel.ID = item.Id;
                   theView.theHydroPostLevelList.Add(theLevel);
               }

           }

          

          return View(theView);
        }

       public ActionResult DeleteLevel(int SiteCode, int DataValueID)
       {
           Hydro.HydroServiceClient theClient = new Hydro.HydroServiceClient();
           
           return Content("");
       }

    }
}
