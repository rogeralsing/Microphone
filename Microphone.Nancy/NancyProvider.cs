using System;
using System.Threading.Tasks;
using Microphone.Core;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Bootstrapper;

namespace Microphone.Nancy
{
    public class NancyProvider<TBootstrapper> : IFrameworkProvider where TBootstrapper : INancyBootstrapper
    {
        public Uri Start(string serviceName, string version)
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
                    var bootstrapper = Activator.CreateInstance<TBootstrapper>();
                    var host = new NancyHost(uri, bootstrapper, hostConfigs);
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

    public class NancyProvider : NancyProvider<DefaultNancyBootstrapper>
    {
    }
}
