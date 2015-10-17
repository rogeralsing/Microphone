using System.Web.Http;
using Microphone.Core;

namespace Microphone.WebApi
{
    public abstract class AutoRegisterApiController : ApiController
    {
        protected Logger Logger { get; } = new Logger();
    }

    public class StatusController : ApiController
    {
        private Logger Logger { get; } = new Logger();
        [Route("status")]
        public string GetStatus()
        {
            Logger.Information("OK");
            return "ok";
        }
    }
}