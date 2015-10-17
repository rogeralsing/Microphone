using System;
using System.Net;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Consul;

namespace Microphone.WebApi
{
    public static class Bootstrap
    {
        public static string ServiceName { get; private set; }
        public static string Version { get; private set; }

        public static void Start(string serviceName, string version)
        {
            ServiceName = serviceName;
            Version = version;
            var serviceId = serviceName + Guid.NewGuid();

            var uri = GetUri();
            var config = new HttpSelfHostConfiguration(uri);

            Console.WriteLine("{0} running on {1}", serviceId, uri);

            var client = new Client();
            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Address = uri.Host,
                ID = serviceId,
                Name = serviceName,
                Port = uri.Port,
                Tags = new[] { version },
                Check = new AgentServiceCheck
                {
                    HTTP = uri + "status",
                    Interval = TimeSpan.FromSeconds(1),
                    TTL = TimeSpan.Zero,
                    Timeout = TimeSpan.Zero
                }
            });

            config.Routes.MapHttpRoute(
                "API Default", "{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }


        private static Uri GetUri(int port = 0)
        {
            port = port == 0 ? FreeTcpPort() : port;
            var uri = new Uri("http://localhost:" + port);
            return uri;
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
