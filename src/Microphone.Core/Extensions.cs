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

        public static Uri ResolveUri(this IServiceDiscovery self, string serviceName, Uri relativeUri, string scheme = "http")
        {
            return self.ResolveUriAsync(serviceName,relativeUri,scheme).Result;
        }
        public static Uri ResolveUri(this IServiceDiscovery self, string serviceName, string relativeUri, string scheme = "http")
        {
            return self.ResolveUriAsync(serviceName,relativeUri,scheme).Result;
        }

        public static async Task<Uri> ResolveUriAsync(this IServiceDiscovery self, string serviceName, Uri relativeUri, string scheme = "http")
        {
            if (relativeUri.IsAbsoluteUri)
                throw new ArgumentException($"{nameof(relativeUri)} should be relative",nameof(relativeUri));

            if (string.IsNullOrWhiteSpace(scheme))
                throw new ArgumentException($"{nameof(scheme)} may not be null or whitespace",nameof(scheme));

	        if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException($"{nameof(serviceName)} may not be null or whitespace",nameof(serviceName));

            var res = await self.ResolveServicesAsync(serviceName).ConfigureAwait(false);
            if (res.Length == 0)
                throw new Exception($"No healthy instance of the service '{serviceName}' was found");

            var service = res[ThreadLocalRandom.Current.Next(0, res.Length)];
            var baseUri = new Uri($"{scheme}://{service.Host}:{service.Port}");
            Uri uri;
            if (!Uri.TryCreate(baseUri,relativeUri,out uri)){
                return uri;
            }
            throw new Exception("Failed to combine absolute and relative Uri");
        }
        //Single instance
        public static Task<Uri> ResolveUriAsync(this IServiceDiscovery self, string serviceName, string relativeUri, string scheme = "http")
        {
            return self.ResolveUriAsync(serviceName,new Uri(relativeUri,UriKind.Relative),scheme);         
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
