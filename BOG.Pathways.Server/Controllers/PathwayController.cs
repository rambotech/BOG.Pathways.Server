using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BOG.Pathways.Server.Helpers;
using BOG.Pathways.Server.Interface;
using BOG.Pathways.Server.Models;
using BOG.Pathways.Server.StorageModels;


namespace BOG.Pathways.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/pathway")]
    public class PathwayController : Controller
    {
        private IStorage _storage;
        private Security _security;
        private IOptions<Settings> _settings;
        private ILogger<PathwayController> _logger;

        /// <summary>
        /// Instantiate via injection
        /// </summary>
        /// <param name="storage">(injected)</param>
        /// <param name="security">(injected)</param>
        /// <param name="settings">(injected)</param>
        /// <param name="logger">(injected)</param>
        public PathwayController(IStorage storage, Security security, IOptions<Settings> settings, ILogger<PathwayController> logger)
        {
            _storage = storage;
            _security = security;
            _settings = settings;
            _logger = logger;
        }

        // GET: api/pathway
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/pathway/5
        [HttpGet("list", Name = "Get1")]
        public string List()
        {
            return "value";
        }

        // GET: api/pathway/5
        [HttpGet("{id}", Name = "Get2")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/pathway
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/pathway/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
