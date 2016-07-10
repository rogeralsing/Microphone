using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microphone;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System;

namespace AspNetService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get([FromServices]IClusterClient client)
        {
            var res = client.FindServiceInstancesAsync("AspNetService").Result.Select(s => s.Address).ToArray();
            return res;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id, [FromServices]IConfiguration config)
        {
            try
            {
                var res = config[$"Microphone{id}"];
                return "start" + res + "end";
            }
            catch (Exception x)
            {
                return "Error";
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
