namespace HydrologyAgkExportService
{
    partial class HydrologyAgkExportService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Timers.Timer();
            this.timer1.Interval = 1000 * 60 * 60;            // 1 час
            this.timer1.Start();
            // 
            // timer1
            // 
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            // 
            // HydrologyAgkExportService
            // 
            this.ServiceName = "HydrologyAgkExportService";

        }

        #endregion

        private System.Timers.Timer timer1;
    }
}
