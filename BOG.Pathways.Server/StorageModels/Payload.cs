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
        public string ContentType { get; set; } = null;
        public string ContentTransferEncoding { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
