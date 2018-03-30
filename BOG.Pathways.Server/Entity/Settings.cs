using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOG.Pathways.Server.Entity
{
    /// <summary>
    /// The setting for the web server operation.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The site token which allows super, admin and user functions.
        /// </summary>
        public string SuperAccessToken { get; set; } = "Super";
        /// <summary>
        /// The site token which allows admin and user functions.
        /// </summary>
        public string AdminAccessToken { get; set; } = "Admin";
        /// <summary>
        /// The site token which allows user functions.
        /// </summary>
        public string UserAccessToken { get; set; } = "User";
        /// <summary>
        /// The port number for unsecure http service.
        /// </summary>
        public int HttpPortNumber { get; set; } = 5670;
        /// <summary>
        /// The port number for secure https service.
        /// </summary>
        public int HttpsPortNumber { get; set; } = 5671;
        /// <summary>
        /// The maximum size of all payloads which a pathway can contain at any given time.
        /// </summary>
        public Int64 PayloadSizeLimitPathway { get; set; } = 50 * 1024 * 1024;
        /// <summary>
        /// The maximum numbedr of payloads which a pathway can contain at any given time.
        /// </summary>
        public int PayloadCountLimitPathway { get; set; } = 200;
        /// <summary>
        /// The maximum size of all payloads in all pathways which is allowed at any given time.
        /// </summary>
        public Int64 PayloadSizeLimitTotal { get; set; } = 500 * 1024 * 1024;
        /// <summary>
        /// The maximum number of payloads in all pathways which is allowed at any given time.
        /// </summary>
        public int PayloadCountLimitTotal { get; set; } = 2000;
        /// <summary>
        /// The lowest level of logging which should be written to console output.
        /// 0=Trace, 1=Debug, 2=Information, 3=Warning, 4= Error
        /// </summary>
        public int LoggingLevel { get; set; } = 2;
        /// <summary>
        /// An optional list of IP addresses which are always treated as friendly (i.e. allowed unchallenged).
        /// </summary>
        public string[] IpWhitelist { get; set; } = new string[] { };

        /// <summary>
        /// Validates all the settings.
        /// </summary>
        /// <param name="faliureDetails">A list of setting properties found with issues.</param>
        /// <returns>true if validate, false if not.</returns>
        public bool TryValidate(out string faliureDetails)
        {
            StringBuilder details = new StringBuilder();
            if (string.IsNullOrWhiteSpace(this.SuperAccessToken))
                details.AppendLine("SuperAccessToken can not be empty");
            if (string.IsNullOrWhiteSpace(AdminAccessToken))
                details.AppendLine("AdminAccessToken can not be empty");
            if (string.IsNullOrWhiteSpace(UserAccessToken))
                details.AppendLine("UserAccessToken can not be empty");
            if (SuperAccessToken == AdminAccessToken || AdminAccessToken == UserAccessToken || SuperAccessToken == UserAccessToken)
                details.AppendLine("Super, Admin and User AccessTokens can not have duplicate values");
            if (HttpPortNumber <= 0 || HttpPortNumber >= 65534)
                details.AppendLine("HttpPortNumber is out of allowed range (1, 65534)");
            if (HttpsPortNumber <= 0 || HttpsPortNumber >= 65534)
                details.AppendLine("HttpsPortNumber is out of allowed range (1, 65534)");
            if (HttpPortNumber == HttpsPortNumber)
                details.AppendLine("HttpPortNumber and HttpsPortNumber can not contain the same port value");
            if (PayloadCountLimitPathway < 1 || PayloadCountLimitPathway > 1000)
                details.AppendLine("PayloadCountLimitPathway is out of range");
            if (PayloadSizeLimitPathway < 500 * 1024 || PayloadSizeLimitPathway > 500 * 1024 * 1024)
                details.AppendLine("PayloadSizeLimitPathway is out of range");
            if (PayloadCountLimitTotal < 1 || PayloadCountLimitTotal > 2000)
                details.AppendLine("PayloadCountLimitTotal is out of range");
            if (PayloadSizeLimitTotal < 500 * 1024 || PayloadSizeLimitTotal > 2147483648)
                details.AppendLine("PayloadSizeLimitTotal is out of range");
            if (LoggingLevel < 0 || LoggingLevel > 4)
            {
                details.AppendLine("LoggingLevel is out of allowed range (1, 65534)");
                foreach (string ip in IpWhitelist)
                {
                    IPAddress ipAddr = null;
                    bool isValid = IPAddress.TryParse(ip, out ipAddr);
                    if (!isValid)
                    {
                        details.AppendLine("IPWhitelist contains one or more invalid addresses");
                    }
                }
            }

            faliureDetails = details.ToString();
            return faliureDetails.Length == 0;
        }
    }
}
