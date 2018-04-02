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
        [JsonProperty]
        public string Name { get; set; } = string.Empty;
        [JsonProperty]
        public DateTime Created { get; set; } = DateTime.Now;
        [JsonProperty]
        public string ReadToken { get; set; } = string.Empty;
        [JsonProperty]
        public string WriteToken { get; set; } = string.Empty;
        [JsonProperty]
        public int MaxPayloads { get; set; } = 200;
        [JsonProperty]
        public int MaxReferences { get; set; } = 20;
        [JsonProperty]
        public int ReadSize { get; set; } = 0;
        [JsonProperty]
        public int WriteSize { get; set; } = 0;
        [JsonProperty]
        public int ReadTally { get; set; } = 0;
        [JsonProperty]
        public int ReadDeniedTally { get; set; } = 0;
        [JsonProperty]
        public int WriteTally { get; set; } = 0;
        [JsonProperty]
        public int WriteDeniedTally { get; set; } = 0;
        [JsonProperty]
        public DateTime LastReadOn { get; set; } = DateTime.MinValue;
        [JsonProperty]
        public DateTime LastWriteOn { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public ConcurrentQueue<Payload> Payloads { get; set; } = new ConcurrentQueue<Payload>();
        [JsonProperty]
        public int PayloadCount { get { return Payloads.Count; } }

        [JsonIgnore]
        public ConcurrentDictionary<string, Payload> References { get; set; } = new ConcurrentDictionary<string, Payload>();
        [JsonProperty]
        public int ReferenceCount { get { return References.Count; } }
    }
}
