using System;
using System.Threading.Tasks;
using Microphone.Core.Util;
using Newtonsoft.Json;

namespace Microphone
{
    public static class ClusterProviderExtensions
    {
        //Sync API
        public static ServiceInformation[] ResolveServices(this IServiceDiscovery self, string serviceName)
        {
            return self.ResolveServicesAsync(serviceName).Result;
        }

        public static ServiceInformation ResolveService(this IServiceDiscovery self, string serviceName)
        {
            return self.ResolveServiceAsync(serviceName).Result;
        }

        //Single instance
        public static async Task<ServiceInformation> ResolveServiceAsync(this IServiceDiscovery self, string serviceName)
        {
            var res = await self.ResolveServicesAsync(serviceName).ConfigureAwait(false);
            if (res.Length == 0)
                throw new Exception($"No healthy instance of the service '{serviceName}' was found");

            return res[ThreadLocalRandom.Current.Next(0, res.Length)];
        }
        public static async Task<T> KeyValueGetAsync<T>(this IKeyValueStore self, string key)
        {
            var strValue = await self.KeyValueGetAsync(key);
            return JsonConvert.DeserializeObject<T>(strValue);
        }
        public static Task KeyValuePutAsync(this IKeyValueStore self, string key, object value)
        {
            var json = JsonConvert.SerializeObject(value);
            return self.KeyValuePutAsync(key, json);
        }

	    public static void KeyValuePut(this IKeyValueStore self, string key, string value)
        {
            self.KeyValuePutAsync(key,value).Wait();
        }
        public static void KeyValuePut(this IKeyValueStore self, string key, object value)
        {
            self.KeyValuePutAsync(key,value).Wait();
        }
        public static T KeyValueGet<T>(this IKeyValueStore self, string key)
        {
            return self.KeyValueGetAsync<T>(key).Result;
        }

        public static string KeyValueGet(this IKeyValueStore self, string key)
        {
            return self.KeyValueGetAsync(key).Result;
        }
    }
}
