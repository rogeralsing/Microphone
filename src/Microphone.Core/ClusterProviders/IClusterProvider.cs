using System;
using System.Threading.Tasks;
using Microphone.Core.Util;

namespace Microphone.Core.ClusterProviders
{
    public interface IServiceDiscovery 
    {
        Task<ServiceInformation[]> FindServiceInstancesAsync(string name);
    }
    public interface IKeyValueStore {
	    Task KeyValuePutAsync(string key, object value);
        Task<T> KeyValueGetAsync<T>(string key);
    }
    public interface IClusterAgent : IServiceDiscovery, IKeyValueStore {
    }
    
    public interface IClusterProvider : IClusterAgent
    {
        Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri);
        Task BootstrapClientAsync();
    }

    public static class ClusterProviderExtensions
    {
        public static async Task<ServiceInformation> FindServiceInstanceAsync(this IClusterAgent self,
            string serviceName)
        {
            var res = await self.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            if (res.Length == 0)
                throw new Exception("Could not find service");

            return res[ThreadLocalRandom.Current.Next(0, res.Length)];
        }
    }
}