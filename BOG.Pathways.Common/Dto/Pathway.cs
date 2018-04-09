using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOG.Pathways.Common.Dto
{
    /// <summary>
    /// Container for pathway request data to server
    /// </summary>
    public class CreatePathwayRequest
    {
        [JsonProperty]
        public string Id { get; set; }
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
    }

    public class CreatePathwayResponse
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// Container for pathway response to clinet.
    /// </summary>
    public class GetPathwayResponse
    {
        [JsonProperty]
        public string Name { get; set; } = string.Empty;
        [JsonProperty]
        public int MaxPayloadsCount { get; set; } = 200;
        [JsonProperty]
        public Int64 MaxTotalPayloadsSize { get; set; } = 50000000;
        [JsonProperty]
        public int MaxReferencesCount { get; set; } = 10;
        [JsonProperty]
        public Int64 MaxTotalReferencesSize { get; set; } = 5000000;
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

        [JsonProperty]
        public int PayloadsCount { get; set; } = 0;
        [JsonProperty]
        public Int64 PayloadsSize { get; set; } = 0;

        [JsonProperty]
        public int ReferencesCount { get; set; } = 0;
        [JsonProperty]
        public Int64 ReferencesSize { get; set; } = 0;
    }

    /// <summary>
    /// Container for pathway response to clinet.
    /// </summary>
    public class GetPathwayListResponse
    {
        List<GetPathwayResponse> PathwayList = new List<GetPathwayResponse>();
    }
}
