using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microphone;
using System.Linq;

namespace AspNetService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get([FromServices]IClusterClient client)
        {
            var res = client.ResolveServicesAsync("AspNetService").Result.Select(s => s.Address).ToArray();
            return res;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "hello";
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
