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
        // Config
        [JsonProperty]
        public string Id { get; set; } = string.Empty;
        [JsonProperty]
        public string ReadToken { get; set; } = string.Empty;
        [JsonProperty]
        public string WriteToken { get; set; } = string.Empty;
        [JsonProperty]
        public int MaxPayloadsCount { get; set; } = 200;
        [JsonProperty]
        public Int64 MaxTotalPayloadsSize { get; set; } = 50000000;
        [JsonProperty]
        public int MaxReferencesCount { get; set; } = 10;
        [JsonProperty]
        public Int64 MaxTotalReferencesSize { get; set; } = 5000000;

        // Operational Tracking
        [JsonProperty]
        public DateTime Created { get; set; } = DateTime.Now;
        [JsonProperty]
        public DateTime Reset { get; set; } = DateTime.Now;
        [JsonProperty]
        public Int64 PayloadReadSize { get; set; } = 0;
        [JsonProperty]
        public Int64 PayloadWriteSize { get; set; } = 0;
        [JsonProperty]
        public int PayloadReadCount { get; set; } = 0;
        [JsonProperty]
        public int PayloadReadFailCount { get; set; } = 0;
        [JsonProperty]
        public int PayloadWriteCount { get; set; } = 0;
        [JsonProperty]
        public int PayloadWriteFailCount { get; set; } = 0;
        [JsonProperty]
        public DateTime PayloadLastReadOn { get; set; } = DateTime.MinValue;
        [JsonProperty]
        public DateTime PayloadLastWriteOn { get; set; } = DateTime.MinValue;

        [JsonProperty]
        public Int64 ReferenceReadSize { get; set; } = 0;
        [JsonProperty]
        public Int64 ReferenceWriteSize { get; set; } = 0;
        [JsonProperty]
        public int ReferenceReadCount { get; set; } = 0;
        [JsonProperty]
        public int ReferenceReadFailCount { get; set; } = 0;
        [JsonProperty]
        public int ReferenceWriteCount { get; set; } = 0;
        [JsonProperty]
        public int ReferenceWriteFailCount { get; set; } = 0;
        [JsonProperty]
        public DateTime ReferenceLastReadOn { get; set; } = DateTime.MinValue;
        [JsonProperty]
        public DateTime ReferenceLastWriteOn { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public ConcurrentQueue<Payload> Payloads { get; set; } = new ConcurrentQueue<Payload>();
        [JsonProperty]
        public int PayloadsCount { get { return Payloads.Count; } }
        [JsonProperty]
        public Int64 PayloadsSize { get; set; } = 0;

        [JsonIgnore]
        public ConcurrentDictionary<string, Payload> References { get; set; } = new ConcurrentDictionary<string, Payload>();
        [JsonProperty]
        public int ReferencesCount { get { return References.Count; } }
        [JsonProperty]
        public Int64 ReferencesSize { get; set; } = 0;
    }
}
