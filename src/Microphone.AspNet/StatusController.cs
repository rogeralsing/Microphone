using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microphone.Core;

namespace Microphone.AspNet
{
    [Route("/status")]
    public class StatusController : Controller
    {
        private ILogger _log;
        private IHealthCheck _checkHealth;

        public StatusController(ILoggerFactory loggerFactory, IHealthCheck checkHealth = null)
        {
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
