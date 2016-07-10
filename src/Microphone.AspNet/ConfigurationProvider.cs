using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Microphone.AspNet
{
    public class MicrophoneConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MicrophoneConfigurationProvider();
        }
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

            value = Cluster.Agent.KeyValueGetAsync(key).Result;
            return true;
        }
    }
}