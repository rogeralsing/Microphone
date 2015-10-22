using System;
using System.Threading.Tasks;

namespace Microphone.Core.ClusterProviders
{
    public interface IClusterProvider
    {
        Task<ServiceInformation[]> FindServiceAsync(string name);
        Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri);
    }
}
