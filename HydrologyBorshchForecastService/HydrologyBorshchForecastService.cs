using HydrologyBorshchForecastCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrologyBorshchForecastService
{
    public partial class HydrologyBorshchForecastService : ServiceBase
    {
 

        public HydrologyBorshchForecastService()
        {
            InitializeComponent();


        }

        protected override void OnStart(string[] args)
        {
            timer1.Start();
        }

        protected override void OnStop()
        {
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            ILogger theLogger = null;
            IBot theBot = null;
            try
            {
                theLogger = new LoggerNLog();
                theBot = new Bot(theLogger);

                theBot.Parser(DateTime.Now);

                timer1.Stop();
                timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                theLogger.Log(ex.Message);
            }
        }
        
    }
}
