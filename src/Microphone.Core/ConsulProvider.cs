using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microphone.Core.Util;
using Microsoft.Extensions.Configuration;

namespace Microphone.Consul
{
    public class ConsulProvider : IClusterProvider
    {
        private readonly string _consulHost;
        private readonly int _consulPort;
        private readonly bool _useEbayFabio;
        private string _serviceId;
        private string _serviceName;
        private Uri _uri;
        private string _version;
        private ILogger _log;

        public ConsulProvider(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            var consulHost = configuration["ConsulHost"] ?? "localhost";
            var consulPort = int.Parse(configuration["ConsulPort"] ?? "8500");
            var consulFabio = bool.Parse(configuration["ConsulFabio"] ?? "false");

            _log = loggerFactory.CreateLogger("Microphone.ConsulProvider");
            _consulHost = consulHost;
            _consulPort = consulPort;
            _useEbayFabio = consulFabio;
        }

        private string RootUrl => $"http://{_consulHost}:{_consulPort}";
        private string CriticalServicesUrl => $"{RootUrl}/v1/health/state/critical";
        private string RegisterServiceUrl => $"{RootUrl}/v1/agent/service/register";

        public async Task<ServiceInformation[]> FindServiceInstancesAsync(string name)
        {
            if (_useEbayFabio)
            {
                return new[] { new ServiceInformation($"http://{_consulHost}", 9999) };
            }


            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(ServiceHealthUrl(name));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }

                var body = await response.Content.ReadAsStringAsync();
                var res1 = JArray.Parse(body);
                var res =
                    res1.Select(
                        entry =>
                            new ServiceInformation(
                                entry["Service"]["Address"].Value<string>(),
                                entry["Service"]["Port"].Value<int>())
                        ).ToArray();

                return res;
            }
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;
            _version = version;
            _uri = uri;
            var port = uri.Port;

            var localIp = DnsUtils.GetLocalIPAddress();
            var check = $"http://{localIp}:{port}/status";

            _log.LogInformation($"Using Consul at {_consulHost}:{_consulPort}");
            _log.LogInformation($"Registering service {serviceId} at http://{localIp}:{port}");
            _log.LogInformation($"Registering health check at http://{localIp}:{port}/status");
            var payload = new
            {
                ID = serviceId,
                Name = serviceName,
                Tags = new[] { $"urlprefix-/{serviceName}" },
                Address = localIp,
                // ReSharper disable once RedundantAnonymousTypePropertyName
                Port = uri.Port,
                Check = new
                {
                    HTTP = check,
                    Interval = "1s"
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
                _log.LogInformation($"Registration successful");
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
                    throw new Exception($"There is no value for key \"{key}\"");
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not get value");
                }

                var body = await response.Content.ReadAsStringAsync();
                var deserializedBody = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(body);
                var bytes = Convert.FromBase64String((string)deserializedBody[0]["Value"]);
                var strValue = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                return strValue;
            }
        }

        private string KeyValueUrl(string key) => RootUrl + "/v1/kv/" + key;
        private string ServiceHealthUrl(string service) => RootUrl + "/v1/health/service/" + service;
        private string DeregisterServiceUrl(string service) => RootUrl + "/v1/agent/service/deregister/" + service;
    }
}