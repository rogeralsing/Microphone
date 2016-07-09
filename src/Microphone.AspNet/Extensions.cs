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
        public static void AddMicrophone<TCluster>(this IServiceCollection services) where TCluster:class, IClusterProvider 
        {
            services.AddSingleton<IClusterProvider,TCluster>();
            ServiceDescriptor s = new ServiceDescriptor(typeof(IClusterAgent),provider =>provider.GetService<IClusterProvider>(), ServiceLifetime.Singleton);
            services.Add(s);
        }
        public static void AddMicrophone<TCluster,THealthCheck>(this IServiceCollection services) where TCluster:class, IClusterProvider where THealthCheck:class,IHealthCheck 
        {
            services.AddMicrophone<TCluster>();
            services.AddSingleton<IHealthCheck,THealthCheck>();
        }
        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, string serviceName, string version)
        {
            var loggingFactory = self.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var clusterProvider = self.ApplicationServices.GetRequiredService<IClusterProvider>();
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