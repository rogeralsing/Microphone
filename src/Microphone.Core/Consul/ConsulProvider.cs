using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microphone.Consul
{
    public class ConsulProvider : IClusterProvider
    {
        private readonly string _consulHealthCheckPath;
        private readonly string _consulHost;
        private readonly int _consulPort;
        private readonly int _heartbeat;
        private readonly ILogger _log;
        private readonly ConsulNameResolution _nameResolution;

        public ConsulProvider(ILoggerFactory loggerFactory, IOptions<ConsulOptions> configuration)
        {
            _log = loggerFactory.CreateLogger("Microphone.ConsulProvider");
            _consulHost = configuration.Value.Host;
            _consulPort = configuration.Value.Port;
            _consulHealthCheckPath = configuration.Value.HealthCheckPath;
            _nameResolution = configuration.Value.NameResolution;
            _heartbeat = configuration.Value.Heartbeat;
        }

        private string RootUrl => $"http://{_consulHost}:{_consulPort}";
        private string RegisterServiceUrl => $"{RootUrl}/v1/agent/service/register";

        public async Task<ServiceInformation[]> GetServiceInstancesAsync(string serviceName, params string[] tags)
        {
            if (_nameResolution == ConsulNameResolution.EbayFabio)
            {
                return new[] {new ServiceInformation($"http://{_consulHost}", 9999)};
            }

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(ServiceHealthUrl(serviceName));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }

                var body = await response.Content.ReadAsStringAsync();
                var res1 = JArray.Parse(body);
                var res =
                    res1.
                        Where(entry =>
                        {
                            if (tags.Length == 0)
                                return true;

                            var arr = entry["Service"]["Tags"].Select(i => i.Value<string>()).ToArray();
                            return tags.All(arr.Contains);
                        }).
                        Select(
                            entry =>
                                new ServiceInformation(
                                    entry["Service"]["Address"].Value<string>(),
                                    entry["Service"]["Port"].Value<int>())
                        ).ToArray();

                return res;
            }
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri,
            params string[] tags)
        {
            var port = uri.Port;
            var host = uri.Host;
            var check = $"http://{host}:{port}{_consulHealthCheckPath}";
            var t = new List<string>(tags);
            if (_nameResolution == ConsulNameResolution.EbayFabio)
            {
                t.Add($"urlprefix-/{serviceName}");
            }

            _log.LogInformation($"Using Consul at {_consulHost}:{_consulPort}");
            _log.LogInformation($"Registering service {serviceId} at http://{host}:{port}");
            _log.LogInformation($"Registering health check at http://{host}:{port}/status");

            var payload = new
            {
                ID = serviceId,
                Name = serviceName,
                Tags = t,
                Address = host,
                Port = port,
                Check = new
                {
                    HTTP = check,
                    Interval = $"{_heartbeat}s"
                }
            };

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json);

                var res = await client.PostAsync(RegisterServiceUrl, content);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not register service");
                }
                _log.LogInformation("Registration successful");
            }
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(value);

                var response = await client.PutAsync(KeyValueUrl(key), content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not put value");
                }
            }
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(KeyValueUrl(key));

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"There is no value for key '{key}'");
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Could not get value for key '{key}'");
                }

                var body = await response.Content.ReadAsStringAsync();
                var deserializedBody = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(body);
                var bytes = Convert.FromBase64String((string) deserializedBody[0]["Value"]);
                var strValue = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                return strValue;
            }
        }

        private string KeyValueUrl(string key) => $"{RootUrl}/v1/kv/{key}";
        private string ServiceHealthUrl(string service) => $"{RootUrl}/v1/health/service/{service}?passing";
    }
}