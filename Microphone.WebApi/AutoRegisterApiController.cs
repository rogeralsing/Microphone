using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Consul;
using Microphone.Core;

namespace Microphone.WebApi
{
    public abstract class AutoRegisterApiController : ApiController
    {
        protected Logger Logger { get; } = new Logger();

        protected string GetConfig()
        {
            var client = new Client();
            var key = "ServiceConfig:" + Bootstrap.ServiceName;
            var response = client.KV.Get(key);
            var res = System.Text.Encoding.UTF8.GetString(response.Response.Value);
            return res;
        }

        protected ServiceInformation[] FindService(string name)
        {
            Logger.Information("{ServiceName} lookup {OtherServiceName}", Bootstrap.ServiceName, name);
            var client = new Client();
            var others = client.Catalog.Service(name);

            return
                others.Response.Select(other => new ServiceInformation(other.ServiceAddress, other.ServicePort))
                    .ToArray();
        }
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