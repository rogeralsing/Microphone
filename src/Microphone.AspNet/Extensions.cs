using System;
using System.Linq;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Microphone.AspNet
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, ILoggerFactory loggingFactory, IClusterProvider clusterProvider,
            string serviceName, string version)
        {
             var features = self.Properties["server.Features"] as FeatureCollection;
             
             var logger = loggingFactory.CreateLogger("Microphone");
            try
            {
               
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First().Replace("*", "localhost");
                var uri = new Uri(address);
                
                Microphone.Core.Logger.SetLogger(logger);
                Cluster.Bootstrap(new AspNetProvider(uri.Port), clusterProvider, serviceName, version);
            }
            catch(Exception x)
            {
               logger.LogCritical(x.ToString());
            }
            return self;
        }
    }
}