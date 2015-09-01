using HydrologyWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydrologyWeb.Controllers
{
    public class ExpenseController : Controller
    {
        //
        // GET: /Expense/

        public ActionResult Index()
        {

            string dirAppData = HttpContext.Server.MapPath("~/App_Data/");
            string dirExpense = dirAppData + "Expense/";

            bool exists = System.IO.Directory.Exists(dirExpense);

            if (!exists)
                System.IO.Directory.CreateDirectory(dirExpense);

            string[] filePaths = Directory.GetFiles(dirExpense);

            ViewExpense theViewExpense = new ViewExpense();

            foreach(var item in filePaths)
            {
                theViewExpense.Add(item); 
            }

            return View();
        }

    }
}
