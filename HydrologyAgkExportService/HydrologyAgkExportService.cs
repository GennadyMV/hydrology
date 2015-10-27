using HydrologyCore.Agk;
using HydrologyCore.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyAgkExportService
{
    public partial class HydrologyAgkExportService : ServiceBase
    {
        public HydrologyAgkExportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ILogger theLoggerNLog = new LoggerNLog();
            IExport theExport = new ExportFromHydroService(theLoggerNLog);
            theExport.Run();
        }

        protected override void OnStop()
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ILogger theLoggerNLog = new LoggerNLog();
            IExport theExport = new ExportFromHydroService(theLoggerNLog);
            theExport.Run();
        }
    }
}
