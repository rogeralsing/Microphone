using System;
using System.Linq;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Server.Features;

namespace Microphone.AspNet
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, IClusterProvider clusterProvider,
            string serviceName, string version)
        {
            var features = self.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First().Replace("*", "localhost");
            var uri = new Uri(address);
            Cluster.Bootstrap(new AspNetProvider(uri.Port), clusterProvider, serviceName, version);
            return self;
        }
    }
}