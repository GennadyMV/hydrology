using HydrologyCore.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyCore.Agk
{
    public class ExportFromHydroService : IExport
    {
        ILogger theLogger;
        public ExportFromHydroService(ILogger theLogger)
        {
            this.theLogger = theLogger;
        }
        void IExport.Run()
        {
            try
            {
                this.theLogger.Log("Export:Run");
                AMS.Profile.Ini Ini = new AMS.Profile.Ini(AppDomain.CurrentDomain.BaseDirectory + "AgkExportSettings.ini");
                if (!Ini.HasSection("COMMON"))
                {
                    Ini.SetValue("COMMON", "AgkDataFolder", "C:\\Данные_АГК");
                    Ini.SetValue("COMMON", "CountPrevMonth", "1");
                }

                string strAgkDataFolder = Ini.GetValue("COMMON", "AgkDataFolder", "C:\\Данные_АГК");
                int countPrevMonth = Ini.GetValue("COMMON", "CountPrevMonth", 1);

                strAgkDataFolder.TrimEnd(new char[] {'\\'});

                if (!Directory.Exists(strAgkDataFolder))
                {
                    Directory.CreateDirectory(strAgkDataFolder);
                }

                Hydro.HydroServiceClient theHydro = new Hydro.HydroServiceClient("BasicHttpBinding_IHydroService");
                const int TYPE_AGK = 6;
                foreach (var Site in theHydro.GetSiteList(TYPE_AGK))
                {
                    string strSiteFolder = strAgkDataFolder + "\\" + Site.SiteCode.ToString();
                    if (!Directory.Exists(strSiteFolder))
                    {
                        Directory.CreateDirectory(strSiteFolder);
                        theLogger.Log(strSiteFolder);
                    }

                    strSiteFolder = strSiteFolder + "\\уровни";
                    if (!Directory.Exists(strSiteFolder))
                    {
                        Directory.CreateDirectory(strSiteFolder);
                        theLogger.Log(strSiteFolder);
                    }
                    DateTime bgnDate = new DateTime(DateTime.Now.AddMonths(-countPrevMonth).Year,
                        DateTime.Now.AddMonths(-countPrevMonth).Month, countPrevMonth);
                    DateTime endDate = new DateTime(DateTime.Now.AddMonths(-countPrevMonth).Year,
                        DateTime.Now.AddMonths(-countPrevMonth).Month,
                        DateTime.DaysInMonth(DateTime.Now.AddMonths(-countPrevMonth).Year, DateTime.Now.AddMonths(-countPrevMonth).Month));
                    const int WATER_LEVEL = 2;

                    string strFileName = strSiteFolder + "\\" + Site.SiteCode.ToString() + "_" + bgnDate.ToString("yyyy_MM") + ".asc";
                    theLogger.Log(strFileName);
                    var fileHandle = File.CreateText(strFileName);
                    var DataValueList = theHydro.GetDataValues(Site.SiteId, bgnDate, endDate, WATER_LEVEL, null, null, null);
                    if (DataValueList != null)
                    {
                        foreach (var item in DataValueList)
                        {
                            string line = item.Date.ToString("dd.MM.yyyy hh:mm:ss") + "\t" + item.Value.ToString() + "\tсм";
                            fileHandle.WriteLine(line);
                        }
                    }
                    fileHandle.Close();
                    //for (DateTime currDate = bgnDate; currDate <= endDate; currDate.AddDays(1))
                    //{
                    //    var DataValueList = theHydro.GetDataValues(Site.SiteId, TYPE_AGK, currDate, WATER_LEVEL, null, null);
                    //    foreach (var item in DataValueList)
                    //    {
                            
                    //    }
                    //}

                }


                this.theLogger.Log(strAgkDataFolder);
            }
            catch (Exception ex)
            {
                this.theLogger.Error(ex.Message);
                this.theLogger.Error(ex.Source);
                this.theLogger.Error(ex.StackTrace);
            }
        }
    }
}
