using System;

namespace Microphone.Core.ClusterProviders
{
    public interface IClusterProvider
    {
        ServiceInformation[] FindService(string name);
        void RegisterService(string serviceName, string serviceId, string version, Uri uri);
        string GetConfig();
    }
}
