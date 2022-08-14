using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace VPSOnlineLog.Libs
{
    public static class LogUtils
    {
        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        public static void WriteDynamicJsonObject(JObject jsonObj, string fileName)
        {
            var streamWriter = File.CreateText(fileName);
            var jsonWriter = new JsonTextWriter(streamWriter);
            jsonObj.WriteTo(jsonWriter);
        }

        public static void StreamWrite(object obj, string fileName)
        {
            using (var streamWriter = File.CreateText(fileName))
            {
                using(var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    JsonSerializer.CreateDefault(_options).Serialize(jsonWriter, obj);
                }
            }
        }
    }
}
