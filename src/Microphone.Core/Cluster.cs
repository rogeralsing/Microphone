using Microphone.Core.ClusterProviders;
using Microphone.Core.Util;
using Microsoft.Extensions.Logging;

namespace Microphone.Core
{
    public static class Cluster
    {
        private static IClusterProvider _clusterProvider;
        private static IFrameworkProvider _frameworkProvider;
        public static IClusterAgent Agent => _clusterProvider;
        public static void BootstrapClient(IClusterProvider clusterProvider)
        {
            _clusterProvider = clusterProvider;
            _clusterProvider.BootstrapClientAsync().Wait();
        }

        public static void RegisterService(IFrameworkProvider frameworkProvider, IClusterProvider clusterProvider,
            string serviceName, string version, ILogger log)
        {
            log.LogInformation("Bootstrapping Microphone");
            _frameworkProvider = frameworkProvider;
            var uri = _frameworkProvider.GetUri();
            var serviceId = $"{serviceName}_{DnsUtils.GetLocalEscapedIPAddress()}_{uri.Port}";
            _clusterProvider = clusterProvider;
            try
            {
                _clusterProvider.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
            }
            catch
            {
                log.LogError($"Could not register service {serviceId} using {frameworkProvider.GetType().Name}");
            }
        }
    }
}