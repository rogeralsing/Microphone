using System;
using System.Linq;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microphone.AspNet
{
    public static class Extensions
    {
        public static void AddMicrophone(this IServiceCollection services) 
        {
            ServiceDescriptor s = new ServiceDescriptor(typeof(IClusterAgent),provider => Cluster.Agent, ServiceLifetime.Transient);
            services.Add(s);
        }
        public static void AddMicrophoneHealthCheck<THealthCheck>(this IServiceCollection services) where THealthCheck:class,IHealthCheck 
        {
            services.AddSingleton<IHealthCheck,THealthCheck>();
        }
        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, ILoggerFactory loggingFactory, IClusterProvider clusterProvider,
            string serviceName, string version)
        {
            var features = self.Properties["server.Features"] as FeatureCollection;
            var logger = loggingFactory.CreateLogger("Microphone.AspNet");
            try
            {
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First().Replace("*", "localhost");
                var uri = new Uri(address);
                Cluster.Bootstrap(new AspNetProvider(uri.Port), clusterProvider, serviceName, version, logger);
            }
            catch(Exception x)
            {
                logger.LogCritical(x.ToString());
            }
            return self;
        }
    }
}