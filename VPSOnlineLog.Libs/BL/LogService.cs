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
        string ProductFolderName;
        string LogFileName;
        const string ONLINE = "ON";
        const string OFFLINE = "OFFLINE !";
        const string CompanyFolderName = "KPrasetyo";

        public LogService()
        {
            LastLogModel = new LogModel();
            AllLogDatas = new List<LogModel>();
            ProductFolderName = ConfigurationManager.AppSettings["ProductFolderName"];
            LogFileName = ConfigurationManager.AppSettings["LogFileName"];
        }

        public void StartLog() {
            try
            {
                if (string.IsNullOrEmpty(LogFileName) || string.IsNullOrEmpty(LogFileName)) return;

                var companyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CompanyFolderName);
                var productPath = Path.Combine(companyPath, ProductFolderName);
                var logFilePath = Path.Combine(productPath, LogFileName);

                if(!Directory.Exists(productPath))
                    Directory.CreateDirectory(productPath);
                    
                if (!File.Exists(logFilePath))
                    File.Create(logFilePath);

                using (StreamReader r = new StreamReader(logFilePath))
                {
                    string json = r.ReadToEnd();
                    AllLogDatas = JsonConvert.DeserializeObject<List<LogModel>>(json) ?? new List<LogModel>();
                    if (AllLogDatas.Count > 0)
                        LastLogModel = AllLogDatas.OrderBy(x => x.LogDate).LastOrDefault();
                }

                var isOnline = CheckForInternetConnection();
                if (isOnline && LastLogModel?.LogName != ONLINE)
                {
                    AllLogDatas.Add(new LogModel
                    {
                        LogId = LastLogModel.LogId + 1,
                        LogName = ONLINE,
                        LogDate = DateTime.Now
                    });
                    WriteLog(logFilePath);
                }
                else if (!isOnline && LastLogModel.LogName != OFFLINE)
                {
                    AllLogDatas.Add(new LogModel
                    {
                        LogId = LastLogModel.LogId + 1,
                        LogName = OFFLINE,
                        LogDate = DateTime.Now
                    });
                    WriteLog(logFilePath);
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

        private void WriteLog(string FilePath){
            try
            {
                AllLogDatas = AllLogDatas
                    .OrderByDescending(x => x.LogDate)
                    .TakeWhile((x, y) => y < 100)
                    .ToList();

                LogUtils.StreamWrite(AllLogDatas, FilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
