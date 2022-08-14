using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VPSOnlineLog.WinService
{
    internal class LogUtils
    {
        public static void WriteDynamicJsonObject(JObject jsonObj, string fileName)
        {
            var streamWriter = File.CreateText(fileName);
            var jsonWriter = new JsonTextWriter(streamWriter);
            jsonObj.WriteTo(jsonWriter);
        }

        public static void StreamWrite(object obj, string fileName)
        {
            var streamWriter = File.CreateText(fileName);
            var jsonWriter = new JsonTextWriter(streamWriter);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, obj);
        }
    }
}
