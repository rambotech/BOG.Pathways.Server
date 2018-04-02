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
    public class MemoryStorage : IStorage
    {
        public Dictionary<string, IpWatch> IpWatchList { get; set; } = new Dictionary<string, IpWatch>();
        public Dictionary<string, Pathway> PathwayList { get; set; } = new Dictionary<string, Pathway>();
        public Settings Configuration { get; set; } = new Settings { };
        public Dictionary<string, string> IpCaller = new Dictionary<string, string>();

        public MemoryStorage()
        {
            Reset();
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
