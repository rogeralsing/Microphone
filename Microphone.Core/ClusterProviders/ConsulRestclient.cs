using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microphone.Core.ClusterProviders
{
    public class ConsulRestClient
    {
        public async Task RegisterServiceAsync(string serviceName,string serviceId,Uri address)
        {
            var payload = new
            {
                ID = serviceId,
                Name = serviceName,
                Tags = new string[] {},
                Address = address.Host,
                // ReSharper disable once RedundantAnonymousTypePropertyName
                Port = address.Port,
                Check = new
                {
                    HTTP = address + "status",
                    Interval = "1s"
                }
            };

            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json);

            var res = await client.PostAsync("http://localhost:8500/v1/agent/service/register", content).ConfigureAwait(false);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not register service");
            }
        }

        public async Task<ServiceInformation[]> FindServiceAsync(string serviceName)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("http://localhost:8500/v1/health/service/" + serviceName).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not find services");
            }
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var res = JArray.Parse(body);

            return res.Select(entry => new ServiceInformation(entry["Service"]["Address"].Value<string>(), entry["Service"]["Port"].Value<int>())).ToArray();
        }

        public async Task<string[]> GetCriticalServicesAsync()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("http://localhost:8500/v1/health/state/critical").ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not get service health");
            }
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var res = JArray.Parse(body);
            return res.Cast<JObject>().Select(service => service["ServiceID"].Value<string>()).ToArray();
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            var client = new HttpClient();
            var response =
                await client.GetAsync("http://localhost:8500/v1/agent/service/deregister/" + serviceId).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not de register service");
            }
        }
    }
}
