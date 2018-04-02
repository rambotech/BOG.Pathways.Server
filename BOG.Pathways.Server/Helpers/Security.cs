using BOG.Pathways.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.Helpers
{
    /// <summary>
    /// Methods for validation of patterns and security
    /// </summary>
    public class Security
    {
        /// <summary>
        /// Site Access Level: the level of access to the site.
        /// </summary>
        public enum AccessLevel : int {
            /// <summary>
            /// Super user
            /// </summary>
            Super = 3,
            /// <summary>
            /// Admininstrator
            /// </summary>
            Admin = 2,
            /// <summary>
            /// User
            /// </summary>
            User = 1,
            /// <summary>
            /// No access
            /// </summary>
            None = 0
        }

        /// <summary>
        /// Pathway Access Level: the level of access to a specific pathway.
        /// </summary>
        public enum PathwayAccessLevel : int
        {
            /// <summary>
            /// No access
            /// </summary>
            None = 0,
            /// <summary>
            /// Read access only.
            /// </summary>
            Read = 1,
            /// <summary>
            /// Write or read access.
            /// </summary>
            Write = 2
        }

        IStorage _storage;

        /// <summary>
        /// Instantiate with injection.
        /// </summary>
        /// <param name="storage"></param>
        public Security(IStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Check the Access token for validity and level of access.
        /// </summary>
        /// <param name="ipAddress">IP Address of the client making the request.</param>
        /// <param name="token">value</param>
        /// <returns>AccessLevel</returns>
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

        /// <summary>
        /// Checks whether an IP Address appears in the whitelist of IP Addresses
        /// </summary>
        /// <param name="ip">The v4 or v6 address.</param>
        /// <returns></returns>
        public bool IsWhiteListed(string ip)
        {
            return _storage.Configuration.IpWhitelist.Contains(IPAddress.Parse(ip).ToString());
        }

        /// <summary>
        /// Checks whether the pathway exists.
        /// </summary>
        /// <param name="pathway">The name of the pathway</param>
        /// <returns>true if it exists</returns>
        public bool PathwayExists(string pathway)
        {
            return _storage.PathwayList.ContainsKey(pathway);
        }

        /// <summary>
        /// Checks the validity of the pathway token for access.
        /// </summary>
        /// <param name="pathway">The name of the pathway to check</param>
        /// <param name="token">The value</param>
        /// <returns>The access level granted by the token value.</returns>
        /// <notes>An error is thrown if the pathway is not found.</notes>
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

        /// <summary>
        /// Ensure a value used as an id meets the character criteria.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if valid pattern.</returns>
        public bool IsValidId(string id)
        {
            var isValid = true;

            for (var index = 0; isValid && index < id.Length; index++)
            {
                var thisChar = id[index]; 
                switch (index)
                {
                    case 0:
                        isValid = (thisChar >= 'A' && thisChar <= 'Z') || (thisChar >= 'a' && thisChar <= 'z');
                        break;

                    default:
                        isValid = thisChar == '.' ||
                            thisChar == '_' || thisChar == '-' || (thisChar >= 'A' && thisChar <= 'Z') ||
                            (thisChar >= 'a' && thisChar <= 'z') || (thisChar >= '0' && thisChar <= '9');
                        break;
                }
            }
            return isValid;
        }
    }
}
