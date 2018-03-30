using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOG.Pathways.Server.StorageModels
{
    public class Pathway
    {
        public string Owner { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public string ReadToken { get; set; } = string.Empty;
        public string WriteToken { get; set; } = string.Empty;
        public int MaxPayloads { get; set; } = 200;
        public int MaxReferences { get; set; } = 20;
        public int ReadSize { get; set; } = 0;
        public int WriteSize { get; set; } = 0;
        public int ReadTally { get; set; } = 0;
        public int ReadDeniedTally { get; set; } = 0;
        public int WriteTally { get; set; } = 0;
        public int WriteDeniedTally { get; set; } = 0;
        public DateTime LastReadOn { get; set; } = DateTime.MinValue;
        public DateTime LastWriteOn { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public ConcurrentQueue<Payload> Payloads { get; set; } = new ConcurrentQueue<Payload>();

        [JsonIgnore]
        public ConcurrentDictionary<string, Payload> References { get; set; } = new ConcurrentDictionary<string, Payload>();
    }
}
