using System;
using System.Threading.Tasks;

namespace Microphone
{
    public interface IServiceDiscovery
    {
        Task<ServiceInformation[]> GetServiceInstancesAsync(string name);
    }
    public interface IKeyValueStore
    {
        Task KeyValuePutAsync(string key, string value);
        Task<string> KeyValueGetAsync(string key);
    }
    public interface IClusterClient : IServiceDiscovery, IKeyValueStore
    {
    }

    public interface IClusterProvider : IClusterClient
    {
        Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri);
    }
}
