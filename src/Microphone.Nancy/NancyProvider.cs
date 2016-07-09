using System;
using System.Threading.Tasks;
using Microphone.Core;
using Nancy;
using Nancy.Hosting.Self;

namespace Microphone.Nancy
{
    public class NancyProvider : IFrameworkProvider
    {
        public Uri GetUri(string serviceName, string version)
        {
            var uri = Configuration.GetUri();
            var conf = GetConfiguration();
            var host = GetHost(uri, conf);
            return uri;
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