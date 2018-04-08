using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BOG.Pathways.Common.Dto;
using BOG.Pathways.Server.Helpers;
using BOG.Pathways.Server.Interface;
using BOG.Pathways.Server.Models;
using BOG.Pathways.Server.StorageModels;
using BOG.Pathways.Server.Materialized;
using BOG.Pathways.Server.Entity;

namespace BOG.Pathways.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/pathway")]
    public class PathwayController : Controller
    {
        private IStorage _storage;
        private Security _security;
        private IOptions<BOG.Pathways.Server.StorageModels.Settings> _settings;
        private ILogger<PathwayController> _logger;

        /// <summary>
        /// Instantiate via injection
        /// </summary>
        /// <param name="storage">(injected)</param>
        /// <param name="security">(injected)</param>
        /// <param name="settings">(injected)</param>
        /// <param name="logger">(injected)</param>
        public PathwayController(IStorage storage, Security security, IOptions<BOG.Pathways.Server.StorageModels.Settings> settings, ILogger<PathwayController> logger)
        {
            _storage = storage;
            _security = security;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Return a summary for the specified pathway
        /// </summary>
        /// <returns></returns>
        [HttpPost("create/{id}", Name = "CreatePathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(409, Type = typeof(ErrorResponse))]
        [ProducesResponseType(429, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult CreatePathway([FromHeader(Name = "Access-Token")] string accessToken, [FromRoute] string id, [FromBody] CreatePathwayRequest pathwayReq)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/create/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token");
                    return Unauthorized();
            }
            if (!_security.IsValidId(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (_storage.PathwayList.ContainsKey(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} already exists.");
                return StatusCode(409, ErrorResponseManifest.Get(2));
            }
            if (_storage.PathwayList.Count >= _settings.Value.PathwayMaximumPayloads)
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} at maximum payloads.");
                return StatusCode(409, ErrorResponseManifest.Get(2));
            }
            _storage.PathwayList.Add(id, new StorageModels.Pathway
            {
                Name = pathwayReq.Name,
                ReadToken = pathwayReq.ReadToken,
                WriteToken = pathwayReq.WriteToken,
                MaxPayloadsCount = pathwayReq.MaxPayloadsCount,
                MaxTotalPayloadsSize = pathwayReq.MaxTotalPayloadsSize,
                MaxReferencesCount = pathwayReq.MaxReferencesCount,
                MaxTotalReferencesSize = pathwayReq.MaxTotalReferencesSize
            });
            return Ok();
        }

        /// <summary>
        /// Return a summary for the specified pathway
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetPathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200, Type = typeof(GetPathwayResponse))]
        [ProducesResponseType(204, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult GetPathway([FromHeader(Name = "Access-Token")] string accessToken, string id)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token");
                    return Unauthorized();
            }
            if (!_security.IsValidId(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (!_storage.PathwayList.ContainsKey(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} found.");
                return NoContent();
            }
            var pathway = _storage.PathwayList[id];
            return Ok(PathwayToGerPathwayResponse(pathway));
        }

        /// <summary>
        /// Remove a pathway
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete/{id}", Name = "DeletePathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(204, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult DeletePathway([FromHeader(Name = "Access-Token")] string accessToken, string id)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/delete/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token");
                    return Unauthorized();
            }
            if (!_security.IsValidId(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (!_storage.PathwayList.ContainsKey(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} found.");
                return NoContent();
            }
            var pathway = _storage.PathwayList[id];
            return Ok(PathwayToGerPathwayResponse(pathway));
        }

        #region Private Helpers
        private GetPathwayResponse PathwayToGerPathwayResponse(BOG.Pathways.Server.StorageModels.Pathway pathway)
        {
            return new GetPathwayResponse
            {
                Name = pathway.Name,
                MaxPayloadsCount = pathway.MaxPayloadsCount,
                MaxTotalPayloadsSize = pathway.MaxTotalPayloadsSize,
                MaxReferencesCount = pathway.MaxReferencesCount,
                MaxTotalReferencesSize = pathway.MaxTotalReferencesSize,
                Created = pathway.Created,
                Reset = pathway.Reset,
                PayloadReadSize = pathway.PayloadReadSize,
                PayloadWriteSize = pathway.PayloadWriteSize,
                PayloadReadCount = pathway.PayloadReadCount,
                PayloadReadFailCount = pathway.PayloadReadFailCount,
                PayloadWriteCount = pathway.PayloadWriteCount,
                PayloadWriteFailCount = pathway.PayloadWriteFailCount,
                PayloadLastReadOn = pathway.PayloadLastReadOn,
                PayloadLastWriteOn = pathway.PayloadLastWriteOn,
                ReferenceReadSize = pathway.ReferenceReadSize,
                ReferenceWriteSize = pathway.ReferenceWriteSize,
                ReferenceReadCount = pathway.ReferenceReadCount,
                ReferenceReadFailCount = pathway.ReferenceReadFailCount,
                ReferenceWriteCount = pathway.ReferenceWriteCount,
                ReferenceWriteFailCount = pathway.ReferenceWriteFailCount,
                ReferenceLastReadOn = pathway.ReferenceLastReadOn,
                ReferenceLastWriteOn = pathway.ReferenceLastWriteOn,
                PayloadsCount = pathway.PayloadsCount,
                PayloadsSize = pathway.PayloadsSize,
                ReferencesCount = pathway.ReferencesCount,
                ReferencesSize = pathway.ReferencesSize
            };
        }
        #endregion
    }
}
