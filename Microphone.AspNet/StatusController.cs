using Microphone.Core;
using Microsoft.AspNet.Mvc;

namespace Microphone.AspNet
{
    [Route("/status")]
    public class StatusController : Controller
    {
        [HttpGet]
        public string Get()
        {
            Logger.Information("OK");
            return "OK";
        }
    }
}