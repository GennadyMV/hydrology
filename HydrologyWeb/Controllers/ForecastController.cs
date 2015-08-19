using HydrologyBorshchForecastEntity.Models;
using HydrologyWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydrologyWeb.Controllers
{
    public class ForecastController : Controller
    {
        //
        // GET: /Forecast/

        public ActionResult Index(int YYYY = -1, int MM = -1, int DD = -1, string tab="river")
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
            int[] HydroPostCodeArray = {6005, 6010, 6016, 6022, 6024, 6027, 6030, 5002, 5004,
                                       5012, 5016, 5019, 5020, 5024, 6280, 6286, 6291, 6295, 6364, 6369};
            BorshchForecast theModel = new BorshchForecast(currDate);
            theModel.tab = tab;
            foreach(var i in HydroPostCodeArray) 
            {
                River theRiver = River.GetByDate(currDate, i);
                if (theRiver == null)
                {
                    theRiver = new River();
                    theRiver.HydroPostCode = i;
                }
                theModel.theRiverList.Add(theRiver);
            }

            theModel.theReservoirCOSMO = Reservoir.GetByDate(currDate, "COSMO");
            if (theModel.theReservoirCOSMO == null)
            {
                theModel.theReservoirCOSMO = new Reservoir();
            }
            theModel.theReservoirJMA = Reservoir.GetByDate(currDate, "JMA");
            if (theModel.theReservoirJMA == null)
            {
                theModel.theReservoirJMA = new Reservoir();
            }
            theModel.theReservoirNCEP = Reservoir.GetByDate(currDate, "NCEP");
            if (theModel.theReservoirNCEP == null)
            {
                theModel.theReservoirNCEP = new Reservoir();
            }
            theModel.theReservoirUKMO = Reservoir.GetByDate(currDate, "UKMO");
            if (theModel.theReservoirUKMO == null)
            {
                theModel.theReservoirUKMO = new Reservoir();
            }
            return View(theModel);
        }

        //
        // GET: /Forecast/Excel

        public ActionResult Excel(int YYYY = -1, int MM = -1, int DD = -1)
        {
            try
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
                int[] HydroPostCodeArray = {6005, 6010, 6016, 6022, 6024, 6027, 6030, 5002, 5004,
                                           5012, 5016, 5019, 5020, 5024, 6280, 6286, 6291, 6295, 6364, 6369};
                BorshchForecast theModel = new BorshchForecast(currDate);

                foreach (var j in HydroPostCodeArray)
                {
                    River theRiver = River.GetByDate(currDate, j);
                    if (theRiver == null)
                    {
                        theRiver = new River();
                        theRiver.HydroPostCode = j;
                    }
                    theModel.theRiverList.Add(theRiver);
                }

                theModel.theReservoirCOSMO = Reservoir.GetByDate(currDate, "COSMO");
                if (theModel.theReservoirCOSMO == null)
                {
                    theModel.theReservoirCOSMO = new Reservoir();
                }
                theModel.theReservoirJMA = Reservoir.GetByDate(currDate, "JMA");
                if (theModel.theReservoirJMA == null)
                {
                    theModel.theReservoirJMA = new Reservoir();
                }
                theModel.theReservoirNCEP = Reservoir.GetByDate(currDate, "NCEP");
                if (theModel.theReservoirNCEP == null)
                {
                    theModel.theReservoirNCEP = new Reservoir();
                }
                theModel.theReservoirUKMO = Reservoir.GetByDate(currDate, "UKMO");
                if (theModel.theReservoirUKMO == null)
                {
                    theModel.theReservoirUKMO = new Reservoir();
                }

                string nameExcel = HttpContext.Server.MapPath("~/App_Data/");
                string fileName = "BorshchForecast-" + theModel.forecastDate.ToString("yyyy-MM-dd") + ".xls";
                string fileNameTemp = string.Format(@"{0}.xls", Guid.NewGuid());
                nameExcel += fileNameTemp;

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    throw new Exception("Не удалось создать объект Excel");
                }
                xlApp.Visible = false;
                xlApp.DisplayAlerts = false;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();

                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                int i = 1;
                xlWorkSheet.get_Range("a1", "m2").Merge(false);
                var chartRange = xlWorkSheet.get_Range("a1", "m2");
                chartRange.FormulaR1C1 = "Фактические уровни воды в бассейне р.Амур на " + theModel.forecastDate.ToString("dd.MM.yyyy") +
                    " года и \nпрогноз уровней воды (в см над нулем графика поста) на " +
                    theModel.forecastDate.AddDays(1).ToString("dd.MM") + " - " +
                    theModel.forecastDate.AddDays(6).ToString("dd.MM.yyyy") + " года";
                chartRange.HorizontalAlignment = 3;
                chartRange.VerticalAlignment = 3;

                i++;
                i++;
                xlWorkSheet.Cells[i, 1] = "Индекс";
                xlWorkSheet.Cells[i, 2] = "Река - Пункт";
                xlWorkSheet.Cells[i, 3] = "Пойма";
                xlWorkSheet.Cells[i, 4] = "НЯ, см";
                xlWorkSheet.Cells[i, 5] = "ОЯ, см";
                xlWorkSheet.Cells[i, 6] = "Н факт, см";
                xlWorkSheet.Cells[i, 7] = "Н прогноз " + theModel.forecastDate.AddDays(1).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 8] = "Н прогноз " + theModel.forecastDate.AddDays(2).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 9] = "Н прогноз " + theModel.forecastDate.AddDays(3).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 10] = "Н прогноз " + theModel.forecastDate.AddDays(4).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 11] = "Н прогноз " + theModel.forecastDate.AddDays(5).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 12] = "Н прогноз " + theModel.forecastDate.AddDays(6).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 13] = "\"0\" графика поста";

                foreach (var river in theModel.theRiverList)
                {
                    i++;
                    xlWorkSheet.Cells[i, 1] = river.HydroPostCode.ToString();
                    xlWorkSheet.Cells[i, 2] = river.Gauge.ToString();
                    xlWorkSheet.Cells[i, 3] = river.FloodPlaneMark.ToString();
                    xlWorkSheet.Cells[i, 4] = river.AdverseFact.ToString();
                    xlWorkSheet.Cells[i, 5] = river.DangerFact.ToString();
                    xlWorkSheet.Cells[i, 6] = river.Level_obs.ToString();
                    xlWorkSheet.Cells[i, 7] = river.Level_for1.ToString();
                    xlWorkSheet.Cells[i, 8] = river.Level_for2.ToString();
                    xlWorkSheet.Cells[i, 9] = river.Level_for3.ToString();
                    xlWorkSheet.Cells[i, 10] = river.Level_for4.ToString();
                    xlWorkSheet.Cells[i, 11] = river.Level_for5.ToString();
                    xlWorkSheet.Cells[i, 12] = river.Level_for6.ToString();
                    xlWorkSheet.Cells[i, 13] = river.Height.ToString();

                }

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Нуль поста - высота отметки нуля графика поста в м Б.С.";

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Нф - фактический уровень воды на 8-00 местного времени по сотоянию " + theModel.forecastDate.ToString("dd.MM.yyyy");

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Критические значения уровня воды в см над нулем графика поста:";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Пойма - уровень, при котором происходит выход воды на пойму";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "НЯ - отметка неблагоприятного явления";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "ОЯ - отметка опасного явления";

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Дата выпуска прогноза: " + theModel.forecastDate.ToShortDateString();

                // Зейское водохранилище
                i += 3;

                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Прогноз суточного притока воды к Зейскому водохранилищу (куб. м/с) на " +
                    theModel.forecastDate.AddDays(1).ToString("dd.MM") + " - " +
                    theModel.forecastDate.AddDays(6).ToString("dd.MM.yyyy");

                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Приток фактический на " + theModel.forecastDate.ToString("dd.MM.yyyy");

                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Q в/б = " + theModel.forecastDate.ToString("dd.MM.yyyy") + " куб м/с";

                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Q г/м = " + theModel.forecastDate.ToString("dd.MM.yyyy") + " куб м/с";


                i += 1;
                xlWorkSheet.Cells[i, 1] = "Модель";
                xlWorkSheet.Cells[i, 2] = "Q прогноз " + theModel.forecastDate.AddDays(1).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 3] = "Q прогноз " + theModel.forecastDate.AddDays(2).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 4] = "Q прогноз " + theModel.forecastDate.AddDays(3).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 5] = "Q прогноз " + theModel.forecastDate.AddDays(4).ToString("dd.MM.yy");
                xlWorkSheet.Cells[i, 6] = "Q прогноз " + theModel.forecastDate.AddDays(5).ToString("dd.MM.yy");
                i += 1;
                xlWorkSheet.Cells[i, 1] = theModel.theReservoirCOSMO.MeteoModel;
                xlWorkSheet.Cells[i, 2] = theModel.theReservoirCOSMO.Inflow_for1;
                xlWorkSheet.Cells[i, 3] = theModel.theReservoirCOSMO.Inflow_for2;
                xlWorkSheet.Cells[i, 4] = theModel.theReservoirCOSMO.Inflow_for3;
                xlWorkSheet.Cells[i, 5] = theModel.theReservoirCOSMO.Inflow_for4;
                xlWorkSheet.Cells[i, 6] = theModel.theReservoirCOSMO.Inflow_for5;
                i += 1;
                xlWorkSheet.Cells[i, 1] = theModel.theReservoirNCEP.MeteoModel;
                xlWorkSheet.Cells[i, 2] = theModel.theReservoirNCEP.Inflow_for1;
                xlWorkSheet.Cells[i, 3] = theModel.theReservoirNCEP.Inflow_for2;
                xlWorkSheet.Cells[i, 4] = theModel.theReservoirNCEP.Inflow_for3;
                xlWorkSheet.Cells[i, 5] = theModel.theReservoirNCEP.Inflow_for4;
                xlWorkSheet.Cells[i, 6] = theModel.theReservoirNCEP.Inflow_for5;
                i += 1;
                xlWorkSheet.Cells[i, 1] = theModel.theReservoirUKMO.MeteoModel;
                xlWorkSheet.Cells[i, 2] = theModel.theReservoirUKMO.Inflow_for1;
                xlWorkSheet.Cells[i, 3] = theModel.theReservoirUKMO.Inflow_for2;
                xlWorkSheet.Cells[i, 4] = theModel.theReservoirUKMO.Inflow_for3;
                xlWorkSheet.Cells[i, 5] = theModel.theReservoirUKMO.Inflow_for4;
                xlWorkSheet.Cells[i, 6] = theModel.theReservoirUKMO.Inflow_for5;
                i += 1;
                xlWorkSheet.Cells[i, 1] = theModel.theReservoirJMA.MeteoModel;
                xlWorkSheet.Cells[i, 2] = theModel.theReservoirJMA.Inflow_for1;
                xlWorkSheet.Cells[i, 3] = theModel.theReservoirJMA.Inflow_for2;
                xlWorkSheet.Cells[i, 4] = theModel.theReservoirJMA.Inflow_for3;
                xlWorkSheet.Cells[i, 5] = theModel.theReservoirJMA.Inflow_for4;
                xlWorkSheet.Cells[i, 6] = theModel.theReservoirJMA.Inflow_for5;

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "COSMO - модель COSMO (Россия)";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "NCEP - модель Национального центра прогнозов (США)";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "UKMO - модель Метеорологического бюро Великобритании";
                i += 1;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "JMA - модель Японского метеорологического центра";

                i += 2;
                xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString()).Merge(false);
                chartRange = xlWorkSheet.get_Range("a" + i.ToString(), "m" + i.ToString());
                xlWorkSheet.Cells[i, 1] = "Дата выпуска прогноза: " + theModel.forecastDate.ToShortDateString();


                xlWorkBook.SaveAs(nameExcel);
                xlWorkBook.Close(true);
                xlApp.Quit();

                byte[] fileBytes = System.IO.File.ReadAllBytes(nameExcel);
                System.IO.File.Delete(nameExcel);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message + "\n" + ex.StackTrace;
                return View();
            }

        }

    }
}
