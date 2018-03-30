using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOG.Pathways.Server.Entity
{
    /// <summary>
    /// Container for pathway details: also for storing pathway activity.
    /// </summary>
    public class Pathway
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int MaxPayloadsCount { get; set; } = 200;
        [JsonProperty]
        public Int64 MaxTotalPayloadsSize { get; set; } = 50000000;
        [JsonProperty]
        public int MaxReferencesCount { get; set; } = 10;
        [JsonProperty]
        public Int64 MaxTotalReferencesSize { get; set; } = 5000000;

        [JsonProperty]
        public int PayloadsCount { get; set; } = 0;
        [JsonProperty]
        public Int64 PayloadsSize { get; set; } = 0;
        [JsonProperty]
        public int PayloadsReadCount { get; set; } = 0;
        [JsonProperty]
        public int PayloadsReadFailCount { get; set; } = 0;
        [JsonProperty]
        public Int64 PayloadsReadSize { get; set; } = 0;
        [JsonProperty]
        public int PayloadsWriteCount { get; set; } = 0;
        [JsonProperty]
        public int PayloadsWriteFailCount { get; set; } = 0;
        [JsonProperty]
        public Int64 PayloadsWriteSize { get; set; } = 0;

        [JsonProperty]
        public int ReferencesCount { get; set; } = 0;
        [JsonProperty]
        public Int64 ReferencesSize { get; set; } = 0;

        [JsonProperty]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonProperty]
        public DateTime LastPayloadRead { get; set; } = DateTime.MinValue;
        [JsonProperty]
        public DateTime LastPayloadWrite { get; set; } = DateTime.MinValue;

        [JsonProperty]
        public DateTime LastReferenceRead { get; set; } = DateTime.MinValue;
        [JsonProperty]
        public DateTime LastReferenceWrite { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public Dictionary<string, string> References { get; set; } = new Dictionary<string, string>();
        [JsonIgnore]
        public Queue<Payload> Payloads { get; set; } = new Queue<Payload>();
    }
}
