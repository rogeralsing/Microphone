using System;
using System.Threading.Tasks;
using Microphone.Core.Util;
using Newtonsoft.Json;

namespace Microphone
{
    public static class ClusterProviderExtensions
    {
        public static async Task<T> KeyValueGetAsync<T>(this IKeyValueStore self, string key)
        {
            var strValue = await self.KeyValueGetAsync(key);
            return JsonConvert.DeserializeObject<T>(strValue);
        }
        public static Task KeyValuePutAsync(this IKeyValueStore self,string key, object value)
        {
            var json = JsonConvert.SerializeObject(value);
            return self.KeyValuePutAsync(key, json);
        }
        public static async Task<ServiceInformation> FindServiceInstanceAsync(this IClusterClient self,
            string serviceName)
        {
            var res = await self.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            if (res.Length == 0)
                throw new Exception($"No healthy instance of the service '{serviceName}' was found");

            return res[ThreadLocalRandom.Current.Next(0, res.Length)];
        }
    }
}