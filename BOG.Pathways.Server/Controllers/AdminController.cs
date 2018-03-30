using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BOG.Pathways.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private IStorage _storage;

        public AdminController(IStorage storage)
        {
            _storage = storage;
        }

        // GET: api/admin/summary
        [HttpGet("summary", Name = "AdminGetSummary")]
        public string List()
        {
            return "value";
        }


    }
}