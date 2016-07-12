using System;
using Microphone.Core.Util;
using Microsoft.Extensions.Logging;

namespace Microphone
{
    public static class Cluster
    {
        private static IClusterClient agent;
        public static IClusterClient Client => agent;

        public static void RegisterService(Uri uri, IClusterProvider clusterProvider,
            string serviceName, string version, ILogger log)
        {          
            log.LogInformation("Bootstrapping Microphone");
            var serviceId = $"{serviceName}_{DnsUtils.GetLocalEscapedIPAddress()}_{uri.Port}";
            try
            {
                clusterProvider.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
            }
            catch
            {
                log.LogError($"Could not register service {serviceId}");
            }
            agent = clusterProvider;
        }
    }
}
