using System;
using System.Net;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microphone.Core;

namespace Microphone.WebApi
{
    public static class Bootstrap
    {
        public static void Start(string serviceName, string version)
        {
            var serviceId = serviceName + Guid.NewGuid();

            var uri = Configuration.GetUri();
            var config = new HttpSelfHostConfiguration(uri);

            Console.WriteLine("{0} running on {1}", serviceId, uri);

            Cluster.RegisterService(serviceName,serviceId,version,uri);

            config.Routes.MapHttpRoute(
                "API Default", "{controller}/{id}",
                new { id = RouteParameter.Optional });

            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }
    }
}
