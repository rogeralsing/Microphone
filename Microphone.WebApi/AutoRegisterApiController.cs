using System.Web.Http;
using Microphone.Core;

namespace Microphone.WebApi
{
    public class StatusController : ApiController
    {
        [Route("status")]
        public string GetStatus()
        {
            Logger.Information("OK");
            return "ok";
        }
    }
}