using Microphone.Core;
using Microsoft.AspNetCore.Mvc;

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