using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOG.Pathways.Server.StorageModels
{
    [JsonObject]
    public class Payload
    {
        public string IdempotentID { get; set; } = Guid.NewGuid().ToString("N").ToString();
        public DateTime PostTimeUtc { get; set; } = DateTime.UtcNow;
        public string ContentType { get; set; } = "application/octet-stream";
        public string ContentTransferEncoding { get; set; } = "base64";
        public string ContentMD5 { get; set; } = string.Empty;
    }
}
