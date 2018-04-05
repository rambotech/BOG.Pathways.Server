using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.StorageModels
{
    /// <summary>
    /// Define the pathway server behavior
    /// </summary>
    public class Settings
    {
        public string IpAddress { get; set; } = string.Empty;
        public string PublicName = "Default";
        public string SuperAccessToken = "SUPER";
        public string AdminAccessToken = "ADMIN";
        public string UserAccessToken = "USER";
        public int HttpPortNumber = 5670;
        public int HttpsPortNumber = 5671;
        public int PayloadSizeLimit = 524288;
        public int PathwayMaximumPayloads = 200;
        public Int64 TotalPayloadSizeLimit = 2147483648;
        public int LoggingLevel = 2;
        public List<string> IpWhitelist = new List<string>();
    }
}
