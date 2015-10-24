using Microphone.Core.Util;
using System;
using System.Threading.Tasks;

namespace Microphone.Core.ClusterProviders
{
    public interface IClusterProvider
    {
        Task<ServiceInformation[]> FindServiceInstancesAsync(string name);
        Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri);
    }

    public static class ClusterProviderExtensions
    {
        public async static Task<ServiceInformation> FindServiceInstanceAsync(this IClusterProvider self, string serviceName)
        {
            var res = await self.FindServiceInstancesAsync(serviceName);
            if (res.Length == 0)
                throw new Exception("Could not find service");

            return res[ThreadLocalRandom.Current.Next(0, res.Length - 1)];
        }
    }
}
