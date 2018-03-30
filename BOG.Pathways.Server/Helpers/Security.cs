using BOG.Pathways.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.Helpers
{
    public class Security
    {
        public enum AccessLevel : int { Super = 3, Admin = 2, User = 1, None = 0 }
        public enum PathwayAccessLevel : int { None = 0, Read = 1, Write = 2 }
        IStorage _storage;

        public Security(IStorage storage)
        {
            _storage = storage;
        }

        public AccessLevel ValidateAccessToken(string ipAddress, string token)
        {
            var result = AccessLevel.None;
            if (string.Compare(token, _storage.Configuration.SuperAccessToken, false) == 0)
            {
                result = AccessLevel.Super;
            }
            else if (string.Compare(token, _storage.Configuration.AdminAccessToken, false) == 0)
            {
                result = AccessLevel.Admin;
            }
            else if (string.Compare(token, _storage.Configuration.UserAccessToken, false) == 0)
            {
                result = AccessLevel.User;
            }

            return result;
        }

        public bool IsWhiteListed (string ip)
        {
            return _storage.Configuration.IpWhitelist.Contains(ip);
        }

        public bool PathwayExists(string pathway)
        {
            return _storage.PathwayList.ContainsKey(pathway);
        }

        public PathwayAccessLevel ValidatePathwayToken(string pathway, string token)
        {
            if (!PathwayExists(pathway)) throw new ArgumentException($"Unknown pathway specified: {pathway}");

            var result = PathwayAccessLevel.None;

            if (string.Compare(token, _storage.PathwayList[pathway].WriteToken, false) == 0)
            {
                result = PathwayAccessLevel.Write;
            }
            else if (string.Compare(token, _storage.PathwayList[pathway].ReadToken, false) == 0)
            {
                result = PathwayAccessLevel.Read;
            }
            return result;
        }
    }
}
