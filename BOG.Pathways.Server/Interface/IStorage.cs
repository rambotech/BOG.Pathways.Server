using BOG.Pathways.Server.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.Interface
{
    public interface IStorage
    {
        Dictionary<string, IpWatch> IpWatchList { get; set; }
        Dictionary<string, Pathway> PathwayList { get; set; }
        Settings Configuration { get; set; }

        void Amnesty();
        void Reset();
    }
}
