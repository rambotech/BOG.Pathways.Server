using BOG.Pathways.Server.StorageModels;
using BOG.Pathways.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.Materialized
{
    public class PathwaySummary : IMaterializedView
    {
        public List<Pathway> Pathways { get; set; } = new List<Pathway>();
    }
}
