using System;

namespace VPSOnlineLog.Libs.Models
{
    public class LogModel
    {
        public long LogId { get; set; } = 0;
        public string LogName { get; set; }
        public DateTime LogDate { get; set; }
    }
}
