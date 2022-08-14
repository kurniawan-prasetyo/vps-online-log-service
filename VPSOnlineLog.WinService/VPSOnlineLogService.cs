using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Timers;

namespace VPSOnlineLog.WinService
{
    public partial class VPSOnlineLogService : ServiceBase
    {
        bool IsOnCheckingProcess;
        Model.LogModel LastLogModel;
        List<Model.LogModel> AllLogDatas;
        string LogFilePath;

        public VPSOnlineLogService()
        {
            InitializeComponent();

            IsOnCheckingProcess = false;
            LastLogModel = new Model.LogModel();
            AllLogDatas = new List<Model.LogModel>();
            LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        }

        protected override void OnStart(string[] args)
        {
            if (!string.IsNullOrEmpty(LogFilePath))
            {
                using (StreamReader r = new StreamReader(LogFilePath))
                {
                    string json = r.ReadToEnd();
                    AllLogDatas = JsonConvert.DeserializeObject<List<Model.LogModel>>(json);
                    if (AllLogDatas != null && AllLogDatas.Count > 0)
                        LastLogModel = AllLogDatas.OrderBy(x => x.LogDate).LastOrDefault();
                }

                Timer timer = new Timer();
                timer.Interval = 1000; // 60 seconds
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }
        }

        protected override void OnStop()
        {
            IsOnCheckingProcess = false;
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            if(!IsOnCheckingProcess)
                CheckStatus();
        }

        void CheckStatus() {

            if(CheckForInternetConnection() && LastLogModel.LogName == "OFFLINE")


            IsOnCheckingProcess = false;
        }


        public static bool CheckForInternetConnection(int timeoutMs = 4000, string url = null)
        {
            try
            {
                url = url ?? "http://www.gstatic.com/generate_204";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
