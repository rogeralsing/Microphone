using System;
using Microphone.Core.ClusterProviders;

namespace Microphone.Core
{
    public static class Cluster
    {
        private static IClusterProvider clusterProvider;
        private static IFrameworkProvider _frameworkProvider;

        public static string GetConfig()
        {
            return clusterProvider.GetConfig();
        }

        public static ServiceInformation[] FindService(string name)
        {
            return clusterProvider.FindService(name);
        }

        public static void Bootstrap<TFramework, TProvider>(string serviceName, string version)
            where TFramework : IFrameworkProvider, new()
            where TProvider : IClusterProvider, new()
        {
            _frameworkProvider = new TFramework();
            var uri = _frameworkProvider.Start(serviceName, version);
            var serviceId = serviceName + Guid.NewGuid();
            clusterProvider = new TProvider();
            clusterProvider.RegisterService(serviceName, serviceId, version, uri);
        }          
    }
}