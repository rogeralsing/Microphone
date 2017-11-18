using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microphone.Core.ClusterProviders
{
    public class ConsulRestClient
    {
        private readonly int consulPort = 8500;

        public ConsulRestClient()
        {
            Int32.TryParse(ConfigurationManager.AppSettings["Consul:Port"], out consulPort);
            consulPort = consulPort == 0 ? 8500 : consulPort;
        }

        public ConsulRestClient(int port)
        {
            consulPort = port;
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, Uri address)
        {
            var payload = new
            {
                ID = serviceId,
                Name = serviceName,
                Tags = new[] { $"urlprefix-/{serviceName}" },
                Address = Dns.GetHostName(),
                // ReSharper disable once RedundantAnonymousTypePropertyName
                Port = address.Port,
                Check = new
                {
                    HTTP = address + "status",
                    Interval = "1s"
                }
            };

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json);

                var res =
                    await
                        client.PutAsync($"http://localhost:{consulPort}/v1/agent/service/register", content)
                            .ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not register service");
                }
            }
        }

        public async Task<ServiceInformation[]> FindServiceAsync(string serviceName)
        {
            using (HttpClient client = new HttpClient())
            {
                var response =
                    await
                        client.GetAsync($"http://localhost:{consulPort}/v1/health/service/" + serviceName)
                            .ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JArray.Parse(body);

                return res.Select(entry => new ServiceInformation(entry["Service"]["Address"].Value<string>(),
                    entry["Service"]["Port"].Value<int>())).ToArray();
            }
        }

        public async Task<string[]> GetCriticalServicesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var response =
                    await
                        client.GetAsync($"http://localhost:{consulPort}/v1/health/state/critical").ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not get service health");
                }
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JArray.Parse(body);
                return res.Cast<JObject>().Select(service => service["ServiceID"].Value<string>()).ToArray();
            }
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            using (var client = new HttpClient())
            {
                var response =
                    await
                        client.PutAsync($"http://localhost:{consulPort}/v1/agent/service/deregister/" + serviceId, null)
                            .ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not de register service");
                }
            }
        }

        public async Task KVPutAsync(string key, object value)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(value);
                var content = new StringContent(json);

                var response =
                    await
                        client.PutAsync($"http://localhost:{consulPort}/v1/kv/" + key, content).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not put value");
                }
            }
        }

        public async Task<T> KVGetAsync<T>(string key)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"http://localhost:{consulPort}/v1/kv/" + key).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"There is no value for key \"{key}\"");
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not get value");
                }

                var result = response.Content.ReadAsStringAsync();

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedBody = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(body);
                var bytes = Convert.FromBase64String((string)deserializedBody[0]["Value"]);
                var strValue = Encoding.UTF8.GetString(bytes);

                return JsonConvert.DeserializeObject<T>(strValue);
            }
        }
    }
}