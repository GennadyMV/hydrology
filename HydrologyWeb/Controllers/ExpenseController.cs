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
                theViewExpense.Add(Path.GetFileNameWithoutExtension(item)); 
            }

            return View(theViewExpense);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    string dirAppData = HttpContext.Server.MapPath("~/App_Data/");
                    string dirExpense = dirAppData + "Expense/";

                    var path = Path.Combine(dirExpense, fileName);
                    file.SaveAs(path);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Show(string id)
        {
            ViewBag.Title = id;

            string dirAppData = HttpContext.Server.MapPath("~/App_Data/");
            string dirExpense = dirAppData + "Expense/";

            bool exists = System.IO.Directory.Exists(dirExpense);

            if (!exists)
                System.IO.Directory.CreateDirectory(dirExpense);

            string[] filePaths = Directory.GetFiles(dirExpense);

            string fileExcel = "";
            foreach (var item in filePaths)
            {
                if (id == Path.GetFileNameWithoutExtension(item))
                {
                    fileExcel = item;
                }
                
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                throw new Exception("Не удалось создать объект Excel");
            }
            xlApp.Visible = false;
            xlApp.DisplayAlerts = false;

            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(fileExcel);
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
           
            var range = xlWorkSheet.UsedRange;

            ViewBag.Rows = range.Rows.Count;
            ViewBag.Columns = range.Columns.Count;

            string[,] theDatas = new string [range.Rows.Count, range.Columns.Count];

            for (int rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
            {
                for (int cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    string str = "";
                    if ((range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value != null)
                    {
                        str = (string)(range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value.ToString();
                    }
                    
                    theDatas[rCnt-1, cCnt-1] = str;
                    
                }
            }

            xlWorkBook.Close();
            xlApp.Quit();


            return View(theDatas);
        }

    }
}
