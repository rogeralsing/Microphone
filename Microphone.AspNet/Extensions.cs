using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microsoft.AspNet.Builder;


namespace Microphone.AspNet
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMicrophone(this IApplicationBuilder self, IClusterProvider clusterProvider, string serviceName, string version)
        {
            Cluster.Bootstrap(new AspNetProvider(), clusterProvider, serviceName, version);
            return self;
        }
    }
}
