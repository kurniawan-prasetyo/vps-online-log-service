using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using VPSOnlineLog.Libs.BL;

namespace VPSOnlineLog.WinService
{
    public partial class VPSOnlineLogService : ServiceBase
    {
        bool IsOnCheckingProcess;
        EventLog _eventlog;
        const string event_log_source = "VPS Online Log";
        const string event_log_name = "Service Log";

        public VPSOnlineLogService()
        {
            InitializeComponent();

            _eventlog = new EventLog();
            if (!EventLog.SourceExists(event_log_source))
            {
                EventLog.CreateEventSource(event_log_source, event_log_name);
            }
            _eventlog.Source = event_log_source;
            _eventlog.Log = event_log_name;


            try
            {
                IsOnCheckingProcess = false;
            }
            catch (Exception ex)
            {
                _eventlog.WriteEntry(ex.InnerException?.Message ?? ex.Message ?? "Unknown Error!");
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                System.Threading.Thread.Sleep(10000);

                Timer timer = new Timer();
                timer.Interval = 1000; 
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }
            catch (Exception ex)
            {
                _eventlog.WriteEntry(ex.InnerException?.Message ?? ex.Message ?? "Unknown Error!");
            }
        }

        protected override void OnStop()
        {
            try
            {
                IsOnCheckingProcess = false;
            }
            catch (Exception ex)
            {
                _eventlog.WriteEntry(ex.InnerException?.Message ?? ex.Message ?? "Unknown Error!");
            }
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                if (!IsOnCheckingProcess)
                    CheckStatus();
            }
            catch (Exception ex)
            {
                _eventlog.WriteEntry(ex.InnerException?.Message ?? ex.Message ?? "Unknown Error!");
            }
        }

        void CheckStatus() {
            try
            {
                IsOnCheckingProcess = true;
                var log_service = new LogService();
                log_service.StartLog();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsOnCheckingProcess = false;
            }
        }
    }
}
