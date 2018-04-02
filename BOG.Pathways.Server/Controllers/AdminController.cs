using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.Helpers;
using BOG.Pathways.Server.Interface;
using BOG.Pathways.Server.Materialized;
using BOG.Pathways.Server.StorageModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BOG.Pathways.Server.Controllers
{
    /// <summary>
    /// Controls administration api calls
    /// </summary>
    [Produces("application/json")]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private IStorage _storage;
        private Security _security;
        private IOptions<Settings> _settings;
        private ILogger _logger;

        /// <summary>
        /// Instantiate via injection
        /// </summary>
        /// <param name="storage">(injected)</param>
        /// <param name="security">(injected)</param>
        /// <param name="settings">(injected)</param>
        /// <param name="logger">(injected)</param>
        public AdminController(IStorage storage, Security security, IOptions<Settings> settings, ILogger logger)
        {
            _storage = storage;
            _security = security;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Return a summary of pathway statistics
        /// </summary>
        /// <returns></returns>
        [HttpGet("pathway/summary", Name = "AdminPathwaySummary")]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(200, Type = typeof(PathwaySummary))]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult Summary([FromHeader(Name = "Access-Token")] string accessToken)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/admin/clients )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token");
                    return Unauthorized();
                case Security.AccessLevel.User:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: insufficient access token");
                    return Forbid();
            }
            if (_storage.PathwayList.Count == 0)
            {
                _logger.LogInformation($"{clientIpAddress}: no pathways found.");
                return NotFound();
            }
            var pathwaySummary = new PathwaySummary();
            foreach (var key in _storage.PathwayList.Keys)
            {
                pathwaySummary.Pathways.Add(_storage.PathwayList[key]);
            }
            return Ok(pathwaySummary);
        }
    }
}
