using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Consul;
using Nancy;
using Nancy.Hosting.Self;

namespace NancyBotstrapper
{
    public class Bootstrap
    {
        public static string ServiceName { get;  private set; }
        public static string Version { get; private set; }

        public static void Start(string serviceName, string version)
        {
            ServiceName = serviceName;
            Version = version;
            var serviceId = serviceName + Guid.NewGuid();
            var uri = GetUri();
            var conf = GetConfiguration();
            var host = GetHost(uri, conf);

            Console.WriteLine("{0} running on {1}", serviceId, uri);

            var client = new Client();
            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Address = uri.Host,
                ID = serviceId,
                Name = serviceName,
                Port = uri.Port,
                Tags = new[] {version},
                Check = new AgentServiceCheck
                {
                    HTTP = uri + "status",
                    Interval = TimeSpan.FromSeconds(1),
                    TTL = TimeSpan.Zero,
                    Timeout = TimeSpan.Zero
                }
            });
        }

        private static NancyHost GetHost(Uri uri, HostConfiguration hostConfigs)
        {
            while (true)
            {
                try
                {
                    var host = new NancyHost(uri, new DefaultNancyBootstrapper(), hostConfigs);
                    host.Start();
                    return host;
                }
                catch
                {
                    Task.Delay(1000).Wait();
                    Console.WriteLine("Port allocation failed, retrying.");
                }
            }
        }

        private static HostConfiguration GetConfiguration()
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations =
                {
                    CreateAutomatically = true
                }
            };
            return hostConfigs;
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
            var port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}