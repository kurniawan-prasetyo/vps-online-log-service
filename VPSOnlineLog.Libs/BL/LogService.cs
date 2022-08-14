using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Linq;
using VPSOnlineLog.Libs.Models;
using System.Security.Cryptography;

namespace VPSOnlineLog.Libs.BL
{
    public class LogService
    {
        LogModel LastLogModel;
        List<LogModel> AllLogDatas;
        string LogFilePath;
        const string ONLINE = "ON";
        const string OFFLINE = "OFFLINE !";

        public LogService()
        {
            LastLogModel = new LogModel();
            AllLogDatas = new List<LogModel>();
            LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        }

        public void StartLog() {
            try
            {
                if (string.IsNullOrEmpty(LogFilePath)) return;

                if (!File.Exists(LogFilePath))
                    File.Create(LogFilePath);

                using (StreamReader r = new StreamReader(LogFilePath))
                {
                    string json = r.ReadToEnd();
                    AllLogDatas = JsonConvert.DeserializeObject<List<LogModel>>(json) ?? new List<LogModel>();
                    if (AllLogDatas.Count > 0)
                        LastLogModel = AllLogDatas.OrderBy(x => x.LogDate).LastOrDefault();
                }

                var isOnline = CheckForInternetConnection();
                if (isOnline && LastLogModel.LogName != ONLINE)
                {
                    AllLogDatas.Add(new LogModel
                    {
                        LogId = LastLogModel.LogId + 1,
                        LogName = ONLINE,
                        LogDate = DateTime.Now
                    });
                    WriteLog();
                }
                else if (!isOnline && LastLogModel.LogName != OFFLINE)
                {
                    AllLogDatas.Add(new LogModel
                    {
                        LogId = LastLogModel.LogId + 1,
                        LogName = OFFLINE,
                        LogDate = DateTime.Now
                    });
                    WriteLog();
                }
                else 
                { 
                    // Same Status, Do not change anything!
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool CheckForInternetConnection(int timeoutMs = 4000, string url = null)
        {
            try
            {
                url = url ?? @"http://www.gstatic.com/generate_204";

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

        private void WriteLog() {
            try
            {
                AllLogDatas = AllLogDatas
                    .OrderByDescending(x => x.LogDate)
                    .TakeWhile((x, y) => y < 100)
                    .ToList();

                LogUtils.StreamWrite(AllLogDatas, LogFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
