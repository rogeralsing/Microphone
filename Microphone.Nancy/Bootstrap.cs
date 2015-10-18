using System;
using System.Threading.Tasks;
using Microphone.Core;
using Nancy;
using Nancy.Hosting.Self;

namespace Microphone.Nancy
{
    public class Bootstrap
    {

        public static void Start(string serviceName, string version)
        {
            var serviceId = serviceName + Guid.NewGuid();
            var uri = Configuration.GetUri();
            var conf = GetConfiguration();
            var host = GetHost(uri, conf);

            Console.WriteLine("{0} running on {1}", serviceId, uri);

            Cluster.RegisterService(serviceName, serviceId, version, uri);
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
    }
}