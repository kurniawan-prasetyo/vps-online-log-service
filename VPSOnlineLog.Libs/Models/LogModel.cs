using System;

namespace VPSOnlineLog.Libs.Models
{
    public class LogModel
    {
        public string LogId { get; set; }
        public string LogName { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsOnline { get; set; }
    }
}
