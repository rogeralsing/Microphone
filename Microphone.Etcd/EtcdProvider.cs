using System;
using System.Net.Http;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microphone.Etcd
{
    public class EtcdProvider : IClusterProvider
    {
        public ServiceInformation[] FindService(string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterService(string serviceName, string serviceId, string version, Uri uri)
        {
            RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            var url = $"http://127.0.0.1:2379/v2/keys/services/{serviceName}/{serviceId}";
            var client = new HttpClient();
            var payload = new
            {
                value = uri.ToString(),
                xyz = "1123",
                ttl = "1000"
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json);
            var response = await client.PutAsync(url, content).ConfigureAwait(false);

        }

        public string GetConfig()
        {
            return "";
        }       
    }
}
