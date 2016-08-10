using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microphone.AspNet
{
    public class MicrophoneBuilder
    {
        private IServiceCollection services;

        public MicrophoneBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public MicrophoneBuilder AddHealthCheck<THealthCheck>() where THealthCheck:class,IHealthCheck
        {
            services.AddSingleton<IHealthCheck,THealthCheck>();
            return this;
        }
    }
    public static class Extensions
    {
        public static IConfigurationBuilder AddMicrophoneKeyValueStore(this IConfigurationBuilder self)
        {
            self.Add(new MicrophoneConfigurationSource());
            return self;
        }
        public static MicrophoneBuilder AddMicrophone<TCluster>(this IServiceCollection services) where TCluster:class, IClusterProvider 
        {
            services.AddOptions();
            services.AddSingleton<IClusterProvider,TCluster>();
            ServiceDescriptor s = new ServiceDescriptor(typeof(IClusterClient),provider =>provider.GetService<IClusterProvider>(), ServiceLifetime.Singleton);
            services.Add(s);
            return new MicrophoneBuilder(services);
        }

        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, string serviceName, string version)
        {
            var features = self.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First().Replace("*", "localhost");
            var tmpuri = new Uri(address);
            var host = Microphone.Util.DnsUtils.GetLocalIPAddress();
            var port = tmpuri.Port;
            var uri = new Uri($"http://{host}:{port}");
            return self.UseMicrophone(serviceName,version,uri);
        }

        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, string serviceName, string version, Uri serviceUri)
        {
            var loggingFactory = self.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var clusterProvider = self.ApplicationServices.GetRequiredService<IClusterProvider>();
            var logger = loggingFactory.CreateLogger("Microphone.AspNet");
            try
            {
                Cluster.RegisterService(serviceUri, clusterProvider, serviceName, version, logger);
            }
            catch(Exception x)
            {
                logger.LogCritical(x.ToString());
            }
            return self;
        }
    }
}