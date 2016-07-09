using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Microphone.AspNet
{
    [Route("/status")]
    public class StatusController : Controller
    {
        private ILogger _log;
        private ICheckHealth _checkHealth;

        public StatusController(ILoggerFactory loggerFactory, ICheckHealth checkHealth=null) {
            _log = loggerFactory.CreateLogger("Microphone");
            _checkHealth = checkHealth;
        }
        [HttpGet]
        public async Task<object> Get()
        {
            var machine = System.Environment.MachineName;
            _log.LogInformation("Status OK");
            if (_checkHealth != null) {
                await _checkHealth.CheckHealth();
            }
            var status = new {
                Machine = machine,
                Result = "OK",
            };
            return status;
        }
    }
}