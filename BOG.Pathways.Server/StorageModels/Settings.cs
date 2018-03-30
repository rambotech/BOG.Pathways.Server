using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.StorageModels
{
    public class Settings
    {
        public string IpAddress { get; set; } = string.Empty;
        public string PublicName = "Default";
        public string SuperAccessToken = "0a1fe9bc-7548-45ae-bebd-9263cf112e07";
        public string AdminAccessToken = "36a1a42f-079b-4bde-8660-10b101465f22";
        public string UserAccessToken = "9bcc39ce-0b6f-418f-9dbf-b7653d391cb1";
        public int HttpPortNumber = 5670;
        public int HttpsPortNumber = 5671;
        public int PayloadSizeLimit = 524288;
        public int PathwayMaximumPayloads = 200;
        public Int64 TotalPayloadSizeLimit = 2147483648;
        public int LoggingLevel = 2;
        public List<string> IpWhitelist = new List<string>();
    }
}
