using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BOG.Pathways.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/pathway")]
    public class PathwayController : Controller
    {
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
