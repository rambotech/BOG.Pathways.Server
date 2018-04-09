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
using static BOG.Pathways.Server.Helpers.Security;
using System.Text;

namespace BOG.Pathways.Server.Controllers
{
    /// <summary>
    /// API Controller for pathway actions.
    /// </summary>
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
        /// Create a pathway
        /// </summary>
        /// <returns>varies: see method declaration</returns>
        [HttpPost("create", Name = "CreatePathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(409, Type = typeof(ErrorResponse))]
        [ProducesResponseType(429, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult CreatePathway([FromHeader(Name = "Access-Token")] string accessToken, [FromBody] CreatePathwayRequest pathwayReq)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/create )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                case Security.AccessLevel.User:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token");
                    return Unauthorized();
            }
            if (!_security.IsValidId(pathwayReq.Id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {pathwayReq.Id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (_storage.PathwayList.ContainsKey(pathwayReq.Id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {pathwayReq.Id} already exists.");
                return StatusCode(409, ErrorResponseManifest.Get(2));
            }
            if (_storage.PathwayList.Count >= _settings.Value.PathwayMaximumPayloads)
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {pathwayReq.Id} at maximum payloads.");
                return StatusCode(429, ErrorResponseManifest.Get(3));
            }
            _storage.PathwayList.Add(pathwayReq.Id, new StorageModels.Pathway
            {
                Id = pathwayReq.Id,
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
        /// <returns>varies: see method declaration</returns>
        [HttpGet("view/{id}", Name = "GetPathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200, Type = typeof(GetPathwayResponse))]
        [ProducesResponseType(204)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult GetPathway(
            [FromHeader(Name = "Access-Token")] string accessToken,
            [FromHeader(Name = "Pathway-Token")] string pathwayToken,
            string id)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                case Security.AccessLevel.User:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token value");
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
        /// Remove a pathway: drops all payloads and references with it.
        /// </summary>
        /// <returns>varies: see method declaration</returns>
        [HttpDelete("delete/{id}", Name = "DeletePathway")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult DeletePathway(
            [FromHeader(Name = "Access-Token")] string accessToken,
            string id)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/delete/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                case Security.AccessLevel.User:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token value");
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
            _storage.PathwayList.Remove(id);
            return Ok(ErrorResponseManifest.Get(0));
        }

        /// <summary>
        /// Deposit a payload to a pathway
        /// </summary>
        /// <returns>varies: see method declaration</returns>
        [HttpPost("deposit/{id}", Name = "DepositPathwayPayload")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(405, Type = typeof(ErrorResponse))]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(429, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult DepositPathwayPayload(
            [FromHeader(Name = "Access-Token")] string accessToken,
            [FromHeader(Name = "Pathway-Token")] string pathwayToken,
            string id,
            [FromBody] string payload)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/deposit/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token value");
                    return Unauthorized();
            }
            if (!_security.IsValidId(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (!_storage.PathwayList.ContainsKey(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} not found.");
                return NoContent();
            }
            var pathway = _storage.PathwayList[id];
            pathway.PayloadWriteCount++;
            pathway.PayloadWriteFailCount++;
            switch (ValidatePathwayToken(id, pathwayToken))
            {
                case PathwayAccessLevel.None:
                    _logger.LogInformation($"{clientIpAddress}: pathway {id} deposit denied: no access.");
                    return StatusCode(405, ErrorResponseManifest.Get(8));

                case PathwayAccessLevel.Read:
                    _logger.LogInformation($"{clientIpAddress}: pathway {id} deposit denied: read access supplied.");
                    return StatusCode(405, ErrorResponseManifest.Get(9));
            }
            if (pathway.PayloadsCount >= pathway.MaxPayloadsCount)
            {
                _logger.LogInformation($"{clientIpAddress}: pathway deposit would exceed pathway maximum payload count.");
                return StatusCode(429, ErrorResponseManifest.Get(4));
            }
            var newPayload = new Payload
            {
                Content = payload
            };
            if (this.HttpContext.Request.Headers.ContainsKey("Content-Type"))
            {
                newPayload.ContentType = this.HttpContext.Request.Headers["Content-Type"];
            }
            if (this.HttpContext.Request.Headers.ContainsKey("Content-Transfer-Encoding"))
            {
                newPayload.ContentTransferEncoding = this.HttpContext.Request.Headers["Content-Transfer-Encoding"];
            }
            pathway.Payloads.Enqueue(newPayload);
            pathway.PayloadsSize += newPayload.Content.Length;
            pathway.PayloadWriteFailCount--;
            return Ok(ErrorResponseManifest.Get(0));
        }

        /// <summary>
        /// Withdraw a payload from a pathway
        /// </summary>
        /// <returns>varies: see method declaration</returns>
        [HttpGet("withdraw/{id}", Name = "WithdrawPathwayPayload")]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405, Type = typeof(ErrorResponse))]
        [ProducesResponseType(429, Type = typeof(ErrorResponse))]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(204)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public IActionResult WithdrawPathwayPayload(
            [FromHeader(Name = "Access-Token")] string accessToken,
            [FromHeader(Name = "Pathway-Token")] string pathwayToken,
            string id)
        {
            string clientIpAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{clientIpAddress}: Method call to ( /api/pathway/withdraw/{{id}} )");
            _storage.IpWatchList[clientIpAddress].MethodCallTally++;
            switch (_security.ValidateAccessToken(clientIpAddress, accessToken ?? string.Empty))
            {
                case Security.AccessLevel.None:
                    _storage.IpWatchList[clientIpAddress].FailedAttempts++;
                    _logger.LogInformation($"{clientIpAddress}: invalid access token value");
                    return Unauthorized();
            }
            if (!_security.IsValidId(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway identifier not a valid format: {id}.");
                return BadRequest(ErrorResponseManifest.Get(1));
            }
            if (!_storage.PathwayList.ContainsKey(id))
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} not found.");
                return NotFound();
            }
            switch (ValidatePathwayToken(id, pathwayToken))
            {
                case PathwayAccessLevel.None:
                    _logger.LogInformation($"{clientIpAddress}: pathway {id} withdrawal denied: no access.");
                    return StatusCode(405, ErrorResponseManifest.Get(8));

                case PathwayAccessLevel.Write:
                    _logger.LogInformation($"{clientIpAddress}: pathway {id} withdrawal denied: write access supplied.");
                    return StatusCode(405, ErrorResponseManifest.Get(9));
            }
            var pathway = _storage.PathwayList[id];
            if (pathway.Payloads.Count == 0)
            {
                _logger.LogInformation($"{clientIpAddress}: pathway {id} has no payloads.");
                return NoContent();
            }
            Payload thisPayload = null;
            int retries = 3;
            while (retries >= 0)
            {
                if (_storage.PathwayList[id].Payloads.TryDequeue(out thisPayload)) break;
                retries--;
            }
            if (retries == -1)
            {
                _logger.LogError($"{clientIpAddress}: pathway {id} not able to retrieve payload within three tries.");
                return StatusCode(429, ErrorResponseManifest.Get(10));
            }

            Response.ContentType = thisPayload.ContentType;
            this.HttpContext.Response.Headers["Content-Transfer-Encoding"] = thisPayload.ContentTransferEncoding;
            return Ok(thisPayload.Content);
        }

        //public IActionResult SetPathwayReference()
        //{
        //    return NotFound();
        //}

        //public IActionResult RemovePathwayReference()
        //{
        //    return NotFound();
        //}

        #region Private Helpers

        private PathwayAccessLevel ValidatePathwayToken(string pathwayId, string tokenValue)
        {
            var result = PathwayAccessLevel.None;
            if (_storage.PathwayList.ContainsKey(pathwayId))
            {
                if (string.Compare(_storage.PathwayList[pathwayId].ReadToken, tokenValue, false) == 0)
                {
                    result = PathwayAccessLevel.Read;
                }
                else if (string.Compare(_storage.PathwayList[pathwayId].WriteToken, tokenValue, false) == 0)
                {
                    result = PathwayAccessLevel.Write;
                }
            }
            return result;
        }

        private GetPathwayResponse PathwayToGerPathwayResponse(BOG.Pathways.Server.StorageModels.Pathway pathway)
        {
            return new GetPathwayResponse
            {
                Name = pathway.Id,
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
