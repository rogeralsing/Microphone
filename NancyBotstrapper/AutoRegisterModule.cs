using System.Collections.Generic;
using System.Linq;
using Consul;
using Nancy;

namespace NancyBotstrapper
{
    public abstract class AutoRegisterModule : NancyModule
    {
        private readonly Logger _logger;

        protected Logger Logger => _logger;

        protected AutoRegisterModule()
        {
            _logger = new Logger();
            Before += ctx =>
            {
                _logger.Information("{Url} - {Method}", ctx.Request.Url , ctx.Request.Method );
                return null;
            };

            Get["/status"] = _ => "ok";
        }

        protected IEnumerable<ServiceInformation> FindService(string name)
        {
            Logger.Information("{ServiceName} lookup {OtherServiceName}",Bootstrap.ServiceName,name);
            var client = new Client();
            var others = client.Catalog.Service(name);

            return others.Response.Select(other => new ServiceInformation(other.ServiceAddress,other.ServicePort));
        }
    }
}
