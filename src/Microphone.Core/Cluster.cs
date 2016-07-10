using Microphone.Core.ClusterProviders;
using Microphone.Core.Util;
using Microsoft.Extensions.Logging;

namespace Microphone.Core
{
    public static class Cluster
    {
        private static IClusterClient agent;
        public static IClusterClient Agent => agent;

        public static void RegisterService(IFrameworkProvider frameworkProvider, IClusterProvider clusterProvider,
            string serviceName, string version, ILogger log)
        {
            log.LogInformation("Bootstrapping Microphone");
            var uri = frameworkProvider.GetUri();
            var serviceId = $"{serviceName}_{DnsUtils.GetLocalEscapedIPAddress()}_{uri.Port}";
            try
            {
                clusterProvider.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
            }
            catch
            {
                log.LogError($"Could not register service {serviceId} using {frameworkProvider.GetType().Name}");
            }
            agent = clusterProvider;
        }
    }
}
