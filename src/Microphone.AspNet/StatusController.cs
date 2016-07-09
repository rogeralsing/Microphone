using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Microphone.AspNet
{
    [Route("/status")]
    public class StatusController : Controller
    {
        private ILogger _log;
        public StatusController(ILoggerFactory loggerFactory) {
            _log = loggerFactory.CreateLogger("Microphone");
        }
        [HttpGet]
        public object Get()
        {
            var machine = System.Environment.MachineName;
            _log.LogInformation("Status OK");
            var status = new {
                Machine = machine,
                Result = "OK",
            };
            return status;
        }
    }
}