using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microphone.Etcd
{
    public class EtcdOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 2379;
        public int Heartbeat { get; set; } = 1;
        public int TimeToLive { get; set; } = 3;
    }
    public class EtcdProvider : IClusterProvider
    {
        private readonly int _ectdHeartbeart;
        private readonly int _ectdTtl;
        private readonly string _etcdHost;
        private readonly int _etcdPort;
        private string _serviceId;
        private string _serviceName;
        private readonly ILogger _log;
        private readonly IHealthCheck _healthCheck;

        public EtcdProvider(ILoggerFactory loggerFactory, IOptions<EtcdOptions> configuration,IHealthCheck healthCheck = null)
        {
            _log = loggerFactory.CreateLogger("Microphone.EtcdProvider");
            var etcdHost = configuration.Value.Host;
            var etcdPort = configuration.Value.Port;
            var etcdTtl = configuration.Value.TimeToLive;
            var etcdHeartbeat = configuration.Value.Heartbeat;

            _etcdHost = etcdHost;
            _etcdPort = etcdPort;
            _ectdTtl = etcdTtl;
            _ectdHeartbeart = etcdHeartbeat;
            _healthCheck = healthCheck;
        }

        private string RootUrl => $"http://{_etcdHost}:{_etcdPort}";

        public async Task<ServiceInformation[]> GetServiceInstancesAsync(string serviceName)
        {
            var url = ServiceUrl(serviceName);
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }
                _log.LogInformation($"{_serviceName} lookup {serviceName}");
                var body = await response.Content.ReadAsStringAsync();
                var res = JObject.Parse(body);
                var nodes = res["node"]["nodes"];
                return (from node in nodes
                    let uriStr = node["value"].Value<string>()
                    where !string.IsNullOrEmpty(uriStr)
                    let uri = new Uri(uriStr)
                    select new ServiceInformation(uri.AbsoluteUri, uri.Port))
                    .ToArray();
            }
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;

            Func<Task> registerService = async () =>
            {
                var url = RegisterServiceUrl(serviceName, serviceId);
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("value", uri.ToString()),
                        new KeyValuePair<string, string>("ttl", _ectdTtl.ToString())
                    });
                    await client.PutAsync(url, content);
                }
            };

            await registerService();

            StartHeartbeat(registerService);
        }

        public Task BootstrapClientAsync()
        {
            return Task.FromResult(0);
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            var url = KeyValueUrl(key);
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("value", value)
                });
                await client.PutAsync(url, content);
            }
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            var url = KeyValueUrl(key);
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }

        private string ServiceUrl(string serviceName) => $"{RootUrl}{serviceName}";

        private string RegisterServiceUrl(string serviceName, string serviceId) => $"{ServiceUrl(serviceName)}/{serviceId}";

        private string KeyValueUrl(string key) => $"{RootUrl}/v2/keys/microphone/values/{key}";

        private void StartHeartbeat(Func<Task> registerService)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try{
                        if (_healthCheck != null)
                        {
                            await _healthCheck.CheckHealth();
                        }
                        await registerService();
                        _log.LogInformation("OK");
                    }
                    catch{

                    }

                    await Task.Delay(TimeSpan.FromSeconds(_ectdHeartbeart));

                }
            });
        }
    }
}
