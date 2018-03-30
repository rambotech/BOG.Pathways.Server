﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.StorageModels
{
    public class IpWatch
    {
        public string IpAddress { get; set; } = string.Empty;
        public bool IsWhitelisted { get; set; } = false;
        public int BadTokenTally { get; set; } = 0;
        public DateTime LatestAttempt { get; set; } = DateTime.MinValue;
        public int FailedAttempts { get; set; } = 0;
        public int MethodCallTally { get; set; } = 0;
        public int PublicCallTally { get; set; } = 0;
    }
}