using System;

namespace Microphone.Core.ClusterProviders
{
    public class EtcdProvider : IClusterProvider
    {
        public ServiceInformation[] FindService(string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterService(string serviceName, string serviceId, string version, Uri uri)
        {
            throw new NotImplementedException();
        }

        public string GetConfig()
        {
            throw new NotImplementedException();
        }
    }
}
