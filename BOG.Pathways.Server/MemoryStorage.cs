using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.StorageModels;
using BOG.Pathways.Server.Interface;
using Microsoft.EntityFrameworkCore;

namespace BOG.Pathways.Server
{
    /// <summary>
    /// The collection of items persisted on the server, and static across calls.
    /// </summary>
    public class MemoryStorage : IStorage
    {
        /// <summary>
        /// The collection of IP client addresses and their statistics.
        /// </summary>
        public Dictionary<string, IpWatch> IpWatchList { get; set; } = new Dictionary<string, IpWatch>();
        /// <summary>
        /// The collection of pathways and their data.
        /// </summary>
        public Dictionary<string, Pathway> PathwayList { get; set; } = new Dictionary<string, Pathway>();
        /// <summary>
        /// Settings for the site operation.
        /// </summary>
        public Settings Configuration { get; set; } = new Settings { };
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> IpCaller = new Dictionary<string, string>();

        public MemoryStorage()
        {

        }

        public void Amnesty()
        {
            IpWatchList.Clear();
            foreach (string ip in Configuration.IpWhitelist)
            {
                IpWatchList.Add(ip, new IpWatch { IpAddress = ip, IsWhitelisted = true });
            }
        }

        public void Reset()
        {
            Amnesty();
            PathwayList.Clear();
            //Configuration = Helpers.Converter()
        }
    }
}
