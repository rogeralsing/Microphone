using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Microphone.AspNet
{
    public class MicrophoneConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new MicrophoneConfigurationProvider();
    }
    public class MicrophoneConfigurationProvider : IConfigurationProvider
    {
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            return new string[]{};
        }

        public IChangeToken GetReloadToken() => _reloadToken;

        public void Load()
        {
        }

        public void Set(string key, string value)
        {
        }

        public bool TryGet(string key, out string value)
        {
            value = null;
            if (!key.StartsWith("Microphone"))
                return false;

            key = key.Substring("Microphone".Length);

            //HACK: if we could use DI to get the cluster client it would be nicer
            value = Cluster.Client.KeyValueGet(key);
            return true;
        }
    }
}