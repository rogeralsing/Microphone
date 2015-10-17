using System.Collections.Generic;
using System.Linq;
using Consul;
using Microphone.Core;
using Nancy;

namespace Microphone.Nancy
{
    public abstract class AutoRegisterModule : NancyModule
    {
        private readonly Logger _logger;

        protected Logger Logger => _logger;

        protected AutoRegisterModule()
        {
            _logger = new Logger();
            //Before += ctx =>
            //{
            //    _logger.Information("{Url} - {Method}", ctx.Request.Url , ctx.Request.Method );
            //    return null;
            //};

            Get["/status"] = _ =>
            {
                Logger.Information("OK");
                return "ok";
            };
        }

        protected string GetConfig()
        {
            var client = new Client();
            var key = "ServiceConfig:" + Bootstrap.ServiceName;
            var response = client.KV.Get(key);
            var res = System.Text.Encoding.UTF8.GetString(response.Response.Value);
            return res;
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
