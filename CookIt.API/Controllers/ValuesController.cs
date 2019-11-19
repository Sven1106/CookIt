using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CookIt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext context;
        public ValuesController(DataContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var bla = this.context.Words.ToList();
            return Ok(new string[] { "here", "there" });
        }
    }
}
